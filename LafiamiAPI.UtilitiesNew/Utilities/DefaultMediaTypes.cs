using System;
using System.IO;

namespace LafiamiAPI.Utilities.Utilities
{
    public static class DefaultMediaTypes
    {
        public static string GetContentType(string FilePath)
        {
            Uri uri = new Uri(FilePath);
            string Extension = Path.GetExtension(uri.AbsolutePath).ToLower();

            switch (Extension)
            {
                case ".pdf":
                    return "application/pdf";
                case ".txt":
                    return "text/plain";
                case ".bmp":
                    return "image/bmp";
                case ".gif":
                    return "image/gif";
                case ".png":
                    return "image/png";
                case ".jpg":
                    return "image/jpeg";
                case ".jpeg":
                    return "image/jpeg";
                case ".xls":
                    return "application/vnd.ms-excel";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".csv":
                    return "text/csv";
                case ".html":
                    return "text/html";
                case ".xml":
                    return "text/xml";
                case ".zip":
                    return "application/zip";
                case ".ico":
                    return "image/x-icon";
                default:
                    return "application/octet-stream";

            }

        }

        public static string GetFilename(string FilePath)
        {
            Uri uri = new Uri(FilePath);
            return Path.GetFileName(uri.AbsolutePath).ToLower();
        }
    }
}
