using NUnit.Framework;
using System;
using System.IO;
using BCrypt.Net;

namespace CerberPass.Tests
{
    [TestFixture]
    public class LoginTests
    {
        private const string TestPassword = "TestPassword123";
        private string _tempEncryptedDbPath;
        private string _tempDecryptedDbPath;

        [SetUp]
        public void Setup()
        { 
            // Tworzenie zaszyfrowanej bazy danych testowej
            _tempEncryptedDbPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");
            _tempDecryptedDbPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.db");

            // Hashowanie testowego has³a przy u¿yciu BCrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(TestPassword);

            // Szyfrowanie bazy danych
            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();

                byte[] salt = new byte[16];
                new Random().NextBytes(salt);

                // Generowanie klucza i wektora inicjalizacyjnego
                var keyGenerator = new System.Security.Cryptography.Rfc2898DeriveBytes(TestPassword, salt);
                byte[] key = keyGenerator.GetBytes(32);
                byte[] iv = keyGenerator.GetBytes(16);

                // Zapisanie zaszyfrowanych danych do pliku
                using (var fileStream = new FileStream(_tempEncryptedDbPath, FileMode.Create, FileAccess.Write))
                {
                    fileStream.Write(salt, 0, salt.Length);

                    using (var encryptor = aes.CreateEncryptor(key, iv))
                    using (var cryptoStream = new System.Security.Cryptography.CryptoStream(fileStream, encryptor, System.Security.Cryptography.CryptoStreamMode.Write))
                    {
                        string testData = $"CREATE TABLE password (password TEXT); INSERT INTO password (password) VALUES ('{hashedPassword}');";
                        byte[] data = System.Text.Encoding.UTF8.GetBytes(testData);
                        cryptoStream.Write(data, 0, data.Length);
                    }
                }
            }
        }

        [Test]
        public void ValidateAndDecryptDatabase_ValidPassword_ReturnsTrue()
        {
            // Arrange
            var login = new Login();

            // Act
            bool isValid = login.ValidateAndDecryptDatabase(_tempEncryptedDbPath, TestPassword);

            // Assert
            NUnit.Framework.Assert.That(isValid, Is.True, "Metoda powinna zwróciæ true dla poprawnego has³a.");
        }

        [Test]
        public void ValidateAndDecryptDatabase_InvalidPassword_ReturnsFalse()
        {
            // Arrange
            var login = new Login();

            // Act
            bool isValid = login.ValidateAndDecryptDatabase(_tempEncryptedDbPath, "WrongPassword");

            // Assert
            NUnit.Framework.Assert.That(isValid, Is.False, "Metoda powinna zwróciæ false dla niepoprawnego has³a.");
        }

        [TearDown]
        public void TearDown()
        {
            // Czyszczenie plików tymczasowych
            if (File.Exists(_tempEncryptedDbPath)) File.Delete(_tempEncryptedDbPath);
            if (File.Exists(_tempDecryptedDbPath)) File.Delete(_tempDecryptedDbPath);
        }
    }

    public class Login
    {
        public bool ValidateAndDecryptDatabase(string encryptedDbPath, string password)
        {
            try
            {
                using (var fileStream = new FileStream(encryptedDbPath, FileMode.Open, FileAccess.Read))
                {
                    // Odczytaj sól z pliku
                    byte[] salt = new byte[16];
                    fileStream.Read(salt, 0, salt.Length);

                    // Wygeneruj klucz i IV z has³a
                    var keyGenerator = new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt);
                    byte[] key = keyGenerator.GetBytes(32);  // Klucz AES (256-bit)
                    byte[] iv = keyGenerator.GetBytes(16);   // IV AES (128-bit)

                    // Dekryptowanie danych
                    using (var aes = System.Security.Cryptography.Aes.Create())
                    {
                        aes.Key = key;
                        aes.IV = iv;

                        using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                        using (var cryptoStream = new System.Security.Cryptography.CryptoStream(fileStream, decryptor, System.Security.Cryptography.CryptoStreamMode.Read))
                        {
                            using (var reader = new StreamReader(cryptoStream))
                            {
                                string decryptedData = reader.ReadToEnd();

                                // SprawdŸ, czy dane zawieraj¹ has³o
                                return decryptedData.Contains("password");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Logowanie b³êdu (jeœli wyst¹pi)
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
