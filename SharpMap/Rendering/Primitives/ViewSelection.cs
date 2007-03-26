using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Styles;

namespace SharpMap.Rendering
{
    public abstract class ViewSelection<TViewPoint, TViewSize, TViewRegion> : IViewSelection<TViewPoint, TViewSize, TViewRegion>
        where TViewPoint : IViewVector
        where TViewSize : IViewVector
        where TViewRegion : IViewMatrix, new()
    {
        private GraphicsPath<TViewPoint, TViewRegion> _path;
        private StylePen _outline;
        private StyleColor _fill;
        private TViewPoint _anchorPoint;
        private TViewRegion _boundingRegion = new TViewRegion();

        public static readonly StylePen DefaultOutline;
        public static StyleColor DefaultFill = new StyleColor(255, 32, 32, 32);

        static ViewSelection()
        {
            DefaultOutline = new StylePen(StyleColor.Black, 1.0f);
            DefaultOutline.DashStyle = LineDashStyle.Dash;
            DefaultOutline.DashBrushes = new StyleBrush[] { new SolidStyleBrush(StyleColor.White) };
            DefaultOutline.DashPattern = new float[] { 4, 4 };
        }

        public ViewSelection()
        {
            _path = CreatePath();
            _outline = DefaultOutline;
            _fill = DefaultFill;
        }

        protected abstract GraphicsPath<TViewPoint, TViewRegion> CreatePath();

        #region IViewSelection<TViewPoint,TViewSize,TViewRegion> Members
        public void AddPoint(TViewPoint point)
        {
            if (Path.Points.Count == 0 && ((IViewVector)AnchorPoint) == null)
                AnchorPoint = point;

            Path.Points.Add(point);
            recomputeBoundingRegion();
        }

        public void Expand(TViewSize size)
        {
            for (int pointIndex = 0; pointIndex < Path.Points.Count; pointIndex++)
            {
                throw new NotImplementedException("implement this");
            }

            recomputeBoundingRegion();
        }

        public void MoveTo(TViewPoint location)
        {
            throw new NotImplementedException("implement this");
        }

        public void RemovePoint(TViewPoint point)
        {
            _path.Points.Remove(point);
        }

        public GraphicsPath<TViewPoint, TViewRegion> Path
        {
            get { return _path.Clone(); }
        }

        public TViewPoint AnchorPoint
        {
            get { return _anchorPoint; }
            set { _anchorPoint = value; }
        }

        public TViewRegion BoundingRegion
        {
            get { return _boundingRegion; }
        }

        #endregion
        public StylePen OutlineStyle
        {
            get { return _outline; }
            set 
            {
                if (value == null)
                    value = DefaultOutline;

                _outline = value; 
            }
        }

        public StyleColor FillColor
        {
            get { return _fill; }
            private set { _fill = value; }
        }

        private void recomputeBoundingRegion()
        {
            double[,] boxElements = BoundingRegion.Elements;
            bool recorded = false;

            foreach (TViewPoint point in Path.Points)
            {
                double[] pointElements = point.Elements;
                for (int elementIndex = 0; elementIndex < pointElements.Length; elementIndex++)
                {
                    if (!recorded)
                    {
                        boxElements[0, elementIndex] = pointElements[elementIndex];
                        boxElements[1, elementIndex] = pointElements[elementIndex];
                        recorded = true;
                    }
                    else
                    {
                        if (pointElements[elementIndex] > boxElements[0, elementIndex])
                            boxElements[0, elementIndex] = pointElements[elementIndex];

                        if (pointElements[elementIndex] > boxElements[1, elementIndex])
                            boxElements[1, elementIndex] = pointElements[elementIndex];
                    }
                }
            }

            BoundingRegion.Elements = boxElements;
        }
    }
}
