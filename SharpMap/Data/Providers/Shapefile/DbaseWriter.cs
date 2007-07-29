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
using System.IO;
using System.Text;
using System.Data;

namespace SharpMap.Data.Providers
{
    internal class DbaseWriter : IDisposable
    {
        private const string NumberFormatTemplate = "{0,:F}";
        private DbaseHeader _header = new DbaseHeader();
        private FileStream _dbaseFileStream;
        private BinaryReader _dbaseReader;
        private BinaryWriter _dbaseWriter;
        private DataTable _schema;
        private bool _disposed = false;
        private StringBuilder _format = new StringBuilder(NumberFormatTemplate, 32);

        #region Object Construction/Destruction
        public DbaseWriter(string filename)
        {
            _dbaseFileStream = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            _dbaseWriter = new BinaryWriter(_dbaseFileStream, Encoding.ASCII);
            _dbaseReader = new BinaryReader(_dbaseFileStream);

            if (_dbaseFileStream.Length > 0)
            {
                _header = DbaseHeader.ParseDbfHeader(_dbaseReader);
                _schema = DbaseSchema.GetFeatureTableForFields(_header.Columns);
            }
        }

        public DbaseWriter(string filename, DataTable schema)
            : this(File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read), schema)
        { }

        public DbaseWriter(string filename, DbaseField[] fields)
            : this(File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read), fields)
        { }

        public DbaseWriter(FileStream file, DataTable schema)
        {
            _dbaseFileStream = file;
            _dbaseWriter = new BinaryWriter(_dbaseFileStream, Encoding.ASCII);

            if (file.CanRead)
            {
                _dbaseReader = new BinaryReader(_dbaseFileStream);
            }

            _schema = schema;
            _header.Columns = DbaseSchema.GetFields(schema);
            writeFullHeader();
        }

        public DbaseWriter(FileStream file, DbaseField[] fields)
        {
            _dbaseFileStream = file;
            _dbaseWriter = new BinaryWriter(_dbaseFileStream, Encoding.ASCII);
            
            if (file.CanRead)
            {
                _dbaseReader = new BinaryReader(_dbaseFileStream);
            }
            
            _header.Columns = fields;
            _schema = DbaseSchema.GetFeatureTableForFields(fields);
            writeFullHeader();
        }

        ~DbaseWriter()
        {
            Dispose(false);
        }

        #region Dispose Pattern
        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (!Disposed)
            {
                Dispose(true);
                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) // Deterministically dispose managed resources
            {
                if (_dbaseWriter != null)
                {
                    _dbaseWriter.Close();
                }

                if (_dbaseReader != null)
                {
                    _dbaseReader.Close();
                }

                if (_dbaseFileStream != null)
                {
                    _dbaseFileStream.Close();
                }
            }
        }

        protected internal bool Disposed
        {
            get { return _disposed; }
            private set { _disposed = value; }
        }
        #endregion

        public void Close()
        {
            (this as IDisposable).Dispose();
        }
        #endregion

        public DbaseField[] Columns
        {
            get { return _header == null ? null : _header.Columns; }
            set
            {
                if (_header != null)
                {
                    throw new InvalidOperationException("Can't set columns after schema has been defined.");
                }

                _header = new DbaseHeader();
                _header.Columns = value;
            }
        }

        public void AddRows(DataTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException("table");
            }

            foreach (DataRow row in table.Rows)
            {
                AddRow(row);
            }
        }

        public void AddRow(DataRow row)
        {
            _dbaseWriter.Seek((int)(_header.RecordCount * _header.RecordLength + _header.HeaderLength), SeekOrigin.Begin);

            writeRow(row);
            _dbaseWriter.Seek(0, SeekOrigin.End);
            _dbaseWriter.Write(DbaseConstants.FileTerminator);
            _header.LastUpdate = DateTime.Now;
            _header.RecordCount++;
            updateHeader();
            _dbaseWriter.Flush();
        }

        public void DeleteRow(UInt32 rowIndex)
        {
            throw new NotImplementedException("Not implemented in this version.");
        }

        public void UpdateRow(UInt32 rowIndex, DataRow row)
        {
            if (rowIndex < 0 || rowIndex >= _header.RecordCount)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            _dbaseWriter.Seek((int)(rowIndex * _header.RecordLength + _header.HeaderLength), SeekOrigin.Begin);
            writeRow(row);

            _header.LastUpdate = DateTime.Now;
            _dbaseWriter.Flush();
        }

        private void writeRow(DataRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException("row");
            }

            if (row.Table == null)
            {
                throw new ArgumentException("Row must be associated to a table.");
            }

            _dbaseWriter.Write(DbaseConstants.NotDeletedIndicator);

            for (int i = 0; i < _header.Columns.Length; i++)
            {
                int rowColumnIndex = row.Table.Columns.Contains(DbaseSchema.OidColumnName) ? i + 1 : i;

                // TODO: reconsider type checking
                //if ((_header.Columns[i].DataType != row.Table.Columns[rowColumnIndex].DataType) || (_header.Columns[i].Length != row.Table.Columns[rowColumnIndex].MaxLength))
                //    throw new SchemaMismatchException(String.Format("Row doesn't match this DbaseWriter schema at column {0}", i));

                switch (Type.GetTypeCode(_header.Columns[i].DataType))
                {
                    case TypeCode.Boolean:
                        if (row[rowColumnIndex] == null || row[rowColumnIndex] == DBNull.Value)
                        {
                            _dbaseWriter.Write(DbaseConstants.BooleanNullChar);
                        }
                        else
                        {
                            writeBoolean((bool)row[rowColumnIndex]);
                        }
                        break;
                    case TypeCode.DateTime:
                        if (row[rowColumnIndex] == null || row[rowColumnIndex] == DBNull.Value)
                        {
                            writeNullDateTime();
                        }
                        else
                        {
                            writeDateTime((DateTime)row[rowColumnIndex]);
                        }
                        break;
                    case TypeCode.Single:
                    case TypeCode.Double:
                        if (row[rowColumnIndex] == null || row[rowColumnIndex] == DBNull.Value)
                        {
                            writeNullNumber(_header.Columns[i].Length);
                        }
                        else
                        {
                            writeNumber(Convert.ToDouble(row[rowColumnIndex]), _header.Columns[i].Length, _header.Columns[i].Decimals);
                        }
                        break;
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                        if (row[rowColumnIndex] == null || row[rowColumnIndex] == DBNull.Value)
                        {
                            writeNullNumber(_header.Columns[i].Length);
                        }
                        else
                        {
                            writeNumber(Convert.ToInt64(row[rowColumnIndex]), _header.Columns[i].Length);
                        }
                        break;
                    case TypeCode.String:
                        if (row[rowColumnIndex] == null || row[rowColumnIndex] == DBNull.Value)
                        {
                            writeNullString(_header.Columns[i].Length);
                        }
                        else
                        {
                            writeString((string)row[rowColumnIndex], _header.Columns[i].Length);
                        }
                        break;
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Decimal:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.DBNull:
                    case TypeCode.Empty:
                    case TypeCode.Object:
                    default:
                        throw new NotSupportedException(String.Format("Type not supported: {0}", _header.Columns[i].DataType));
                }
            }
        }

        private void writeNullDateTime()
        {
            _dbaseWriter.Write(DbaseConstants.NullDateValue);
        }

        private void writeNullString(int length)
        {
            _dbaseWriter.Write(new String('\0', length));
        }

        private void writeNullNumber(int length)
        {
            _dbaseWriter.Write(new String(DbaseConstants.NumericNullIndicator, length));
        }

        private void writeNumber(double value, short length, byte decimalPlaces)
        {
            // Create number format string
            _format.Length = 0;
            _format.Append(NumberFormatTemplate);
            _format.Insert(5, decimalPlaces).Insert(3, length);
            string number = String.Format(SharpMap.Map.NumberFormat_EnUS, _format.ToString(), value);
            _dbaseWriter.Write(number);
        }

        private void writeNumber(long value, short length)
        {
            // Create number format string
            writeNumber(value, length, 0);
        }

        private void writeDateTime(DateTime dateTime)
        {
            _dbaseWriter.Write(dateTime.ToString("yyyyMMDD"));
        }

        private void writeString(string value, int length)
        {
            value = (value ?? String.Empty) + new String('\0', length);
            _dbaseWriter.Write(value.Substring(0, length));
        }

        private void writeBoolean(bool value)
        {
            _dbaseWriter.Write(value ? "T" : "F");
        }

        private void updateHeader()
        {
            _dbaseWriter.Seek(1, SeekOrigin.Begin);
            _dbaseWriter.Write(new byte[3] { (byte)(_header.LastUpdate.Year - DbaseConstants.DbaseEpoch), (byte)_header.LastUpdate.Month, (byte)_header.LastUpdate.Day });
            _dbaseWriter.Write(_header.RecordCount);
        }

        private void writeFullHeader()
        {
            _dbaseWriter.Seek(0, SeekOrigin.Begin);
            _dbaseWriter.Write((byte)DbaseConstants.DbfVersionCode);
            _dbaseWriter.Write(new byte[3] 
				{	(byte)(_header.LastUpdate.Year - DbaseConstants.DbaseEpoch), 
					(byte)_header.LastUpdate.Month, 
					(byte)_header.LastUpdate.Day 
				});
            _dbaseWriter.Write(_header.RecordCount);
            _dbaseWriter.Write(_header.HeaderLength);
            _dbaseWriter.Write(_header.RecordLength);
            _dbaseWriter.Write(new byte[DbaseConstants.EncodingOffset - (int)_dbaseWriter.BaseStream.Position]);
            _dbaseWriter.Write(_header.LanguageDriver);
            _dbaseWriter.Write(new byte[2]);

            foreach (DbaseField field in _header.Columns)
            {
                string colName = field.ColumnName + new String('\0', DbaseConstants.FieldNameLength);
                byte[] colNameBytes = Encoding.ASCII.GetBytes(colName.Substring(0, DbaseConstants.FieldNameLength));
                _dbaseWriter.Write(colNameBytes);
                char fieldTypeCode = DbaseSchema.GetFieldTypeCode(field.DataType);
                _dbaseWriter.Write(fieldTypeCode);
                _dbaseWriter.Write(field.Address);

                if (fieldTypeCode == 'N' || fieldTypeCode == 'F')
                {
                    _dbaseWriter.Write((byte)field.Length);
                    _dbaseWriter.Write(field.Decimals);
                }
                else
                {
                    _dbaseWriter.Write((byte)field.Length);
                    _dbaseWriter.Write((byte)(field.Length >> 8));
                }

                _dbaseWriter.Write(new byte[14]);
            }

            _dbaseWriter.Write(DbaseConstants.HeaderTerminator);
        }
    }
}
