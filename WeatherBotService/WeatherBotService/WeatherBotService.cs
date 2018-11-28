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
        private static readonly LogBuilder Log = new LogBuilder(@"C:\WayfarersWeather\log.txt");
        private DateTime TimeOfLastWeatherChange { get; set; }
        private DateTime TimeOfLastToken { get; set; }
        private static RedditUpdateEngine _updateEngine;

        public WeatherBotService()
        {
            InitializeComponent();

            _timer = new Timer();

            using (var file = File.OpenText("settings.txt"))
            {
                using (var reader = new JsonTextReader(file))
                {
                    var settings = (JObject) JToken.ReadFrom(reader);

                    _updateEngine = new RedditUpdateEngine(
                        (string) settings["user"], (string) settings["password"],
                        (string) settings["refresh_token"], (string) settings["thread_uri"]);
                }
            }

            TimeOfLastWeatherChange = DateTime.MinValue;
            TimeOfLastToken = DateTime.MinValue;
        }

        protected override void OnStart(string[] args)
        {
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

            if (DateTime.UtcNow.Hour - TimeOfLastToken.Hour >= 1)
            {
                Log.Write("Refreshing bearer token.");
                _updateEngine.GetNewAccessToken();
            }

            if (DateTime.UtcNow.Hour - TimeOfLastWeatherChange.Hour >= 8)
            {
                Log.Write("Weather Updated");

                TimeOfLastWeatherChange = DateTime.UtcNow;

                var weather = new WeatherGenerator();

                var updateString = BuildPostString(weather.Effect);

                _updateEngine.PostUpdate(updateString);
            }
            else
            {
                // Pull current status and update time only.
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
