using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace EncryptionAES
{
    public class EncryptionAlgorithm
    {
        public static string EncryptMesssage(string messageToEncrypt, string secretKey)
        {
            byte[] encryptedBytes = null;
            string encryptedMessage = null;

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = Convert.FromBase64String(secretKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform aesEncryptTransform = aesCryptoProvider.CreateEncryptor();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptTransform, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(messageToEncrypt);
                    }

                    encryptedBytes = memoryStream.ToArray();
                    encryptedMessage = Convert.ToBase64String(encryptedBytes);
                }
            }


            return encryptedMessage;
        }
    }
}
