/*
 * Based upon initial work from Michael Petito http://stackoverflow.com/users/185200/michael-petito
 * and Shane, http://stackoverflow.com/users/904128/shane thanks for that. ;-)
 */

using System;
using System.Globalization;
using System.Text;

namespace SharpOtpLib
{
    /// <summary>
    /// Converts strings to base32 byte arrays and the other way around
    /// For more information on Base32 see:
    /// http://en.wikipedia.org/wiki/Base32 or http://tools.ietf.org/html/rfc4648
    /// </summary>
    public static class Base32
    {

        /// <summary>
        /// Converts byte array to ASCII string
        /// </summary>
        /// <param name="data">byte array to decode into a string</param>
        /// <returns>decoded string</returns>
        public static string Convert(byte[] data)
        {
            const int inByteSize = 8;
            const int outByteSize = 5;
            const string base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";

            int i = 0, index = 0;
            var builder = new StringBuilder((data.Length + 7) * inByteSize / outByteSize);

            while (i < data.Length)
            {
                int currentByte = data[i];
                int digit;

                //Is the current digit going to span a byte boundary?
                if (index > (inByteSize - outByteSize))
                {
                    int nextByte;

                    if ((i + 1) < data.Length)
                    {
                        nextByte = data[i + 1];
                    }
                    else
                    {
                        nextByte = 0;
                    }

                    digit = currentByte & (0xFF >> index);
                    index = (index + outByteSize) % inByteSize;
                    digit <<= index;
                    digit |= nextByte >> (inByteSize - index);
                    i++;
                }
                else
                {
                    digit = (currentByte >> (inByteSize - (index + outByteSize))) & 0x1F;
                    index = (index + outByteSize) % inByteSize;

                    if (index == 0)
                    {
                        i++;
                    }
                }

                builder.Append(base32Alphabet[digit]);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Convert a base32 complient ASCII string to byte array
        /// </summary>
        /// <param name="input">valid characters are: ABCDEFGHIJKLMNOPQRSTUVWXYZ234567</param>
        /// <returns>base32 encoded byte array</returns>
        /// <exception cref="ArgumentNullException">if string is null or empty</exception>
        /// <exception cref="ArgumentException">if string contains non-valid base32 characters</exception>
        public static byte[] Convert(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException("input");
            }

            input = input.TrimEnd('='); //remove padding characters
            int byteCount = input.Length * 5 / 8; //this must be TRUNCATED
            byte[] returnArray = new byte[byteCount];

            byte curByte = 0, bitsRemaining = 8;
            int arrayIndex = 0;

            foreach (char c in input)
            {
                int cValue;
                try
                {
                     cValue = CharToValue(c);
                }
                catch (ArgumentException)
                {
                    string s = c.ToString(CultureInfo.InvariantCulture) +
                               " is not a valid base 32 character. Allowed characters are: ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
                    throw new ArgumentException(s, "input");
                }

                int mask;
                if (bitsRemaining > 5)
                {
                    mask = cValue << (bitsRemaining - 5);
                    curByte = (byte)(curByte | mask);
                    bitsRemaining -= 5;
                }
                else
                {
                    mask = cValue >> (5 - bitsRemaining);
                    curByte = (byte)(curByte | mask);
                    returnArray[arrayIndex++] = curByte;
                    curByte = (byte)(cValue << (3 + bitsRemaining));
                    bitsRemaining += 3;
                }
            }

            //if we didn't end with a full byte
            if (arrayIndex != byteCount)
            {
                returnArray[arrayIndex] = curByte;
            }

            return returnArray;
        }

        private static int CharToValue(char c)
        {
            var value = (int)c;

            //65-90 == uppercase letters
            if (value < 91 && value > 64)
            {
                return value - 65;
            }
            //50-55 == numbers 2-7
            if (value < 56 && value > 49)
            {
                return value - 24;
            }
            //97-122 == lowercase letters
            if (value < 123 && value > 96)
            {
                return value - 97;
            }

            throw new ArgumentException("Character is not a Base32 character.", "c");
        }
    }
}
