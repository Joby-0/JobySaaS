using DbModels;
using DbRepos;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2;
using Models;
using Microsoft.Extensions.Logging;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using GoogleYouTubeService = Google.Apis.YouTube.v3.YouTubeService;
using Microsoft.AspNetCore.Http;
using Google.Apis.YouTube.v3.Data;
using Configuration;
using Microsoft.Extensions.Caching.Memory;
using Models.DTO;
namespace Services;

public class YoutubeService : IYoutubeService
{
    readonly SocialAccountDbRepo _repo;
    readonly MediaDbRepo _mediaRepo;
    readonly OrganizationDbRepo _organizationRepo;
    readonly IConfiguration _configuration;
    readonly Encryptions _encryptions;
    private readonly IMemoryCache _cache;
    readonly ILogger<IYoutubeService> _logger;

    readonly string _clientId;
    readonly string _clientSecret;
    readonly string _redirectUri;
    readonly string _scopes;
    public YoutubeService(SocialAccountDbRepo repo, MediaDbRepo mediaRepo, OrganizationDbRepo organizationRepo, IConfiguration configuration, Encryptions encryptions, IMemoryCache cache, ILogger<IYoutubeService> logger)
    {
        _repo = repo;
        _mediaRepo = mediaRepo;
        _organizationRepo = organizationRepo;
        _configuration = configuration;
        _logger = logger;
        _encryptions = encryptions;
        _cache = cache;
        _clientId = _configuration["GoogleOAuth:ClientId"];
        _clientSecret = _configuration["GoogleOAuth:ClientSecret"];
        _redirectUri = _configuration["GoogleOAuth:RedirectUri"];
        _scopes = _configuration["GoogleOAuth:Scopes"];
    }

    public async Task<ServiceResult<string>> Connect(Guid organizationId)
    {
        try
        {
            if (string.IsNullOrEmpty(_clientId))
                return ServiceResult<string>.Fail("Google Client ID is missing.");
            if (string.IsNullOrEmpty(_redirectUri))
                return ServiceResult<string>.Fail("Google redirect URI is missing.");
            if (string.IsNullOrEmpty(_scopes))
                return ServiceResult<string>.Fail("Google OAuth scope is missing.");

            // random, unguessable token — this is what actually goes in the URL, not the org ID itself
            var csrfToken = Guid.NewGuid().ToString("N");

            _cache.Set(
                $"oauth-state:{csrfToken}",
                organizationId,
                TimeSpan.FromMinutes(10)); // matches how long you'd reasonably expect someone to sit on Google's consent screen

            var url = "https://accounts.google.com/o/oauth2/v2/auth" +
                $"?client_id={Uri.EscapeDataString(_clientId)}" +
                $"&redirect_uri={Uri.EscapeDataString(_redirectUri)}" +
                $"&scope={Uri.EscapeDataString(_scopes)}" +
                $"&response_type=code" +
                $"&access_type=offline" +
                $"&prompt=consent" +
                $"&state={Uri.EscapeDataString(csrfToken)}";

            return ServiceResult<string>.Ok("YouTube authorization URL created.", url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating YouTube authorization URL.");
            return ServiceResult<string>.Fail($"Failed to generate YouTube authorization URL: {ex.Message}");
        }
    }

    public async Task<ServiceResult<Guid>> Callback(string code, string state)
    {
        try
        {
            var cacheKey = $"oauth-state:{state}";

            if (!_cache.TryGetValue(cacheKey, out Guid organizationId))
            {
                return ServiceResult<Guid>.Fail("This authorization request is invalid or has expired. Please try connecting again.");
            }

            // one-time use — remove immediately so the same state value can't be replayed
            _cache.Remove(cacheKey);

            var tokenResponse = await HandleCallback(code);

            if (tokenResponse == null)
                return ServiceResult<Guid>.Fail("Failed to receive a response from Google.");
            if (tokenResponse.IsStale)
                return ServiceResult<Guid>.Fail("The authorization request has expired.");
            if (string.IsNullOrEmpty(tokenResponse.AccessToken))
                return ServiceResult<Guid>.Fail("No access token was returned.");
            if (string.IsNullOrEmpty(tokenResponse.RefreshToken))
                return ServiceResult<Guid>.Fail("No refresh token was returned.");

            // This is the one place we build a client from a "raw", not-yet-stored token
            // (there's no SocialAccount row to hang it off yet), so it doesn't go through
            // GetYoutubeClientAsync.
            var credential = GoogleCredential.FromAccessToken(tokenResponse.AccessToken);
            var youtube = new GoogleYouTubeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "AllMedia"
            });

            var channelRequest = youtube.Channels.List("snippet,statistics");
            channelRequest.Mine = true;
            var channelResponse = await channelRequest.ExecuteAsync();

            var channel = channelResponse.Items?.FirstOrDefault();
            if (channel is null)
            {
                return ServiceResult<Guid>.Fail("No YouTube channel was found for this account. Please ensure that the account has an associated YouTube channel.");
            }

            var username = channel.Snippet.Title;

            var result = await _repo.SaveSocialAccountAsync(new SocialAccountDbM
            {
                Platform = SocialPlatform.YouTube,

                Username = username,
                CostumUrl = channel.Snippet.CustomUrl,
                ProfileImageUrl = channel.Snippet.Thumbnails.Default__.Url,
                AccessToken = _encryptions.AesEncryptToBase64(tokenResponse.AccessToken),
                RefreshToken = _encryptions.AesEncryptToBase64(tokenResponse.RefreshToken),
                TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresInSeconds ?? 0),
                CreatedAt = DateTime.UtcNow,
                LastSync = DateTime.UtcNow,
                Status = SocialAccountStatus.Connected,
                OrganizationId = organizationId,
                Followers = channel.Statistics.SubscriberCount ?? 0
            });

            if (result.Contains("Failed"))
            {
                return ServiceResult<Guid>.Fail(result);
            }

            return ServiceResult<Guid>.Ok("YouTube account connected successfully.", organizationId);
        }
        catch (Exception ex)
        {
            return ServiceResult<Guid>.Fail($"Failed to connect YouTube account: {ex.Message}");
        }
    }

    private async Task<TokenResponse> HandleCallback(string code)
    {
        // In a real implementation, this would exchange the 'code' for an access token.
        // Then save it to the database via the repo.


        var flow = new GoogleAuthorizationCodeFlow(
            new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                },
                Scopes = _scopes.Split(' ')
            });

        var token = await flow.ExchangeCodeForTokenAsync(
            "user", // maybe change this if using flow.LoadTokenAsync(...) somethime
            code,
            _redirectUri,
            CancellationToken.None);

        return token;
    }

    private async Task<ServiceResult<GoogleYouTubeService>> GetYoutubeClientAsync(ISocialAccount account)
    {
        if (!IsAccessTokenValid(account))
        {
            var refreshResult = await RefreshTokenAsync(account);
            if (!refreshResult.Success)
                return ServiceResult<GoogleYouTubeService>.Fail(refreshResult.Error!);

        }

        string accessToken;
        try
        {
            accessToken = _encryptions.AesDecryptFromBase64<string>(account.AccessToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to decrypt YouTube access token for social account {AccountId}.", account.Id);
            return ServiceResult<GoogleYouTubeService>.Fail("Failed to read the stored access token.");
        }

        if (string.IsNullOrEmpty(accessToken))
            return ServiceResult<GoogleYouTubeService>.Fail("Access token is missing.");

        var credential = GoogleCredential.FromAccessToken(accessToken);
        var youtube = new GoogleYouTubeService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "AllMedia"
        });

        return ServiceResult<GoogleYouTubeService>.Ok("",youtube);
    }
    public async Task<ServiceResult<string>> UploadVideoAsync(Guid mediaId, string title, string description, string categoryId, Guid accountId, Guid requestUserId)
    {
        var account = await _repo.GetSocialAccountByIdAsync(accountId);
        if (account is null || account.Platform != SocialPlatform.YouTube)
            return ServiceResult<string>.Fail("The selected YouTube account could not be found.");

        var membership = await _organizationRepo.GetUserOrganizationAsync(account.OrganizationId, requestUserId);
        if (membership is null)
            return ServiceResult<string>.Fail("You do not have access to this organization.");

        var media = await _mediaRepo.GetByIdAsync(account.OrganizationId, mediaId);
        if (media is null)
            return ServiceResult<string>.Fail("Media could not be found.");
        if (media.FileContent is null || media.FileContent.Length == 0)
            return ServiceResult<string>.Fail("The selected media has no stored file content.");

        // Ready-to-use client for this account — refreshes the token behind the scenes if needed.
        var clientResult = await GetYoutubeClientAsync(account);
        if (!clientResult.Success)
            return ServiceResult<string>.Fail(clientResult.Error!);

        var youtube = clientResult.Data;

        var youtubeVideo = new Video
        {
            Snippet = new VideoSnippet
            {
                Title = string.IsNullOrWhiteSpace(title) ? media.Title : title,
                Description = string.IsNullOrWhiteSpace(description) ? media.Description : description,
                CategoryId = categoryId
            },

            Status = new VideoStatus
            {
                PrivacyStatus = "private"
            }
        };

        await using var stream = new MemoryStream(media.FileContent, writable: false);

        var upload = youtube.Videos.Insert(youtubeVideo, "snippet,status", stream, media.MimeType ?? "video/mp4");

        var uploadResult = await upload.UploadAsync();

        //save video to db
        if (uploadResult.Status != Google.Apis.Upload.UploadStatus.Completed || upload.ResponseBody is null)
            return ServiceResult<string>.Fail(uploadResult.Exception?.Message ?? "YouTube upload failed.");

        var videoId = upload.ResponseBody.Id;

        await _repo.UploadVideoAsync(new SocialVideoDbM
        {
            Id = Guid.NewGuid(),
            VideoId = videoId,
            MediaId = media.Id,
            Platform = SocialPlatform.YouTube,
            Status = uploadResult.Status == Google.Apis.Upload.UploadStatus.Completed ? VideoUploadStatus.Completed : VideoUploadStatus.Failed,
            CreatedAt = DateTime.UtcNow,
            // ProcessingPercentage = 
            FailureReason = uploadResult.Exception?.Message
        });

        return ServiceResult<string>.Ok("Video uploaded successfully.", videoId);
    }
    public async Task<ServiceResult<string>> GetAccessTokenAsync(Guid accountId)
    {
        var account = await _repo.GetSocialAccountByIdAsync(accountId);
        if (account == null)
        {
            return ServiceResult<string>.Fail("Social account not found.");
        }

        // If the token is still valid, just decrypt and hand it back.
        if (IsAccessTokenValid(account))
        {
            return ServiceResult<string>.Ok("Access token is valid.", _encryptions.AesDecryptFromBase64<string>(account.AccessToken));
        }

        // Otherwise refresh it. RefreshTokenAsync updates `account` in place (encrypted)
        // and persists it, so we can decrypt straight from it afterwards — no need to
        // re-fetch from the repo.
        var refreshResult = await RefreshTokenAsync(account);

        if (!refreshResult.Success)
        {
            return ServiceResult<string>.Fail(refreshResult.Error!);
        }

        return ServiceResult<string>.Ok("Access token refreshed.", _encryptions.AesDecryptFromBase64<string>(account.AccessToken));
    }
    public async Task<ServiceResult<string>> RefreshTokenAsync(ISocialAccount account)
    {
        try
        {
            if (string.IsNullOrEmpty(account.RefreshToken))
            {
                return ServiceResult<string>.Fail("No refresh token is available.");
            }

            var flow = new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = _clientId,
                        ClientSecret = _clientSecret
                    },
                    Scopes = _scopes.Split(' ')
                });

            var refreshTokenDecrypted = _encryptions.AesDecryptFromBase64<string>(account.RefreshToken);

            var newToken = await flow.RefreshTokenAsync("user", refreshTokenDecrypted, CancellationToken.None);

            if (string.IsNullOrEmpty(newToken.AccessToken))
            {
                return ServiceResult<string>.Fail("Google did not return a new access token.");
            }
           
            account.AccessToken = _encryptions.AesEncryptToBase64(newToken.AccessToken);

            account.TokenExpiresAt = DateTime.UtcNow.AddSeconds(newToken.ExpiresInSeconds ?? 3600);

            
            var refreshToken = string.IsNullOrEmpty(newToken.RefreshToken)
                ? account.RefreshToken
                : _encryptions.AesEncryptToBase64(newToken.RefreshToken);

            account.RefreshToken = refreshToken;

            await _repo.UpdateSocialAccountAsync(
                account.Id,
                new UpdateSocialAccountDto
                {
                    AccessToken = account.AccessToken,
                    TokenExpiresAt = account.TokenExpiresAt,
                    LastSync = DateTime.UtcNow,
                    Status = SocialAccountStatus.Connected,
                    RefreshToken = refreshToken
                });

            return ServiceResult<string>.Ok("YouTube access token refreshed successfully.", newToken.AccessToken);
        }
        catch (TokenResponseException ex)
        {
            _logger.LogError(ex,"Failed to refresh YouTube token for social account {AccountId}.", account.Id);

            if (ex.Error?.Error == "invalid_grant")
            {
                return ServiceResult<string>.Fail("The YouTube authorization is no longer valid. The account needs to be reconnected.");
            }

            return ServiceResult<string>.Fail($"Failed to refresh YouTube access token: {ex.Error?.Error}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh YouTube access token for social account {AccountId}.", account.Id);

            return ServiceResult<string>.Fail("Failed to refresh YouTube access token.");
        }
    }
    private bool IsAccessTokenValid(ISocialAccount account)
    {
        return !string.IsNullOrEmpty(account.AccessToken)
            && account.TokenExpiresAt > DateTime.UtcNow.AddMinutes(5);
    }

    public async Task<ServiceResult<SocialAccountDetails>> GetAccountDetailsAsync(SocialAccountDbM account)
    {
        var clientResult = await GetYoutubeClientAsync(account);
        if (!clientResult.Success)
            return ServiceResult<SocialAccountDetails>.Fail(clientResult.Error!);

        var youtube = clientResult.Data;

        var channelRequest = youtube.Channels.List("snippet,statistics");
        channelRequest.Mine = true;
        var channelResponse = await channelRequest.ExecuteAsync();

        var channel = channelResponse.Items?.FirstOrDefault();
        if (channel is null)
            return ServiceResult<SocialAccountDetails>.Fail("No YouTube channel was found for this account.");

        var details = new SocialAccountDetails
        {
            AccountName = channel.Snippet.Title,
            Handle = channel.Snippet.CustomUrl,
            ProfileImageUrl = channel.Snippet.Thumbnails.Default__.Url,
            Followers = channel.Statistics.SubscriberCount ?? 0,
            VideoCount = channel.Statistics.VideoCount ?? 0,
            CommentCount = channel.Statistics.CommentCount ?? 0
        };

        return ServiceResult<SocialAccountDetails>.Ok("YouTube account details retrieved.", details);
    }
}