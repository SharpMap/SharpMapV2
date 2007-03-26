using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Data;

namespace SharpMap.Presentation
{
    public interface IFeaturesDataView
    {
        IList<FeatureDataRow> Features { get; set; }
        IEnumerable<FeatureDataRow> SelectedFeatures { get; set; }
        event EventHandler<FeaturesSelectionChangedEventArgs> FeaturesSelectedChanged;
    }
}
