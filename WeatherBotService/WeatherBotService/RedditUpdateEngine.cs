using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using RedditSharp;
using RedditSharp.Things;

namespace WeatherBotService
{
    public class RedditUpdateEngine
    {
        private string _bearerToken;
        private readonly string _refreshToken;
        private static readonly HttpClient Client = new HttpClient();
        private readonly string _encodedAccountCredentials;

        public WeatherPattern CurrentWeather { get; set; }
        public DateTime CurrentDate { get; set; }
        public DateTime CurrentTime { get; set; }
        public Season CurrentSeason { get; set; }
        public Phase CurrentPhase { get; set; }

        public RedditUpdateEngine(string accountCredentials, string refreshToken)
        {
            _refreshToken = refreshToken;
            _encodedAccountCredentials = Base64Encode(accountCredentials);
        }

        internal void PostUpdate(WeatherPattern weather, DateTime dateTime)
        {

        }

        internal void GetThreadContent()
        {

        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        internal async void GetNewAccessToken()
        {
            var values = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"refresh_token", _refreshToken}
            };

            Client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Basic", Base64Encode(_encodedAccountCredentials));
            var content = new FormUrlEncodedContent(values);

            var response = await Client.PostAsync("https://www.reddit.com/api/v1/access_token", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var convertedString = JsonConvert.SerializeObject(responseString);


            var reddit = new Reddit();
        }
    }
}