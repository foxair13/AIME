using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.SV.Services
{
    public class CryptoService
    {
        private readonly IConfiguration _configuration;
        private readonly string _encryptionKey;
        public static string GetToken()
        {
            return new CipherService().Encode();
        }

        public CryptoService(IConfiguration configuration)
        {
            _configuration = configuration;
            _encryptionKey = configuration["EncryptionKey"]; // You should provide a secure way to store and retrieve the key
        }
        public IConfiguration GetConfiguration()
        {
            return _configuration;
        }

        private string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public string Encrypt(string text)
        {
            using (Aes aesAlg = Aes.Create())
            {
                var cipher = new VigenereService();
                aesAlg.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(_encryptionKey.Substring(0, 16));

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }

                    return  cipher.Encrypt(Base64UrlEncode(msEncrypt.ToArray()), _encryptionKey); 
                }
            }
        }

        public void EncryptConnectionsInConfig()
        {
            var connectionsSection = _configuration.GetSection("Connections");
            foreach (var connection in connectionsSection.GetChildren())
            {
                var encryptedValue = Encrypt(connection.Value);
                connectionsSection[connection.Key] = encryptedValue;
            }
        }
        public string Hit(string cipherText)
        {
            var aesDecrypted = new VigenereService().Decrypt(cipherText, _encryptionKey);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                aesAlg.IV = Encoding.UTF8.GetBytes(_encryptionKey.Substring(0, 16));

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                byte[] encryptedBytes = Base64UrlDecode(aesDecrypted);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        public IConfiguration HitConnectionsInConfig()
        {
            var connectionsSection = _configuration.GetSection("Connections");
            foreach (var connection in connectionsSection.GetChildren())
            {
                var decryptedValue = Hit(connection.Value);
                connectionsSection[connection.Key] = decryptedValue;
            }
            return connectionsSection;
        }

       

        private byte[] Base64UrlDecode(string input)
        {

            input = input.Replace('-', '+').Replace('_', '/');


            int padding = input.Length % 4;
            if (padding > 0)
            {
                input = input.PadRight(input.Length + (4 - padding), '=');
            }

            return Convert.FromBase64String(input);
        }



    }
}
