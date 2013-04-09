using System;

namespace SharpOtpLib
{
    /// <summary>
    /// If a device is not found in the device "database", this is thrown.
    /// </summary>
    public class DeviceNotFoundException : Exception
    {
        /// <summary>
        /// The specified device ident was not found.
        /// </summary>
        /// <param name="ident">Ident of device</param>
        /// <param name="message">Message to user</param>
        public DeviceNotFoundException(string ident, string message) :base(message)
        {
            Ident = ident;
        }

        /// <summary>
        /// The specified device ident was not found.
        /// </summary>
        /// <param name="ident">Ident of device</param>
        /// <param name="message">Message to user</param>
        /// <param name="innerException">Inner exception if any</param>
        public DeviceNotFoundException(string ident, string message, Exception innerException):base(message, innerException)
        {
            Ident = ident;
        }

        /// <summary>
        /// Ident of token device
        /// </summary>
        public string Ident { get; private set; }
        
    }
}
