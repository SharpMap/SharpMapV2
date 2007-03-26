using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Rendering;

namespace SharpMap.Styles
{
    [Serializable]
    public class StylePen
    {
        private float _miterLimit;
        private StyleBrush _backgroundBrush;
        private float _dashOffset;
        private LineDashCap _dashCap;
        private LineDashStyle _dashStyle;
        private float[] _dashPattern;
        private StyleBrush[] _dashBrushes;
        private StyleLineCap _startCap;
        private StyleLineCap _endCap;
        private StyleLineJoin _lineJoin;
        private ViewMatrix2D _transform;
        private float _width;
        private float[] _compoundArray;
        private StylePenAlignment _alignment;

        public StylePen(StyleColor color, float width)
            : this(new SolidStyleBrush(color), width) { }

        public StylePen(StyleBrush backgroundBrush, float width)
        {
            _backgroundBrush = backgroundBrush;
            _width = width;
        }

        public StylePenAlignment Alignment
        {
            get { return _alignment; }
            set { _alignment = value; }
        }

        public float[] CompoundArray
        {
            get { return _compoundArray; }
            set { _compoundArray = value; }
        }

        public float MiterLimit
        {
            get { return _miterLimit; }
            set { _miterLimit = value; }
        }

        public StyleBrush BackgroundBrush
        {
            get { return _backgroundBrush; }
            set { _backgroundBrush = value; }
        }

        public float DashOffset
        {
            get { return _dashOffset; }
            set { _dashOffset = value; }
        }

        public float[] DashPattern
        {
            get { return _dashPattern; }
            set { _dashPattern = value; }
        }

        public StyleBrush[] DashBrushes
        {
            get { return _dashBrushes; }
            set { _dashBrushes = value; }
        }

        public LineDashStyle DashStyle
        {
            get { return _dashStyle; }
            set { _dashStyle = value; }
        }

        public StyleLineCap StartCap
        {
            get { return _startCap; }
            set { _startCap = value; }
        }

        public StyleLineCap EndCap
        {
            get { return _endCap; }
            set { _endCap = value; }
        }

        public LineDashCap DashCap
        {
            get { return _dashCap; }
            set { _dashCap = value; }
        }

        public StyleLineJoin LineJoin
        {
            get { return _lineJoin; }
            set { _lineJoin = value; }
        }

        public ViewMatrix2D Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }

        public float Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public override string ToString()
        {
            return String.Format(
                "StylePen - Width: {0}; Background: {1}; Alignment: {2}; CompoundArray: {3}; MiterLimit: {4}; DashOffset: {5}; DashPattern: {6}; DashBrushes: {7}; DashStyle: {8}; StartCap: {9}; EndCap: {10}; DashCap: {11}; LineJoin: {12}; Transform: {13};", 
                Width, BackgroundBrush.ToString(), Alignment, printFloatArray(CompoundArray), MiterLimit, DashOffset, DashPattern, printBrushes(DashBrushes), DashStyle, StartCap, EndCap, DashCap, LineJoin, Transform);
        }

        private string printBrushes(StyleBrush[] brushes)
        {
            if (brushes == null || brushes.Length == 0)
                return String.Empty;

            StringBuilder buffer = new StringBuilder();

            foreach (StyleBrush brush in brushes)
            {
                buffer.Append(brush.ToString());
                buffer.Append(", ");
            }

            buffer.Length -= 2;
            return buffer.ToString();
        }

        private string printFloatArray(float[] values)
        {
            if (values == null || values.Length == 0)
                return String.Empty;

            StringBuilder buffer = new StringBuilder();

            foreach (float value in values)
            {
                buffer.AppendFormat("N3", value);
                buffer.Append(", ");
            }

            buffer.Length -= 2;
            return buffer.ToString();
        }
    }
}
