using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public class GraphicsFigure2D : GraphicsFigure<ViewPoint2D, ViewRectangle2D>
    {
        public GraphicsFigure2D(IEnumerable<ViewPoint2D> points, bool isClosed)
            : base(points, isClosed) { }

        protected override ViewRectangle2D ComputeBounds()
        {
            double left = Double.MaxValue;
            double top = Double.MaxValue;
            double right = Double.MinValue;
            double bottom = Double.MinValue;

            foreach (ViewPoint2D point in Points)
            {
                if (left > point.X)
                    left = point.X;
                if (right < point.X)
                    right = point.X;
                if (top > point.Y)
                    top = point.Y;
                if (bottom < point.Y)
                    bottom = point.Y;
            }

            return new ViewRectangle2D(left, right, top, bottom);
        }

        protected override GraphicsFigure<ViewPoint2D, ViewRectangle2D> CreateFigure(IEnumerable<ViewPoint2D> points, bool isClosed)
        {
            return new GraphicsFigure2D(points, isClosed);
        }
    }
}
