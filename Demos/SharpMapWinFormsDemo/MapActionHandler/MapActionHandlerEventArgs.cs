using System;
using SharpMap.Rendering.Rendering2D;

namespace MapViewer.MapActionHandler
{
    public class MapActionHandlerEventArgs : EventArgs
    {
        public MapActionHandlerEventArgs(Point2D point)
        {
            Point = point;
        }

        public Point2D Point { get; protected set; }
    }
}