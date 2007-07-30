using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SharpMap.Geometries;
using SharpMap.Utilities;

namespace SharpMap.Data.Providers
{
	internal sealed class ShapeFileIndex : IDictionary<uint, ShapeFileIndex.IndexEntry>
	{
		#region Fields
		private ShapeFile _shapeFile;
		private readonly SortedList<uint, IndexEntry> _shapeIndex = new SortedList<uint, IndexEntry>();
		private FileInfo _indexFile;
		private ShapeFileHeader _header;
		private bool _isOpen;
		private bool _exclusiveMode;
		#endregion

		#region Object Construction / Disposal
		public ShapeFileIndex(ShapeFile shapeFile)
			: this(new FileInfo(shapeFile.IndexFilename)) 
		{
			_shapeFile = shapeFile;
		}

		private ShapeFileIndex(FileInfo file)
		{
			if (file == null) throw new ArgumentNullException("file");

			if (String.Compare(file.Extension, ".shx", StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				throw new ShapeFileIsInvalidException("Shapefile index must end in '.shx'.");
			}

			if (!file.Exists)
			{
				throw new ShapeFileIsInvalidException("Shapefile must have a .shx index file");
			}

			_indexFile = file;

			using (BinaryReader reader = new BinaryReader(_indexFile.OpenRead()))
			{
				_header = new ShapeFileHeader(reader);
			}
		}

		#endregion

		public ShapeFile ShapeFile
		{
			get { return _shapeFile; }
		}

		public void Save()
		{
			using (BinaryWriter writer = new BinaryWriter(_indexFile.Open(FileMode.Truncate, FileAccess.Write, FileShare.None)))
			{
				_header.WriteHeader(writer, computeIndexLengthInWords());

				foreach (IndexEntry entry in _shapeIndex.Values)
				{
					writer.Write(ByteEncoder.GetBigEndian(entry.Offset));
					writer.Write(ByteEncoder.GetBigEndian(entry.Length));
				}
			}
		}

		public void AddFeatureToIndex(FeatureDataRow<uint> feature)
		{
			uint id = feature.Id;

			if (ContainsKey(id))
			{
				throw new ShapeFileInvalidOperationException("Cannot add a feature with the same id to the index more than once.");
			}

			_header.Envelope = BoundingBox.Join(_header.Envelope, feature.Geometry.GetBoundingBox());

			int length = ShapeFile.ComputeGeometryLengthInWords(feature.Geometry);
			int offset = ComputeShapeFileSizeInWords();

			IndexEntry entry = new IndexEntry(length, offset);
			_shapeIndex[id] = entry;
		}

		#region IDictionary<uint,IndexEntry> Members

		void IDictionary<uint, IndexEntry>.Add(uint key, ShapeFileIndex.IndexEntry value)
		{
			_shapeIndex.Add(key, value);
		}

		public bool ContainsKey(uint key)
		{
			return _shapeIndex.ContainsKey(key);
		}

		ICollection<uint> IDictionary<uint, IndexEntry>.Keys
		{
			get { return _shapeIndex.Keys; }
		}

		bool IDictionary<uint, IndexEntry>.Remove(uint key)
		{
			return _shapeIndex.Remove(key);
		}

		public bool TryGetValue(uint key, out ShapeFileIndex.IndexEntry value)
		{
			return _shapeIndex.TryGetValue(key, out value);
		}

		ICollection<ShapeFileIndex.IndexEntry> IDictionary<uint, IndexEntry>.Values
		{
			get { return _shapeIndex.Values; }
		}

		public ShapeFileIndex.IndexEntry this[uint key]
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

		#region ICollection<KeyValuePair<uint,IndexEntry>> Members

		void ICollection<KeyValuePair<uint, IndexEntry>>.Add(KeyValuePair<uint, IndexEntry> item)
		{
			_shapeIndex.Add(item.Key, item.Value);
		}

		void ICollection<KeyValuePair<uint, IndexEntry>>.Clear()
		{
			_shapeIndex.Clear();
		}

		bool ICollection<KeyValuePair<uint, IndexEntry>>.Contains(KeyValuePair<uint, IndexEntry> item)
		{
			return (_shapeIndex as ICollection<KeyValuePair<uint, IndexEntry>>).Contains(item);
		}

		void ICollection<KeyValuePair<uint, IndexEntry>>.CopyTo(KeyValuePair<uint, IndexEntry>[] array, int arrayIndex)
		{
			(_shapeIndex as ICollection<KeyValuePair<uint, IndexEntry>>).CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return _shapeIndex.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		bool ICollection<KeyValuePair<uint, IndexEntry>>.Remove(KeyValuePair<uint, IndexEntry> item)
		{
			return (_shapeIndex as ICollection<KeyValuePair<uint, IndexEntry>>).Remove(item);
		}

		#endregion

		#region IEnumerable<KeyValuePair<uint,IndexEntry>> Members

		public IEnumerator<KeyValuePair<uint, IndexEntry>> GetEnumerator()
		{
			foreach (KeyValuePair<uint, IndexEntry> entry in _shapeIndex)
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

		internal uint GetNextId()
		{
			return (uint)_shapeIndex.Count;
		}

		internal int ComputeShapeFileSizeInWords()
		{
			IndexEntry lastEntry = _shapeIndex.Values[_shapeIndex.Count - 1];
			return lastEntry.Offset + lastEntry.Length;
		}

		private int computeIndexLengthInWords()
		{
			return ((_shapeIndex.Count * ShapeFileConstants.IndexRecordByteLength) 
				+ ShapeFileConstants.HeaderSizeBytes) / 2;
		}

		private void writeIndex()
		{
			using (FileStream indexStream = _indexFile.Open(FileMode.Create, FileAccess.Write, FileShare.None))
			using (BinaryWriter indexWriter = new BinaryWriter(indexStream))
			{
				_header.WriteHeader(indexWriter, computeIndexLengthInWords());

				foreach (KeyValuePair<uint, IndexEntry> entry in _shapeIndex)
				{
					long offset = (entry.Key * ShapeFileConstants.IndexRecordByteLength) + ShapeFileConstants.HeaderSizeBytes;
					indexWriter.Seek((int)offset, SeekOrigin.Begin);
					indexWriter.Write(ByteEncoder.GetBigEndian(entry.Value.Offset));
					indexWriter.Write(ByteEncoder.GetBigEndian(entry.Value.Length));
				}

				indexWriter.Flush();
			}
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
				uint recordNumber = 0;
				while (indexStream.Position < indexStream.Length)
				{
					int offset = ByteEncoder.GetBigEndian(indexReader.ReadInt32());
					int length = ByteEncoder.GetBigEndian(indexReader.ReadInt32());
					IndexEntry entry = new IndexEntry(length, offset);
					_shapeIndex[recordNumber++] = entry;
				}
			}
		}
		#endregion

		#region IndexEntry struct
		/// <summary>
		/// Entry for each feature to determine the position 
		/// and length of the geometry in the .shp file.
		/// </summary>
		public struct IndexEntry
		{
			private readonly int _offset;
			private readonly int _length;

			public IndexEntry(int length, int offset)
			{
				_length = length;
				_offset = offset;
			}

			/// <summary>
			/// Number of 16-bit words taken up by the record.
			/// </summary>
			public int Length
			{
				get { return _length; }
			}

			/// <summary>
			/// Offset of the record in 16-bit words from the 
			/// beginning of the shapefile.
			/// </summary>
			public int Offset
			{
				get { return _offset; }
			}

			/// <summary>
			/// Number of bytes in the shapefile record.
			/// </summary>
			public int ByteLength
			{
				get { return _length * 2; }
			}

			/// <summary>
			/// Record offest in bytes from the beginning of the shapefile.
			/// </summary>
			public int AbsoluteByteOffset
			{
				get { return _offset * 2; }
			}

			public override string ToString()
			{
				return String.Format("[IndexEntry] Offset: {0}; Length: {1}; Stream Position: {2}", 
					Offset, Length, AbsoluteByteOffset);
			}
		}
		#endregion
	}
}
