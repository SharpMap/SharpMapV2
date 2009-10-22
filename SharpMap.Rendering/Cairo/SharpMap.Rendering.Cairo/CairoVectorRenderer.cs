using System;
using System.Collections.Generic;
using Cairo;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;
using CairoPath = Cairo.Path;
namespace SharpMap.Rendering.Cairo
{
    public class CairoVectorRenderer : VectorRenderer2D<CairoRenderObject>
    {
        private readonly Dictionary<SymbolLookupKey, Surface> _symbolCache = new Dictionary<SymbolLookupKey, Surface>();


        public override IEnumerable<CairoRenderObject> RenderPaths(IEnumerable<Path2D> paths, StylePen line,
                                                                   StylePen highlightLine, StylePen selectLine,
                                                                   StylePen outline, StylePen highlightOutline,
                                                                   StylePen selectOutline, RenderState renderState)
        {
            foreach (Path2D path in paths)
            {
                
                CairoRenderObject holder =
                    new CairoRenderObject(path, null, null, null, ViewConverter.Convert(line),
                                        ViewConverter.Convert(highlightLine), ViewConverter.Convert(selectLine),
                                        ViewConverter.Convert(outline), ViewConverter.Convert(highlightOutline),
                                        ViewConverter.Convert(selectOutline));

                holder.State = renderState;

                yield return holder;
            }
        }

        public override IEnumerable<CairoRenderObject> RenderPaths(IEnumerable<Path2D> paths, StylePen outline,
                                                                   StylePen highlightOutline, StylePen selectOutline,
                                                                   RenderState renderState)
        {
            SolidStyleBrush transparentBrush = new SolidStyleBrush(StyleColor.Transparent);

            return RenderPaths(paths, transparentBrush, transparentBrush,
                               transparentBrush, outline, highlightOutline, selectOutline, renderState);
        }

        public override IEnumerable<CairoRenderObject> RenderPaths(IEnumerable<Path2D> paths, StyleBrush fill,
                                                                   StyleBrush highlightFill, StyleBrush selectFill,
                                                                   StylePen outline, StylePen highlightOutline,
                                                                   StylePen selectOutline, RenderState renderState)
        {
            foreach (Path2D path in paths)
            {
                Path2D cairoPath = ViewConverter.Convert(path);

                CairoRenderObject holder = new CairoRenderObject(cairoPath, ViewConverter.Convert(fill),
                                                             ViewConverter.Convert(highlightFill),
                                                             ViewConverter.Convert(selectFill),
                                                             null, null, null,
                                                             ViewConverter.Convert(outline),
                                                             ViewConverter.Convert(highlightOutline),
                                                             ViewConverter.Convert(selectOutline));

                holder.State = renderState;

                yield return holder;
            }
        }

        public override IEnumerable<CairoRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                     RenderState renderState)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<CairoRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                     ColorMatrix highlight, ColorMatrix select,
                                                                     RenderState renderState)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<CairoRenderObject> RenderSymbols(IEnumerable<Point2D> locations, Symbol2D symbolData,
                                                                     Symbol2D highlightSymbolData,
                                                                     Symbol2D selectSymbolData, RenderState renderState)
        {
            throw new NotImplementedException();
        }

        #region Nested type: SymbolLookupKey

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
    }
}