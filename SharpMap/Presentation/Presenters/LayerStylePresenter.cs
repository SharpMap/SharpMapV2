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
using GeoAPI.Coordinates;
using NPack.Interfaces;
using SharpMap.Layers;
using SharpMap.Presentation.Views;
using SharpMap.Symbology;

namespace SharpMap.Presentation.Presenters
{
    /// <summary>
    /// Provides a presenter for the <see cref="Style"/> of an <see cref="ILayer"/>.
    /// </summary>
    public class LayerStylePresenter<TCoordinate> : BasePresenter<TCoordinate, ILayerStyleView>
        where TCoordinate : ICoordinate<TCoordinate>, IEquatable<TCoordinate>,
                            IComparable<TCoordinate>, IConvertible,
                            IComputable<Double, TCoordinate>
    {
        /// <summary>
        /// Creates a new instance of a <see cref="LayerStylePresenter{TCoordinate}"/> with the given 
        /// <see cref="Map{TCoordinate}"/> instance and the given 
        /// concrete <see cref="ILayersView"/> implementation.
        /// </summary>
        /// <param name="map"><see cref="Map{TCoordinate}"/> instance to present.</param>
        /// <param name="view"><see cref="ILayerStyleView"/> to present to.</param>
        public LayerStylePresenter(Map<TCoordinate> map, ILayerStyleView view)
            :base(map, view) { }
    }
}