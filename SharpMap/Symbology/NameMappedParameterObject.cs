using System;
using System.Collections.Generic;
using GeoAPI.DataStructures.Collections.Generic;

namespace SharpMap.Symbology
{
    public abstract class NameMappedParameterObject 
    {
        private readonly HybridDictionary<String, ParameterValue> _parameterMap
            = new HybridDictionary<String, ParameterValue>();

        protected IDictionary<String, ParameterValue> ParameterMap
        {
            get { return _parameterMap; }
        }
    }
}