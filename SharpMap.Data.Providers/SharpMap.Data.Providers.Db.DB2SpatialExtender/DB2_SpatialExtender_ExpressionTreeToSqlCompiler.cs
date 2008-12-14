/*
 *  The attached / following is part of SharpMap.Data.Providers.DB2_SpatialExtender
 *  SharpMap.Data.Providers.PostGis is free software © 2008 Ingenieurgruppe IVV GmbH & Co. KG, 
 *  www.ivv-aachen.de; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: Felix Obermaier 2008
 *  
 *  This work is based on SharpMap.Data.Providers.Db by John Diss for 
 *  Newgrove Consultants Ltd, www.newgrove.com
 *  
 *  Other than that, this spatial data provider requires:
 *  - SharpMap by Rory Plaire, Morten Nielsen and others released under LGPL
 *    http://www.codeplex.com/SharpMap
 *    
 *  - GeoAPI.Net by Rory Plaire, Morten Nielsen and others released under LGPL
 *    http://www.codeplex.com/GeoApi
 *  
 *  - DB2 .NET Data Provider from IBM
 *    http://www-01.ibm.com/software/data/db2/windows/dotnet.html
 *    
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Db;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.DB2_SpatialExtender
{
    public class DB2_SpatialExtender_ExpressionTreeToSqlCompiler<TOid>
        : ExpressionTreeToSqlCompilerBase<TOid>
    {

        public DB2_SpatialExtender_ExpressionTreeToSqlCompiler(DB2_SpatialExtender_Provider<TOid> provider,
                                                       Expression query
                                                       )

            : base(provider, query)
        {

        }

        protected override void WriteSpatialGeometryExpressionSql(StringBuilder builder, SpatialOperation op,
                                                                  IGeometry geom)
        {

            if (op != SpatialOperation.None)
            {
                IGeometry geoms =
                    (Provider.SpatialReference == null || Provider.SpatialReference.EqualParams(Provider.OriginalSpatialReference))
                    ?
                    geom
                    :
                    Provider.CoordinateTransformation.Inverse.Transform(geom, Provider.GeometryFactory);
                    
                builder.Append(string.Format(" {0}.ST_{1}( {0}.ST_GEOMFROMWKB(CAST({2} AS BLOB(2G)), {3}), {4} ) = 1",
                    DB2_SpatialExtender_ProviderStatic.DefaultSpatialSchema,
                    Enum.GetName(typeof(SpatialOperation), op).ToUpper(),
                    CreateParameter(geom).ParameterName,
                    ((DB2_SpatialExtender_Provider<TOid>)Provider).DB2SrsId,
                    Provider.QualifyColumnName(Provider.GeometryColumn)));
            }

        }

        protected override void WriteSpatialExtentsExpressionSql(StringBuilder builder,
                                                                 SpatialOperation spatialOperation, IExtents ext)
        {
            //IExtents2D exts = (IExtents2D)ext;
            //ST_SetSRID('BOX3D(0 0,1 1)'::box3d,4326)
            String whereClause = "";

            IExtents2D exts = 
                (Provider.SpatialReference == null || Provider.SpatialReference.EqualParams(Provider.OriginalSpatialReference))
                ?
                (IExtents2D)ext
                :
                (IExtents2D)Provider.CoordinateTransformation.Inverse.Transform(ext, Provider.GeometryFactory);

            whereClause = string.Format(" {0}.ENVELOPESINTERSECT({1}, CAST({2} AS DOUBLE), CAST({3} AS DOUBLE), CAST({4} AS DOUBLE), CAST({5} AS DOUBLE), {6}) = 1",
                DB2_SpatialExtender_ProviderStatic.DefaultSpatialSchema,
                Provider.QualifyColumnName(Provider.GeometryColumn),
                exts.XMin.ToString("g", System.Globalization.CultureInfo.InvariantCulture), //CreateParameter(exts.XMin).ParameterName,
                exts.YMin.ToString("g", System.Globalization.CultureInfo.InvariantCulture), //CreateParameter(exts.YMin).ParameterName,
                exts.XMax.ToString("g", System.Globalization.CultureInfo.InvariantCulture), //CreateParameter(exts.XMax).ParameterName,
                exts.YMax.ToString("g", System.Globalization.CultureInfo.InvariantCulture), //CreateParameter(exts.YMax).ParameterName,
                ((DB2_SpatialExtender_Provider<TOid>)Provider).DB2SrsId);
            builder.Append(whereClause);
        }

        //public override IDataParameter CreateParameter<TValue>(TValue value)
        //{
        //    ///if TValue is System.Object we need to expand it to generate the correct parameter type
        //    if (typeof(TValue) == typeof(object) && value.GetType() != typeof(object))
        //        return CreateParameterFromObject(value);

        //    object key = value;
        //    if (Equals(null, value))
        //        key = new NullValue<TValue>();

        //    //if (ParameterCache.ContainsKey(key))
        //    //    return ParameterCache[key];

        //    IDataParameter p;
        //    if (value is IGeometry)
        //        p = Provider.DbUtility.CreateParameter(string.Format("iparam{0}", ParameterCache.Count),
        //                                               ((IGeometry)value).AsBinary(),
        //                                               ParameterDirection.Input);


        //    else
        //        p = Provider.DbUtility.CreateParameter(string.Format("iparam{0}", ParameterCache.Count),
        //                                               value,
        //                                               ParameterDirection.Input);

        //    ParameterCache.Add(key, p);

        //    return p;
        //}

    }

}