/*
 *  The attached / following is part of SharpMap
 *  SharpMap is free software © 2008 Newgrove Consultants Limited, 
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

namespace SharpMap.Expressions
{
    /// <summary>
    /// An Expression which takes twho GeometryExpressions and computes a third GeometryExpression
    /// defined for : Difference, Intersection, SymDifference and Union
    /// </summary>
    public class SpatialAnalysisGeometricExpression : SpatialAnalysisExpression<GeometryExpression, GeometryExpression>
    {
        public SpatialAnalysisGeometricExpression(GeometryExpression expressionLeft, SpatialAnalysisOperation operation, GeometryExpression expressionRight)
            : base(expressionLeft, operation, expressionRight)
        {
        }

        protected override GeometryExpression GetValue()
        {
            IGeometry a = LeftExpression.Geometry, b = RightExpression.Geometry, c;
            switch (SpatialAnalysisOperator)
            {
                case SpatialAnalysisOperation.Difference:
                    {
                        c = a.Difference(b);
                        break;
                    }
                case SpatialAnalysisOperation.Intersection:
                    {
                        c = a.Intersection(b);
                        break;
                    }
                case SpatialAnalysisOperation.SymDifference:
                    {
                        c = a.SymmetricDifference(b);
                        break;
                    }
                case SpatialAnalysisOperation.Union:
                    {
                        c = a.Union(b);
                        break;
                    }
                default:
                    throw new ArgumentException("This expression works only with the SpatialAnalysisOperations: Difference, Intersection, SymDifference and Union. See other SpatialAnalysisExpressions.");
            }

            return new GeometryExpression(c);
        }

        public override Expression Clone()
        {
            return new SpatialAnalysisGeometricExpression((GeometryExpression)LeftExpression.Clone(), SpatialAnalysisOperator,
                                                          (GeometryExpression)RightExpression.Clone());
        }

        public override bool Contains(Expression other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(Expression other)
        {
            return base.Equals(other as SpatialAnalysisExpression);
        }
    }
}