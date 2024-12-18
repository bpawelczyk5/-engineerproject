using System;
using System.IO;
using System.Data.SQLite;

namespace CerberPass.Services
{
    public static class Utilities
    {
        public static void LogDebug(string message)
        {
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "debug_log.txt");
            File.AppendAllText(logPath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
        public static void ForceCloseSQLiteConnections(string dbPath)
        {
            SQLiteConnection.ClearAllPools();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            LogDebug($"Zamknięto wszystkie połączenia SQLite dla bazy: {dbPath}");
        }
    }
}