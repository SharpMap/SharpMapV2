using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public class GraphicsPath2D : GraphicsPath<ViewPoint2D, ViewRectangle2D>
    {
        public GraphicsPath2D() 
            :base() { }

        public GraphicsPath2D(IEnumerable<ViewPoint2D> points)
            : base(points) { }

        public GraphicsPath2D(IEnumerable<ViewPoint2D> points, bool closeFigure)
            : base(points, closeFigure) { }

        public GraphicsPath2D(IEnumerable<GraphicsFigure<ViewPoint2D, ViewRectangle2D>> figures)
            : base(figures) { }

        protected override GraphicsPath<ViewPoint2D, ViewRectangle2D> CreatePath(IEnumerable<ViewPoint2D> points, bool closeFigure)
        {
            return new GraphicsPath2D(points, closeFigure);
        }

        protected override GraphicsPath<ViewPoint2D, ViewRectangle2D> CreatePath(IEnumerable<GraphicsFigure<ViewPoint2D, ViewRectangle2D>> figures)
        {
            return new GraphicsPath2D(figures);
        }

        protected override GraphicsFigure<ViewPoint2D, ViewRectangle2D> CreateFigure(IEnumerable<ViewPoint2D> points, bool closeFigure)
        {
            return new GraphicsFigure2D(points, closeFigure);
        }

        protected override ViewRectangle2D ComputeBounds()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
