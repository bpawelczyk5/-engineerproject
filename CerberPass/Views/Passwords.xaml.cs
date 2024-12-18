using CerberPass.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Wpf.Ui.Controls;
using System.Security.Cryptography;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks; // Dodano dla async

namespace CerberPass.Views
{
    public partial class Passwords : UserControl
    {
        public Passwords()
        {
            InitializeComponent();
            LoadDataFromMainTable();
        }

        private void LoadDataFromMainTable()
        {
            string dbPath = Session.Instance.DatabasePath;

            if (string.IsNullOrEmpty(dbPath))
            {
                System.Windows.MessageBox.Show("Baza danych nie została wybrana.");
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();
                    string sql = "SELECT id, name, user_name, password, url, is_favourite, iv, iv_username, iv_name, iv_url, is_lastcopied FROM main";

                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, connection);
                    DataSet dataSet = new DataSet();
                    dataAdapter.Fill(dataSet, "main");

                    DataTable dataTable = dataSet.Tables["main"];
                    _fullDataList = new List<PasswordEntry>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        _fullDataList.Add(new PasswordEntry
                        {
                            ID = Convert.ToInt32(row["id"]),
                            Name = AES.Decrypt(row["name"].ToString(), row["iv_name"].ToString()),

                            // Odszyfruj nazwę użytkownika i hasło podczas ładowania z bazy danych
                            Username = AES.Decrypt(row["user_name"].ToString(), row["iv_username"].ToString()),
                            EncryptedPassword = row["password"].ToString(),
                            Url = AES.Decrypt(row["url"].ToString(), row["iv_url"].ToString()),
                            IsFavourite = Convert.ToBoolean(row["is_favourite"]),
                            IV = row["iv"].ToString(),
                            IV_Username = row["iv_username"].ToString(),
                            IsLastCopied = Convert.ToInt32(row["is_lastcopied"])
                        });
                    }


                    dataListView.ItemsSource = _fullDataList;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas ładowania danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void Button_Click(object sender, RoutedEventArgs e)
        {
            addPasswordDialog.Visibility = Visibility.Visible;
        }

        private void CloseButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            addPasswordDialog.Visibility = Visibility.Collapsed;
            editPasswordDialog.Visibility = Visibility.Collapsed;
        }

        private async void addPasswordDialog_ButtonClicked(object sender, RoutedEventArgs e)
        {
            string dbPath = Session.Instance.DatabasePath;

            if (string.IsNullOrEmpty(dbPath))
            {
                System.Windows.MessageBox.Show("Baza danych nie została wybrana.");
                return;
            }

            // Logowanie ścieżki bazy danych
            Utilities.LogDebug($"Ścieżka bazy danych: {dbPath}");

            int rowsAffected = 0; // Deklaracja zmiennej rowsAffected
            string originalDbPath = Session.Instance.OriginalDatabasePath; // Pobierz ścieżkę do oryginalnej bazy

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();
                    Utilities.LogDebug("Połączenie z bazą danych zostało otwarte.");

                    // Szyfruj
                    var (encryptedPassword, ivPassword) = AES.Encrypt(passwordTextBox.Password);
                    var (encryptedUsername, ivUsername) = AES.Encrypt(usernameTextBox.Text);
                    var (encryptedName, ivName) = AES.Encrypt(nameTextBox.Text);
                    var (encryptedUrl, ivUrl) = AES.Encrypt(urlTextBox.Text);

                    string sql = "INSERT INTO main (name, user_name, password, url, iv, iv_username, iv_name, iv_url) VALUES (@name, @user_name, @password, @url, @iv, @iv_username, @iv_name, @iv_url)";

                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", encryptedName);
                        command.Parameters.AddWithValue("@user_name", encryptedUsername);
                        command.Parameters.AddWithValue("@password", encryptedPassword);
                        command.Parameters.AddWithValue("@url", encryptedUrl);
                        command.Parameters.AddWithValue("@iv", ivPassword);
                        command.Parameters.AddWithValue("@iv_username", ivUsername);
                        command.Parameters.AddWithValue("@iv_name", ivName);
                        command.Parameters.AddWithValue("@iv_url", ivUrl);

                        rowsAffected = command.ExecuteNonQuery(); // Przypisanie wartości do rowsAffected
                        Utilities.LogDebug($"Liczba zmienionych wierszy: {rowsAffected}");
                    }
                }

                if (rowsAffected > 0)
                {
                    Utilities.LogDebug($"Dodano nowy rekord: Name={nameTextBox.Text}, Username={usernameTextBox.Text}, URL={urlTextBox.Text}");
                }
                else
                {
                    Utilities.LogDebug("Nie dodano żadnych rekordów.");
                }

                LoadDataFromMainTable();
                ClearForm();
                addPasswordDialog.Visibility = Visibility.Collapsed;

                // Zapisz zmiany z tymczasowej bazy do oryginalnej
                await SaveChangesToOriginalDatabase(originalDbPath); // Dodano await
                LoadDataFromHome();
            }
            catch (Exception ex)
            {
                Utilities.LogDebug($"Wystąpił błąd podczas dodawania danych: {ex.Message}");
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas dodawania danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void TogglePasswordVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.Tag is int recordId)
            {
                var passwordEntry = _fullDataList.FirstOrDefault(entry => entry.ID == recordId);
                if (passwordEntry != null)
                {
                    // Przełącz widoczność hasła
                    passwordEntry.IsPasswordVisible = !passwordEntry.IsPasswordVisible;
                }
            }
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Wpf.Ui.Controls.Button;
            if (button != null)
            {
                var url = button.Tag as string;
                if (!string.IsNullOrEmpty(url))
                {
                    // Sprawdź, czy URL zaczyna się od "http://" lub "https://"
                    if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                    {
                        // Dodaj przedrostek "https://", jeśli brakuje
                        url = "https://" + url;
                    }

                    try
                    {
                        // Utwórz obiekt Uri, aby wymusić poprawny format URL
                        var uri = new Uri(url);

                        // Sprawdź, czy URI to adres WWW (http lub https)
                        if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = url,
                                UseShellExecute = true
                            });
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("The provided link is not a valid web URL.");
                        }
                    }
                    catch (UriFormatException)
                    {
                        System.Windows.MessageBox.Show("Invalid URL format.");
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"Failed to open URL: {ex.Message}");
                    }
                }
            }
        }


        private void ShowInfoBar()
        {
            infoCopy.IsOpen = true;
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            timer.Tick += (sender, args) =>
            {
                infoCopy.IsOpen = false;
                timer.Stop();
            };
            timer.Start();
        }

        private async void CopyLoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.DataContext is PasswordEntry passwordData)
            {
                string username = passwordData.Username;
                Clipboard.SetText(username);
                ShowInfoBar();

                // Aktualizuj rekord w bazie danych, używając ID
                await UpdateIsLastCopied(passwordData.ID);
            }
        }

        private async void CopyPasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.DataContext is PasswordEntry passwordData)
            {
                string decryptedPassword = AES.Decrypt(passwordData.EncryptedPassword, passwordData.IV);
                Clipboard.SetText(decryptedPassword);
                ShowInfoBar();

                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(30)
                };

                timer.Tick += (s, args) =>
                {
                    Clipboard.Clear();
                    timer.Stop();
                };

                timer.Start();

                // Aktualizuj rekord w bazie danych, używając ID
                await UpdateIsLastCopied(passwordData.ID);
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntry = (PasswordEntry)((Wpf.Ui.Controls.Button)sender).Tag;

            // Załaduj dane do pól tekstowych dialogu
            editNameTextBox.Text = selectedEntry.Name;
            editUsernameTextBox.Text = selectedEntry.Username;
            editUrlTextBox.Text = selectedEntry.Url;

            // Wyczyść pole hasła, aby było puste przy każdej nowej edycji
            editPasswordTextBox.Password = string.Empty;

            // Otwórz dialog edycji
            editPasswordDialog.Visibility = Visibility.Visible;

            // Przechowaj wpis, aby użyć go podczas zapisu zmian
            editPasswordDialog.Tag = selectedEntry;
        }

        private async void EditPasswordDialog_ButtonClicked(object sender, RoutedEventArgs e)
        {
            var editedEntry = (PasswordEntry)editPasswordDialog.Tag;

            // Aktualizuj wartości z formularza
            editedEntry.Name = editNameTextBox.Text;
            editedEntry.Username = editUsernameTextBox.Text;
            editedEntry.Url = editUrlTextBox.Text;

            // Szyfrowanie hasła
            var (encryptedPassword, ivPassword) = AES.Encrypt(editPasswordTextBox.Password);
            editedEntry.EncryptedPassword = encryptedPassword;
            editedEntry.IV = ivPassword;

            // Szyfrowanie nazwy użytkownika
            var (encryptedUsername, ivUsername) = AES.Encrypt(editedEntry.Username);
            editedEntry.Username = encryptedUsername;
            editedEntry.IV_Username = ivUsername;

            var (encryptedName, ivName) = AES.Encrypt(editedEntry.Name);
            editedEntry.Name = encryptedName;
            editedEntry.IV_Name = ivName;

            var (encryptedUrl, ivUrl) = AES.Encrypt(editedEntry.Url);
            editedEntry.Url = encryptedUrl;
            editedEntry.IV_Url = ivUrl;

            UpdatePasswordInDatabase(editedEntry);
            editPasswordDialog.Visibility = Visibility.Collapsed;
            LoadDataFromMainTable();

            // Zapisz zmiany do oryginalnej bazy danych
            await SaveChangesToOriginalDatabase(Session.Instance.OriginalDatabasePath);
            LoadDataFromHome();
        }


        private void UpdatePasswordInDatabase(PasswordEntry entry)
        {
            string dbPath = Session.Instance.DatabasePath;

            if (string.IsNullOrEmpty(dbPath))
            {
                System.Windows.MessageBox.Show("Baza danych nie została wybrana.");
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();

                    string sql = "UPDATE main SET name = @name, user_name = @username, password = @password, url = @url, iv = @iv, iv_username = @ivUsername, iv_name = @ivName, iv_url = @ivUrl WHERE id = @id";

                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", entry.Name);
                        command.Parameters.AddWithValue("@username", entry.Username);
                        command.Parameters.AddWithValue("@password", entry.EncryptedPassword);
                        command.Parameters.AddWithValue("@url", entry.Url);
                        command.Parameters.AddWithValue("@iv", entry.IV);
                        command.Parameters.AddWithValue("@ivUsername", entry.IV_Username);
                        command.Parameters.AddWithValue("@ivName", entry.IV_Name);
                        command.Parameters.AddWithValue("@ivUrl", entry.IV_Url);
                        command.Parameters.AddWithValue("@id", entry.ID);

                        command.ExecuteNonQuery();
                    }
                }

                Utilities.LogDebug($"Zaktualizowano rekord: ID={entry.ID}, Name={entry.Name}, Username={entry.Username}, URL={entry.Url}");

                LoadDataFromMainTable();
            }
            catch (Exception ex)
            {
                Utilities.LogDebug($"Wystąpił błąd podczas aktualizacji danych: {ex.Message}");
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas aktualizacji danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ClearForm()
        {
            nameTextBox.Text = string.Empty;
            usernameTextBox.Text = string.Empty;
            passwordTextBox.Password = string.Empty;
            urlTextBox.Text = string.Empty;
        }

        public class PasswordEntry : INotifyPropertyChanged
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Username { get; set; }  // Odszyfrowana nazwa użytkownika
            public string EncryptedPassword { get; set; }  // Zaszyfrowane hasło z bazy
            public string Url { get; set; }
            public bool IsFavourite { get; set; }
            public string IV { get; set; }               // Wektor inicjalizacyjny dla hasła
            public string IV_Username { get; set; }
            public string IV_Name { get; set; }
            public string IV_Url { get; set; }
            public int IsLastCopied { get; set; }
            private bool _isPasswordVisible;
            public bool IsPasswordVisible
            {
                get => _isPasswordVisible;
                set
                {
                    _isPasswordVisible = value;
                    OnPropertyChanged(nameof(DisplayPassword));
                }
            }

            // Właściwość wyświetlająca odszyfrowane hasło lub gwiazdki
            public string DisplayPassword => IsPasswordVisible
                ? AES.Decrypt(EncryptedPassword, IV)
                : new string('*', 8);  // Wyświetla 8 gwiazdek jako placeholder

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Sprawdzenie, czy nadawca zdarzenia to przycisk z odpowiednim tagiem
            if (sender is Wpf.Ui.Controls.Button button && button.Tag is int id)
            {
                string dbPath = Session.Instance.DatabasePath;

                if (string.IsNullOrEmpty(dbPath))
                {
                    System.Windows.MessageBox.Show("Baza danych nie została wybrana.");
                    return;
                }

                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        connection.Open();

                        string sql = "DELETE FROM main WHERE id = @id";  // Usuwanie rekordu na podstawie ID

                        using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@id", id);
                            command.ExecuteNonQuery();
                        }
                    }

                    LoadDataFromMainTable();  // Odświeżenie danych po usunięciu rekordu

                    // Zapisz zmiany do oryginalnej bazy danych
                    await SaveChangesToOriginalDatabase(Session.Instance.OriginalDatabasePath);
                    LoadDataFromHome();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Wystąpił błąd podczas usuwania danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void FavouriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.Tag is int id) // Oczekujemy, że Tag zawiera ID
            {
                // Pobierz bieżący status ulubionych z bazy danych dla danego ID
                bool isCurrentlyFavourite = GetFavouriteStatusFromDb(id);

                // Odwróć status ulubionych
                bool newStatus = !isCurrentlyFavourite;

                // Zaktualizuj status ulubionych w bazie danych
                ToggleFavouriteStatus(id, newStatus);

                // Zaktualizuj ikonę przycisku po kliknięciu
                UpdateFavouriteIcon(button, newStatus);

                // Opcjonalnie: Odśwież listę główną, aby zaktualizować widok
                LoadDataFromMainTable();

                // Zapisz zmiany do oryginalnej bazy danych
                await SaveChangesToOriginalDatabase(Session.Instance.OriginalDatabasePath);
                LoadDataFromHome();
            }
            else
            {
                System.Windows.MessageBox.Show("Błąd: ID nie zostało poprawnie przekazane.");
            }
        }


        private bool GetFavouriteStatusFromDb(int id)
        {
            string dbPath = Session.Instance.DatabasePath;

            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                connection.Open();
                string sql = "SELECT is_favourite FROM main WHERE id = @id"; // Używamy ID jako kryterium
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    var result = command.ExecuteScalar();
                    return Convert.ToBoolean(result);
                }
            }
        }

        private void UpdateFavouriteIcon(Wpf.Ui.Controls.Button button, bool isFavourite)
        {
            // Znajdź kontrolkę SymbolIcon wewnątrz przycisku
            if (button.Content is Wpf.Ui.Controls.SymbolIcon icon)
            {
                icon.Filled = isFavourite;  // Zmień wypełnienie ikony na podstawie nowego statusu
            }
        }

        private void ToggleFavouriteStatus(int id, bool isFavourite)
        {
            string dbPath = Session.Instance.DatabasePath;

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();
                    string updateSql = "UPDATE main SET is_favourite = @isFavourite WHERE id = @id";

                    using (SQLiteCommand updateCommand = new SQLiteCommand(updateSql, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@isFavourite", isFavourite ? 1 : 0);
                        updateCommand.Parameters.AddWithValue("@id", id); // Używamy ID jako kryterium
                        updateCommand.ExecuteNonQuery();
                    }
                }

                // Odśwież listę, aby zaktualizować widok
                LoadDataFromMainTable();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas aktualizacji statusu ulubionych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<PasswordEntry> _fullDataList;

        private void FilterData(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                dataListView.ItemsSource = _fullDataList;
            }
            else
            {
                var filteredData = _fullDataList
                    .Where(p => p.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                p.Username.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                                p.Url.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                dataListView.ItemsSource = filteredData;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchTextBox = sender as Wpf.Ui.Controls.TextBox;
            if (searchTextBox != null)
            {
                FilterData(searchTextBox.Text);
            }
        }

        private async Task UpdateIsLastCopied(int id)
        {
            string dbPath = Session.Instance.DatabasePath;

            if (string.IsNullOrEmpty(dbPath))
            {
                System.Windows.MessageBox.Show("Baza danych nie została wybrana.");
                return;
            }

            try
            {
                using (var connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    await connection.OpenAsync();

                    // Zwiększ wartość `is_lastcopied` dla wszystkich elementów powyżej 0
                    var incrementCommand = connection.CreateCommand();
                    incrementCommand.CommandText = @"
                UPDATE main
                SET is_lastcopied = is_lastcopied + 1
                WHERE is_lastcopied > 0;
            ";
                    await incrementCommand.ExecuteNonQueryAsync();

                    // Ustaw `is_lastcopied` na 1 dla najbardziej niedawno skopiowanego elementu
                    var updateCommand = connection.CreateCommand();
                    updateCommand.CommandText = @"
                UPDATE main
                SET is_lastcopied = 1
                WHERE id = @id;
            ";
                    updateCommand.Parameters.AddWithValue("@id", id);
                    await updateCommand.ExecuteNonQueryAsync();
                }

                // Zapisz zmiany do oryginalnej bazy danych
                await SaveChangesToOriginalDatabase(Session.Instance.OriginalDatabasePath);
                LoadDataFromMainTable();
                LoadDataFromHome();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas aktualizacji: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task SaveChangesToOriginalDatabase(string originalDbPath)
        {
            if (string.IsNullOrEmpty(originalDbPath))
            {
                System.Windows.MessageBox.Show("Nie znaleziono ścieżki do oryginalnej bazy danych.");
                return;
            }
            try
            {
                string tempDbPath = Session.Instance.DatabasePath;
                string password = Session.Instance.UserPassword;

                if (string.IsNullOrEmpty(tempDbPath))
                {
                    Utilities.LogDebug("Tymczasowa baza danych nie jest dostępna.");
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    Utilities.LogDebug("Hasło użytkownika nie jest dostępne.");
                    return;
                }
                Utilities.LogDebug($"Rozpoczęto zapis zmian do oryginalnej bazy danych: {originalDbPath} z tymczasowej {tempDbPath}");
                Utilities.ForceCloseSQLiteConnections(tempDbPath);
                await Task.Delay(100);

                await Task.Run(() =>
                {
                    EncryptAndReplaceDatabase(tempDbPath, originalDbPath, password);
                });

                Utilities.LogDebug($"Zapis zmian do oryginalnej bazy danych: {originalDbPath} zakończony.");

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas zapisywania zmian w bazie danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadDataFromHome()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Window.GetWindow(this) is MainWindow mainWindow)
                {
                    if (mainWindow.MainContent.Content is Home home)
                    {
                        home.LoadFavorites();
                        home.LoadLastCopied();
                    }
                }
            });
        }
        public void EncryptAndReplaceDatabase(string tempDbPath, string originalDbPath, string password)
        {
            byte[] key, iv;
            byte[] salt = GetSaltFromEncryptedDb(originalDbPath);

            using (var keyGenerator = new Rfc2898DeriveBytes(password, salt))
            {
                key = keyGenerator.GetBytes(32);
                iv = keyGenerator.GetBytes(16);
            }

            try
            {
                using (var tempFileStream = new FileStream(tempDbPath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    byte[] fileData = new byte[tempFileStream.Length];
                    tempFileStream.Read(fileData, 0, fileData.Length);

                    using (var originalFileStream = new FileStream(originalDbPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    {
                        originalFileStream.Seek(0, SeekOrigin.Begin);

                        using (var encryptor = Aes.Create().CreateEncryptor(key, iv))
                        using (var cryptoStream = new CryptoStream(originalFileStream, encryptor, CryptoStreamMode.Write))
                        {
                            originalFileStream.Write(salt, 0, salt.Length);
                            cryptoStream.Write(fileData, 0, fileData.Length);
                        }
                    }
                }

                Utilities.LogDebug($"Baza danych '{originalDbPath}' została zaszyfrowana i nadpisana.");
            }
            catch (Exception ex)
            {
                Utilities.LogDebug($"Błąd podczas szyfrowania bazy danych: {ex.Message}");
                throw;
            }
        }
        private void AttemptFileReplace(string originalDbPath, string tempEncryptedFilePath)
        {
            int maxRetries = 5;
            int retryDelayMs = 100;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    File.Delete(originalDbPath);
                    File.Move(tempEncryptedFilePath, originalDbPath);
                    return;
                }
                catch (IOException ex)
                {
                    Utilities.LogDebug($"Błąd podczas nadpisywania pliku  {originalDbPath} próba {i + 1} z {maxRetries} {ex.Message}");
                    if (i == maxRetries - 1)
                    {
                        throw;
                    }
                    Thread.Sleep(retryDelayMs);
                }
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
                Utilities.LogDebug($"Błąd podczas pobierania salt z bazy danych: {ex.Message}");
                throw;
            }
            return salt;
        }
    }
}