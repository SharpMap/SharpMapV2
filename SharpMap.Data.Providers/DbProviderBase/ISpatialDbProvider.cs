using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Data.Providers.Db
{
    public interface ISpatialDbProvider
    {
        IDbUtility DbUtility { get; }
    }

    public interface ISpatialDbProvider<TDbUtility> 
        : ISpatialDbProvider where TDbUtility : IDbUtility
    {
        new TDbUtility DbUtility { get; }
    }
}
