using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using GeoAPI.IO;
using SharpMap.Rendering.Gdi;
using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Rendering.Web
{
    internal class WebGdiVectorRenderer : GdiVectorRenderer
    {
        private readonly Dictionary<SymbolLookupKey, Bitmap> _symbolCache = new Dictionary<SymbolLookupKey, Bitmap>();

        public override IEnumerable<GdiRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbol,
                                                                   Symbol2D highlightSymbol, Symbol2D selectSymbol,
                                                                   RenderState renderState)
        {
            if (renderState == RenderState.Selected) symbol = selectSymbol;
            if (renderState == RenderState.Highlighted) symbol = highlightSymbol;

            foreach (Point2D location in locations)
            {
                Bitmap bitmapSymbol = getSymbol(symbol);
                if (bitmapSymbol.PixelFormat != PixelFormat.Undefined)
                {
                    System.Drawing.Drawing2D.Matrix transform = ViewConverter.Convert(symbol.AffineTransform);
                    System.Drawing.Imaging.ColorMatrix colorTransform = ViewConverter.Convert(symbol.ColorTransform);
                    RectangleF bounds = new RectangleF(ViewConverter.Convert(location), bitmapSymbol.Size);
                    GdiRenderObject holder = new GdiRenderObject(bitmapSymbol, bounds, transform, colorTransform);
                    holder.State = renderState;
                    yield return holder;
                }
                else
                    Debug.WriteLine("Unkbown pixel format");
            }
        }

        private Bitmap getSymbol(Symbol2D symbol2D)
        {
            if (symbol2D == null)
            {
                return null;
            }

            SymbolLookupKey key = new SymbolLookupKey(symbol2D.GetHashCode());
            Bitmap symbol;
            _symbolCache.TryGetValue(key, out symbol);

            if (symbol == null)
            {
                lock (symbol2D)
                {
                    MemoryStream data = new MemoryStream();
                    symbol2D.SymbolData.Position = 0;

                    using (BinaryReader reader = new BinaryReader(new NondisposingStream(symbol2D.SymbolData)))
                    {
                        data.Write(reader.ReadBytes((Int32) symbol2D.SymbolData.Length), 0,
                                   (Int32) symbol2D.SymbolData.Length);
                    }

                    symbol = new Bitmap(data);
                    if (symbol.PixelFormat != PixelFormat.Undefined)
                    {
                        _symbolCache[key] = symbol;
                    }
                    else
                    {
                        symbol = null;
                    }
                }
            }

            return symbol ?? getSymbol(symbol2D);
        }
    }
}