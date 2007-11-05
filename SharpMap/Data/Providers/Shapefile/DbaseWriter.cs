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
    internal partial class DbaseFile
    {
        private class DbaseWriter : IDisposable
        {
            private readonly DbaseFile _dbaseFile;
            private const string NumberFormatTemplate = "{0,:F}";
            private readonly StringBuilder _format = new StringBuilder(NumberFormatTemplate, 32);
            private readonly BinaryReader _binaryReader;
            private readonly BinaryWriter _binaryWriter;
            private bool _disposed = false;

            #region Object Construction/Destruction
            public DbaseWriter(DbaseFile file)
			{
            	_dbaseFile = file;
                _binaryWriter = new BinaryWriter(file.DataStream, file.Encoding);
                _binaryReader = new BinaryReader(file.DataStream, file.Encoding);
            }

            ~DbaseWriter()
            {
                dispose(false);
            }

            #region Dispose Pattern
            #region IDisposable Members

            public void Dispose()
            {
                if (IsDisposed)
                {
                    return;
                }

                dispose(true);
                IsDisposed = true;
                GC.SuppressFinalize(this);
            }

            #endregion

            private void dispose(bool disposing)
            {
                if (disposing) // Deterministically dispose managed resources
                {
                    if (_binaryWriter != null)
                    {
                        _binaryWriter.Close();
                    }

                    if (_binaryReader != null)
                    {
                        _binaryReader.Close();
                    }
                }
            }

            internal bool IsDisposed
            {
                get { return _disposed; }
                private set { _disposed = value; }
            }
            #endregion
            #endregion

            internal void BeginWrite()
            {
            }

            internal void EndWrite()
            {
                _binaryWriter.Flush();
                _binaryWriter.Seek(0, SeekOrigin.End);
                _binaryWriter.Write(DbaseConstants.FileTerminator);
            }

            internal void UpdateHeader(DbaseHeader header)
            {
                _binaryWriter.Seek(1, SeekOrigin.Begin);
                byte[] dateBytes = new byte[3] { 
                    (byte)(header.LastUpdate.Year - DbaseConstants.DbaseEpoch),
                    (byte)header.LastUpdate.Month, 
                    (byte)header.LastUpdate.Day };
                _binaryWriter.Write(dateBytes);
                _binaryWriter.Write(header.RecordCount);
            }

            internal void WriteFullHeader(DbaseHeader header)
            {
                _binaryWriter.Seek(0, SeekOrigin.Begin);
                _binaryWriter.Write(DbaseConstants.DbfVersionCode);
                UpdateHeader(header);
                _binaryWriter.Write(header.HeaderLength);
                _binaryWriter.Write(header.RecordLength);
                _binaryWriter.Write(new byte[DbaseConstants.EncodingOffset - (int)_binaryWriter.BaseStream.Position]);
                _binaryWriter.Write(header.LanguageDriver);
                _binaryWriter.Write(new byte[2]);

                foreach (DbaseField field in header.Columns)
                {
                    string colName = field.ColumnName + new String('\0', DbaseConstants.FieldNameLength);
                    byte[] colNameBytes = Encoding.ASCII.GetBytes(colName.Substring(0, DbaseConstants.FieldNameLength));
                    _binaryWriter.Write(colNameBytes);
                    char fieldTypeCode = DbaseSchema.GetFieldTypeCode(field.DataType);
                    _binaryWriter.Write(fieldTypeCode);
                    _binaryWriter.Write(0); // Address field isn't supported

                    if (fieldTypeCode == 'N' || fieldTypeCode == 'F')
                    {
                        _binaryWriter.Write((byte)field.Length);
                        _binaryWriter.Write(field.Decimals);
                    }
                    else
                    {
                        _binaryWriter.Write((byte)field.Length);
                        _binaryWriter.Write((byte)(field.Length >> 8));
                    }

                    _binaryWriter.Write(new byte[14]);
                }

                _binaryWriter.Write(DbaseConstants.HeaderTerminator);
            }

            internal void WriteRow(DataRow row)
            {
                if (row == null)
                {
                    throw new ArgumentNullException("row");
                }

                if (row.Table == null)
                {
                    throw new ArgumentException("Row must be associated to a table.");
                }

                _binaryWriter.Write(DbaseConstants.NotDeletedIndicator);

                DbaseHeader header = _dbaseFile._header;

                foreach (DbaseField column in header.Columns)
                {
					if(!row.Table.Columns.Contains(column.ColumnName) 
						|| String.Compare(column.ColumnName, DbaseSchema.OidColumnName,
							StringComparison.CurrentCultureIgnoreCase) == 0)
					{
						continue;
					}

                    // TODO: reconsider type checking
                    //if ((header.Columns[rowColumnIndex].DataType != row.Table.Columns[rowColumnIndex].DataType) || (header.Columns[rowColumnIndex].Length != row.Table.Columns[rowColumnIndex].MaxLength))
                    //    throw new SchemaMismatchException(String.Format("Row doesn't match this DbaseWriter schema at column {0}", i));

                    switch (Type.GetTypeCode(column.DataType))
                    {
                        case TypeCode.Boolean:
							if (row[column.ColumnName] == null || row[column.ColumnName] == DBNull.Value)
                            {
                                _binaryWriter.Write(DbaseConstants.BooleanNullChar);
                            }
                            else
                            {
								writeBoolean((bool)row[column.ColumnName]);
                            }
                            break;
                        case TypeCode.DateTime:
							if (row[column.ColumnName] == null || row[column.ColumnName] == DBNull.Value)
                            {
                                writeNullDateTime();
                            }
                            else
                            {
								writeDateTime((DateTime)row[column.ColumnName]);
                            }
                            break;
                        case TypeCode.Single:
                        case TypeCode.Double:
							if (row[column.ColumnName] == null || row[column.ColumnName] == DBNull.Value)
                            {
                                writeNullNumber(column.Length);
                            }
                            else
                            {
								writeNumber(Convert.ToDouble(row[column.ColumnName]), 
									column.Length, column.Decimals);
                            }
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
							if (row[column.ColumnName] == null || row[column.ColumnName] == DBNull.Value)
                            {
                                writeNullNumber(column.Length);
                            }
                            else
                            {
								writeNumber(Convert.ToInt64(row[column.ColumnName]), column.Length);
                            }
                            break;
                        case TypeCode.String:
							if (row[column.ColumnName] == null || row[column.ColumnName] == DBNull.Value)
                            {
                                writeNullString(column.Length);
                            }
                            else
                            {
								writeString((string)row[column.ColumnName], column.Length);
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
                            throw new NotSupportedException(String.Format(
								"Type not supported: {0}", column.DataType));
                    }
                }
            }

            #region Private helper methods
            private void writeNullDateTime()
            {
                _binaryWriter.Write(DbaseConstants.NullDateValue);
            }

            private void writeNullString(int length)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(new String('\0', length));
                _binaryWriter.Write(bytes);
            }

            private void writeNullNumber(int length)
            {
                _binaryWriter.Write(new String(DbaseConstants.NumericNullIndicator, length));
            }

            private void writeNumber(double value, short length, byte decimalPlaces)
            {
                // Create number format string
                _format.Length = 0;
                _format.Append(NumberFormatTemplate);
                _format.Insert(5, decimalPlaces).Insert(3, length);
                string number = String.Format(DbaseConstants.StorageNumberFormat, _format.ToString(), value);
                byte[] bytes = Encoding.ASCII.GetBytes(number);
                _binaryWriter.Write(bytes);
            }

            private void writeNumber(long value, short length)
            {
                // Create number format string
                writeNumber(value, length, 0);
            }

            private void writeDateTime(DateTime dateTime)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(dateTime.ToString("yyyyMMdd"));
                _binaryWriter.Write(bytes);
            }

            private void writeString(string value, int length)
            {
                value = (value ?? String.Empty) + new String((char)0x0, length);
                byte[] chars = _dbaseFile.Encoding.GetBytes(value.Substring(0, length));
                _binaryWriter.Write(chars);
            }

            private void writeBoolean(bool value)
            {
                byte[] bytes = value ? Encoding.ASCII.GetBytes("T") : Encoding.ASCII.GetBytes("F");
                _binaryWriter.Write(bytes);
            }
            #endregion
        }
    }
}
