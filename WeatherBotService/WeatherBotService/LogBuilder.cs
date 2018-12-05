using System;
using System.IO;

namespace WeatherBotService
{
    /// <summary>
    /// Base class for building log files.
    /// </summary>
    public sealed class LogBuilder
    {
        /// <summary>Path to the log file.</summary>
        private readonly string _logFile;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="path">String path of the log file.</param>
        public LogBuilder(string path)
        {
            _logFile = path;

            new FileInfo(path).Directory?.Create(); // If directory does not exist, create it.
        }

        /// <summary>
        /// Appends a string to the log.
        /// </summary>
        /// <param name="logMessage">String message to append.</param>
        public void Write(string logMessage)
        {
            using (TextWriter w = File.AppendText(_logFile))
            {
                w.Write($"\r\n{DateTime.UtcNow} : {logMessage}");
            }
        }
    }
}