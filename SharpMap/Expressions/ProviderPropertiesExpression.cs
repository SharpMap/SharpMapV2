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
using System.Collections.Generic;

namespace SharpMap.Expressions
{
    public class ProviderPropertiesExpression : Expression
    {
        private readonly CollectionExpression<ProviderPropertyExpression> _providerProps;

        public ProviderPropertiesExpression(CollectionExpression<ProviderPropertyExpression> _providerProps)
        {
            this._providerProps = _providerProps;
        }

        public ProviderPropertiesExpression(IEnumerable<ProviderPropertyExpression> properties)
            : this(new CollectionExpression<ProviderPropertyExpression>(properties)) {}

        public CollectionExpression<ProviderPropertyExpression> ProviderProperties
        {
            get { return _providerProps; }
        }


        public override Boolean Contains(Expression other)
        {
            return ReferenceEquals(this, other);
        }

        public override Expression Clone()
        {
            return
                new ProviderPropertiesExpression(
                    (CollectionExpression<ProviderPropertyExpression>) ProviderProperties.Clone());
        }

        public override Boolean Equals(Expression other)
        {
            ProviderPropertiesExpression providerPropertiesExpression = other as ProviderPropertiesExpression;

            if (ReferenceEquals(providerPropertiesExpression, null))
            {
                return false;
            }

            if (ReferenceEquals(providerPropertiesExpression, this))
            {
                return true;
            }

            return ProviderProperties.Equals(providerPropertiesExpression.ProviderProperties);
        }
    }
}