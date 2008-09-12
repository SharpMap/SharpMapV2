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
//
// classes slider_ctrl_impl, slider_ctrl
//
//----------------------------------------------------------------------------
using System;
using AGG.Color;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;
namespace AGG.UI
{
    //--------------------------------------------------------slider_ctrl_impl
    public class SliderWidget<T> : SimpleVertexSourceWidget<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T m_border_width;
        T m_border_extra;
        T m_text_thickness;
        T m_value;
        T m_preview_value;
        T m_min;
        T m_max;
        uint m_num_steps;
        bool m_descending;
        string m_label = "";
        T m_xs1;
        T m_ys1;
        T m_xs2;
        T m_ys2;
        T m_pdx;
        protected bool m_mouse_move;
        private RGBA_Doubles m_background_color;
        private RGBA_Doubles m_triangle_color;
        private RGBA_Doubles m_text_color;
        private RGBA_Doubles m_pointer_preview_color;
        private RGBA_Doubles m_pointer_color;

        T[] m_vx = new T[32];
        T[] m_vy = new T[32];

        Ellipse<T> m_ellipse;

        uint m_idx;
        uint m_vertex;

        GsvText<T> m_text;
        ConvStroke<T> m_text_poly;
        PathStorage<T> m_storage;

        public SliderWidget(double x1, double y1, double x2, double y2)
            : this(M.New<T>(x1), M.New<T>(y1), M.New<T>(x2), M.New<T>(y2))
        { }

        public SliderWidget(T x1, T y1, T x2, T y2)
            : base(x1, y1, x2, y2)
        {
            m_border_width = M.One<T>();
            m_border_extra = y2.Subtract(y1).Divide(2);
            m_text_thickness = M.One<T>();
            m_pdx = M.Zero<T>();
            m_mouse_move = false;
            m_value = M.New<T>(0.5);
            m_preview_value = M.New<T>(0.5);
            m_min = M.Zero<T>();
            m_max = M.One<T>();
            m_num_steps = (0);
            m_descending = (false);
            m_ellipse = new Ellipse<T>();
            m_storage = new PathStorage<T>();
            m_text = new GsvText<T>();
            m_text_poly = new ConvStroke<T>(m_text);

            calc_box();

            m_background_color = (new RGBA_Doubles(1.0, 0.9, 0.8));
            m_triangle_color = (new RGBA_Doubles(0.7, 0.6, 0.6));
            m_text_color = (new RGBA_Doubles(0.0, 0.0, 0.0));
            m_pointer_preview_color = (new RGBA_Doubles(0.6, 0.4, 0.4, 0.4));
            m_pointer_color = (new RGBA_Doubles(0.8, 0.0, 0.0, 0.6));
        }

        public void border_width(T t, T extra)
        {
            m_border_width = t;
            m_border_extra = extra;
            calc_box();
        }
        public void range(double min, double max) { range(M.New<T>(min), M.New<T>(max)); }
        public void range(T min, T max) { m_min = min; m_max = max; }
        public void num_steps(uint num) { m_num_steps = num; }
        public void label(String fmt)
        {
            m_label = fmt;
        }
        public void text_thickness(T t) { m_text_thickness = t; }

        public bool descending() { return m_descending; }
        public void descending(bool v) { m_descending = v; }

        public T value() { return m_value.Multiply(m_max.Subtract(m_min)).Add(m_min); }
        public void value(double value)
        {
            this.value(M.New<T>(value));  
        }
        public void value(T value)
        {
            m_preview_value = value.Subtract(m_min).Divide(m_max.Subtract(m_min));
            if (m_preview_value.GreaterThan(1.0)) m_preview_value = M.One<T>();
            if (m_preview_value.LessThan(0.0)) m_preview_value = M.Zero<T>();
            normalize_value(true);
        }

        public override bool InRect(T x, T y)
        {
            GetTransform().Inverse.Transform(ref x, ref y);
            return Bounds.HitTest(x, y);
        }

        private bool IsOver(T x, T y)
        {
            T xp = m_xs1.Add(m_xs2.Subtract(m_xs1).Multiply(m_value));
            T yp = m_ys1.Add(m_ys2).Divide(2.0);
            return MathUtil.CalcDistance(x, y, xp, yp).LessThanOrEqualTo(Bounds.Top.Subtract(Bounds.Bottom));
        }

        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            GetTransform().Inverse.Transform(ref x, ref y);

            T xp = m_xs1.Add(m_xs2.Subtract(m_xs1).Multiply(m_value));
            T yp = m_ys1.Add(m_ys2).Divide(2.0);

            if (IsOver(x, y))
            {
                m_pdx = xp.Subtract(x);
                m_mouse_move = true;
                mouseEvent.Handled = true;
            }
        }

        public override void OnMouseUp(MouseEventArgs mouseEvent)
        {
            if (m_mouse_move)
            {
                m_mouse_move = false;
                normalize_value(true);
                mouseEvent.Handled = true;
            }
        }

        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            GetTransform().Inverse.Transform(ref x, ref y);

            if (m_mouse_move)
            {
                T xp = x.Add(m_pdx);
                m_preview_value = xp.Subtract(m_xs1).Divide(m_xs2.Subtract(m_xs1));
                if (m_preview_value.LessThan(0.0)) m_preview_value = M.Zero<T>();
                if (m_preview_value.GreaterThan(1.0)) m_preview_value = M.One<T>();
                mouseEvent.Handled = true;
            }
        }

        public override void OnKeyDown(KeyEventArgs keyEvent)
        {
            double d = 0.005;
            if (m_num_steps != 0)
            {
                d = 1.0 / m_num_steps;
            }

            if (keyEvent.KeyCode == Keys.Right
                || keyEvent.KeyCode == Keys.Up)
            {
                m_preview_value.AddEquals(d);
                if (m_preview_value.GreaterThan(1.0)) m_preview_value = M.One<T>();
                normalize_value(true);
                keyEvent.Handled = true;
            }

            if (keyEvent.KeyCode == Keys.Left
                || keyEvent.KeyCode == Keys.Down)
            {
                m_preview_value.SubtractEquals(d);
                if (m_preview_value.LessThan(0.0)) m_preview_value = M.Zero<T>();
                normalize_value(true);
                keyEvent.Handled = true;
            }
        }

        // Vertex source interface
        public override uint num_paths() { return 6; }

        public override void Rewind(uint idx)
        {
            m_idx = idx;

            switch (idx)
            {
                default:

                case 0:                 // Background
                    m_vertex = 0;
                    m_vx[0] = Bounds.Left.Subtract(m_border_extra);
                    m_vy[0] = Bounds.Bottom.Subtract(m_border_extra);
                    m_vx[1] = Bounds.Right.Add(m_border_extra);
                    m_vy[1] = Bounds.Bottom.Subtract(m_border_extra);
                    m_vx[2] = Bounds.Right.Add(m_border_extra);
                    m_vy[2] = Bounds.Top.Add(m_border_extra);
                    m_vx[3] = Bounds.Left.Subtract(m_border_extra);
                    m_vy[3] = Bounds.Top.Add(m_border_extra);
                    break;

                case 1:                 // Triangle
                    m_vertex = 0;
                    if (m_descending)
                    {
                        m_vx[0] = Bounds.Left;
                        m_vy[0] = Bounds.Bottom;
                        m_vx[1] = Bounds.Right;
                        m_vy[1] = Bounds.Bottom;
                        m_vx[2] = Bounds.Left;
                        m_vy[2] = Bounds.Top;
                        m_vx[3] = Bounds.Left;
                        m_vy[3] = Bounds.Bottom;
                    }
                    else
                    {
                        m_vx[0] = Bounds.Left;
                        m_vy[0] = Bounds.Bottom;
                        m_vx[1] = Bounds.Right;
                        m_vy[1] = Bounds.Bottom;
                        m_vx[2] = Bounds.Right;
                        m_vy[2] = Bounds.Top;
                        m_vx[3] = Bounds.Left;
                        m_vy[3] = Bounds.Bottom;
                    }
                    break;

                case 2:
                    m_text.Text = m_label;
                    if (m_label.Length > 0)
                    {
                        string buf;
                        buf = string.Format(m_label, value());
                        m_text.Text = buf;
                    }
                    m_text.StartPoint(Bounds.Left, Bounds.Bottom);
                    m_text.SetFontSize(Bounds.Top.Subtract(Bounds.Bottom).Multiply(1.2));
                    m_text_poly.Width = m_text_thickness;
                    m_text_poly.LineJoin = LineJoin.RoundJoin;
                    m_text_poly.LineCap = LineCap.Round;
                    m_text_poly.Rewind(0);
                    break;

                case 3:                 // pointer preview
                    m_ellipse.Init(m_xs1.Add(m_xs2.Subtract(m_xs1).Multiply(m_preview_value)),
                        m_ys1.Add(m_ys2).Multiply(2.0),
                        Bounds.Top.Subtract(Bounds.Bottom),
                        Bounds.Top.Subtract(Bounds.Bottom),
                        32);
                    break;


                case 4:                 // pointer
                    normalize_value(false);
                    m_ellipse.Init(m_xs1.Add(m_xs2.Subtract(m_xs1).Multiply(m_value)),
                        m_ys1.Add(m_ys2).Divide(2.0),
                        Bounds.Top.Subtract(Bounds.Bottom),
                        Bounds.Top.Subtract(Bounds.Bottom),
                        32);
                    m_ellipse.Rewind(0);
                    break;

                case 5:
                    m_storage.RemoveAll();
                    if (m_num_steps != 0)
                    {
                        uint i;
                        T d = m_xs2.Subtract(m_xs1).Divide(m_num_steps);
                        if (d.GreaterThan(0.004)) d = M.New<T>(0.004);
                        for (i = 0; i < m_num_steps + 1; i++)
                        {
                            T x = m_xs1.Add(m_xs2.Subtract(m_xs1).Multiply(i).Divide(m_num_steps));
                            m_storage.MoveTo(x, Bounds.Bottom);
                            m_storage.LineTo(x.Subtract(d.Multiply(Bounds.Right.Subtract(Bounds.Left))), Bounds.Bottom.Subtract(m_border_extra));
                            m_storage.LineTo(x.Add(d.Multiply(Bounds.Right.Subtract(Bounds.Left))), Bounds.Bottom.Subtract(m_border_extra));
                        }
                    }
                    break;
            }
        }

        public override uint Vertex(out T x, out T y)
        {
            x = M.Zero<T>();
            y = M.Zero<T>();
            uint PathAndFlags = (uint)Path.Commands.LineTo;
            switch (m_idx)
            {
                case 0:
                    if (m_vertex == 0) PathAndFlags = (uint)Path.Commands.MoveTo;
                    if (m_vertex >= 4) PathAndFlags = (uint)Path.Commands.Stop;
                    x = m_vx[m_vertex];
                    y = m_vy[m_vertex];
                    m_vertex++;
                    break;

                case 1:
                    if (m_vertex == 0) PathAndFlags = (uint)Path.Commands.MoveTo;
                    if (m_vertex >= 4) PathAndFlags = (uint)Path.Commands.Stop;
                    x = m_vx[m_vertex];
                    y = m_vy[m_vertex];
                    m_vertex++;
                    break;

                case 2:
                    PathAndFlags = m_text_poly.Vertex(out x, out y);
                    //return (uint)Path.path_commands_e.path_cmd_stop;
                    break;

                case 3:
                case 4:
                    PathAndFlags = m_ellipse.Vertex(out x, out y);
                    break;

                case 5:
                    PathAndFlags = m_storage.Vertex(out x, out y);
                    break;

                default:
                    PathAndFlags = (uint)Path.Commands.Stop;
                    break;
            }

            if (!Path.IsStop(PathAndFlags))
            {
                GetTransform().Transform(ref x, ref y);
            }

            return PathAndFlags;
        }

        private void calc_box()
        {
            m_xs1 = Bounds.Left.Add(m_border_width);
            m_ys1 = Bounds.Bottom.Add(m_border_width);
            m_xs2 = Bounds.Right.Subtract(m_border_width);
            m_ys2 = Bounds.Top.Subtract(m_border_width);
        }

        private bool normalize_value(bool preview_value_flag)
        {
            bool ret = true;
            if (m_num_steps != 0)
            {
                int step = (int)(m_preview_value.Multiply(m_num_steps).Add(0.5).ToInt());
                ret = m_value.NotEqual(M.New<T>(step).Divide(m_num_steps));
                m_value = M.New<T>(step).Divide(m_num_steps);
            }
            else
            {
                m_value = m_preview_value;
            }

            if (preview_value_flag)
            {
                m_preview_value = m_value;
            }
            return ret;
        }

        public void background_color(RGBA_Doubles c) { m_background_color = c; }
        public void pointer_color(RGBA_Doubles c) { m_pointer_color = c; }

        public override IColorType color(uint i)
        {
            switch (i)
            {
                case 0:
                    return m_background_color;

                case 1:
                    return m_triangle_color;

                case 2:
                    return m_text_color;

                case 3:
                    return m_pointer_preview_color;

                case 4:
                    return m_pointer_color;

                case 5:
                    return m_text_color;
            }

            return m_background_color;
        }
    };
}
