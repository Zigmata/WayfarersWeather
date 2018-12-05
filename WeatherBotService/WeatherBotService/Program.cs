﻿using System.ServiceProcess;

namespace WeatherBotService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new WeatherBotService()
            };
            ServiceBase.Run(servicesToRun);
        }
    }
}
