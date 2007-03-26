using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SharpMap.Rendering
{
    interface IRasterRenderer<TViewPoint, TViewSize, TViewRectangle, TRenderObject> : IRenderer<TViewPoint, TViewSize, TViewRectangle, TRenderObject>
        where TViewPoint : IViewVector
        where TViewSize : IViewVector
        where TViewRectangle : IViewMatrix
    {
        void DrawRaster(Stream rasterData, TViewRectangle viewBounds, TViewRectangle rasterBounds);
        void DrawRaster(Stream rasterData, TViewRectangle viewBounds, TViewRectangle rasterBounds, IViewMatrix rasterTransform);
    }
}
