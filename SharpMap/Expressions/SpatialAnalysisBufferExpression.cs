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

namespace SharpMap.Expressions
{
    /// <summary>
    /// A SpatialAnalysisExpression which returns a GeometryExpression corresponding to buffering the input GeometryExpression by a distance
    /// </summary>
    [Serializable]
    public class SpatialAnalysisBufferExpression
        : SpatialAnalysisExpression<GeometryExpression, LiteralExpression<double>>
    {

        public SpatialAnalysisBufferExpression(GeometryExpression expressionLeft, double bufferDistance)
            : this(expressionLeft, new LiteralExpression<double>(bufferDistance))
        {

        }

        public SpatialAnalysisBufferExpression(GeometryExpression expressionLeft, LiteralExpression<double> bufferExpression)
            : base(expressionLeft, SpatialAnalysisOperation.Buffer, bufferExpression)
        {
        }

        protected override GeometryExpression GetValue()
        {
            return new GeometryExpression(LeftExpression.Geometry.Buffer(RightExpression.Value));
        }

        public override Expression Clone()
        {
            return new SpatialAnalysisBufferExpression((GeometryExpression) LeftExpression.Clone(),
                                                       (LiteralExpression<double>) RightExpression.Clone());
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