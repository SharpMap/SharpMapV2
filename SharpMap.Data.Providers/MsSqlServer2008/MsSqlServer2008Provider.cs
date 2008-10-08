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
using System.Data;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Db;
using SharpMap.Data.Providers.MsSqlServer2008;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers
{
    public enum SqlServer2008ExtentsMode
    {
        /// <summary>
        /// Requires no additional components but can be very slow for large datasets
        /// </summary>
        QueryIndividualFeatures = 0,
        /// <summary>
        /// Requires SqlSpatialTools be installed on the db server
        /// </summary>
        UseSqlSpatialTools = 1,

        /// <summary>
        /// Requires no additional components but does require additional columns in the form of [GeomColumnName]_Envelope_MinX, [GeomColumnName]_Envelope_MinY, [GeomColumnName]_Envelope_MaxX, [GeomColumnName]_Envelope_MaxY
        /// Initial tests seem to show this is the fastest.
        /// </summary>
        UseEnvelopeColumns = 2
    }

    public class MsSqlServer2008Provider<TOid>
        : SpatialDbProviderBase<TOid>
    {
        #region Delegates

        public delegate string IndexNameGenerator(MsSqlServer2008Provider<TOid> provider);

        #endregion

        private SqlServer2008ExtentsMode _sqlServer2008ExtentsMode = SqlServer2008ExtentsMode.QueryIndividualFeatures;

        private IndexNameGenerator _indexNameGenerator = o => string.Format("sidx_{0}_{1}", o.Table,
                                                                            o.GeometryColumn);

        public MsSqlServer2008Provider(IGeometryFactory geometryFactory,
                                       String connectionString,
                                       String tableName)
            : this(
                geometryFactory, connectionString, null, tableName, null, null, false,
                SqlServer2008ExtentsMode.QueryIndividualFeatures)
        {
        }

        public MsSqlServer2008Provider(IGeometryFactory geometryFactory,
                                       String connectionString,
                                       String tableSchema,
                                       String tableName,
                                       String oidColumn,
                                       String geometryColumn, Boolean withNoLock, SqlServer2008ExtentsMode sqlServer2008ExtentsCalculationMode)
            : base(new SqlServerDbUtility(),
                   geometryFactory,
                   connectionString,
                   tableSchema,
                   tableName,
                   oidColumn,
                   geometryColumn)
        {
            WithNoLock = withNoLock;
            SqlServer2008ExtentsCalculationMode = sqlServer2008ExtentsCalculationMode;
        }

        public SqlServer2008ExtentsMode SqlServer2008ExtentsCalculationMode
        {
            get { return _sqlServer2008ExtentsMode; }
            set { _sqlServer2008ExtentsMode = value; }
        }

        /// <summary>
        /// a delegate which takes in this provider and uses it to create the index name. (or a comma seperated list of index names)
        /// The default is sidx_[tablename]_[geometrycolumnname]
        /// use in conjunction with ForceIndex. Note: the index does not need to be a spatial one but must exist on the table. 
        /// </summary>
        public IndexNameGenerator IndexName
        {
            get { return _indexNameGenerator; }
            set { _indexNameGenerator = value; }
        }

        /// <summary>
        /// Set this to true to force Sql Server to use an index. Often it will not by default. 
        /// </summary>
        public bool ForceIndex { get; set; }

        public bool WithNoLock { get; set; }


        public override String GeometryColumnConversionFormatString
        {
            get { return "{0}.STAsBinary()"; }
        }

        public override string GeomFromWkbFormatString
        {
            get { throw new NotImplementedException(); }
        }

        public override IExtents GetExtents()
        {
            using (IDbConnection conn = DbUtility.CreateConnection(ConnectionString))
            using (IDbCommand cmd = DbUtility.CreateCommand())
            {
                cmd.Connection = conn;
                switch (SqlServer2008ExtentsCalculationMode)
                {
                    case SqlServer2008ExtentsMode.UseSqlSpatialTools:
                        {
                            cmd.CommandText =
                                string.Format(
                                    @"
    declare @envelope Geometry
    select @envelope = dbo.GeometryEnvelopeAggregate({0}) from {1}.{2} {3}
    select 
        @envelope.STPointN(2).STX as MinX, 
        @envelope.STPointN(2).STY as MinY, 
        @envelope.STPointN(4).STX as MaxX, 
        @envelope.STPointN(4).STY as MaxY",
                                   GeometryColumn , TableSchema, Table, WithNoLock ? " WITH(NOLOCK) " : "");
                            break;
                        }
                    case SqlServer2008ExtentsMode.UseEnvelopeColumns:
                        {
                            cmd.CommandText = string.Format(
                                "SELECT MIN({0}_Envelope_MinX), MIN({0}_Envelope_MinY), MAX({0}_Envelope_MaxX), MAX({0}_Envelope_MaxY) FROM {1}.{2} {3}",
                                GeometryColumn, TableSchema, Table, WithNoLock ? " WITH(NOLOCK) " : "");
                            break;
                        }
                    default:
                        {
                            cmd.CommandText =
                                string.Format(
                                    @"
    select 
	    Min(Geom.STEnvelope().STPointN(1).STX)as MinX, 
	    Min(Geom.STEnvelope().STPointN(1).STY) as MinY,  
	    Max(Geom.STEnvelope().STPointN(3).STX) as MaxX, 
	    Max(Geom.STEnvelope().STPointN(3).STY) as MaxY from {0}.{1} {2}",
                                    TableSchema, Table, WithNoLock ? " WITH(NOLOCK) " : "");
                            break;
                        }
                }

                cmd.CommandType = CommandType.Text;
                double xmin, ymin, xmax, ymax;
                conn.Open();
                using (IDataReader r = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (r.Read())
                    {
                        xmin = r.GetDouble(0);
                        ymin = r.GetDouble(1);
                        xmax = r.GetDouble(2);
                        ymax = r.GetDouble(3);
                        return GeometryFactory.CreateExtents2D(xmin, ymin, xmax, ymax);
                    }
                }
            }

            return GeometryFactory.CreateExtents();
        }

        protected override ExpressionTreeToSqlCompilerBase CreateSqlCompiler(Expression expression)
        {
            return new MsSqlServer2008ExpressionTreeToSqlCompiler(DbUtility, SelectAllColumnNames,
                                                                  GeometryColumnConversionFormatString, expression,
                                                                  TableSchema, Table, OidColumn,
                                                                  GeometryColumn, Srid);
        }


        protected override IDbCommand PrepareCommand(Expression query)
        {
            Expression exp = query;

            if (DefinitionQuery != null)
                exp = new BinaryExpression(query, BinaryOperator.And, DefinitionQuery);

            ExpressionTreeToSqlCompilerBase compiler = CreateSqlCompiler(exp);

            string sql = string.Format(" {0} SELECT {1}  FROM {2}{6} {3} {4} {5}",
                                       compiler.SqlParamDeclarations,
                                       string.IsNullOrEmpty(compiler.SqlColumns)
                                           ? string.Join(",", Enumerable.ToArray(SelectAllColumnNames()))
                                           : compiler.SqlColumns,
                                       compiler.QualifiedTableName,
                                       compiler.SqlJoinClauses,
                                       string.IsNullOrEmpty(compiler.SqlWhereClause) ? "" : " WHERE ",
                                       compiler.SqlWhereClause,
                                       GetWithClause());

            IDbCommand cmd = DbUtility.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;

            foreach (IDataParameter p in compiler.ParameterCache.Values)
                cmd.Parameters.Add(p);

            return cmd;
        }

        protected string GetWithClause()
        {
            if (!WithNoLock && !ForceIndex)
                return "";

            if (WithNoLock && !ForceIndex)
                return " WITH(NOLOCK) ";
            if (ForceIndex && !WithNoLock)
                return string.Format(" WITH(INDEX({0})) ", IndexName(this));

            return string.Format(" WITH(NOLOCK,INDEX({0})) ", IndexName(this));
        }

        public override DataTable GetSchemaTable()
        {
            DataTable dt = base.GetSchemaTable(true);
            dt.Columns[GeometryColumn].DataType = typeof (byte[]);
                //the natural return type is the native sql Geometry we need to override this to avoid a schema merge exception
            return dt;
        }
    }
}