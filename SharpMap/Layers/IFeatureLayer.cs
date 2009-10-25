// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using GeoAPI.Geometries;
using SharpMap.Data;
using System.Globalization;
using SharpMap.Expressions;
using SharpMap.Rendering.Symbolize;
//using SharpMap.Rendering.Thematics;

namespace SharpMap.Layers
{
    /// <summary>
    /// Interface to a layer of features in a <see cref="Map"/>.
    /// </summary>
    public interface IFeatureLayer : ILayer
    {
        /// <summary>
        /// Gets the data source for this layer as a more 
        /// strongly-typed <see cref="IFeatureProvider"/>.
        /// </summary>
        new IFeatureProvider DataSource { get; }

        /// <summary>
        /// Gets a <see cref="FeatureDataTable"/> of cached features for the layer.
        /// </summary>
        FeatureDataTable Features { get; }

        /// <summary>
        /// Gets a <see cref="FeatureDataView"/> of features which have been 
        /// highlighted.
        /// </summary>
        FeatureDataView HighlightedFeatures { get; }

        FeatureQueryExpression HighlightedFilter { get; set; }

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> used to encode text
        /// and format numbers for this layer.
        /// </summary>
        CultureInfo Locale { get; }

        /// <summary>
        /// Gets a <see cref="FeatureDataView"/> of features which have been 
        /// selected.
        /// </summary>
        FeatureDataView SelectedFeatures { get; }

        FeatureQueryExpression SelectedFilter { get; set; }

        IGeometryFactory GeometryFactory { get; set; }

        Boolean AreFeaturesSelectable { get; set; }

        IEnumerable<FeatureDataRow> Select(FeatureQueryExpression query);

        ///// <summary>
        ///// Gets or sets the theme used to generate styles for rendered features.
        ///// </summary>
        //ITheme Theme { get; set; }



    }

    public interface IGeometryLayer : IFeatureLayer
    {
        IGeometrySymbolizer Symbolizer { get; }
    }

    public interface ILabelLayer : IFeatureLayer
    {
        TextSymbolizingDelegate TextFormatter { get; }
        ITextSymbolizer Symbolizer { get; }
    }
}