using System;
using SharpMap.Expressions;

namespace SharpMap.Layers
{
    public class LayerDataLoadedEventArgs : EventArgs
    {
        private readonly Expression _expression;
        private readonly Object _result;

        public LayerDataLoadedEventArgs(Expression expression, Object result)
        {
            _expression = expression;
            _result = result;
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
