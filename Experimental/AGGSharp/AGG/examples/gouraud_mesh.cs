//#define SourceDepth24

using System;
using AGG.Color;
using AGG.Gamma;
//using pix_format_e = AGG.UI.PlatformSupportAbstract.PixelFormats;
using AGG.PixelFormat;
using AGG.Rasterizer;
using AGG.Rendering;
using AGG.Scanline;
using AGG.UI;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;

namespace AGG
{
    public class gouraud_mesh_application<T> : AGG.UI.win32.PlatformSupport<T>
          where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public struct mesh_point
        {
            public T x, y;
            public T dx, dy;
            public RGBA_Bytes color;
            public RGBA_Bytes dc;

            public mesh_point(T x_, T y_,
                       T dx_, T dy_,
                       RGBA_Bytes c, RGBA_Bytes dc_)
            {
                x = (x_);
                y = (y_);
                dx = (dx_);
                dy = (dy_);
                color = (c);
                dc = (dc_);
            }
        };

        public struct mesh_triangle
        {
            public uint p1, p2, p3;

            public mesh_triangle(uint i, uint j, uint k)
            {
                p1 = (i);
                p2 = (j);
                p3 = (k);
            }
        };

        public struct mesh_edge
        {
            public uint p1, p2;
            public int tl, tr;

            public mesh_edge(uint p1_, uint p2_, int tl_, int tr_)
            {
                p1 = (p1_);
                p2 = (p2_);
                tl = (tl_);
                tr = (tr_);
            }
        };

        static System.Random rand = new Random();

        static double random(double v1, double v2)
        {
            return (v2 - v1) * (rand.Next() % 1000) / 999.0 + v1;
        }


        public class mesh_ctrl
        {
            int m_cols;
            int m_rows;
            int m_drag_idx;
            T m_drag_dx;
            T m_drag_dy;
            T m_cell_w;
            T m_cell_h;
            T m_start_x;
            T m_start_y;
            VectorPOD<mesh_point> m_vertices = new VectorPOD<mesh_point>();
            VectorPOD<mesh_triangle> m_triangles = new VectorPOD<mesh_triangle>();
            VectorPOD<mesh_edge> m_edges = new VectorPOD<mesh_edge>();

            public mesh_ctrl()
            {
                m_cols = (0);
                m_rows = (0);
                m_drag_idx = (-1);
                m_drag_dx = M.Zero<T>();
                m_drag_dy = M.Zero<T>();
            }

            public void generate(int cols, int rows,
                                    double cell_w, double cell_h,
                                    double start_x, double start_y)
            {
                generate(cols, rows, M.New<T>(cell_w), M.New<T>(cell_h), M.New<T>(start_x), M.New<T>(start_y));
            }

            public void generate(int cols, int rows,
                                     T cell_w, T cell_h,
                                     T start_x, T start_y)
            {
                m_cols = cols;
                m_rows = rows;
                m_cell_w = cell_w;
                m_cell_h = cell_h;
                m_start_x = start_x;
                m_start_y = start_y;

                m_vertices.RemoveAll();
                for (int i = 0; i < m_rows; i++)
                {
                    T x = start_x;
                    for (int j = 0; j < m_cols; j++)
                    {
                        T dx = M.New<T>(random(-0.5, 0.5));
                        T dy = M.New<T>(random(-0.5, 0.5));
                        RGBA_Bytes c = new RGBA_Bytes(rand.Next() & 0xFF, rand.Next() & 0xFF, rand.Next() & 0xFF);
                        RGBA_Bytes dc = new RGBA_Bytes(rand.Next() & 1, rand.Next() & 1, rand.Next() & 1);
                        m_vertices.Add(new mesh_point(x, start_y, dx, dy, c, dc));
                        x.AddEquals(cell_w);
                    }
                    start_y.AddEquals(cell_h);
                }



                //  4---3
                //  |t2/|
                //  | / |
                //  |/t1|
                //  1---2
                m_triangles.RemoveAll();
                m_edges.RemoveAll();
                for (int i = 0; i < m_rows - 1; i++)
                {
                    for (int j = 0; j < m_cols - 1; j++)
                    {
                        int p1 = i * m_cols + j;
                        int p2 = p1 + 1;
                        int p3 = p2 + m_cols;
                        int p4 = p1 + m_cols;
                        m_triangles.Add(new mesh_triangle((uint)p1, (uint)p2, (uint)p3));
                        m_triangles.Add(new mesh_triangle((uint)p3, (uint)p4, (uint)p1));

                        int curr_cell = i * (m_cols - 1) + j;
                        int left_cell = j != 0 ? (int)(curr_cell - 1) : -1;
                        int bott_cell = i != 0 ? (int)(curr_cell - (m_cols - 1)) : -1;

                        int curr_t1 = curr_cell * 2;
                        int curr_t2 = curr_t1 + 1;

                        int left_t1 = (left_cell >= 0) ? left_cell * 2 : -1;
                        int left_t2 = (left_cell >= 0) ? left_t1 + 1 : -1;

                        int bott_t1 = (bott_cell >= 0) ? bott_cell * 2 : -1;
                        int bott_t2 = (bott_cell >= 0) ? bott_t1 + 1 : -1;

                        m_edges.Add(new mesh_edge((uint)p1, (uint)p2, curr_t1, bott_t2));
                        m_edges.Add(new mesh_edge((uint)p1, (uint)p3, curr_t2, curr_t1));
                        m_edges.Add(new mesh_edge((uint)p1, (uint)p4, left_t1, curr_t2));

                        if (j == m_cols - 2) // Last column
                        {
                            m_edges.Add(new mesh_edge((uint)p2, (uint)p3, curr_t1, -1));
                        }

                        if (i == m_rows - 2) // Last row
                        {
                            m_edges.Add(new mesh_edge((uint)p3, (uint)p4, curr_t2, -1));
                        }
                    }
                }
            }

            public void randomize_points(double delta)
            {
                randomize_points(M.New<T>(delta));
            }

            public void randomize_points(T delta)
            {
                uint i, j;
                for (i = 0; i < m_rows; i++)
                {
                    for (j = 0; j < m_cols; j++)
                    {
                        T xc = m_cell_w.Multiply(j).Add(m_start_x);
                        T yc = m_cell_h.Multiply(i).Add(m_start_y);
                        T x1 = xc.Subtract(m_cell_w.Divide(4));
                        T y1 = yc.Subtract(m_cell_h.Divide(4));
                        T x2 = xc.Add(m_cell_w.Divide(4));
                        T y2 = yc.Add(m_cell_h.Divide(40));
                        mesh_point p = vertex(j, i);
                        p.x.AddEquals(p.dx);
                        p.y.AddEquals(p.dy);
                        if (p.x.LessThan(x1)) { p.x = x1; p.dx = p.dx.Negative(); }
                        if (p.y.LessThan(y1)) { p.y = y1; p.dy = p.dy.Negative(); }
                        if (p.x.GreaterThan(x2)) { p.x = x2; p.dx = p.dx.Negative(); }
                        if (p.y.GreaterThan(y2)) { p.y = y2; p.dy = p.dy.Negative(); }
                    }
                }
            }


            public void rotate_colors()
            {
                uint i;
                for (i = 1; i < m_vertices.Size(); i++)
                {
                    RGBA_Bytes c = m_vertices[i].color;
                    RGBA_Bytes dc = m_vertices[i].dc;

                    int r = (int)c.R_Byte + (dc.R_Byte != 0 ? 5 : -5);
                    int g = (int)c.G_Byte + (dc.G_Byte != 0 ? 5 : -5);
                    int b = (int)c.B_Byte + (dc.B_Byte != 0 ? 5 : -5);

                    uint dcr = dc.R_Byte, dcg = dc.G_Byte, dcb = dc.B_Byte, dca = dc.A_Byte;


                    if (r < 0)
                    {
                        r = 0;
                        dcr = dc.R_Byte ^ 1;
                    }
                    if (r > 255)
                    {
                        r = 255;
                        dcr = dc.R_Byte ^ 1;
                    }
                    if (g < 0)
                    {
                        g = 0;
                        dcg = dc.G_Byte ^ 1;
                    }
                    if (g > 255)
                    {
                        g = 255;
                        dcg = dc.G_Byte ^ 1;
                    }
                    if (b < 0)
                    {
                        b = 0;
                        dcb = dc.B_Byte ^ 1;
                    }
                    if (b > 255)
                    {
                        b = 255;
                        dcb = dc.B_Byte ^ 1;
                    }

                    dc = new RGBA_Bytes(dcr, dcg, dcb, dca);


                    //if (r < 0)
                    //{
                    //    r = 0;
                    //    dc.R_Byte ^= 1;
                    //}
                    //if (r > 255)
                    //{
                    //    r = 255;
                    //    dc.R_Byte ^= 1;
                    //}
                    //if (g < 0)
                    //{
                    //    g = 0;
                    //    dc.G_Byte ^= 1;
                    //}
                    //if (g > 255)
                    //{
                    //    g = 255;
                    //    dc.G_Byte ^= 1;
                    //}
                    //if (b < 0)
                    //{
                    //    b = 0;
                    //    dc.B_Byte ^= 1;
                    //}
                    //if (b > 255)
                    //{
                    //    b = 255;
                    //    dc.B_Byte ^= 1;
                    //}


                    //c.R_Byte = (uint)r;
                    //c.G_Byte = (uint)g;
                    //c.B_Byte = (uint)b;

                    c = new RGBA_Bytes((uint)r, (uint)g, (uint)b, c.A_Byte);
                }
            }


            public bool OnMouseDown(MouseEventArgs mouseEvent)
            {
                T x = M.New<T>(mouseEvent.X);
                T y = M.New<T>(mouseEvent.Y);
                if (mouseEvent.Button == MouseButtons.Left)
                {
                    int i;
                    for (i = 0; i < m_vertices.Size(); i++)
                    {
                        if (MathUtil.CalcDistance(x, y, m_vertices[i].x, m_vertices[i].y).LessThan(5))
                        {
                            m_drag_idx = i;
                            m_drag_dx = x.Subtract(m_vertices[i].x);
                            m_drag_dy = y.Subtract(m_vertices[i].y);
                            return true;
                        }
                    }
                }
                return false;
            }

            public bool OnMouseMove(MouseEventArgs mouseEvent)
            {
                T x = M.New<T>(mouseEvent.X);
                T y = M.New<T>(mouseEvent.Y);
                if (mouseEvent.Button == MouseButtons.Left)
                {
                    if (m_drag_idx >= 0)
                    {
                        m_vertices.Array[m_drag_idx].x = x.Subtract(m_drag_dx);
                        m_vertices.Array[m_drag_idx].y = y.Subtract(m_drag_dy);
                        return true;
                    }
                }

                return false;
            }

            public bool OnMouseUp(MouseEventArgs mouseEvent)
            {
                bool ret = m_drag_idx >= 0;
                m_drag_idx = -1;
                return ret;
            }

            public uint num_vertices() { return m_vertices.Size(); }
            public mesh_point vertex(uint i) { return m_vertices[i]; }

            public mesh_point vertex(uint x, uint y) { return m_vertices[(int)y * m_rows + (int)x]; }

            public uint num_triangles() { return m_triangles.Size(); }
            public mesh_triangle triangle(uint i) { return m_triangles[i]; }

            public uint num_edges() { return m_edges.Size(); }
            public mesh_edge edge(uint i) { return m_edges[i]; }
        };

        public class styles_gouraud : IStyleHandler
        {
            VectorPOD<SpanGouraudRgba<T>> m_triangles = new VectorPOD<SpanGouraudRgba<T>>();

            public styles_gouraud(mesh_ctrl mesh, GammaLut gamma)
            {
                uint i;
                for (i = 0; i < mesh.num_triangles(); i++)
                {
                    mesh_triangle t = mesh.triangle(i);
                    mesh_point p1 = mesh.vertex(t.p1);
                    mesh_point p2 = mesh.vertex(t.p2);
                    mesh_point p3 = mesh.vertex(t.p3);

                    RGBA_Bytes c1 = p1.color;
                    RGBA_Bytes c2 = p2.color;
                    RGBA_Bytes c3 = p3.color;
                    c1.ApplyGammaDir(gamma);
                    c2.ApplyGammaDir(gamma);
                    c3.ApplyGammaDir(gamma);
                    SpanGouraudRgba<T> gouraud = new SpanGouraudRgba<T>(c1, c2, c3,
                                         p1.x, p1.y,
                                         p2.x, p2.y,
                                         p3.x, p3.y);
                    gouraud.Prepare();
                    m_triangles.Add(gouraud);
                }
            }

            public bool IsSolid(uint style) { return false; }

            public RGBA_Bytes Color(uint style) { return new RGBA_Bytes(0, 0, 0, 0); }

            public unsafe void GenerateSpan(RGBA_Bytes* span, int x, int y, uint len, uint style)
            {
                m_triangles[style].Generate(span, x, y, len);
            }
        };

        mesh_ctrl m_mesh = new mesh_ctrl();
        GammaLut m_gamma = new GammaLut();


        gouraud_mesh_application(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            //        m_gamma.gamma(2.0);
        }

        public override void OnInitialize()
        {
            m_mesh.generate(20, 20, 17, 17, 40, 40);
        }


        public override void OnDraw()
        {
#if SourceDepth24
            pixfmt_alpha_blend_rgb pf = new pixfmt_alpha_blend_rgb(rbuf_window(), new blender_bgr());
#else
            FormatRGBA pf = new FormatRGBA(rbuf_window(), new BlenderBGRA());
#endif
            FormatClippingProxy clippingProxy = new FormatClippingProxy(pf);
            clippingProxy.Clear(new RGBA_Doubles(0, 0, 0));

            RasterizerScanlineAA<T> ras = new RasterizerScanlineAA<T>();
            ScanlineUnpacked8 sl = new ScanlineUnpacked8();
            ScanlineBin sl_bin = new ScanlineBin();

            RasterizerCompoundAA<T> rasc = new RasterizerCompoundAA<T>();
            SpanAllocator alloc = new SpanAllocator();

            uint i;
            styles_gouraud styles = new styles_gouraud(m_mesh, m_gamma);
            start_timer();
            rasc.Reset();
            //rasc.clip_box(40, 40, width() - 40, height() - 40);
            for (i = 0; i < m_mesh.num_edges(); i++)
            {
                mesh_edge e = m_mesh.edge(i);
                mesh_point p1 = m_mesh.vertex(e.p1);
                mesh_point p2 = m_mesh.vertex(e.p2);
                rasc.Styles(e.tl, e.tr);
                rasc.MoveToDbl(p1.x, p1.y);
                rasc.LineToDbl(p2.x, p2.y);
            }

            Renderer<T>.RenderCompound(rasc, sl, sl_bin, clippingProxy, alloc, styles);
            double tm = elapsed_time();

            GsvText<T> t = new GsvText<T>();
            t.SetFontSize(10.0);

            ConvStroke<T> pt = new ConvStroke<T>(t);
            pt.Width = M.New<T>(1.5);
            pt.LineCap = LineCap.Round;
            pt.LineJoin = LineJoin.RoundJoin;

            string buf = string.Format("{0:F2} ms, {1} triangles, {2:F0} tri/sec",
                tm,
                m_mesh.num_triangles(),
                m_mesh.num_triangles() / tm * 1000.0);
            t.StartPoint(10.0, 10.0);
            t.Text = buf;

            ras.AddPath(pt);
            Renderer<T>.RenderSolid(clippingProxy, ras, sl, new RGBA_Bytes(1, 1, 1).GetAsRGBA_Bytes());


            if (m_gamma.Gamma() != 1.0)
            {
                pf.ApplyGammaInv(m_gamma);
            }
        }

        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
            if (m_mesh.OnMouseMove(mouseEvent))
            {
                force_redraw();
            }

            base.OnMouseMove(mouseEvent);
        }

        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            if (m_mesh.OnMouseDown(mouseEvent))
            {
                force_redraw();
                mouseEvent.Handled = true;
            }

            base.OnMouseDown(mouseEvent);
        }

        public override void OnMouseUp(MouseEventArgs mouseEvent)
        {
            if (m_mesh.OnMouseUp(mouseEvent))
            {
                force_redraw();
            }

            base.OnMouseUp(mouseEvent);
        }

        public override void OnIdle()
        {
            m_mesh.randomize_points(1.0);
            m_mesh.rotate_colors();
            force_redraw();
        }

        public override void OnControlChanged()
        {
        }

        public static void StartDemo()
        {
#if SourceDepth24
            gouraud_mesh_application app = new gouraud_mesh_application(pix_format_e.pix_format_rgb24, agg.ui.platform_support_abstract.ERenderOrigin.OriginBottomLeft);
#else
            gouraud_mesh_application<T> app = new gouraud_mesh_application<T>(PixelFormats.pix_format_rgba32, ERenderOrigin.OriginBottomLeft);
#endif
            app.Caption = "AGG Example. Seemless Gouraud mesh Shading";

            if (app.init(400, 400, 0))//, (uint)window_flag_e.window_resize))
            {
                app.wait_mode(false);
                app.run();
            }
        }


    };

    public static class App
    {
        [STAThread]
        public static void Main(string[] args)
        {
            gouraud_mesh_application<DoubleComponent>.StartDemo();
        }
    }
}

