using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace SharpOtpLib
{
    /// <summary>
    /// This class holds misc utilities that did not fit elsewhere.
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// This uses the Crytographic Random Number Generator to generate a unique
        /// random base32 complient secret
        /// </summary>
        /// <param name="length">length of secret</param>
        /// <returns>Base32 complient secret as string</returns>
        public static string GenerateSecret(int length)
        {

            var alphabet = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "2", "3", "4", "5", "6", "7" };
            var randomString = "";
            var byteSeed = new byte[length];

            using (var rng = new RNGCryptoServiceProvider())
            {
                //Get crytographic random bytes
                rng.GetBytes(byteSeed);
            }           

            //use the first 5bits (0x1f) of each byte to get a base32 character.
            for (int i = 0; i < length; i++)
            {
                int number = byteSeed[i] & 0x1f;
                randomString += alphabet[number];
            }

            return randomString;
        }

        /// <summary>
        /// This uses the Crytographic Random Number Generator to generate a unique
        /// random base32 complient secret. The secret is 160bits long as recommended by
        /// RFC4226.
        /// </summary>
        /// <returns>Random secret as byte array</returns>
        public static byte[] GenerateSecret()
        {
            var byteSeed = new byte[20];

            using (var rng = new RNGCryptoServiceProvider())
            {
                
                //Get crytographic random bytes
                rng.GetBytes(byteSeed);
            }
            return byteSeed;
        }

        /// <summary>
        /// Checks a string if it is Base32 valid (does not contain other than
        /// A-Z, a-z and 2-7
        /// </summary>
        /// <param name="key">private key to check</param>
        /// <returns>true if valid base32 string</returns>
        internal static bool IsBase32Valid(string key)
        {
            //Only match Base32 charaters, see http://en.wikipedia.org/wiki/Base32

            //if key contains other characters than a-z and 2-7, return false.
            return String.IsNullOrEmpty(Regex.Match(key, "(?i)(?![a-z2-7]).").ToString());
        }

        /// <summary>
        /// Generates a new random 256bit AES key.
        /// This can be used for the encryption of keys in the HOTP/TOTP classes
        /// </summary>
        /// <returns>256bit AES key</returns>
        public static byte[] GenerateNewAesKey()
        {
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.KeySize = 256;
                aesAlg.GenerateKey();
                return aesAlg.Key;
            }
        }

        /// <summary>
        /// Generates a new random Initialization vector. 
        /// This can be used for the encryption of keys in the HOTP/TOTP classes
        /// </summary>
        /// <returns>returns a new Initialization vector</returns>
        public static byte[] GenerateNewAesIv()
        {
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.GenerateIV();
                return aesAlg.IV;
            }
        }


        internal static byte[] Encrypt(byte[] data, byte[] key, byte[] iv)
        {
            // Check arguments. 
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException("data");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");

            byte[] encrypted;
            // Create an AesManaged object 
            // with the specified key and IV. 
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption. 
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (BinaryWriter swEncrypt = new BinaryWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(data);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }


            // Return the encrypted bytes from the memory stream. 
            return encrypted;

        }

        internal static byte[] Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            // Check arguments. 
            if (data == null || data.Length <= 0)
                throw new ArgumentNullException("data");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException("iv");


            // Create an AesManaged object 
            // with the specified key and IV. 
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption. 
                using (MemoryStream msDecrypt = new MemoryStream(data)) //convery byte array to stream
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) //descrypt stream
                    {
                        using (MemoryStream ms = new MemoryStream()) //read descypted stream and convery to byte array.
                        {
                            int read;
                            byte[] buffer = new byte[1];

                            while ((read = csDecrypt.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                ms.Write(buffer, 0, read);
                            }
                            return ms.ToArray();
                        }
                    }
                }

            }
        }
    }
}
