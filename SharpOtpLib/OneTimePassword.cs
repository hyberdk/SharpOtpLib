using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace SharpOtpLib
{
    internal delegate void UpdateDevice(ref Device devices, long counter);

    /// <summary>
    /// This is the base class for HashbasedOneTimePassword and
    /// TimebasedOneTimePasswords, this implements shared functionality
    /// </summary>
    public abstract class OneTimePassword
    {
       
        private int _window = 2;
        private CodeLength _length = CodeLength.Six;
        private readonly byte[] _aesKey;
        private readonly byte[] _aesIv;

        private bool _enableThrottling = true;
        private int _throttlingDelay = 5000;

        /// <summary>
        /// Create a new instance, with the default encryption key and vector. Since this
        /// is a well known (just look up the source) it is NOT recommended to use this,
        /// as anyone can decrypt our secret keys!
        /// </summary>
        protected OneTimePassword()
        {
            _aesIv = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            _aesKey = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 
                                   17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };
            Devices = new Devices(_aesKey, _aesIv);
        }

        /// <summary>
        /// Create a new instance with your encryption key and vector. This is the
        /// recommended way of creating a new instance of this class!
        /// </summary>
        /// <param name="aesKey">AES key (max 265bit)</param>
        /// <param name="aesIv">Initialization vector, recommended is 128bit</param>
        protected OneTimePassword(byte[] aesKey, byte[] aesIv)
        {
            _aesIv = aesIv;
            _aesKey = aesKey;
            Devices = new Devices(_aesKey, _aesIv);
        }

        /// <summary>
        /// Result of Code verification
        /// </summary>
        public enum VerifyResult
        {
            /// <summary>
            /// Success: Code matched, good!
            /// </summary>
            Success = 0,

            /// <summary>
            /// Failure: Code does not match within the allowed offset.
            /// </summary>
            Failed = 10,

            /// <summary>
            /// Failure: Ident was not found
            /// </summary>
            FailedIdent = 11,

            /// <summary>
            /// Failure: Maximum number of attempts has been made.
            /// Even if the code was correct it will still return as failure.
            /// You need to unlock the ident before a success can be achieved.
            /// </summary>
            FailedMaxAttempts = 12,

            /// <summary>
            /// Failure: Code has already been used. This only applies to Time-based codes.
            /// </summary>
            FailedUsedCode = 13

        }

        /// <summary>
        /// Number of code digits.
        /// </summary>
        /// <remarks>
        /// Background info (RFC4226):
        /// Implementations MUST extract a 6-digit code at a minimum and possibly
        /// 7 and 8-digit code.  Depending on security requirements, Digit = 7 or
        /// more SHOULD be considered in order to extract a longer HOTP value.</remarks>
        public enum CodeLength
        {
            /// <summary>
            /// Use 6 digit code, this is the one you usually want.
            /// </summary>
            Six = 6,
            
            /// <summary>
            /// Use 7 digit code
            /// </summary>
            Seven = 7,
            
            /// <summary>
            /// Use 8 digit code
            /// </summary>
            Eight = 8
        }

        /// <summary>
        /// Quick Response code type.
        /// </summary>
        public enum QrCodeType
        {
            /// <summary>
            /// HMAC-based One-Time Password
            /// </summary>
            HOTP,

            /// <summary>
            /// Time-based One-Time Password
            /// </summary>
            TOTP
        }

        /// <summary>
        /// This generates a 6, 7 or 8 digit code from the secret key and seed counter.
        /// </summary>
        /// <param name="key">Secret key</param>
        /// <param name="counter">8byte counter to seed into the key</param>
        ///<param name="length">length of key, most often 6 digits is used.</param>
        ///<returns>Code digits</returns>
        /// <exception cref="ArgumentNullException">If key is null</exception>
        /// <remarks><![CDATA[
        /// Background info (RFC4226):
        /// The following code example describes the extraction of a dynamic
        /// binary code given that hmac_result is a byte array with the HMAC-
        /// SHA-1 result:
        /// 
        ///      int offset   =  hmac_result[19] & 0xf ;
        ///      int bin_code = (hmac_result[offset]  & 0x7f) << 24
        ///         | (hmac_result[offset+1] & 0xff) << 16
        ///         | (hmac_result[offset+2] & 0xff) <<  8
        ///         | (hmac_result[offset+3] & 0xff) ;
        /// 
        /// SHA-1 HMAC Bytes (Example)
        /// 
        /// -------------------------------------------------------------
        /// | Byte Number                                               |
        /// -------------------------------------------------------------
        /// |00|01|02|03|04|05|06|07|08|09|10|11|12|13|14|15|16|17|18|19|
        /// -------------------------------------------------------------
        /// | Byte Value                                                |
        /// -------------------------------------------------------------
        /// |1f|86|98|69|0e|02|ca|16|61|85|50|ef|7f|19|da|8e|94|5b|55|5a|
        /// -------------------------------***********----------------++|
        ///
        /// * The last byte (byte 19) has the hex value 0x5a.
        /// * The value of the lower 4 bits is 0xa (the offset value).
        /// * The offset value is byte 10 (0xa).
        /// * The value of the 4 bytes starting at byte 10 is 0x50ef7f19,
        ///   which is the dynamic binary code DBC1.
        /// * The MSB of DBC1 is 0x50 so DBC2 = DBC1 = 0x50ef7f19 .
        /// * HOTP = DBC2 modulo 10^6 = 872921.
        ///
        /// We treat the dynamic binary code as a 31-bit, unsigned, big-endian
        /// integer; the first byte is masked with a 0x7f.
        ///
        /// We then take this number modulo 1,000,000 (10^6) to generate the 6-
        /// digit HOTP value 872921 decimal.
        ///]]></remarks>
        public static string Generate(byte[] key, long counter, CodeLength length)
        {

            byte[] hmacResult;
            byte[] byteCounter = BitConverter.GetBytes(counter);

            if (key == null) throw new ArgumentNullException("key", "Key may not be null");


            //If we are on a system that uses Little Endian, like intel ;-)
            //RFC4226 states we need Big Endian.
            if (BitConverter.IsLittleEndian) byteCounter = byteCounter.Reverse().ToArray();

            //do hash
            using (var hmac = new HMACSHA1(key))
            {
                hmacResult = hmac.ComputeHash(byteCounter);
            }

            var offset = hmacResult[19] & 0xf;

            //first byte is masked with a 0x7f as per RFC
            var binCode = (hmacResult[offset] & 0x7f) << 24
                    | (hmacResult[offset + 1] & 0xff) << 16
                    | (hmacResult[offset + 2] & 0xff) << 8
                    | (hmacResult[offset + 3] & 0xff);

            //modulo our binCode with the code length (6, 7 or 8)
            //make sure to pad left with 0's.
            int i = (int) length;
            var code = (binCode%Math.Pow(10, i)).ToString(CultureInfo.InvariantCulture).PadLeft(i, '0');

            return code;
            
        }

        /// <summary>
        /// This generates a 6, 7 or 8 digit code from the secret key and seed counter.
        /// </summary>
        /// <param name="key">Secret key</param>
        /// <param name="counter">8byte counter to seed into the key</param>
        ///<returns>Code digits</returns>
        /// <exception cref="ArgumentNullException">If key is null</exception>
        /// <remarks><![CDATA[
        /// Background info (RFC4226):
        /// The following code example describes the extraction of a dynamic
        /// binary code given that hmac_result is a byte array with the HMAC-
        /// SHA-1 result:
        /// 
        ///      int offset   =  hmac_result[19] & 0xf ;
        ///      int bin_code = (hmac_result[offset]  & 0x7f) << 24
        ///         | (hmac_result[offset+1] & 0xff) << 16
        ///         | (hmac_result[offset+2] & 0xff) <<  8
        ///         | (hmac_result[offset+3] & 0xff) ;
        /// 
        /// SHA-1 HMAC Bytes (Example)
        /// 
        /// -------------------------------------------------------------
        /// | Byte Number                                               |
        /// -------------------------------------------------------------
        /// |00|01|02|03|04|05|06|07|08|09|10|11|12|13|14|15|16|17|18|19|
        /// -------------------------------------------------------------
        /// | Byte Value                                                |
        /// -------------------------------------------------------------
        /// |1f|86|98|69|0e|02|ca|16|61|85|50|ef|7f|19|da|8e|94|5b|55|5a|
        /// -------------------------------***********----------------++|
        ///
        /// * The last byte (byte 19) has the hex value 0x5a.
        /// * The value of the lower 4 bits is 0xa (the offset value).
        /// * The offset value is byte 10 (0xa).
        /// * The value of the 4 bytes starting at byte 10 is 0x50ef7f19,
        ///   which is the dynamic binary code DBC1.
        /// * The MSB of DBC1 is 0x50 so DBC2 = DBC1 = 0x50ef7f19 .
        /// * HOTP = DBC2 modulo 10^6 = 872921.
        ///
        /// We treat the dynamic binary code as a 31-bit, unsigned, big-endian
        /// integer; the first byte is masked with a 0x7f.
        ///
        /// We then take this number modulo 1,000,000 (10^6) to generate the 6-
        /// digit HOTP value 872921 decimal.
        ///]]></remarks>
        internal string Generate(byte[] key, long counter)
        {
            return Generate(key, counter, Length);
        }

        /// <summary>
        /// Verifies a code from a OTP devices.
        /// </summary>
        /// <param name="ident">Identifier of devices</param>
        /// <param name="code">Code entered by user</param>
        /// <returns></returns>
        public abstract VerifyResult Verify(string ident, string code);

        internal void DoThrottling(ref Device devices)
        {
            if (devices.Failures == 0 || !EnableThrottling) return;
            Thread.Sleep(ThrottlingDelay * devices.Failures);
        }

        internal bool HasMaxFailures(ref Device devices)
        {
            if (MaxAttempts == 0 || devices.Failures < MaxAttempts) return false;
            devices.Failures++;
            return true;
        }

        /// <summary>
        /// Maximum number of attempts that can be achieved before the ident is lock
        /// and will always return failed..
        /// Default is: 3
        /// </summary>
        /// <remarks>
        /// Background info (RFC4226):
        /// We RECOMMEND setting a throttling parameter T, which defines the
        /// maximum number of possible attempts for One-Time Password validation.
        /// The validation server manages individual counters per HOTP devices in
        /// order to take note of any failed attempt.  We RECOMMEND T not to be
        /// too large, particularly if the resynchronization method used on the
        /// server is window-based, and the window size is large.  T SHOULD be
        /// set as low as possible, while still ensuring that usability is not
        /// significantly impacted.</remarks>
        public int MaxAttempts { get { return Devices.MaxAttempts ; } set { Devices.MaxAttempts = value; } }

        /// <summary>
        /// Resyncs a remote devices with the local devices database.
        /// You can enter multiple codes, for the resync to succeed, all the
        /// codes need to be verified. Its more secure to use multiple codes.
        /// 
        /// If is successfully syncs it use the call back for you to unlock the
        /// device and set the correct counter.
        /// </summary>
        ///<param name="device">device that you want to resync</param>
        ///<param name="codes">list of codes that should be verified</param>
        ///<param name="counter">start counter, meaning on what interval should se start resync'ing</param>
        ///<param name="updateDevice">call back delegate for when the resync was successfull, we need to update the device differently depending on HOTP/TOTP</param>
        ///<param name="window">overrides global window for the resync. Set to -1 to use the default window</param>
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
        internal bool Resync(Device device, string[] codes, long counter, UpdateDevice updateDevice, int window = -1)
        {

            for (int i = 0; i <= window; i++)
            {
                //check forward in totp/hotp counter
                if (Generate(Decrypt(device.Key), counter + i) == codes[0] && ResyncCheck(ref device, codes, counter + i))
                {
                        updateDevice(ref device, counter + i + codes.Length); //delegate to update devices.
                        return true;
                }

                //check backwards in totp/hotp counter
                if (Generate(Decrypt(device.Key), counter - i) == codes[0] && ResyncCheck(ref device, codes, counter - i))
                {
                    updateDevice(ref device, counter - i + codes.Length); //delegate to update devices.
                    return true;
                }
            }
            return false;
        }

        private bool ResyncCheck(ref Device devices, string[] codes, long counter)
        {

            for (int i = 0; i < codes.Length; i++)
            {
                string code = Generate(Decrypt(devices.Key), counter + i);
                if (code != codes[i]) return false;
            }
            return true;
        }

        ///// <summary>
        ///// This method should inplement a way of resyncing the remote
        ///// devices with our local database.
        ///// You can enter multiple codes, for the resync to succeed, all the
        ///// codes need to be verified. Its more secure to use multiple codes.
        ///// </summary>
        ///// <param name="ident">ident of remote devices</param>
        ///// <param name="codes">list of codes that should be verified</param>
        ///// <param name="window">overrides global window for the resync. Set to -1 to use the default window</param>
        ///// <returns>true if resync was successfull</returns>
        ///// <remarks>
        ///// Background info (RFC4226):
        ///// As we suggested for the resynchronization to enter a short sequence
        ///// (say, 2 or 3) of HOTP values, we could generalize the concept to the
        ///// protocol, and add a parameter L that would define the length of the
        ///// HOTP sequence to enter.
        /////
        ///// Per default, the value L SHOULD be set to 1, but if security needs to
        ///// be increased, users might be asked (possibly for a short period of
        ///// time, or a specific operation) to enter L HOTP values.
        /////
        ///// This is another way, without increasing the HOTP length or using
        ///// alphanumeric values to tighten security.
        /////
        ///// Note: The system MAY also be programmed to request synchronization on
        ///// a regular basis (e.g., every night, twice a week, etc.) and to
        ///// achieve this purpose, ask for a sequence of L HOTP values.</remarks>
        //public abstract bool Resync(string ident, int[] codes, int window = -1);

        /// <summary>
        /// Look-ahead window. Default is 2
        /// </summary>
        /// <remarks>
        /// Background info (RFC4226):
        /// We RECOMMEND setting a look-ahead parameter s on the server, which
        /// defines the size of the look-ahead window.  In a nutshell, the server
        /// can recalculate the next s HOTP-server values, and check them against
        /// the received HOTP client.
        /// 
        /// The upper bound set by the parameter s ensures the server does not go
        /// on checking HOTP values forever (causing a denial-of-service attack)
        /// and also restricts the space of possible solutions for an attacker
        /// trying to manufacture HOTP values. s SHOULD be set as low as
        /// possible, while still ensuring that usability is not impacted.
        /// </remarks>
        public int Window { get { return _window; } set { _window = value; } }

        /// <summary>
        /// Sets/gets the current code length. Default is 6 digits
        /// </summary>
        public CodeLength Length { get { return _length; } set { _length = value; } }
        
        /// <summary>
        /// Sets the throttling delay, this delay (in ms) is multiplied with the
        /// failed attempts. Default is 5000ms as suggested in RFC4226
        /// </summary>
        /// <remarks>
        /// Background info (RFC4226):
        /// An option would be to implement a delay scheme to avoid a brute
        /// force attack.  After each failed attempt A, the authentication server
        /// would wait for an increased T*A number of seconds, e.g., say T = 5,
        /// then after 1 attempt, the server waits for 5 seconds, at the second
        /// failed attempt, it waits for 5*2 = 10 seconds, etc.
        /// </remarks>
        public int ThrottlingDelay { get { return _throttlingDelay; } set { _throttlingDelay = value; } }

        /// <summary>
        /// Enables throttling to prevent bruteforce attacks.
        /// Default is "true"
        /// </summary>
        /// <remarks>
        /// Background info (RFC4226):
        /// Truncating the HMAC-SHA-1 value to a shorter value makes a brute
        /// force attack possible.  Therefore, the authentication server needs to
        /// detect and stop brute force attacks.
        /// We RECOMMEND setting a throttling parameter T, which defines the
        /// maximum number of possible attempts for One-Time Password validation.
        /// The validation server manages individual counters per HOTP devices in
        /// order to take note of any failed attempt.
        /// </remarks>
        public bool EnableThrottling { get { return _enableThrottling; } set { _enableThrottling = value; } }

        /// <summary>
        /// Renders a Quick Reponse code, that can be used with Google Authenticator or compatiple software.
        /// </summary>
        /// <param name="ident">ident of device as shown to user</param>
        /// <param name="key">private key</param>
        /// <param name="type">type of QR code, HOTP or TOTP</param>
        /// <param name="size">pixels height/width of png image</param>
        /// <returns>png image as byte array</returns>
        public static byte[] RenderQrCode(string ident, byte[] key, QrCodeType type, int size=200)
        {
            string url = "otpauth://" + type.ToString().ToLower() + "/" + ident + "?secret=" + Base32.Convert(key);
            
            QrEncoder encoder = new QrEncoder(ErrorCorrectionLevel.M);
            QrCode qrCode;
            encoder.TryEncode(url, out qrCode);

            ISizeCalculation isize = new FixedCodeSize(size, QuietZoneModules.Zero);
            GraphicsRenderer gRenderer = new GraphicsRenderer(isize);

            using (MemoryStream ms = new MemoryStream())
            {
                gRenderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);
                return ms.GetBuffer();
            }

        }

        /// <summary>
        /// Device list of "token", think of a device like an RSA or Vasco hardware token or a Google Authenticator soft-token
        /// </summary>
        public Devices Devices { get; set; }

        internal byte[] Decrypt(byte[] data)
        {
            return Utils.Decrypt(data, _aesKey, _aesIv);
        }
    }

}