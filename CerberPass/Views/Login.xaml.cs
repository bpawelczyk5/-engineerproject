using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using BCrypt.Net;
using CerberPass.Services;
using CerberPass.Windows;
using Microsoft.Win32;
using System.Threading.Tasks; // Dodano dla async

namespace CerberPass.Views
{
    public partial class Login : UserControl
    {
        private string selectedDatabasePath;
        private string temporaryDecryptedPath;

        public Login()
        {
            InitializeComponent();
        }

        private void SelectDatabase_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                selectedDatabasePath = openFileDialog.FileName;
                databaseNameTextBlock.Text = Path.GetFileName(selectedDatabasePath);
                LogDebug($"Wybrano bazę danych: {selectedDatabasePath}");
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedDatabasePath))
            {
                MessageBox.Show("Proszę wybierz najpierw bazę danych.");
                LogDebug("Nie wybrano bazy danych podczas logowania.");
                return;
            }

            string password = loginPassword.Password;

            if (ValidateAndDecryptDatabase(selectedDatabasePath, password))
            {
                Session.Instance.DatabasePath = temporaryDecryptedPath;
                Session.Instance.UserName = "ZalogowanyUzytkownik";
                Session.Instance.OriginalDatabasePath = selectedDatabasePath;
                Session.Instance.UserPassword = password;

                MessageBox.Show("Logowanie udane!");
                LogDebug("Logowanie powiodło się. Przekierowanie do głównego okna aplikacji.");

                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                Window loginWindow = Window.GetWindow(this);
                loginWindow.Close();
            }
            else
            {
                MessageBox.Show("Błędne dane. Spróbuj jeszcze raz!");
                LogDebug("Logowanie nieudane: błędne hasło lub problem z odszyfrowaniem bazy danych.");
            }
        }


        private bool ValidateAndDecryptDatabase(string encryptedDbPath, string password)
        {
            SQLiteConnection conn = null;

            try
            {
                temporaryDecryptedPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");
                LogDebug($"Rozpoczęto odszyfrowywanie bazy danych: {encryptedDbPath} do {temporaryDecryptedPath}");

                DecryptDatabase(encryptedDbPath, temporaryDecryptedPath, password);

                conn = new SQLiteConnection($"Data Source={temporaryDecryptedPath};Version=3;");
                conn.Open();
                LogDebug("Otwarto odszyfrowaną bazę danych w celu weryfikacji hasła.");

                string query = "SELECT password FROM password LIMIT 1";
                SQLiteCommand command = new SQLiteCommand(query, conn);
                string hashedPassword = command.ExecuteScalar()?.ToString();

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                LogDebug($"Weryfikacja hasła zakończona: {isPasswordValid}");

                return isPasswordValid;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error podczas odszyfrowywania bazy danych: {ex.Message}");
                LogDebug($"Błąd odszyfrowywania bazy danych: {ex.Message}");
                return false;
            }
            finally
            {
                conn?.Close();
                conn?.Dispose();
                ForceCloseSQLiteConnections(temporaryDecryptedPath);
            }
        }

        private void DecryptDatabase(string encryptedDbPath, string temporaryDecryptedPath, string password)
        {
            byte[] key, iv;
            byte[] salt = GetSaltFromEncryptedDb(encryptedDbPath);

            using (var keyGenerator = new Rfc2898DeriveBytes(password, salt))
            {
                key = keyGenerator.GetBytes(32);
                iv = keyGenerator.GetBytes(16);
            }

            try
            {
                using (var encryptedFileStream = new FileStream(encryptedDbPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    encryptedFileStream.Seek(16, SeekOrigin.Begin);

                    using (var decryptedFileStream = new FileStream(temporaryDecryptedPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (var decryptor = Aes.Create().CreateDecryptor(key, iv))
                        using (var cryptoStream = new CryptoStream(decryptedFileStream, decryptor, CryptoStreamMode.Write))
                        {
                            encryptedFileStream.CopyTo(cryptoStream);
                        }
                    }
                }
                LogDebug($"Baza danych odszyfrowana: {temporaryDecryptedPath}");
            }
            catch (Exception ex)
            {
                LogDebug($"Błąd podczas odszyfrowywania bazy danych: {ex.Message}");
                throw;
            }
        }
        public byte[] GetSaltFromEncryptedDb(string encryptedDbPath)
        {
            byte[] salt = new byte[16];
            try
            {
                using (var fileStream = new FileStream(encryptedDbPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fileStream.Read(salt, 0, 16); // Odczytaj salt z początku pliku
                }
            }
            catch (Exception ex)
            {
                LogDebug($"Błąd podczas pobierania salt z bazy danych: {ex.Message}");
                throw;
            }
            return salt;
        }

        //Usunięte odwołania do nieistniejących kontrolek
        /* private void AddRecord_Click(object sender, RoutedEventArgs e)
         {
             string name = nameTextBox.Text;
             string username = usernameTextBox.Text;
             string url = urlTextBox.Text;

             if (!string.IsNullOrEmpty(temporaryDecryptedPath))
             {
                 using (SQLiteConnection conn = new SQLiteConnection($"Data Source={temporaryDecryptedPath};Version=3;"))
                 {
                     conn.Open();

                     string query = "INSERT INTO your_table_name (Name, Username, URL) VALUES (@name, @username, @url)";
                     SQLiteCommand command = new SQLiteCommand(query, conn);
                     command.Parameters.AddWithValue("@name", name);
                     command.Parameters.AddWithValue("@username", username);
                     command.Parameters.AddWithValue("@url", url);

                     int rowsAffected = command.ExecuteNonQuery();
                      LogDebug($"Dodano nowy rekord: Name={name}, Username={username}, URL={url}, RowsAffected={rowsAffected}");
                 }
             }
             else
             {
                 MessageBox.Show("Tymczasowa baza danych jest niedostępna.");
             }
         }*/

        public void CleanTemporaryDatabase()
        {
            if (!string.IsNullOrEmpty(temporaryDecryptedPath) && File.Exists(temporaryDecryptedPath))
            {
                try
                {
                    ForceCloseSQLiteConnections(temporaryDecryptedPath);

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    File.Delete(temporaryDecryptedPath);
                    LogDebug("Tymczasowa baza danych została usunięta.");
                }
                catch (IOException ex)
                {
                    LogDebug($"Nie udało się usunąć tymczasowej bazy danych: {ex.Message}");
                }
            }
        }

        private void ForceCloseSQLiteConnections(string dbPath)
        {
            SQLiteConnection.ClearAllPools();
            LogDebug($"Zamknięto wszystkie połączenia SQLite dla bazy: {dbPath}");
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = (LoginWindow)Window.GetWindow(this);
            loginWindow.LoginContent.Content = new Register();
            LogDebug("Przekierowano do okna rejestracji.");
        }

        public void LogDebug(string message)
        {
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "debug_log.txt");
            File.AppendAllText(logPath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
    }
}