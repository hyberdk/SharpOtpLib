using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace SharpOtpLib
{
    /// <summary>
    /// This is the class that stores devices. Remember that a device could be a physical token like RSA or Vasco, but
    /// it can also be a soft-device like Google Authenticator etc. This class stores information about all the 
    /// different devices that is configured in TOTP or HOTP. There should be no need to create your own instance of
    /// this class as its used solely by the HOTP/TOTP classes.
    /// </summary>
    public class Devices
    {
        private List<Device> _db = new List<Device>();
        private int _maxAttempts = 3;
        private readonly byte[] _aesKey;
        private readonly byte[] _aesIv;

        /// <summary>
        /// initiates a new "device database", you need to supply a encryption key and vector.
        /// </summary>
        /// <param name="aesKey">AES encryption key, max 256bit</param>
        /// <param name="aesIv">AES Initialization Vector, usually 128bit</param>
        public Devices(byte[] aesKey, byte[] aesIv)
        {
            _aesKey = aesKey;
            _aesIv = aesIv;
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
        internal int MaxAttempts { get { return _maxAttempts; } set { _maxAttempts = value; } }

        /// <summary>
        /// Checkes if the current devices is locked, meaning that it will always return
        /// failed back. Please use the method UnLock to unlock the devices.
        /// </summary>
        /// <param name="ident">ident of devices</param>
        /// <returns>true if maximum number of attempts has been tried for devices, false if not (or device not found)</returns>
        public bool IsLocked(string ident)
        {
            return _db.Any(devices => devices.Ident == ident && devices.Failures >= MaxAttempts);
        }

        /// <summary>
        /// Unlocks a devices. This resets number of failures for a devices to 0
        /// </summary>
        /// <param name="ident">ident of devices</param>
        /// <remarks>Does not throw and exception if ident not found</remarks>
        public void UnLock(string ident)
        {
            foreach (Device devices in _db.Where(devices => devices.Ident == ident))
            {
                devices.Failures = 0;
            }
        }

        internal bool TryGet(string ident, out Device devices)
        {
            devices = null;
            if (ident == null) return false;

            foreach (var d in _db)
            {
                if (d.Ident != ident) continue;
                devices = d;
                return true;
            }
            //devices not found, returning false.
            return false;
        }

        /// <summary>
        /// Adds a new OTP devices.
        /// </summary>
        /// <param name="ident">Ident of the HOTP devices, this could also be a username.
        /// We just need something unique to identify by.</param>
        /// <param name="key">Secret key</param>
        /// <param name="offset">Optional HOTP start code offset, default is 0. This is not used for TOTP</param>
        /// <exception cref="ArgumentException">if ident already exist</exception>
        public void Add(string ident, byte[] key, int offset = 0)
        {

            lock (_db)
            {
                if (_db.Any(devices => devices.Ident == ident))
                {
                    throw new ArgumentException("Ident already present", "ident");
                }

                _db.Add(new Device
                {
                    HotpCounter = offset,
                    Ident = ident,
                    Key = Utils.Encrypt(key, _aesKey, _aesIv),
                    Failures = 0
                });
            }
        }

        /// <summary>
        /// Flushes all devices currently available in "database"
        /// </summary>
        public void Flush()
        {
            _db = new List<Device>();
        }

        /// <summary>
        /// Get the secret key for a device
        /// </summary>
        /// <param name="ident">ident of device</param>
        /// <returns>Unencrypted secret key</returns>
        /// <exception cref="DeviceNotFoundException">if device ident is not found</exception>
        public byte[] GetKey(string ident)
        {
            Device device = Get(ident);
            return Utils.Decrypt(device.Key, _aesKey, _aesIv);
        }

        /// <summary>
        /// Sets a secret key for an ident. Use this to change the secret for a
        /// specific device.
        /// </summary>
        /// <param name="ident">ident of device</param>
        /// <param name="key">Unencrypted secret key</param>
        /// <exception cref="DeviceNotFoundException">if device ident is not found</exception>
        public void SetKey(string ident, byte[] key)
        {
            Device device = Get(ident);
            device.Key = Utils.Encrypt(key, _aesKey, _aesIv);
        }

        /// <summary>
        /// Get the offset (time skew) for a device. This only applies
        /// to TOTP devices. It always will return 0 for a HOTP device
        /// </summary>
        /// <param name="ident">ident of device</param>
        /// <returns>Offset in timesteps</returns>
        /// <exception cref="DeviceNotFoundException">if device ident is not found</exception>
        public int GetOffset(string ident)
        {
            Device device = Get(ident);
            return device.TotpOffset;
        }

        /// <summary>
        /// Get the current counter, this only applies to HOTP devices.
        /// If a TOTP device, this will return 0
        /// </summary>
        /// <param name="ident">ident of device</param>
        /// <returns>returns the current counter seed</returns>
        /// <exception cref="DeviceNotFoundException">if device ident is not found</exception>
        public long GetCounter(string ident)
        {
            Device device = Get(ident);
            return device.HotpCounter;
        }

        /// <summary>
        /// Get the number of wrong codes tested by a device (failures)
        /// </summary>
        /// <param name="ident">ident of device</param>
        /// <returns>number of failures since last success</returns>
        /// <exception cref="DeviceNotFoundException">if device ident is not found</exception>
        public int GetFailures(string ident)
        {
            Device device = Get(ident);
            return device.Failures;
        }

        /// <summary>
        /// Gets a list of device idents available
        /// </summary>
        /// <returns>list of idents</returns>
        public List<string> GetIdents()
        {
            var idents = new List<string>();
            lock (_db)
            {
                idents.AddRange(_db.Select(device => device.Ident));
            }
            return idents;
        } 

        /// <summary>
        /// Removes a OTP devices
        /// </summary>
        /// <param name="ident">Ident of devices to remove</param>
        public void Remove(string ident)
        {
            lock (_db)
            {
                int index = 0;
                while (index < _db.Count)
                {
                    if (_db[index].Ident == ident)
                    {
                        _db.RemoveAt(index);
                    }
                    index++;
                }
            }
        }

        internal Device Get(string ident)
        {
            Device devices;
            if (ident == null) throw new ArgumentNullException("ident");
            if (!TryGet(ident, out devices) || devices == null) 
                throw new DeviceNotFoundException(ident, "Ident not found");
            return devices;
        }

        /// <summary>
        /// Checks if a deivce exist in the "database"
        /// </summary>
        /// <param name="ident">ident of devices</param>
        /// <returns>true if found, false if not.</returns>
        public bool Contains(string ident)
        {
            return _db.Any(devices => devices.Ident == ident);
        }

        /// <summary>
        /// Saves a device database to disk. Please note that this is not thread safe!
        /// </summary>
        /// <param name="path">full path where to write xml file containing Devices</param>
        /// <exception cref="UnauthorizedAccessException">Access is denied.</exception>
        /// <exception cref="ArgumentException">path is an empty string ("") or path contains the name of a system devices (com1, com2, and so on).</exception>
        /// <exception cref="ArgumentNullException">path is null.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive. </exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. 
        /// For example, on Windows-based platforms, paths must be less than 248 characters, 
        /// and file names must be less than 260 characters. </exception>
        /// <exception cref="IOException">path includes an incorrect or invalid syntax for file name, directory name, or volume label syntax. </exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public void Save(string path)
        {
            var serializer = new XmlSerializer(typeof(List<Device>), new XmlRootAttribute("EncryptedList"));

            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, _db);
            }
        }

        /// <summary>
        /// Loads an existing device "database" into memory. After it has been loaded
        /// each key is decrypted temporary to make sure that decryption is working.
        /// </summary>
        /// <param name="path">full path to xml file containing Devices</param>
        /// <exception cref="ArgumentException">path is an empty string ("").</exception>
        /// <exception cref="ArgumentNullException">path is null.</exception>
        /// <exception cref="FileNotFoundException">The file cannot be found.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid, such as being on an unmapped drive.</exception>
        /// <exception cref="IOException">path includes an incorrect or invalid syntax for file name, directory name, or volume label. </exception>
        /// <exception cref="InvalidOperationException">An error occurred during deserialization. The original exception is available using the InnerException property.</exception>
        /// <exception cref="OtpDecryptionException">One or more keys failed the decryption test which means that at lease some keys are not usable.
        /// You should check that the xml file has not been tampered with or that you use the correct AES key and Initialization Vector (IV)</exception>
        public void Load(string path)
        {
            var deserializer = new XmlSerializer(typeof (List<Device>), new XmlRootAttribute("EncryptedList"));

            using (TextReader reader = new StreamReader(path))
            {
                _db = (List<Device>)deserializer.Deserialize(reader);
            }

            DecryptTest();
        }


        private void DecryptTest()
        {
            var failedkeys = new List<string>();
            foreach (Device devices in _db)
            {
                try
                {
                    Utils.Decrypt(devices.Key, _aesKey, _aesIv);
                }
                catch (CryptographicException)
                {
                    failedkeys.Add(devices.Ident);
                }
            }
            if (failedkeys.Count != 0)
            {
                throw new OtpDecryptionException("Keys failed to decrypt, your \"database\" is broken. Make sure you use the correct key and vector.", failedkeys.ToArray());
            }
        }
    }
}
