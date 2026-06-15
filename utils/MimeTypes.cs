using System.Collections.Generic;
using System.IO;

namespace CloudFileSyncBotAPI.Utils
{
    public static class MimeTypes
    {
        private static readonly Dictionary<string, string> _types = new()
        {
            { ".txt", "text/plain" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".pdf", "application/pdf" },
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
        };

        public static string GetMimeType(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            return _types.TryGetValue(ext, out var mime) ? mime : "application/octet-stream";
        }
    }
}
