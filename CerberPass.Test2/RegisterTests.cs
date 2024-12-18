using CerberPass.Views;
using NUnit.Framework;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CerberPass.Tests
{
    [TestFixture]
    public class RegisterTests
    {
        private Register _register;
        private string _logFilePath;

        [SetUp]
        public void Setup()
        {
            _register = new Register();
            _logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "debug_log.txt");

            // Upewnij siê, ¿e plik logu jest czysty przed ka¿dym testem
            if (File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
            }
        }

        [Test]
        public void CreateDatabase_Click_ShouldLogMessage_WhenPasswordsDoNotMatch()
        {
            // Arrange
            var passwordBox = new PasswordBox { Password = "password123" };
            var repeatPasswordBox = new PasswordBox { Password = "password124" };
            var dbNameTextBox = new TextBox { Text = "TestDB" };

            // Ustawienie kontrolki w obiekcie Register
            _register.GetType().GetProperty("passwordBox").SetValue(_register, passwordBox);
            _register.GetType().GetProperty("repeatPasswordBox").SetValue(_register, repeatPasswordBox);
            _register.GetType().GetProperty("dbNameText Box").SetValue(_register, dbNameTextBox);

            // Act
            _register.CreateDatabase_Click(null, null);

            // Assert
            string logContent = File.ReadAllText(_logFilePath);
            Assert.IsTrue(logContent.Contains("Has³a nie s¹ takie same. Spróbuj ponownie."));
        }

        [Test]
        public void CreateDatabase_Click_ShouldLogMessage_WhenDatabaseNameIsEmpty()
        {
            // Arrange
            var passwordBox = new PasswordBox { Password = "password123" };
            var repeatPasswordBox = new PasswordBox { Password = "password123" };
            var dbNameTextBox = new TextBox { Text = "" }; // Pusta nazwa bazy danych

            // Ustawienie kontrolki w obiekcie Register
            _register.GetType().GetProperty("passwordBox").SetValue(_register, passwordBox);
            _register.GetType().GetProperty("repeatPasswordBox").SetValue(_register, repeatPasswordBox);
            _register.GetType().GetProperty("dbNameTextBox").SetValue(_register, dbNameTextBox);

            // Act
            _register.CreateDatabase_Click(null, null);

            // Assert
            string logContent = File.ReadAllText(_logFilePath);
            Assert.IsTrue(logContent.Contains("Proszê wprowadŸ nazwê bazy danych."));
        }

        [TearDown]
        public void TearDown()
        {
            // Upewnij siê, ¿e plik logu jest usuwany po ka¿dym teœcie
            if (File.Exists(_logFilePath))
            {
                File.Delete(_logFilePath);
            }
        }
    }
}