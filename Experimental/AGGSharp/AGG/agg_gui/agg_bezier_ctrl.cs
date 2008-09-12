//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
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
//
// classes bezier_ctrl_impl, bezier_ctrl
//
//----------------------------------------------------------------------------
using System;
using AGG.Color;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;
namespace AGG.UI
{
    //--------------------------------------------------------bezier_ctrl_impl
    public class bezier_ctrl_impl<T> : SimpleVertexSourceWidget<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        Curve4<T> m_curve = new Curve4<T>();
        Ellipse<T> m_ellipse;
        ConvStroke<T> m_stroke;
        polygon_ctrl_impl<T> m_poly;
        uint m_idx;

        public bezier_ctrl_impl()
            :
            base(M.Zero<T>(), M.Zero<T>(), M.One<T>(), M.One<T>())
        {
            m_stroke = new ConvStroke<T>(m_curve);
            m_poly = new polygon_ctrl_impl<T>(4, M.New<T>(5.0));
            m_idx = (0);
            m_ellipse = new Ellipse<T>();

            m_poly.in_polygon_check(false);
            m_poly.SetXN(0, M.New<T>(100.0));
            m_poly.SetYN(0, M.Zero<T>());
            m_poly.SetXN(1, M.New<T>(100.0));
            m_poly.SetYN(1, M.New<T>(50.0));
            m_poly.SetXN(2, M.New<T>(50.0));
            m_poly.SetYN(2, M.New<T>(100.0));
            m_poly.SetXN(3, M.Zero<T>());
            m_poly.SetYN(3, M.New<T>(100.0));
        }

        public void curve(T x1, T y1,
                                      T x2, T y2,
                                      T x3, T y3,
                                      T x4, T y4)
        {
            m_poly.SetXN(0, x1);
            m_poly.SetYN(0, y1);
            m_poly.SetXN(1, x2);
            m_poly.SetYN(1, y2);
            m_poly.SetXN(2, x3);
            m_poly.SetYN(2, y3);
            m_poly.SetXN(3, x4);
            m_poly.SetYN(3, y4);
            curve();
        }

        public Curve4<T> curve()
        {
            m_curve.Init(m_poly.xn(0), m_poly.yn(0),
                         m_poly.xn(1), m_poly.yn(1),
                         m_poly.xn(2), m_poly.yn(2),
                         m_poly.xn(3), m_poly.yn(3));
            return m_curve;
        }

        public T x1() { return m_poly.xn(0); }
        public T y1() { return m_poly.yn(0); }
        public T x2() { return m_poly.xn(1); }
        public T y2() { return m_poly.yn(1); }
        public T x3() { return m_poly.xn(2); }
        public T y3() { return m_poly.yn(2); }
        public T x4() { return m_poly.xn(3); }
        public T y4() { return m_poly.yn(3); }

        public void x1(T x) { m_poly.SetXN(0, x); }
        public void y1(T y) { m_poly.SetYN(0, y); }
        public void x2(T x) { m_poly.SetXN(1, x); }
        public void y2(T y) { m_poly.SetYN(1, y); }
        public void x3(T x) { m_poly.SetXN(2, x); }
        public void y3(T y) { m_poly.SetYN(2, y); }
        public void x4(T x) { m_poly.SetXN(3, x); }
        public void y4(T y) { m_poly.SetYN(3, y); }

        public void line_width(T w) { m_stroke.Width = w; }
        public T line_width() { return m_stroke.Width; }

        public void point_radius(T r) { m_poly.point_radius(r); }
        public T point_radius() { return m_poly.point_radius(); }

        public override bool InRect(T x, T y)
        {
            return false;
        }

        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            GetTransform().Inverse.Transform(ref x, ref y);
            m_poly.OnMouseDown(new MouseEventArgs(mouseEvent, x.ToDouble(), y.ToDouble()));
        }

        public override void OnMouseUp(MouseEventArgs mouseEvent)
        {
            double x = mouseEvent.X;
            double y = mouseEvent.Y;
            m_poly.OnMouseUp(new MouseEventArgs(mouseEvent, x, y));
        }

        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            GetTransform().Inverse.Transform(ref x, ref y);
            m_poly.OnMouseMove(new MouseEventArgs(mouseEvent, x.ToDouble(), y.ToDouble()));
        }

        public override void OnKeyDown(KeyEventArgs keyEvent)
        {
            m_poly.OnKeyDown(keyEvent);
        }

        // Vertex source interface
        public override uint num_paths() { return 7; }
        public override void Rewind(uint idx)
        {
            m_idx = idx;

            m_curve.ApproximationScale = base.scale();
            switch (idx)
            {
                default:
                case 0:                 // Control line 1
                    m_curve.Init(m_poly.xn(0), m_poly.yn(0),
                                m_poly.xn(0).Add(m_poly.xn(1)).Multiply(0.5),
                                m_poly.yn(0).Add(m_poly.yn(1)).Multiply(0.5),
                                m_poly.xn(0).Add(m_poly.xn(1)).Multiply(0.5),
                                m_poly.yn(0).Add(m_poly.yn(1)).Multiply(0.5),
                                 m_poly.xn(1), m_poly.yn(1));
                    m_stroke.Rewind(0);
                    break;

                case 1:                 // Control line 2
                    m_curve.Init(m_poly.xn(2), m_poly.yn(2),
                                m_poly.xn(2).Add(m_poly.xn(3)).Multiply(0.5),
                                m_poly.yn(2).Add(m_poly.yn(3)).Multiply(0.5),
                                m_poly.xn(2).Add(m_poly.xn(3)).Multiply(0.5),
                                m_poly.yn(2).Add(m_poly.yn(3)).Multiply(0.5),
                                 m_poly.xn(3), m_poly.yn(3));
                    m_stroke.Rewind(0);
                    break;

                case 2:                 // Curve itself
                    m_curve.Init(m_poly.xn(0), m_poly.yn(0),
                                 m_poly.xn(1), m_poly.yn(1),
                                 m_poly.xn(2), m_poly.yn(2),
                                 m_poly.xn(3), m_poly.yn(3));
                    m_stroke.Rewind(0);
                    break;

                case 3:                 // Point 1
                    m_ellipse.Init(m_poly.xn(0), m_poly.yn(0), point_radius(), point_radius(), 20);
                    m_ellipse.Rewind(0);
                    break;

                case 4:                 // Point 2
                    m_ellipse.Init(m_poly.xn(1), m_poly.yn(1), point_radius(), point_radius(), 20);
                    m_ellipse.Rewind(0);
                    break;

                case 5:                 // Point 3
                    m_ellipse.Init(m_poly.xn(2), m_poly.yn(2), point_radius(), point_radius(), 20);
                    m_ellipse.Rewind(0);
                    break;

                case 6:                 // Point 4
                    m_ellipse.Init(m_poly.xn(3), m_poly.yn(3), point_radius(), point_radius(), 20);
                    m_ellipse.Rewind(0);
                    break;
            }
        }
        public override uint Vertex(out T x, out T y)
        {
            x = M.Zero<T>();
            y = M.Zero<T>();
            uint cmd = (uint)Path.Commands.Stop;
            switch (m_idx)
            {
                case 0:
                case 1:
                case 2:
                    cmd = m_stroke.Vertex(out x, out y);
                    break;

                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                    cmd = m_ellipse.Vertex(out x, out y);
                    break;
            }

            if (!Path.IsStop(cmd))
            {
                GetTransform().Transform(ref x, ref y);
            }
            return cmd;
        }
    };



    //----------------------------------------------------------bezier_ctrl
    //template<class IColorType> 
    public class bezier_ctrl<T> : bezier_ctrl_impl<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        RGBA_Doubles m_color;

        public bezier_ctrl()
        {
            m_color = new RGBA_Doubles(0.0, 0.0, 0.0);
        }

        public void line_color(IColorType c) { m_color = c.GetAsRGBA_Doubles(); }
        public override IColorType color(uint i) { return m_color; }
    };


    //--------------------------------------------------------curve3_ctrl_impl
    public class curve3_ctrl_impl<T> : SimpleVertexSourceWidget<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        Curve3<T> m_curve;
        Ellipse<T> m_ellipse;
        ConvStroke<T> m_stroke;
        polygon_ctrl_impl<T> m_poly;
        uint m_idx;

        public curve3_ctrl_impl()
            :
            base(M.Zero<T>(), M.Zero<T>(), M.One<T>(), M.One<T>())
        {
            m_stroke = new ConvStroke<T>(m_curve);
            m_poly = new polygon_ctrl_impl<T>(3, M.New<T>(5.0));
            m_idx = 0;
            m_curve = new Curve3<T>();
            m_ellipse = new Ellipse<T>();

            m_poly.in_polygon_check(false);
            m_poly.SetXN(0, M.New<T>(100.0));
            m_poly.SetYN(0, M.Zero<T>());
            m_poly.SetXN(1, M.New<T>(100.0));
            m_poly.SetYN(1, M.New<T>(50.0));
            m_poly.SetXN(2, M.New<T>(50.0));
            m_poly.SetYN(2, M.New<T>(100.0));
        }

        public void curve(T x1, T y1,
                                     T x2, T y2,
                                     T x3, T y3)
        {
            m_poly.SetXN(0, x1);
            m_poly.SetYN(0, y1);
            m_poly.SetXN(1, x2);
            m_poly.SetYN(1, y2);
            m_poly.SetXN(2, x3);
            m_poly.SetYN(2, y3);
            curve();
        }

        public Curve3<T> curve()
        {
            m_curve.Init(m_poly.xn(0), m_poly.yn(0),
                         m_poly.xn(1), m_poly.yn(1),
                         m_poly.xn(2), m_poly.yn(2));
            return m_curve;
        }

        T x1() { return m_poly.xn(0); }
        T y1() { return m_poly.yn(0); }
        T x2() { return m_poly.xn(1); }
        T y2() { return m_poly.yn(1); }
        T x3() { return m_poly.xn(2); }
        T y3() { return m_poly.yn(2); }

        void x1(T x) { m_poly.SetXN(0, x); }
        void y1(T y) { m_poly.SetYN(0, y); }
        void x2(T x) { m_poly.SetXN(1, x); }
        void y2(T y) { m_poly.SetYN(1, y); }
        void x3(T x) { m_poly.SetXN(2, x); }
        void y3(T y) { m_poly.SetYN(2, y); }

        void line_width(T w) { m_stroke.Width = w; }
        T line_width() { return m_stroke.Width; }

        void point_radius(T r) { m_poly.point_radius(r); }
        T point_radius() { return m_poly.point_radius(); }

        public override bool InRect(T x, T y)
        {
            return false;
        }

        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            GetTransform().Inverse.Transform(ref x, ref y);
            m_poly.OnMouseDown(new MouseEventArgs(mouseEvent, x.ToDouble(), y.ToDouble()));
        }

        public override void OnMouseUp(MouseEventArgs mouseEvent)
        {
            double x = mouseEvent.X;
            double y = mouseEvent.Y;
            m_poly.OnMouseUp(new MouseEventArgs(mouseEvent, x, y));
        }

        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            GetTransform().Inverse.Transform(ref x, ref y);
            m_poly.OnMouseMove(new MouseEventArgs(mouseEvent, x.ToDouble(), y.ToDouble()));
        }

        public override void OnKeyDown(KeyEventArgs keyEvent)
        {
            m_poly.OnKeyDown(keyEvent);
        }

        // Vertex source interface
        public override uint num_paths() { return 6; }

        public override void Rewind(uint idx)
        {
            m_idx = idx;

            switch (idx)
            {
                default:
                case 0:                 // Control line
                    m_curve.Init(m_poly.xn(0), m_poly.yn(0),
                                m_poly.xn(0).Add(m_poly.xn(1)).Multiply(0.5),
                                m_poly.yn(0).Add(m_poly.yn(1)).Multiply(0.5),
                                 m_poly.xn(1), m_poly.yn(1));
                    m_stroke.Rewind(0);
                    break;

                case 1:                 // Control line 2
                    m_curve.Init(m_poly.xn(1), m_poly.yn(1),
                                m_poly.xn(1).Add(m_poly.xn(2)).Multiply(0.5),
                                m_poly.yn(1).Add(m_poly.yn(2)).Multiply(0.5),
                                 m_poly.xn(2), m_poly.yn(2));
                    m_stroke.Rewind(0);
                    break;

                case 2:                 // Curve itself
                    m_curve.Init(m_poly.xn(0), m_poly.yn(0),
                                 m_poly.xn(1), m_poly.yn(1),
                                 m_poly.xn(2), m_poly.yn(2));
                    m_stroke.Rewind(0);
                    break;

                case 3:                 // Point 1
                    m_ellipse.Init(m_poly.xn(0), m_poly.yn(0), point_radius(), point_radius(), 20);
                    m_ellipse.Rewind(0);
                    break;

                case 4:                 // Point 2
                    m_ellipse.Init(m_poly.xn(1), m_poly.yn(1), point_radius(), point_radius(), 20);
                    m_ellipse.Rewind(0);
                    break;

                case 5:                 // Point 3
                    m_ellipse.Init(m_poly.xn(2), m_poly.yn(2), point_radius(), point_radius(), 20);
                    m_ellipse.Rewind(0);
                    break;
            }
        }

        public override uint Vertex(out T x, out T y)
        {
            x = M.Zero<T>();
            y = M.Zero<T>();
            uint cmd = (uint)Path.Commands.Stop;
            switch (m_idx)
            {
                case 0:
                case 1:
                case 2:
                    cmd = m_stroke.Vertex(out x, out y);
                    break;

                case 3:
                case 4:
                case 5:
                case 6:
                    cmd = m_ellipse.Vertex(out x, out y);
                    break;
            }

            if (!Path.IsStop(cmd))
            {
                GetTransform().Transform(ref x, ref y);
            }
            return cmd;
        }

    };

    //----------------------------------------------------------curve3_ctrl
    //template<class IColorType> 
    public class curve3_ctrl<T> : curve3_ctrl_impl<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        IColorType m_color;

        public curve3_ctrl()
        {
            m_color = new RGBA_Doubles(0.0, 0.0, 0.0);
        }

        public void line_color(IColorType c) { m_color = c; }
        public override IColorType color(uint i) { return m_color; }
    };
}
