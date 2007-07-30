using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Styles
{
    public interface IFeatureLayerStyle
    {
        /// <summary>
        /// Gets or sets a value to determine if features can 
        /// be selected on this layer.
        /// </summary>
        bool AreFeaturesSelectable { get; set; }
    }
}
