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
    public abstract class ProviderPropertyExpression : Expression
    {
        private readonly PropertyNameExpression _propertyNameExpression;
        private readonly LiteralExpression _propertyValueExpression;

        protected ProviderPropertyExpression(PropertyNameExpression nameExpression,
                                             LiteralExpression propertyValueExpression)
        {
            _propertyNameExpression = nameExpression;
            _propertyValueExpression = propertyValueExpression;
        }

        public PropertyNameExpression PropertyNameExpression
        {
            get { return _propertyNameExpression; }
        }

        public LiteralExpression PropertyValueExpression
        {
            get { return _propertyValueExpression; }
        }
    }

    public abstract class ProviderPropertyExpression<TValue> : ProviderPropertyExpression
    {
        public ProviderPropertyExpression(PropertyNameExpression propertyNameExpression,
                                          LiteralExpression<TValue> value)
            : base(propertyNameExpression, value) {}

        public ProviderPropertyExpression(String propertyName,
                                          TValue value)
            : this(new PropertyNameExpression(propertyName), new LiteralExpression<TValue>(value)) {}


        public new LiteralExpression<TValue> PropertyValueExpression
        {
            get { return (LiteralExpression<TValue>) base.PropertyValueExpression; }
        }

        public override Boolean Equals(Expression other)
        {
            var pp2 = other as ProviderPropertyExpression<TValue>;

            return !Equals(null, pp2)
                   && Equals(pp2.PropertyNameExpression, PropertyNameExpression)
                   && Equals(pp2.PropertyValueExpression, PropertyValueExpression);
        }

        public override Boolean Contains(Expression other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            throw new NotImplementedException();
        }
    }
}