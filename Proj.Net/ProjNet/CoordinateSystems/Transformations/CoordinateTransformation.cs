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

using System;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using NPack.Interfaces;

namespace ProjNet.CoordinateSystems.Transformations
{
	/// <summary>
	/// Describes a coordinate transformation. This class only describes a 
	/// coordinate transformation, it does not actually perform the transform 
	/// operation on points. To transform points you must use a <see cref="MathTransform"/>.
	/// </summary>
    public class CoordinateTransformation<TCoordinate> : ICoordinateTransformation<TCoordinate>
        where TCoordinate : ICoordinate, IEquatable<TCoordinate>, IComparable<TCoordinate>, IComputable<TCoordinate>, IConvertible
    {
        private readonly ICoordinateSystem<TCoordinate> _source;
        private readonly ICoordinateSystem<TCoordinate> _target;
        private readonly TransformType _transformType;
        private readonly String _areaOfUse;
        private readonly String _authority;
        private readonly long _authorityCode;
        private readonly IMathTransform<TCoordinate> _mathTransform;
        private readonly String _name;
        private readonly String _remarks;

		/// <summary>
		/// Initializes an instance of a CoordinateTransformation
		/// </summary>
		/// <param name="source">Source coordinate system</param>
		/// <param name="target">Target coordinate system</param>
		/// <param name="transformType">Transformation type</param>
		/// <param name="mathTransform">Math transform</param>
		/// <param name="name">Name of transform</param>
		/// <param name="authority">Authority</param>
		/// <param name="authorityCode">Authority code</param>
		/// <param name="areaOfUse">Area of use</param>
		/// <param name="remarks">Remarks</param>
		internal CoordinateTransformation(ICoordinateSystem<TCoordinate> source, ICoordinateSystem<TCoordinate> target, TransformType transformType, IMathTransform<TCoordinate> mathTransform, 
										String name, String authority, long authorityCode, String areaOfUse, String remarks)
		{
			_target = target;
			_source = source;
			_transformType = transformType;
			_mathTransform = mathTransform;
			_name = name;
			_authority = authority;
			_authorityCode = authorityCode;
			_areaOfUse = areaOfUse;
			_remarks = remarks;			
		}

		#region ICoordinateTransformation Members

		/// <summary>
		/// Human readable description of domain in source coordinate system.
		/// </summary>		
		public String AreaOfUse
		{
			get { return _areaOfUse; }
		}

		/// <summary>
		/// Authority which defined transformation and parameter values.
		/// </summary>
		/// <remarks>
		/// An Authority is an organization that maintains definitions of Authority Codes. For example the European Petroleum Survey Group (EPSG) maintains a database of coordinate systems, and other spatial referencing objects, where each object has a code number ID. For example, the EPSG code for a WGS84 Lat/Lon coordinate system is ‘4326’
		/// </remarks>
		public String Authority
		{
			get { return _authority; }
		}

		/// <summary>
		/// Code used by authority to identify transformation. An empty String is used for no code.
		/// </summary>
		/// <remarks>The AuthorityCode is a compact String defined by an Authority to reference a particular spatial reference object. For example, the European Survey Group (EPSG) authority uses 32 bit integers to reference coordinate systems, so all their code strings will consist of a few digits. The EPSG code for WGS84 Lat/Lon is ‘4326’.</remarks>
		public long AuthorityCode
		{
			get { return _authorityCode; }
		}

		/// <summary>
		/// Gets math transform.
		/// </summary>
        public IMathTransform<TCoordinate> MathTransform
		{
			get { return _mathTransform; }
		}

		/// <summary>
		/// Name of transformation.
		/// </summary>
		public String Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets the provider-supplied remarks.
		/// </summary>
		public String Remarks
		{
			get { return _remarks; }
		}

		/// <summary>
		/// Source coordinate system.
		/// </summary>
        public ICoordinateSystem<TCoordinate> Source
		{
			get { return _source; }
		}

		/// <summary>
		/// Target coordinate system.
		/// </summary>
        public ICoordinateSystem<TCoordinate> Target
		{
			get { return _target; }
		}

		/// <summary>
		/// Semantic type of transform. For example, a datum transformation or a coordinate conversion.
		/// </summary>
		public TransformType TransformType
		{
			get { return _transformType; }
		}

		#endregion
	}
}
