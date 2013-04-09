using System;

namespace SharpOtpLib
{
    /// <summary>
    /// Implementation of the HMAC-Based One-Time Password Algorithm (HOTP) Algorithm
    ///  
    /// This is also compatiple with GoogleAuthenticator, that you can find at 
    /// http://code.google.com/p/google-authenticator/ or other TOTP complient
    /// devicess or software
    /// 
    /// This implementation is based (and complient with) RFC4226
    /// See: http://tools.ietf.org/rfc/rfc4226.txt or http://en.wikipedia.org/wiki/HOTP
    /// </summary>
    public sealed class HashbasedOneTimePassword : OneTimePassword
    {
        /// <summary>
        /// Create a new instance, with the default encryption key and vector. Since this
        /// is a well known (just look up the source) it is NOT recommended to use this,
        /// as anyone can decrypt our secret keys!
        /// </summary>
        public HashbasedOneTimePassword() :base(){}

        /// <summary>
        /// Create a new instance with your encryption key and vector. This is the
        /// recommended way of creating a new instance of this class!
        /// </summary>
        /// <param name="aesKey">AES key (max 265bit)</param>
        /// <param name="aesIv">Initialization vector, recommended is 128bit</param>
        public HashbasedOneTimePassword(byte[] aesKey, byte[] aesIv) :base(aesKey, aesIv) {}

        /// <summary>
        /// Verified a code entered by user.
        /// </summary>
        /// <param name="ident">ident of devices</param>
        /// <param name="code">6, 7 or 8 digit code we need to test</param>
        /// <exception cref="DeviceNotFoundException">if device ident is not found</exception>
        /// <returns>result :-)</returns>
        public override VerifyResult Verify(string ident, string code)
        {
            //get devices
            Device device = Devices.Get(ident);

            //check failures
            if (HasMaxFailures(ref device))
            {
                DoThrottling(ref device);
                return VerifyResult.FailedMaxAttempts;
            }
            
            //check code
            int offset = 0;
            do
            {
                if (Generate(Decrypt(device.Key), device.HotpCounter + offset) == code)
                {
                    device.HotpCounter += offset + 1; //set and resync counter
                    device.Failures = 0;
                    return VerifyResult.Success;
                }
                offset++;
            } while (offset < Window);

            //do throttling
            DoThrottling(ref device);

            device.Failures++;
            return VerifyResult.Failed;
        }

        /// <summary>
        /// Resyncs a remote devices with the local devices database.
        /// You can enter multiple codes, for the resync to succeed, all the
        /// codes need to be verified. Its more secure to use multiple codes.
        /// 
        /// If is successfully syncs it will unlock the device by setting the 
        /// failed count to 0
        /// </summary>
        ///<param name="ident">Ident of device you want to resync </param>
        ///<param name="codes">list of codes that should be verified</param>
        ///<param name="window">overrides global window for the resync. Set to -1 to use the default window. 
        /// Note that this is both positive and negative window</param>
        ///<exception cref="ArgumentNullException">If codes or ident is null</exception>
        ///<exception cref="ArgumentOutOfRangeException">If window is less than -1</exception>
        /// <exception cref="OtpDecryptionException">if key failed to decrypt</exception>
        ///<returns>true if resync was successfull</returns>
        /// <remarks>
        /// Background info (RFC4226):
        /// As we suggested for the resynchronization to enter a short sequence
        /// (say, 2 or 3) of HOTP values, we could generalize the concept to the
        /// protocol, and add a parameter L that would define the length of the
        /// HOTP sequence to enter.
        ///
        /// Per default, the value L SHOULD be set to 1, but if security needs to
        /// be increased, users might be asked (possibly for a short period of
        /// time, or a specific operation) to enter L HOTP values.
        ///
        /// This is another way, without increasing the HOTP length or using
        /// alphanumeric values to tighten security.
        ///
        /// Note: The system MAY also be programmed to request synchronization on
        /// a regular basis (e.g., every night, twice a week, etc.) and to
        /// achieve this purpose, ask for a sequence of L HOTP values.
        /// </remarks>
        public bool Resync(string ident, string[] codes, int window = -1)
        {
            Device devices = Devices.Get(ident);

            //some checks to make sure user does not mess up things.
            if (codes == null) throw new ArgumentNullException("codes");
            if (codes.Length == 0) throw new ArgumentOutOfRangeException("codes", "There must be atleast one code in array");
            if (window < -1) throw new ArgumentOutOfRangeException("window", "Window cannot be less than -1");
            if (window == -1) window = Window;

            return Resync(devices, codes, devices.HotpCounter, UpdateDevice, window);

        }

        private static void UpdateDevice(ref Device devices, long counter)
        {
            devices.HotpCounter = counter;
            devices.Failures = 0;
        }

        /// <summary>
        /// This generates a QR (Quick Response) code that can be used to scan with a
        /// mobile devices etc. This is complient with GoogleAuthenticator found on 
        /// Google Play (https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2)
        /// </summary>
        /// <param name="ident">ident of devices</param>
        /// <param name="size">Size of png, default is 200px</param>
        /// <returns>Quick Response code png as byte array</returns>
        /// <exception cref="DeviceNotFoundException">if ident cannot be found</exception>
        /// <exception cref="OtpDecryptionException">if device key failed to decrypt</exception>
        public byte[] RenderQrCode(string ident, int size = 200)
        {
            Device devices = Devices.Get(ident);

            byte[] qr = RenderQrCode(devices.Ident, Decrypt(devices.Key), QrCodeType.HOTP, size);
            return qr;
        }
    }
}