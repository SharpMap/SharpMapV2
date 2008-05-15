using System;
using SharpMap.Expressions;

namespace SharpMap.Data
{
    public interface IAsyncProvider : IProvider
    {
        IAsyncResult BeginExecuteQuery(Expression query, AsyncCallback callback);
        Object EndExecuteQuery(IAsyncResult asyncResult);
    }
}
