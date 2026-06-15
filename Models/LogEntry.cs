namespace CloudFileSyncBotAPI.Models
{
    public class LogEntry
    {
        public string FilePath { get; set; }          // Full path of the file
        public string ChangeType { get; set; }        // Created, Modified, Deleted
        public DateTime TimeStamp { get; set; }       // Time when event happened
        public string FolderSource { get; set; }      // Folder being monitored
    }
}
