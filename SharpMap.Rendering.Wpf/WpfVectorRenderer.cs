using System;
using System.Collections.Generic;
using System.Windows;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Rendering.Wpf
{
    public class WpfVectorRenderer : VectorRenderer2D<DependencyObject>
    {
        public override IEnumerable<DependencyObject> RenderPaths(IEnumerable<Path2D> paths, StylePen line,
                                                                  StylePen highlightLine, StylePen selectLine,
                                                                  StylePen outline, StylePen highlightOutline,
                                                                  StylePen selectOutline, RenderState renderState)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DependencyObject> RenderPaths(IEnumerable<Path2D> paths, StylePen outline,
                                                                  StylePen highlightOutline, StylePen selectOutline,
                                                                  RenderState renderState)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DependencyObject> RenderPaths(IEnumerable<Path2D> paths, StyleBrush fill,
                                                                  StyleBrush highlightFill, StyleBrush selectFill,
                                                                  StylePen outline, StylePen highlightOutline,
                                                                  StylePen selectOutline, RenderState renderState)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DependencyObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                    RenderState renderState)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DependencyObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                    ColorMatrix highlight, ColorMatrix select,
                                                                    RenderState renderState)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DependencyObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                    Symbol2D highlightSymbolData,
                                                                    Symbol2D selectSymbolData, RenderState renderState)
        {
            throw new NotImplementedException();
        }
    }
}