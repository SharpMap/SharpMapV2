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
using SharpMap.Geometries;
using SharpMap.Utilities;

namespace SharpMap.Converters.WellKnownBinary
{
    /// <summary>
    /// Converts a <see cref="SharpMap.Geometries.Geometry"/> instance 
    /// to a Well-Known Binary string representation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Well-Known Binary Representation for <see cref="SharpMap.Geometries.Geometry"/> 
    /// (WKBGeometry) provides a portable representation of a <see cref="SharpMap.Geometries.Geometry"/> 
    /// value as a contiguous stream of bytes. It permits <see cref="SharpMap.Geometries.Geometry"/> 
    /// values to be exchanged between an ODBC client and an SQL database in binary form.
    /// </para>
    /// <para>
    /// The Well-Known Binary Representation for <see cref="SharpMap.Geometries.Geometry"/> 
    /// is obtained by serializing a <see cref="SharpMap.Geometries.Geometry"/>
    /// instance as a sequence of numeric types drawn from the set {Unsigned Integer, Double} and
    /// then serializing each numeric type as a sequence of bytes using one of two well defined,
    /// standard, binary representations for numeric types (NDR, XDR). The specific binary encoding
    /// (NDR or XDR) used for a geometry byte stream is described by a one byte tag that precedes
    /// the serialized bytes. The only difference between the two encodings of geometry is one of
    /// byte order, the XDR encoding is Big Endian, the NDR encoding is Little Endian.</para>
    /// </remarks> 
    public class GeometryToWkb
    {
        //private const byte WKBByteOrder = 0;

        /// <summary>
        /// Encodes a <see cref="Geometry"/> to Well-Known Binary format
        /// and writes it to a byte array using little endian byte encoding.
        /// </summary>
        /// <param name="g">The geometry to encode as WKB.</param>
        /// <returns>WKB representation of the geometry.</returns>
        public static byte[] Write(Geometry g)
        {
            return Write(g, WkbByteOrder.Ndr);
        }

        /// <summary>
        /// Encodes a <see cref="Geometry"/> to Well-Known Binary format
        /// and writes it to a byte array using the specified encoding.
        /// </summary>
        /// <param name="g">The geometry to encode as WKB.</param>
        /// <param name="wkbByteOrder">Byte order to encode values in.</param>
        /// <returns>WKB representation of the geometry.</returns>
        public static byte[] Write(Geometry g, WkbByteOrder wkbByteOrder)
        {
            if (g == null) throw new ArgumentNullException("g");

            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            //Write the byteOrder format.
            bw.Write((byte)wkbByteOrder);

            //Write the type of this geometry
            writeType(g, bw, wkbByteOrder);

            //Write the geometry
            writeGeometry(g, bw, wkbByteOrder);

            return ms.ToArray();
        }

        #region Private helper methods

        /// <summary>
        /// Writes the type number for this geometry.
        /// </summary>
        /// <param name="geometry">The geometry to determine the type of.</param>
        /// <param name="writer">Binary Writer</param>
        /// <param name="byteOrder">Byte order to encode values in.</param>
        private static void writeType(Geometry geometry, BinaryWriter writer, WkbByteOrder byteOrder)
        {
            //Points are type 1.
            if (geometry is Point)
            {
                writeUInt32((uint)WkbGeometryType.Point, writer, byteOrder);
            }
            //Linestrings are type 2.
            else if (geometry is LineString)
            {
                writeUInt32((uint)WkbGeometryType.LineString, writer, byteOrder);
            }
            //Polygons are type 3.
            else if (geometry is Polygon)
            {
                writeUInt32((uint)WkbGeometryType.Polygon, writer, byteOrder);
            }
            //Mulitpoints are type 4.
            else if (geometry is MultiPoint)
            {
                writeUInt32((uint)WkbGeometryType.MultiPoint, writer, byteOrder);
            }
            //Multilinestrings are type 5.
            else if (geometry is MultiLineString)
            {
                writeUInt32((uint)WkbGeometryType.MultiLineString, writer, byteOrder);
            }
            //Multipolygons are type 6.
            else if (geometry is MultiPolygon)
            {
                writeUInt32((uint)WkbGeometryType.MultiPolygon, writer, byteOrder);
            }
            //Geometrycollections are type 7.
            else if (geometry is GeometryCollection)
            {
                writeUInt32((uint)WkbGeometryType.GeometryCollection, writer, byteOrder);
            }
            //If the type is not of the above 7 throw an exception.
            else
            {
                throw new ArgumentException("Invalid Geometry Type");
            }
        }

        /// <summary>
        /// Writes the geometry to the binary writer.
        /// </summary>
        /// <param name="geometry">The geometry to be written.</param>
        /// <param name="bWriter"></param>
        /// <param name="byteOrder">Byte order to encode values in.</param>
        private static void writeGeometry(Geometry geometry, BinaryWriter bWriter, WkbByteOrder byteOrder)
        {
            //Write the point.
            if (geometry is Point)
            {
                writePoint((Point)geometry, bWriter, byteOrder);
            }
            else if (geometry is LineString)
            {
                LineString ls = (LineString)geometry;
                writeLineString(ls, bWriter, byteOrder);
            }
            else if (geometry is Polygon)
            {
                writePolygon((Polygon)geometry, bWriter, byteOrder);
            }
            //Write the Multipoint.
            else if (geometry is MultiPoint)
            {
                writeMultiPoint((MultiPoint)geometry, bWriter, byteOrder);
            }
            //Write the Multilinestring.
            else if (geometry is MultiLineString)
            {
                writeMultiLineString((MultiLineString)geometry, bWriter, byteOrder);
            }
            //Write the Multipolygon.
            else if (geometry is MultiPolygon)
            {
                writeMultiPolygon((MultiPolygon)geometry, bWriter, byteOrder);
            }
            //Write the Geometrycollection.
            else if (geometry is GeometryCollection)
            {
                writeGeometryCollection((GeometryCollection)geometry, bWriter, byteOrder);
            }
            //If the type is not of the above 7 throw an exception.
            else
            {
                throw new ArgumentException("Invalid Geometry Type");
            }
        }

        /// <summary>
        /// Writes a Point.
        /// </summary>
        /// <param name="point">The point to be written.</param>
        /// <param name="writer">Writer to persist WKB values to.</param>
        /// <param name="byteOrder">Byte order to encode values in.</param>
        private static void writePoint(Point point, BinaryWriter writer, WkbByteOrder byteOrder)
        {
            //Write the x coordinate.
            writeDouble(point.X, writer, byteOrder);
            //Write the y coordinate.
            writeDouble(point.Y, writer, byteOrder);
        }

        /// <summary>
        /// Writes a LineString.
        /// </summary>
        /// <param name="ls">The linestring to be written.</param>
        /// <param name="writer">Writer to persist WKB values to.</param>
        /// <param name="byteOrder">Byte order to encode values in.</param>
        private static void writeLineString(LineString ls, BinaryWriter writer, WkbByteOrder byteOrder)
        {
            //Write the number of points in this linestring.
            writeUInt32((uint)ls.Vertices.Count, writer, byteOrder);

            //Loop on each vertices.
            foreach (Point p in ls.Vertices)
            {
                writePoint(p, writer, byteOrder);
            }
        }


        /// <summary>
        /// Writes a polygon.
        /// </summary>
        /// <param name="poly">The polygon to be written.</param>
        /// <param name="writer">Writer to persist WKB values to.</param>
        /// <param name="byteOrder">Byte order to encode values in.</param>
        private static void writePolygon(Polygon poly, BinaryWriter writer, WkbByteOrder byteOrder)
        {
            //Get the number of rings in this polygon.
            int numRings = poly.InteriorRings.Count + 1;

            //Write the number of rings to the stream (add one for the shell)
            writeUInt32((uint)numRings, writer, byteOrder);

            //Write the exterior of this polygon.
            writeLineString(poly.ExteriorRing, writer, byteOrder);

            //Loop on the number of rings - 1 because we already wrote the shell.
            foreach (LinearRing lr in poly.InteriorRings)
            {
                //Write the (lineString)LinearRing.
                writeLineString(lr, writer, byteOrder);
            }
        }

        /// <summary>
        /// Writes a multipoint.
        /// </summary>
        /// <param name="mp">The multipoint to be written.</param>
        /// <param name="writer">Writer to persist WKB values to.</param>
        /// <param name="byteOrder">Byte order to encode values in.</param>
        private static void writeMultiPoint(MultiPoint mp, BinaryWriter writer, WkbByteOrder byteOrder)
        {
            //Write the number of points.
            writeUInt32((uint)mp.Points.Count, writer, byteOrder);

            //Loop on the number of points.
            foreach (Point p in mp.Points)
            {
                //Write Points Header
                writer.Write((byte)byteOrder);
                writeUInt32((uint)WkbGeometryType.Point, writer, byteOrder);
                //Write each point.
                writePoint(p, writer, byteOrder);
            }
        }

        /// <summary>
        /// Writes a multilinestring.
        /// </summary>
        /// <param name="mls">The multilinestring to be written.</param>
        /// <param name="writer">Writer to persist WKB values to.</param>
        /// <param name="byteOrder">Byte order to encode values in.</param>
        private static void writeMultiLineString(MultiLineString mls, BinaryWriter writer, WkbByteOrder byteOrder)
        {
            //Write the number of linestrings.
            writeUInt32((uint)mls.LineStrings.Count, writer, byteOrder);

            //Loop on the number of linestrings.
            foreach (LineString ls in mls.LineStrings)
            {
                //Write LineString Header
                writer.Write((byte)byteOrder);
                writeUInt32((uint)WkbGeometryType.LineString, writer, byteOrder);
                //Write each linestring.
                writeLineString(ls, writer, byteOrder);
            }
        }

        /// <summary>
        /// Writes a multipolygon.
        /// </summary>
        /// <param name="mp">The mulitpolygon to be written.</param>
        /// <param name="writer">Writer to persist WKB values to.</param>
        /// <param name="byteOrder">Byte order to encode values in.</param>
        private static void writeMultiPolygon(MultiPolygon mp, BinaryWriter writer, WkbByteOrder byteOrder)
        {
            //Write the number of polygons.
            writeUInt32((uint)mp.Polygons.Count, writer, byteOrder);

            //Loop on the number of polygons.
            foreach (Polygon poly in mp.Polygons)
            {
                //Write polygon header
                writer.Write((byte)byteOrder);
                writeUInt32((uint)WkbGeometryType.Polygon, writer, byteOrder);
                //Write each polygon.
                writePolygon(poly, writer, byteOrder);
            }
        }


        /// <summary>
        /// Writes a GeometryCollection instance.
        /// </summary>
        /// <param name="gc">The GeometryCollection to be written.</param>
        /// <param name="writer">Writer to persist WKB values to.</param>
        /// <param name="byteOrder">Byte order to encode values in.</param>
        private static void writeGeometryCollection(GeometryCollection gc, BinaryWriter writer, WkbByteOrder byteOrder)
        {
            //Get the number of geometries in this geometrycollection.
            int numGeometries = gc.NumGeometries;

            //Write the number of geometries.
            writeUInt32((uint)numGeometries, writer, byteOrder);

            //Loop on the number of geometries.
            for (int i = 0; i < numGeometries; i++)
            {
                //Write the byte-order format of the following geometry.
                writer.Write((byte)byteOrder);
                //Write the type of each geometry.
                writeType(gc[i], writer, byteOrder);
                //Write each geometry.
                writeGeometry(gc[i], writer, byteOrder);
            }
        }

        /// <summary>
        /// Writes an unsigned 32-bit integer to the BinaryWriter using the specified byte encoding.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="writer">Writer to persist WKB values to.</param>
        /// <param name="byteOrder">Byte order to encode values in.</param>
        private static void writeUInt32(UInt32 value, BinaryWriter writer, WkbByteOrder byteOrder)
        {
            if (byteOrder == WkbByteOrder.Xdr)
            {
                writer.Write(ByteEncoder.GetBigEndian(value));
            }
            else
            {
                writer.Write(ByteEncoder.GetLittleEndian(value));
            }
        }

        /// <summary>
        /// Writes a double floating point value to the BinaryWriter using the specified byte encoding.
        /// </summary>
        /// <param name="value">Value to write.</param>
        /// <param name="writer">Writer to persist WKB values to.</param>
        /// <param name="byteOrder">Byte order to encode values in.</param>
        private static void writeDouble(double value, BinaryWriter writer, WkbByteOrder byteOrder)
        {
            if (byteOrder == WkbByteOrder.Xdr)
            {
                writer.Write(ByteEncoder.GetBigEndian(value));
            }
            else
            {
                writer.Write(ByteEncoder.GetLittleEndian(value));
            }
        }
        #endregion
    }
}
