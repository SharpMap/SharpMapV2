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
using System.Collections.Generic;
using System.Text;
using SharpMap.Geometries;
using System.IO;
using System.Globalization;

namespace SharpMap.Converters.WellKnownText
{
	/// <summary>
	/// Converts a Well-Known Text representation to a <see cref="Geometry"/> instance.
	/// </summary>
	/// <remarks>
	/// <para>
    /// The Well-Known Text (WKT) representation of Geometry is designed to 
    /// exchange geometry data in ASCII form.
    /// </para>
	/// Examples of WKT representations of geometry objects are:
	/// <list type="table">
	/// <listheader><term>Geometry </term>
    /// <description>WKT Representation</description>
    /// </listheader>
	/// <item><term>A Point</term>
	/// <description>POINT(15 20)<br/> Note that point coordinates are specified 
    /// with no separating comma.</description></item>
	/// <item><term>A LineString with four points:</term>
	/// <description>LINESTRING(0 0, 10 10, 20 25, 50 60)</description></item>
	/// <item><term>A Polygon with one exterior ring and one interior ring:</term>
	/// <description>POLYGON((0 0,10 0,10 10,0 10,0 0),(5 5,7 5,7 7,5 7, 5 5))</description></item>
	/// <item><term>A MultiPoint with three Point values:</term>
	/// <description>MULTIPOINT(0 0, 20 20, 60 60)</description></item>
	/// <item><term>A MultiLineString with two LineString values:</term>
	/// <description>MULTILINESTRING((10 10, 20 20), (15 15, 30 15))</description></item>
	/// <item><term>A MultiPolygon with two Polygon values:</term>
	/// <description>MULTIPOLYGON(((0 0,10 0,10 10,0 10,0 0)),((5 5,7 5,7 7,5 7, 5 5)))</description></item>
	/// <item><term>A GeometryCollection consisting of two Point values and one LineString:</term>
	/// <description>GEOMETRYCOLLECTION(POINT(10 10), POINT(30 30), LINESTRING(15 15, 20 20))</description></item>
	/// </list>
	/// </remarks>
	public class GeometryFromWkt
    {
		/// <summary>
		/// Converts a Well-known text representation to a 
        /// <see cref="Geometry"/>.
		/// </summary>
		/// <param name="wellKnownText">
        /// A <see cref="Geometry"/> tagged text string 
        /// (see the OpenGIS Simple Features Specification).
        /// </param>
		/// <returns>
        /// Returns a <see cref="Geometry"/> specified by wellKnownText.  
        /// Throws an exception if there is a parsing problem.
        /// </returns>
		public static Geometry Parse(string wellKnownText)
		{
			// throws a parsing exception is there is a problem.
			StringReader reader = new StringReader(wellKnownText);
			return Parse(reader);
		}

		/// <summary>
		/// Converts a Well-Known Text representation to a 
        /// <see cref="Geometry"/>.
		/// </summary>
		/// <param name="reader">
        /// A reader which will return a Geometry Tagged Text
		/// string (see the OpenGIS Simple Features Specification).</param>
		/// <returns>
        /// Returns a <see cref="Geometry"/> read from StreamReader. 
		/// An exception will be thrown if there is a parsing problem.
        /// </returns>
		public static Geometry Parse(TextReader reader)
		{
			WktStreamTokenizer tokenizer = new WktStreamTokenizer(reader);

			return readGeometryTaggedText(tokenizer);
		}

		/// <summary>
		/// Returns the next array of Coordinates in the stream.
		/// </summary>
		/// <param name="tokenizer">
        /// Tokenizer over a stream of text in Well-Known Text format. 
        /// The next element returned by the stream should be "(" 
        /// (the beginning of "(x1 y1, x2 y2, ..., xn yn)" or "EMPTY".
        /// </param>
		/// <returns>
        /// The next array of Coordinates in the stream, or an empty array of 
        /// "EMPTY" is the next element returned by the stream.
        /// </returns>
		private static List<Point> getCoordinates(WktStreamTokenizer tokenizer)
		{
			List<Point> coordinates = new List<Point>();

			string nextToken = getNextEmptyOrOpener(tokenizer);
            
            if (nextToken == "EMPTY")
            {
                return coordinates;
            }
			
            Point externalCoordinate = new Point();
			Point internalCoordinate = new Point();

			externalCoordinate.X = getNextNumber(tokenizer);
			externalCoordinate.Y = getNextNumber(tokenizer);
			coordinates.Add(externalCoordinate);
			nextToken = getNextCloserOrComma(tokenizer);
			
            while (nextToken==",")
			{
				internalCoordinate = new Point();
				internalCoordinate.X = getNextNumber(tokenizer);
				internalCoordinate.Y = getNextNumber(tokenizer);
				coordinates.Add(internalCoordinate);
				nextToken = getNextCloserOrComma(tokenizer);
			}

			return coordinates;
		}


		/// <summary>
		/// Returns the next number in the stream.
		/// </summary>
		/// <param name="tokenizer">
        /// Tokenizer over a stream of text in well-known text format. 
        /// The next token must be a number.
        /// </param>
		/// <returns>Returns the next number in the stream.</returns>
        /// <exception cref="ParseException">Thrown if the next token is not a number.</exception>
		private static double getNextNumber(WktStreamTokenizer tokenizer)
		{
			tokenizer.NextToken();
			return tokenizer.GetNumericValue();
		}

		/// <summary>
		/// Returns the next "EMPTY" or "(" in the stream as uppercase text.
		/// </summary>
		/// <param name="tokenizer">
        /// Tokenizer over a stream of text in Well-Known Text
		/// format. The next token must be "EMPTY" or "(".
        /// </param>
		/// <returns>
        /// The next "EMPTY" or "(" in the stream as uppercase text.
        /// </returns>
        /// <exception cref="ParseException">
        /// Thrown if the next token is not "EMPTY" or "(".
        /// </exception>
		private static string getNextEmptyOrOpener(WktStreamTokenizer tokenizer)
		{
			tokenizer.NextToken();
			string nextWord = tokenizer.GetStringValue();
            
            if (nextWord == "EMPTY" || nextWord == "(")
            {
                return nextWord;
            }

			throw new ParseException("Expected 'EMPTY' or '(' but encountered '" + nextWord + "'");

		}

		/// <summary>
		/// Returns the next ")" or "," in the stream.
		/// </summary>
		/// <param name="tokenizer">
        /// Tokenizer over a stream of text in Well-Known Text
		/// format. The next token must be ")" or ",".
        /// </param>
		/// <returns>Returns the next ")" or "," in the stream.</returns>
        /// <exception cref="ParseException">
        /// Thrown if the next token is not ")" or ",".
        /// </exception>
		private static string getNextCloserOrComma(WktStreamTokenizer tokenizer)
		{
			tokenizer.NextToken();
			string nextWord = tokenizer.GetStringValue();

			if (nextWord == "," || nextWord == ")")
			{
				return nextWord;
			}

			throw new ParseException("Expected ')' or ',' but encountered '" + nextWord + "'");
		}

		/// <summary>
		/// Returns the next ")" in the stream.
		/// </summary>
		/// <param name="tokenizer">
        /// Tokenizer over a stream of text in Well-Known Text
		/// format. The next token must be ")".
        /// </param>
		/// <returns>
        /// Returns the next ")" in the stream.
        /// </returns>
        /// <exception cref="ParseException">
        /// Thrown if the next token is not ")".
        /// </exception>
		private static string getNextCloser(WktStreamTokenizer tokenizer)
		{

			string nextWord = getNextWord(tokenizer);

            if (nextWord == ")")
            {
                return nextWord;
            }

            throw new ParseException("Expected ')' but encountered '" + nextWord + "'");
		}

		/// <summary>
		/// Returns the next word in the stream as uppercase text.
		/// </summary>
		/// <param name="tokenizer">
        /// Tokenizer over a stream of text in Well-Known Text
		/// format. The next token must be a word.
        /// </param>
		/// <returns>Returns the next word in the stream as uppercase text.</returns>
        /// <exception cref="ParseException">
        /// Thrown if the next token is not a word.
        /// </exception>
		private static string getNextWord(WktStreamTokenizer tokenizer)
		{
			TokenType type = tokenizer.NextToken();
			string token = tokenizer.GetStringValue();
            if (type == TokenType.Number)
            {
                throw new Exception("Expected a number but got " + token);
            }
            else if (type == TokenType.Word)
            {
                return token.ToUpper();
            }
            else if (token == "(")
            {
                return "(";
            }
            else if (token == ")")
            {
                return ")";
            }
            else if (token == ",")
            {
                return ",";
            }

			throw new ParseException("Not a valid symbol in WKT format.");
		}

		/// <summary>
		/// Creates a Geometry using the next token in the stream.
		/// </summary>
		/// <param name="tokenizer">Tokenizer over a stream of text in Well-Known Text
		/// format. The next tokens must form a &lt;Geometry Tagged Text&gt;.</param>
		/// <returns>Returns a Geometry specified by the next token in the stream.</returns>
		/// <remarks>
		/// Exception is thrown if the coordinates used to create a Polygon
		/// shell and holes do not form closed linestrings, or if an unexpected
		/// token is encountered.
        /// </remarks>
        /// <exception cref="ParseException">
        /// Thrown if an unexpected token is encountered.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Thrown if an unsupported geometry is encountered.
        /// </exception>
		private static Geometry readGeometryTaggedText(WktStreamTokenizer tokenizer)
		{
			tokenizer.NextToken();
			string type = tokenizer.GetStringValue().ToUpper();
			Geometry geometry = null;

			switch (type)
			{
				case "POINT":
				    geometry = readPointText(tokenizer);
				    break;
				case "LINESTRING":
				    geometry = readLineStringText(tokenizer);
				    break;
				case "MULTIPOINT":
				    geometry = readMultiPointText(tokenizer);
				    break;
				case "MULTILINESTRING":
				    geometry = readMultiLineStringText(tokenizer);
				    break;
				case "POLYGON":
				    geometry = readPolygonText(tokenizer);
				    break;
				case "MULTIPOLYGON":
				    geometry = readMultiPolygonText(tokenizer);
				    break;
				case "GEOMETRYCOLLECTION":
				    geometry = readGeometryCollectionText(tokenizer);
				    break;
				default:
                    throw new NotSupportedException(String.Format(WktStreamTokenizer.NumberFormat_enUS, 
                        "Geometrytype '{0}' is not supported.", type));
			}

			return geometry;
		}

		/// <summary>
		/// Creates a <see cref="MultiPolygon"/> using the next token in the stream.
		/// </summary>
		/// <param name="tokenizer">tokenizer over a stream of text in Well-Known Text
		/// format. The next tokens must form a MultiPolygon.</param>
		/// <returns>
        /// A <see cref="MultiPolygon"/> specified by the next token in the 
		/// stream.
        /// </returns>
        /// <exception cref="ParseException">
        /// Thrown if an unexpected token is encountered or 
        /// if if the coordinates used to create the <see cref="Polygon"/>
        /// shells and holes do not form closed linestrings.
        /// </exception>
		private static MultiPolygon readMultiPolygonText(WktStreamTokenizer tokenizer)
		{
			MultiPolygon polygons = new MultiPolygon();
			string nextToken = getNextEmptyOrOpener(tokenizer);

            if (nextToken == "EMPTY")
            {
                return polygons;
            }

			Polygon polygon = readPolygonText(tokenizer);
			polygons.Polygons.Add(polygon);
			nextToken = getNextCloserOrComma(tokenizer);
			
            while (nextToken == ",")
			{
				polygon = readPolygonText(tokenizer);
				polygons.Polygons.Add(polygon);
				nextToken = getNextCloserOrComma(tokenizer);
			}

			return polygons;
		}

		/// <summary>
		/// Creates a Polygon using the next token in the stream.
		/// </summary>
		/// <param name="tokenizer">
        /// Tokenizer over a stream of text in Well-Known Text
		/// format. The next tokens must form a &lt;Polygon Text&gt;.</param>
		/// <returns>
        /// Returns a Polygon specified by the next token
		/// in the stream.
        /// </returns>
        /// <exception cref="ParseException">
        /// Thrown if an unexpected token is encountered or 
        /// if if the coordinates used to create the <see cref="Polygon"/>
        /// shells and holes do not form closed linestrings.
        /// </exception>
		private static Polygon readPolygonText(WktStreamTokenizer tokenizer)
		{
			Polygon pol = new Polygon();
			string nextToken = getNextEmptyOrOpener(tokenizer);

            if (nextToken == "EMPTY")
            {
                return pol;
            }
			
            pol.ExteriorRing = new LinearRing(getCoordinates(tokenizer));
			nextToken = getNextCloserOrComma(tokenizer);
			
            while (nextToken == ",")
			{
				//Add holes
				pol.InteriorRings.Add(new LinearRing(getCoordinates(tokenizer)));
				nextToken = getNextCloserOrComma(tokenizer);
			}

			return pol;
		}


		/// <summary>
		/// Creates a Point using the next token in the stream.
		/// </summary>
		/// <param name="tokenizer">
        /// Tokenizer over a stream of text in Well-Known Text
		/// format. The next tokens must form a &lt;Point Text&gt;.
        /// </param>
		/// <returns>
        /// Returns a Point specified by the next token in
		/// the stream.
        /// </returns>
        /// <exception cref="ParseException">
        /// Thrown if an unexpected token is encountered.
        /// </exception>
		private static Point readPointText(WktStreamTokenizer tokenizer)
		{
			Point p = new Point();
			string nextToken = getNextEmptyOrOpener(tokenizer);
            
            if (nextToken == "EMPTY")
            {
                return p;
            }

			p.X = getNextNumber(tokenizer);
			p.Y = getNextNumber(tokenizer);
			getNextCloser(tokenizer);

			return p;
		}

		/// <summary>
		/// Creates a Point using the next token in the stream.
		/// </summary>
		/// <param name="tokenizer">
        /// Tokenizer over a stream of text in Well-Known Text
		/// format. The next tokens must form a &lt;Point Text&gt;.
        /// </param>
		/// <returns>
        /// Returns a Point specified by the next token in
		/// the stream.
        /// </returns>
        /// <exception cref="ParseException">
        /// Thrown if an unexpected token is encountered.
        /// </exception>
		private static MultiPoint readMultiPointText(WktStreamTokenizer tokenizer)
		{
			MultiPoint mp = new MultiPoint();
			string nextToken = getNextEmptyOrOpener(tokenizer);
            
            if (nextToken == "EMPTY")
            {
                return mp;
            }

			mp.Points.Add(new SharpMap.Geometries.Point(getNextNumber(tokenizer),getNextNumber(tokenizer)));
			nextToken = getNextCloserOrComma(tokenizer);

			while (nextToken == ",")
			{
				mp.Points.Add(new SharpMap.Geometries.Point(getNextNumber(tokenizer), getNextNumber(tokenizer)));
				nextToken = getNextCloserOrComma(tokenizer);
			}

			return mp;
		}

		/// <summary>
		/// Creates a <see cref="MultiLineString"/> using the next token in the stream. 
		/// </summary>
		/// <param name="tokenizer">
        /// Tokenizer over a stream of text in Well-Known Text format. 
        /// The next tokens must form a &lt;MultiLineString Text&gt;.
        /// </param>
		/// <returns>
        /// A <see cref="MultiLineString"/> specified by the next token in the stream.
        /// </returns>
		private static MultiLineString readMultiLineStringText(WktStreamTokenizer tokenizer)
		{
			MultiLineString lines = new MultiLineString();
			string nextToken = getNextEmptyOrOpener(tokenizer);
            
            if (nextToken == "EMPTY")
            {
                return lines;
            }

			lines.LineStrings.Add(readLineStringText(tokenizer));
			nextToken = getNextCloserOrComma(tokenizer);
			
            while (nextToken == ",")
			{
				lines.LineStrings.Add(readLineStringText(tokenizer));
				nextToken = getNextCloserOrComma(tokenizer);
			}
			
            return lines;
		}

		/// <summary>
		/// Creates a LineString using the next token in the stream.
		/// </summary>
		/// <param name="tokenizer">
        /// Tokenizer over a stream of text in Well-Known Text format.  
        /// The next tokens must form a &lt;LineString Text&gt;.
        /// </param>
		/// <returns>
        /// Returns a LineString specified by the next token in the stream.
        /// </returns>
        /// <exception cref="ParseException">
        /// Thrown if an unexpected token is encountered.
        /// </exception>
		private static LineString readLineStringText(WktStreamTokenizer tokenizer)
		{
			return new LineString(getCoordinates(tokenizer));
		}

		/// <summary>
		/// Creates a <see cref="GeometryCollection"/> using the next token in the stream.
		/// </summary>
		/// <param name="tokenizer">
        /// Tokenizer over a stream of text in Well-Known Text
		/// format. The next tokens must form a GeometryCollection Text.
        /// </param>
		/// <returns>
		/// A <see cref="GeometryCollection"/> specified by the next token in the stream.
        /// </returns>
        /// <exception cref="ParseException">
        /// Thrown if an unexpected token is encountered.
        /// </exception>
		private static GeometryCollection readGeometryCollectionText(WktStreamTokenizer tokenizer)
		{
			GeometryCollection geometries = new GeometryCollection();			
			string nextToken = getNextEmptyOrOpener(tokenizer);

            if (nextToken.Equals("EMPTY"))
            {
                return geometries;
            }
			
            geometries.Collection.Add(readGeometryTaggedText(tokenizer));
			nextToken = getNextCloserOrComma(tokenizer);
			
            while (nextToken.Equals(","))
			{
				geometries.Collection.Add(readGeometryTaggedText(tokenizer));
				nextToken = getNextCloserOrComma(tokenizer);
			}

			return geometries;
		}        

	}
}
