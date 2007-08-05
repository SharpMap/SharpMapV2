// Copyright 2006 - Morten Nielsen (www.iter.dk)
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
//
// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SharpMap.Geometries;
using SharpMap.Data;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.CoordinateSystems;

namespace SharpMap.Data.Providers
{
    /// <summary>
    /// Datasource for storing a limited set of geometries.
    /// </summary>
    /// <remarks>
    /// <para>The GeometryProvider doesn’t utilize performance optimizations of spatial indexing,
    /// and thus is primarily meant for rendering a limited set of Geometries.</para>
    /// <para>A common use of the GeometryProvider is for highlighting a set of selected features.</para>
    /// <example>
    /// The following example gets data within a BoundingBox of another datasource and adds it to the map.
    /// <code lang="C#">
    /// List&#60;Geometry&#62; geometries = myMap.Layers[0].DataSource.GetGeometriesInView(myBox);
    /// VectorLayer laySelected = new VectorLayer("Selected Features");
    /// laySelected.DataSource = new GeometryProvider(geometries);
    /// laySelected.Style.Outline = new Pen(Color.Magenta, 3f);
    /// laySelected.Style.EnableOutline = true;
    /// myMap.Layers.Add(laySelected);
    /// </code>
    /// </example>
    /// <example>
    /// Adding points of interest to the map. This is useful for vehicle tracking etc.
    /// <code lang="C#">
    /// List&#60;SharpMap.Geometries.Geometry&#62; geometries = new List&#60;SharpMap.Geometries.Geometry&#62;();
    /// //Add two points
    /// geometries.Add(new SharpMap.Geometries.Point(23.345,64.325));
    /// geometries.Add(new SharpMap.Geometries.Point(23.879,64.194));
    /// SharpMap.Layers.VectorLayer layerVehicles = new SharpMap.Layers.VectorLayer("Vechicles");
    /// layerVehicles.DataSource = new SharpMap.Data.Providers.GeometryProvider(geometries);
    /// layerVehicles.Style.Symbol = Bitmap.FromFile(@"C:\data\car.gif");
    /// myMap.Layers.Add(layerVehicles);
    /// </code>
    /// </example>
    /// </remarks>
    public class GeometryProvider : IProvider<uint>, IDisposable
    {
		private ICoordinateTransformation _coordinateTransformation;
		private ICoordinateSystem _coordinateSystem;
        private List<Geometry> _geometries = new List<Geometry>();
        private int _srid = -1;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryProvider"/>
        /// </summary>
        /// <param name="geometries">Set of geometries that this datasource should contain</param>
        public GeometryProvider(IEnumerable<Geometry> geometries)
        {
            _geometries = new List<Geometry>(geometries);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryProvider"/>
        /// </summary>
        /// <param name="feature">Feature to be in this datasource</param>
        public GeometryProvider(FeatureDataRow feature)
        {
            _geometries = new List<Geometry>();
            _geometries.Add(feature.Geometry);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryProvider"/>
        /// </summary>
        /// <param name="features">Features to be included in this datasource</param>
        public GeometryProvider(FeatureDataTable features)
        {
            _geometries = new List<Geometry>();

            for (int i = 0; i < features.Count; i++)
            {
                _geometries.Add(features[i].Geometry);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryProvider"/>
        /// </summary>
        /// <param name="geometry">Geometry to be in this datasource</param>
        public GeometryProvider(Geometry geometry)
        {
            _geometries = new List<Geometry>();
            _geometries.Add(geometry);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryProvider"/>
        /// </summary>
        /// <param name="wellKnownBinaryGeometry"><see cref="SharpMap.Geometries.Geometry"/> as Well-known Binary to be included in this datasource</param>
        public GeometryProvider(byte[] wellKnownBinaryGeometry)
            : this(SharpMap.Converters.WellKnownBinary.GeometryFromWKB.Parse(wellKnownBinaryGeometry))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryProvider"/>
        /// </summary>
        /// <param name="wellKnownTextGeometry"><see cref="SharpMap.Geometries.Geometry"/> as Well-known Text to be included in this datasource</param>
        public GeometryProvider(string wellKnownTextGeometry)
            : this(SharpMap.Converters.WellKnownText.GeometryFromWKT.Parse(wellKnownTextGeometry))
        {
        }

        #endregion

		/// <summary>
		/// Gets or sets the geometries this datasource contains.
		/// </summary>
		public IList<Geometry> Geometries
		{
			get { return _geometries; }
			set { _geometries.Clear(); _geometries.AddRange(value); }
		}

        #region IProvider Members

        /// <summary>
        /// Returns features within the specified bounding box
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
		public IEnumerable<Geometry> GetGeometriesInView(BoundingBox box)
        {
            List<Geometry> list = new List<Geometry>();

            foreach (Geometry g in _geometries)
            {
                if (!g.IsEmpty())
                {
                    if (g.GetBoundingBox().Intersects(box))
                    {
                        list.Add(g);
                    }
                }
            }

            return list.AsReadOnly();
        }

        /// <summary>
        /// Returns all objects whose boundingbox intersects 'bbox'.
        /// </summary>
        /// <param name="bbox"></param>
        /// <returns></returns>
        public IEnumerable<uint> GetObjectIdsInView(BoundingBox box)
        {
            for (uint i = 0; i < _geometries.Count; i++)
            {
                if (_geometries[(int)i].GetBoundingBox().Intersects(box))
                {
					yield return i;
                }
            }
        }

        /// <summary>
        /// Returns the geometry corresponding to the Object ID
        /// </summary>
        /// <param name="oid">Object ID</param>
        /// <returns>geometry</returns>
        public Geometry GetGeometryById(uint oid)
        {
            return _geometries[(int)oid];
        }

        /// <summary>
        /// Throws an NotSupportedException. Attribute data is not supported by this datasource
        /// </summary>
        /// <param name="geom"></param>
        /// <param name="ds">FeatureDataSet to fill data into</param>
        public void ExecuteIntersectionQuery(Geometry geom, FeatureDataSet ds)
        {
            throw new NotSupportedException("Attribute data is not supported by the GeometryProvider.");
        }

        /// <summary>
        /// Throws an NotSupportedException. Attribute data is not supported by this datasource
        /// </summary>
        /// <param name="box"></param>
        /// <param name="ds">FeatureDataSet to fill data into</param>
        public void ExecuteIntersectionQuery(BoundingBox box, FeatureDataSet ds)
        {
            throw new NotSupportedException("Attribute data is not supported by the GeometryProvider.");
        }

        public void GetSchema(FeatureDataTable table)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ExecuteIntersectionQuery(Geometry geom, FeatureDataTable table)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public System.Data.IDataReader ExecuteIntersectionQuery(Geometry geom)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ExecuteIntersectionQuery(BoundingBox box, FeatureDataTable table)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public System.Data.IDataReader ExecuteIntersectionQuery(BoundingBox box)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Returns the number of features in the dataset
        /// </summary>
        /// <returns>number of features</returns>
        public int GetFeatureCount()
        {
            return _geometries.Count;
        }

        /// <summary>
        /// Throws an NotSupportedException. Attribute data is not supported by this datasource
        /// </summary>
        /// <param name="RowID"></param>
        /// <returns></returns>
        public FeatureDataRow<uint> GetFeature(uint oid)
        {
            throw new NotSupportedException("Attribute data is not supported by the GeometryProvider.");
        }

        /// <summary>
        /// Boundingbox of dataset
        /// </summary>
        /// <returns>boundingbox</returns>
        public BoundingBox GetExtents()
        {
            BoundingBox box = BoundingBox.Empty;

            if (_geometries.Count == 0)
            {
                return box;
            }

            foreach (Geometry g in Geometries)
            {
                if (!g.IsEmpty())
                {
                    box.ExpandToInclude(g.GetBoundingBox());
                }
            }

            return box;
        }

        /// <summary>
        /// Gets the connection ID of the datasource
        /// </summary>
        /// <remarks>
        /// The ConnectionID is meant for Connection Pooling which doesn't apply to this datasource. Instead
        /// <c>String.Empty</c> is returned.
        /// </remarks>
        public string ConnectionId
        {
            get { return String.Empty; }
        }

        /// <summary>
        /// Opens the datasource
        /// </summary>
        public void Open()
        {
            //Do nothing;
        }

        /// <summary>
        /// Closes the datasource
        /// </summary>
        public void Close()
        {
            //Do nothing;
        }

        /// <summary>
        /// Returns true if the datasource is currently open
        /// </summary>
        public bool IsOpen
        {
            get { return true; }
		}

		/// <summary>
		/// The spatial reference ID.
		/// </summary>
		public int Srid
		{
			get { return _srid; }
			set { _srid = value; }
		}

		public ICoordinateSystem SpatialReference
		{
			get { return _coordinateSystem; }
			set { _coordinateSystem = value; }
		}

		public ICoordinateTransformation CoordinateTransformation
		{
			get { return _coordinateTransformation; }
			set { _coordinateTransformation = value; }
		}

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            _geometries = null;
        }

        #endregion
    }
}