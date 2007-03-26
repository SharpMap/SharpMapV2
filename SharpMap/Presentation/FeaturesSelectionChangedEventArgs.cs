using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Data;

namespace SharpMap.Presentation
{
    public class FeaturesSelectionChangedEventArgs : EventArgs
    {
        IEnumerable<FeatureDataRow> _selectedFeatures;

        public FeaturesSelectionChangedEventArgs(IEnumerable<FeatureDataRow> selectedFeatures)
        {
            _selectedFeatures = selectedFeatures;
        }

        public IEnumerable<FeatureDataRow> SelectedFeatures
        {
            get { return _selectedFeatures; }
        }
    }
}
