using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Rendering;

namespace SharpMap.Styles
{
    public class LinearGradientStyleBrush : StyleBrush
    {
        private ColorBlend _colorBlend;
        private StyleColor _startColor = StyleColor.Black;
        private StyleColor _endColor = StyleColor.White;
        private IViewMatrix _transform;

        public ColorBlend ColorBlend
        {
            get { return _colorBlend; }
            set { _colorBlend = value; }
        }

        public StyleColor StartColor
        {
            get { return _startColor; }
            set { _startColor = value; }
        }

        public StyleColor EndColor
        {
            get { return _endColor; }
            set { _endColor = value; }
        }

        public IViewMatrix Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        public override string ToString()
        {
            return String.Format("LinearGradientStyleBrush - Start: {0}; End: {1}", StartColor, EndColor);
        }
    }
}
