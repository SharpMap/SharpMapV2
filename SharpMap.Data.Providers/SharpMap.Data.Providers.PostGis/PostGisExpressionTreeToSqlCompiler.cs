/*
 *  The attached / following is part of SharpMap.Data.Providers.PostGis
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
 *  - Npgsql - .Net Data Provider for Postgresql Provider by 
 *    Josh Cooley,Francisco Figueiredo jr. and others, 
 *    released under this license http://npgsql.projects.postgresql.org/license.html
 *    http://npgsql.projects.postgresql.org/
 *    
 */
using System;
using System.Globalization;
using System.Text;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Db;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.PostGis
{
    internal class PostGisExpressionTreeToSqlCompiler<TOid>
        : ExpressionTreeToSqlCompilerBase<TOid>
    {
        public PostGisExpressionTreeToSqlCompiler(PostGisProvider<TOid> provider,
                                                  Expression query
            )
            : base(provider, query)
        {
        }

        protected override void WriteSpatialGeometryExpressionSqlInternal(StringBuilder builder, SpatialOperation op,
                                                                  IGeometry geom)
        {
            //int? srid = Provider.ParseSrid(geom.Srid);
            //if (!srid.HasValue || srid < 0)
            //    srid = Provider.ParseSrid(Provider.Srid);
            //if (srid < 0) srid = Provider.ParseSrid(PostGisProviderStatic.DefaultSrid);

            if (op != SpatialOperation.None)
            {
                builder.Append(string.Format(" ST_{0}( ST_GeomFromWKB({1}, {2}), {3} )",
                                             Enum.GetName(typeof (SpatialOperation), op),
                                             CreateParameter(geom).ParameterName,
                                             SridForQuery(geom.Srid),
                                             Provider.QualifyColumnName(Provider.GeometryColumn)));

                //builder.Append(" AND");
                //WriteSpatialExtentsExpressionSqlInternal(builder, op, geom.Extents);
            }
        }

        protected override void WriteSpatialExtentsExpressionSqlInternal(StringBuilder builder,
                                                                 SpatialOperation spatialOperation, IExtents ext)
        {
            var exts = (IExtents2D) ext;
            //ST_SetSRID('BOX3D(0 0,1 1)'::box3d,4326)
            String whereClause = "";

            whereClause = string.Format(" {5} && ST_SetSRID('BOX3D({0} {1}, {2} {3})'::box3d, {4})",
                                        String.Format(CultureInfo.InvariantCulture, "{0}",
                                                      CreateParameter(exts.XMin).Value),
                                        //CreateParameter(exts.XMin).ParameterName,
                                        String.Format(CultureInfo.InvariantCulture, "{0}",
                                                      CreateParameter(exts.YMin).Value),
                                        //CreateParameter(exts.YMin).ParameterName,
                                        String.Format(CultureInfo.InvariantCulture, "{0}",
                                                      CreateParameter(exts.XMax).Value),
                                        //CreateParameter(exts.XMax).ParameterName,
                                        String.Format(CultureInfo.InvariantCulture, "{0}",
                                                      CreateParameter(exts.YMax).Value),
                                        //CreateParameter(exts.YMax).ParameterName,
                                        SridForQuery(exts.SpatialReference.AuthorityCode),
                                        Provider.QualifyColumnName(Provider.GeometryColumn));
            builder.Append(whereClause);
        }

        private int SridForQuery(string srid)
        {
            int? ret = Provider.ParseSrid(srid);
            if (!ret.HasValue || ret < 0)
                ret = Provider.ParseSrid(Provider.Srid);
            if (ret < 0) ret = Provider.ParseSrid(PostGisProviderStatic.DefaultSrid);

            return ret.Value;
        }
    }
}