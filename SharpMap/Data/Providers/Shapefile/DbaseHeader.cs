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
using System.IO;
using System.Text;
using System.Data;

namespace SharpMap.Data.Providers.ShapeFile
{
    internal class DbaseHeader
    {
        private DateTime _lastUpdate = DateTime.Now;
        private UInt32 _numberOfRecords;
        private Int16 _headerLength;
        private Int16 _recordLength;
        private byte _languageDriver;
        private DbaseField[] _dbaseColumns;

        internal byte LanguageDriver
        {
            get { return _languageDriver; }
            set { _languageDriver = value; }
        }

        internal DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = value; }
        }

        internal UInt32 RecordCount
        {
            get { return _numberOfRecords; }
            set { _numberOfRecords = value; }
        }

        internal DbaseField[] Columns
        {
            get { return _dbaseColumns; }
            set
            {
                _dbaseColumns = value;
                HeaderLength = (short)((DbaseConstants.ColumnDescriptionLength * _dbaseColumns.Length) 
                    + DbaseConstants.ColumnDescriptionOffset + 1 /* For terminator */);
                RecordLength = 1; //Deleted flag length

                Array.ForEach(_dbaseColumns, delegate(DbaseField field) { RecordLength += field.Length; });
            }
        }

        internal Int16 HeaderLength
        {
            get { return _headerLength; }
            private set { _headerLength = value; }
        }

        internal Int16 RecordLength
        {
            get { return _recordLength; }
            private set { _recordLength = value; }
        }

        internal Encoding FileEncoding
        {
            get { return DbaseEncodingRegistry.GetEncoding(LanguageDriver); }
        }

        public override string ToString()
        {
            return String.Format("[DbaseHeader] Records: {0}; Columns: {1}; Last Update: {2}; "+
                "Record Length: {3}; Header Length: {4}", RecordCount, Columns.Length, 
                LastUpdate, RecordLength, HeaderLength);
        }

        /// <summary>
        /// Returns a DataTable that describes the column metadata of the DBase file.
        /// </summary>
        /// <returns>A DataTable that describes the column metadata.</returns>
        internal DataTable GetSchemaTable()
        {
            DataTable schema = ProviderSchemaHelper.CreateSchemaTable();

            foreach (DbaseField dbf in _dbaseColumns)
            {
                schema.Columns.Add(dbf.ColumnName, dbf.DataType);
            }

            for (int i = 0; i < _dbaseColumns.Length; i++)
            {
                DataRow r = schema.NewRow();
                r[ProviderSchemaHelper.ColumnNameColumn] = _dbaseColumns[i].ColumnName;
                r[ProviderSchemaHelper.ColumnSizeColumn] = _dbaseColumns[i].Length;
                r[ProviderSchemaHelper.ColumnOrdinalColumn] = i;
                r[ProviderSchemaHelper.NumericPrecisionColumn] = _dbaseColumns[i].Decimals;
                r[ProviderSchemaHelper.NumericScaleColumn] = 0;
                r[ProviderSchemaHelper.DataTypeColumn] = _dbaseColumns[i].DataType;
                r[ProviderSchemaHelper.AllowDBNullColumn] = true;
                r[ProviderSchemaHelper.IsReadOnlyColumn] = true;
                r[ProviderSchemaHelper.IsUniqueColumn] = false;
                r[ProviderSchemaHelper.IsRowVersionColumn] = false;
                r[ProviderSchemaHelper.IsKeyColumn] = false;
                r[ProviderSchemaHelper.IsAutoIncrementColumn] = false;
                r[ProviderSchemaHelper.IsLongColumn] = false;

                // specializations, if ID is unique
                //if (_ColumnNames[i] == "ID")
                //	r["IsUnique"] = true;

                schema.Rows.Add(r);
            }

            return schema;
        }

        internal static DbaseHeader ParseDbfHeader(BinaryReader reader)
        {
            DbaseHeader header = new DbaseHeader();

            if (reader.ReadByte() != DbaseConstants.DbfVersionCode)
            {
                throw new NotSupportedException("Unsupported DBF Type");
            }

            header.LastUpdate = new DateTime(reader.ReadByte() + DbaseConstants.DbaseEpoch, 
                reader.ReadByte(), reader.ReadByte()); //Read the last update date
            header.RecordCount = reader.ReadUInt32(); // read number of records.
            short storedHeaderLength = reader.ReadInt16(); // read length of header structure.
            short storedRecordLength = reader.ReadInt16(); // read length of a record
            reader.BaseStream.Seek(DbaseConstants.EncodingOffset, SeekOrigin.Begin); //Seek to encoding flag
            header.LanguageDriver = reader.ReadByte(); //Read and parse Language driver
            reader.BaseStream.Seek(DbaseConstants.ColumnDescriptionOffset, SeekOrigin.Begin); //Move past the reserved bytes
            int numberOfColumns = (storedHeaderLength - DbaseConstants.ColumnDescriptionOffset) /
                DbaseConstants.ColumnDescriptionLength;  // calculate the number of DataColumns in the header

            DbaseField[] columns = new DbaseField[numberOfColumns];

            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = new DbaseField();
                columns[i].ColumnName = header.FileEncoding.GetString(reader.ReadBytes(11)).Replace("\0", "").Trim();
                char fieldtype = reader.ReadChar();
                columns[i].Address = reader.ReadInt32();
                short fieldLength = reader.ReadByte();

                if (fieldtype == 'N' || fieldtype == 'F')
                {
                    columns[i].Decimals = reader.ReadByte();
                }
                else
                {
                    fieldLength += (short)(reader.ReadByte() << 8);
                }

                columns[i].Length = fieldLength;

                switch (fieldtype)
                {
                    case 'L':
                        columns[i].DataType = typeof(bool);
                        break;
                    case 'C':
                        columns[i].DataType = typeof(string);
                        break;
                    case 'D':
                        columns[i].DataType = typeof(DateTime);
                        break;
                    case 'N':
                        //If the number doesn't have any decimals, make the type an integer, if possible
                        if (columns[i].Decimals == 0)
                        {
                            if (columns[i].Length <= 4) columns[i].DataType = typeof(Int16);
                            else if (columns[i].Length <= 9) columns[i].DataType = typeof(Int32);
                            else if (columns[i].Length <= 18) columns[i].DataType = typeof(Int64);
                            else columns[i].DataType = typeof(double);
                        }
                        else
                            columns[i].DataType = typeof(double);
                        break;
                    case 'F':
                        columns[i].DataType = typeof(float);
                        break;
                    case 'B':
                        columns[i].DataType = typeof(byte[]);
                        break;
                    default:
                        throw new NotSupportedException("Invalid or unknown DBase field type '" + 
                            fieldtype + "' in column '" + columns[i].ColumnName + "'");
                }

                // Move stream to next field record
                reader.BaseStream.Seek(DbaseConstants.BytesFromEndOfDecimalInFieldRecord, SeekOrigin.Current);
            }

            header.Columns = columns;

            if (storedHeaderLength != header.HeaderLength)
            {
                throw new InvalidDbaseFileException("Recorded header length doesn't equal computed header length.");
            }

            if (storedRecordLength != header.RecordLength)
            {
                throw new InvalidDbaseFileException("Recorded record length doesn't equal computed record length.");
            }

            return header;
        }
    }
}
