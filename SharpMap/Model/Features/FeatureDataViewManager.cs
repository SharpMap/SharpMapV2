using System;
using System.Data;

namespace SharpMap
{
    public class FeatureDataViewManager : DataViewManager
    {
        public FeatureDataViewManager(FeatureDataSet dataSet, bool locked)
        {
            throw new NotImplementedException();
        }

        public new FeatureDataView CreateDataView(DataTable table)
        {
            if (DataSet == null)
            {
                base.CreateDataView(table);
            }

            if (!(table is FeatureDataTable))
            {
                throw new ArgumentException("DataTable must be of type FeatureDataTable");
            }

            FeatureDataView view = new FeatureDataView(table as FeatureDataTable);
            view.SetDataViewManager(this);
            return view;
        }

        internal void DecrementViewCount()
        {
            throw new NotImplementedException();
        }

        internal void IncrementViewCount()
        {
            throw new NotImplementedException();
        }
    }
}
