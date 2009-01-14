using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SharpMap.Data;

namespace SharpMap.Demo.FormatConverter
{
    public class ConvertData<TSource, TTarget> : IConvertData<TSource, TTarget>
    {

        private readonly FeatureDataTable<TTarget> _target;
        private readonly int _oidColumnIndex;

        public ConvertData(IFeatureDataRecord source, int oidColumnIndex)
        {
            _oidColumnIndex = oidColumnIndex;
            string oidColName = source.GetName(oidColumnIndex);

            FeatureDataTable<TTarget> trgt = new FeatureDataTable<TTarget>("SchemaTable", oidColName, source.Geometry.Factory);

            for (int i = 0; i < source.FieldCount; i++)
                if (i != oidColumnIndex)
                    trgt.Columns.Add(new DataColumn(source.GetName(i), source.GetFieldType(i)));



            _target = trgt;
        }

        public FeatureDataRow<TTarget> ConvertRow(IFeatureDataRecord source)
        {
            if (typeof(TSource) == typeof(TTarget))
                return source as FeatureDataRow<TTarget>;

            FeatureDataRow<TTarget> row = _target.NewRow((TTarget)Convert.ChangeType(source.GetOid(), typeof(TTarget)));
            object[] vals = new object[source.FieldCount];
            vals[_oidColumnIndex] = (TTarget)Convert.ChangeType(source.GetOid(), typeof(TTarget));
            source.GetValues(vals);
            row.ItemArray = vals;
            row.Geometry = source.Geometry;

            return row;
        }

        #region IConvertData Members



        #endregion

        #region IConvertData Members

        FeatureDataRow IConvertData.ConvertRecord(IFeatureDataRecord source)
        {
            return ConvertRow(source);
        }

        #endregion
    }
}
