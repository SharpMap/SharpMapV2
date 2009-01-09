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
using GeoAPI.Geometries;
using SharpMap.Data;
using System.Globalization;
using SharpMap.Expressions;

namespace SharpMap.Layers
{
    /// <summary>
    /// Interface to a layer of features in a <see cref="Map{TCoordinate}"/>.
    /// </summary>
	public interface IFeatureLayer : ILayer
	{
        /// <summary>
        /// Gets the data source for this layer as a more 
        /// strongly-typed <see cref="IFeatureProvider"/>.
        /// </summary>
		new IFeatureProvider DataSource { get; }

        /// <summary>
        /// Gets an <see cref="IFeaturesView"/> of cached features for the layer.
        /// </summary>
        IFeaturesView Features { get; }

        /// <summary>
        /// Gets an <see cref="IFeaturesView"/> of features which have been 
        /// highlighted.
        /// </summary>
		IFeaturesView HighlightedFeatures { get; }

        LogicExpression HighlightedFilter { get; set; }

        /// <summary>
        /// Gets the <see cref="CultureInfo"/> used to encode text
        /// and format numbers for this layer.
        /// </summary>
        CultureInfo Locale { get; }

        /// <summary>
        /// Gets an <see cref="IFeaturesView"/> of features which have been 
        /// selected.
        /// </summary>
        IFeaturesView SelectedFeatures { get; }

        LogicExpression SelectedFilter { get; set; }

        IGeometryFactory GeometryFactory { get; set; }

        Boolean IsInteractive { get; set; }

        IFeaturesView Select(FeatureQueryExpression query);
    }
}