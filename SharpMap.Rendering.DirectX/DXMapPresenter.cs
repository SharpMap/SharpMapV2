using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Rendering;

namespace SharpMap.Rendering.DirectX
{
    public class DXMapPresenter : BaseMapPresenter2D
    {
        public DXMapPresenter(Map map, DXMapView view, IEnumerable<IToolsView> toolViews)
            : base(map, view, toolViews) { }
    }
}
