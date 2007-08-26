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

using System.Collections.Generic;
using System.Text;

namespace SharpMap.Data.Providers.ShapeFile
{
    internal static class DbaseEncodingRegistry
    {
        private static readonly Dictionary<byte, Encoding> _dbaseToEncoding = new Dictionary<byte, Encoding>();

        static DbaseEncodingRegistry()
        {
            _dbaseToEncoding[0x01] = Encoding.GetEncoding(437); //DOS USA code page 437 
            _dbaseToEncoding[0x02] = Encoding.GetEncoding(850); // DOS Multilingual code page 850 
            _dbaseToEncoding[0x03] = Encoding.GetEncoding(1252); // Windows ANSI code page 1252 
            _dbaseToEncoding[0x04] = Encoding.GetEncoding(10000); // Standard Macintosh 
            _dbaseToEncoding[0x08] = Encoding.GetEncoding(865); // Danish OEM
            _dbaseToEncoding[0x09] = Encoding.GetEncoding(437); // Dutch OEM
            _dbaseToEncoding[0x0A] = Encoding.GetEncoding(850); // Dutch OEM Secondary codepage
            _dbaseToEncoding[0x0B] = Encoding.GetEncoding(437); // Finnish OEM
            _dbaseToEncoding[0x0D] = Encoding.GetEncoding(437); // French OEM
            _dbaseToEncoding[0x0E] = Encoding.GetEncoding(850); // French OEM Secondary codepage
            _dbaseToEncoding[0x0F] = Encoding.GetEncoding(437); // German OEM
            _dbaseToEncoding[0x10] = Encoding.GetEncoding(850); // German OEM Secondary codepage
            _dbaseToEncoding[0x11] = Encoding.GetEncoding(437); // Italian OEM
            _dbaseToEncoding[0x12] = Encoding.GetEncoding(850); // Italian OEM Secondary codepage
            _dbaseToEncoding[0x13] = Encoding.GetEncoding(932); // Japanese Shift-JIS
            _dbaseToEncoding[0x14] = Encoding.GetEncoding(850); // Spanish OEM secondary codepage
            _dbaseToEncoding[0x15] = Encoding.GetEncoding(437); // Swedish OEM
            _dbaseToEncoding[0x16] = Encoding.GetEncoding(850); // Swedish OEM secondary codepage
            _dbaseToEncoding[0x17] = Encoding.GetEncoding(865); // Norwegian OEM
            _dbaseToEncoding[0x18] = Encoding.GetEncoding(437); // Spanish OEM
            _dbaseToEncoding[0x19] = Encoding.GetEncoding(437); // English OEM (Britain)
            _dbaseToEncoding[0x1A] = Encoding.GetEncoding(850); // English OEM (Britain) secondary codepage
            _dbaseToEncoding[0x1B] = Encoding.GetEncoding(437); // English OEM (U.S.)
            _dbaseToEncoding[0x1C] = Encoding.GetEncoding(863); // French OEM (Canada)
            _dbaseToEncoding[0x1D] = Encoding.GetEncoding(850); // French OEM secondary codepage
            _dbaseToEncoding[0x1F] = Encoding.GetEncoding(852); // Czech OEM
            _dbaseToEncoding[0x22] = Encoding.GetEncoding(852); // Hungarian OEM
            _dbaseToEncoding[0x23] = Encoding.GetEncoding(852); // Polish OEM
            _dbaseToEncoding[0x24] = Encoding.GetEncoding(860); // Portuguese OEM
            _dbaseToEncoding[0x25] = Encoding.GetEncoding(850); // Portuguese OEM secondary codepage
            _dbaseToEncoding[0x26] = Encoding.GetEncoding(866); // Russian OEM
            _dbaseToEncoding[0x37] = Encoding.GetEncoding(850); // English OEM (U.S.) secondary codepage
            _dbaseToEncoding[0x40] = Encoding.GetEncoding(852); // Romanian OEM
            _dbaseToEncoding[0x4D] = Encoding.GetEncoding(936); // Chinese GBK (PRC)
            _dbaseToEncoding[0x4E] = Encoding.GetEncoding(949); // Korean (ANSI/OEM)
            _dbaseToEncoding[0x4F] = Encoding.GetEncoding(950); // Chinese Big5 (Taiwan)
            _dbaseToEncoding[0x50] = Encoding.GetEncoding(874); // Thai (ANSI/OEM)
            _dbaseToEncoding[0x57] = Encoding.GetEncoding(1252); // ANSI
            _dbaseToEncoding[0x58] = Encoding.GetEncoding(1252); // Western European ANSI
            _dbaseToEncoding[0x59] = Encoding.GetEncoding(1252); // Spanish ANSI
            _dbaseToEncoding[0x64] = Encoding.GetEncoding(852); // Eastern European MS–DOS
            _dbaseToEncoding[0x65] = Encoding.GetEncoding(866); // Russian MS–DOS
            _dbaseToEncoding[0x66] = Encoding.GetEncoding(865); // Nordic MS–DOS
            _dbaseToEncoding[0x67] = Encoding.GetEncoding(861); // Icelandic MS–DOS
            //_dbaseToEncoding[0x68] = Encoding.GetEncoding(895); // Kamenicky (Czech) MS-DOS 
            //_dbaseToEncoding[0x69] = Encoding.GetEncoding(620); // Mazovia (Polish) MS-DOS 
            _dbaseToEncoding[0x6A] = Encoding.GetEncoding(737); // Greek MS–DOS (437G)
            _dbaseToEncoding[0x6B] = Encoding.GetEncoding(857); // Turkish MS–DOS
            _dbaseToEncoding[0x6C] = Encoding.GetEncoding(863); // French–Canadian MS–DOS
            _dbaseToEncoding[0x78] = Encoding.GetEncoding(950); // Taiwan Big 5
            _dbaseToEncoding[0x79] = Encoding.GetEncoding(949); // Hangul (Wansung)
            _dbaseToEncoding[0x7A] = Encoding.GetEncoding(936); // PRC GBK
            _dbaseToEncoding[0x7B] = Encoding.GetEncoding(932); // Japanese Shift-JIS
            _dbaseToEncoding[0x7C] = Encoding.GetEncoding(874); // Thai Windows/MS–DOS
            _dbaseToEncoding[0x7D] = Encoding.GetEncoding(1255); // Hebrew Windows 
            _dbaseToEncoding[0x7E] = Encoding.GetEncoding(1256); // Arabic Windows 
            _dbaseToEncoding[0x86] = Encoding.GetEncoding(737); // Greek OEM
            _dbaseToEncoding[0x87] = Encoding.GetEncoding(852); // Slovenian OEM
            _dbaseToEncoding[0x88] = Encoding.GetEncoding(857); // Turkish OEM
            _dbaseToEncoding[0x96] = Encoding.GetEncoding(10007); // Russian Macintosh 
            _dbaseToEncoding[0x97] = Encoding.GetEncoding(10029); // Eastern European Macintosh 
            _dbaseToEncoding[0x98] = Encoding.GetEncoding(10006); // Greek Macintosh 
            _dbaseToEncoding[0xC8] = Encoding.GetEncoding(1250); // Eastern European Windows
            _dbaseToEncoding[0xC9] = Encoding.GetEncoding(1251); // Russian Windows
            _dbaseToEncoding[0xCA] = Encoding.GetEncoding(1254); // Turkish Windows
            _dbaseToEncoding[0xCB] = Encoding.GetEncoding(1253); // Greek Windows
            _dbaseToEncoding[0xCC] = Encoding.GetEncoding(1257); // Baltic Windows
        }

        public static Encoding GetEncoding(byte dbasecode)
        {
            Encoding encoding;

            if (_dbaseToEncoding.TryGetValue(dbasecode, out encoding))
            {
                return encoding;
            }
            else
            {
                return Encoding.ASCII;
            }
        }
    }
}
