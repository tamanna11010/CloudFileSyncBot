# Cloud File Sync Bot

A real-time file monitoring and cloud synchronization system built using C#, ASP.NET Core, and Google Drive API.

The application monitors one or more local folders for file creation, modification, and deletion events. Detected changes are displayed through an interactive dashboard and automatically synchronized with Google Drive for cloud backup.

## Features

* Real-time file monitoring using FileSystemWatcher
* Automatic Google Drive synchronization
* Multi-folder monitoring support
* Interactive dashboard for viewing file activity
* Search and filter logs by folder
* Export logs to CSV
* Clear log history
* Upload status tracking
* Pause and Resume monitoring
* Retry mechanism for locked files
* Temporary file filtering (`~`, `~$`, `.tmp`)
* ASP.NET Core Web API integration

## Technologies Used

* C#
* ASP.NET Core Web API
* .NET 6
* Google Drive API
* OAuth 2.0 Authentication
* HTML
* CSS
* JavaScript
* FileSystemWatcher

## System Architecture

```text
User Interface
      ↓
ASP.NET Core Web API
      ↓
FileWatcherService
      ↓
Log Manager
      ↓
Google Drive Integration
```

## Project Structure

```text
CloudFileSyncBotAPI
│
├── Controllers
│   └── SyncController.cs
│
├── Models
│   └── LogEntry.cs
│
├── Services
│   ├── FileWatcherService.cs
│   ├── DriveUploader.cs
│   └── LogManager.cs
│
├── Utils
├── wwwroot
│   └── dashboard
│
├── Program.cs
├── appsettings.json
└── README.md
```

## How It Works

1. User selects one or more folders to monitor.
2. FileSystemWatcher continuously tracks file activities.
3. Events such as Create, Modify, and Delete are detected.
4. Logs are generated and displayed on the dashboard.
5. Changed files are automatically uploaded to Google Drive.
6. Upload status is updated in real time.

## Setup Instructions

### Prerequisites

* Visual Studio 2022
* .NET 6 SDK or later
* Google Cloud Project
* Google Drive API enabled
* OAuth Credentials

### Installation

1. Clone the repository

```bash
git clone https://github.com/YOUR_USERNAME/Cloud-File-Sync-Bot.git
```

2. Open the solution in Visual Studio 2022

3. Add your own Google API credentials file:

```text
credentials.json
```

4. Build the solution

```bash
dotnet build
```

5. Run the application

```bash
dotnet run
```

6. Authenticate with your Google account when prompted.

## Documentation

📄 [Project Documentation](Cloud_File_Sync_Bot_Documentation.pdf)

## Future Enhancements

* Two-way cloud synchronization
* Dropbox integration
* OneDrive integration
* AWS S3 support
* Persistent database logging
* Email notifications
* Role-based authentication
* Cross-platform support (Linux/macOS)
* Analytics dashboard

## Screenshots

* Folder Monitoring Dashboard
* File Activity Logs
* Google Drive Synchronization
* Upload Status Tracking

## Author

**Tamanna Batra**

Bachelor of Engineering (Computer Science and Engineering)

Chandigarh University

## License

This project is developed for educational and academic purposes.
