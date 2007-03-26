using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Presentation
{
    [Serializable]
    public class MapPresentationPropertyChangedEventArgs<TParam> : EventArgs
    {
        private TParam _previousValue;
        private TParam _currentValue;

        public MapPresentationPropertyChangedEventArgs(TParam previousValue, TParam currentValue)
        {
            _previousValue = previousValue;
            _currentValue = currentValue;
        }

        public TParam PreviousValue
        {
            get { return _previousValue; }
        }

        public TParam CurrentValue
        {
            get { return _currentValue; }
        }
    }
}
