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
    /// A SpatialAnalysisExpression which returns a GeometryExpression corresponding to the ConvexHull of the input GeometryExpression
    /// </summary>
    [Serializable]
    public class SpatialAnalysisConvexHullExpression
        : SpatialAnalysisExpression<GeometryExpression, NullExpression>
    {
        public SpatialAnalysisConvexHullExpression(GeometryExpression expressionLeft)
            : base(expressionLeft, SpatialAnalysisOperation.ConvexHull, new NullExpression())
        {
        }

        protected override GeometryExpression GetValue()
        {
            return new GeometryExpression(LeftExpression.Geometry.ConvexHull());
        }

        public override Expression Clone()
        {
            return new SpatialAnalysisConvexHullExpression((GeometryExpression)LeftExpression.Clone());
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