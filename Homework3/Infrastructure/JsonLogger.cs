using Homework3.Interfaces;
using System.IO;
using System.Text.Json;

namespace Homework3.Infrastructure
{
    public class JsonLogger : ILogger
    {
        private readonly string _filePath;

        public JsonLogger(string filePath = "log.json")
        {
            _filePath = filePath;
        }

        public void Log(string message)
        {
            var logEntry = new
            {
                Timestamp = DateTime.Now,
                Message = message
            };

            string json = JsonSerializer.Serialize(logEntry, new JsonSerializerOptions { WriteIndented = true });
            File.AppendAllText(_filePath, json + Environment.NewLine);
        }
    }
}
