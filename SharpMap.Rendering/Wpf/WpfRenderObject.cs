using System;
using System.Windows;
using System.Windows.Media;
using WpfPath = System.Windows.Media.StreamGeometry;
using WpfPoint = System.Windows.Point;
using WpfPen = System.Windows.Media.Pen;
using WpfBrush = System.Windows.Media.Brush;
using WpfBitmap = System.Windows.Media.Imaging.BitmapSource;
using WpfRectangle = System.Windows.Rect;
using WpfMatrix = System.Windows.Media.Matrix;
using WpfFont = System.Windows.Media.Typeface;


namespace SharpMap.Rendering.Wpf
{
    public abstract class WpfRenderObject : DependencyObject
    {
        private readonly RenderState _renderState;

        protected WpfRenderObject(RenderState renderState)
        {
            _renderState = renderState;
        }

        public RenderState RenderState { get { return _renderState; } }
    }

    public class WpfVectorRenderObject : WpfRenderObject
    {
        public WpfVectorRenderObject(RenderState renderState, WpfPath path, WpfPen line, WpfPen outline, WpfBrush fill)
            :base(renderState)
        {
            Path = path;
            Line = line;
            Outline = outline;
            Fill = fill;
        }

        public readonly WpfPath Path;
        public readonly WpfPen Line;
        public readonly WpfPen Outline;
        public readonly WpfBrush Fill;
    }

    public class WpfPointRenderObject : WpfRenderObject
    {
        public WpfPointRenderObject(RenderState renderState, WpfPoint point, WpfRectangle bounds, WpfMatrix transform, WpfBitmap symbol)
            : base(renderState)
        {
            Point = point;
            Bounds = bounds;
            Symbol = symbol;
            Transform = transform;
        }

        public readonly WpfPoint Point;
        public readonly WpfBitmap Symbol;
        public readonly WpfRectangle Bounds;
        public readonly WpfMatrix Transform;
    }

    public class WpfRasterRenderObject : WpfRenderObject
    {
        public WpfRasterRenderObject(RenderState renderState, WpfRectangle bounds, WpfBitmap raster)
            : base(renderState)
        {
            Bounds = bounds;
            Raster = raster;
        }

        public readonly WpfBitmap Raster;
        public readonly WpfRectangle Bounds;

    }

    public class WpfTextRenderObject<T> : WpfRenderObject
    {
        internal WpfTextRenderObject(RenderState renderState, FormattedText text, T location)
            : base(renderState)
        {
            Text = text;
            Location = location;
        }

        public readonly FormattedText Text;
        public readonly T Location;
    }
}