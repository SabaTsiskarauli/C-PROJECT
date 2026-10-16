using System;
using System.IO;

namespace FitZone.Infrastructure
{
    public static class FileLogger
    {
        private static readonly object _lock = new object();

        private static readonly string _logPath = Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "..",
                "..",
                "..",
                "..",
                "FitZone.Infrastructure",
                "Logs",
                "fitzone.log"));

        public static void Info(string message)
        {
            Write("INFO", message, null);
        }

        public static void Error(string message, Exception ex = null)
        {
            Write("ERROR", message, ex);
        }

        private static void Write(
            string level,
            string message,
            Exception ex)
        {
            try
            {
                lock (_lock)
                {
                    var directory =
                        Path.GetDirectoryName(_logPath);

                    if (!string.IsNullOrEmpty(directory))
                        Directory.CreateDirectory(directory);

                    string line =
                        $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} " +
                        $"[{level}] {message}";

                    if (ex != null)
                    {
                        line += Environment.NewLine +
                                $"    {ex.GetType().Name}: {ex.Message}";
                    }

                    File.AppendAllText(
                        _logPath,
                        line + Environment.NewLine);
                }
            }
            catch
            {
                // Logging must never crash the application.
            }
        }
    }
}
