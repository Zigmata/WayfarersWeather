﻿using System;
using System.IO;
using System.ServiceProcess;
using System.Timers;
using Timer = System.Timers.Timer;

namespace WeatherBotService
{
    public partial class WeatherBotService : ServiceBase
    {
        private readonly Timer _timer;
        private const string LogFile = @"C:\WayfarersWeather\log.txt";
        public DateTime TimeOfLastWeatherChange { get; set; }
        private static readonly RedditUpdateEngine UpdateEngine = new RedditUpdateEngine();

        public WeatherBotService()
        {
            InitializeComponent();

            new FileInfo(LogFile).Directory?.Create();
            _timer = new Timer();
        }

        protected override void OnStart(string[] args)
        {
            Log("Service started.");
            _timer.Enabled = true;
            _timer.Interval = 60 * 1000; // Triggers after one minute
            _timer.Elapsed += OnElapsedTime;
        }

        private void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            Log("Timer Elapsed");

            // Reset timer
            if ((int)_timer.Interval != 60 * 1000)
                _timer.Interval = 60 * 1000;

            if ((DateTime.UtcNow.Hour - TimeOfLastWeatherChange.Hour) >= 8)
            {
                Log("Weather Updated");

                TimeOfLastWeatherChange = DateTime.UtcNow;

                var weather = new WeatherPattern();

                // Some more stuff with building the update.

                UpdateEngine.PostUpdate(weather, DateTime.UtcNow);
            }
        }

        protected override void OnStop()
        {
            Log("Service stopped.");
        }

        private static void Log(string logMessage)
        {
            using (TextWriter w = File.AppendText(LogFile))
            {
                w.Write($"\r\n{DateTime.UtcNow} : {logMessage}");
            }
        }
    }
}
