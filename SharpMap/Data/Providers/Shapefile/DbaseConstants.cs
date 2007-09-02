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
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace SharpMap.Data.Providers.ShapeFile
{
    internal class DbaseConstants
    {
        internal static readonly byte DbfVersionCode = 0x03;
        internal static readonly int EncodingOffset = 29;
        internal static readonly int ColumnDescriptionOffset = 32;
        internal static readonly int ColumnDescriptionLength = 32;
        internal static readonly int BytesFromEndOfDecimalInFieldRecord = 14;
        internal static readonly char NumericNullIndicator = '*';
        internal static readonly char DeletedIndicator = '*';
        internal static readonly char NotDeletedIndicator = ' ';
        internal static readonly char BooleanNullChar = '?';
        internal static readonly string NullDateValue = new String('0', 8);
        internal static readonly byte HeaderTerminator = 0x0d;
        internal static readonly byte FileTerminator = 0x1a;
        internal static readonly int DbaseEpoch = 1900;
        internal static readonly int FieldNameLength = 11;

        internal static readonly CultureInfo StorageNumberFormat 
            = new CultureInfo("en-US", false);
    }
}
