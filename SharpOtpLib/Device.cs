using System;
using System.Collections.Generic;

namespace SharpOtpLib
{
    /// <summary>
    /// This class holds information about a OneTimePassword devices (aka token device).
    /// A devices can be a physical device, softdevice or something else. Think of a
    /// physical device as an RSA or VASCO token or the Google Authenticator software.
    /// </summary>
    /// <remarks>
    /// Background info (RFC4226):
    /// A tamper-resistant devices MUST be used to store the master key and
    /// derive the shared secrets from the master key and some public
    /// information.  The main benefit would be to avoid the exposure of the
    /// shared secrets at any time and also avoid specific requirements on
    /// storage, since the shared secrets could be generated on-demand when
    /// needed at provisioning and validation time.
    /// </remarks>
    [Serializable]
    public class Device

{

    /// <summary>
    /// Ident for devices, this is a unique id for the device.
    /// </summary>
    public string Ident { get; set; }

    /// <summary>
    /// Secret key for devices. Note: this is the encrypted key! 
    /// Use GetKey to get an unencrypted key.
    /// Use SetKey to set an unencrypted key.
    /// </summary>
    public byte[] Key { get; set; }

    /// <summary>
    /// Number of failures for devices
    /// </summary>
    public int Failures { get; set; }

    /// <summary>
    /// Offset for TOTP, can be positive and negative
    /// Does not apply to HOTP
    /// </summary>
    public int TotpOffset { get; set; }

    /// <summary>
    /// Counter for HOTP, this does not apply to TOTP
    /// This should be incremented on every use.
    /// </summary>
    public long HotpCounter { get; set; }

    /// <summary>
    /// Used Conters for TOTP so that we cannot use the same
    /// code twice.
    /// </summary>
    public List<long> TotpUsedCounter { get; set; }

  }
}
