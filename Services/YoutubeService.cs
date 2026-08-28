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
namespace Services;

public class YoutubeService : IYoutubeService
{
    readonly YoutubeDbRepo _repo;
    readonly IConfiguration _configuration;
    readonly Encryptions _encryptions;
    private readonly IMemoryCache _cache;
    readonly ILogger<IYoutubeService> _logger;

    readonly string _clientId;
    readonly string _clientSecret;
    readonly string _redirectUri;
    readonly string _scopes;
    public YoutubeService(YoutubeDbRepo repo, IConfiguration configuration, Encryptions encryptions, IMemoryCache cache, ILogger<IYoutubeService> logger)
    {
        _repo = repo;
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

    public async Task<ServiceResult<string>> Callback(string code, string state)
    {
        try
        {
            var cacheKey = $"oauth-state:{state}";

            if (!_cache.TryGetValue(cacheKey, out Guid organizationId))
            {
                return ServiceResult<string>.Fail("This authorization request is invalid or has expired. Please try connecting again.");
            }

            // one-time use — remove immediately so the same state value can't be replayed
            _cache.Remove(cacheKey);

            var tokenResponse = await HandleCallback(code);

            if (tokenResponse == null)
                return ServiceResult<string>.Fail("Failed to receive a response from Google.");
            if (tokenResponse.IsStale)
                return ServiceResult<string>.Fail("The authorization request has expired.");
            if (string.IsNullOrEmpty(tokenResponse.AccessToken))
                return ServiceResult<string>.Fail("No access token was returned.");
            if (string.IsNullOrEmpty(tokenResponse.RefreshToken))
                return ServiceResult<string>.Fail("No refresh token was returned.");

            var credential = GoogleCredential.FromAccessToken(tokenResponse.AccessToken);
            var youtube = new GoogleYouTubeService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "AllMedia"
            });

            var channelRequest = youtube.Channels.List("snippet");
            channelRequest.Mine = true;
            var channelResponse = await channelRequest.ExecuteAsync();

            var channel = channelResponse.Items?.FirstOrDefault();
            if (channel is null)
            {
                return ServiceResult<string>.Fail("No YouTube channel was found for this account. Please ensure that the account has an associated YouTube channel.");
            }

            var username = channel.Snippet.Title;

            var result = await _repo.SaveSocialAccountAsync(new SocialAccountDbM
            {
                Platform = "YouTube",
                Username = username,
                AccessToken = _encryptions.AesEncryptToBase64(tokenResponse.AccessToken),
                RefreshToken = _encryptions.AesEncryptToBase64(tokenResponse.RefreshToken),
                TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresInSeconds ?? 0),
                OrganizationId = organizationId
            });

            if (result.Contains("Failed"))
            {
                return ServiceResult<string>.Fail(result);
            }

            return ServiceResult<string>.Ok("YouTube account connected successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResult<string>.Fail($"Failed to connect YouTube account: {ex.Message}");
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

    public async Task<ServiceResult<string>> UploadVideoAsync(IFormFile video, string title, string description, string categoryId, ISocialAccount account)
    {
        // Kollar om access token är giltig, annars refreshar den
        var tokenResult = await GetAccessTokenAsync(account);

        //om tokenResult inte är success, returnera fail med error
        if (!tokenResult.Success)
        {
            return ServiceResult<string>.Fail(tokenResult.Error!);
        }

        var accessToken = tokenResult.Message; //the token is the message here

        if (string.IsNullOrEmpty(accessToken))
        {
            return ServiceResult<string>.Fail("Access token is missing.");
        }

        //save video to db

        // YouTube upload goes here
        var credential = GoogleCredential.FromAccessToken(accessToken);
        var youtube = new GoogleYouTubeService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "AllMedia"
        });
        var youtubeVideo = new Video
        {
            Snippet = new VideoSnippet
            {
                Title = title,
                Description = description,
                CategoryId = categoryId
            },

            Status = new VideoStatus
            {
                PrivacyStatus = "private"
            }
        };

        await using var stream = video.OpenReadStream();

        var upload = youtube.Videos.Insert(youtubeVideo, "snippet,status", stream, video.ContentType);

        var uploadResult = await upload.UploadAsync();

        return ServiceResult<string>.Ok("Video uploaded successfully.");
    }

    public async Task<ServiceResult<string>> GetAccessTokenAsync(ISocialAccount account)
    {
        //borde inte kunna vara null, men kollar ändå
        if (account == null)
        {
            return ServiceResult<string>.Fail("Social account is null.");
        }

        // om giltig hoppar över den här delen och returnerar access token
        if (!IsAccessTokenValid(account))
        {
            // Refresh token
            var refreshResult = await RefreshTokenAsync(account);

            if (!refreshResult.Success)
            {
                return ServiceResult<string>.Fail(refreshResult.Error!);
            }
            //hämtar den nya access token från databasen, eftersom den kan ha uppdaterats annars får man den gamla access token som inte är giltig
            var socialAccount = await _repo.GetSocialAccountByIdAsync(account.Id);

            //borde inte kunna vara null, men kollar ändå
            if (socialAccount == null)
            {
                return ServiceResult<string>.Fail("Social account not found.");
            }
            //returnerar den nya access token som message
            return ServiceResult<string>.Ok(socialAccount.AccessToken);
        }

        return ServiceResult<string>.Ok(account.AccessToken);
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

            var newToken = await flow.RefreshTokenAsync(
                "user",
                account.RefreshToken,
                CancellationToken.None);

            if (string.IsNullOrEmpty(newToken.AccessToken))
            {
                return ServiceResult<string>.Fail("Google did not return a new access token.");
            }

            account.AccessToken = newToken.AccessToken;

            account.TokenExpiresAt = DateTime.UtcNow.AddSeconds(newToken.ExpiresInSeconds ?? 3600);

            await _repo.UpdateSocialAccountAsync(account.Id, account);

            return ServiceResult<string>.Ok("YouTube access token refreshed successfully.", newToken.AccessToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh YouTube access token.");

            return ServiceResult<string>.Fail($"Failed to refresh YouTube access token: {ex.Message}");
        }
    }
    private bool IsAccessTokenValid(ISocialAccount account)
    {
        return !string.IsNullOrEmpty(account.AccessToken)
            && account.TokenExpiresAt > DateTime.UtcNow.AddMinutes(5);
    }
}
