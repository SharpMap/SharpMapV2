using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Data.Providers
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
    }
}
