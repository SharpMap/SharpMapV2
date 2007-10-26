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
using System.Collections.Generic;
using System.Text;

namespace SharpMap.CoordinateSystems
{
	/// <summary>
	/// The Projection class defines the standard information stored with a projection
	/// objects. A projection object implements a coordinate transformation from a geographic
	/// coordinate system to a projected coordinate system, given the ellipsoid for the
	/// geographic coordinate system. It is expected that each coordinate transformation of
	/// interest, e.g., Transverse Mercator, Lambert, will be implemented as a class of
	/// type Projection, supporting the IProjection interface.
	/// </summary>
	public class Projection : Info, IProjection
	{
		private List<ProjectionParameter> _parameters;
		private String _className;

		internal Projection(String className, IList<ProjectionParameter> parameters,
			String name, String authority, Int64 code, String alias,
			String remarks, String abbreviation)
			: base(name, authority, code, alias, abbreviation, remarks)
		{
			_parameters = new List<ProjectionParameter>(parameters);
			_className = className;
		}

		#region Predefined projections
		#endregion

		#region IProjection Members

		/// <summary>
		/// Gets the number of parameters of the projection.
		/// </summary>
		public Int32 NumParameters
		{
			get { return _parameters.Count; }
		}

		/// <summary>
		/// Gets or sets the parameters of the projection
		/// </summary>
		internal IList<ProjectionParameter> Parameters
		{
			get { return _parameters; }
			set { _parameters = new List<ProjectionParameter>(value); }
		}

		/// <summary>
		/// Gets an indexed parameter of the projection.
		/// </summary>
		/// <param name="n">Index of parameter</param>
		/// <returns>n'th parameter</returns>
		public ProjectionParameter GetParameter(Int32 n)
		{
			return _parameters[n];
		}

		/// <summary>
		/// Gets an named parameter of the projection.
		/// </summary>
		/// <remarks>The parameter name is case insensitive</remarks>
		/// <param name="name">Name of parameter</param>
		/// <returns>parameter or null if not found</returns>
		public ProjectionParameter GetParameter(String name)
		{
			return _parameters.Find(delegate(ProjectionParameter par)
					{ return par.Name.Equals(name, StringComparison.OrdinalIgnoreCase); });
		}

		/// <summary>
		/// Gets the projection classification name (e.g. "Transverse_Mercator").
		/// </summary>
		public String ClassName
		{
			get { return _className; }
		}

		/// <summary>
		/// Returns the Well-known text for this object
		/// as defined in the simple features specification.
		/// </summary>
		public override String Wkt
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat("PROJECTION[\"{0}\"", Name);

				if (!String.IsNullOrEmpty(Authority) && AuthorityCode > 0)
				{
					sb.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", Authority, AuthorityCode);
				}

				sb.Append("]");
				return sb.ToString();
			}
		}

		/// <summary>
		/// Gets an XML representation of this object
		/// </summary>
		public override String Xml
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendFormat(NumberFormat, "<CS_Projection Classname=\"{0}\">{1}", ClassName, InfoXml);

				foreach (ProjectionParameter param in Parameters)
				{
					sb.Append(param.XML);
				}

				sb.Append("</CS_Projection>");
				return sb.ToString();
			}
		}

		/// <summary>
		/// Checks whether the values of this instance is equal to the values of another instance.
		/// Only parameters used for coordinate system are used for comparison.
		/// Name, abbreviation, authority, alias and remarks are ignored in the comparison.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns>True if equal</returns>
		public override Boolean EqualParams(object obj)
		{
			if (!(obj is Projection))
			{
				return false;
			}

			Projection proj = obj as Projection;

			if (proj.NumParameters != this.NumParameters)
			{
				return false;
			}

			for (Int32 i = 0; i < _parameters.Count; i++)
			{
				ProjectionParameter param = _parameters.Find(delegate(ProjectionParameter par)
				{
					return par.Name.Equals(proj.GetParameter(i).Name, StringComparison.OrdinalIgnoreCase);
				});

				if (param == null)
				{
					return false;
				}

				if (param.Value != proj.GetParameter(i).Value)
				{
					return false;
				}
			}
			return true;
		}

		#endregion
	}
}
