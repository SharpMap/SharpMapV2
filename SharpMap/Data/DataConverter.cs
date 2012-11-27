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
using System.Data;
using GeoAPI.Geometries;
#if !DOTNET35
using GeoAPI.SystemCoreReplacement;
#endif
namespace SharpMap.Data
{
    public class DataConverter<TSource, TTarget>
    {
        private readonly Func<TSource, TTarget> _conversionDelegate;

        private readonly int _oidColumnIndex;
        private readonly FeatureDataTable<TTarget> _target;

        public DataConverter(IFeatureDataRecord source, int oidColumnIndex, IGeometryFactory geometryFactory)
            : this(
                delegate(TSource o) { return (TTarget) Convert.ChangeType(o, typeof (TTarget)); }, source, oidColumnIndex, geometryFactory)
        {
        }

        public DataConverter(Func<TSource, TTarget> conversionDlgt, IFeatureDataRecord source, int oidColumnIndex, IGeometryFactory geometryFactory)
        {
            _conversionDelegate = conversionDlgt;
            _oidColumnIndex = oidColumnIndex;
            string oidColName = source.GetName(oidColumnIndex);

            FeatureDataTable<TTarget> trgt = new FeatureDataTable<TTarget>("SchemaTable", oidColName,
                                                                           geometryFactory);

            for (int i = 0; i < source.FieldCount; i++)
                if (i != oidColumnIndex)
                    trgt.Columns.Add(new DataColumn(source.GetName(i), source.GetFieldType(i)));


            _target = trgt;
        }

        public FeatureDataTable<TTarget> Model
        {
            get { return _target; }
        }

        public FeatureDataRow<TTarget> ConvertRow(IFeatureDataRecord source)
        {
            if (typeof(TSource) == typeof(TTarget) && source is FeatureDataRow)
                return source as FeatureDataRow<TTarget>;
            TTarget oid = _conversionDelegate((TSource)source.GetOid());
            FeatureDataRow<TTarget> row = _target.NewRow(oid);
            object[] vals = new object[source.FieldCount];
            vals[_oidColumnIndex] = oid;
            source.GetValues(vals);
            row.ItemArray = vals;
            //row.Geometry = source.Geometry.IsValid ? source.Geometry : source.Geometry.Buffer(0.0);
            row.Geometry = source.Geometry;

            return row;
        }
    }
}