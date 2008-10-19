using System;
using SharpMap.Rendering.Rendering2D;

namespace MapViewer.MapActionHandler
{
    public class MapActionHandler : IMapActionHandler
    {
        private Point2D beginPoint;
        private Point2D endPoint;
        private Point2D hoverPoint;

        #region IMapActionHandler Members

        public event EventHandler<MapActionHandlerEventArgs> Begin;
        public event EventHandler<MapActionHandlerEventArgs> Hover;
        public event EventHandler<MapActionHandlerEventArgs> End;

        public Point2D BeginPoint
        {
            get { return beginPoint; }
            set
            {
                beginPoint = value;
                HoverPoint = value;
                OnBeginPointSet();
                EndPoint = Point2D.Empty;
            }
        }

        public Point2D HoverPoint
        {
            get { return hoverPoint; }
            set
            {
                hoverPoint = value;
                OnHoverPontSet();
            }
        }

        public Point2D EndPoint
        {
            get { return endPoint; }
            set
            {
                endPoint = value;
                OnEndPointSet();
            }
        }

        #endregion

        protected void OnBeginPointSet()
        {
            if (!BeginPoint.IsEmpty && Begin != null) Begin(this, new MapActionHandlerEventArgs(BeginPoint));
        }

        protected void OnHoverPontSet()
        {
            if (!HoverPoint.IsEmpty && Hover != null) Hover(this, new MapActionHandlerEventArgs(HoverPoint));
        }

        protected void OnEndPointSet()
        {
            if (!EndPoint.IsEmpty && End != null) End(this, new MapActionHandlerEventArgs(EndPoint));
        }
    }
}