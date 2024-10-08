using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeInspector.Models
{
    public struct LogEntry
    {
        public string Text { get; }
        public DateTime TimeStamp { get; }
        public LogLevel Level { get; }

        public LogEntry(string text, DateTime timestamp, LogLevel level) => (Text, TimeStamp, Level) = (text, timestamp, level);
        public LogEntry(string text) : this(text, DateTime.Now, LogLevel.Info) { }
    }

    public enum LogLevel
    {
        Info,
        Warn,
        Error,
        Success
    }
}
