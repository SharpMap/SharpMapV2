/*
 *  The attached / following is part of SharpMap.Data.Providers.MsSqlServer2008
 *  SharpMap.Data.Providers.MsSqlServer2008 is free software © 2008 Newgrove Consultants Limited, 
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
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Db;
using SharpMap.Data.Providers.MsSqlServer2008;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers
{
    public class MsSqlServer2008Provider<TOid>
        : SpatialDbProviderBase<TOid>
    {
        public MsSqlServer2008Provider(IGeometryFactory geometryFactory, 
                                       String connectionString, 
                                       String tableName)
            : this(geometryFactory, connectionString, null, tableName, null, null) { }

        public MsSqlServer2008Provider(IGeometryFactory geometryFactory, 
                                       String connectionString, 
                                       String tableSchema,
                                       String tableName, 
                                       String oidColumn, 
                                       String geometryColumn)
            : base(new SqlServerDbUtility(), 
                   geometryFactory, 
                   connectionString, 
                   tableSchema, 
                   tableName, 
                   oidColumn,
                   geometryColumn) { }


        public override String GeometryColumnConversionFormatString
        {
            get { return "{0}.STAsBinary()"; }
        }

        public override IExtents GetExtents()
        {
            return GeometryFactory.CreateExtents();

            // TODO: There doesnt seem to be any way of getting bounding box coords easily without db clr or sql/mssqlspatial...
        }

        protected override ExpressionTreeToSqlCompilerBase CreateSqlCompiler(Expression expression)
        {
            return new MsSqlServer2008ExpressionTreeToSqlCompiler(DbUtility, SelectAllColumnNames,
                                                                  GeometryColumnConversionFormatString, expression,
                                                                  TableSchema, Table, OidColumn,
                                                                  GeometryColumn, Srid);
        }
    }
}
