using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using GeoAPI.IO;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

using WpfPath = System.Windows.Media.StreamGeometry;
using WpfPoint = System.Windows.Point;
using WpfPen = System.Windows.Media.Pen;
using WpfBrush = System.Windows.Media.Brush;
using WpfBitmap = System.Windows.Media.Imaging.BitmapSource;
using WpfBitmapImage = System.Windows.Media.Imaging.BitmapImage;
using WpfMatrix = System.Windows.Media.Matrix;
using WpfRectangle = System.Windows.Rect;

namespace SharpMap.Rendering.Wpf
{
    public class WpfVectorRenderer : VectorRenderer2D<WpfRenderObject>
    {
        private readonly Dictionary<SymbolLookupKey, BitmapSource> _symbolCache = new Dictionary<SymbolLookupKey, BitmapSource>();

        public override IEnumerable<WpfRenderObject> RenderPaths(IEnumerable<Path2D> paths, StylePen line,
                                                                  StylePen highlightLine, StylePen selectLine,
                                                                  StylePen outline, StylePen highlightOutline,
                                                                  StylePen selectOutline, RenderState renderState)
        {
            foreach (Path2D path2D in paths)
            {
                WpfPath path = ViewConverter.Convert(path2D);
                WpfPen wpfLine = null, wpfOutline = null;
                switch (renderState)
                {
                    case RenderState.Normal:
                        wpfLine = ViewConverter.Convert(line);
                        wpfOutline = ViewConverter.Convert(outline);
                        break;
                    case RenderState.Selected:
                        wpfLine = ViewConverter.Convert(selectLine);
                        wpfOutline = ViewConverter.Convert(selectOutline);
                        break;
                    case RenderState.Highlighted:
                        wpfLine = ViewConverter.Convert(highlightLine);
                        wpfOutline = ViewConverter.Convert(highlightOutline);
                        break;
                    case RenderState.Unknown:
                        yield break;
                }
                yield return new WpfVectorRenderObject(renderState, path, wpfLine, wpfOutline, null);
            }
        }

        public override IEnumerable<WpfRenderObject> RenderPaths(IEnumerable<Path2D> paths, StylePen outline,
                                                                  StylePen highlightOutline, StylePen selectOutline,
                                                                  RenderState renderState)
        {
            foreach (Path2D path2D in paths)
            {
                WpfPath path = ViewConverter.Convert(path2D);
                WpfPen wpfOutline = null;
                switch (renderState)
                {
                    case RenderState.Normal:
                        wpfOutline = ViewConverter.Convert(outline);
                        break;
                    case RenderState.Selected:
                        wpfOutline = ViewConverter.Convert(selectOutline);
                        break;
                    case RenderState.Highlighted:
                        wpfOutline = ViewConverter.Convert(highlightOutline);
                        break;
                    case RenderState.Unknown:
                        yield break;
                }
                yield return new WpfVectorRenderObject(renderState, path, null, wpfOutline, null);
            }
        }

        public override IEnumerable<WpfRenderObject> RenderPaths(IEnumerable<Path2D> paths, StyleBrush fill,
                                                                  StyleBrush highlightFill, StyleBrush selectFill,
                                                                  StylePen outline, StylePen highlightOutline,
                                                                  StylePen selectOutline, RenderState renderState)
        {
            foreach (Path2D path2D in paths)
            {
                WpfPath path = ViewConverter.Convert(path2D);
                WpfPen wpfOutline = null;
                WpfBrush wpfFill = null;
                switch (renderState)
                {
                    case RenderState.Normal:
                        wpfFill = ViewConverter.Convert(fill);
                        wpfOutline = ViewConverter.Convert(outline);
                        break;
                    case RenderState.Selected:
                        wpfFill = ViewConverter.Convert(selectFill);
                        wpfOutline = ViewConverter.Convert(selectOutline);
                        break;
                    case RenderState.Highlighted:
                        wpfFill = ViewConverter.Convert(highlightFill);
                        wpfOutline = ViewConverter.Convert(highlightOutline);
                        break;
                    case RenderState.Unknown:
                        yield break;
                }
                yield return new WpfVectorRenderObject(renderState, path, null, wpfOutline, wpfFill);
            }
        }

        public override IEnumerable<WpfRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                    RenderState renderState)
        {
            return RenderSymbols(locations, symbolData, symbolData, symbolData, renderState);
        }

        public override IEnumerable<WpfRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                    ColorMatrix highlight, ColorMatrix select,
                                                                    RenderState renderState)
        {
            Symbol2D highlightSymbol = (Symbol2D)symbolData.Clone();
            highlightSymbol.ColorTransform = highlight;

            Symbol2D selectSymbol = (Symbol2D)symbolData.Clone();
            selectSymbol.ColorTransform = select;

            return RenderSymbols(locations, symbolData, highlightSymbol, selectSymbol, renderState);
        }

        public override IEnumerable<WpfRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                    Symbol2D highlightSymbolData,
                                                                    Symbol2D selectSymbolData, RenderState renderState)
        {
            if (renderState == RenderState.Selected) symbolData = selectSymbolData;
            if (renderState == RenderState.Highlighted) symbolData = highlightSymbolData;

            WpfBitmap bitmapSymbol = GetSymbol(symbolData);
            foreach (Point2D location in locations)
            {
                WpfMatrix transform = ViewConverter.Convert(symbolData.AffineTransform);
                WpfPoint point = ViewConverter.Convert(location);
                //GdiColorMatrix colorTransform = ViewConverter.Convert(symbol.ColorTransform);
                WpfRectangle bounds = new WpfRectangle(ViewConverter.Convert(location), new Size(bitmapSymbol.PixelWidth, bitmapSymbol.PixelHeight));
                yield return new WpfPointRenderObject(renderState, point, bounds, transform, bitmapSymbol);
            }
        }

        #region Nested types

        protected struct SymbolLookupKey : IEquatable<SymbolLookupKey>
        {
            public readonly Int32 SymbolId;

            public SymbolLookupKey(Int32 symbolId)
            {
                SymbolId = symbolId;
            }

            #region IEquatable<SymbolLookupKey> Members

            public Boolean Equals(SymbolLookupKey other)
            {
                return other.SymbolId == SymbolId;
            }

            #endregion
        }

        #endregion

        #region Private helper methods

        private WpfBitmap GetSymbol(Symbol2D symbol2D)
        {
            if (symbol2D == null)
            {
                return null;
            }

            SymbolLookupKey key = new SymbolLookupKey(symbol2D.GetHashCode());
            WpfBitmap symbol;
            _symbolCache.TryGetValue(key, out symbol);

            if (symbol == null)
            {
                MemoryStream data = new MemoryStream();
                symbol2D.SymbolData.Position = 0;

                using (BinaryReader reader = new BinaryReader(new NondisposingStream(symbol2D.SymbolData)))
                {
                    data.Write(reader.ReadBytes((Int32)symbol2D.SymbolData.Length), 0, (Int32)symbol2D.SymbolData.Length);
                }
                data.Seek(0, SeekOrigin.Begin);

                //Create the symbol
                WpfBitmapImage bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = data;
                bmp.EndInit();

                symbol = bmp;
                _symbolCache[key] = symbol;
            }

            return symbol;
        }

        #endregion

    }
}