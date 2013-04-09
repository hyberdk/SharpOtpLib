using System.Globalization;
using NUnit.Framework;
using SharpOtpLib;
using System;

namespace SharpOtpLibTests
{
    
    
    /// <summary>
    ///This is a test class for UtilsTest and is intended
    ///to contain all UtilsTest Unit Tests
    ///</summary>
    [TestFixture]
    public class UtilsTest
    {

        [Test]
        public void GenerateStringSecretTest()
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            int length = 25;
            int expectedLength = length;
            string secret = Utils.GenerateSecret(length);
            string secret2 = Utils.GenerateSecret(length);
            
            Assert.AreNotEqual(secret, secret2); //test that 2 secrets are not equal

            foreach (char c in secret) //test that char is base32 complient.
            {
                Assert.IsTrue(alphabet.Contains(c.ToString(CultureInfo.InvariantCulture)));
            }

            Console.WriteLine("Generated string: " + secret);
            Assert.AreEqual(expectedLength, secret.Length);
        }

        [Test]
        public void GenerateByteSecretTest()
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            byte[] secret = Utils.GenerateSecret();
            byte[] secret2 = Utils.GenerateSecret();
            Assert.AreEqual(secret.Length, 20); //test that we get 20 bytes back
            Assert.AreNotEqual(secret, secret2); //test that we get 2 different secrets back

            string actual = Base32.Convert(secret);

            Assert.AreEqual(actual.Length, 32); //test that 32 characters are returned.

            foreach (char c in actual) //test that char is base32 complient.
            {
                Assert.IsTrue(alphabet.Contains(c.ToString(CultureInfo.InvariantCulture)));
            }

        }
    }
}
