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

// Note:
// Good stuff on DBase format: http://www.clicketyclick.dk/databases/xbase/format/

using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using SharpMap.Indexing.BinaryTree;

namespace SharpMap.Data.Providers.ShapeFile
{
    internal class DbaseReader : IDisposable
    {
        private static readonly NumberFormatInfo NumberFormat_enUS
            = new CultureInfo("en-US", false).NumberFormat;

        #region Instance fields

        private readonly string _filename;
        private FeatureDataTable<uint> _baseTable;
        private FileStream _dbaseFileStream;
        private BinaryReader _dbaseReader;
        private Encoding _encoding;
        private DbaseHeader _header;
        private bool _headerIsParsed;
        private bool _isDisposed = false;
        private bool _isOpen;

        #endregion

        #region Object Construction/Destruction

        /// <summary>
        /// Creates a new instance of a <see cref="DbaseReader"/> from the 
        /// <paramref name="filename"> specified path</paramref>.
        /// </summary>
        /// <param name="filename">The path of the DBF file to read</param>
        /// <exception cref="FileNotFoundException"><paramref name="filename">The passed 
        /// filename</paramref> doesn't exist</exception>
        public DbaseReader(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(String.Format("Could not find file \"{0}\"", filename));
            }

            _filename = filename;
            _headerIsParsed = false;
        }

        ~DbaseReader()
        {
            Dispose(false);
        }

        #region Dispose Pattern

        /// <summary>
        /// Gets a value which indicates if this object is disposed: true if it is, false otherwise
        /// </summary>
        /// <seealso cref="Dispose"/>
        public bool IsDisposed
        {
            get { return _isDisposed; }
            private set { _isDisposed = value; }
        }

        /// <summary>
        /// Closes all files and disposes of all resources.
        /// </summary>
        /// <seealso cref="Close"/>
        void IDisposable.Dispose()
        {
            if (!IsDisposed)
            {
                Dispose(true);
                IsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) // Do deterministic finalization of managed resources
            {
                if (_dbaseReader != null) _dbaseReader.Close();
                _dbaseReader = null;
                _dbaseFileStream = null;
                if (_baseTable != null) _baseTable.Dispose();
                _baseTable = null;
            }

            _isOpen = false;
        }

        /// <summary>
        /// Closes the xBase file.
        /// </summary>
        /// <seealso cref="IsOpen"/>
        /// <seealso cref="Open()"/>
        /// <seealso cref="Dispose" />
        /// <exception cref="ObjectDisposedException">Thrown when the method is called and
        /// object has been disposed.</exception>
        public void Close()
        {
            (this as IDisposable).Dispose();
        }

        #endregion

        #endregion

        /// <summary>
        /// Gets or sets the <see cref="System.Text.Encoding"/> used for parsing strings 
        /// from the DBase DBF file.
        /// </summary>
        /// <remarks>
        /// If the encoding type isn't set, the dbase driver will try to determine 
        /// the correct <see cref="System.Text.Encoding"/>.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when the property is 
        /// fetched and/or set and object has been disposed.
        /// </exception>
        internal Encoding Encoding
        {
            get
            {
                checkState();
                return _encoding;
            }
            set
            {
                checkState();
                _encoding = value;
            }
        }

        /// <summary>
        /// Gets a value which indicates if the reader is open: 
        /// true if it is, false otherwise.
        /// </summary>
        internal bool IsOpen
        {
            get { return _isOpen; }
        }

        /// <summary>
        /// Returns an empty <see cref="FeatureDataTable"/> 
        /// with the same schema as this reader.
        /// </summary>
        internal FeatureDataTable<uint> NewTable
        {
            get
            {
                checkState();
                return _baseTable.Clone();
            }
        }

        /// <summary>
        /// Number of records in the DBF file.
        /// </summary>
        internal UInt32 RecordCount
        {
            get
            {
                checkState(); 
                return _header.RecordCount; 
            }
        }

        /// <summary>
        /// Gets the feature at the specified Object ID
        /// </summary>
        /// <param name="oid">Row index. Zero-based.</param>
        /// <param name="table">The feature table containing the schema used to 
        /// read the row.</param>
        /// <returns>The feature row for the given row index</returns>
        /// <exception cref="InvalidDbaseFileOperationException">
        /// Thrown if this reader is closed (check <see cref="IsOpen"/> before calling), 
        /// or if the column is an unsupported type.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="oid"/> 
        /// &lt; 0 or <paramref name="oid"/> &gt;= <see cref="RecordCount"/></exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="table"/> is 
        /// null</exception>
        /// <exception cref="ObjectDisposedException">Thrown when the method is called and 
        /// object has been disposed</exception>
        internal FeatureDataRow<uint> GetFeature(uint oid, FeatureDataTable<uint> table)
        {
            checkState();

            if (!_isOpen)
            {
                throw new InvalidDbaseFileOperationException("An attempt was made to read from a closed dBase file");
            }

            if (oid < 0 || oid >= RecordCount)
            {
                throw new ArgumentOutOfRangeException("Invalid DataRow requested at index " + oid.ToString());
            }

            if (ReferenceEquals(table, null))
            {
                throw new ArgumentNullException("table");
            }

            _dbaseFileStream.Seek((oid * _header.RecordLength) + _header.HeaderLength, SeekOrigin.Begin);

            if (_dbaseReader.ReadChar() == DbaseConstants.DeletedIndicator) //is record marked deleted?
            {
                return null;
            }

            FeatureDataRow<uint> dr = table.NewRow(oid);

            foreach (DbaseField field in _header.Columns)
            {
                try
                {
                    dr[field.ColumnName] = readDbfValue(field);
                }
                catch (NotSupportedException)
                {
                    throw new InvalidDbaseFileOperationException(
                        String.Format("Column type {0} is not supported.", field.DataType));
                }
            }

            return dr;
        }

        /// <summary>
        /// Gets a value for the given <paramref name="oid">row index</paramref> and 
        /// <paramref name="colId">column index</paramref>
        /// </summary>
        /// <param name="oid">Index of the row to retrieve value from. Zero-based.</param>
        /// <param name="colId">Index of the column to retrieve value from. Zero-based.</param>
        /// <returns>The value at the given (row, column).</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the method is called and 
        /// object has been disposed</exception>
        /// <exception cref="InvalidDbaseFileOperationException">Thrown if this reader is 
        /// closed (check <see cref="IsOpen"/> before calling), or if the column is an 
        /// unsupported type.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="oid"/> is 
        /// less than 0 or greater than <see cref="RecordCount"/> - 1, 
        /// or if <paramref name="colId"/> is less than 0 or greater than 
        /// <see cref="Columns.Length"/> - 1.</exception>
        internal object GetValue(uint oid, int colId)
        {
            checkState();

            if (!_isOpen)
            {
                throw new InvalidDbaseFileOperationException("An attempt was made to read from a closed DBF file");
            }

            if (oid < 0 || oid >= _header.RecordCount)
            {
                throw new ArgumentOutOfRangeException("Invalid DataRow requested at index " + oid);
            }

            if (colId >= _header.Columns.Length || colId < 0)
            {
                throw new ArgumentOutOfRangeException("colId", colId, "Column index out of range.");
            }

            // Compute the position in the file stream for the requested row and column
            long offset = _header.HeaderLength + oid * _header.RecordLength;

            for (int i = 0; i < colId; i++)
            {
                offset += _header.Columns[i].Length;
            }

            // Seek to the computed offset
            _dbaseFileStream.Seek(offset, SeekOrigin.Begin);

            try
            {
                return readDbfValue(_header.Columns[colId]);
            }
            catch (NotSupportedException)
            {
                throw new InvalidDbaseFileOperationException(
                    String.Format("Column type {0} is not supported.", _header.Columns[colId].DataType));
            }
        }

        internal DataTable GetSchemaTable()
        {
            checkState();
            return _header.GetSchemaTable();
        }

        /// <summary>
        /// Opens the <see cref="DbaseReader"/> on the file specified in the constructor.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when the method is called 
        /// and object has been disposed.
        /// </exception>
        internal void Open()
        {
            Open(false);
        }

        /// <summary>
        /// Opens the <see cref="DbaseReader"/> on the file specified in the constructor, 
        /// with a value determining if the file is locked for exclusive read access or not.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when the method is called 
        /// and object has been disposed.
        /// </exception>
        internal void Open(bool exclusive)
        {
            checkState();

            // TODO: implement asynchronous reading
            if (exclusive)
            {
                _dbaseFileStream = new FileStream(_filename, FileMode.Open, FileAccess.Read,
                                                  FileShare.Read, 4096, FileOptions.None);
            }
            else
            {
                _dbaseFileStream = new FileStream(_filename, FileMode.OpenOrCreate, FileAccess.Read,
                                                  FileShare.ReadWrite, 4096, FileOptions.None);
            }

            _dbaseReader = new BinaryReader(_dbaseFileStream);
            _isOpen = true;

            if (!_headerIsParsed) //Don't read the header if it's already parsed
            {
                _header = DbaseHeader.ParseDbfHeader(_dbaseReader);
                _baseTable = DbaseSchema.GetFeatureTableForFields(_header.Columns);
                _headerIsParsed = true;
            }
        }

        #region Private helper methods

        private void checkState()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Attempt to access a disposed DbaseReader object");
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
                                               "yyyyMMdd", NumberFormat_enUS, DateTimeStyles.None, out date))
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

                    if (double.TryParse(temp, NumberStyles.Float, NumberFormat_enUS, out dbl))
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

                    if (Int16.TryParse(temp16, NumberStyles.Float, NumberFormat_enUS, out i16))
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

                    if (Int32.TryParse(temp32, NumberStyles.Float, NumberFormat_enUS, out i32))
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

                    if (Int64.TryParse(temp64, NumberStyles.Float, NumberFormat_enUS, out i64))
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

                    if (float.TryParse(temp4, NumberStyles.Float, NumberFormat_enUS, out f))
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
                        string value = (_encoding == null)
                                           ? _header.FileEncoding.GetString(chars)
                                           : _encoding.GetString(chars);
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


        // Binary Tree not working yet on Mono 
        // see bug: http://bugzilla.ximian.com/show_bug.cgi?id=78502
#if !MONO
        /// <summary>
        /// Indexes a DBF column in a binary tree (B-Tree) [NOT COMPLETE]
        /// </summary>
        /// <typeparam name="TValue">datatype to be indexed</typeparam>
        /// <param name="columnId">Column to index</param>
        /// <returns>A <see cref="SharpMap.Indexing.BinaryTree.BinaryTree{T, UInt32}"/> data 
        /// structure indexing values in a column to a row index</returns>
        /// <exception cref="ObjectDisposedException">Thrown when the method is called and 
        /// object has been disposed</exception>
        private BinaryTree<UInt32, TValue> createDbfIndex<TValue>(int columnId) where TValue : IComparable<TValue>
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("Attempt to access a disposed DbaseReader object");
            }

            BinaryTree<UInt32, TValue> tree = new BinaryTree<UInt32, TValue>();

            for (uint i = 0; i < _header.RecordCount; i++)
            {
                tree.Add(new BinaryTree<uint, TValue>.ItemValue(i, (TValue)GetValue(i, columnId)));
            }

            return tree;
        }
#endif
        #endregion

        #region Lucene Indexing (EXPERIMENTAL)

        /*
		/// <summary>
		/// Creates an index on the columns for faster searching [EXPERIMENTAL - Requires Lucene dependencies]
		/// </summary>
		/// <returns></returns>
		public string CreateLuceneIndex()
		{
			string dir = this._filename + ".idx";
			if (!System.IO.Directory.Exists(dir))
				System.IO.Directory.CreateDirectory(dir);
			Lucene.Net.Index.IndexWriter iw = new Lucene.Net.Index.IndexWriter(dir,new Lucene.Net.Analysis.Standard.StandardAnalyzer(),true);

			for (uint i = 0; i < this._NumberOfRecords; i++)
			{
				FeatureDataRow dr = GetFeature(i,this.NewTable);
				Lucene.Net.Documents.Document doc = new Lucene.Net.Documents.Document();
				// Add the object-id as a field, so that index can be maintained.
				// This field is not stored with document, it is indexed, but it is not
	            // tokenized prior to indexing.
				//doc.Add(Lucene.Net.Documents.Field.UnIndexed("SharpMap_oid", i.ToString())); //Add OID index

				foreach(System.Data.DataColumn col in dr.Table.Columns) //Add and index values from DBF
				{
					if(col.DataType.Equals(typeof(string)))
						// Add the contents as a valued Text field so it will get tokenized and indexed.
						doc.Add(Lucene.Net.Documents.Field.UnStored(col.ColumnName,(string)dr[col]));
					else
						doc.Add(Lucene.Net.Documents.Field.UnStored(col.ColumnName, dr[col].ToString()));
				}
				iw.AddDocument(doc);
			}
			iw.Optimize();
			iw.Close();
			return this._filename + ".idx";
		}
		*/

        #endregion
    }
}