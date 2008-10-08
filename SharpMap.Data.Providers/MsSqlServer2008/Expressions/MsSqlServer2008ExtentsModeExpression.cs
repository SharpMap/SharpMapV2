/*
 *  The attached / following is part of SharpMap.Data.Providers.MsSqlServer2008
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
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.MsSqlServer2008.Expressions
{
    public class MsSqlServer2008ExtentsModeExpression
        : ProviderPropertyExpression<SqlServer2008ExtentsMode>
    {
        public MsSqlServer2008ExtentsModeExpression(SqlServer2008ExtentsMode value)
            : base("SqlServer2008ExtentsMode", value)
        {
        }

        public override Expression Clone()
        {
            return new MsSqlServer2008ExtentsModeExpression(PropertyValueExpression.Value);
        }
    }
}