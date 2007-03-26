using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public class ViewSelection2D : ViewSelection<ViewPoint2D, ViewSize2D, ViewRectangle2D>
    {
        public static ViewSelection2D CreateRectangluarSelection(ViewPoint2D location, ViewSize2D size)
        {
            ViewSelection2D selection = new ViewSelection2D();
            selection.AddPoint(location);
            selection.AddPoint(location);
            selection.AddPoint(location);
            selection.AddPoint(location);

            selection.Expand(size);
            
            return selection;
        }

        protected override GraphicsPath<ViewPoint2D, ViewRectangle2D> CreatePath()
        {
            return new GraphicsPath2D(new ViewPoint2D[0]);
        }
    }
}
