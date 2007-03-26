using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Rendering;

namespace SharpMap.Presentation
{
    [Serializable]
    public class ViewPointActionEventArgs<TViewPoint> : EventArgs
        where TViewPoint : IViewVector
    {
        private TViewPoint _viewPoint;

        public TViewPoint ViewPoint
        {
            get { return _viewPoint; }
            set { _viewPoint = value; }
        }
    }
}
