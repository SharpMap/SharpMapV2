using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using SharpMap.Geometries;
using SharpMap.Rendering;
using SharpMap.Styles;

namespace SharpMap.Presentation
{
    public interface IMapView2D
    {
        ViewSize2D ViewSize { get; set; }
        void ShowRenderedObject(ViewPoint2D location, object renderedObject);
        event EventHandler<MapActionEventArgs> Hover;
        event EventHandler<MapActionEventArgs> BeginAction;
        event EventHandler<MapActionEventArgs> MoveTo;
        event EventHandler<MapActionEventArgs> EndAction;
        event EventHandler ViewSizeChanged;
    }

    public class MapActionEventArgs : EventArgs
    {
        private ViewPoint2D _actionPoint;

        public MapActionEventArgs(ViewPoint2D actionPoint)
        {
            _actionPoint = actionPoint;
        }

        public ViewPoint2D ActionPoint
        {
            get { return _actionPoint; }
            protected set { _actionPoint = value; }
        }
    }
}
