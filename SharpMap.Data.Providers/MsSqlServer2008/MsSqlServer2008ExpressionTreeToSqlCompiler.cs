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
using System.Text;
using GeoAPI.Geometries;
using SharpMap.Data.Providers.Db;
using SharpMap.Expressions;
using SharpMap.Utilities.SridUtility;

namespace SharpMap.Data.Providers.MsSqlServer2008
{
    public class MsSqlServer2008ExpressionTreeToSqlCompiler<TOId>
        : ExpressionTreeToSqlCompilerBase<TOId>
    {
        public MsSqlServer2008ExpressionTreeToSqlCompiler(MsSqlServer2008Provider<TOId> provider, Expression exp)
            : base(provider, exp)
        {
        }

        protected string DeclareSqlGeometry(IGeometry geometry)
        {
            int? geomSrid = SridMap.DefaultInstance.Process(geometry.SpatialReference, (int?)null);
            int? provSrid = SridMap.DefaultInstance.Process(Provider.SpatialReference, (int?)null);

            string declaredParamName = string.Format("@dparam_{0}", ParameterDeclarations.Count);
            ParameterDeclarations.Add(
                string.Format("DECLARE {0} geometry\n SET {0} = geometry::STGeomFromWKB({1},{2}).MakeValid()\n ",
                              declaredParamName,
                              CreateParameter(geometry).ParameterName,
                             geomSrid.HasValue && geomSrid.Value > 0
                                  ? geomSrid.Value
                                  : !provSrid.HasValue || provSrid.Value < 0
                                        ? 0
                                        : provSrid.Value));

            return declaredParamName;
        }


        protected override void WriteSpatialGeometryExpressionSqlInternal(StringBuilder builder,
                                                                  SpatialOperation op,
                                                                  IGeometry geom)
        {
            builder.AppendFormat(" {0}.{1}.{2}({3}) = 1 ",
                                 Provider.Table,
                                 GetGeometryColumn(),
                                 GetSpatialMethodName(op),
                                 DeclareSqlGeometry(geom));
        }

        private static string GetSpatialMethodName<TEnum>(TEnum op)
        {
            return string.Format("ST{0}", Enum.GetName(typeof(TEnum), op));
        }

        protected override void WriteSpatialExtentsExpressionSqlInternal(StringBuilder builder,
                                                                 SpatialOperation spatialOperation, IExtents ext)
        {
            /// seems faster to test the actual geometry and take advantage of spatial indexing
            /// than to test the envelope without spatial indexing.
            WriteSpatialGeometryExpressionSqlInternal(builder, spatialOperation, ext.ToGeometry());
        }

        protected override void VisitSpatialAnalysisExpressionInternal(StringBuilder builder, SpatialAnalysisExpression expression)
        {
            string paramName = expression.RightExpression is GeometryExpression
                                   ? DeclareSqlGeometry(TransformGeometry((expression.RightExpression as GeometryExpression).Geometry))
                                   :
                                       expression.RightExpression is LiteralExpression
                                           ? CreateParameterFromObject(
                                                 ((LiteralExpression)expression.RightExpression).Value).ParameterName
                                           : string.Empty;

            builder.AppendFormat(" {0}.{1}.{2}({3}) ",
                      Provider.Table,
                      GetGeometryColumn(),
                      GetSpatialMethodName(expression.SpatialAnalysisOperator),
                      paramName);
        }

        private string GetGeometryColumn() 
        {
            MsSqlServer2008Provider<TOId> provider = (MsSqlServer2008Provider<TOId>)Provider;
            return provider.ValidatesGeometry ? String.Format("{0}.MakeValid()", provider.GeometryColumn) : provider.GeometryColumn;
        }
    }
}