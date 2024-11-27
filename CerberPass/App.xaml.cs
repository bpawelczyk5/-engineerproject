using CerberPass.Services;
using System;
using System.IO;
using System.Windows;

namespace CerberPass
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // Usuń tymczasowy plik bazy danych, jeśli istnieje
            string tempDbPath = Session.Instance.DatabasePath;

            if (!string.IsNullOrEmpty(tempDbPath) && File.Exists(tempDbPath))
            {
                try
                {
                    File.Delete(tempDbPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Nie udało się usunąć tymczasowego pliku: {ex.Message}");
                }
            }
        }
    }
}
