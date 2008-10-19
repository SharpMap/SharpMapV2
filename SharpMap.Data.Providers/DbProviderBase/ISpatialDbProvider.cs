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