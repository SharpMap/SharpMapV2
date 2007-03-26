using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public struct PositionedRenderObject2D<TRenderObject>
    {
        private readonly ViewPoint2D _location;
        private readonly TRenderObject _renderObject;

        public PositionedRenderObject2D(ViewPoint2D location, TRenderObject renderObject)
        {
            _location = location;
            _renderObject = renderObject;
        }

        public ViewPoint2D Location
        {
            get { return _location; }
        }

        public TRenderObject RenderObject
        {
            get { return _renderObject; }
        }
    }
}
