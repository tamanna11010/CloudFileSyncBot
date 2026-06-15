using Microsoft.AspNetCore.Mvc;
using CloudFileSyncBotAPI.Services;
using CloudFileSyncBotAPI.Models;
using System.Collections.Generic;

namespace CloudFileSyncBotAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly FileWatcherService _fileWatcherService;
        private readonly LogManager _logManager;
        private readonly DriveUploader _driveUploader;

        public SyncController(FileWatcherService fileWatcherService, LogManager logManager, DriveUploader uploader)
        {
            _fileWatcherService = fileWatcherService;
            _logManager = logManager;
            _driveUploader = uploader;
        }

        [HttpPost("start")]
        public IActionResult StartMonitoring([FromBody] List<string> folderPaths)
        {
            if (folderPaths == null || folderPaths.Count == 0)
                return BadRequest("No folders provided.");

            _fileWatcherService.StartWatching(folderPaths);
            return Ok("Monitoring started.");
        }

        [HttpGet("logs")]
        public ActionResult<List<LogEntry>> GetLogs()
        {
            return Ok(_logManager.GetLogs());
        }

        [HttpPost("clear")]
        public IActionResult ClearLogs()
        {
            _logManager.ClearLogs();
            return Ok("Logs cleared.");
        }

        [HttpPost("pause")]
        public IActionResult Pause()
        {
            _fileWatcherService.PauseMonitoring();
            return Ok("Monitoring paused.");
        }

        [HttpPost("resume")]
        public IActionResult Resume()
        {
            _fileWatcherService.ResumeMonitoring();
            return Ok("Monitoring resumed.");
        }

        [HttpGet("uploadstatus")]
        public ActionResult<CloudFileSyncBotAPI.Models.UploadStatus> UploadStatus()
        {
            return Ok(_driveUploader.GetLastUploadStatus());
        }
    }
}
