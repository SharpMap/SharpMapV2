
/*
 *	Portions of this file are  © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 *
 *  Original notices below.
 * 
 */
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------

using System;
using AGG.Color;
using AGG.PixelFormat;
using AGG.Rasterizer;
using AGG.Scanline;
using AGG.Span;
using AGG.Transform;
using AGG.VertexSource;
using NPack.Interfaces;

namespace AGG.Rendering
{
    public class Renderer<T> : RendererBase<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        const int CoverFull = 255;
        protected IScanlineCache m_ScanlineCache;

        public Renderer()
        {
        }

        public Renderer(IPixelFormat PixelFormat, RasterizerScanlineAA<T> Rasterizer, IScanlineCache ScanlineCache)
            : base(PixelFormat, Rasterizer)
        {
            m_ScanlineCache = ScanlineCache;
        }

        public override IScanlineCache ScanlineCache
        {
            get { return m_ScanlineCache; }
            set { m_ScanlineCache = value; }
        }

        public override void Render(IVertexSource<T> vertexSource, uint pathIndexToRender, RGBA_Bytes colorBytes)
        {
            m_Rasterizer.Reset();
            IAffineTransformMatrix<T> transform = GetTransform();
            if (!transform.Equals(MatrixFactory<T>.NewIdentity(VectorDimension.Two)))
            {
                vertexSource = new ConvTransform<T>(vertexSource, transform);
            }
            m_Rasterizer.AddPath(vertexSource, pathIndexToRender);
            Renderer<T>.RenderSolid(m_PixelFormat, m_Rasterizer, m_ScanlineCache, colorBytes);
        }

        public override void Clear(IColorType color)
        {
            FormatClippingProxy clipper = (FormatClippingProxy)m_PixelFormat;
            if (clipper != null)
            {
                clipper.Clear(color);
            }
        }

        #region RenderSolid

#if use_timers
        static CNamedTimer PrepareTimer = new CNamedTimer("Prepare");
#endif

        //========================================================render_scanlines
        public static void RenderSolid(IPixelFormat pixFormat, IRasterizer<T> rasterizer, IScanlineCache scanLine, RGBA_Bytes color)
        {
            if (rasterizer.RewindScanlines())
            {
                scanLine.Reset(rasterizer.MinX, rasterizer.MaxX);
#if use_timers
                PrepareTimer.Start();
#endif
                //renderer.prepare();
#if use_timers
                PrepareTimer.Stop();
#endif
                while (rasterizer.SweepScanline(scanLine))
                {
                    Renderer<T>.RenderSolidSingleScanLine(pixFormat, scanLine, color);
                }
            }
        }
        #endregion

        #region RenderSolidSingleScanLine
#if use_timers
        static CNamedTimer render_scanline_aa_solidTimer = new CNamedTimer("render_scanline_aa_solid");
        static CNamedTimer render_scanline_aa_solid_blend_solid_hspan = new CNamedTimer("render_scanline_aa_solid_blend_solid_hspan");
        static CNamedTimer render_scanline_aa_solid_blend_hline = new CNamedTimer("render_scanline_aa_solid_blend_hline");
#endif
        //================================================render_scanline_aa_solid
        private static void RenderSolidSingleScanLine(IPixelFormat pixFormat, IScanlineCache scanLine, RGBA_Bytes color)
        {
#if use_timers
            render_scanline_aa_solidTimer.Start();
#endif
            int y = scanLine.Y;
            uint num_spans = scanLine.NumSpans;
            ScanlineSpan scanlineSpan = scanLine.Begin();

            byte[] ManagedCoversArray = scanLine.Covers;
            unsafe
            {
                fixed (byte* pCovers = ManagedCoversArray)
                {
                    for (; ; )
                    {
                        int x = scanlineSpan.X;
                        if (scanlineSpan.Len > 0)
                        {
#if use_timers
                            render_scanline_aa_solid_blend_solid_hspan.Start();
#endif
                            pixFormat.BlendSolidHSpan(x, y, (uint)scanlineSpan.Len, color, &pCovers[scanlineSpan.CoverIndex]);
#if use_timers
                            render_scanline_aa_solid_blend_solid_hspan.Stop();
#endif
                        }
                        else
                        {
#if use_timers
                            render_scanline_aa_solid_blend_hline.Start();
#endif
                            pixFormat.BlendHLine(x, y, (x - (int)scanlineSpan.Len - 1), color, pCovers[scanlineSpan.CoverIndex]);
#if use_timers
                            render_scanline_aa_solid_blend_hline.Stop();
#endif
                        }
                        if (--num_spans == 0) break;
                        scanlineSpan = scanLine.GetNextScanlineSpan();
                    }
                }
            }
#if use_timers
            render_scanline_aa_solidTimer.Stop();
#endif
        }
        #endregion

        #region RenderSolidAllPaths
#if use_timers
        static CNamedTimer AddPathTimer = new CNamedTimer("AddPath");
        static CNamedTimer RenderSLTimer = new CNamedTimer("RenderSLs");
#endif
        //========================================================render_all_paths
        public static void RenderSolidAllPaths(IPixelFormat pixFormat,
            IRasterizer<T> ras,
            IScanlineCache sl,
            IVertexSource<T> vs,
            RGBA_Bytes[] color_storage,
            uint[] path_id,
            uint num_paths)
        {
            for (uint i = 0; i < num_paths; i++)
            {
                ras.Reset();

#if use_timers
                AddPathTimer.Start();
#endif
                ras.AddPath(vs, path_id[i]);
#if use_timers
                AddPathTimer.Stop();
#endif


#if use_timers
                RenderSLTimer.Start();
#endif
                RenderSolid(pixFormat, ras, sl, color_storage[i]);
#if use_timers
                RenderSLTimer.Stop();
#endif
            }
        }
        #endregion

        #region GenerateAndRenderSingleScanline
        static VectorPOD<RGBA_Bytes> tempSpanColors = new VectorPOD<RGBA_Bytes>();

#if use_timers
        static CNamedTimer blend_color_hspan = new CNamedTimer("blend_color_hspan");
#endif
        //======================================================render_scanline_aa
        private static void GenerateAndRenderSingleScanline(IScanlineCache sl, IPixelFormat ren,
                                SpanAllocator alloc, ISpanGenerator<T> span_gen)
        {
            int y = sl.Y;
            uint num_spans = sl.NumSpans;
            ScanlineSpan scanlineSpan = sl.Begin();

            byte[] ManagedCoversArray = sl.Covers;
            unsafe
            {
                fixed (byte* pCovers = ManagedCoversArray)
                {
                    for (; ; )
                    {
                        int x = scanlineSpan.X;
                        int len = scanlineSpan.Len;
                        if (len < 0) len = -len;

                        if (tempSpanColors.Capacity() < len)
                        {
                            tempSpanColors.Allocate((uint)(len));
                        }

                        fixed (RGBA_Bytes* pColors = tempSpanColors.Array)
                        {
                            span_gen.Generate(pColors, x, y, (uint)len);
#if use_timers
                            blend_color_hspan.Start();
#endif
                            ren.BlendColorHSpan(x, y, (uint)len, pColors, (scanlineSpan.Len < 0) ? null : &pCovers[scanlineSpan.CoverIndex], pCovers[scanlineSpan.CoverIndex]);
#if use_timers
                            blend_color_hspan.Stop();
#endif
                        }

                        if (--num_spans == 0) break;
                        scanlineSpan = sl.GetNextScanlineSpan();
                    }
                }
            }
        }
        #endregion

        #region GenerateAndRender
        //=====================================================render_scanlines_aa
        public static void GenerateAndRender(IRasterizer<T> ras, IScanlineCache sl, IPixelFormat ren,
                                 SpanAllocator alloc, ISpanGenerator<T> span_gen)
        {
            if (ras.RewindScanlines())
            {
                sl.Reset(ras.MinX, ras.MaxX);
                span_gen.Prepare();
                while (ras.SweepScanline(sl))
                {
                    GenerateAndRenderSingleScanline(sl, ren, alloc, span_gen);
                }
            }
        }
        #endregion

        #region RenderCompound
        public static void RenderCompound(RasterizerCompoundAA<T> ras,
                                       IScanlineCache sl_aa,
                                       IScanlineCache sl_bin,
                                       IPixelFormat pixelFormat,
                                       SpanAllocator alloc,
                                       IStyleHandler sh)
        {
#if true
            unsafe
            {
                if (ras.RewindScanlines())
                {
                    int min_x = ras.MinX;
                    int len = ras.MaxX - min_x + 2;
                    sl_aa.Reset(min_x, ras.MaxX);
                    sl_bin.Reset(min_x, ras.MaxX);

                    //typedef typename BaseRenderer::color_type color_type;
                    ArrayPOD<RGBA_Bytes> color_span = alloc.Allocate((uint)len * 2);
                    byte[] ManagedCoversArray = sl_aa.Covers;
                    fixed (byte* pCovers = ManagedCoversArray)
                    {
                        fixed (RGBA_Bytes* pColorSpan = color_span.Array)
                        {
                            int mix_bufferOffset = len;
                            uint num_spans;

                            uint num_styles;
                            uint style;
                            bool solid;
                            while ((num_styles = ras.SweepStyles()) > 0)
                            {
                                if (num_styles == 1)
                                {
                                    // Optimization for a single style. Happens often
                                    //-------------------------
                                    if (ras.SweepScanline(sl_aa, 0))
                                    {
                                        style = ras.Style(0);
                                        if (sh.IsSolid(style))
                                        {
                                            // Just solid fill
                                            //-----------------------
                                            RenderSolidSingleScanLine(pixelFormat, sl_aa, sh.Color(style));
                                        }
                                        else
                                        {
                                            // Arbitrary span generator
                                            //-----------------------
                                            ScanlineSpan span_aa = sl_aa.Begin();
                                            num_spans = sl_aa.NumSpans;
                                            for (; ; )
                                            {
                                                len = span_aa.Len;
                                                sh.GenerateSpan(pColorSpan,
                                                                 span_aa.X,
                                                                 sl_aa.Y,
                                                                 (uint)len,
                                                                 style);

                                                pixelFormat.BlendColorHSpan(span_aa.X,
                                                                      sl_aa.Y,
                                                                      (uint)span_aa.Len,
                                                                      pColorSpan,
                                                                      &pCovers[span_aa.CoverIndex], 0);
                                                if (--num_spans == 0) break;
                                                span_aa = sl_aa.GetNextScanlineSpan();
                                            }
                                        }
                                    }
                                }
                                else // there are multiple styles
                                {
                                    if (ras.SweepScanline(sl_bin, -1))
                                    {
                                        // Clear the spans of the mix_buffer
                                        //--------------------
                                        ScanlineSpan span_bin = sl_bin.Begin();
                                        num_spans = sl_bin.NumSpans;
                                        for (; ; )
                                        {
                                            Basics.MemClear((byte*)&pColorSpan[mix_bufferOffset + span_bin.X - min_x],
                                                   span_bin.Len * sizeof(RGBA_Bytes));

                                            if (--num_spans == 0) break;
                                            span_bin = sl_bin.GetNextScanlineSpan();
                                        }

                                        for (uint i = 0; i < num_styles; i++)
                                        {
                                            style = ras.Style(i);
                                            solid = sh.IsSolid(style);

                                            if (ras.SweepScanline(sl_aa, (int)i))
                                            {
                                                //IColorType* colors;
                                                //IColorType* cspan;
                                                //typename ScanlineAA::cover_type* covers;
                                                ScanlineSpan span_aa = sl_aa.Begin();
                                                num_spans = sl_aa.NumSpans;
                                                if (solid)
                                                {
                                                    // Just solid fill
                                                    //-----------------------
                                                    for (; ; )
                                                    {
                                                        RGBA_Bytes c = sh.Color(style);
                                                        len = span_aa.Len;
                                                        RGBA_Bytes* colors = &pColorSpan[mix_bufferOffset + span_aa.X - min_x];
                                                        byte* covers = &pCovers[span_aa.CoverIndex];
                                                        do
                                                        {
                                                            if (*covers == CoverFull)
                                                            {
                                                                *colors = c;
                                                            }
                                                            else
                                                            {
                                                                colors->Add(c, *covers);
                                                            }
                                                            ++colors;
                                                            ++covers;
                                                        }
                                                        while (--len != 0);
                                                        if (--num_spans == 0) break;
                                                        span_aa = sl_aa.GetNextScanlineSpan();
                                                    }
                                                }
                                                else
                                                {
                                                    // Arbitrary span generator
                                                    //-----------------------
                                                    for (; ; )
                                                    {
                                                        len = span_aa.Len;
                                                        RGBA_Bytes* colors = &pColorSpan[mix_bufferOffset + span_aa.X - min_x];
                                                        RGBA_Bytes* cspan = pColorSpan;
                                                        sh.GenerateSpan(cspan,
                                                                         span_aa.X,
                                                                         sl_aa.Y,
                                                                         (uint)len,
                                                                         style);
                                                        byte* covers = &pCovers[span_aa.CoverIndex];
                                                        do
                                                        {
                                                            if (*covers == CoverFull)
                                                            {
                                                                *colors = *cspan;
                                                            }
                                                            else
                                                            {
                                                                colors->Add(*cspan, *covers);
                                                            }
                                                            ++cspan;
                                                            ++colors;
                                                            ++covers;
                                                        }
                                                        while (--len != 0);
                                                        if (--num_spans == 0) break;
                                                        span_aa = sl_aa.GetNextScanlineSpan();
                                                    }
                                                }
                                            }
                                        }

                                        // Emit the blended result as a color hspan
                                        //-------------------------
                                        span_bin = sl_bin.Begin();
                                        num_spans = sl_bin.NumSpans;
                                        for (; ; )
                                        {
                                            pixelFormat.BlendColorHSpan(span_bin.X,
                                                                  sl_bin.Y,
                                                                  (uint)span_bin.Len,
                                                                  &pColorSpan[mix_bufferOffset + span_bin.X - min_x],
                                                                  null,
                                                                  CoverFull);
                                            if (--num_spans == 0) break;
                                            span_bin = sl_bin.GetNextScanlineSpan();
                                        }
                                    } // if(ras.sweep_scanline(sl_bin, -1))
                                } // if(num_styles == 1) ... else
                            } // while((num_styles = ras.sweep_styles()) > 0)
                        }
                    }
                } // if(ras.rewind_scanlines())
#endif
            }
        }
        #endregion
    }

    /*
    //==============================================renderer_scanline_aa_solid
    public class renderer_scanline_aa_solid : IRenderer
    {
        private IPixelFormat m_ren;
        private IColorType m_color;

        //--------------------------------------------------------------------
        public renderer_scanline_aa_solid(IPixelFormat ren) 
        {
            m_ren = ren;
            m_color = new rgba(0,0,0,1);
        }

        public void Attach(IPixelFormat ren)
        {
            m_ren = ren;
        }
        
        //--------------------------------------------------------------------
        public IColorType Color
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = Value;
            }
        }

        //--------------------------------------------------------------------
        public void prepare() {}

        //--------------------------------------------------------------------
        public void render(IScanline sl)
        {
            renderer_scanlines.render_scanline_aa_solid(sl, m_ren, m_color.Get_rgba8());
        }
    };

    //====================================================renderer_scanline_aa
    template<class BaseRenderer, class SpanAllocator, class SpanGenerator> 
    class renderer_scanline_aa
    {
    public:
        typedef BaseRenderer  base_ren_type;
        typedef SpanAllocator alloc_type;
        typedef SpanGenerator span_gen_type;

        //--------------------------------------------------------------------
        renderer_scanline_aa() : m_ren(0), m_alloc(0), m_span_gen(0) {}
        renderer_scanline_aa(base_ren_type& ren, 
                             alloc_type& alloc, 
                             span_gen_type& span_gen) :
            m_ren(&ren),
            m_alloc(&alloc),
            m_span_gen(&span_gen)
        {}
        void Attach(base_ren_type& ren, 
                    alloc_type& alloc, 
                    span_gen_type& span_gen)
        {
            m_ren = &ren;
            m_alloc = &alloc;
            m_span_gen = &span_gen;
        }
        
        //--------------------------------------------------------------------
        void prepare() { m_span_gen->prepare(); }

        //--------------------------------------------------------------------
        template<class Scanline> void render(const Scanline& sl)
        {
            render_scanline_aa(sl, *m_ren, *m_alloc, *m_span_gen);
        }

    private:
        base_ren_type* m_ren;
        alloc_type*    m_alloc;
        span_gen_type* m_span_gen;
    };

    //===============================================render_scanline_bin_solid
    template<class Scanline, class BaseRenderer, class ColorT> 
    void render_scanline_bin_solid(const Scanline& sl, 
                                   BaseRenderer& ren, 
                                   const ColorT& color)
    {
        uint num_spans = sl.num_spans();
        typename Scanline::const_iterator span = sl.begin();
        for(;;)
        {
            ren.blend_hline(span->x, 
                            sl.y(), 
                            span->x - 1 + ((span->len < 0) ? 
                                              -span->len : 
                                               span->len), 
                               color, 
                               cover_full);
            if(--num_spans == 0) break;
            ++span;
        }
    }

    //==============================================render_scanlines_bin_solid
    template<class Rasterizer, class Scanline, 
             class BaseRenderer, class ColorT>
    void render_scanlines_bin_solid(Rasterizer& ras, Scanline& sl, 
                                    BaseRenderer& ren, const ColorT& color)
    {
        if(ras.rewind_scanlines())
        {
            // Explicitly convert "color" to the BaseRenderer color type.
            // For example, it can be called with color type "rgba", while
            // "rgba8" is needed. Otherwise it will be implicitly 
            // converted in the loop many times.
            //----------------------
            typename BaseRenderer::color_type ren_color(color);

            sl.reset(ras.min_x(), ras.max_x());
            while(ras.sweep_scanline(sl))
            {
                //render_scanline_bin_solid(sl, ren, ren_color);

                // This code is equivalent to the above call (copy/paste). 
                // It's just a "manual" optimization for old compilers,
                // like Microsoft Visual C++ v6.0
                //-------------------------------
                uint num_spans = sl.num_spans();
                typename Scanline::const_iterator span = sl.begin();
                for(;;)
                {
                    ren.blend_hline(span->x, 
                                    sl.y(), 
                                    span->x - 1 + ((span->len < 0) ? 
                                                      -span->len : 
                                                       span->len), 
                                       ren_color, 
                                       cover_full);
                    if(--num_spans == 0) break;
                    ++span;
                }
            }
        }
    }

    //=============================================renderer_scanline_bin_solid
    template<class BaseRenderer> class renderer_scanline_bin_solid
    {
    public:
        typedef BaseRenderer base_ren_type;
        typedef typename base_ren_type::color_type color_type;

        //--------------------------------------------------------------------
        renderer_scanline_bin_solid() : m_ren(0) {}
        explicit renderer_scanline_bin_solid(base_ren_type& ren) : m_ren(&ren) {}
        void Attach(base_ren_type& ren)
        {
            m_ren = &ren;
        }
        
        //--------------------------------------------------------------------
        void color(const color_type& c) { m_color = c; }
        const color_type& color() const { return m_color; }

        //--------------------------------------------------------------------
        void prepare() {}

        //--------------------------------------------------------------------
        template<class Scanline> void render(const Scanline& sl)
        {
            render_scanline_bin_solid(sl, *m_ren, m_color);
        }
        
    private:
        base_ren_type* m_ren;
        color_type m_color;
    };

    //======================================================render_scanline_bin
    template<class Scanline, class BaseRenderer, 
             class SpanAllocator, class SpanGenerator> 
    void render_scanline_bin(const Scanline& sl, BaseRenderer& ren, 
                             SpanAllocator& alloc, SpanGenerator& span_gen)
    {
        int y = sl.y();

        uint num_spans = sl.num_spans();
        typename Scanline::const_iterator span = sl.begin();
        for(;;)
        {
            int x = span->x;
            int len = span->len;
            if(len < 0) len = -len;
            typename BaseRenderer::color_type* colors = alloc.allocate(len);
            span_gen.generate(colors, x, y, len);
            ren.blend_color_hspan(x, y, len, colors, 0, cover_full); 
            if(--num_spans == 0) break;
            ++span;
        }
    }

    //=====================================================render_scanlines_bin
    template<class Rasterizer, class Scanline, class BaseRenderer, 
             class SpanAllocator, class SpanGenerator>
    void render_scanlines_bin(Rasterizer& ras, Scanline& sl, BaseRenderer& ren, 
                              SpanAllocator& alloc, SpanGenerator& span_gen)
    {
        if(ras.rewind_scanlines())
        {
            sl.reset(ras.min_x(), ras.max_x());
            span_gen.prepare();
            while(ras.sweep_scanline(sl))
            {
                render_scanline_bin(sl, ren, alloc, span_gen);
            }
        }
    }

    //====================================================renderer_scanline_bin
    template<class BaseRenderer, class SpanAllocator, class SpanGenerator> 
    class renderer_scanline_bin
    {
    public:
        typedef BaseRenderer  base_ren_type;
        typedef SpanAllocator alloc_type;
        typedef SpanGenerator span_gen_type;

        //--------------------------------------------------------------------
        renderer_scanline_bin() : m_ren(0), m_alloc(0), m_span_gen(0) {}
        renderer_scanline_bin(base_ren_type& ren, 
                              alloc_type& alloc, 
                              span_gen_type& span_gen) :
            m_ren(&ren),
            m_alloc(&alloc),
            m_span_gen(&span_gen)
        {}
        void Attach(base_ren_type& ren, 
                    alloc_type& alloc, 
                    span_gen_type& span_gen)
        {
            m_ren = &ren;
            m_alloc = &alloc;
            m_span_gen = &span_gen;
        }
        
        //--------------------------------------------------------------------
        void prepare() { m_span_gen->prepare(); }

        //--------------------------------------------------------------------
        template<class Scanline> void render(const Scanline& sl)
        {
            render_scanline_bin(sl, *m_ren, *m_alloc, *m_span_gen);
        }

    private:
        base_ren_type* m_ren;
        alloc_type*    m_alloc;
        span_gen_type* m_span_gen;
    };

    //=======================================render_scanlines_compound_layered
    template<class Rasterizer, 
             class ScanlineAA, 
             class BaseRenderer, 
             class SpanAllocator,
             class StyleHandler>
    void render_scanlines_compound_layered(Rasterizer& ras, 
                                           ScanlineAA& sl_aa,
                                           BaseRenderer& ren,
                                           SpanAllocator& alloc,
                                           StyleHandler& sh)
    {
        if(ras.rewind_scanlines())
        {
            int min_x = ras.min_x();
            int len = ras.max_x() - min_x + 2;
            sl_aa.reset(min_x, ras.max_x());

            typedef typename BaseRenderer::color_type color_type;
            color_type* color_span   = alloc.allocate(len * 2);
            color_type* mix_buffer   = color_span + len;
            cover_type* cover_buffer = ras.allocate_cover_buffer(len);
            uint num_spans;

            uint num_styles;
            uint style;
            bool     solid;
            while((num_styles = ras.sweep_styles()) > 0)
            {
                typename ScanlineAA::const_iterator span_aa;
                if(num_styles == 1)
                {
                    // Optimization for a single style. Happens often
                    //-------------------------
                    if(ras.sweep_scanline(sl_aa, 0))
                    {
                        style = ras.style(0);
                        if(sh.is_solid(style))
                        {
                            // Just solid fill
                            //-----------------------
                            render_scanline_aa_solid(sl_aa, ren, sh.color(style));
                        }
                        else
                        {
                            // Arbitrary span generator
                            //-----------------------
                            span_aa   = sl_aa.begin();
                            num_spans = sl_aa.num_spans();
                            for(;;)
                            {
                                len = span_aa->len;
                                sh.generate_span(color_span, 
                                                 span_aa->x, 
                                                 sl_aa.y(), 
                                                 len, 
                                                 style);

                                ren.blend_color_hspan(span_aa->x, 
                                                      sl_aa.y(), 
                                                      span_aa->len,
                                                      color_span,
                                                      span_aa->covers);
                                if(--num_spans == 0) break;
                                ++span_aa;
                            }
                        }
                    }
                }
                else
                {
                    int      sl_start = ras.scanline_start();
                    uint sl_len   = ras.scanline_length();

                    if(sl_len)
                    {
                        MemClear(mix_buffer + sl_start - min_x, 
                               sl_len * sizeof(color_type));

                        MemClear(cover_buffer + sl_start - min_x, 
                               sl_len * sizeof(cover_type));

                        int sl_y = 0x7FFFFFFF;
                        uint i;
                        for(i = 0; i < num_styles; i++)
                        {
                            style = ras.style(i);
                            solid = sh.is_solid(style);

                            if(ras.sweep_scanline(sl_aa, i))
                            {
                                uint    cover;
                                color_type* colors;
                                color_type* cspan;
                                cover_type* src_covers;
                                cover_type* dst_covers;
                                span_aa   = sl_aa.begin();
                                num_spans = sl_aa.num_spans();
                                sl_y      = sl_aa.y();
                                if(solid)
                                {
                                    // Just solid fill
                                    //-----------------------
                                    for(;;)
                                    {
                                        color_type c = sh.color(style);
                                        len    = span_aa->len;
                                        colors = mix_buffer + span_aa->x - min_x;
                                        src_covers = span_aa->covers;
                                        dst_covers = cover_buffer + span_aa->x - min_x;
                                        do
                                        {
                                            cover = *src_covers;
                                            if(*dst_covers + cover > cover_full)
                                            {
                                                cover = cover_full - *dst_covers;
                                            }
                                            if(cover)
                                            {
                                                colors->add(c, cover);
                                                *dst_covers += cover;
                                            }
                                            ++colors;
                                            ++src_covers;
                                            ++dst_covers;
                                        }
                                        while(--len);
                                        if(--num_spans == 0) break;
                                        ++span_aa;
                                    }
                                }
                                else
                                {
                                    // Arbitrary span generator
                                    //-----------------------
                                    for(;;)
                                    {
                                        len = span_aa->len;
                                        colors = mix_buffer + span_aa->x - min_x;
                                        cspan  = color_span;
                                        sh.generate_span(cspan, 
                                                         span_aa->x, 
                                                         sl_aa.y(), 
                                                         len, 
                                                         style);
                                        src_covers = span_aa->covers;
                                        dst_covers = cover_buffer + span_aa->x - min_x;
                                        do
                                        {
                                            cover = *src_covers;
                                            if(*dst_covers + cover > cover_full)
                                            {
                                                cover = cover_full - *dst_covers;
                                            }
                                            if(cover)
                                            {
                                                colors->add(*cspan, cover);
                                                *dst_covers += cover;
                                            }
                                            ++cspan;
                                            ++colors;
                                            ++src_covers;
                                            ++dst_covers;
                                        }
                                        while(--len);
                                        if(--num_spans == 0) break;
                                        ++span_aa;
                                    }
                                }
                            }
                        }
                        ren.blend_color_hspan(sl_start, 
                                              sl_y, 
                                              sl_len,
                                              mix_buffer + sl_start - min_x,
                                              0,
                                              cover_full);
                    } //if(sl_len)
                } //if(num_styles == 1) ... else
            } //while((num_styles = ras.sweep_styles()) > 0)
        } //if(ras.rewind_scanlines())
    }
     */
}
