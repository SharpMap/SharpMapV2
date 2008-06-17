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
using SharpMap.Presentation.Views;

namespace SharpMap.Presentation
{
    /// <summary>
    /// Arguments for the <see cref="IAttributeView.FeaturesHighlightedChanged"/> event.
    /// </summary>
    public class FeaturesHighlightedChangedEventArgs : EventArgs
    {
        private readonly IEnumerable<Int32> _highlightedFeatures;
		private readonly String _layerName;

        /// <summary>
        /// Creates a new instance of <see cref="FeaturesHighlightedChangedEventArgs"/>.
        /// </summary>
        /// <param name="layerName">The layer which the features are being highlighted on.</param>
        /// <param name="highlightedFeatures">The highlighted features.</param>
        public FeaturesHighlightedChangedEventArgs(String layerName, 
                                                   IEnumerable<Int32> highlightedFeatures)
        {
			_layerName = layerName;
            _highlightedFeatures = highlightedFeatures;
        }

        /// <summary>
        /// Gets the name of the layer which the features are being highlighted on.
        /// </summary>
    	public String LayerName
    	{
    		get { return _layerName; }
    	}

        /// <summary>
        /// Gets the features which have been highlighted.
        /// </summary>
        public IEnumerable<Int32> HighlightedFeatures
        {
            get { return _highlightedFeatures; }
        }
    }
}