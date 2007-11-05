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
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Globalization;

namespace SharpMap.Data.Providers.ShapeFile
{
	/// <summary>
	/// Represents a dBase file used to store attribute data in a Shapefile.
	/// </summary>
    internal partial class DbaseFile : IDisposable
    {
        #region Nested types
        internal class BufferingStream : Stream
        {
            private Stream _baseStream;
            private bool _isDisposed;

            internal BufferingStream(Stream baseStream)
            {
                _baseStream = baseStream;
            }

            public override bool CanRead
            {
                get
                {
                    checkState();
                    return _baseStream.CanRead;
                }
            }

            public override bool CanSeek
            {
                get
                {
                    checkState();
                    return _baseStream.CanSeek;
                }
            }

            public override bool CanWrite
            {
                get
                {
                    checkState();
                    return _baseStream.CanWrite;
                }
            }

            public override void Flush()
            {
                checkState();
                _baseStream.Flush();
            }

            public override long Length
            {
                get
                {
                    checkState();
                    return _baseStream.Length;
                }
            }

            public override long Position
            {
                get
                {
                    checkState();
                    return _baseStream.Position;
                }
                set
                {
                    checkState();
                    _baseStream.Position = value;
                }
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                checkState();
                return _baseStream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                checkState();
                return _baseStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                checkState();
                _baseStream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                checkState();
                _baseStream.Write(buffer, offset, count);
            }

            public override void Close()
            {
            }

            protected override void Dispose(bool disposing)
            {
                IsDisposed = true;
                _baseStream = null;
            }

            public bool IsDisposed
            {
                get { return _isDisposed; }
                private set { _isDisposed = value; }
            }

            private void checkState()
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().ToString());
                }
            }
        } 
        #endregion

        #region Instance fields

        private readonly string _filename;
        private DbaseHeader _header;
        private FeatureDataTable<uint> _baseTable;
        private bool _headerIsParsed;
        private DbaseReader _reader;
        private DbaseWriter _writer;
        private FileStream _dbaseFileStream;
        private bool _isDisposed = false;
        private bool _isOpen;

        #endregion

        internal DbaseFile(string fileName)
            : this(fileName, true)
        {
        }

        private DbaseFile(string fileName, bool checkExists)
        {
            if (checkExists && !File.Exists(fileName))
            {
                throw new ArgumentException("Cannot find file", "fileName");
            }

            _headerIsParsed = false;
            _filename = fileName;
        }

        #region Dispose Pattern

        ~DbaseFile()
        {
            dispose(false);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            dispose(true);
            GC.SuppressFinalize(this);
            IsDisposed = true;
        }

        #endregion

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

        private void dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing) // Do deterministic finalization of managed resources
            {
                if (_baseTable != null) _baseTable.Dispose();
                _baseTable = null;

                if (_reader != null) _reader.Dispose();
                _reader = null;

                if (_writer != null) _writer.Dispose();
                _writer = null;
            }

            _isOpen = false;
        }
        #endregion

        #region Properties

        #region Columns Property
        public ICollection<DbaseField> Columns
        {
            get { return _header == null ? null : _header.Columns; }
            //set
            //{
            //    if (_header != null)
            //    {
            //        throw new InvalidOperationException(
            //            "Can't set columns after schema has been defined.");
            //    }

            //    _header.Columns = value;
            //}
        }
        #endregion

        internal CultureInfo CultureInfo
        {
            get
            {
                checkState();
                return DbaseLocaleRegistry.GetCulture(_header.LanguageDriver);
            }
        }

        private Stream DataStream
        {
            get { return _dbaseFileStream; }
        }

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
                return DbaseLocaleRegistry.GetEncoding(_header.LanguageDriver);
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
        #endregion

        #region Methods

        internal static DbaseFile CreateDbaseFile(string fileName, DataTable schema, CultureInfo culture, Encoding encoding)
        {
            DbaseFile file = new DbaseFile(fileName, false);
            byte languageDriverCode = DbaseLocaleRegistry.GetLanguageDriverCode(culture, encoding);
            file._header = new DbaseHeader(languageDriverCode, DateTime.Now, 0);
            file._header.Columns = DbaseSchema.GetFields(schema, file._header);
        	file._headerIsParsed = true;
			file.Open();
			file.Save();
            return file;
        }

        internal void Save()
        {
            _writer.BeginWrite();
            _writer.WriteFullHeader(_header);
            _writer.EndWrite();
        }

        internal void AddRow(DataRow row)
        {
            if (row == null) throw new ArgumentNullException("row");

            DataStream.Seek((int)ComputeByteOffsetToRecord(_header.RecordCount), 
                SeekOrigin.Begin);

            _writer.BeginWrite();
            _writer.WriteRow(row);
            _header.LastUpdate = DateTime.Now;
            _header.RecordCount++;
            _writer.UpdateHeader(_header);
            _writer.EndWrite();
        }

        internal void AddRows(DataTable table)
        {
            if (table == null) throw new ArgumentNullException("table");

            _writer.BeginWrite();

            foreach (DataRow row in table.Rows)
            {
                _writer.WriteRow(row);
                _header.RecordCount++;
            }

            _header.LastUpdate = DateTime.Now;
            _writer.UpdateHeader(_header);
            _writer.EndWrite();
        }

        /// <summary>
        /// Closes the xBase file.
        /// </summary>
        /// <seealso cref="IsOpen"/>
        /// <seealso cref="Open()"/>
        /// <seealso cref="Dispose" />
        /// <exception cref="ObjectDisposedException">
        /// Thrown when the method is called and
        /// object has been disposed.
        /// </exception>
        public void Close()
        {
            (this as IDisposable).Dispose();
        }

        internal long ComputeByteOffsetToRecord(UInt32 row)
        {
            return _header.HeaderLength + (row * _header.RecordLength);
        }

        internal void DeleteRow(UInt32 rowIndex)
        {
            // TODO: implement DbaseFile.DeleteRow
            throw new NotImplementedException("Not implemented in this version.");
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
        internal FeatureDataRow<uint> GetAttributes(uint oid, FeatureDataTable<uint> table)
        {
            checkState();

            if (!_isOpen)
            {
                throw new InvalidDbaseFileOperationException(
                    "An attempt was made to read from a closed dBase file");
            }

            if (oid < 0 || oid >= RecordCount)
            {
                throw new ArgumentOutOfRangeException(
                    "Invalid DataRow requested at index " + oid);
            }

            if (ReferenceEquals(table, null))
            {
                throw new ArgumentNullException("table");
            }


            if (_reader.IsRowDeleted(oid)) //is record marked deleted?
            {
                return null;
            }

            FeatureDataRow<uint> dr = table.NewRow(oid);

            foreach (DbaseField field in _header.Columns)
            {
                try
                {
                    dr[field.ColumnName] = _reader.GetValue(oid, field);
                }
                catch (NotSupportedException)
                {
                    throw new InvalidDbaseFileOperationException(
                        String.Format("Column type {0} is not supported.", field.DataType));
                }
            }

            return dr;
        }

        internal DataTable GetSchemaTable()
        {
            checkState();
            return _header.GetSchemaTable();
        }

		internal void SetTableSchema(FeatureDataTable target, SchemaMergeAction mergeAction)
		{
			_baseTable.MergeSchema(target, mergeAction);
		}

        /// <summary>
        /// Returns an empty <see cref="FeatureDataTable"/> 
        /// with the same schema as this DBase file.
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
        /// Opens the <see cref="DbaseReader"/> on the file 
        /// specified in the constructor.
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
        /// Opens the <see cref="DbaseFile"/> on the file name
        /// specified in the constructor, 
        /// with a value determining if the file is locked for 
        /// exclusive read access or not.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Thrown when the method is called 
        /// and object has been disposed.
        /// </exception>
        internal void Open(bool exclusive)
        {
            checkState();

            // TODO: implement asynchronous access
			_dbaseFileStream = new FileStream(_filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, 
				exclusive ? FileShare.None : FileShare.Read, 4096, FileOptions.None);

            _isOpen = true;

            if (!_headerIsParsed) //Don't read the header if it's already parsed
            {
				_header = DbaseHeader.ParseDbfHeader(new BufferingStream(DataStream));
                _baseTable = DbaseSchema.GetFeatureTableForFields(_header.Columns);
                _headerIsParsed = true;
            }

			_writer = new DbaseWriter(this);
			_reader = new DbaseReader(this);
        }

        internal void UpdateRow(UInt32 rowIndex, DataRow row)
        {
            if (row == null) throw new ArgumentNullException("row");

            if (rowIndex < 0 || rowIndex >= _header.RecordCount)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            DataStream.Seek(ComputeByteOffsetToRecord(rowIndex), SeekOrigin.Begin);
            _writer.BeginWrite();
            _writer.WriteRow(row);
            _header.LastUpdate = DateTime.Now;
            _writer.UpdateHeader(_header);
            _writer.EndWrite();
        }
        #endregion

        #region Private helper methods

        private void checkState()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(
                    "Attempt to access a disposed DbaseReader object");
            }
        }

//            // Binary Tree not working yet on Mono 
//            // see bug: http://bugzilla.ximian.com/show_bug.cgi?id=78502
//#if !MONO
//            /// <summary>
//            /// Indexes a DBF column in a binary tree (B-Tree) [NOT COMPLETE]
//            /// </summary>
//            /// <typeparam name="TValue">datatype to be indexed</typeparam>
//            /// <param name="columnId">Column to index</param>
//            /// <returns>A <see cref="SharpMap.Indexing.BinaryTree.BinaryTree{T, UInt32}"/> data 
//            /// structure indexing values in a column to a row index</returns>
//            /// <exception cref="ObjectDisposedException">Thrown when the method is called and 
//            /// object has been disposed</exception>
//            private BinaryTree<UInt32, TValue> createDbfIndex<TValue>(int columnId) where TValue : IComparable<TValue>
//            {
//                if (_isDisposed)
//                {
//                    throw new ObjectDisposedException(
//                        "Attempt to access a disposed DbaseReader object");
//                }

//                BinaryTree<UInt32, TValue> tree = new BinaryTree<UInt32, TValue>();

//                for (uint i = 0; i < _header.RecordCount; i++)
//                {
//                    tree.Add(new BinaryTree<uint, TValue>.ItemValue(i, (TValue)GetValue(i, columnId)));
//                }

//                return tree;
//            }
//#endif

        //    #region Lucene Indexing (EXPERIMENTAL)

        //    /*
        ///// <summary>
        ///// Creates an index on the columns for faster searching [EXPERIMENTAL - Requires Lucene dependencies]
        ///// </summary>
        ///// <returns></returns>
        //private string createLuceneIndex()
        //{
        //    string dir = this._filename + ".idx";
        //    if (!System.IO.Directory.Exists(dir))
        //        System.IO.Directory.CreateDirectory(dir);
        //    Lucene.Net.Index.IndexWriter iw = new Lucene.Net.Index.IndexWriter(dir,new Lucene.Net.Analysis.Standard.StandardAnalyzer(),true);

        //    for (uint i = 0; i < this._NumberOfRecords; i++)
        //    {
        //        FeatureDataRow dr = GetFeature(i,this.NewTable);
        //        Lucene.Net.Documents.Document doc = new Lucene.Net.Documents.Document();
        //        // Add the object-id as a field, so that index can be maintained.
        //        // This field is not stored with document, it is indexed, but it is not
        //        // tokenized prior to indexing.
        //        //doc.Add(Lucene.Net.Documents.Field.UnIndexed("SharpMap_oid", i.ToString())); //Add OID index

        //        foreach(System.Data.DataColumn col in dr.Table.Columns) //Add and index values from DBF
        //        {
        //            if(col.DataType.Equals(typeof(string)))
        //                // Add the contents as a valued Text field so it will get tokenized and indexed.
        //                doc.Add(Lucene.Net.Documents.Field.UnStored(col.ColumnName,(string)dr[col]));
        //            else
        //                doc.Add(Lucene.Net.Documents.Field.UnStored(col.ColumnName, dr[col].ToString()));
        //        }
        //        iw.AddDocument(doc);
        //    }
        //    iw.Optimize();
        //    iw.Close();
        //    return this._filename + ".idx";
        //}
        //*/

        #endregion
    }
}
