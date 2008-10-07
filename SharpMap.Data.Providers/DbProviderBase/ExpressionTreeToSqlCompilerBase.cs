/*
 *  The attached / following is part of SharpMap.Data.Providers.Db
 *  SharpMap.Data.Providers.Db is free software © 2008 Newgrove Consultants Limited, 
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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.Db
{
    public abstract class ExpressionTreeToSqlCompilerBase
    {
        private readonly Expression _expression;
        private readonly Dictionary<object, IDataParameter> _parameterCache = new Dictionary<object, IDataParameter>();
        private readonly List<string> _parameterDeclarations = new List<string>();
        private readonly List<string> _projectedColumns = new List<string>();
        private readonly List<string> _tableJoinStrings = new List<string>();
        private string _sqlColumns;
        private string _sqlJoinClauses;
        private string _sqlParamDeclarations;
        private string _sqlWhereClause;
        private bool built;


        protected ExpressionTreeToSqlCompilerBase(
            IDbUtility dbUtility,
            Func<IEnumerable<string>> selectStarDelegate,
            string geometryColumnFormatString,
            Expression expression,
            string tableSchema,
            string tableName,
            string oidColumnName,
            string geometryColumnName,
            int? srid)
        {
            DbUtility = dbUtility;
            _expression = expression;
            OidColumn = oidColumnName;
            TableSchema = tableSchema;
            GeometryColumn = geometryColumnName;
            Table = tableName;
            Srid = srid;
            SelectStarDelegate = selectStarDelegate;
            GeometryColumnFormatString = geometryColumnFormatString;
        }

        public string SqlWhereClause
        {
            get
            {
                EnsureBuilt();
                return _sqlWhereClause;
            }
        }

        public string SqlParamDeclarations
        {
            get
            {
                EnsureBuilt();
                _sqlParamDeclarations = _sqlParamDeclarations ??
                                        string.Join(" ", Enumerable.ToArray(ParameterDeclarations));
                return _sqlParamDeclarations;
            }
        }

        public string SqlJoinClauses
        {
            get
            {
                EnsureBuilt();
                _sqlJoinClauses = _sqlJoinClauses ?? string.Join(" ", Enumerable.ToArray(TableJoinStrings));
                return _sqlJoinClauses;
            }
        }

        public string SqlColumns
        {
            get
            {
                EnsureBuilt();
                _sqlColumns = _sqlColumns ?? string.Join(", ", Enumerable.ToArray(ProjectedColumns));
                return _sqlColumns;
            }
        }

        public Dictionary<object, IDataParameter> ParameterCache
        {
            get { return _parameterCache; }
        }

        protected IList<string> TableJoinStrings
        {
            get { return _tableJoinStrings; }
        }

        protected IList<string> ParameterDeclarations
        {
            get { return _parameterDeclarations; }
        }

        protected IList<string> ProjectedColumns
        {
            get { return _projectedColumns; }
        }


        public int? Srid { get; set; }


        private IDbUtility DbUtility { get; set; }

        #region database info

        /// <summary>
        /// The Database schema to which the Table belongs
        /// </summary>
        public string TableSchema { get; set; }


        /// <summary>
        /// The name of the table or view we are querying
        /// </summary>
        /// <remarks>TODO: we may be able to get this to work with arbirtary derived views eg:
        /// instance.Table = "(Select tbl2.* from tbl1 inner join tbl2 on tbl1.id = tbl2.id where tbl1.attribute in ('attribute1', 'attribute2') as tbl3" </remarks>
        public string Table { get; set; }

        /// <summary>
        /// The name of the geomery column for the table or view we are querying
        /// </summary>
        public string GeometryColumn { get; set; }

        /// <summary>
        /// The unique id column for the table or view we are querying
        /// </summary>
        public string OidColumn { get; set; }

        public virtual string QualifiedTableName
        {
            get
            {
                GuardValueNotNull(Table, "Table");
                string name = Table;

                if (!string.IsNullOrEmpty(TableSchema))
                    name = string.Format("{0}.{1}", TableSchema, name);

                return name;
            }
        }

        protected Func<IEnumerable<string>> SelectStarDelegate { get; set; }

        public string GeometryColumnFormatString { get; set; }

        protected void GuardValueNotNull<T>(T value, string name)
        {
            if (Equals(value, default(T)) || (typeof(T) == typeof(string)
                                              && string.IsNullOrEmpty(value as string)))
                throw new InvalidOperationException(string.Format("{0} cannot be null.", name));
        }

        public virtual string QualifyColumnName(string columnName)
        {
            GuardValueNotNull(Table, "Table");
            GuardValueNotNull(columnName, "ColumnName");

            return string.Format("{0}.{1}", Table, columnName);
        }

        /// <summary>
        /// Create or retrieves from cache a parameter using the classes supplied parameter prefix.
        /// The name is autogenerated. Use the name property of the returned param in your queries.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual IDataParameter CreateParameter(object value)
        {
            if (ParameterCache.ContainsKey(value))
                return ParameterCache[value];

            object paramValue = value;
            if (value is IGeometry)
                paramValue = ((IGeometry)value).AsBinary();

            IDataParameter p = DbUtility.CreateParameter(string.Format("iparam{0}", ParameterCache.Count),
                                                         paramValue,
                                                         ParameterDirection.Input);

            ParameterCache.Add(value, p);

            return p;
        }

        #endregion

        private void EnsureBuilt()
        {
            if (!built)
            {
                _sqlWhereClause = BuildSql();
                built = true;
            }
        }

        private string BuildSql()
        {
            var sb = new StringBuilder();
            VisitExpression(sb, _expression);
            return sb.ToString();
        }

        protected virtual void VisitExpression(StringBuilder builder, Expression exp)
        {
            if (exp == null)
                return;

            if (exp is ProjectionExpression)
                VisitProjectionExpression((ProjectionExpression)exp);
            else if (exp is SpatialBinaryExpression)
                VisitSpatialBinaryExpression(builder, (SpatialBinaryExpression)exp);
            else if (exp is FeatureQueryExpression)
                VisitFeatureQueryExpression(builder, (FeatureQueryExpression)exp);
            else if (exp is QueryExpression)
                VisitQueryExpression(builder, (QueryExpression)exp);
            else if (exp is CollectionBinaryExpression)
                VisitCollectionBinaryExpression(builder, (CollectionBinaryExpression)exp);
            else if (exp is BinaryExpression)
                VisitBinaryExpression(builder, (BinaryExpression)exp);
            else if (exp is AttributeBinaryStringExpression)
                VisitBinaryStringExpression(builder, (AttributeBinaryStringExpression)exp);
            else if (exp is LiteralExpression)
                VisitValueExpression(builder, (LiteralExpression)exp);
            else if (exp is PropertyNameExpression)
                VisitAttributeExpression(builder, (PropertyNameExpression)exp);
            else if (exp is CollectionExpression)
                VisitCollectionExpression(builder, (CollectionExpression)exp);
            else
                throw new NotImplementedException(string.Format("Unknown Expression Type {0}", exp.GetType()));
        }



        private void VisitQueryExpression(StringBuilder builder, QueryExpression exp)
        {
            if (exp == null)
                return;

            builder.Append("(");
            VisitExpression(builder, exp.Projection);
            VisitExpression(builder, exp.Predicate);
            builder.Append(")");
        }

        protected virtual void VisitProjectionExpression(ProjectionExpression exp)
        {
            if (exp == null)
                return;

            if (exp is AllAttributesExpression)
            {
                foreach (string s in SelectStarDelegate())
                    ProjectedColumns.Add(s);
            }

            else if (exp is AttributesProjectionExpression)
                VisitAttributeProjectionExpression((AttributesProjectionExpression)exp);
        }

        protected virtual void VisitAttributeProjectionExpression(AttributesProjectionExpression exp)
        {
            if (exp == null)
                return;

            foreach (PropertyNameExpression pn in exp.Attributes.Collection)
            {
                ProjectedColumns.Add(
                    string.Compare(pn.PropertyName, GeometryColumn, StringComparison.InvariantCultureIgnoreCase) == 0
                        ? string.Format(GeometryColumnFormatString, pn.PropertyName)
                        : pn.PropertyName);
            }
        }

        private IEnumerable<string> EnumerableConverter(IEnumerable enu)
        {
            foreach (object o in enu)
                yield return DbUtility.FormatValue(o);
        }

        protected virtual void VisitCollectionExpression(StringBuilder builder, CollectionExpression exp)
        {
            if (exp == null)
                return;

            var sb = new StringBuilder();

            sb.Append(string.Join(", ",
                                  Enumerable.ToArray(EnumerableConverter(exp.Collection))));

            if (sb.Length > 0)
                builder.AppendFormat("({0})", sb);
        }

        protected virtual void VisitCollectionBinaryExpression(StringBuilder builder, CollectionBinaryExpression exp)
        {
            if (exp == null)
                return;

            var sb = new StringBuilder();
            VisitExpression(sb, exp.Left);
            sb.Append(GetCollectionExpressionString(exp.Op));
            VisitExpression(sb, exp.Right);

            if (sb.Length > 0)
                builder.AppendFormat("({0})", sb);
        }

        private string GetCollectionExpressionString(CollectionOperator op)
        {
            switch (op)
            {
                case CollectionOperator.In:
                    return " IN ";
                default:
                    throw new NotImplementedException();
            }
        }

        protected virtual void VisitAttributeExpression(StringBuilder builder, PropertyNameExpression exp)
        {
            if (exp == null)
                return;

            builder.Append(
                string.Compare(GeometryColumn, exp.PropertyName, StringComparison.InvariantCultureIgnoreCase) == 0
                    ? string.Format(GeometryColumnFormatString, QualifyColumnName(exp.PropertyName))
                    : QualifyColumnName(exp.PropertyName));
        }


        protected virtual void VisitValueExpression(StringBuilder builder, LiteralExpression exp)
        {
            if (exp == null)
                return;

            builder.AppendFormat(CreateParameter(exp.Value).ParameterName);
        }


        protected virtual void VisitFeatureQueryExpression(StringBuilder builder, FeatureQueryExpression exp)
        {
            if (exp == null)
                return;

            var sb = new StringBuilder();

            if (exp.AttributePredicate != null)
                VisitExpression(sb, exp.AttributePredicate);

            if (exp.OidPredicate != null)
            {
                if (sb.Length > 0)
                    sb.Append(" AND ");
                VisitExpression(sb, exp.OidPredicate);
            }

            if (exp.SpatialPredicate != null)
            {
                if (sb.Length > 0)
                    sb.Append(" AND ");
                VisitExpression(sb, exp.SpatialPredicate);
            }
            if (sb.Length > 0)
                builder.AppendFormat("({0})", sb);
        }

        protected virtual void VisitSpatialBinaryExpression(StringBuilder builder, SpatialBinaryExpression exp)
        {
            if (exp == null)
                return;

            if (exp.SpatialExpression is ExtentsExpression)
                WriteSpatialExtentsExpressionSql(builder, exp.Op, (exp.SpatialExpression).Extents);
            else if (exp.SpatialExpression is GeometryExpression)
                WriteSpatialGeometryExpressionSql(builder, exp.Op, ((GeometryExpression)exp.SpatialExpression).Geometry);
            else
                throw new NotImplementedException(string.Format("{0} is not implemented", exp.GetType()));
        }


        protected virtual void VisitBinaryExpression(StringBuilder builder, BinaryExpression exp)
        {
            if (exp == null)
                return;

            var sb = new StringBuilder();

            VisitExpression(sb, exp.Left);

            if (sb.Length > 0)
                sb.Append(GetBinaryExpressionString(exp.Op));

            VisitExpression(sb, exp.Right);

            if (sb.Length > 0)
                builder.AppendFormat("({0})", sb);
        }

        protected virtual void VisitBinaryStringExpression(StringBuilder builder, AttributeBinaryStringExpression exp)
        {
            if (exp == null)
                return;

            var sb = new StringBuilder();
            VisitExpression(sb, exp.Left);

            if (sb.Length > 0)
                sb.Append(GetBinaryStringExpressionString(exp.Op));

            VisitStringLiteralExpression(sb, exp.Op, exp.Right);

            if (sb.Length > 0)
                builder.AppendFormat("({0})", sb);

        }

        private void VisitStringLiteralExpression(StringBuilder sb, BinaryStringOperator binaryStringOperator, LiteralExpression<string> expression)
        {
            string v;
            switch (binaryStringOperator)
            {

                case BinaryStringOperator.StartsWith:
                    v = expression.Value + "%";
                    break;
                case BinaryStringOperator.EndsWith:
                    v = "%" + expression.Value;
                    break;
                case BinaryStringOperator.Contains:
                    v = "%" + expression.Value + "%";
                    break;
                case BinaryStringOperator.Equals:
                case BinaryStringOperator.NotEquals:
                default:
                    v = expression.Value;
                    break;
            }

            sb.Append(CreateParameter(v).ParameterName);
        }

        protected virtual string GetBinaryStringExpressionString(BinaryStringOperator binaryStringOperator)
        {
            switch (binaryStringOperator)
            {
                case BinaryStringOperator.Equals:
                    return " = ";
                case BinaryStringOperator.NotEquals:
                    return " <> ";
                default:
                    return " LIKE ";
            }
        }

        protected virtual string GetBinaryExpressionString(BinaryOperator binaryOperator)
        {
            switch (binaryOperator)
            {
                case BinaryOperator.And:
                    return " AND ";
                case BinaryOperator.Equals:
                    return " = ";
                case BinaryOperator.GreaterThan:
                    return " > ";
                case BinaryOperator.GreaterThanOrEqualTo:
                    return " >= ";
                case BinaryOperator.LessThan:
                    return " < ";
                case BinaryOperator.LessThanOrEqualTo:
                    return " <= ";
                case BinaryOperator.Like:
                    return " LIKE ";
                case BinaryOperator.NotEquals:
                    return " <> ";
                case BinaryOperator.Or:
                    return " OR ";
                default:
                    throw new ArgumentException("Unknown binary operator");
            }
        }

        protected abstract void WriteSpatialExtentsExpressionSql(StringBuilder builder,
                                                                 SpatialOperation spatialOperation, IExtents ext);

        protected abstract void WriteSpatialGeometryExpressionSql(StringBuilder builder, SpatialOperation op,
                                                                  IGeometry geom);
    }
}