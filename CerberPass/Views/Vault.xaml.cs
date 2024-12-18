using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Xml.Linq;
using CerberPass.Services;
using Wpf.Ui.Controls;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Threading.Tasks;
using System.IO;
using CerberPass.Services;

namespace CerberPass.Views
{
    public partial class Vault : UserControl
    {
        private List<CreditCardEntry> creditCardData;
        private List<ContactEntry> contactData;
        private List<NoteEntry> noteData;

        public Vault()
        {
            InitializeComponent();
            InitializeDefaultToggle();
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            // Load initial data for each section
            LoadCreditCards();
            LoadContacts();
            LoadNotes();
        }

        public class ValidationHelper
        {
            // Walidacja dla CVV - musi mieć 3 cyfry
            public static bool IsValidCVV(string cvv)
            {
                return Regex.IsMatch(cvv, @"^\d{3}$");
            }

            // Walidacja dla daty - musi być w formacie MM/YY
            public static bool IsValidDate(string date)
            {
                return Regex.IsMatch(date, @"^\d{2}/\d{2}$");
            }

            // Walidacja dla numeru karty kredytowej - musi mieć 16 cyfr
            public static bool IsValidCreditCardNumber(string cardNumber)
            {
                return Regex.IsMatch(cardNumber, @"^\d{16}$");
            }

            // Walidacja dla numeru telefonu - musi zaczynać się od +48 i mieć 9 cyfr po nim
            public static bool IsValidPhoneNumber(string phoneNumber)
            {
                return Regex.IsMatch(phoneNumber, @"^\+48\d{9}$");
            }

            // Walidacja dla adresu email - musi zawierać "@" oraz "."
            public static bool IsValidEmail(string email)
            {
                return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            }
        }
        private void LoadCreditCards()
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
                    string sql = "SELECT id, name, iv_name, card_number, iv_cardnumber, date, iv_date, cvv, iv_cvv FROM credit_card";
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, connection);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    creditCardData = new List<CreditCardEntry>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var creditCardEntry = new CreditCardEntry
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Name = AES.Decrypt(row["name"].ToString(), row["iv_name"].ToString()),
                            EncryptedCardNumber = row["card_number"].ToString(),
                            EncryptedDate = row["date"].ToString(),
                            EncryptedCvv = row["cvv"].ToString(),
                            IVCardNumber = row["iv_cardnumber"].ToString(),
                            IVDate = row["iv_date"].ToString(),
                            IVCVV = row["iv_cvv"].ToString(),
                            IsCardNumberVisible = false,  // Początkowo numer karty ukryty
                            IsDateVisible = false,        // Początkowo data ukryta
                            IsCvvVisible = false          // Początkowo CVV ukryte
                        };

                        creditCardData.Add(creditCardEntry);
                    }

                    creditCardsListView.ItemsSource = creditCardData; // Załaduj dane do widoku
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas ładowania danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void LoadContacts()
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
                    string sql = "SELECT id, name, lastname, phone_number, email, iv_name, iv_lastname, iv_phonenumber, iv_email FROM contacts";
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, connection);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    contactData = new List<ContactEntry>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        var contactEntry = new ContactEntry
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Name = AES.Decrypt(row["name"].ToString(), row["iv_name"].ToString()),
                            LastName = AES.Decrypt(row["lastname"].ToString(), row["iv_lastname"].ToString()),
                            EncryptedPhoneNumber = row["phone_number"].ToString(),
                            EncryptedEmail = row["email"].ToString(),
                            IVName = row["iv_name"].ToString(),
                            IVLastName = row["iv_lastname"].ToString(),
                            IVPhoneNumber = row["iv_phonenumber"].ToString(),
                            IVEmail = row["iv_email"].ToString(),
                            IsPhoneNumberVisible = false,  // Początkowo numer telefonu ukryty
                            IsEmailVisible = false  // Początkowo numer telefonu ukryty
                        };

                        contactData.Add(contactEntry);
                    }

                    contactsListView.ItemsSource = contactData; // Załaduj dane do widoku
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas ładowania danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadNotes()
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

                    string sql = "SELECT id, name, text, iv_name, iv FROM notes";  // Dodaj pole IV, jeśli jest używane
                    SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(sql, connection);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    noteData = new List<NoteEntry>();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        // Odszyfruj nazwę notatki
                        string decryptedName = AES.Decrypt(row["name"].ToString(), row["iv_name"].ToString());

                        noteData.Add(new NoteEntry
                        {
                            Id = Convert.ToInt32(row["id"]),
                            Name = decryptedName, // Przypisz odszyfrowaną nazwę
                            EncryptedText = row["text"].ToString(),
                            IVName = row["iv_name"].ToString(),
                            IV = row["iv"].ToString()  // Wczytaj IV dla tekstu
                        });
                    }

                    FilterData();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas ładowania danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void FilterData()
        {
            string searchText = SearchTextBox.Text.ToLower();

            if (CreditCardsToggle.IsChecked == true)
            {
                var filteredData = creditCardData.Where(cc =>
                    cc.Name.ToLower().Contains(searchText) ||
                    cc.EncryptedCardNumber.ToLower().Contains(searchText) ||
                    cc.EncryptedDate.ToLower().Contains(searchText) ||
                    cc.EncryptedCvv.ToLower().Contains(searchText)).ToList();

                creditCardsListView.ItemsSource = filteredData;
            }
            else if (ContactsToggle.IsChecked == true)
            {
                var filteredData = contactData.Where(c =>
                    c.Name.ToLower().Contains(searchText) ||
                    c.LastName.ToLower().Contains(searchText) ||
                    c.EncryptedPhoneNumber.ToLower().Contains(searchText) ||
                    c.EncryptedEmail.ToLower().Contains(searchText)).ToList();

                contactsListView.ItemsSource = filteredData;
            }
            else if (NotesToggle.IsChecked == true)
            {
                var filteredData = noteData.Where(n =>
                    n.Name.ToLower().Contains(searchText) ||
                    n.EncryptedText.ToLower().Contains(searchText)).ToList();

                notesListView.ItemsSource = filteredData;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterData();
        }

        private void InitializeDefaultToggle()
        {
            // Default to CreditCardsToggle being checked
            CreditCardsToggle.IsChecked = true;
            UpdateToggleStates();
            LoadCreditCards(); // Load credit card data by default
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch checkedToggle)
            {
                // Uncheck all other toggles
                foreach (var toggle in new[] { CreditCardsToggle, ContactsToggle, NotesToggle })
                {
                    if (toggle != checkedToggle)
                    {
                        toggle.IsChecked = false;
                    }
                }

                // Update the states and load data based on the checked toggle
                UpdateToggleStates();
                LoadDataForCurrentToggle(checkedToggle);
            }
        }

        private void UpdateToggleStates()
        {
            // Enable all toggles first
            foreach (var toggle in new[] { CreditCardsToggle, ContactsToggle, NotesToggle })
            {
                toggle.IsEnabled = true;
            }

            // Disable the currently checked toggle
            var currentCheckedToggle = FindCheckedToggle();
            if (currentCheckedToggle != null)
            {
                currentCheckedToggle.IsEnabled = false;
            }
        }

        private ToggleSwitch FindCheckedToggle()
        {
            // Check which toggle is currently checked
            if (CreditCardsToggle.IsChecked == true) return CreditCardsToggle;
            if (ContactsToggle.IsChecked == true) return ContactsToggle;
            if (NotesToggle.IsChecked == true) return NotesToggle;
            return null;
        }

        private void LoadDataForCurrentToggle(ToggleSwitch checkedToggle)
        {
            // Manage visibility and data loading based on the currently checked toggle
            if (checkedToggle == CreditCardsToggle)
            {
                creditCardsListView.Visibility = Visibility.Visible;
                contactsListView.Visibility = Visibility.Collapsed;
                notesListView.Visibility = Visibility.Collapsed;
                LoadCreditCards();
            }
            else if (checkedToggle == ContactsToggle)
            {
                contactsListView.Visibility = Visibility.Visible;
                creditCardsListView.Visibility = Visibility.Collapsed;
                notesListView.Visibility = Visibility.Collapsed;
                LoadContacts();
            }
            else if (checkedToggle == NotesToggle)
            {
                notesListView.Visibility = Visibility.Visible;
                creditCardsListView.Visibility = Visibility.Collapsed;
                contactsListView.Visibility = Visibility.Collapsed;
                LoadNotes();
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

        private void CopyCreditCardButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.DataContext is CreditCardEntry creditCardData)
            {
                // Odszyfruj numer karty
                string decryptedCardNumber = AES.Decrypt(creditCardData.EncryptedCardNumber, creditCardData.IVCardNumber);

                // Skopiuj odszyfrowany numer karty kredytowej do schowka
                Clipboard.SetText(decryptedCardNumber);
                ShowInfoBar(); // Pokaż pasek informacyjny

                // Ustaw timer, aby usunąć numer ze schowka po 30 sekundach
                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(30)
                };

                timer.Tick += (s, args) =>
                {
                    // Usuń zawartość schowka
                    Clipboard.Clear();
                    timer.Stop(); // Zatrzymaj timer
                };

                timer.Start(); // Rozpocznij timer
            }
            else
            {
                System.Windows.MessageBox.Show("Nie znaleziono odpowiednich danych w DataContext.");
            }
        }

        private void CopyDateButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.DataContext is CreditCardEntry creditCardData)
            {
                // Odszyfruj datę
                string decryptedDate = AES.Decrypt(creditCardData.EncryptedDate, creditCardData.IVDate);

                // Skopiuj odszyfrowaną datę do schowka
                Clipboard.SetText(decryptedDate);
                ShowInfoBar(); // Pokaż pasek informacyjny

                // Ustaw timer, aby usunąć datę ze schowka po 30 sekundach
                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(30)
                };

                timer.Tick += (s, args) =>
                {
                    // Usuń zawartość schowka
                    Clipboard.Clear();
                    timer.Stop(); // Zatrzymaj timer
                };

                timer.Start(); // Rozpocznij timer
            }
            else
            {
                System.Windows.MessageBox.Show("Nie znaleziono odpowiednich danych w DataContext.");
            }
        }

        private void CopyCVVButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.DataContext is CreditCardEntry creditCardData)
            {
                // Odszyfruj CVV
                string decryptedCVV = AES.Decrypt(creditCardData.EncryptedCvv, creditCardData.IVCVV);

                // Skopiuj odszyfrowane CVV do schowka
                Clipboard.SetText(decryptedCVV);
                ShowInfoBar(); // Pokaż pasek informacyjny

                // Ustaw timer, aby usunąć CVV ze schowka po 30 sekundach
                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(30)
                };

                timer.Tick += (s, args) =>
                {
                    // Usuń zawartość schowka
                    Clipboard.Clear();
                    timer.Stop(); // Zatrzymaj timer
                };

                timer.Start(); // Rozpocznij timer
            }
            else
            {
                System.Windows.MessageBox.Show("Nie znaleziono odpowiednich danych w DataContext.");
            }
        }

        private async void AddCreditCard()
        {
            string dbPath = Session.Instance.DatabasePath;
            string originalDbPath = Session.Instance.OriginalDatabasePath; // Pobierz ścieżkę do oryginalnej bazy

            if (string.IsNullOrEmpty(dbPath))
            {
                System.Windows.MessageBox.Show("Baza danych nie została wybrana.");
                return;
            }

            // Walidacja numeru karty kredytowej, daty ważności i CVV
            if (!ValidationHelper.IsValidCreditCardNumber(cardNumberTextBox.Password))
            {
                System.Windows.MessageBox.Show("Numer karty musi mieć 16 cyfr.");
                return;
            }

            if (!ValidationHelper.IsValidDate(expirationDateTextBox.Password))
            {
                System.Windows.MessageBox.Show("Data ważności musi być w formacie MM/YY.");
                return;
            }

            if (!ValidationHelper.IsValidCVV(cvvTextBox.Password))
            {
                System.Windows.MessageBox.Show("CVV musi mieć 3 cyfry.");
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();

                    // Zapis danych w bazie z odpowiednimi IV (wektory inicjujące)
                    string sql = "INSERT INTO credit_card (name, card_number, date, cvv, iv_name, iv_cardnumber, iv_date, iv_cvv) VALUES (@name, @card_number, @date, @cvv, @iv_name, @iv_cardnumber, @iv_date, @iv_cvv)";

                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        // Szyfrowanie danych
                        var (encryptedName, ivName) = AES.Encrypt(creditCardNameTextBox.Text);
                        var (encryptedCardNumber, ivCardNumber) = AES.Encrypt(cardNumberTextBox.Password);
                        var (encryptedDate, ivDate) = AES.Encrypt(expirationDateTextBox.Password);
                        var (encryptedCVV, ivCvv) = AES.Encrypt(cvvTextBox.Password);

                        // Dodanie zaszyfrowanych danych oraz odpowiadających im IV
                        command.Parameters.AddWithValue("@name", encryptedName);
                        command.Parameters.AddWithValue("@card_number", encryptedCardNumber);
                        command.Parameters.AddWithValue("@date", encryptedDate);
                        command.Parameters.AddWithValue("@cvv", encryptedCVV);
                        command.Parameters.AddWithValue("@iv_name", ivName);
                        command.Parameters.AddWithValue("@iv_cardnumber", ivCardNumber);
                        command.Parameters.AddWithValue("@iv_date", ivDate);
                        command.Parameters.AddWithValue("@iv_cvv", ivCvv);

                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                // Załaduj dane kart kredytowych, wyczyść formularz i zamknij okno dialogowe
                LoadCreditCards();
                ClearCreditCardForm();
                addCreditCardDialog.Visibility = Visibility.Collapsed;

                // Zapisz zmiany z tymczasowej bazy do oryginalnej
                await SaveChangesToOriginalDatabase(originalDbPath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas dodawania danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void AddContact()
        {
            string dbPath = Session.Instance.DatabasePath;
            string originalDbPath = Session.Instance.OriginalDatabasePath; // Pobierz ścieżkę do oryginalnej bazy

            if (string.IsNullOrEmpty(dbPath))
            {
                System.Windows.MessageBox.Show("Baza danych nie została wybrana.");
                return;
            }

            // Walidacja numeru telefonu i adresu e-mail
            if (!ValidationHelper.IsValidPhoneNumber(contactPhoneNumberTextBox.Text))
            {
                System.Windows.MessageBox.Show("Numer telefonu musi zaczynać się od +48 i mieć 9 cyfr.");
                return;
            }

            if (!ValidationHelper.IsValidEmail(contactEmailTextBox.Text))
            {
                System.Windows.MessageBox.Show("Adres e-mail musi zawierać '@' oraz '.'");
                return;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    connection.Open();

                    string sql = "INSERT INTO contacts (name, lastname, phone_number, email, iv_name, iv_lastname, iv_phonenumber, iv_email) VALUES (@name, @lastname, @phone_number, @email, @iv_name, @iv_lastname, @iv_phonenumber, @iv_email)";

                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        // Szyfrowanie danych
                        var (encryptedName, ivName) = AES.Encrypt(contactNameTextBox.Text);
                        var (encryptedLastName, ivLastName) = AES.Encrypt(contactLastNameTextBox.Text);
                        var (encryptedPhoneNumber, ivPhoneNumber) = AES.Encrypt(contactPhoneNumberTextBox.Text);
                        var (encryptedEmail, ivEmail) = AES.Encrypt(contactEmailTextBox.Text);

                        // Dodawanie zaszyfrowanych danych jako parametrów SQL
                        command.Parameters.AddWithValue("@name", encryptedName);
                        command.Parameters.AddWithValue("@lastname", encryptedLastName);
                        command.Parameters.AddWithValue("@phone_number", encryptedPhoneNumber);
                        command.Parameters.AddWithValue("@email", encryptedEmail);
                        command.Parameters.AddWithValue("@iv_name", ivName);
                        command.Parameters.AddWithValue("@iv_lastname", ivLastName);
                        command.Parameters.AddWithValue("@iv_phonenumber", ivPhoneNumber);
                        command.Parameters.AddWithValue("@iv_email", ivEmail);

                        // Wykonanie zapisu do bazy
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                // Załadowanie kontaktów i wyczyszczenie formularza
                LoadContacts();
                ClearContactForm();
                addContactDialog.Visibility = Visibility.Collapsed;

                // Zapisz zmiany z tymczasowej bazy do oryginalnej
                await SaveChangesToOriginalDatabase(originalDbPath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas dodawania danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddNote()
        {
            string dbPath = Session.Instance.DatabasePath;
            string originalDbPath = Session.Instance.OriginalDatabasePath; // Pobierz ścieżkę do oryginalnej bazy

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

                    string sql = "INSERT INTO notes (name, text, iv_name, iv) VALUES (@name, @text, @iv_name, @iv)";

                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        // Szyfruj nazwę notatki przed zapisaniem
                        var (encryptedName, ivName) = AES.Encrypt(noteTitleTextBox.Text);

                        // Uzyskanie tekstu z RichTextBox
                        string text = new TextRange(noteContentTextBox.Document.ContentStart, noteContentTextBox.Document.ContentEnd).Text;

                        // Szyfruj tekst notatki
                        var (encryptedText, ivText) = AES.Encrypt(text);  // Użyj swojej metody szyfrowania

                        command.Parameters.AddWithValue("@name", encryptedName);
                        command.Parameters.AddWithValue("@text", encryptedText);
                        command.Parameters.AddWithValue("@iv_name", ivName);  // Zapisz IV dla nazwy
                        command.Parameters.AddWithValue("@iv", ivText);  // Zapisz IV dla tekstu

                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                LoadNotes();
                ClearNoteForm();
                addNoteDialog.Visibility = Visibility.Collapsed;
                // Zapisz zmiany z tymczasowej bazy do oryginalnej
                await SaveChangesToOriginalDatabase(originalDbPath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas dodawania danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CopyPhoneNumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button)
            {
                // Upewnij się, że tag przycisku zawiera odpowiednie dane kontaktowe
                var contactEntry = button.DataContext as ContactEntry;
                if (contactEntry != null)
                {
                    // Odszyfruj numer telefonu
                    string decryptedPhoneNumber = AES.Decrypt(contactEntry.EncryptedPhoneNumber, contactEntry.IVPhoneNumber);
                    // Skopiuj odszyfrowany numer telefonu do schowka
                    Clipboard.SetText(decryptedPhoneNumber);
                    ShowInfoBar();

                    // Ustaw timer, aby usunąć numer ze schowka po 30 sekundach
                    var timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(30)
                    };

                    timer.Tick += (s, args) =>
                    {
                        // Usuń zawartość schowka
                        Clipboard.Clear();
                        timer.Stop(); // Zatrzymaj timer
                    };

                    timer.Start(); // Rozpocznij timer
                }
            }
        }

        private void CopyEmailButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button)
            {
                // Upewnij się, że tag przycisku zawiera odpowiednie dane kontaktowe
                var contactEntry = button.DataContext as ContactEntry;
                if (contactEntry != null)
                {
                    // Odszyfruj numer telefonu
                    string decryptedEmail = AES.Decrypt(contactEntry.EncryptedEmail, contactEntry.IVEmail);
                    // Skopiuj odszyfrowany numer telefonu do schowka
                    Clipboard.SetText(decryptedEmail);
                    ShowInfoBar();
                }
            }
        }


        private void AddCreditCardDialog_ButtonClicked(object sender, RoutedEventArgs e)
        {
            AddCreditCard();
        }

        private void AddContactDialog_ButtonClicked(object sender, RoutedEventArgs e)
        {
            AddContact();
        }

        private void AddNoteDialog_ButtonClicked(object sender, RoutedEventArgs e)
        {
            AddNote();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (CreditCardsToggle.IsChecked == true)
            {
                addCreditCardDialog.Visibility = Visibility.Visible;
            }
            else if (ContactsToggle.IsChecked == true)
            {
                addContactDialog.Visibility = Visibility.Visible;
            }
            else if (NotesToggle.IsChecked == true)
            {
                addNoteDialog.Visibility = Visibility.Visible;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            addCreditCardDialog.Visibility = Visibility.Collapsed;
            addContactDialog.Visibility = Visibility.Collapsed;
            addNoteDialog.Visibility = Visibility.Collapsed;
            editCreditCardDialog.Visibility = Visibility.Collapsed;
            editContactDialog.Visibility = Visibility.Collapsed;
            editNoteDialog.Visibility = Visibility.Collapsed;
        }

        private void ClearCreditCardForm()
        {
            creditCardNameTextBox.Text = string.Empty;
            cardNumberTextBox.Password = string.Empty;
            expirationDateTextBox.Password = string.Empty;
            cvvTextBox.Password = string.Empty;
        }

        private void ClearContactForm()
        {
            contactNameTextBox.Text = string.Empty;
            contactLastNameTextBox.Text = string.Empty;
            contactPhoneNumberTextBox.Text = string.Empty;
            contactEmailTextBox.Text = string.Empty;
        }

        private void ClearNoteForm()
        {
            noteTitleTextBox.Text = string.Empty;
            // Czyszczenie RichTextBox
            noteContentTextBox.Document.Blocks.Clear();
        }

        private void EditCreditCard_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntry = (CreditCardEntry)((Wpf.Ui.Controls.Button)sender).Tag;

            // Sprawdź, czy selectedEntry jest null
            if (selectedEntry == null)
            {
                System.Windows.MessageBox.Show("Nie znaleziono wybranej karty do edycji.", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Przerwij dalsze wykonywanie metody
            }

            // Odszyfruj przed załadowaniem do formularza
            string decryptedCardNumber = AES.Decrypt(selectedEntry.EncryptedCardNumber, selectedEntry.IVCardNumber);
            string decryptedDate = AES.Decrypt(selectedEntry.EncryptedDate, selectedEntry.IVDate);

            // Załaduj dane do pól tekstowych dialogu
            editCreditCardNameTextBox.Text = selectedEntry.Name;
            editCardNumberTextBox.Password = decryptedCardNumber;
            editExpirationDateTextBox.Password = decryptedDate;

            // Wyczyść pole, aby było puste przy każdej nowej edycji
            editCvvTextBox.Password = string.Empty;

            // Otwórz dialog edycji
            editCreditCardDialog.Visibility = Visibility.Visible;

            // Przechowaj wpis, aby użyć go podczas zapisu zmian
            editCreditCardDialog.Tag = selectedEntry;
        }


        private async void EditCreditCardDialog_ButtonClicked(object sender, RoutedEventArgs e)
        {
            // Pobierz wpis przechowywany w Tag
            var editedEntry = (CreditCardEntry)editCreditCardDialog.Tag;

            // Zaktualizuj wpis na podstawie danych z formularza
            editedEntry.Name = editCreditCardNameTextBox.Text;
            editedEntry.EncryptedCardNumber = editCardNumberTextBox.Password; // Przechowuj odszyfrowany numer karty
            editedEntry.EncryptedDate = editExpirationDateTextBox.Password;   // Przechowuj odszyfrowaną datę
            editedEntry.EncryptedCvv = editCvvTextBox.Password;               // Przechowuj odszyfrowane CVV

            // Wywołaj metodę aktualizującą wpis w bazie danych
            EditCreditCard(editedEntry);

            // Zamknij dialog po zapisaniu zmian
            editCreditCardDialog.Visibility = Visibility.Collapsed;

            // Odśwież widok listy
            LoadCreditCards();

            // Zapisz zmiany z tymczasowej bazy do oryginalnej
            await SaveChangesToOriginalDatabase(Session.Instance.OriginalDatabasePath);
        }


        private void EditCreditCard(CreditCardEntry entry)
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

                    string sql = "UPDATE credit_card SET name = @name, card_number = @card_number, date = @date, cvv = @cvv, iv_name = @iv_name, iv_date = @iv_date, iv_cardnumber = @iv_cardnumber, iv_cvv = @iv_cvv WHERE id = @id";

                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        // Szyfrowanie danych, w tym także kolumny name
                        var (encryptedName, ivName) = AES.Encrypt(entry.Name);
                        var (encryptedCardNumber, ivCardNumber) = AES.Encrypt(entry.EncryptedCardNumber);
                        var (encryptedDate, ivDate) = AES.Encrypt(entry.EncryptedDate);
                        var (encryptedCVV, ivCvv) = AES.Encrypt(entry.EncryptedCvv);

                        command.Parameters.AddWithValue("@name", encryptedName); // Używamy zaszyfrowanego name
                        command.Parameters.AddWithValue("@card_number", encryptedCardNumber);
                        command.Parameters.AddWithValue("@date", encryptedDate);
                        command.Parameters.AddWithValue("@cvv", encryptedCVV);
                        command.Parameters.AddWithValue("@id", entry.Id);

                        command.Parameters.AddWithValue("@iv_name", ivName);
                        command.Parameters.AddWithValue("@iv_cardnumber", ivCardNumber);
                        command.Parameters.AddWithValue("@iv_date", ivDate);
                        command.Parameters.AddWithValue("@iv_cvv", ivCvv);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            System.Windows.MessageBox.Show("Nie znaleziono rekordu do zaktualizowania.");
                        }
                    }
                }

                LoadCreditCards(); // Ponowne załadowanie danych po edycji
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas dodawania danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void EditContact_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntry = (ContactEntry)((Wpf.Ui.Controls.Button)sender).Tag;

            // Sprawdź, czy selectedEntry jest null
            if (selectedEntry == null)
            {
                System.Windows.MessageBox.Show("Nie znaleziono wybranej karty do edycji.", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Przerwij dalsze wykonywanie metody
            }

            // Odszyfruj przed załadowaniem do formularza
            string decryptedPhoneNumber = AES.Decrypt(selectedEntry.EncryptedPhoneNumber, selectedEntry.IVPhoneNumber);
            string decryptedEmail = AES.Decrypt(selectedEntry.EncryptedEmail, selectedEntry.IVEmail);

            // Załaduj dane do pól tekstowych dialogu
            editContactNameTextBox.Text = selectedEntry.Name;
            editContactLastNameTextBox.Text = selectedEntry.LastName;
            editContactPhoneNumberTextBox.Text = decryptedPhoneNumber; // Użyj odszyfrowanego numeru
            editContactEmailTextBox.Text = decryptedEmail;

            // Otwórz dialog edycji
            editContactDialog.Visibility = Visibility.Visible;

            // Przechowaj wpis, aby użyć go podczas zapisu zmian
            editContactDialog.Tag = selectedEntry;
        }


        private async void EditContactDialog_ButtonClicked(object sender, RoutedEventArgs e)
        {
            // Pobierz wpis przechowywany w Tag
            var editedEntry = (ContactEntry)editContactDialog.Tag;

            // Zaktualizuj wpis na podstawie danych z formularza
            editedEntry.Name = editContactNameTextBox.Text;
            editedEntry.LastName = editContactLastNameTextBox.Text;
            editedEntry.EncryptedPhoneNumber = editContactPhoneNumberTextBox.Text;
            editedEntry.EncryptedEmail = editContactEmailTextBox.Text;

            // Wywołaj metodę aktualizującą wpis w bazie danych
            EditContact(editedEntry);

            // Zamknij dialog po zapisaniu zmian
            editContactDialog.Visibility = Visibility.Collapsed;

            // Odśwież widok listy
            LoadContacts();

            // Zapisz zmiany z tymczasowej bazy do oryginalnej
            await SaveChangesToOriginalDatabase(Session.Instance.OriginalDatabasePath);
        }

        private void EditContact(ContactEntry entry)
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

                    string sql = "UPDATE contacts SET name = @name, lastname = @lastname, phone_number = @phone_number, email = @email, iv_name = @iv_name, iv_lastname = @iv_lastname, iv_phonenumber = @iv_phonenumber, iv_email = @iv_email WHERE id = @id";

                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        // Szyfrowanie
                        var (encryptedName, ivName) = AES.Encrypt(entry.Name);
                        var (encryptedLastName, ivLastName) = AES.Encrypt(entry.LastName);
                        var (encryptedPhoneNumber, ivPhoneNumber) = AES.Encrypt(entry.EncryptedPhoneNumber);
                        var (encryptedEmail, ivEmail) = AES.Encrypt(entry.EncryptedEmail);

                        command.Parameters.AddWithValue("@name", encryptedName);
                        command.Parameters.AddWithValue("@lastname", encryptedLastName);
                        command.Parameters.AddWithValue("@phone_number", encryptedPhoneNumber);
                        command.Parameters.AddWithValue("@email", encryptedEmail);
                        command.Parameters.AddWithValue("@iv_name", ivName);
                        command.Parameters.AddWithValue("@iv_lastname", ivLastName);
                        command.Parameters.AddWithValue("@iv_phonenumber", ivPhoneNumber);
                        command.Parameters.AddWithValue("@iv_email", ivEmail);
                        command.Parameters.AddWithValue("@id", entry.Id);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            System.Windows.MessageBox.Show("Nie znaleziono rekordu do zaktualizowania.");
                        }
                    }
                }

                LoadContacts();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas dodawania danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditNote_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntry = (NoteEntry)((Wpf.Ui.Controls.Button)sender).Tag;

            if (selectedEntry == null)
            {
                System.Windows.MessageBox.Show("Nie znaleziono wybranej notatki do edycji.", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                return; // Przerwij dalsze wykonywanie metody
            }

            // Załaduj dane do pól tekstowych dialogu
            editNoteTitleTextBox.Text = selectedEntry.Name;

            // Odszyfruj tekst przed wyświetleniem
            string decryptedText = AES.Decrypt(selectedEntry.EncryptedText, selectedEntry.IV); // Zmienna IVText musi być dostępna
                                                                                               // Wyczyść zawartość RichTextBox przed ustawieniem nowego tekstu
            editNoteContentTextBox.Document.Blocks.Clear();

            // Ustaw odszyfrowany tekst jako zawartość RichTextBox
            var flowDoc = new FlowDocument();
            var paragraph = new Paragraph
            {
                FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
                FontSize = 14
            };
            paragraph.Inlines.Add(new Run(decryptedText));
            flowDoc.Blocks.Add(paragraph);
            editNoteContentTextBox.Document = flowDoc;

            // Otwórz dialog edycji
            editNoteDialog.Visibility = Visibility.Visible;

            // Przechowaj wpis, aby użyć go podczas zapisu zmian
            editNoteDialog.Tag = selectedEntry;
        }



        private async void EditNoteDialog_ButtonClicked(object sender, RoutedEventArgs e)
        {
            var editedEntry = (NoteEntry)editNoteDialog.Tag;

            // Zaktualizuj wpis na podstawie danych z formularza
            editedEntry.Name = editNoteTitleTextBox.Text;

            // Pobierz zawartość z RichTextBox i zapisz jako zwykły tekst
            TextRange textRange = new TextRange(editNoteContentTextBox.Document.ContentStart, editNoteContentTextBox.Document.ContentEnd);
            editedEntry.EncryptedText = textRange.Text.Trim(); // Pobieranie zawartości RichTextBox

            // Wywołaj metodę aktualizującą wpis w bazie danych
            EditNote(editedEntry);

            // Zamknij dialog po zapisaniu zmian
            editNoteDialog.Visibility = Visibility.Collapsed;

            // Odśwież widok listy
            LoadNotes();

            // Zapisz zmiany z tymczasowej bazy do oryginalnej
            await SaveChangesToOriginalDatabase(Session.Instance.OriginalDatabasePath);
        }

        private void EditNote(NoteEntry entry)
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

                    string sql = "UPDATE notes SET name = @name, text = @text, iv_name = @iv_name, iv = @iv WHERE id = @id";

                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        // Szyfruj nazwę notatki przed zapisaniem
                        var (encryptedName, ivName) = AES.Encrypt(entry.Name);

                        // Pobierz zawartość z RichTextBox i zapisz jako zaszyfrowany tekst
                        TextRange textRange = new TextRange(editNoteContentTextBox.Document.ContentStart, editNoteContentTextBox.Document.ContentEnd);
                        var (encryptedText, ivText) = AES.Encrypt(textRange.Text.Trim());  // Szyfruj tekst

                        command.Parameters.AddWithValue("@name", encryptedName);
                        command.Parameters.AddWithValue("@text", encryptedText);
                        command.Parameters.AddWithValue("@iv_name", ivName);  // Zapisz IV dla nazwy
                        command.Parameters.AddWithValue("@iv", ivText);  // Zapisz IV dla tekstu
                        command.Parameters.AddWithValue("@id", entry.Id);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            System.Windows.MessageBox.Show("Nie znaleziono rekordu do zaktualizowania.");
                        }
                    }
                }

                LoadNotes();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas aktualizacji danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenEditDialog(int id, string tableName)
        {
            // Implementation for opening an edit dialog based on the tableName
            // You will need to create corresponding edit dialogs and methods
        }

        private string GetSelectedTableName()
        {
            if (CreditCardsToggle.IsChecked == true)
                return "credit_card";
            if (ContactsToggle.IsChecked == true)
                return "contacts";
            if (NotesToggle.IsChecked == true)
                return "notes";
            throw new InvalidOperationException("No table is selected");
        }

        private async void DeleteCreditCard_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Wpf.Ui.Controls.Button;
            var creditCard = button?.Tag as CreditCardEntry;

            if (creditCard == null)
            {
                System.Windows.MessageBox.Show("Nie można znaleźć powiązanego elementu!");
                return;
            }

            string dbPath = Session.Instance.DatabasePath;
            string originalDbPath = Session.Instance.OriginalDatabasePath;

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

                    string query = "DELETE FROM credit_card WHERE Id = @Id";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", creditCard.Id);
                        var result = command.ExecuteNonQuery();  // Wykonanie zapytania
                    }

                    connection.Close();
                }

                LoadCreditCards();  // Odśwież dane po usunięciu
                await SaveChangesToOriginalDatabase(originalDbPath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas usuwania danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void DeleteContact_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Wpf.Ui.Controls.Button;
            var contact = button?.Tag as ContactEntry;
            string originalDbPath = Session.Instance.OriginalDatabasePath;


            if (contact != null)
            {
                string dbPath = Session.Instance.DatabasePath;

                try
                {
                    using (SQLiteConnection connection = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                    {
                        connection.Open();

                        string query = "DELETE FROM contacts WHERE Id = @Id";
                        using (SQLiteCommand command = new SQLiteCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@Id", contact.Id);
                            command.ExecuteNonQuery();
                        }

                        connection.Close();
                    }

                    LoadContacts();  // Odśwież dane po usunięciu
                    await SaveChangesToOriginalDatabase(originalDbPath);
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Wystąpił błąd podczas usuwania danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void DeleteNotes_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Wpf.Ui.Controls.Button;
            var note = button?.Tag as NoteEntry; // Upewnij się, że NoteEntry jest poprawną klasą
            string originalDbPath = Session.Instance.OriginalDatabasePath;


            if (note == null)
            {
                System.Windows.MessageBox.Show("Nie można znaleźć powiązanego elementu!");
                return;
            }

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

                    string query = "DELETE FROM notes WHERE Id = @Id"; // Upewnij się, że to jest poprawna tabela
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", note.Id); // Upewnij się, że Id jest poprawnie przekazywane
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }

                LoadNotes(); // Odśwież dane po usunięciu
                await SaveChangesToOriginalDatabase(originalDbPath);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Wystąpił błąd podczas usuwania danych: {ex.Message}", "Błąd", System.Windows.MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowNote_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Wpf.Ui.Controls.Button;
            if (button != null)
            {
                var selectedNote = (NoteEntry)button.Tag;
                selectedNote.IsTextVisible = !selectedNote.IsTextVisible;  // Przełącz widoczność tekstu
            }
        }

        private void ShowContact_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Wpf.Ui.Controls.Button;
            if (button != null)
            {
                var selectedContact = (ContactEntry)button.Tag;  // Pobierz wybrany kontakt
                selectedContact.IsPhoneNumberVisible = !selectedContact.IsPhoneNumberVisible;  // Przełącz widoczność numeru telefonu
            }
        }

        private void ShowEmail_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.Tag is ContactEntry contact)
            {
                contact.IsEmailVisible = !contact.IsEmailVisible;
            }
        }

        private void ShowCardNumber_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.Tag is CreditCardEntry cardEntry)
            {
                cardEntry.IsCardNumberVisible = !cardEntry.IsCardNumberVisible;
            }
        }

        private void ShowDate_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.Tag is CreditCardEntry cardEntry)
            {
                cardEntry.IsDateVisible = !cardEntry.IsDateVisible;
            }
        }

        private void ShowCVV_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.Tag is CreditCardEntry cardEntry)
            {
                cardEntry.IsCvvVisible = !cardEntry.IsCvvVisible;
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

        public void EncryptAndReplaceDatabase(string tempDbPath, string originalDbPath, string password)
        {
            byte[] key, iv;
            byte[] salt = GetSaltFromEncryptedDb(originalDbPath);

            using (var keyGenerator = new Rfc2898DeriveBytes(password, salt))
            {
                key = keyGenerator.GetBytes(32);
                iv = keyGenerator.GetBytes(16);
            }

            string tempEncryptedFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");


            try
            {
                using (var tempFileStream = new FileStream(tempDbPath, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    byte[] fileData = new byte[tempFileStream.Length];
                    tempFileStream.Read(fileData, 0, fileData.Length);

                    using (var tempEncryptedFileStream = new FileStream(tempEncryptedFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (var encryptor = Aes.Create().CreateEncryptor(key, iv))
                        using (var cryptoStream = new CryptoStream(tempEncryptedFileStream, encryptor, CryptoStreamMode.Write))
                        {
                            tempEncryptedFileStream.Write(salt, 0, salt.Length);
                            cryptoStream.Write(fileData, 0, fileData.Length);
                        }
                    }
                }



                // Replace the original file

                AttemptFileReplace(originalDbPath, tempEncryptedFilePath);

                Utilities.LogDebug($"Baza danych '{originalDbPath}' została zaszyfrowana i nadpisana.");

            }
            catch (Exception ex)
            {
                Utilities.LogDebug($"Błąd podczas szyfrowania bazy danych: {ex.Message}");
                throw;
            }
            finally
            {
                if (File.Exists(tempEncryptedFilePath))
                {
                    File.Delete(tempEncryptedFilePath);
                }
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
        public void EncryptDatabase(string tempDbPath, string originalDbPath, string password)
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

    //------------------------------------------------------------------------------------------------------- MODELS <START> ---------------------------------------------------------------------------------//
    public class CreditCardEntry : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EncryptedCardNumber { get; set; }
        public string EncryptedDate { get; set; }
        public string EncryptedCvv { get; set; }

        public string IVCardNumber { get; set; }
        public string IVCVV { get; set; }
        public string IVDate { get; set; }

        private bool _isCardNumberVisible;
        private bool _isDateVisible;
        private bool _isCvvVisible;

        // Numer karty: ukrywa wszystko oprócz ostatnich czterech cyfr, dodaje spacje co 4 cyfry
        public string DisplayCardNumber => _isCardNumberVisible
            ? FormatCardNumber(AES.Decrypt(EncryptedCardNumber, IVCardNumber))
            : FormatCardNumber(MaskCardNumber(AES.Decrypt(EncryptedCardNumber, IVCardNumber)));

        // Data karty: ukrywa miesiąc i rok
        public string DisplayDate => _isDateVisible
            ? AES.Decrypt(EncryptedDate, IVDate)
            : MaskDate(AES.Decrypt(EncryptedDate, IVDate));

        // CVV: ukryty za gwiazdkami
        public string DisplayCvv => _isCvvVisible
            ? AES.Decrypt(EncryptedCvv, IVCVV)
            : new string('*', 3);

        public bool IsCardNumberVisible
        {
            get => _isCardNumberVisible;
            set
            {
                _isCardNumberVisible = value;
                OnPropertyChanged(nameof(DisplayCardNumber));
            }
        }

        public bool IsDateVisible
        {
            get => _isDateVisible;
            set
            {
                _isDateVisible = value;
                OnPropertyChanged(nameof(DisplayDate));
            }
        }

        public bool IsCvvVisible
        {
            get => _isCvvVisible;
            set
            {
                _isCvvVisible = value;
                OnPropertyChanged(nameof(DisplayCvv));
            }
        }

        private string MaskCardNumber(string cardNumber)
        {
            // Ukryj wszystkie cyfry z wyjątkiem ostatnich 4
            return cardNumber.Length > 4
                ? new string('*', cardNumber.Length - 4) + cardNumber[^4..]
                : new string('*', cardNumber.Length);
        }

        private string FormatCardNumber(string cardNumber)
        {
            // Dodaje spacje co 4 cyfry
            return string.Join(" ", Enumerable.Range(0, cardNumber.Length / 4)
                                              .Select(i => cardNumber.Substring(i * 4, Math.Min(4, cardNumber.Length - i * 4))));
        }

        private string MaskDate(string date)
        {
            // Maskuje datę w formacie **/**
            if (date.Length == 5 && date.Contains("/"))
            {
                return "**/**";
            }
            return new string('*', date.Length);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ContactEntry : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string EncryptedPhoneNumber { get; set; }
        public string EncryptedEmail { get; set; }

        public string IVName { get; set; }
        public string IVLastName { get; set; }
        public string IVPhoneNumber { get; set; }
        public string IVEmail { get; set; }

        // Zmienna kontrolująca widoczność numeru telefonu
        private bool _isPhoneNumberVisible;
        private bool _isEmailVisible;

        // Zwraca odszyfrowany numer telefonu lub gwiazdki
        public string DisplayPhoneNumber => _isPhoneNumberVisible ? AES.Decrypt(EncryptedPhoneNumber, IVPhoneNumber) : new string('*', 10);  // Odszyfrowany numer lub gwiazdki

        public string DisplayEmail
        {
            get
            {
                if (_isEmailVisible)
                {
                    return AES.Decrypt(EncryptedEmail, IVEmail); // Pełny, odszyfrowany e-mail
                }
                else
                {
                    var decryptedEmail = AES.Decrypt(EncryptedEmail, IVEmail);
                    var atIndex = decryptedEmail.IndexOf('@');
                    return atIndex > 0
                        ? new string('*', atIndex) + decryptedEmail.Substring(atIndex)
                        : new string('*', decryptedEmail.Length);
                }
            }
        }

        public bool IsPhoneNumberVisible
        {
            get => _isPhoneNumberVisible;
            set
            {
                _isPhoneNumberVisible = value;
                OnPropertyChanged(nameof(DisplayPhoneNumber));  // Powiadamiamy, że DisplayPhoneNumber się zmieniło
            }
        }

        public bool IsEmailVisible
        {
            get => _isEmailVisible;
            set
            {
                _isEmailVisible = value;
                OnPropertyChanged(nameof(DisplayEmail));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    public class NoteEntry : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EncryptedText { get; set; }
        public string IV { get; set; }
        public string IVName { get; set; }

        // Zmienna do kontrolowania widoczności tekstu
        private bool _isTextVisible;

        // Tekst, który wyświetlamy w RichTextBox
        public string DisplayText => _isTextVisible ? AES.Decrypt(EncryptedText, IV) : new string('*', 10);  // Odszyfrowany tekst lub gwiazdki

        public bool IsTextVisible
        {
            get => _isTextVisible;
            set
            {
                _isTextVisible = value;
                OnPropertyChanged(nameof(DisplayText));  // Powiadamiamy, że DisplayText się zmieniło
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void LogDebug(string message)
        {
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "debug_log.txt");
            File.AppendAllText(logPath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
    }
}
//------------------------------------------------------------------------------------------------------- MODELS <END> ---------------------------------------------------------------------------------//