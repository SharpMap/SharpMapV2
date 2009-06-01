/*
 *	This file is part of SharpMap.Demo.FormatConverter
 *  SharpMap.Demo.FormatConverter is free software © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */

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
