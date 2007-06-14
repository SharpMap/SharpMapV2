using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Presentation
{
    public abstract class BasePresenter<TView> : IDisposable
    {
        private SharpMap.Map.Map _map;
        private TView _view;

        protected BasePresenter(SharpMap.Map.Map map, TView view)
        {
        }

        public SharpMap.Map.Map Map
        {
            get { return _map; }
        }

        public TView View
        {
            get { return _view; }
            protected set { _view = value; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
