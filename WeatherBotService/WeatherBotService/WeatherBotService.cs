using Newtonsoft.Json;
using System;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Newtonsoft.Json.Linq;
using Timer = System.Timers.Timer;

namespace WeatherBotService
{
    public partial class WeatherBotService : ServiceBase
    {
        private readonly Timer _timer;
        private static readonly LogBuilder Log = new LogBuilder(@"C:\WayfarersWeather\Logs\serviceLog.txt");
        private DateTime _timeOfLastWeatherChange;
        private DateTime _timeOfLastToken;
        private static RedditUpdateEngine _updateEngine;
        private WeatherGenerator _currentWeather;

        public WeatherBotService()
        {
            InitializeComponent();

            // Initialize the timer
            _timer = new Timer();

            // Read JSON from settings file and instantiate the RedditUpdateEngine
            using (var file = File.OpenText(@"C:\WayfarersWeather\settings.txt"))
            {
                using (var reader = new JsonTextReader(file))
                {
                    var settings = (JObject) JToken.ReadFrom(reader);

                    _updateEngine = new RedditUpdateEngine(
                        (string) settings["user"], (string) settings["password"],
                        (string) settings["refresh_token"], (string) settings["thread_uri"]);
                }
            }

            // Initialize the time fields to their minimums.
            _timeOfLastWeatherChange = DateTime.MinValue;
            _timeOfLastToken = DateTime.MinValue;
        }

        protected override void OnStart(string[] args)
        {
            System.Diagnostics.Debugger.Launch();

            Log.Write("Service started.");
            _timer.Enabled = true;
            _timer.Interval = 60 * 1000; // Triggers after one minute
            _timer.Elapsed += OnElapsedTime;
        }

        private void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            // Reset timer
            if ((int)_timer.Interval != 60 * 1000)
                _timer.Interval = 60 * 1000;

            // If the last token was acquired an hour or more ago, get a new token.
            if (DateTime.UtcNow.Hour - _timeOfLastToken.Hour >= 1)
            {
                Log.Write("Refreshing bearer token.");
                _timeOfLastToken = DateTime.UtcNow;

                try
                {
                    _updateEngine.GetNewAccessToken();
                }
                catch (Exception exception)
                {
                    Log.Write($"Error acquiring access token: {exception.Message}");
                }
            }

            // If the weather hasn't been changed in four hours, generate a new pattern and store it in _currentWeather.
            if (DateTime.UtcNow.Hour - _timeOfLastWeatherChange.Hour >= 4)
            {
                Log.Write("Weather Updated");
                _timeOfLastWeatherChange = DateTime.UtcNow;

                _currentWeather = new WeatherGenerator();
            }

            // Log the current time, and post an update with _currentWeather.
            var updateString = BuildPostString(_currentWeather.Effect);

            try
            {
                _updateEngine.PostUpdate(updateString);
            }
            catch (Exception exception)
            {
                Log.Write($"An error occurred while posting the update: {exception.Message}");
            }
        }

        protected override void OnStop()
        {
            Log.Write("Service stopped.");
        }

        private static string BuildPostString(string weatherPattern)
        {
            var date = DateTime.UtcNow.ToString("f");
            var postContent = new StringBuilder();
            postContent.AppendFormat($"*{date}*\r\n");
            postContent.AppendFormat("\r\n");
            postContent.AppendFormat(weatherPattern);

            return postContent.ToString();
        }
    }
}
