using System;
using SharpMap.Expressions;

namespace SharpMap.Layers
{
    public class LayerDataLoadedEventArgs : EventArgs
    {
        private readonly Expression _expression;
        private readonly Object _result;
        private readonly ILayer _layer;

        public LayerDataLoadedEventArgs(ILayer layer, Expression expression, Object result)
        {
            _expression = expression;
            _layer = layer;
            _result = result;
        }

        public ILayer Layer
        {
            get { return _layer; }
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public Object Result
        {
            get { return _result; }
        }
    }
}
