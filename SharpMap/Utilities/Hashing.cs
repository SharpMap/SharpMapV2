using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace SharpMap.Utilities
{
    public static class Hashing
    {
        private const string Digits = "0123456789ABCDEF";
        private static readonly SHA1Managed _hash = new SHA1Managed();

        public static string Hash(Stream data)
        {
            long streamPos = data.Position;
            data.Seek(0, SeekOrigin.Begin);
            byte[] hashValue;

            lock (_hash)
                hashValue = _hash.ComputeHash(data);

            data.Position = streamPos;

            StringBuilder builder = new StringBuilder();

            foreach (byte b in hashValue)
            {
                builder.Append(Digits[b >> 4]);
                builder.Append(Digits[b & 0x07]);
            }

            return builder.ToString();
        }
    }
}
