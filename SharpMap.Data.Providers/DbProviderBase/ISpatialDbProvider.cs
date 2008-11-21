/*
 *  The attached / following is part of SharpMap.Data.Providers.Db
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
using System;
using SharpMap.Expressions;

namespace SharpMap.Data.Providers.Db
{
    public interface ISpatialDbProvider : IFeatureProvider
    {
        String ConnectionString { get; set; }
        String Table { get; set; }
        String TableSchema { get; set; }
        String GeometryColumn { get; set; }

        /// <summary>
        /// Name of column that contains the Object ID.
        /// </summary>
        String OidColumn { get; set; }

        /// <summary>
        /// Definition query used for limiting dataset (WHERE clause)
        /// </summary>
        PredicateExpression DefinitionQuery { get; set; }

        IDbUtility DbUtility { get; }
        String GeometryColumnConversionFormatString { get; }
        String GeomFromWkbFormatString { get; }
        String QualifiedTableName { get; }
        ProviderPropertiesExpression DefaultProviderProperties { get; set; }
    }

    public interface ISpatialDbProvider<TDbUtility> where TDbUtility : IDbUtility, new()
    {
        TDbUtility DbUtility { get; }
    }
}