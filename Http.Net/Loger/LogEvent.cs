using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Http.Net.Loger
{
    public class LogEvent
    {
        public Guid Id => Guid.NewGuid();
        public LogLevel LogLevel { get; private set; }
        public string Message { get; private set; }
        public DateTime Time => DateTime.Now;

        public LogEvent()
        {

        }

        public LogEvent(LogLevel logLevel, string message)
        {
            LogLevel = logLevel;
            Message = message;
        }
    }
}
