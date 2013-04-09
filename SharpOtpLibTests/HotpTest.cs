using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using SharpOtpLib;

namespace SharpOtpLibTests
{
    
    
    /// <summary>
    ///This is a test class for HOTPTest and is intended
    ///to contain all HOTPTest Unit Tests
    ///</summary>
    [TestFixture()]
    public class HotpTest
    {

        [Test]
        public void VerifyOneTimePassword()
        {
            
            HashbasedOneTimePassword hotp = new HashbasedOneTimePassword();
            byte[] key = new byte[20];
            for (int i = 0; i < 20; i++) key[i] = 23;
            string ident = "hotp-test";

            hotp.Devices.Add(ident, key);

            OneTimePassword.VerifyResult result = hotp.Verify(ident, "928856");
            Assert.AreEqual(result, OneTimePassword.VerifyResult.Success);

            result = hotp.Verify(ident, "216764");
            Assert.AreEqual(result, OneTimePassword.VerifyResult.Success); 
            
            result = hotp.Verify(ident, "449156");
            Assert.AreEqual(result, OneTimePassword.VerifyResult.Success);
        }


        [Test]
        public void AddContainsRemoveTest()
        {
            HashbasedOneTimePassword hotp = new HashbasedOneTimePassword();
            string[] idents = { "blah", "blah2", "blah3", "hotp-test", "bla4", "blah5", "blah6"};
            byte[] secret = new byte[] { 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23 };

            foreach (string ident in idents)
            {
                hotp.Devices.Add(ident, secret);
            }

            Assert.IsTrue(hotp.Devices.Contains("hotp-test"));

            hotp.Devices.Remove("hotp-test");

            Assert.IsFalse(hotp.Devices.Contains("hotp-test"));

        }

        [Test]
        public void VerifyUsedCodeTest()
        {
            HashbasedOneTimePassword hotp = new HashbasedOneTimePassword();
            byte[] key = new byte[20];
            for (int i = 0; i < 20; i++) key[i] = 23;
            string ident = "hotp-test";

            hotp.Devices.Add(ident, key);

            OneTimePassword.VerifyResult result = hotp.Verify(ident, "928856");
            Assert.AreEqual(result, OneTimePassword.VerifyResult.Success);
            
            result = hotp.Verify(ident, "928856");
            Assert.AreEqual(result, OneTimePassword.VerifyResult.Failed);
        }

        [Test]
        public void MaxFailuresAndUnLockTest()
        {

            HashbasedOneTimePassword hotp = new HashbasedOneTimePassword();
            string ident = "hotp-test";
            byte[] key = new byte[20];
            for (int i = 0; i < 20; i++) key[0] = 23;
            OneTimePassword.VerifyResult result;

            hotp.EnableThrottling = false;
            hotp.Devices.Add(ident, key);

            for (int i = 0; i < hotp.MaxAttempts; i++)
            {
                Assert.IsTrue(!hotp.Devices.IsLocked(ident));
                result = hotp.Verify(ident, "000000");
                Assert.AreEqual(result, OneTimePassword.VerifyResult.Failed);
            }

            Assert.IsTrue(hotp.Devices.IsLocked(ident));
            result = hotp.Verify(ident, "000000");
            Assert.AreEqual(result, OneTimePassword.VerifyResult.FailedMaxAttempts);

            hotp.Devices.UnLock(ident);
            Assert.IsTrue(!hotp.Devices.IsLocked(ident));
            result = hotp.Verify(ident, "000000");
            Assert.AreEqual(result, OneTimePassword.VerifyResult.Failed);

        }


        [Test]
        public void GenerateQrTest()
        {
            HashbasedOneTimePassword hotp = new HashbasedOneTimePassword();
            string ident = "hotp-test";
            byte[] secret = new byte[] { 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23 };

            hotp.Devices.Add(ident, secret);

            byte[] qr = hotp.RenderQrCode(ident);
            Assert.IsTrue(qr.Length != 0);

            File.WriteAllBytes("hotp-code.png", qr);

        }


        [Test]
        public void ResyncAheadTest()
        {
            HashbasedOneTimePassword hotp = new HashbasedOneTimePassword();
            string ident = "hotp-test";
            byte[] secret = new byte[] { 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23 };

            hotp.Devices.Add(ident, secret);
            hotp.Window = 0;

            string[] codes = {"565787", "194430", "825406"}; //code number 5, 6 and 7

            bool resyncResult = hotp.Resync(ident, codes, 20);
            Assert.IsTrue(resyncResult);

            OneTimePassword.VerifyResult result = hotp.Verify(ident, "061268"); //code number 8
            Assert.AreEqual(result, OneTimePassword.VerifyResult.Success);
        }


        [Test]
        public void ResyncBehindTest()
        {
            HashbasedOneTimePassword hotp = new HashbasedOneTimePassword();
            string ident = "hotp-test";
            byte[] secret = new byte[] { 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23 };
            string[] goodCodes = {
                                  "928856", "216764", "449156", "760917", "791329", "565787", "194430", "825406", "061268", "606702", 
                                  "908214", "901682", "902138", "629799", "068004", "151821", "063133", "633799", "416418", "763933"
                              };

            hotp.Devices.Add(ident, secret);
            hotp.Window = 0;
            hotp.EnableThrottling = false;
            hotp.MaxAttempts = 999;

            for (int i = 0; i < goodCodes.Length; i++)
            {
                //run some numbers up.
                Assert.AreEqual(OneTimePassword.VerifyResult.Success, hotp.Verify(ident, goodCodes[i])); 
            }

            string[] codes = { "565787", "194430", "825406" }; //code number 5, 6 and 7

            bool resyncResult = hotp.Resync(ident, codes, 18);
            Assert.IsTrue(resyncResult);

            OneTimePassword.VerifyResult result = hotp.Verify(ident, "061268"); //code number 8
            Assert.AreEqual(result, OneTimePassword.VerifyResult.Success);
        }

        [Test]
        public void SaveLoadDbTest()
        {
            HashbasedOneTimePassword hotp = new HashbasedOneTimePassword();
            string ident = "hotp-test";
            byte[] secret = new byte[] { 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23 };

            hotp.Devices.Add(ident, secret);

            hotp.Devices.Save("db.xml");

            Assert.IsTrue(File.Exists("db.xml"));

            HashbasedOneTimePassword hotp2 = new HashbasedOneTimePassword();
            hotp2.Devices.Load("db.xml");
            Assert.IsTrue(hotp.Devices.Contains(ident));

            OneTimePassword.VerifyResult result = hotp2.Verify(ident, "216764");
            Assert.AreEqual(OneTimePassword.VerifyResult.Success, result);

        }

        [Test]
        [ExpectedException(typeof(OtpDecryptionException))]
        public void DecryptFailureTest()
        {
            HashbasedOneTimePassword hotp = new HashbasedOneTimePassword();
            string ident = "hotp-test";
            byte[] secret = new byte[] { 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23 };

            hotp.Devices.Add(ident, secret);
            hotp.Devices.Save("db.xml");

            HashbasedOneTimePassword hotp2 = new HashbasedOneTimePassword(Utils.GenerateNewAesKey(), Utils.GenerateNewAesIv());
            hotp2.Devices.Load("db.xml");

        }
    }
}
