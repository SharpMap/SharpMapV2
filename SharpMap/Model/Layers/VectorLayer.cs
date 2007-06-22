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
    /// Class for vector layer properties.
    /// </summary>
    /// <example>
    /// Adding a <see cref="VectorLayer"/> to a map:
    /// </example>
    public class VectorLayer : Layer, IFeatureLayer
    {
        private IProvider _dataSource;
		private Predicate<FeatureDataRow> _featureSelectionClause;
		private readonly object _selectedFeaturesSync = new object();
		private List<FeatureDataRow> _selectedFeatures = new List<FeatureDataRow>();

		/// <summary>
		/// Initializes a new, empty vector layer.
		/// </summary>
		public VectorLayer()
		{
		}

        /// <summary>
        /// Initializes a new layer with the given name.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        public VectorLayer(string layername)
        {
            this.LayerName = layername;
        }

        /// <summary>
        /// Initializes a new layer with the given name and datasource.
        /// </summary>
        /// <param name="layername">Name of the layer.</param>
        /// <param name="dataSource">Data source.</param>
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

		public new VectorStyle Style
		{
			get { return base.Style as VectorStyle; }
			set { base.Style = value; }
		}
		
		#region IFeatureLayer Members
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
                {
                    feature.Geometry = GeometryTransform.TransformGeometry(feature.Geometry, CoordinateTransformation.MathTransform);
                }

                yield return feature;
            }
		}

		public event EventHandler SelectedFeaturesChanged;

		public IList<FeatureDataRow> SelectedFeatures
		{
			get
			{
				lock (_selectedFeaturesSync)
				{
					return _selectedFeatures;
				}
			}
			set
			{
				lock (_selectedFeaturesSync)
				{
					_selectedFeatures.Clear();
					_selectedFeatures.AddRange(value);
					onSelectedFeaturesChanged();
				}
			}
		}

		#endregion IFeatureLayer Members

		#region ILayer Members
		/// <summary>
        /// Returns the extent of the layer.
        /// </summary>
        /// <returns>Bounding box corresponding to the extent of the features in the layer.</returns>
        public override BoundingBox Envelope
        {
            get
            {
                if (DataSource == null)
                {
					return BoundingBox.Empty;
                }

                bool wasOpen = DataSource.IsOpen;

                if (!wasOpen)
                {
                    DataSource.Open();
                }

                BoundingBox box = DataSource.GetExtents();

                if (!wasOpen) //Restore state
                {
                    DataSource.Close();
                }

                if (CoordinateTransformation != null)
                {
                    return GeometryTransform.TransformBox(box, CoordinateTransformation.MathTransform);
                }

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
                {
                    throw new InvalidOperationException("DataSource property not set on layer '" + this.LayerName + "'");
                }

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
        /// Disposes the object.
        /// </summary>
        protected override void  Dispose(bool disposing)
        {
			if (IsDisposed)
			{
				return;
			}

            if (DataSource is IDisposable)
            {
                (DataSource as IDisposable).Dispose();
            }

			base.Dispose(disposing);
        }

        #endregion

		private void onSelectedFeaturesChanged()
		{
			EventHandler e = SelectedFeaturesChanged;

			if (e != null)
			{
				e(null, EventArgs.Empty);
			}
		}
	}
}
