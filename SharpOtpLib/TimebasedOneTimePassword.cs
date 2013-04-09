using System;
using System.Collections.Generic;

namespace SharpOtpLib
{
    /// <summary>
    /// Implementation of the Time-Based One-Time Password (TOTP) Algorithm
    /// 
    /// This is also compatiple with GoogleAuthenticator, that you can find at 
    /// http://code.google.com/p/google-authenticator/ or other TOTP complient
    /// devicess or software
    /// 
    /// This implementation is based (and complient with) RFC6238
    /// http://tools.ietf.org/html/rfc6238
    /// http://en.wikipedia.org/wiki/Time-based_One-time_Password_Algorithm
    /// </summary>
    public sealed class TimebasedOneTimePassword : OneTimePassword
    {

        readonly static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        /// <summary>
        /// Uses default RECOMMENDED time-step of 30sec, however this uses the default
        /// encryption key and vector which is NOT recommended! For security please set
        /// AES key and Initialization Vector.
        /// </summary>
        /// <param name="timestep">Set timestep, default is 30sec as per RFC6238</param>
        public TimebasedOneTimePassword(int timestep=30) : base()
        {
            TimeStep = timestep; //Recommended time step as per RFC6238
        }

        /// <summary>
        /// Set time-step to something else than default and set encryption key and initialization vector
        /// and optionally set the timestep.
        /// </summary>
        /// <param name="aesKey"></param>
        /// <param name="aesIv"></param>
        /// <param name="timestep">Set timestep, default is 30sec as per RFC6238</param>
        public TimebasedOneTimePassword(byte[] aesKey, byte[] aesIv, int timestep = 30) : base(aesKey, aesIv)
        {
            TimeStep = timestep;
        }

        /// <summary>
        /// Verified a code entered by user.
        /// </summary>
        /// <param name="ident">ident of devices</param>
        /// <param name="code">6, 7 or 8 digit code we need to test</param>
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
            long timeInterval = GetCurrentTimeInterval(TimeStep);
            long curInterval = timeInterval + device.TotpOffset;
            int offset = 0;
            do
            {
                //positive offset
                if (Generate(Decrypt(device.Key), curInterval + offset) == code)
                    return UpdateDevice(ref device, offset, timeInterval);

                //negative offset.. (offset * -1)
                if (Generate(Decrypt(device.Key), curInterval + (offset * -1)) == code)
                    return UpdateDevice(ref device, (offset * -1), timeInterval);

                offset++;
            } while (offset < Window+1);

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
        /// failed count to 0 and clear the used codes list.
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

            long counter = GetCurrentTimeInterval(TimeStep) + devices.TotpOffset;

            return Resync(devices, codes, counter, UpdateDevice, window);

        }

        private void UpdateDevice(ref Device devices, long counter)
        {
            devices.TotpOffset = (int) (counter - GetCurrentTimeInterval(TimeStep));
            devices.Failures = 0;
            devices.TotpUsedCounter = new List<long>();
        }

        private VerifyResult UpdateDevice(ref Device devices, int newOffset, long currentCounter)
        {
            if (CheckUsedCounter(ref devices, newOffset + currentCounter)) return VerifyResult.FailedUsedCode;

            devices.TotpOffset = newOffset; //set offset from devices + the current offset.
            devices.Failures = 0;
            return VerifyResult.Success;
        }

        private bool CheckUsedCounter(ref Device devices, long counter)
        {
            if (devices.TotpUsedCounter == null) devices.TotpUsedCounter = new List<long>();

            //make sure we can only use a counter once.
            if (devices.TotpUsedCounter.Contains(counter))
            {
                devices.Failures++;
                return true;
            }
            devices.TotpUsedCounter.Add(counter);
            return false;
        }

        /// <summary>
        /// The current time interval
        /// </summary>
        public long CurrentInterval { get { return GetCurrentTimeInterval(TimeStep); }
        }

        private static long GetCurrentTimeInterval(int timestep)
        {
            return (long)Math.Floor((DateTime.UtcNow - UnixEpoch).TotalSeconds) / timestep;
        }

        /// <summary>
        /// TimeStep is the intervals that new codes are generated in seconds
        /// The default (and recommended value) is 30. You need to set this
        /// with the constructor.
        /// </summary>
        /// <remarks>
        /// Background info (RFC6238):
        /// The time-step size has an impact on both security and usability.  A
        /// larger time-step size means a larger validity window for an OTP to be
        /// accepted by a validation system.  There are implications for using a
        /// larger time-step size, as follows:
        ///
        /// First, a larger time-step size exposes a larger window to attack.
        /// When an OTP is generated and exposed to a third party before it is
        /// consumed, the third party can consume the OTP within the time-step
        /// window.
        ///
        /// We RECOMMEND a default time-step size of 30 seconds.  This default
        /// value of 30 seconds is selected as a balance between security and
        /// usability.</remarks>
        public int TimeStep { get; private set; }

        /// <summary>
        /// Milli-seconds before the current code is no longer valid.
        /// </summary>
        public long CodeTimeout
        {
            get
            {
                long curInt = GetCurrentTimeInterval(TimeStep);
                var ms = (long) (DateTime.UtcNow - UnixEpoch.AddSeconds(curInt*TimeStep)).TotalMilliseconds;
                return ms;
            }
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
        public byte[] RenderQrCode(string ident, int size=200)
        {
            Device device = Devices.Get(ident);
            return RenderQrCode(device.Ident, Decrypt(device.Key), QrCodeType.TOTP, size);

        }

        /// <summary>
        /// Generates a onetime password code
        /// </summary>
        /// <param name="key">shared secret</param>
        /// <param name="offset">interval offset, default is 0</param>
        /// <param name="length">length of code, six, seven or eight. Default is six</param>
        /// <param name="timeStep">timestep eg. secs code should work, default is 30</param>
        /// <returns>code generated</returns>
        public static string Generate(byte[] key, int offset = 0, CodeLength length = CodeLength.Six, int timeStep=30)
        {
            return Generate(key, GetCurrentTimeInterval(timeStep) + offset, length);
        }
    }
}
