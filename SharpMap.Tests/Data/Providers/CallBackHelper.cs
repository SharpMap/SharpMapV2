using System;

namespace SharpMap.Tests.Data.Providers
{
    internal class CallBackHelper
    {
        private IAsyncResult _result;

        public void CallBack(IAsyncResult result)
        {
            _result = result;
        }

        public IAsyncResult Result
        {
            get { return _result; }
        }
    }
}
