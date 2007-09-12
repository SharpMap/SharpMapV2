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
using SharpMap.Features;
using SharpMap.Geometries;
using SharpMap.Rendering;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Layers
{
    /// <summary>
    /// A layer to generate labels from feature data.
    /// </summary>
    /// <example>
    /// Creates a new label layer and sets the label text to the 
    /// "Name" column in the FeatureDataTable of the datasource
    /// <code lang="C#">
    /// //Set up a label layer
    /// </code>
    /// </example>
    public class LabelLayer<TLabel> : FeatureLayer
    {
        #region Nested Classes

        /// <summary>
        /// Delegate method for creating advanced label text.
        /// </summary>
        /// <param name="feature">The feature to label.</param>
        /// <returns>A string to display as the label for the feature.</returns>
        public delegate string GenerateLabelTextDelegate(FeatureDataRow feature);

        #endregion

        #region Fields

        private int _priority;
        private string _rotationColumn;
        private GenerateLabelTextDelegate _getLabelMethod;
        private string _labelColumn;
        private LabelFilterDelegate _labelFilter;
        private MultipartGeometryLabelingBehavior _multipartGeometryBehaviour;

        #endregion

        #region Object Construction / Disposal

        /// <summary>
        /// Creates a new instance of a LabelLayer with the given name.
        /// </summary>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="dataSource">Data source provider for the layer.</param>
        public LabelLayer(string layerName, IFeatureLayerProvider dataSource)
            : base(layerName, dataSource)
        {
            _multipartGeometryBehaviour = MultipartGeometryLabelingBehavior.Default;
            _labelFilter = LabelCollisionDetection2D.SimpleCollisionDetection;
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the object.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (DataSource is IDisposable && DataSource != null)
            {
                (DataSource as IDisposable).Dispose();
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
        /// Delegate for performing filtering on labels.
        /// </summary>
        /// <remarks>
        /// Default method is 
        /// <see cref="LabelCollisionDetection2D.SimpleCollisionDetection"/>.
        /// </remarks>
        public LabelFilterDelegate LabelFilter
        {
            get { return _labelFilter; }
            set { _labelFilter = value; }
        }

        /// <summary>
        /// Data column or expression where label text is extracted from.
        /// </summary>
        /// <remarks>
        /// This property is overriden by the 
        /// <see cref="GenerateLabelTextDelegate"/>.
        /// </remarks>
        public string LabelColumn
        {
            get { return _labelColumn; }
            set { _labelColumn = value; }
        }

        /// <summary>
        /// Gets or sets the method for creating a custom label string 
        /// based on a feature.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this method is not null, it will override the 
        /// <see cref="LabelColumn"/> value.
        /// </para>
        /// <para>
        /// The label delegate must take a <see cref="FeatureDataRow"/> 
        /// and return a string.
        /// </para>
        /// <example>
        /// Creating a label-text by combining attributes "ROADNAME" 
        /// and "STATE" into one string, using an anonymous delegate:
        /// <code lang="C#">
        /// myLabelLayer.LabelStringDelegate = delegate(FeatureDataRow fdr)
        ///				{ return fdr["ROADNAME"].ToString() + ", " + fdr["STATE"].ToString(); };
        /// </code>
        /// </example>
        /// </remarks>
        public GenerateLabelTextDelegate LabelTextDelegate
        {
            get { return _getLabelMethod; }
            set { _getLabelMethod = value; }
        }

        /// <summary>
        /// Data column from where the label rotation is derived.
        /// If this is empty, rotation will be zero, or aligned to a linestring.
        /// Rotation are in degrees (positive = clockwise).
        /// </summary>
        public string RotationColumn
        {
            get { return _rotationColumn; }
            set { _rotationColumn = value; }
        }

        /// <summary>
        /// A value indication the priority of the label in cases of label-collision detection.
        /// </summary>
        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public string GetLabelText(FeatureDataRow feature)
        {
            if (_getLabelMethod != null)
            {
                return _getLabelMethod(feature);
            }
            else
            {
                if (feature.IsNull(LabelColumn))
                {
                    return String.Empty;
                }
                else
                {
                    return feature[LabelColumn].ToString();
                }
            }
        }

        /// <summary>
        /// Clones the object
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            throw new NotImplementedException();
        }

        #region IFeatureLayer Members


        public System.Globalization.CultureInfo Locale
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}