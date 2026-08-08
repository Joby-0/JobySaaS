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
namespace Services;

public class YoutubeService : IYoutubeService
{
    readonly YoutubeDbRepo _repo;
    readonly IConfiguration _configuration;
    readonly ILogger<IYoutubeService> _logger;

    readonly string _clientId;
    readonly string _clientSecret;
    readonly string _redirectUri;
    readonly string _scopes;
    public YoutubeService(YoutubeDbRepo repo, IConfiguration configuration, ILogger<IYoutubeService> logger)
    {
        _repo = repo;
        _configuration = configuration;
        _logger = logger;
        _clientId = _configuration["GoogleOAuth:ClientId"];
        _clientSecret = _configuration["GoogleOAuth:ClientSecret"];
        _redirectUri = _configuration["GoogleOAuth:RedirectUri"];
        _scopes = _configuration["GoogleOAuth:Scopes"];
    }

    public async Task<ServiceResult> Connect()
    {
        try
        {
            if (string.IsNullOrEmpty(_clientId))
                return ServiceResult.Fail("Google Client ID is missing.");

            if (string.IsNullOrEmpty(_redirectUri))
                return ServiceResult.Fail("Google redirect URI is missing.");

            if (string.IsNullOrEmpty(_scopes))
                return ServiceResult.Fail("Google OAuth scope is missing.");

            var url = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={_clientId}&redirect_uri={_redirectUri}&scope={_scopes}&response_type=code&access_type=offline&prompt=consent";

            return ServiceResult.Ok("YouTube authorization URL created.", url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating YouTube authorization URL.");
            return ServiceResult.Fail($"Failed to generate YouTube authorization URL: {ex.Message}");
        }
    }

    public async Task<ServiceResult> Callback(string code)
    {
        try
        {
            var tokenResponse = await HandleCallback(code);

            if (tokenResponse == null)
            {
                return ServiceResult.Fail("Failed to receive a response from Google.");
            }

            if (tokenResponse.IsStale)
            {
                return ServiceResult.Fail("The authorization request has expired.");
            }

            if (string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                return ServiceResult.Fail("No access token was returned.");
            }

            if (string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                return ServiceResult.Fail("No refresh token was returned.");
            }

            //get username 
            var credential = GoogleCredential.FromAccessToken(tokenResponse.AccessToken);
            var youtube = new GoogleYouTubeService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "AllMedia"
                });

            var channelRequest = youtube.Channels.List("snippet");

            channelRequest.Mine = true;

            var channelResponse = await channelRequest.ExecuteAsync();

            if (channelResponse.Items == null || !channelResponse.Items.Any())
            {
                return ServiceResult.Fail("No YouTube channel was found for this account. Please ensure that the account has an associated YouTube channel.");
            }

            var channel = channelResponse.Items.FirstOrDefault();
            
            //borde inte kunna vara null, men kollar ändå eftersom man kollar innan om listan med kanaler är tom
            if (channel == null)
            {
                return ServiceResult.Fail("No YouTube channel was found for this account. Please ensure that the account has an associated YouTube channel.");
            }

            var channelId = channel.Id;
            var username = channel.Snippet.Title;
            // var profilePictureUrl = channel.Snippet.Thumbnails.High?.Url;


            // Save SocialAccount here
            await _repo.SaveSocialAccountAsync(new SocialAccountDbM
            {
                Platform = "YouTube",
                Username = username,
                // ProfilePictureUrl = profilePictureUrl, vill man ha det kanske
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                TokenExpires = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresInSeconds ?? 0),
                OrganizationId = Guid.NewGuid() // Replace with actual organization ID in a real implementation.
            });

            return ServiceResult.Ok("YouTube account connected successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail($"Failed to connect YouTube account: {ex.Message}");
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
            "user",
            code,
            _redirectUri,
            CancellationToken.None);

        return token;
    }

    public async Task<ServiceResult> UploadVideoAsync(IFormFile video, string title, string description, string categoryId, ISocialAccount account)
    {
        // Kollar om access token är giltig, annars refreshar den
        var tokenResult = await GetAccessTokenAsync(account);

        //om tokenResult inte är success, returnera fail med error
        if (!tokenResult.Success)
        {
            return ServiceResult.Fail(tokenResult.Error!);
        }

        var accessToken = tokenResult.Message; //the token is the message here

        if (string.IsNullOrEmpty(accessToken))
        {
            return ServiceResult.Fail("Access token is missing.");
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

        return ServiceResult.Ok("Video uploaded successfully.");
    }

    public async Task<ServiceResult> GetAccessTokenAsync(ISocialAccount account)
    {
        //borde inte kunna vara null, men kollar ändå
        if (account == null)
        {
            return ServiceResult.Fail("Social account is null.");
        }

        // om giltig hoppar över den här delen och returnerar access token
        if (!IsAccessTokenValid(account))
        {
            // Refresh token
            var refreshResult = await RefreshTokenAsync(account);

            if (!refreshResult.Success)
            {
                return ServiceResult.Fail(refreshResult.Error!);
            }
            //hämtar den nya access token från databasen, eftersom den kan ha uppdaterats annars får man den gamla access token som inte är giltig
            var socialAccount = await _repo.GetSocialAccountByIdAsync(account.Id);

            //borde inte kunna vara null, men kollar ändå
            if (socialAccount == null)
            {
                return ServiceResult.Fail("Social account not found.");
            }
            //returnerar den nya access token som message
            return ServiceResult.Ok(socialAccount.AccessToken);
        }

        return ServiceResult.Ok(account.AccessToken);
    }
    public async Task<ServiceResult> RefreshTokenAsync(ISocialAccount account)
    {
        try
        {
            if (string.IsNullOrEmpty(account.RefreshToken))
            {
                return ServiceResult.Fail("No refresh token is available.");
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
                return ServiceResult.Fail("Google did not return a new access token.");
            }

            account.AccessToken = newToken.AccessToken;

            account.TokenExpires = DateTime.UtcNow.AddSeconds(newToken.ExpiresInSeconds ?? 3600);

            await _repo.UpdateSocialAccountAsync(account.Id, account);

            return ServiceResult.Ok("YouTube access token refreshed successfully.", newToken.AccessToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh YouTube access token.");

            return ServiceResult.Fail($"Failed to refresh YouTube access token: {ex.Message}");
        }
    }
    private bool IsAccessTokenValid(ISocialAccount account)
    {
        return !string.IsNullOrEmpty(account.AccessToken)
            && account.TokenExpires > DateTime.UtcNow.AddMinutes(5);
    }
}
