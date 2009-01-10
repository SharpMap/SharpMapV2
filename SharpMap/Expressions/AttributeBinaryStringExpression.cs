/*
 *  The attached / following is part of SharpMap
 *  this file © 2008 Newgrove Consultants Limited, 
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
    public class AttributeBinaryStringExpression : BinaryLogicExpressionBase<BinaryStringOperator>
    {
        protected AttributeBinaryStringExpression(PropertyNameExpression left,
                                                  BinaryStringOperator op,
                                                  StringExpression right)
            : base(left, op, right) {}

        public AttributeBinaryStringExpression(PropertyNameExpression left, BinaryStringOperator op, String value)
            : this(left, op, new StringExpression(value)) {}

        public AttributeBinaryStringExpression(String propertyName, BinaryStringOperator op, String value)
            : this(new PropertyNameExpression(propertyName), op, new StringExpression(value)) {}

        public AttributeBinaryStringExpression(String propertyName,
                                               BinaryStringOperator op,
                                               String value,
                                               StringComparison comparison)
            : this(new PropertyNameExpression(propertyName), op, new StringExpression(value, comparison)) {}

        public AttributeBinaryStringExpression(PropertyNameExpression left,
                                               BinaryStringOperator op,
                                               String value,
                                               StringComparison comparison)
            : this(left, op, new StringExpression(value, comparison)) {}


        protected override BinaryLogicExpressionBase<BinaryStringOperator> Create(Expression left,
                                                                             BinaryStringOperator op,
                                                                             Expression right)
        {
            return new AttributeBinaryStringExpression((PropertyNameExpression) left, op, (StringExpression) right);
        }

        public new PropertyNameExpression Left
        {
            get { return (PropertyNameExpression) base.Left; }
        }

        public new StringExpression Right
        {
            get { return (StringExpression) base.Right; }
        }
    }
}