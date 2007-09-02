// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using SharpMap.Features;
using SharpMap.Indexing.BinaryTree;

namespace SharpMap.Data.Providers.ShapeFile
{
    internal partial class DbaseFile : IDisposable
    {
        private class DbaseReader : IDisposable
        {
            #region Instance fields
            private readonly DbaseFile _dbaseFile;
            private BinaryReader _dbaseReader;
            private bool _isDisposed = false;
            private bool _isOpen;

            #endregion

            #region Object Construction/Destruction

            /// <summary>
            /// Creates a new instance of a <see cref="DbaseReader"/> for the
			/// <paramref name="file" />.
            /// </summary>
			/// <param name="file">The controlling DbaseFile instance.</param>
            public DbaseReader(DbaseFile file)
            {
                _dbaseFile = file;
                _dbaseReader = new BinaryReader(file.DataStream, file.Encoding);
            }

            #region Dispose Pattern

            ~DbaseReader()
            {
                dispose(false);
            }

            /// <summary>
            /// Gets a value which indicates if this object is disposed: 
            /// <see langword="true"/> if it is, <see langword="false"/> otherwise
            /// </summary>
            /// <seealso cref="Dispose"/>
            internal bool IsDisposed
            {
                get { return _isDisposed; }
                private set { _isDisposed = value; }
            }

            #region IDisposable members

            /// <summary>
            /// Closes all files and disposes of all resources.
            /// </summary>
            /// <seealso cref="Close"/>
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
                if (IsDisposed)
                {
                    return;
                }

                if (disposing) // Do deterministic finalization of managed resources
                {
                    // Closing the reader closes the file stream
                    if (_dbaseReader != null) _dbaseReader.Close();
                    _dbaseReader = null;
                }
            }

            #endregion

            #endregion

            internal bool IsRowDeleted(UInt32 row)
            {
                long rowOffset = _dbaseFile.ComputeByteOffsetToRecord(row);
                _dbaseFile.DataStream.Seek(rowOffset, SeekOrigin.Begin);
                return _dbaseReader.ReadChar() == DbaseConstants.DeletedIndicator;
            }

            /// <summary>
            /// Gets a value for the given <paramref name="row">row index</paramref> and 
            /// <paramref name="column">column index</paramref>.
            /// </summary>
            /// <param name="row">
            /// Index of the row to retrieve value from. Zero-based.
            /// </param>
            /// <param name="column">
            /// Index of the column to retrieve value from. Zero-based.
            /// </param>
            /// <returns>
            /// The value at the given (row, column).
            /// </returns>
            /// <exception cref="ObjectDisposedException">
            /// Thrown when the method is called and 
            /// object has been disposed.
            /// </exception>
            /// <exception cref="InvalidDbaseFileOperationException">
            /// Thrown if this reader is 
            /// closed (check <see cref="IsOpen"/> before calling), or if the column is an 
            /// unsupported type.
            /// </exception>
            /// <exception cref="ArgumentOutOfRangeException">
            /// Thrown if <paramref name="oid"/> is 
            /// less than 0 or greater than <see cref="RecordCount"/> - 1, 
            /// or if <paramref name="colId"/> is less than 0 or greater than 
            /// <see cref="Columns.Length"/> - 1.
            /// </exception>
            internal object GetValue(uint row, DbaseField column)
            {
                checkState();

                DbaseHeader header = _dbaseFile._header;

                if (!_isOpen)
                {
                    throw new InvalidDbaseFileOperationException(
                        "An attempt was made to read from a closed DBF file");
                }

                if (row < 0 || row >= header.RecordCount)
                {
                    throw new ArgumentOutOfRangeException(
                        "Invalid row requested at index " + row);
                }

                // Compute the position in the file stream for the requested row and column
                long offset = header.HeaderLength + (row * header.RecordLength);
                offset += _dbaseFile.ComputeByteOffsetToFieldInRecord(column);

                // Seek to the computed offset
                _dbaseFile.DataStream.Seek(offset, SeekOrigin.Begin);

                try
                {
                    return readDbfValue(column);
                }
                catch (NotSupportedException)
                {
                    throw new InvalidDbaseFileOperationException(
                        String.Format("Column type {0} is not supported.", column.DataType));
                }
            }

            #region Private helper methods

            private void checkState()
            {
                if (_isDisposed)
                {
                    throw new ObjectDisposedException(
                        "Attempt to access a disposed DbaseReader object");
                }
            }

            private object readDbfValue(DbaseField dbf)
            {
                if (ReferenceEquals(dbf, null))
                {
                    throw new ArgumentNullException("dbf");
                }

                switch (Type.GetTypeCode(dbf.DataType))
                {
                    case TypeCode.Boolean:
                        char tempChar = (char)_dbaseReader.ReadByte();
                        return ((tempChar == 'T') || (tempChar == 't') || (tempChar == 'Y') || (tempChar == 'y'));

                    case TypeCode.DateTime:
                        DateTime date;
                        // Mono has not yet implemented DateTime.TryParseExact
#if !MONO
                        if (DateTime.TryParseExact(Encoding.UTF8.GetString((_dbaseReader.ReadBytes(8))),
                                                   "yyyyMMdd", DbaseConstants.StorageNumberFormat, 
                                                   DateTimeStyles.None, out date))
                        {
                            return date;
                        }
                        else
                        {
                            return DBNull.Value;
                        }
#else
					try 
					{
						return date = DateTime.ParseExact ( Encoding.UTF7.GetString((br.ReadBytes(8))), 	
						"yyyyMMdd", Map.numberFormat_EnUS, DateTimeStyles.None );
					}
					catch ( Exception e )
					{
						return DBNull.Value;
					}
#endif
                    case TypeCode.Double:
                        string temp = Encoding.UTF8.GetString(_dbaseReader.ReadBytes(dbf.Length)).Replace("\0", "").Trim();
                        double dbl;

                        if (double.TryParse(temp, NumberStyles.Float, DbaseConstants.StorageNumberFormat, out dbl))
                        {
                            return dbl;
                        }
                        else
                        {
                            return DBNull.Value;
                        }

                    case TypeCode.Int16:
                        string temp16 =
                            Encoding.UTF8.GetString((_dbaseReader.ReadBytes(dbf.Length))).Replace("\0", "").Trim();
                        Int16 i16;

                        if (Int16.TryParse(temp16, NumberStyles.Float, DbaseConstants.StorageNumberFormat, out i16))
                        {
                            return i16;
                        }
                        else
                        {
                            return DBNull.Value;
                        }

                    case TypeCode.Int32:
                        string temp32 =
                            Encoding.UTF8.GetString((_dbaseReader.ReadBytes(dbf.Length))).Replace("\0", "").Trim();
                        Int32 i32;

                        if (Int32.TryParse(temp32, NumberStyles.Float, DbaseConstants.StorageNumberFormat, out i32))
                        {
                            return i32;
                        }
                        else
                        {
                            return DBNull.Value;
                        }

                    case TypeCode.Int64:
                        string temp64 =
                            Encoding.UTF8.GetString((_dbaseReader.ReadBytes(dbf.Length))).Replace("\0", "").Trim();
                        Int64 i64 = 0;

                        if (Int64.TryParse(temp64, NumberStyles.Float, DbaseConstants.StorageNumberFormat, out i64))
                        {
                            return i64;
                        }
                        else
                        {
                            return DBNull.Value;
                        }

                    case TypeCode.Single:
                        string temp4 = Encoding.UTF8.GetString((_dbaseReader.ReadBytes(dbf.Length)));
                        float f = 0;

                        if (float.TryParse(temp4, NumberStyles.Float, DbaseConstants.StorageNumberFormat, out f))
                        {
                            return f;
                        }
                        else
                        {
                            return DBNull.Value;
                        }

                    case TypeCode.String:
                        {
                            byte[] chars = _dbaseReader.ReadBytes(dbf.Length);
                            string value = _dbaseFile.Encoding.GetString(chars);
                            return value.Replace("\0", "").Trim();
                        }

                    case TypeCode.Char:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.Byte:
                    case TypeCode.DBNull:
                    case TypeCode.Object:
                    case TypeCode.SByte:
                    case TypeCode.Empty:
                    case TypeCode.Decimal:
                    default:
                        throw new NotSupportedException("Cannot parse DBase field '"
                                                        + dbf.ColumnName + "' of type '" + dbf.DataType + "'");
                }
            }
            #endregion
        }
    }
}