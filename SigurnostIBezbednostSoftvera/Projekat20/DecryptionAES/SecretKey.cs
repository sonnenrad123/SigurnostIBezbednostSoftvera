using System;
using System.Security.Cryptography;
using System.Text;

namespace DecryptionAES
{
    public class SecretKey
    {
        public static string sKey = "";
        public static void GenerateKey()
        {
            SymmetricAlgorithm symmAlgorithm = null;

            symmAlgorithm = AesCryptoServiceProvider.Create();

            sKey = Convert.ToBase64String(symmAlgorithm.Key);
        }

    }
}
