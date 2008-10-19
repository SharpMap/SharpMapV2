using System;
using SharpMap.Rendering.Rendering2D;

namespace MapViewer.MapActionHandler
{
    public interface IMapActionHandler
    {
        Point2D BeginPoint { get; set; }
        Point2D HoverPoint { get; set; }
        Point2D EndPoint { get; set; }
        event EventHandler<MapActionHandlerEventArgs> Begin;
        event EventHandler<MapActionHandlerEventArgs> Hover;
        event EventHandler<MapActionHandlerEventArgs> End;
    }
}