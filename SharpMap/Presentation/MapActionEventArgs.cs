using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Geometries;
using SharpMap.Rendering;
using SharpMap.Styles;

namespace SharpMap.Presentation
{
    public class MapActionEventArgs<TPoint> : EventArgs
        where TPoint : IViewVector
    {
        private TPoint _actionPoint;

        public MapActionEventArgs(TPoint actionPoint)
        {
            _actionPoint = actionPoint;
        }

        public TPoint ActionPoint
        {
            get { return _actionPoint; }
            protected set { _actionPoint = value; }
        }
    }
}
