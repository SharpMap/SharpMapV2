using System;
using SharpMap.Expressions;

namespace SharpMap.Data
{
    public interface IAsyncProvider : IProvider
    {
        IAsyncResult BeginExecuteQuery(Expression query, AsyncCallback callback);
        // DESIGN_NOTE: should this be an IEnumerable return?
        Object EndExecuteQuery(IAsyncResult asyncResult);
    }
}
