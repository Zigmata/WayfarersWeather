using System;
using System.IO;

namespace WeatherBotService
{
    public class LogBuilder
    {
        private readonly string _logFile;

        public LogBuilder(string path)
        {
            _logFile = path;

            new FileInfo(path).Directory?.Create();
        }

        public void Write(string logMessage)
        {
            using (TextWriter w = File.AppendText(_logFile))
            {
                w.Write($"\r\n{DateTime.UtcNow} : {logMessage}");
            }
        }
    }
}