// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;

namespace SharpMap.Utilities
{
    public static class ByteEncoder
    {
        #region Endian conversion helper routines
        /// <summary>
        /// Returns the value encoded in Big Endian (PPC, XDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Big-endian encoded value</returns>
        public static int GetBigEndian(int value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return swapByteOrder(value);
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Returns the value encoded in Big Endian (PPC, XDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Big-endian encoded value</returns>
        public static UInt16 GetBigEndian(UInt16 value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return swapByteOrder(value);
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Returns the value encoded in Big Endian (PPC, XDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Big-endian encoded value</returns>
        public static UInt32 GetBigEndian(UInt32 value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return swapByteOrder(value);
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Returns the value encoded in Big Endian (PPC, XDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Big-endian encoded value</returns>
        public static double GetBigEndian(double value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return swapByteOrder(value);
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Returns the value encoded in Little Endian (x86, NDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Little-endian encoded value</returns>
        public static int GetLittleEndian(int value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return value;
            }
            else
            {
                return swapByteOrder(value);
            }
        }

        /// <summary>
        /// Returns the value encoded in Little Endian (x86, NDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Little-endian encoded value</returns>
        public static UInt32 GetLittleEndian(UInt32 value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return value;
            }
            else
            {
                return swapByteOrder(value);
            }
        }

        /// <summary>
        /// Returns the value encoded in Little Endian (x86, NDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Little-endian encoded value</returns>
        public static UInt16 GetLittleEndian(UInt16 value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return value;
            }
            else
            {
                return swapByteOrder(value);
            }
        }

        /// <summary>
        /// Returns the value encoded in Little Endian (x86, NDR) format
        /// </summary>
        /// <param name="value">Value to encode</param>
        /// <returns>Little-endian encoded value</returns>
        public static double GetLittleEndian(double value)
        {
            if (BitConverter.IsLittleEndian)
            {
                return value;
            }
            else
            {
                return swapByteOrder(value);
            }
        }

        /// <summary>
        /// Swaps the byte order of an Int32
        /// </summary>
        /// <param name="value">Int32 to swap</param>
        /// <returns>Byte Order swapped Int32</returns>
        public static int swapByteOrder(int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer, 0, buffer.Length);
            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>
        /// Swaps the byte order of a UInt16
        /// </summary>
        /// <param name="value">UInt16 to swap</param>
        /// <returns>Byte Order swapped UInt16</returns>
        private static UInt16 swapByteOrder(UInt16 value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer, 0, buffer.Length);
            return BitConverter.ToUInt16(buffer, 0);
        }

        /// <summary>
        /// Swaps the byte order of a UInt32
        /// </summary>
        /// <param name="value">UInt32 to swap</param>
        /// <returns>Byte Order swapped UInt32</returns>
        private static UInt32 swapByteOrder(UInt32 value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer, 0, buffer.Length);
            return BitConverter.ToUInt32(buffer, 0);
        }

        /// <summary>
        /// Swaps the byte order of a Double (double precision IEEE 754)
        /// </summary>
        /// <param name="value">Double to swap</param>
        /// <returns>Byte Order swapped Double</returns>
        private static double swapByteOrder(double value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Reverse(buffer, 0, buffer.Length);
            return BitConverter.ToDouble(buffer, 0);
        }
        #endregion
    }
}
