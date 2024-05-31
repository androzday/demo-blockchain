using System.Security.Cryptography;
using System.Xml.Serialization;

namespace demo_blockchain.Utils
{
    public static class RSAUtil
    {
        public static void GenerateRSAKeys(out string publicKey, out string privateKey, int keySize)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
            RSAParameters publicKeyParam = rsa.ExportParameters(false);
            RSAParameters privateKeyParam = rsa.ExportParameters(true);

            publicKey = RSAToString(publicKeyParam);
            privateKey = RSAToString(privateKeyParam);
        }
        public static byte[] Encrypt(byte[] data, string publicKeyXml)
        {
            try
            {
                RSAParameters publicKey = XmlToRSAParameters(publicKeyXml);
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportParameters(publicKey);
                    byte[] encryptedData = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA1);
                    return encryptedData;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            

        }
        public static byte[] Decrypt(byte[] data, string privateKeyXml)
        {
            try
            {
                
                RSAParameters privateKey = XmlToRSAParameters(privateKeyXml);
                using (var rsa = new RSACryptoServiceProvider())
                {
                    rsa.ImportParameters(privateKey);
                    byte[] decryptedData = rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA1);
                    return decryptedData;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        private static string RSAToString(RSAParameters keyRSA)
        {
            StringWriter sw = new StringWriter();
            XmlSerializer xs = new XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, keyRSA);
            return sw.ToString();
        }
        private static RSAParameters XmlToRSAParameters(string keyString)
        {
            // Create a StringReader to read the XML string
            using (StringReader sr = new StringReader(keyString))
            {
                // Create an XmlSerializer instance for RSAParameters
                XmlSerializer xs = new XmlSerializer(typeof(RSAParameters));
                // Deserialize the XML string to RSAParameters object
                return (RSAParameters)xs.Deserialize(sr);
            }
        }
        private static string ConvertToPEM(byte[] data, string type)
        {
            string base64 = Convert.ToBase64String(data);
            string header = $"-----BEGIN {type}-----\n";
            string footer = $"\n-----END {type}-----";

            // Combine header, base64 content, and footer
            return header + base64 + footer;
        }
    }
}
