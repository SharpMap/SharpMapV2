using System;
using AGG.Color;
using AGG.PixelFormat;
using AGG.Rasterizer;
using AGG.Rendering;
using AGG.Scanline;
//using pix_format_e = AGG.UI.PlatformSupportAbstract.PixelFormats;
using AGG.Transform;
using AGG.UI;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;

namespace AGG
{
    public class perspective_application<T> : AGG.UI.win32.PlatformSupport<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        RasterizerScanlineAA<T> g_rasterizer = new RasterizerScanlineAA<T>();
        ScanlinePacked8 g_scanline = new ScanlinePacked8();
        PathStorage<T> g_path = new PathStorage<T>();
        RGBA_Bytes[] g_colors = new RGBA_Bytes[100];
        uint[] g_path_idx = new uint[100];
        uint g_npaths = 0;
        T g_x1 = M.Zero<T>();
        T g_y1 = M.Zero<T>();
        T g_x2 = M.Zero<T>();
        T g_y2 = M.Zero<T>();
        T g_base_dx = M.Zero<T>();
        T g_base_dy = M.Zero<T>();

        UI.polygon_ctrl<T> m_quad;
        UI.rbox_ctrl<T> m_trans_type;

        public void parse_lion()
        {
            g_npaths = AGG.samples.LionParser.parse_lion(g_path, g_colors, g_path_idx);
            AGG.BoundingRect<T>.GetBoundingRect(g_path, g_path_idx, 0, g_npaths, out g_x1, out g_y1, out g_x2, out g_y2);
            g_base_dx = g_x2.Subtract(g_x1).Divide(2.0);
            g_base_dy = g_y2.Subtract(g_y1).Divide(2.0);
            g_path.FlipX(g_x1, g_x2);
            g_path.FlipY(g_y1, g_y2);
        }

        //typedef agg.renderer_base<pixfmt> renderer_base;
        //typedef agg.renderer_scanline_aa_solid<renderer_base> renderer_solid;

        public perspective_application(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            parse_lion();

            m_quad = new AGG.UI.polygon_ctrl<T>(4, 5.0);
            m_trans_type = new AGG.UI.rbox_ctrl<T>(420, 5.0, 420 + 130.0, 55.0);
            m_quad.SetXN(0, g_x1);
            m_quad.SetYN(0, g_y1);
            m_quad.SetXN(1, g_x2);
            m_quad.SetYN(1, g_y1);
            m_quad.SetXN(2, g_x2);
            m_quad.SetYN(2, g_y2);
            m_quad.SetXN(3, g_x1);
            m_quad.SetYN(3, g_y2);

            m_trans_type.add_item("Bilinear");
            m_trans_type.add_item("Perspective");
            m_trans_type.cur_item(0);
            AddChild(m_trans_type);
        }

        public override void OnInitialize()
        {
            T dx = width().Divide(2.0).Subtract(m_quad.xn(1).Subtract(m_quad.xn(0)).Divide(2.0));
            T dy = height().Divide(2.0).Subtract(m_quad.yn(2).Subtract(m_quad.yn(0)).Divide(2.0));
            m_quad.AddXN(0, dx);
            m_quad.AddYN(0, dy);
            m_quad.AddXN(1, dx);
            m_quad.AddYN(1, dy);
            m_quad.AddXN(2, dx);
            m_quad.AddYN(2, dy);
            m_quad.AddXN(3, dx);
            m_quad.AddYN(3, dy);

            base.OnInitialize();
        }

        public override void OnDraw()
        {
            IPixelFormat pixf;
            if (this.bpp() == 32)
            {
                pixf = new FormatRGBA(rbuf_window(), new BlenderBGRA());
                //pixf = new pixfmt_alpha_blend_rgba32(rbuf_window(), new blender_rgba32());
            }
            else
            {
                if (bpp() != 24)
                {
                    throw new System.NotSupportedException();
                }
                pixf = new FormatRGB(rbuf_window(), new BlenderBGR());
            }
            FormatClippingProxy clippingProxy = new FormatClippingProxy(pixf);
            clippingProxy.Clear(new RGBA_Doubles(1, 1, 1));

            g_rasterizer.SetVectorClipBox(M.Zero<T>(), M.Zero<T>(), width(), height());

            if (m_trans_type.cur_item() == 0)
            {
                Bilinear<T> tr = new Bilinear<T>(g_x1, g_y1, g_x2, g_y2, m_quad.polygon());
                if (tr.IsValid())
                {
                    //--------------------------
                    // Render transformed lion
                    //
                    ConvTransform<T> trans = new ConvTransform<T>(g_path, tr);

                    Renderer<T>.RenderSolidAllPaths(clippingProxy, g_rasterizer, g_scanline, trans, g_colors, g_path_idx, g_npaths);
                    //--------------------------



                    //--------------------------
                    // Render transformed ellipse
                    //
                    VertexSource.Ellipse<T> ell = new AGG.VertexSource.Ellipse<T>(g_x1.Add(g_x2).Multiply(0.5), g_y1.Add(g_y2).Multiply(0.5),
                                     g_x2.Subtract(g_x1).Multiply(0.5), g_y2.Subtract(g_y1).Multiply(0.5),
                                     200);
                    ConvStroke<T> ell_stroke = new ConvStroke<T>(ell);
                    ell_stroke.Width = M.New<T>(3.0);
                    ConvTransform<T> trans_ell = new ConvTransform<T>(ell, tr);

                    ConvTransform<T> trans_ell_stroke = new ConvTransform<T>(ell_stroke, tr);

                    g_rasterizer.AddPath(trans_ell);
                    Renderer<T>.RenderSolid(clippingProxy, g_rasterizer, g_scanline, new RGBA_Bytes(0.5, 0.3, 0.0, 0.3));

                    g_rasterizer.AddPath(trans_ell_stroke);
                    Renderer<T>.RenderSolid(clippingProxy, g_rasterizer, g_scanline, new RGBA_Bytes(0.0, 0.3, 0.2, 1.0));
                }
            }
            else
            {
                Perspective<T> tr = new Perspective<T>(g_x1, g_y1, g_x2, g_y2, m_quad.polygon());
                if (tr.IsValid())
                {
                    // Render transformed lion
                    ConvTransform<T> trans = new ConvTransform<T>(g_path, tr);

                    Renderer<T>.RenderSolidAllPaths(clippingProxy, g_rasterizer, g_scanline, trans, g_colors, g_path_idx, g_npaths);

                    // Render transformed ellipse
                    VertexSource.Ellipse<T> FilledEllipse = new AGG.VertexSource.Ellipse<T>(g_x1.Add(g_x2).Multiply(0.5), g_y1.Add(g_y2).Multiply(0.5),
                                     g_x2.Subtract(g_x1).Multiply(0.5), g_y2.Subtract(g_y1).Multiply(0.5),
                                     200);

                    ConvStroke<T> EllipseOutline = new ConvStroke<T>(FilledEllipse);
                    EllipseOutline.Width = M.New<T>(3.0);
                    ConvTransform<T> TransformedFilledEllipse = new ConvTransform<T>(FilledEllipse, tr);

                    ConvTransform<T> TransformedEllipesOutline = new ConvTransform<T>(EllipseOutline, tr);

                    g_rasterizer.AddPath(TransformedFilledEllipse);
                    Renderer<T>.RenderSolid(clippingProxy, g_rasterizer, g_scanline, new RGBA_Bytes(0.5, 0.3, 0.0, 0.3));

                    g_rasterizer.AddPath(TransformedEllipesOutline);
                    Renderer<T>.RenderSolid(clippingProxy, g_rasterizer, g_scanline, new RGBA_Bytes(0.0, 0.3, 0.2, 1.0));
                }
            }

            //--------------------------
            // Render the "quad" tool and controls
            g_rasterizer.AddPath(m_quad);
            Renderer<T>.RenderSolid(clippingProxy, g_rasterizer, g_scanline, new RGBA_Bytes(0, 0.3, 0.5, 0.6));
            //m_trans_type.Render(g_rasterizer, g_scanline, clippingProxy);
            base.OnDraw();
        }

        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            if (mouseEvent.Button == MouseButtons.Left)
            {
                m_quad.OnMouseDown(mouseEvent);
                if (mouseEvent.Handled)
                {
                    force_redraw();
                }
            }

            base.OnMouseDown(mouseEvent);
        }

        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
            if (mouseEvent.Button == MouseButtons.Left)
            {
                m_quad.OnMouseMove(mouseEvent);
                if (mouseEvent.Handled)
                {
                    force_redraw();
                }
            }

            base.OnMouseMove(mouseEvent);
        }

        public override void OnMouseUp(MouseEventArgs mouseEvent)
        {
            m_quad.OnMouseUp(mouseEvent);
            if (mouseEvent.Handled)
            {
                force_redraw();
            }

            base.OnMouseUp(mouseEvent);
        }

        public static void StartDemo()
        {
            perspective_application<T> app = new perspective_application<T>(PixelFormats.pix_format_bgr24, ERenderOrigin.OriginBottomLeft);
            //lion_application app = new lion_application(pix_format_e.pix_format_bgra32, agg.ui.platform_support_abstract.ERenderOrigin.OriginBottomLeft);
            //lion_application app = new lion_application(pix_format_e.pix_format_rgba32, agg.ui.platform_support_abstract.ERenderOrigin.OriginBottomLeft);
            app.Caption = "AGG Example. Perspective Transformations";

            if (app.init(600, 600, (uint)WindowFlags.Risizeable))
            {
                app.run();
            }
        }


    }

    public static class App
    {
        [STAThread]
        public static void Main(string[] args)
        {
            perspective_application<DoubleComponent>.StartDemo();
        }
    }
};

