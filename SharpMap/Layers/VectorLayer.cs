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
using System.Collections.Generic;
using System.Text;

using SharpMap.Geometries;
using GeoPoint = SharpMap.Geometries.Point;
using SharpMap.Data;
using SharpMap.Data.Providers;
using SharpMap.Styles;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Rendering;

namespace SharpMap.Layers
{
    /// <summary>
    /// Class for vector layer properties
    /// </summary>
    /// <example>
    /// Adding a <see cref="VectorLayer"/> to a map:
    /// <code lang="C#">
    /// // Initialize a new map
    /// SharpMap.Map myMap = new SharpMap.Map(new System.Drawing.Size(300,600));
    /// // Create a layer
    /// SharpMap.Layers.VectorLayer myLayer = new SharpMap.Layers.VectorLayer("My layer");
    /// //Add datasource
    /// myLayer.DataSource = new SharpMap.Data.Providers.ShapeFile(@"C:\data\MyShapeData.shp");
    /// // Set up styles
    /// myLayer.Style.Outline = new Pen(Color.Magenta, 3f);
    /// myLayer.Style.EnableOutline = true;
    /// myMap.Layers.Add(myLayer);
    /// // Zoom to fit the data in the view
    /// myMap.ZoomToExtents();
    /// // Render the map:
    /// System.Drawing.Image mapImage = myMap.GetMap();
    /// </code>
    /// </example>
    public class VectorLayer : Layer, IFeatureLayer, IDisposable
    {
        private IProvider _dataSource;
        private Predicate<FeatureDataRow> _featureSelectionClause;

        /// <summary>
        /// Initializes a new layer
        /// </summary>
        /// <param name="layername">Name of layer</param>
        public VectorLayer(string layername)
        {
            this.LayerName = layername;
        }

        /// <summary>
        /// Initializes a new layer with a specified datasource
        /// </summary>
        /// <param name="layername">Name of layer</param>
        /// <param name="dataSource">Data source</param>
        public VectorLayer(string layername, IProvider dataSource)
            : this(layername)
        {
            _dataSource = dataSource;
        }

        /// <summary>
        /// Gets or sets the datasource
        /// </summary>
        public IProvider DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        public IEnumerable<FeatureDataRow> GetFeatures(BoundingBox region)
        {
            FeatureDataSet ds = new FeatureDataSet();

            DataSource.Open();
            DataSource.ExecuteIntersectionQuery(region, ds);
            DataSource.Close();

            FeatureDataTable features = ds.Tables[0] as FeatureDataTable;

            foreach (FeatureDataRow feature in features)
            {
                if (this.CoordinateTransformation != null)
                    feature.Geometry = GeometryTransform.TransformGeometry(feature.Geometry, CoordinateTransformation.MathTransform);

                yield return feature;
            }
        }

        #region ILayer Members
        /// <summary>
        /// Returns the extent of the layer
        /// </summary>
        /// <returns>Bounding box corresponding to the extent of the features in the layer</returns>
        public override BoundingBox Envelope
        {
            get
            {
                if (DataSource == null)
                    throw new InvalidOperationException("DataSource property not set on layer '" + this.LayerName + "'");

                bool wasOpen = DataSource.IsOpen;

                if (!wasOpen)
                    DataSource.Open();

                BoundingBox box = DataSource.GetExtents();
                
                if (!wasOpen) //Restore state
                    DataSource.Close();
                
                if (CoordinateTransformation != null)
                    return GeometryTransform.TransformBox(box, CoordinateTransformation.MathTransform);
                
                return box;
            }
        }

        /// <summary>
        /// Gets or sets the SRID of this VectorLayer's data source
        /// </summary>
        public override int Srid
        {
            get
            {
                if (this.DataSource == null)
                    throw new InvalidOperationException("DataSource property not set on layer '" + this.LayerName + "'");

                return this.DataSource.Srid;
            }
            set { this.DataSource.Srid = value; }
        }


        #endregion

        #region ICloneable Members

        /// <summary>
        /// Clones the layer
        /// </summary>
        /// <returns>cloned object</returns>
        public override object Clone()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            if (DataSource is IDisposable)
                ((IDisposable)DataSource).Dispose();
        }

        #endregion
    }
}
