using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SharpMap.Geometries;

namespace SharpMap
{
    public class FeatureDataView : DataView
    {
        private BoundingBox _visibleRegion;
        private FeatureDataTable _table;

        public FeatureDataView(FeatureDataTable table)
        {
            _table = table;
        }

        public BoundingBox VisibleRegion
        {
            get { return _visibleRegion; }
            set { _visibleRegion = value; }
        }
    }
}
