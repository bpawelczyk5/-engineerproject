using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace CerberPass.Views
{
    public partial class PassGen : UserControl
    {
        public PassGen()
        {
            InitializeComponent();
            GeneratePassword(); // Automatyczne generowanie hasła przy otwarciu zakładki
        }

        private void Button_Accept(object sender, RoutedEventArgs e)
        {
            // Kopiowanie wygenerowanego hasła do schowka
            Clipboard.SetText(PasswordTextBox.Text);
            System.Windows.MessageBox.Show("Hasło skopiowane do schowka!");
        }

        private void PasswordTextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Kopiowanie hasła po kliknięciu na TextBox
            Clipboard.SetText(PasswordTextBox.Text);
            System.Windows.MessageBox.Show("Hasło skopiowane do schowka!");
        }

        private void RefreshPasswordIcon_Click(object sender, RoutedEventArgs e)
        {
            // Generowanie nowego hasła po kliknięciu na ikonę odświeżania
            GeneratePassword();
        }

        private void PasswordLengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PasswordLengthSlider.IsEnabled)
            {
                // Aktualizacja wygenerowanego hasła na podstawie długości
                GeneratePassword();
            }
        }

        private void GeneratePassword()
        {
            StringBuilder password = new StringBuilder();
            Random random = new Random();
            int length = (int)PasswordLengthSlider.Value;

            string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lower = "abcdefghijklmnopqrstuvwxyz";
            string digits = "0123456789";
            string special1 = "!@#$%^&*()";
            string punctuation = ";:'<>,.?+_{}=-`";

            string characterPool = "";

            if (ToggleUppercase.IsChecked == true) characterPool += upper;
            if (ToggleLowercase.IsChecked == true) characterPool += lower;
            if (ToggleNumbers.IsChecked == true) characterPool += digits;
            if (ToggleSpecialChars1.IsChecked == true) characterPool += special1;
            if (TogglePunctuation.IsChecked == true) characterPool += punctuation;

            // Sprawdzenie, czy jest zaznaczona przynajmniej jedna opcja
            if (characterPool.Length == 0)
            {
                PasswordTextBox.Text = "";
                PasswordLengthSlider.IsEnabled = false; // Blokowanie suwaka długości
                System.Windows.MessageBox.Show("Wybierz przynajmniej jedną opcję, aby wygenerować hasło.");
                SelectAllToggleButtons(); // Zaznaczenie wszystkich ToggleButtons
                return;
            }
            else
            {
                PasswordLengthSlider.IsEnabled = true; // Odblokowywanie suwaka
            }

            // Generowanie hasła
            if (length > 0 && characterPool.Length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    password.Append(characterPool[random.Next(characterPool.Length)]);
                }
            }

            PasswordTextBox.Text = password.ToString();
            UpdatePasswordStrength(password.ToString());
        }

        private void UpdatePasswordStrength(string password)
        {
            int score = 0;
            if (password.Length >= 8) score += 20;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]")) score += 20;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]")) score += 20;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[0-9]")) score += 20;
            if (System.Text.RegularExpressions.Regex.IsMatch(password, @"[\W_]")) score += 20;

            PasswordStrengthSlider.Value = score;
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            ResetUI();
            //System.Windows.MessageBox.Show("Odśwież hasło!", "Informacja", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ResetUI()
        {
            // Resetowanie wszystkich przełączników (ToggleButtons)
            // Resetowanie wszystkich przełączników (ToggleButtons)
            ToggleUppercase.IsChecked = false;
            ToggleLowercase.IsChecked = false;
            ToggleNumbers.IsChecked = false;
            ToggleSpecialChars1.IsChecked = false;
            TogglePunctuation.IsChecked = false;

            // Resetowanie suwaków (Sliders)
            PasswordLengthSlider.Value = 0;
            PasswordStrengthSlider.Value = 0;

            // Wyczyść pole z wygenerowanym hasłem (TextBox)
            PasswordTextBox.Clear();

            // Zostaw suwak odblokowany, aby użytkownik mógł go używać po wybraniu opcji
        }

        private void SelectAllToggleButtons()
        {
            // Zaznaczenie wszystkich przełączników (ToggleButtons)
            ToggleUppercase.IsChecked = true;
            ToggleLowercase.IsChecked = true;
            ToggleNumbers.IsChecked = true;
            ToggleSpecialChars1.IsChecked = true;
            TogglePunctuation.IsChecked = true;

            // Odblokowanie suwaka długości
            PasswordLengthSlider.IsEnabled = true;
        }

        private void ToggleOptionChanged(object sender, RoutedEventArgs e)
        {
            // Sprawdzenie, czy przynajmniej jedna opcja jest zaznaczona
            if (ToggleUppercase.IsChecked == true || ToggleLowercase.IsChecked == true ||
                ToggleNumbers.IsChecked == true || ToggleSpecialChars1.IsChecked == true ||
                TogglePunctuation.IsChecked == true)
            {
                PasswordLengthSlider.IsEnabled = true; // Odblokowanie suwaka długości
            }
            else
            {
                PasswordLengthSlider.IsEnabled = false; // Blokowanie suwaka długości
            }
        }

    }
}
