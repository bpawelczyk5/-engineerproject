using CerberPass.Services;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.Xml;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Wpf.Ui.Controls;


namespace CerberPass.Views
{
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
            LoadFavorites();
            LoadLastCopied();
        }

        public async void LoadFavorites()
        {
            List<(int ID, string Name, string Url, string Username, string Password, string NameIV, string UrlIV, string UsernameIV, string PasswordIV)> favorites = new List<(int, string, string, string, string, string, string, string, string)>();


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
                    string sql = "SELECT id, name, url, user_name, password, iv_name, iv_url, iv, iv_username FROM main WHERE is_favourite = 1";
                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                var encryptedName = reader["name"]?.ToString() ?? string.Empty;
                                var encryptedUrl = reader["url"]?.ToString() ?? string.Empty;
                                var encryptedUsername = reader["user_name"]?.ToString() ?? string.Empty;
                                var encryptedPassword = reader["password"]?.ToString() ?? string.Empty;
                                var ivName = reader["iv_name"]?.ToString() ?? string.Empty;
                                var ivUrl = reader["iv_url"]?.ToString() ?? string.Empty;
                                var ivUsername = reader["iv_username"]?.ToString() ?? string.Empty;
                                var ivPassword = reader["iv"]?.ToString() ?? string.Empty;

                                // Odszyfrowanie nazwy i URL przed dodaniem do listy
                                string name = AES.Decrypt(encryptedName, ivName);  // Odszyfrowujemy nazwę z iv_name
                                string url = AES.Decrypt(encryptedUrl, ivUrl);    // Odszyfrowujemy URL z iv_url
                                string username = AES.Decrypt(encryptedUsername, ivUsername);
                                string password = AES.Decrypt(encryptedPassword, ivPassword);

                                favorites.Add((id, name, url, username, password, ivName, ivUrl, ivUsername, ivPassword));
                            }
                        }
                    }
                }

                await DisplayFavorites(favorites);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Błąd podczas ładowania ulubionych: {ex.Message}");
            }
        }


        private async Task DisplayFavorites(List<(int ID, string Name, string Url, string Username, string Password, string NameIV, string UrlIV, string UsernameIV, string PasswordIV)> favorites)
        {
            favoritesWrapPanel.Children.Clear();

            List<Task<(int ID, string Name, ImageSource Icon, string Url, string Username, string Password)>> tasks = new List<Task<(int, string, ImageSource, string, string, string)>>();

            foreach (var (id, name, url, username, password, ivName, ivUrl, ivUsername, ivPassword) in favorites)
            {
                tasks.Add(GetFavicon(url).ContinueWith(task => (id, name, task.Result, url, username, password)));
            }

            var results = await Task.WhenAll(tasks);

            foreach (var (id, name, iconImage, url, username, password) in results)
            {
                var border = new Border
                {
                    Width = 130,
                    Height = 130,
                    Margin = new Thickness(5),
                    Background = Brushes.DimGray,
                    CornerRadius = new CornerRadius(10),
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Child = iconImage != null
                        ? (UIElement)new Wpf.Ui.Controls.Image
                        {
                            Source = iconImage,
                            Width = 50,
                            Height = 50,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        }
                        : (UIElement)new Wpf.Ui.Controls.TextBlock
                        {
                            Text = name, // Wyświetlamy odszyfrowaną nazwę
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            TextAlignment = TextAlignment.Center,
                            Foreground = Brushes.White
                        }
                };

                // Dodajemy dane do tagu lub innego pola, jeśli potrzebujesz go później użyć
                border.Tag = (id, name, iconImage ?? new BitmapImage(), url, username, password);

                border.MouseLeftButtonDown += (s, e) =>
                {
                    FavouriteClick(border);
                };

                favoritesWrapPanel.Children.Add(border);
            }
        }



        private async Task<ImageSource> GetFavicon(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url) || !Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    return null;
                }

                Uri uri = new Uri(url);
                string faviconUrl = $"{uri.Scheme}://{uri.Host}/favicon.ico";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(faviconUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var stream = await response.Content.ReadAsStreamAsync();
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        return bitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Błąd podczas pobierania favicon: {ex.Message}"); // Log error
            }

            return null;
        }

        private void CloseButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            FavouritePasswordDialog.Visibility = Visibility.Collapsed;
        }

        private void FavouriteClick(object sender)
        {
            var border = sender as Border;

            if (border?.Tag is (int id, string name, ImageSource icon, string url, string username, string password))
            {
                readNameTextBox.Text = name;
                readUrlTextBox.Text = url;
                readUsernameTextBox.Text = username;
                readPasswordTextBox.Password = password;

                readNameTextBox.IsReadOnly = true;
                readUsernameTextBox.IsReadOnly = true;
                readPasswordTextBox.IsReadOnly = true;
                readUrlTextBox.IsReadOnly = true;

                FavouritePasswordDialog.Visibility = Visibility.Visible;
            }
            else
            {
                System.Windows.MessageBox.Show("Błąd przy ładowaniu elementu.");
            }
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchBox.Text.ToLower();

            // Sprawdź, czy pole tekstowe jest puste
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Załaduj ponownie wszystkie dane ulubionych, jeśli pole wyszukiwania jest puste
                LoadFavorites();
            }
            else
            {
                // Filtruj dane na podstawie tekstu wyszukiwania
                FilterData(searchText);
            }
        }
        private async void FilterData(string searchText)
        {
            // Tworzymy nową listę, która przechowa przefiltrowane ulubione z IV
            List<(int ID, string Name, string Url, string Username, string Password, string NameIV, string UrlIV, string UsernameIV, string PasswordIV)> filteredFavorites = new List<(int, string, string, string, string, string, string, string, string)>();

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

                    // Pobieramy wszystkie ulubione elementy, nie filtrując ich jeszcze po nazwie, aby odszyfrować
                    string sql = $"SELECT id, name, url, user_name, password, iv_name, iv_url, iv_username, iv FROM main WHERE is_favourite = 1";

                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var id = Convert.ToInt32(reader["id"]);
                                var encryptedName = reader["name"]?.ToString() ?? string.Empty;
                                var url = reader["url"]?.ToString() ?? string.Empty;
                                var username = reader["user_name"]?.ToString() ?? string.Empty;
                                var password = reader["password"]?.ToString() ?? string.Empty;
                                var ivName = reader["iv_name"]?.ToString() ?? string.Empty;
                                var ivUrl = reader["iv_url"]?.ToString() ?? string.Empty;
                                var ivUsername = reader["iv_username"]?.ToString() ?? string.Empty;
                                var ivPassword = reader["iv"]?.ToString() ?? string.Empty;

                                // Odszyfruj nazwę
                                string decryptedName = AES.Decrypt(encryptedName, ivName).ToLower();

                                // Sprawdź, czy odszyfrowana nazwa zawiera tekst wyszukiwania
                                if (decryptedName.Contains(searchText.ToLower()))
                                {
                                    // Dodajemy tylko te elementy, które pasują do wyszukiwania
                                    filteredFavorites.Add((id, decryptedName, url, username, password, ivName, ivUrl, ivUsername, ivPassword));
                                }
                            }
                        }
                    }
                }

                // Wyświetlamy przefiltrowane wyniki w favoritesWrapPanel
                await DisplayFavorites(filteredFavorites);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Błąd podczas filtrowania danych: {ex.Message}");
            }
        }

        public async Task LoadLastCopied()
        {
            List<(int ID, string Name, string Url, string Username, string Password, string NameIV, string UrlIV, string UsernameIV, string PasswordIV)> lastCopied = new List<(int, string, string, string, string, string, string, string, string)>();

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
                    await connection.OpenAsync();

                    string sql = @"
                SELECT id, name, url, user_name, password, iv_name, iv_url, iv_username, iv 
                FROM main 
                WHERE is_lastcopied > 0 
                ORDER BY is_lastcopied ASC
                LIMIT 5;
            ";

                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        using (SQLiteDataReader reader = (SQLiteDataReader)await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int id = Convert.ToInt32(reader["id"]);
                                string name = reader["name"]?.ToString() ?? string.Empty;
                                string url = reader["url"]?.ToString() ?? string.Empty;
                                string username = reader["user_name"]?.ToString() ?? string.Empty;
                                string password = reader["password"]?.ToString() ?? string.Empty;

                                string ivName = reader["iv_name"]?.ToString() ?? string.Empty;
                                string ivUrl = reader["iv_url"]?.ToString() ?? string.Empty;
                                string ivUsername = reader["iv_username"]?.ToString() ?? string.Empty;
                                string ivPassword = reader["iv"]?.ToString() ?? string.Empty;

                                lastCopied.Add((id, name, url, username, password, ivName, ivUrl, ivUsername, ivPassword));
                            }
                        }
                    }
                }

                await DisplayLastCopied(lastCopied);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Błąd podczas ładowania ostatnio skopiowanych: {ex.Message}");
            }
        }

        private async Task DisplayLastCopied(List<(int ID, string Name, string Url, string Username, string Password, string NameIV, string UrlIV, string UsernameIV, string PasswordIV)> lastCopied)
        {
            recentlyCopiedWrapPanel.Children.Clear();

            var mostRecentCopied = lastCopied.TakeLast(5).Reverse().ToList();

            Dictionary<int, Border> existingItems = new Dictionary<int, Border>();

            List<Task<(int ID, string Name, ImageSource Icon, string Url, string Username, string Password)>> tasks = new List<Task<(int, string, ImageSource, string, string, string)>>();

            foreach (var (id, encryptedName, encryptedUrl, encryptedUsername, encryptedPassword, ivName, ivUrl, ivUsername, ivPassword) in mostRecentCopied)
            {
                // Odszyfruj wartości przed ich użyciem
                string name = AES.Decrypt(encryptedName, ivName);
                string url = AES.Decrypt(encryptedUrl, ivUrl);
                string username = AES.Decrypt(encryptedUsername, ivUsername);
                string password = AES.Decrypt(encryptedPassword, ivPassword);

                tasks.Add(GetFavicon(url).ContinueWith(task => (id, name, task.Result, url, username, password)));
            }

            var results = await Task.WhenAll(tasks);

            foreach (var (id, name, iconImage, url, username, password) in results)
            {
                if (existingItems.ContainsKey(id))
                {
                    recentlyCopiedWrapPanel.Children.Remove(existingItems[id]);
                }

                var border = new Border
                {
                    Width = 130,
                    Height = 130,
                    Margin = new Thickness(5),
                    Background = Brushes.DimGray,
                    CornerRadius = new CornerRadius(10),
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Child = iconImage != null
                        ? (UIElement)new Wpf.Ui.Controls.Image
                        {
                            Source = iconImage,
                            Width = 50,
                            Height = 50,
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center
                        }
                        : (UIElement)new Wpf.Ui.Controls.TextBlock
                        {
                            Text = name, // Wyświetlamy odszyfrowaną nazwę
                            VerticalAlignment = VerticalAlignment.Center,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            TextAlignment = TextAlignment.Center,
                            Foreground = Brushes.White
                        }
                };

                border.Tag = (id, name, iconImage ?? new BitmapImage(), url, username, password);

                border.MouseLeftButtonDown += (s, e) =>
                {
                    FavouriteClick(border);
                };

                recentlyCopiedWrapPanel.Children.Insert(0, border);
                existingItems[id] = border;
            }

            if (recentlyCopiedWrapPanel.Children.Count > 5)
            {
                while (recentlyCopiedWrapPanel.Children.Count > 5)
                {
                    recentlyCopiedWrapPanel.Children.RemoveAt(5);
                }
            }
        }


    }
}
