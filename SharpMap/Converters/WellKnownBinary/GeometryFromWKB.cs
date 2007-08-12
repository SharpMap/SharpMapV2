// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
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

// SOURCECODE IS MODIFIED FROM ANOTHER WORK AND IS ORIGINALLY BASED ON GeoTools.NET:
/*
 *  Copyright (C) 2002 Urban Science Applications, Inc. 
 *
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 *
 */

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using SharpMap.Geometries;

namespace SharpMap.Converters.WellKnownBinary
{
	/// <summary>
	/// Converts Well-Known Binary representations to a 
	/// <see cref="Geometry"/> instance.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The Well-Known Binary Representation for <see cref="Geometry"/> 
	/// (WkbGeometry) provides a portable 
	/// representation of a <see cref="Geometry"/> value as a contiguous stream of bytes. 
	/// It permits <see cref="Geometry"/> 
	/// values to be exchanged between an ODBC client and an SQL database in binary form.
	/// </para>
	/// <para>
	/// The Well-Known Binary Representation for <see cref="Geometry"/> 
	/// is obtained by serializing a <see cref="Geometry"/>
	/// instance as a sequence of numeric types drawn from the set {Unsigned Integer, Double} and
	/// then serializing each numeric type as a sequence of bytes using one of two well defined,
	/// standard, binary representations for numeric types (NDR, XDR). The specific binary encoding
	/// (NDR or XDR) used for a geometry byte stream is described by a one byte tag that precedes
	/// the serialized bytes. The only difference between the two encodings of geometry is one of
	/// byte order, the XDR encoding is Big Endian, the NDR encoding is Little Endian.
	/// </para>
	/// </remarks> 
	public class GeometryFromWkb
	{

		/// <summary>
		/// Creates a <see cref="Geometry"/> from the supplied byte[] 
		/// containing the Well-Known Binary representation.
		/// </summary>
		/// <param name="bytes">
		/// A byte[] containing the Well-Known Binary representation.
		/// </param>
		/// <returns>
		/// A <see cref="Geometry"/> created from on supplied 
		/// Well-Known Binary representation.
		/// </returns>
		public static Geometry Parse(byte[] bytes)
		{
			// Create a memory stream using the suppiled byte array.
			using (MemoryStream ms = new MemoryStream(bytes))
			{
				// Create a new binary reader using the newly created memorystream.
				using (BinaryReader reader = new BinaryReader(ms))
				{
					// Call the main create function.
					return Parse(reader);
				}
			}
		}

		/// <summary>
		/// Creates a <see cref="Geometry"/> created from the 
		/// Well-known binary representation.
		/// </summary>
		/// <param name="reader">
		/// A <see cref="System.IO.BinaryReader"/> used to read the 
		/// Well-Known binary representation.
		/// </param>
		/// <returns>
		/// A <see cref="Geometry"/> created from the Well-Known 
		/// Binary representation.
		/// </returns>
		public static Geometry Parse(BinaryReader reader)
		{
			// Get the first byte in the array.  This specifies if the WKB is in
			// XDR (big-endian) format of NDR (little-endian) format.
			byte byteOrder = reader.ReadByte();

			if (!Enum.IsDefined(typeof(WkbByteOrder), byteOrder))
			{
				throw new ArgumentException("Byte order not recognized");
			}

			// Get the type of this geometry.
			uint type = (uint)readUInt32(reader, (WkbByteOrder)byteOrder);

			if (!Enum.IsDefined(typeof(WkbGeometryType), type))
				throw new ArgumentException("Geometry type not recognized");

			switch((WkbGeometryType)type)
			{
				case WkbGeometryType.Point:
					return createWkbPoint(reader, (WkbByteOrder)byteOrder);

				case WkbGeometryType.LineString:
				    return createWkbLineString(reader, (WkbByteOrder)byteOrder);

				case WkbGeometryType.Polygon:
				    return createWkbPolygon(reader, (WkbByteOrder)byteOrder);

				case WkbGeometryType.MultiPoint:
					return createWkbMultiPoint(reader, (WkbByteOrder)byteOrder);

				case WkbGeometryType.MultiLineString:
				    return createWkbMultiLineString(reader, (WkbByteOrder)byteOrder);

				case WkbGeometryType.MultiPolygon:
				    return createWkbMultiPolygon(reader, (WkbByteOrder)byteOrder);

				case WkbGeometryType.GeometryCollection:
				    return createWkbGeometryCollection(reader, (WkbByteOrder)byteOrder);
					
				default:
					throw new NotSupportedException("Geometry type '" + type.ToString() + "' not supported");
			}
		}

		private static Point createWkbPoint(BinaryReader reader, WkbByteOrder byteOrder)
		{
			// Create and return the point.
			return new Point(readDouble(reader, byteOrder), readDouble(reader, byteOrder));
		}

		private static Point[] readCoordinates(BinaryReader reader, WkbByteOrder byteOrder)
		{
			// Get the number of points in this linestring.
			int numPoints = (int)readUInt32(reader, byteOrder);

			// Create a new array of coordinates.
			Point[] coords = new Point[numPoints];

			// Loop on the number of points in the ring.
			for (int i = 0; i < numPoints; i++)
			{
				// Add the coordinate.
				coords[i] = new Point(readDouble(reader, byteOrder), readDouble(reader, byteOrder));
			}
			return coords;
		}

		private static LineString createWkbLineString(BinaryReader reader, WkbByteOrder byteOrder)
		{
		    LineString l = new LineString();
		    l.Vertices.AddRange(readCoordinates(reader, byteOrder));
		    return l;
		}

		private static LinearRing createWkbLinearRing(BinaryReader reader, WkbByteOrder byteOrder)
		{
			LinearRing l = new LinearRing();
			l.Vertices.AddRange(readCoordinates(reader, byteOrder));
			
			// If polygon isn't closed, add the first point to the end (this shouldn't occur for correct WKB data)
			if (l.Vertices[0].X != l.Vertices[l.Vertices.Count - 1].X || l.Vertices[0].Y != l.Vertices[l.Vertices.Count - 1].Y)
			{
				l.Vertices.Add(new Point(l.Vertices[0].X, l.Vertices[0].Y));
			}

			return l;
		}

		private static Polygon createWkbPolygon(BinaryReader reader, WkbByteOrder byteOrder)
		{
			// Get the Number of rings in this Polygon.
			int numRings = (int)readUInt32(reader, byteOrder);

			Debug.Assert(numRings >= 1, "Number of rings in polygon must be 1 or more.");

			Polygon shell = new Polygon(createWkbLinearRing(reader, byteOrder));

			// Create a new array of linearrings for the interior rings.
			for (int i = 0; i < (numRings - 1); i++)
			{
				shell.InteriorRings.Add(createWkbLinearRing(reader, byteOrder));
			}

			// Create and return the Poylgon.
			return shell;
		}

		private static MultiPoint createWkbMultiPoint(BinaryReader reader, WkbByteOrder byteOrder)
		{
			// Get the number of points in this multipoint.
			int numPoints = (int)readUInt32(reader, byteOrder);
	
			// Create a new array for the points.
			MultiPoint points = new MultiPoint();

			// Loop on the number of points.
			for (int i = 0; i < numPoints; i++)
			{
				// Read point header
				reader.ReadByte();
				readUInt32(reader, byteOrder);

				// TODO: Validate type

				// Create the next point and add it to the point array.
				points.Points.Add(createWkbPoint(reader, byteOrder));
			}
			return points;
		}

		private static MultiLineString createWkbMultiLineString(BinaryReader reader, WkbByteOrder byteOrder)
		{
			// Get the number of linestrings in this multilinestring.
			int numLineStrings = (int)readUInt32(reader, byteOrder);

			// Create a new array for the linestrings .
			MultiLineString mline = new MultiLineString();

			// Loop on the number of linestrings.
			for (int i = 0; i < numLineStrings; i++)
			{
				// Read linestring header
				reader.ReadByte();
				readUInt32(reader, byteOrder);

				// Create the next linestring and add it to the array.
				mline.LineStrings.Add(createWkbLineString(reader, byteOrder));
			}

			// Create and return the MultiLineString.
			return mline;
		}

		private static MultiPolygon createWkbMultiPolygon(BinaryReader reader, WkbByteOrder byteOrder)
		{
			// Get the number of Polygons.
			int numPolygons = (int)readUInt32(reader, byteOrder);

			// Create a new array for the Polygons.
			MultiPolygon polygons = new MultiPolygon();

			// Loop on the number of polygons.
			for (int i = 0; i < numPolygons; i++)
			{
				// read polygon header
				reader.ReadByte();
				readUInt32(reader, byteOrder);

				// TODO: Validate type

				// Create the next polygon and add it to the array.
				polygons.Polygons.Add(createWkbPolygon(reader, byteOrder));
			}

			//Create and return the MultiPolygon.
			return polygons;
		}

		private static Geometry createWkbGeometryCollection(BinaryReader reader, WkbByteOrder byteOrder)
		{
			// The next byte in the array tells the number of geometries in this collection.
			int numGeometries = (int)readUInt32(reader, byteOrder);

			// Create a new array for the geometries.
			GeometryCollection geometries = new GeometryCollection();

			// Loop on the number of geometries.
			for (int i = 0; i < numGeometries; i++)
			{
				// Call the main create function with the next geometry.
				geometries.Collection.Add(Parse(reader));
			}

			// Create and return the next geometry.
			return geometries;
		}

		//NOT USED
		//private static int ReadInt32(BinaryReader reader, WKBByteOrder byteOrder)
		//{
		//    if (byteOrder == WKBByteOrder.Xdr)
		//    {
		//        byte[] bytes = BitConverter.GetBytes(reader.ReadInt32()); 
		//        Array.Reverse(bytes);
		//        return BitConverter.ToInt32(bytes, 0);
		//    }
		//    else
		//        return reader.ReadInt32();
		//}

		private static uint readUInt32(BinaryReader reader, WkbByteOrder byteOrder)
		{
			if (byteOrder == WkbByteOrder.Xdr)
			{
				byte[] bytes = BitConverter.GetBytes(reader.ReadUInt32());
				Array.Reverse(bytes);
				return BitConverter.ToUInt32(bytes, 0);
			}
			else
			{
				return reader.ReadUInt32();
			}
		}

		private static double readDouble(BinaryReader reader, WkbByteOrder byteOrder)
		{
			if (byteOrder == WkbByteOrder.Xdr)
			{
				byte[] bytes = BitConverter.GetBytes(reader.ReadDouble());
				Array.Reverse(bytes);
				return BitConverter.ToDouble(bytes, 0);
			}
			else
			{
				return reader.ReadDouble();
			}
		}
	}
}
