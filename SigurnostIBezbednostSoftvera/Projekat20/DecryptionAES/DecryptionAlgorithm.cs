using System.Text;
using System.Security.Cryptography;
using System.IO;
using System;

namespace DecryptionAES
{
    public class DecryptionAlgorithm
    {
        public static string DecryptMessage(string encryptedMessage, string secretKey)
        {
            string decryptedMessage = null;
            byte[] message = Convert.FromBase64String(encryptedMessage);

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = Convert.FromBase64String(secretKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform aesDecryptTransform = aesCryptoProvider.CreateDecryptor();
            using (MemoryStream memoryStream = new MemoryStream(message))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptTransform, CryptoStreamMode.Read))
                {
                    using(StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        decryptedMessage = streamReader.ReadToEnd();
                    }
                }
            }

            return decryptedMessage;
        }
    }
}
