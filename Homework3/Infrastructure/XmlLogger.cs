using Homework3.Interfaces;
using System.IO;
using System.Xml.Linq;

namespace Homework3.Infrastructure
{
    public class XmlLogger : ILogger
    {
        private readonly string _filePath;

        public XmlLogger(string filePath = "log.xml")
        {
            _filePath = filePath;
        }

        public void Log(string message)
        {
            XElement logEntry = new XElement("LogEntry",
                new XElement("Timestamp", DateTime.Now),
                new XElement("Message", message)
            );

            if (File.Exists(_filePath))
            {
                var doc = XDocument.Load(_filePath);
                doc.Root.Add(logEntry);
                doc.Save(_filePath);
            }
            else
            {
                var doc = new XDocument(new XElement("Logs", logEntry));
                doc.Save(_filePath);
            }
        }
    }
}
