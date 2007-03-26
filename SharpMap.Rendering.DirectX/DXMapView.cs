using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering.DirectX
{
    public class DXMapView : IMapView2D
    {
        #region IMapView2D Members

        public ViewSize2D ViewSize
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public void ShowRenderedObject(ViewPoint2D location, object renderedObject)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public event EventHandler<MapActionEventArgs> Hover;

        public event EventHandler<MapActionEventArgs> BeginAction;

        public event EventHandler<MapActionEventArgs> MoveTo;

        public event EventHandler<MapActionEventArgs> EndAction;

        public event EventHandler ViewSizeChanged;

        #endregion
    }
}
