// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
//
// Added the ability to load index less shapefiles, buy using the record header within the data file
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
using System.IO;
using GeoAPI.DataStructures;
using SharpMap.Utilities;

namespace SharpMap.Data.Providers.ShapeFile
{
    internal sealed class ShapeFileIndex : IDictionary<UInt32, ShapeFileIndex.IndexEntry>
    {
        #region Fields
        private ShapeFileProvider _shapeFile;
        private readonly SortedList<UInt32, IndexEntry> _shapeIndex = new SortedList<UInt32, IndexEntry>();
        private FileInfo _indexFile;
        private ShapeFileHeader _header;
        private Boolean _isOpen;
        private Boolean _exclusiveMode;
        #endregion

        #region Object Construction / Disposal
        public ShapeFileIndex(ShapeFileProvider shapeFile)
            : this(shapeFile, new FileInfo(shapeFile.IndexFilename))
        {
        }

        private ShapeFileIndex(ShapeFileProvider shapeFile, FileInfo file)
        {
            if (shapeFile == null) throw new ArgumentNullException("shapeFile");
            if (file == null) throw new ArgumentNullException("file");

            _shapeFile = shapeFile;

            if (String.Compare(file.Extension, ".shx", StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                throw new ShapeFileIsInvalidException("Shapefile index must end in '.shx'.");
            }

            _indexFile = file;

            if (File.Exists(_indexFile.FullName))
            {
                using (FileStream indexStream = _indexFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                using (BinaryReader reader = new BinaryReader(indexStream))
                {
                    _header = new ShapeFileHeader(reader, _shapeFile.GeometryFactory);

                    indexStream.Seek(ShapeFileConstants.HeaderSizeBytes, SeekOrigin.Begin);

                    Int32 featureCount = (Int32)((_indexFile.Length - ShapeFileConstants.HeaderSizeBytes)
                        / ShapeFileConstants.IndexRecordByteLength);

                    for (Int32 id = 0; id < featureCount; id++)
                    {
                        Int32 offset = ByteEncoder.GetBigEndian(reader.ReadInt32());
                        Int32 length = ByteEncoder.GetBigEndian(reader.ReadInt32());

                        IndexEntry entry = new IndexEntry(length, offset);
                        // Record numbers begin at 1. (Shapefile: p. 5)
                        _shapeIndex.Add((UInt32)id + 1, entry);
                    }
                }
            }
            else
            {
                // We need to create the index data from the raw data file
                FileStream dataStream = new FileStream(Path.ChangeExtension(_indexFile.FullName, ".shp"), FileMode.Open,
                                          FileAccess.Read);
                BinaryReader reader = new BinaryReader(dataStream);
                {
                    _header = new ShapeFileHeader(reader, _shapeFile.GeometryFactory);

                    reader.BaseStream.Seek(ShapeFileConstants.HeaderSizeBytes, SeekOrigin.Begin);
                    long offset = ShapeFileConstants.HeaderSizeBytes;
                    long fileSize = _header.FileLengthInWords * 2;
                    Int32 id = 0;

                    while (offset < fileSize)
                    {
                        reader.BaseStream.Seek(offset, 0); //Skip content length
                        uint oid_id =(uint) ByteEncoder.GetBigEndian(reader.ReadInt32());
                        int data_length = 2 * ByteEncoder.GetBigEndian(reader.ReadInt32());

                        if (_shapeIndex.ContainsKey(oid_id) == false)
                        {
                            IndexEntry entry = new IndexEntry(data_length / 2, (int)(offset / 2));
                            // Record numbers begin at 1. (Shapefile: p. 5)
                            _shapeIndex.Add((UInt32)oid_id, entry);

                            offset += data_length; // Add Record data length
                            offset += 8; //  Plus add the record header size
                            ++id;
                        }
                        else
                        {
                            offset = fileSize;
                        }
                    }

                    --id;
                    // Correct the header size
                    _header.FileLengthInWords = (id * 4) + ShapeFileConstants.HeaderSizeBytes;
 
                }
                reader.Close();
                dataStream.Close();
            }
        }

        #endregion

        public ShapeFileProvider ShapeFile
        {
            get { return _shapeFile; }
        }

        public void AddFeatureToIndex(FeatureDataRow<UInt32> feature)
        {
            UInt32 id = feature.Id;

            if (ContainsKey(id))
            {
                throw new ShapeFileInvalidOperationException(
                    "Cannot add a feature with the same id to the index more than once.");
            }

            _header.Extents = _shapeFile.GeometryFactory.CreateExtents(
                _header.Extents, feature.Geometry.Extents);

            Int32 length = ShapeFileProvider.ComputeGeometryLengthInWords(feature.Geometry, ShapeFile.ShapeType);
            Int32 offset = ComputeShapeFileSizeInWords();

            IndexEntry entry = new IndexEntry(length, offset);
            _shapeIndex[id] = entry;
        }

        public Int32 ComputeShapeFileSizeInWords()
        {
            if (_shapeIndex.Count == 0)
            {
                return ShapeFileConstants.HeaderSizeBytes / 2;
            }

            IndexEntry lastEntry = _shapeIndex.Values[_shapeIndex.Count - 1];
            return lastEntry.Offset + lastEntry.Length + ShapeFileConstants.ShapeRecordHeaderByteLength / 2;
        }

        public UInt32 GetNextId()
        {
            return (UInt32)_shapeIndex.Count + 1; //jd: shapefiles have 1 based index
        }

        public void Save()
        {
            using (FileStream indexStream = _indexFile.Open(FileMode.Create, FileAccess.Write, FileShare.None))
            using (BinaryWriter indexWriter = new BinaryWriter(indexStream))
            {
                _header.Extents = ShapeFile.GetExtents();
                _header.FileLengthInWords = computeIndexLengthInWords();
                _header.WriteHeader(indexWriter);

                foreach (IndexEntry entry in _shapeIndex.Values)
                {
                    indexWriter.Write(ByteEncoder.GetBigEndian(entry.Offset));
                    indexWriter.Write(ByteEncoder.GetBigEndian(entry.Length));
                }

                indexWriter.Flush();
            }
        }

#if DEBUG
        /// <summary>
        /// jd: primarily a debugging utility method to aid manually reading through broken shapefiles by offset :(
        /// </summary>
        /// <returns></returns>
        public string SaveToString()
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                SaveToWriter(writer);
                return sb.ToString();
            }
        }

        public void SaveToWriter(TextWriter writer)
        {
            writer.WriteLine("Shapefile Index:");
            writer.WriteLine("Offset\tLength\tAbsoluteOffset\tByteLength");
            foreach (IndexEntry entry in _shapeIndex.Values)
                writer.WriteLine("{0}\t{1}\t{2}\t{3}", entry.Offset, entry.Length, entry.AbsoluteByteOffset, entry.ByteLength);

        }
#endif

        #region IDictionary<UInt32,IndexEntry> Members

        void IDictionary<UInt32, IndexEntry>.Add(UInt32 key, IndexEntry value)
        {
            _shapeIndex.Add(key, value);
        }

        public Boolean ContainsKey(UInt32 key)
        {
            return _shapeIndex.ContainsKey(key);
        }

        ICollection<UInt32> IDictionary<UInt32, IndexEntry>.Keys
        {
            get { return _shapeIndex.Keys; }
        }

        Boolean IDictionary<UInt32, IndexEntry>.Remove(UInt32 key)
        {
            return _shapeIndex.Remove(key);
        }

        public Boolean TryGetValue(UInt32 key, out IndexEntry value)
        {
            return _shapeIndex.TryGetValue(key, out value);
        }

        ICollection<IndexEntry> IDictionary<UInt32, IndexEntry>.Values
        {
            get { return _shapeIndex.Values; }
        }

        public IndexEntry this[UInt32 key]
        {
            get
            {
                return _shapeIndex[key];
            }
            set
            {
                _shapeIndex[key] = value;
            }
        }

        #endregion

        #region ICollection<KeyValuePair<UInt32,IndexEntry>> Members

        void ICollection<KeyValuePair<UInt32, IndexEntry>>.Add(KeyValuePair<UInt32, IndexEntry> item)
        {
            _shapeIndex.Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<UInt32, IndexEntry>>.Clear()
        {
            _shapeIndex.Clear();
        }

        Boolean ICollection<KeyValuePair<UInt32, IndexEntry>>.Contains(KeyValuePair<UInt32, IndexEntry> item)
        {
            return (_shapeIndex as ICollection<KeyValuePair<UInt32, IndexEntry>>).Contains(item);
        }

        void ICollection<KeyValuePair<UInt32, IndexEntry>>.CopyTo(KeyValuePair<UInt32, IndexEntry>[] array, Int32 arrayIndex)
        {
            (_shapeIndex as ICollection<KeyValuePair<UInt32, IndexEntry>>).CopyTo(array, arrayIndex);
        }

        public Int32 Count
        {
            get { return _shapeIndex.Count; }
        }

        public Boolean IsReadOnly
        {
            get { return false; }
        }

        Boolean ICollection<KeyValuePair<UInt32, IndexEntry>>.Remove(KeyValuePair<UInt32, IndexEntry> item)
        {
            return (_shapeIndex as ICollection<KeyValuePair<UInt32, IndexEntry>>).Remove(item);
        }

        #endregion

        #region IEnumerable<KeyValuePair<UInt32,IndexEntry>> Members

        public IEnumerator<KeyValuePair<UInt32, IndexEntry>> GetEnumerator()
        {
            foreach (KeyValuePair<UInt32, IndexEntry> entry in _shapeIndex)
            {
                yield return entry;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Private helper methods
        private Int32 computeIndexLengthInWords()
        {
            return ((_shapeIndex.Count * ShapeFileConstants.IndexRecordByteLength)
                + ShapeFileConstants.HeaderSizeBytes) / 2;
        }

        #region File parsing helpers
        /// <summary>
        /// Parses the .shx shapefile index file
        /// </summary>
        /// <remarks>
        /// The index file is organized to give a matching offset and content length for each entry in the .shp file.
        /// 
        /// From ESRI ShapeFile Technical Description document
        /// 
        /// http://www.esri.com/library/whitepapers/pdfs/shapefile.pdf
        /// 
        /// Byte
        /// Position    Field           Value           Type    Order
        /// ---------------------------------------------------------
        /// Byte 0      Offset          Offset          Integer Big
        /// Byte 4      Content Length  Content Length  Integer Big
        /// 
        /// The Integer type corresponds to the CLS Int32 type.
        /// </remarks>
        private void parseIndex()
        {
            using (FileStream indexStream = _indexFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader indexReader = new BinaryReader(indexStream, Encoding.Unicode))
            {
                indexStream.Seek(ShapeFileConstants.HeaderSizeBytes, SeekOrigin.Begin);
                UInt32 recordNumber = 0;
                while (indexStream.Position < indexStream.Length)
                {
                    Int32 offset = ByteEncoder.GetBigEndian(indexReader.ReadInt32());
                    Int32 length = ByteEncoder.GetBigEndian(indexReader.ReadInt32());
                    IndexEntry entry = new IndexEntry(length, offset);
                    _shapeIndex[recordNumber++] = entry;
                }
            }
        }
        #endregion
        #endregion

        #region IndexEntry struct
        /// <summary>
        /// Entry for each feature to determine the position 
        /// and length of the geometry in the .shp file.
        /// </summary>
        public struct IndexEntry
        {
            private readonly Int32 _offset;
            private readonly Int32 _length;

            public IndexEntry(Int32 length, Int32 offset)
            {
                _length = length;
                _offset = offset;
            }

            /// <summary>
            /// Number of 16-bit words taken up by the record.
            /// </summary>
            public Int32 Length
            {
                get { return _length; }
            }

            /// <summary>
            /// Offset of the record in 16-bit words from the 
            /// beginning of the shapefile.
            /// </summary>
            public Int32 Offset
            {
                get { return _offset; }
            }

            /// <summary>
            /// Number of bytes in the shapefile record.
            /// </summary>
            public Int32 ByteLength
            {
                get { return _length * 2; }
            }

            /// <summary>
            /// Record offest in bytes from the beginning of the shapefile.
            /// </summary>
            public Int32 AbsoluteByteOffset
            {
                get { return _offset * 2; }
            }

            public override String ToString()
            {
                return String.Format("[IndexEntry] Offset: {0}; Length: {1}; Stream Position: {2}",
                    Offset, Length, AbsoluteByteOffset);
            }
        }
        #endregion
    }
}
