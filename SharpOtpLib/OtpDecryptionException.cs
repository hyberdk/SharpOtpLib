using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpOtpLib
{
    /// <summary>
    /// This throws if there is a problem decrypting data from the xml "database"
    /// This is usually due to one of the following reasons. 1. Wrong key and vector is used
    /// 2. Someone tampered with the encrypted data, hence breaking the encryption.
    /// </summary>
    public class OtpDecryptionException : System.Security.Cryptography.CryptographicException
    {
        
        /// <summary>
        /// Throws if decryption failed
        /// </summary>
        /// <param name="message">message for user</param>
        /// <param name="ident">device ident that failed</param>
        public OtpDecryptionException(string message, string ident) : base(message)
        {
            Idents = new[] {ident};
        }

        /// <summary>
        /// Throws if decryption failed
        /// </summary>
        /// <param name="message">message for user</param>
        /// <param name="idents">list of device idents that failed</param>
        public OtpDecryptionException(string message, string[] idents)
            : base(message)
        {
            Idents = idents;
        }

        /// <summary>
        /// Throws if decryption failed
        /// </summary>
        /// <param name="message">message for user</param>
        /// <param name="ident">ident of device that failed</param>
        /// <param name="inner">inner exception, usually a System.Security.Cryptography.CryptographicException</param>
        public OtpDecryptionException(string message, string ident, Exception inner) : base(message, inner)
        {
            Idents = new[] {ident};
        }

        /// <summary>
        /// Throws if decryption failed
        /// </summary>
        /// <param name="message">message for user</param>
        /// <param name="idents">list of ident of devices that has failed</param>
        /// <param name="inner">inner exception, usually a System.Security.Cryptography.CryptographicException</param>
        public OtpDecryptionException(string message, string[] idents, Exception inner)
            : base(message, inner)
        {
            Idents = idents;
        }

        /// <summary>
        /// List of device idents that has failed.
        /// </summary>
        public string[] Idents { get; private set; }

    }
}
