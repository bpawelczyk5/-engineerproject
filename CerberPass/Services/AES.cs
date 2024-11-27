using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AES
{
    // Stały klucz AES
    private static readonly byte[] Key = Convert.FromBase64String("ahFc5YfGRJ84e+v+PGZy3zI8+Js7SxaZEpdbYYzol0A=");

    // Metoda szyfrowania, zwraca zaszyfrowany tekst oraz IV
    public static (string EncryptedText, string IV) Encrypt(string plainText)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Key;
            aes.GenerateIV();  // Generuje unikalny IV dla każdego rekordu
            byte[] iv = aes.IV;

            using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, iv))
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (StreamWriter writer = new StreamWriter(cs))
            {
                writer.Write(plainText);
                writer.Close();
                return (Convert.ToBase64String(ms.ToArray()), Convert.ToBase64String(iv)); // Zwraca zaszyfrowany tekst i IV jako Base64
            }
        }
    }

    // Metoda deszyfrowania przyjmuje zaszyfrowany tekst oraz IV jako argumenty
    public static string Decrypt(string cipherText, string ivBase64)
    {
        try
        {
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            byte[] iv = Convert.FromBase64String(ivBase64);  // Dekoduje IV z Base64
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = iv;

                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (MemoryStream ms = new MemoryStream(cipherBytes))
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (StreamReader reader = new StreamReader(cs))
                {
                    return reader.ReadToEnd(); // Zwraca odszyfrowany tekst
                }
            }
        }
        catch (CryptographicException ex)
        {
            throw new Exception("Deszyfrowanie nie powiodło się: " + ex.Message);
        }
    }
}
