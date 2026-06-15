using System.Collections.Generic;
using System.Linq;
using CloudFileSyncBotAPI.Models;

namespace CloudFileSyncBotAPI.Services
{
    public class LogManager
    {
        private readonly List<LogEntry> _logs = new();

        public void AddLog(LogEntry log) => _logs.Add(log);
        public List<LogEntry> GetLogs() => _logs.ToList();
        public void ClearLogs() => _logs.Clear();
    }
}
