using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using BCrypt.Net;
using CerberPass.Windows;

namespace CerberPass.Views
{
    public partial class Register : UserControl
    {
        public Register()
        {
            InitializeComponent();
        }

        public void CreateDatabase_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password != repeatPasswordBox.Password)
            {
                MessageBox.Show("Hasła nie są takie same. Spróbuj ponownie.");
                return;
            }

            string userPassword = passwordBox.Password;
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userPassword);

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string dbName = dbNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(dbName))
            {
                MessageBox.Show("Proszę wprowadź nazwę bazy danych.");
                return;
            }

            string dbPath = Path.Combine(desktopPath, $"{dbName}.db");

            try
            {
                SQLiteConnection.CreateFile(dbPath);
                LogDebug($"Plik bazy danych '{dbPath}' został utworzony.");

                using (SQLiteConnection conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    conn.Open();
                    LogDebug("Połączenie z bazą danych zostało otwarte.");

                    using (var transaction = conn.BeginTransaction())
                    {
                        CreateTables(conn);

                        string insertPasswordQuery = "INSERT INTO password (password) VALUES (@password)";
                        using (SQLiteCommand insertCommand = new SQLiteCommand(insertPasswordQuery, conn))
                        {
                            insertCommand.Parameters.AddWithValue("@password", hashedPassword);
                            insertCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        LogDebug("Hasło zostało zapisane w tabeli password.");
                    }
                }

                EncryptDatabase(dbPath, userPassword);
                MessageBox.Show("Baza danych została utworzona i zaszyfrowana.");

                LoginWindow loginWindow = (LoginWindow)Window.GetWindow(this);
                loginWindow.LoginContent.Content = new Views.Login();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas tworzenia bazy danych: {ex.Message}");
                LogDebug($"Exception: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        public void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = (LoginWindow)Window.GetWindow(this);
            loginWindow.LoginContent.Content = new Views.Login();
        }

        public void CreateTables(SQLiteConnection conn)
        {
            string[] tableCreationQueries = {
                @"CREATE TABLE IF NOT EXISTS main (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT,
                    iv_name TEXT,
                    user_name TEXT,
                    iv_username TEXT,
                    password TEXT,
                    iv TEXT,
                    url TEXT,
                    iv_url TEXT,
                    is_favourite INTEGER DEFAULT 0,
                    is_lastcopied INTEGER DEFAULT 0
                );",
                @"CREATE TABLE IF NOT EXISTS notes (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT,
                    iv_name TEXT,
                    text TEXT,
                    iv TEXT,
                    is_favourite INTEGER DEFAULT 0
                );",
                @"CREATE TABLE IF NOT EXISTS credit_card (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT,
                    iv_name TEXT,
                    card_number TEXT,
                    iv_cardnumber TEXT,
                    date TEXT,
                    iv_date TEXT,
                    cvv TEXT,
                    iv_cvv TEXT,
                    is_favourite INTEGER DEFAULT 0
                );",
                @"CREATE TABLE IF NOT EXISTS contacts (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT,
                    iv_name TEXT,
                    lastname TEXT,
                    iv_lastname TEXT,
                    phone_number TEXT,
                    iv_phonenumber TEXT,
                    email TEXT,
                    iv_email TEXT,
                    is_favourite INTEGER DEFAULT 0
                );",
                @"CREATE TABLE IF NOT EXISTS password (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    password TEXT
                );"
            };

            foreach (var query in tableCreationQueries)
            {
                ExecuteNonQuery(conn, query);
            }

            LogDebug("Wszystkie tabele zostały pomyślnie utworzone.");
        }

        public void ExecuteNonQuery(SQLiteConnection connection, string query)
        {
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void EncryptDatabase(string filePath, string password)
        {
            byte[] key, iv;
            byte[] salt = GenerateSalt(16);

            using (var keyGenerator = new Rfc2898DeriveBytes(password, salt))
            {
                key = keyGenerator.GetBytes(32);
                iv = keyGenerator.GetBytes(16);
            }

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    byte[] fileData = new byte[fileStream.Length];
                    fileStream.Read(fileData, 0, fileData.Length);

                    fileStream.Seek(0, SeekOrigin.Begin);

                    using (var encryptor = Aes.Create().CreateEncryptor(key, iv))
                    using (var cryptoStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Write))
                    {
                        fileStream.Write(salt, 0, salt.Length);
                        cryptoStream.Write(fileData, 0, fileData.Length);
                    }
                }

                LogDebug($"Baza danych '{filePath}' została zaszyfrowana.");
            }
            catch (Exception ex)
            {
                LogDebug($"Błąd podczas szyfrowania bazy danych: {ex.Message}");
                throw;
            }
        }


        public byte[] GenerateSalt(int size)
        {
            byte[] salt = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public void LogDebug(string message)
        {
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "debug_log.txt");
            File.AppendAllText(logPath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
    }
}