/*
 *  The attached / following is part of SharpMap.Data.Providers.SpatiaLite2
 *  SharpMap.Data.Providers.SpatiaLite2 is free software © 2008 Ingenieurgruppe IVV GmbH & Co. KG, 
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
 *  - SqLite, dedicated to public domain
 *    http://www.sqlite.org
 *  - SpatiaLite-2.2 by Alessandro Furieri released under a disjunctive tri-license:
 *    - 'Mozilla Public License, version 1.1 or later' OR
 *    - 'GNU General Public License, version 2.0 or later' 
 *    - 'GNU Lesser General Public License, version 2.1 or later' <--- this is the one
 *    http://www.gaia-gis.it/spatialite-2.2/index.html
 *    
 *  - SQLite ADO.NET 2.0 Provider by Robert Simpson, dedicated to public domain
 *    http://sourceforge.net/projects/sqlite-dotnet2
 *    
 */
using System;
using System.Collections.Generic;
using System.Text;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Db;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.SpatiaLite2
{
    internal class SpatiaLite2ExpressionTreeToSqlCompiler
        : ExpressionTreeToSqlCompilerBase<long>
    {

        private readonly SpatiaLite2IndexType _spatialIndexType;

        public SpatiaLite2ExpressionTreeToSqlCompiler(SpatialDbProviderBase<long> provider,
                                                       Expression query,
                                                       SpatiaLite2IndexType indexType)

            : base(provider, query)
        {

            //switch (indexType)
            //{
            //    case SpatiaLite2IndexType.None:
            //        throw new SpatiaLite2Exception("indexType must not be 'None'");
            //    default:
            //        break;
            //}
            _spatialIndexType = indexType;
        }


        protected override void WriteSpatialGeometryExpressionSqlInternal(StringBuilder builder, SpatialOperation op,
                                                                  IGeometry geom)
        {

            if (op != SpatialOperation.None)
            {
                builder.Append(string.Format(" {0}( GeomFromWKB({1}), {2} )",
                  Enum.GetName(typeof(SpatialOperation), op),
                  CreateParameter(geom).ParameterName,
                  CreateParameter(Provider.GeometryColumn).ParameterName));

                builder.Append(" AND");
                WriteSpatialExtentsExpressionSqlInternal(builder, op, geom.Extents);
            }

        }

        protected override void WriteSpatialExtentsExpressionSqlInternal(StringBuilder builder,
                                                                 SpatialOperation spatialOperation, IExtents ext)
        {
            IExtents2D exts = (IExtents2D)ext;
            String criteriaClause = "";
            String whereClause = "";

            builder.Append( string.Format( " Mbr{0}( BuildMbr({1}, {2}, {3}, {4}),{5} )",
                spatialOperation.ToString(),
                CreateParameter<double>( exts.XMin ).ParameterName,
                CreateParameter<double>( exts.YMin ).ParameterName,
                CreateParameter<double>( exts.XMax ).ParameterName,
                CreateParameter<double>( exts.YMax ).ParameterName,
                Provider.GeometryColumn ) );
            /*

            switch (_spatialIndexType)
            {
                case SpatiaLite2IndexType.None:
                    switch ( spatialOperation )
                    {
                        case SpatialOperation.Within:
                        case SpatialOperation.Equals:
                            //(minx >= pc->minx && maxx <= pc->maxx && miny >= pc->miny && maxy <= pc->maxy)
                            criteriaClause = string.Format(
                                "({0}>=MBRMinX({4}) AND {2}<=MBRMaxX({4}) AND {1}>=MBRMinY({4}) AND {3}<=MBRMaxY({4}))",
                                "{0}", "{1}", "{2}", "{3}", Provider.GeometryColumn );
                            break;
                        case SpatialOperation.Contains:
                            //(pc->minx >= minx && pc->maxx <= maxx && pc->miny >= miny && pc->maxy <= maxy)
                            criteriaClause = string.Format(
                                "(MBRMinX({4})>={0} AND MBRMaxX({4})<={2} AND MBRMinY({4})>={1} AND MBRMaxY({4})<={3})",
                                "{0}", "{1}", "{2}", "{3}", Provider.GeometryColumn );
                            break;
                        case SpatialOperation.Intersects:
                        case SpatialOperation.Crosses:
                        case SpatialOperation.Overlaps:
                        case SpatialOperation.Touches:
                            criteriaClause = "(MBRMaxX({4})>={0} AND MBRMaxY({4})>={1} AND MBRMinX({4})<={2} AND MBRMinY({4})<={3})";
                            break;
                        default: break;
                    }
                    if ( String.IsNullOrEmpty( criteriaClause ) ) break;

                    whereClause = string.Format( "[{0}].ROWID in (SELECT [{0}].ROWID FROM {1} WHERE {2})", Provider.Table, Provider.QualifiedTableName, criteriaClause );
                    break;

                case SpatiaLite2IndexType.RTree:
                    switch (spatialOperation)
                    {
                        case SpatialOperation.Within:
                        case SpatialOperation.Equals:
                            //(minx >= pc->minx && maxx <= pc->maxx && miny >= pc->miny && maxy <= pc->maxy)
                            criteriaClause = "({0}>=xmin AND {2}<=xmax AND {1}>=ymin AND {3}<=ymax)";
                            break;
                        case SpatialOperation.Contains:
                            //(pc->minx >= minx && pc->maxx <= maxx && pc->miny >= miny && pc->maxy <= maxy)
                            criteriaClause = "(xmin>={0} AND xmax<={2} AND ymin>={1} AND ymax<={3})";
                            break;
                        case SpatialOperation.Intersects:
                        case SpatialOperation.Crosses:
                        case SpatialOperation.Overlaps:
                        case SpatialOperation.Touches:
                            criteriaClause = "(xmax>={0} AND ymax>={1} AND xmin<={2} AND ymin<={3})";
                            break;
                        default: break;
                    }
                    if (String.IsNullOrEmpty(criteriaClause)) break;

                    whereClause = string.Format("[{0}].ROWID in (SELECT pkid FROM idx_{0}_{1} WHERE {2})", Provider.Table, Provider.GeometryColumn, criteriaClause);
                    break;

                case SpatiaLite2IndexType.MBRCache:

                    switch (spatialOperation)
                    {
                        case SpatialOperation.Contains:
                        case SpatialOperation.Equals:
                            criteriaClause = "FilterMbrContains({0}, {1}, {2}, {3})";
                            break;
                        case SpatialOperation.Within:
                            criteriaClause = "FilterMbrWithin({0}, {1}, {2}, {3})";
                            break;
                        case SpatialOperation.Intersects:
                        case SpatialOperation.Crosses:
                        case SpatialOperation.Overlaps:
                        case SpatialOperation.Touches:
                            criteriaClause = "FilterMbrIntersects({0}, {1}, {2}, {3})";
                            break;
                        default:
                            break;
                    }
                    if (String.IsNullOrEmpty(criteriaClause)) break;

                    whereClause = String.Format(" [{0}].ROWID in (SELECT ROWID FROM cache_{0}_{1} WHERE mbr={2})",
                        Provider.Table,
                        Provider.GeometryColumn,
                        criteriaClause);

                    break;

                default:
                    throw new SpatiaLite2Exception("Invalid spatial index type");
            }
            */
            if (!String.IsNullOrEmpty(whereClause))
            {
                builder.AppendFormat(whereClause,
                   CreateParameter(exts.XMin).ParameterName,
                   CreateParameter(exts.YMin).ParameterName,
                   CreateParameter(exts.XMax).ParameterName,
                   CreateParameter(exts.YMax).ParameterName);
            }
            else
            {
                //remove possible trailing " AND" statement
                if (builder.ToString().EndsWith(" AND"))
                    builder.Remove(builder.Length - 4, 4);
            }
        }
    }

}