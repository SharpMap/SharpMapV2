/*
 *  The attached / following is part of SharpMap.Data.Providers.MsSqlSpatial
 *  SharpMap.Data.Providers.MsSqlSpatial is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
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
using System.Text;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Db;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.MsSqlSpatial
{
    public class MsSqlSpatialExpressionTreeToSqlCompiler
        : ExpressionTreeToSqlCompilerBase
    {
        private string _spSchema = "ST";

        public MsSqlSpatialExpressionTreeToSqlCompiler(IDbUtility dbUtility,
                                                       bool withNoLock,
                                                       Func<IEnumerable<string>> selectStarDelegate,
                                                       string geometryColumnFormatString,
                                                       Expression query,
                                                       string spatialSchema,
                                                       string tableSchema,
                                                       string tableName,
                                                       string oidColumnName,
                                                       string geometryColumnName,
                                                       int? srid)
            : base(dbUtility, 
                   selectStarDelegate, 
                   geometryColumnFormatString, 
                   query, 
                   tableSchema, 
                   tableName, 
                   oidColumnName,
                   geometryColumnName, 
                   srid)
        {
            if (!string.IsNullOrEmpty(spatialSchema))
                SpatialSchema = spatialSchema;

            WithNoLock = withNoLock;
        }

        public bool WithNoLock { get; protected set; }

        /// <summary>
        /// The schema to which the spatial functions belong
        /// e.g default for MsSqlSpatial would be "ST"
        /// </summary>
        public string SpatialSchema
        {
            get { return _spSchema; }
            set { _spSchema = value; }
        }


        /// <remarks>
        /// We are going to create an inner join using a TVF which doesn't require the input geometry to be parsed for each row.
        /// Much Much Much faster than the alternative...
        /// </remarks>
        protected override void WriteSpatialGeometryExpressionSql(StringBuilder builder, SpatialOperation op,
                                                                  IGeometry geom)
        {
            if (geom.IsRectangle)
            {
                WriteSpatialExtentsExpressionSql(builder, op, geom.Extents);
                return;
            }

            string built = builder.ToString();

            if (built.EndsWith(" AND "))
                builder.Remove(builder.Length - 5, 5);
            else if (built.EndsWith(" OR "))
                builder.Remove(builder.Length - 4, 5);


            string join = string.Format(" INNER JOIN {0}.RelateQuery({1},{2},{3},{4}) Tbl_{5} ON {6} = Tbl_{5}.{7}{8}",
                                        SpatialSchema,
                                        CreateParameter(Table).ParameterName,
                                        CreateParameter(GeometryColumn).ParameterName,
                                        CreateParameter(geom).ParameterName,
                                        CreateParameter(Enum.GetName(typeof (SpatialOperation), op)).ParameterName,
                                        TableJoinStrings.Count,
                                        QualifyColumnName(OidColumn),
                                        OidColumn,
                                        WithNoLock ? " WITH(NOLOCK) " : "");

            TableJoinStrings.Add(join);
        }

        protected override void WriteSpatialExtentsExpressionSql(StringBuilder builder,
                                                                 SpatialOperation spatialOperation, IExtents ext)
        {
            var exts = (IExtents2D) ext;
            switch (spatialOperation)
            {
                case SpatialOperation.Intersects:
                    {
                        builder.AppendFormat(
                            " {0}.{1}_Envelope_MinX < {2} AND {0}.{1}_Envelope_MinY < {3} AND {0}.{1}_Envelope_MaxX > {4} AND {0}.{1}_Envelope_MaxY > {5}",
                            Table,
                            GeometryColumn,
                            CreateParameter(exts.XMax).ParameterName,
                            CreateParameter(exts.YMax).ParameterName,
                            CreateParameter(exts.XMin).ParameterName,
                            CreateParameter(exts.YMin).ParameterName
                            );
                        break;
                    }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}