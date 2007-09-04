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

using System;
using System.Data;
using SharpMap.Geometries;

namespace SharpMap.Features
{
	/// <summary>
	/// Represents a geographic feature, stored as 
	/// a row of data in a <see cref="FeatureDataTable"/>.
	/// </summary>
#if !DEBUG_STEPINTO
	[System.Diagnostics.DebuggerStepThrough()]
#endif

	[Serializable]
	public class FeatureDataRow : DataRow
	{
		//private FeatureDataTable tableFeatureTable;
        private Geometry _originalGeometry;
        private Geometry _currentGeometry;
        private Geometry _proposedGeometry;
		private bool _isGeometryModified = false;

		internal FeatureDataRow(DataRowBuilder rb)
			: base(rb)
		{
		}

        /// <summary>
        /// Accepts all pending values in the feature and makes them
        /// the current state.
        /// </summary>
		public new void AcceptChanges()
		{
			base.AcceptChanges();
			_isGeometryModified = false;
		}

		/// <summary>
		/// The geometry of the feature.
		/// </summary>
		public Geometry Geometry
		{
			get { return _currentGeometry; }
			set
			{
				if (_currentGeometry == value)
				{
					return;
				}

				_currentGeometry = value;

				if (RowState != DataRowState.Detached)
				{
					_isGeometryModified = true;
					Table.RowGeometryChanged(this);
				}
			}
		}

		/// <summary>
		/// Returns true of the geometry is null.
		/// </summary>
		/// <returns></returns>
		public bool IsGeometryNull()
		{
			return Geometry == null;
		}

        /// <summary>
        /// Gets true if the <see cref="Geometry"/> value for the
        /// feature has been modified.
        /// </summary>
		public bool IsGeometryModified
		{
			get { return _isGeometryModified; }
		}

		/// <summary>
		/// Gets the <see cref="FeatureDataTable"/> for which this
		/// row has schema.
		/// </summary>
		public new FeatureDataTable Table
		{
			get { return base.Table as FeatureDataTable; }
		}

		/// <summary>
		/// Sets the geometry column to null.
		/// </summary>
		public void SetFeatureGeometryNull()
		{
			Geometry = null;
		}
	}
}