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

using SharpMap.Data;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Layers
{
    /// <summary>
    /// A map layer of feature labels.
    /// </summary>
    /// <example>
    /// Creates a new label layer and sets the label text to the 
    /// "Name" column in the FeatureDataTable of the datasource
    /// <code lang="C#">
    /// //Set up a label layer
    /// </code>
    /// </example>
    public class LabelLayer : FeatureLayer
    {
        #region Nested Classes

        /// <summary>
        /// Delegate method for creating advanced label text.
        /// </summary>
        /// <param name="feature">The feature to label.</param>
        /// <returns>A String to display as the label for the feature.</returns>
        public delegate String GenerateLabelTextDelegate(FeatureDataRow feature);

        #endregion

        #region Fields

        private Int32 _priority;
        private String _rotationColumn;
        //private GenerateLabelTextDelegate _getLabelMethod;
        //private String _labelColumn;
        private MultipartGeometryLabelingBehavior _multipartGeometryBehaviour;

        #endregion

        #region Object Construction / Disposal

		/// <summary>
		/// Creates a new instance of a LabelLayer with the given name.
		/// </summary>
		/// <param name="layerName">Name of the layer.</param>
		/// <param name="dataSource">Data source provider for the layer.</param>
		public LabelLayer(String layerName, IFeatureLayerProvider dataSource)
			: base(layerName, dataSource)
		{
			_multipartGeometryBehaviour = MultipartGeometryLabelingBehavior.Default;
		}

        #region IDisposable Members

        /// <summary>
        /// Disposes the object.
        /// </summary>
        protected override void Dispose(Boolean disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (DataSource != null)
            {
                DataSource.Dispose();
            }

            base.Dispose(disposing);
        }

        #endregion

        #endregion

        /// <summary>
        /// Gets or sets labeling behavior on multipart geometries.
        /// </summary>
        /// <remarks>
        /// Default value is <see cref="MultipartGeometryLabelingBehavior.All"/>.
        /// </remarks>
        public MultipartGeometryLabelingBehavior MultipartGeometryLabelingBehaviour
        {
            get { return _multipartGeometryBehaviour; }
            set { _multipartGeometryBehaviour = value; }
        }

        /// <summary>
        /// Data column from where the label rotation is derived.
        /// If this is empty, rotation will be zero, or aligned to a linestring.
        /// Rotation are in degrees (positive = clockwise).
        /// </summary>
        public String RotationColumn
        {
            get { return _rotationColumn; }
            set { _rotationColumn = value; }
        }

        /// <summary>
        /// A value indication the priority of the label in cases of label-collision detection.
        /// </summary>
        public Int32 Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        /// <summary>
        /// Clones the object
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        protected override IStyle CreateStyle()
        {
            return new LabelStyle();
		}
	}
}