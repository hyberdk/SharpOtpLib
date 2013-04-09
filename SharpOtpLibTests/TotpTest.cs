using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using NUnit.Framework;
using SharpOtpLib;
using System;

namespace SharpOtpLibTests
{
    /// <summary>
    ///This is a test class for TOTPTest and is intended
    ///to contain all TotpTest Unit Tests
    ///</summary>
    [TestFixture()]
    public class TotpTest
    {


        /// <summary>
        ///A test for Verify, testing the current interval with no offset
        ///</summary>
        [Test]
        public void VerifyNoOffsetSuccessTest()
        {
            TimebasedOneTimePassword target = new TimebasedOneTimePassword();
            string ident = "test-totp";
            byte[] secret = new byte[] {23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23};

            target.Devices.Add(ident, secret);

            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long counter = (long)Math.Floor((DateTime.UtcNow - unixEpoch).TotalSeconds) / 30;

            string code = OneTimePassword.Generate(secret, counter, OneTimePassword.CodeLength.Six);

            OneTimePassword.VerifyResult expected = OneTimePassword.VerifyResult.Success;
            OneTimePassword.VerifyResult actual;
            actual = target.Verify(ident, code);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        ///A test for Verify, testing the current interval with offset of 5
        ///</summary>
        [Test]
        public void VerifyFiveOffsetSuccessTest()
        {
            TimebasedOneTimePassword target = new TimebasedOneTimePassword();
            string ident = "test-totp";
            byte[] secret = new byte[] { 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23 };
            target.Window = 5;
            target.Devices.Add(ident, secret);

            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long counter = (long)Math.Floor((DateTime.UtcNow - unixEpoch).TotalSeconds) / 30;

            string code = OneTimePassword.Generate(secret, counter-target.Window, OneTimePassword.CodeLength.Six);

            OneTimePassword.VerifyResult expected = OneTimePassword.VerifyResult.Success;
            OneTimePassword.VerifyResult actual;
            actual = target.Verify(ident, code);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Verify, that the code cannot be used twice.
        ///</summary>
        [Test()]
        public void VerifyUsedCodeTest()
        {
            TimebasedOneTimePassword target = new TimebasedOneTimePassword();
            string ident = "test-totp";
            byte[] secret = new byte[] { 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23 };

            target.Devices.Add(ident, secret);

            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long counter = (long)Math.Floor((DateTime.UtcNow - unixEpoch).TotalSeconds) / 30;

            string code = OneTimePassword.Generate(secret, counter, OneTimePassword.CodeLength.Six);

            OneTimePassword.VerifyResult expected = OneTimePassword.VerifyResult.Success;
            OneTimePassword.VerifyResult actual;
            actual = target.Verify(ident, code);
            Assert.AreEqual(expected, actual);

            expected = OneTimePassword.VerifyResult.FailedUsedCode;
            actual = target.Verify(ident, code);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Verify, that the code cannot be used twice.
        /// 
        ///</summary>
        [Test()]
        public void VerifyTooGreatOffsetTest()
        {
            TimebasedOneTimePassword target = new TimebasedOneTimePassword();
            string ident = "test-totp";
            byte[] secret = new byte[] { 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23 };

            target.Window = 3;
            target.Devices.Add(ident, secret);

            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long counter = (long)Math.Floor((DateTime.UtcNow - unixEpoch).TotalSeconds) / 30;

            string code = OneTimePassword.Generate(secret, counter+target.Window+1, OneTimePassword.CodeLength.Six);

            OneTimePassword.VerifyResult expected = OneTimePassword.VerifyResult.Failed;
            OneTimePassword.VerifyResult actual;
            actual = target.Verify(ident, code);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Verify, that throttling is working.
        ///</summary>
        [Test()]
        public void VerifyThrottlingTest()
        {
            TimebasedOneTimePassword target = new TimebasedOneTimePassword();
            string ident = "test-totp";
            byte[] secret = new byte[] { 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23 };

            target.Window = 0;
            target.Devices.Add(ident, secret);

            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long counter = (long)Math.Floor((DateTime.UtcNow - unixEpoch).TotalSeconds) / 30;

            string code = OneTimePassword.Generate(secret, counter - 3, OneTimePassword.CodeLength.Six);

            DateTime start = DateTime.Now;
            target.Verify(ident, code);
            target.Verify(ident, code);

            Assert.IsTrue((DateTime.Now - start).TotalSeconds > 3);

        }


        [Test]
        public void VerifyCodeTimeout()
        {
            TimebasedOneTimePassword totp = new TimebasedOneTimePassword();
            long timeout = totp.CodeTimeout;
            Assert.IsTrue(timeout <= 30000);
        }

        [Test]
        public void GenerateQrTest()
        {
            var totp = new TimebasedOneTimePassword();
            string ident = "totp-test";
            var secret = new byte[] { 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23 };

            totp.Devices.Add(ident, secret);

            byte[] qr = totp.RenderQrCode(ident, 200);
            Assert.IsTrue(qr.Length != 0);

            File.WriteAllBytes("totp-code.png", qr);

        }

        [Test]
        public void ResyncAheadTest()
        {
            TimebasedOneTimePassword totp = new TimebasedOneTimePassword();
            string ident = "hotp-test";
            byte[] secret = new byte[] {23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23};
            string[] goodCodes = new string[3];
            
            const int offset = 15; //how big should our offset be

            //make sure we are not close to a time boundery
            if ((DateTime.Now.Second > 25 && DateTime.Now.Second < 30) ||
                (DateTime.Now.Second > 55 && DateTime.Now.Second < 60))
            {
                Thread.Sleep(6000);
            }

            long curInterval = totp.CurrentInterval;

            //generate some good codes, for our current time + our offset
            for (int i = 0; i < goodCodes.Length; i++)
            {
                goodCodes[i] = OneTimePassword.Generate(secret, curInterval + offset + i, OneTimePassword.CodeLength.Six);
            }

            totp.Devices.Add(ident, secret);
            totp.Window = 0;

            bool resyncResult =  totp.Resync(ident, goodCodes, 20);
            Assert.IsTrue(resyncResult);

            string offsetCode = OneTimePassword.Generate(secret, curInterval + goodCodes.Length + offset, OneTimePassword.CodeLength.Six);
            OneTimePassword.VerifyResult result = totp.Verify(ident, offsetCode);
            Assert.AreEqual(OneTimePassword.VerifyResult.Success, result);


        }


        [Test]
        public void ResyncBehindTest()
        {
            TimebasedOneTimePassword totp = new TimebasedOneTimePassword();
            string ident = "hotp-test";
            byte[] secret = new byte[] { 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23, 23 };
            string[] goodCodes = new string[3];

            const int offset = -15; //how big should our offset be

            //make sure we are not close to a time boundery
            if ((DateTime.Now.Second > 25 && DateTime.Now.Second < 30) ||
                (DateTime.Now.Second > 55 && DateTime.Now.Second < 60))
            {
                Thread.Sleep(6000);
            }

            long curInterval = totp.CurrentInterval;

            //generate some good codes, for our current time + our offset
            for (int i = 0; i < goodCodes.Length; i++)
            {
                goodCodes[i] = OneTimePassword.Generate(secret, curInterval + offset + i, OneTimePassword.CodeLength.Six);
            }

            totp.Devices.Add(ident, secret);
            totp.Window = 0;

            bool resyncResult = totp.Resync(ident, goodCodes, 20);
            Assert.IsTrue(resyncResult);

            string offsetCode = OneTimePassword.Generate(secret, curInterval + goodCodes.Length + offset, OneTimePassword.CodeLength.Six);
            OneTimePassword.VerifyResult result = totp.Verify(ident, offsetCode);
            Assert.AreEqual(OneTimePassword.VerifyResult.Success, result);


        }
    }
}
