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

namespace SharpMap.Expressions
{
    /// <summary>
    /// An SpatialAnalysisExpression which calculates the distance between two GeometryExpressions
    /// </summary>
    public class SpatialAnalysisDistanceExpression
        : SpatialAnalysisExpression<LiteralExpression<double>, GeometryExpression>
    {
        public SpatialAnalysisDistanceExpression(GeometryExpression expressionLeft, GeometryExpression expressionRight)
            : base(expressionLeft, SpatialAnalysisOperation.Distance, expressionRight)
        {
        }

        protected override LiteralExpression<double> GetValue()
        {
            return new LiteralExpression<double>(LeftExpression.Geometry.Distance(RightExpression.Geometry));
        }

        public override Expression Clone()
        {
            return new SpatialAnalysisDistanceExpression((GeometryExpression) LeftExpression.Clone(),
                                                         (GeometryExpression) RightExpression.Clone());
        }

        public override bool Contains(Expression other)
        {
            throw new System.NotImplementedException();
        }

        public override bool Equals(Expression other)
        {
            return base.Equals(other as SpatialAnalysisExpression);
        }
    }
}