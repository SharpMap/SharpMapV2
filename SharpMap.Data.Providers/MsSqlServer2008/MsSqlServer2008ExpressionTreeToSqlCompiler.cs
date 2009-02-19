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

        protected override void WriteSpatialGeometryExpressionSqlInternal(StringBuilder builder,
                                                                  SpatialOperation op,
                                                                  IGeometry geom)
        {

            int? geomSrid = SridMap.DefaultInstance.Process(geom.SpatialReference, (int?)null);
            int? provSrid = SridMap.DefaultInstance.Process(Provider.SpatialReference, (int?)null);

            string declaredParamName = string.Format("@dparam_{0}", ParameterDeclarations.Count);
            ParameterDeclarations.Add(
                string.Format("DECLARE {0} geometry\n SET {0} = geometry::STGeomFromWKB({1},{2}).MakeValid()\n ",
                              declaredParamName,
                              CreateParameter(geom).ParameterName,
                             geomSrid.HasValue && geomSrid.Value > 0
                                  ? geomSrid.Value
                                  : !provSrid.HasValue || provSrid.Value < 0
                                        ? 0
                                        : provSrid.Value));

            builder.AppendFormat(" {0}.{1}.{2}({3}) = 1 ",
                                 Provider.Table,
                                 Provider.GeometryColumn,
                                 GetSpatialMethodName(op),
                                 declaredParamName);
        }

        private static string GetSpatialMethodName(SpatialOperation op)
        {
            return string.Format("ST{0}", Enum.GetName(typeof(SpatialOperation), op));
        }

        protected override void WriteSpatialExtentsExpressionSqlInternal(StringBuilder builder,
                                                                 SpatialOperation spatialOperation, IExtents ext)
        {
            /// seems faster to test the actual geometry and take advantage of spatial indexing
            /// than to test the envelope without spatial indexing.
            WriteSpatialGeometryExpressionSqlInternal(builder, spatialOperation, ext.ToGeometry());
        }
    }
}