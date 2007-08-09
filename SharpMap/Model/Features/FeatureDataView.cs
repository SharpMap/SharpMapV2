using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SharpMap.Geometries;

namespace SharpMap
{
    public class FeatureDataView : DataView, IEnumerable<FeatureDataRow>
    {
        private BoundingBox _visibleRegion;
        private FeatureDataTable _table;

        public FeatureDataView(FeatureDataTable table)
        {
            _table = table;
        }

        protected override void IndexListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            base.IndexListChanged(sender, e);
        }

        public BoundingBox VisibleRegion
        {
            get { return _visibleRegion; }
            set 
            {
                this[
                _visibleRegion = value; 
            }
        }

        #region IEnumerable<FeatureDataRow> Members

        public new IEnumerator<FeatureDataRow> GetEnumerator()
        {
            IEnumerator e = base.GetEnumerator();

            while(e.MoveNext())
            {
                FeatureDataRow feature = e.Current as FeatureDataRow;
                yield return feature;
            }
        }

        #endregion
    }
}
