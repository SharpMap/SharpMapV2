using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Styles
{
    [Serializable]
    public abstract class StyleBrush
    {
        private StyleColor _color = StyleColor.Transparent;

        public StyleBrush() { }

        public StyleBrush(StyleColor color)
        {
            _color = color;
        }

        public StyleColor Color
        {
            get { return _color; }
            set { _color = value; }
        }
    }
}
