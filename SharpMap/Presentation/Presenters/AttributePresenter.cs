// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using System.ComponentModel;
using System.Text;

using SharpMap.Layers;

namespace SharpMap.Presentation
{
    public class AttributePresenter : BasePresenter<IAttributeView>
    {
        private IFeatureLayer _featureLayer;
        private readonly string _selectedFeaturesPropertyName;

        public AttributePresenter(SharpMap.Map map, IFeatureLayer layer, IAttributeView view)
            : base(map, view)
        {
            FeatureLayer = layer;
            FeatureLayer.PropertyChanged += new PropertyChangedEventHandler(handleFeatureLayerPropertyChanged);
        }

        public IFeatureLayer FeatureLayer
        {
            get { return _featureLayer; }
            private set { _featureLayer = value; }
        }

        void handleFeatureLayerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "LayerName":
                    View.Title = FeatureLayer.LayerName;
                    break;
                case "Envelope":
                    break;
                case "Enabled":
                    break;
                case "Srid":
                case "CoordinateSystem":
                case "CoordinateTransformation":
                case "Style":
                default:
                    break;
            }
        }

        void handleFeatureLayerSelectedFeaturesChanged(object sender, EventArgs e)
        {
            View.SelectedFeatures = FeatureLayer.SelectedFeatures;
        }
    }
}
