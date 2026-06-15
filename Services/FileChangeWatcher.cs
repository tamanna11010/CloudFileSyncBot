using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CloudFileSyncBotAPI.Models;

namespace CloudFileSyncBotAPI.Services
{
    public class FileWatcherService
    {
        private readonly LogManager _logManager;
        private readonly DriveUploader _driveUploader;
        private readonly List<FileSystemWatcher> _watchers = new();
        private bool _isPaused = false;

        public FileWatcherService(LogManager logManager, DriveUploader driveUploader)
        {
            _logManager = logManager;
            _driveUploader = driveUploader;
        }

        public void StartWatching(List<string> folderPaths)
        {
            foreach (var path in folderPaths)
            {
                var watcher = new FileSystemWatcher(path)
                {
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true,
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
                };

                watcher.Created += async (s, e) => await HandleEvent(e, "Created", path);
                watcher.Changed += async (s, e) => await HandleEvent(e, "Modified", path);
                watcher.Deleted += async (s, e) => await HandleEvent(e, "Deleted", path);

                _watchers.Add(watcher);
            }
        }

        private async Task HandleEvent(FileSystemEventArgs e, string changeType, string sourceFolder)
        {
            if (_isPaused || string.IsNullOrEmpty(e.FullPath))
                return;

            string fileName = Path.GetFileName(e.FullPath);
            if (fileName.StartsWith("~") || fileName.StartsWith("~$") || fileName.EndsWith(".tmp"))
                return;

            var log = new LogEntry
            {
                FilePath = e.FullPath,
                ChangeType = changeType,
                TimeStamp = DateTime.Now,
                FolderSource = sourceFolder
            };

            _logManager.AddLog(log);

            if (changeType != "Deleted")
                await _driveUploader.UploadFileAsync(e.FullPath);
        }

        public void PauseMonitoring() => _isPaused = true;
        public void ResumeMonitoring() => _isPaused = false;
    }
}
