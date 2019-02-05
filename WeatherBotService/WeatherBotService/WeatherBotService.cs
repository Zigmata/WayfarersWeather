using System;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherBotService
{
    /// <inheritdoc />
    public partial class WeatherBotService : ServiceBase
    {
        /// <summary>Timer for use in triggering updates.</summary>
        private Timer _timer;
        /// <summary><see cref="LogBuilder"/> to use for log entries.</summary>
        private static readonly LogBuilder Log = new LogBuilder(@"C:\WayfarersWeather\Logs\serviceLog.txt");
        /// <summary>Stores <see cref="DateTime"/> of the last weather generation.</summary>
        private DateTime _timeOfLastWeatherChange;
        /// <summary>Stores <see cref="DateTime"/> of the last bearer token generation.</summary>
        private DateTime _timeOfLastToken;
        /// <summary><see cref="RedditUpdateEngine"/> for posting updates and refreshing bearer tokens.</summary>
        private static RedditUpdateEngine _updateEngine;
        /// <summary><see cref="WeatherGenerator"/> used to create update strings.</summary>
        private WeatherGenerator _currentWeather;
        /// <summary>Used to determine which hours the weather will update.</summary>
        private readonly int[] _updateTriggers = {1, 6, 7, 13, 18, 20};

        /// <inheritdoc />
        public WeatherBotService()
        {
            InitializeComponent();
        }

        // Inherited method for actions to take when service is started.
        protected override void OnStart(string[] args)
        {
            // --- Uncomment next line to attach debugger ---
            //System.Diagnostics.Debugger.Launch();

            try
            {
                Initialize();
            }
            catch (Exception e)
            {
                Log.Write(e.Message);
                Log.Write(e.InnerException?.Message);
                Log.Write(e.TargetSite.Name);
                Log.Write(e.StackTrace);
                ExitCode = 1;
                Stop();
                throw;
            }
        }

        // Initializes the class fields and logs the service start time.
        /// <summary>
        /// Initializes the class fields and logs the service start time.
        /// </summary>
        private void Initialize()
        {
            Log.Write("Service started.");

            // Initialize the timer
            _timer = new Timer {Interval = 60 * 1000}; // Triggers after one minute.
            _timer.Elapsed += TimerElapsed;
            _timer.AutoReset = true;
            _timer.Enabled = true;

            // Read JSON from settings file and initialize _updateEngine.
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

            var now = DateTime.UtcNow;

            // Initialize the DateTime fields to their minimums.
            _timeOfLastWeatherChange = now;
            _timeOfLastToken = now;

            // Initialize the bearer token in _updateEngine
            Log.Write("Initializing bearer token.");
            _updateEngine.GetNewAccessToken();

            // Initialize the weather field.
            Log.Write("Initializing weather pattern.");
            _currentWeather = new WeatherGenerator();
            _currentWeather.GenerateWeather(now);

            // Perform initial post.
            var updateString = BuildPostContent(_currentWeather.Effect);
            try
            {
                Log.Write("Initializing thread.");
                _updateEngine.PostUpdate(updateString);
            }
            catch (Exception e)
            {
                Log.Write($"An error occurred while sending the initialization post: {e.Message}");
            }
        }

        // Actions to take when _timer elapses
        /// <summary>
        /// Actions to take when _timer elapses
        /// </summary>
        /// <param name="sender">The object raising this event.</param>
        /// <param name="e">Additional arguments from the raised event.</param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            var now = DateTime.UtcNow;

            // If the last bearer token was acquired an hour or more ago, get a new token.
            if ((int)(now - _timeOfLastToken).TotalHours >= 1)
            {
                Log.Write("Refreshing bearer token.");

                try
                {
                    _updateEngine.GetNewAccessToken();
                }
                catch (Exception exception)
                {
                    Log.Write($"Error acquiring access token: {exception.Message}");
                }

                // Record the time the bearer token was refreshed.
                _timeOfLastToken = now;
            }

            // Check the current time and see if the hour matches any triggers
            // in _updateTriggers. If it does, update the weather.
            var change = false;
            foreach (var i in _updateTriggers)
            {
                if (now.Hour == i && _timeOfLastWeatherChange.Hour != i)
                    change = true;
            }
            if (change)
                UpdateWeather(now);

            // Post an update with _currentWeather.
            var updateString = BuildPostContent(_currentWeather.Effect);
            try
            {
                // Log.Write("Posting update."); // verbose, enable if desired
                _updateEngine.PostUpdate(updateString);
            }
            catch (Exception exception)
            {
                Log.Write($"An error occurred while posting the update: {exception.Message}");
            }
        }

        // Updates the current weather description
        private void UpdateWeather(DateTime time)
        {
            // Dispose the old object.
            _currentWeather.Dispose();

            // Log time.
            Log.Write("Weather Updated");

            // Instantiate a new WeatherGenerator and generate an effect string.
            _currentWeather = new WeatherGenerator();
            _currentWeather.GenerateWeather(time);

            // Record the time the weather was updated.
            _timeOfLastWeatherChange = time;
        }

        // Inherited method for actions to take when the service is stopped.
        protected override void OnStop()
        {
            Log.Write("Service stopped.");
        }

        // Builds the formatted string to post to Reddit.
        /// <summary>
        /// Builds the formatted string to post to Reddit.
        /// </summary>
        /// <param name="weatherPattern">String description of the weather effect to post.</param>
        /// <returns></returns>
        private static string BuildPostContent(string weatherPattern)
        {
            var date = DateTime.UtcNow.ToString("f"); // Formatting for full date with short time.
            var postContent = new StringBuilder();
            postContent.AppendFormat($"**{date}**\r\n"); // Double asterisks format the date in boldface
            postContent.AppendFormat("\r\n"); // Two line returns are needed for Reddit posts to create a new paragraph.
            postContent.AppendFormat($"*{weatherPattern}*"); // Single asterisks format the weather description in italics.

            return postContent.ToString();
        }
    }
}
