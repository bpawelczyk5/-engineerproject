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

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                selectedDatabasePath = openFileDialog.FileName;
                databaseNameTextBlock.Text = Path.GetFileName(selectedDatabasePath);
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedDatabasePath))
            {
                MessageBox.Show("Proszę wybierz najpierw bazę danych.");
                return;
            }

            string password = loginPassword.Password;

            // Validate credentials and decrypt database
            if (ValidateAndDecryptDatabase(selectedDatabasePath, password))
            {
                // Store session information
                Session.Instance.DatabasePath = temporaryDecryptedPath;
                Session.Instance.UserName = "ZalogowanyUzytkownik"; // Możesz pobrać tę wartość z bazy danych

                MessageBox.Show("Logowanie udane!");

                // Open MainWindow
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                // Zamknij LoginWindow
                Window loginWindow = Window.GetWindow(this);
                loginWindow.Close();
            }
            else
            {
                MessageBox.Show("Błędne dane. Spróbuj jeszcze raz!");
            }
        }

        private bool ValidateAndDecryptDatabase(string encryptedDbPath, string password)
        {
            try
            {
                // Tymczasowa ścieżka do odszyfrowanej bazy danych
                temporaryDecryptedPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");

                // Odszyfruj bazę danych
                DecryptDatabase(encryptedDbPath, temporaryDecryptedPath, password);

                // Sprawdź poprawność hasła
                using (SQLiteConnection conn = new SQLiteConnection($"Data Source={temporaryDecryptedPath};Version=3;"))
                {
                    conn.Open();

                    string query = "SELECT password FROM password LIMIT 1";
                    SQLiteCommand command = new SQLiteCommand(query, conn);

                    string hashedPassword = command.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(hashedPassword))
                    {
                        // Weryfikacja hasła przy użyciu bcrypt
                        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error podczas odszyfrowywania bazy danych: {ex.Message}");
            }

            return false;
        }

        private void DecryptDatabase(string encryptedFilePath, string decryptedFilePath, string password)
        {
            byte[] key, iv;
            byte[] salt = new byte[16]; // Zakładamy, że sól ma 16 bajtów

            using (var fileStream = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Read))
            {
                // Odczytaj sól z pliku
                fileStream.Read(salt, 0, salt.Length);

                using (var keyGenerator = new Rfc2898DeriveBytes(password, salt))
                {
                    key = keyGenerator.GetBytes(32); // Klucz AES (256 bitów)
                    iv = keyGenerator.GetBytes(16); // IV AES (128 bitów)
                }

                // Odszyfruj dane
                byte[] encryptedData = new byte[fileStream.Length - salt.Length];
                fileStream.Read(encryptedData, 0, encryptedData.Length);

                using (var decryptor = Aes.Create().CreateDecryptor(key, iv))
                using (var cryptoStream = new CryptoStream(new MemoryStream(encryptedData), decryptor, CryptoStreamMode.Read))
                using (var outputStream = new FileStream(decryptedFilePath, FileMode.Create, FileAccess.Write))
                {
                    cryptoStream.CopyTo(outputStream);
                }
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = (LoginWindow)Window.GetWindow(this);
            loginWindow.LoginContent.Content = new Register();
        }
    }
}
