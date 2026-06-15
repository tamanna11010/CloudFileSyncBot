using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CloudFileSyncBotAPI.Models;
using CloudFileSyncBotAPI.Utils;

public class DriveUploader
{
    private readonly DriveService _driveService;
    private readonly string _driveFolderId = "root";
    private CloudFileSyncBotAPI.Models.UploadStatus _lastUploadStatus;

    public DriveUploader()
    {
        _driveService = InitializeService();
    }

    private DriveService InitializeService()
    {
        using var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read);
        string credPath = "token.json";

        var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.Load(stream).Secrets,
            new[] { DriveService.Scope.DriveFile },
            "user", CancellationToken.None,
            new FileDataStore(credPath, true)).Result;

        return new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Cloud File Sync Bot"
        });
    }

    public async Task UploadFileAsync(string localPath)
    {
        string fileName = Path.GetFileName(localPath);

        if (fileName.StartsWith("~") || fileName.StartsWith("~$") || fileName.EndsWith(".tmp"))
        {
            Console.WriteLine($"[Skipped] {fileName} is a temp/system file.");
            return;
        }

        try
        {
            await UploadToDrive(localPath);
        }
        catch (IOException ex) when (IsFileLocked(ex))
        {
            Console.WriteLine($"[Retry] {fileName} is locked. Retrying in 3 seconds...");
            await Task.Delay(3000);
            try
            {
                await UploadToDrive(localPath);
            }
            catch (IOException retryEx)
            {
                Console.WriteLine($"[Retry Failed] {retryEx.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Upload Error] {ex.Message}");
        }
    }

    private async Task UploadToDrive(string filePath)
    {
        var fileMetadata = new Google.Apis.Drive.v3.Data.File
        {
            Name = Path.GetFileName(filePath),
            Parents = new List<string> { _driveFolderId }
        };

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var request = _driveService.Files.Create(fileMetadata, stream, MimeTypes.GetMimeType(filePath));
        request.Fields = "id";
        await request.UploadAsync();

        _lastUploadStatus = new CloudFileSyncBotAPI.Models.UploadStatus
        {
            File = filePath,
            Time = DateTime.Now
        };

        Console.WriteLine($"[Uploaded] {filePath} → Drive at {DateTime.Now}");
    }

    private bool IsFileLocked(IOException ex)
    {
        return ex.Message.Contains("being used by another process");
    }

    public CloudFileSyncBotAPI.Models.UploadStatus GetLastUploadStatus() => _lastUploadStatus;

}
