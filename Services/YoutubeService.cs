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

            return ServiceResult.Ok(
                        "YouTube authorization URL created.", url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating YouTube authorization URL.");
            return ServiceResult.Fail(
                $"Failed to generate YouTube authorization URL: {ex.Message}");
        }
    }

    public async Task<ServiceResult> Callback(string code)
    {
        try
        {
            var tokenResponse = await HandleCallback(code);

            if (tokenResponse == null)
            {
                return ServiceResult.Fail(
                    "Failed to receive a response from Google.");
            }

            if (tokenResponse.IsStale)
            {
                return ServiceResult.Fail(
                    "The authorization request has expired.");
            }

            if (string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                return ServiceResult.Fail(
                    "No access token was returned.");
            }

            if (string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                return ServiceResult.Fail(
                    "No refresh token was returned.");
            }

            //get username 
            var credential = GoogleCredential.FromAccessToken(tokenResponse.AccessToken);
            var youtube = new GoogleYouTubeService(
                new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "YourSaaS"
                });

            var channelRequest = youtube.Channels.List("snippet");

            channelRequest.Mine = true;

            var channelResponse =
                await channelRequest.ExecuteAsync();

            var channel = channelResponse.Items.FirstOrDefault();

            if (channel == null)
            {
                return ServiceResult.Fail(
                    "No YouTube channel was found for this account.");
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

            return ServiceResult.Ok(
                "YouTube account connected successfully.");
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail(
                $"Failed to connect YouTube account: {ex.Message}");
        }
    }
    public async Task<TokenResponse> HandleCallback(string code)
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
}
