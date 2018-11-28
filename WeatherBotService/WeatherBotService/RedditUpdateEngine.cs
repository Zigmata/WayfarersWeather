using System;
using System.Security.Authentication;
using Newtonsoft.Json.Linq;
using RedditSharp;
using RestSharp;
using RestSharp.Authenticators;

namespace WeatherBotService
{
    public class RedditUpdateEngine
    {
        private string _bearerToken;
        private readonly string _refreshToken;
        private readonly string _userName;
        private readonly string _password;
        private static readonly RestClient Client = new RestClient();
        private static Uri _threadUri;

        /// <summary>
        /// Default constructor. Acquires new bearer token on instantiation.
        /// </summary>
        /// <param name="userName">Reddit bot ID.</param>
        /// <param name="password">Reddit bot secret.</param>
        /// <param name="refreshToken">Refresh token for authentication.</param>
        /// <param name="threadUri">URL of the thread to be edited.</param>
        public RedditUpdateEngine(string userName, string password, string refreshToken, string threadUri)
        {
            _refreshToken = refreshToken;
            _userName = userName;
            _password = password;
            _threadUri = new Uri(threadUri);

            // Repeat access token request until valid token is gained.
            while (!GetNewAccessToken())
                GetNewAccessToken();
        }

        /// <summary>
        /// Edits the thread designated in the "thread_uri" field from settings.txt.
        /// </summary>
        /// <param name="content">New text to post.</param>
        internal bool PostUpdate(string content)
        {
            // Init Reddit class with bearer token.
            var reddit = new Reddit(_bearerToken);

            // Verify user is authenticated and valid.
            if (reddit.User == null)
                throw new AuthenticationException("User not authenticated.");

            // Grab the post from settings.txt, and update the content.
            var post = reddit.GetPost(_threadUri);
            post.EditText(content);

            return true;
        }

        /// <summary>
        /// Gets new bearer token and stores it in <c>_bearerToken</c>, using the "refresh_token" field from settings.txt.
        /// </summary>
        /// <returns>Returns <c>true</c> when the token is successfully refreshed.</returns>
        internal bool GetNewAccessToken()
        {
            // Configure Uri for new token and authentication header
            Client.BaseUrl = new Uri("https://www.reddit.com/api/v1/access_token");
            Client.Authenticator = new HttpBasicAuthenticator(_userName, _password);

            // Create new POST method and set string for request body
            var request = new RestRequest(Method.POST);
            var body = $"grant_type=refresh_token&refresh_token={_refreshToken}";

            // Add relevant headers and set the body parameter
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("undefined", body, ParameterType.RequestBody);

            // Execute the request
            var response = Client.Execute(request);

            // Report if token request fails.
            if (!response.IsSuccessful)
                return false;

            // Parse JSON response
            var responseObject = JObject.Parse(response.Content);

            // Set the bearer token from the returned JSON string
            _bearerToken = (string) responseObject["access_token"];

            return true;
        }
    }
}