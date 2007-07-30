using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.Utilities;
using System.IO;
using SharpMap.Geometries;

namespace SharpMap.Data.Providers
{
	internal class ShapeFileHeader
	{
		private ShapeType _shapeType;
		private BoundingBox _envelope; 

		public ShapeFileHeader(BinaryReader reader)
		{
			parseHeader(reader);
		}

		public ShapeType ShapeType
		{
			get { return _shapeType; }
			private set { _shapeType = value; }
		}

		public BoundingBox Envelope
		{
			get { return _envelope; }
			set { _envelope = value; }
		}

		public void WriteHeader(BinaryWriter writer, int fileWordLength)
		{
			writer.Seek(0, SeekOrigin.Begin);
			writer.Write(ByteEncoder.GetBigEndian(ShapeFileConstants.HeaderStartCode));
			writer.Write(new byte[20]);
			writer.Write(ByteEncoder.GetBigEndian(fileWordLength));
			writer.Write(ByteEncoder.GetLittleEndian(ShapeFileConstants.VersionCode));
			writer.Write(ByteEncoder.GetLittleEndian((int)ShapeType));
			writer.Write(ByteEncoder.GetLittleEndian(Envelope.Left));
			writer.Write(ByteEncoder.GetLittleEndian(Envelope.Bottom));
			writer.Write(ByteEncoder.GetLittleEndian(Envelope.Right));
			writer.Write(ByteEncoder.GetLittleEndian(Envelope.Top));
			writer.Write(new byte[32]); // Z-values and M-values
		}

		#region File parsing helpers
		/// <summary>
		/// Reads and parses the header of the .shp index file
		/// </summary>
		/// <remarks>
		/// From ESRI ShapeFile Technical Description document
		/// 
		/// http://www.esri.com/library/whitepapers/pdfs/shapefile.pdf
		/// 
		/// Byte
		/// Position    Field           Value       Type    Order
		/// -----------------------------------------------------
		/// Byte 0      File Code       9994        Integer Big
		/// Byte 4      Unused          0           Integer Big
		/// Byte 8      Unused          0           Integer Big
		/// Byte 12     Unused          0           Integer Big
		/// Byte 16     Unused          0           Integer Big
		/// Byte 20     Unused          0           Integer Big
		/// Byte 24     File Length     File Length Integer Big
		/// Byte 28     Version         1000        Integer Little
		/// Byte 32     Shape Type      Shape Type  Integer Little
		/// Byte 36     Bounding Box    Xmin        Double  Little
		/// Byte 44     Bounding Box    Ymin        Double  Little
		/// Byte 52     Bounding Box    Xmax        Double  Little
		/// Byte 60     Bounding Box    Ymax        Double  Little
		/// Byte 68*    Bounding Box    Zmin        Double  Little
		/// Byte 76*    Bounding Box    Zmax        Double  Little
		/// Byte 84*    Bounding Box    Mmin        Double  Little
		/// Byte 92*    Bounding Box    Mmax        Double  Little
		/// 
		/// * Unused, with value 0.0, if not Measured or Z type
		/// 
		/// The "Integer" type corresponds to the CLS Int32 type, and "Double" to CLS Double (IEEE 754).
		/// </remarks>
		private void parseHeader(BinaryReader reader)
		{
			reader.BaseStream.Seek(0, SeekOrigin.Begin);

			//Check file header
			if (ByteEncoder.GetBigEndian(reader.ReadInt32()) != ShapeFileConstants.HeaderStartCode)
			{
				throw new ShapeFileIsInvalidException("Invalid ShapeFile (.shp)");
			}

			reader.BaseStream.Seek(24, 0); //seek to File Length
			int fileLength = ByteEncoder.GetBigEndian(reader.ReadInt32()); //Read filelength as big-endian. The length is number of 16-bit words in file

			reader.BaseStream.Seek(32, 0); //seek to ShapeType
			ShapeType = (ShapeType)reader.ReadInt32();

			//Read the spatial bounding box of the contents
			reader.BaseStream.Seek(36, 0); //seek to box
			Envelope = new BoundingBox(
				ByteEncoder.GetLittleEndian(reader.ReadDouble()),
				ByteEncoder.GetLittleEndian(reader.ReadDouble()),
				ByteEncoder.GetLittleEndian(reader.ReadDouble()),
				ByteEncoder.GetLittleEndian(reader.ReadDouble()));
		}
		#endregion
		
		private int computeMainFileLengthInWords(ShapeFileIndex index)
		{
			int length = ShapeFileConstants.HeaderSizeBytes / 2;

			foreach (KeyValuePair<uint, ShapeFileIndex.IndexEntry> kvp in index)
			{
				length += kvp.Value.Length + ShapeFileConstants.ShapeRecordHeaderByteLength / 2;
			}

			return length;
		}

		private int computeIndexFileLengthInWords(ShapeFileIndex index)
		{
			int length = ShapeFileConstants.HeaderSizeBytes / 2;

			length += index.Count * ShapeFileConstants.IndexRecordByteLength / 2;

			return length;
		}
	}
}
