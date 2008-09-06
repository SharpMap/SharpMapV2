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
// classes rbox_ctrl_impl, rbox_ctrl
//
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using AGG.Color;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;
namespace AGG.UI
{
    //------------------------------------------------------------------------
    public class rbox_ctrl_impl<T> : SimpleVertexSourceWidget<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T m_border_width;
        T m_border_extra;
        T m_text_thickness;
        T m_text_height;
        T m_text_width;
        List<string> m_items;
        int m_cur_item;

        T m_xs1;
        T m_ys1;
        T m_xs2;
        T m_ys2;

        T[] m_vx = new T[32];
        T[] m_vy = new T[32];
        uint m_draw_item;
        T m_dy;

        Ellipse<T> m_ellipse;
        ConvStroke<T> m_ellipse_poly;
        GsvText<T> m_text;
        ConvStroke<T> m_text_poly;

        uint m_idx;
        uint m_vertex;

        public rbox_ctrl_impl(double x1, double y1, double x2, double y2)
            : this(M.New<T>(x1), M.New<T>(y1), M.New<T>(x2), M.New<T>(y2))
        { }

        public rbox_ctrl_impl(T x1, T y1, T x2, T y2)
            : base(x1, y1, x2, y2)
        {
            m_border_width = M.One<T>();
            m_border_extra = M.Zero<T>();
            m_text_thickness = M.New<T>(1.5);
            m_text_height = M.New<T>(9.0);
            m_text_width = M.Zero<T>();
            m_cur_item = -1;
            m_ellipse = new Ellipse<T>();
            m_ellipse_poly = new ConvStroke<T>(m_ellipse);
            m_text = new GsvText<T>();
            m_text_poly = new ConvStroke<T>(m_text);
            m_idx = (0);
            m_vertex = (0);
            m_items = new List<string>();
            calc_rbox();
        }

        public void border_width(double t)
        {
            border_width(M.New<T>(t));
        }

        public void border_width(T t)
        {
            border_width(t, M.Zero<T>());
        }
        public void border_width(double t, double extra)
        {
            border_width(M.New<T>(t), M.New<T>(extra));
        }
        public void border_width(T t, T extra)
        {
            m_border_width = t;
            m_border_extra = extra;
            calc_rbox();
        }
        public void text_thickness(double t)
        {
            text_thickness(M.New<T>(t));  
        }
        public void text_thickness(T t) { m_text_thickness = t; }


        public void text_size(double h)
        {
            text_size(M.New<T>(h));
        }

        public void text_size(T h)
        {
            text_size(h, M.Zero<T>());
        }
        public void text_size(double h, double w)
        {
            text_size(M.New<T>(h), M.New<T>(w));
        }
        public void text_size(T h, T w)
        {
            m_text_width = w;
            m_text_height = h;
        }

        public void add_item(string text)
        {
            m_items.Add(text);
        }

        public int cur_item() { return m_cur_item; }
        public void cur_item(int i) { m_cur_item = i; }

        public override bool InRect(T x, T y)
        {
            GetTransform().Inverse.Transform(ref x, ref y);
            return Bounds.HitTest(x, y);
        }

        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            GetTransform().Inverse.Transform(ref x, ref y);
            uint i;
            int NumItems = m_items.Count;
            for (i = 0; i < NumItems; i++)
            {
                T xp = m_xs1.Add(m_dy.Divide(1.3));
                T yp = m_ys1.Add(m_dy.Multiply(i)).Add(m_dy.Divide(1.3));
                if (MathUtil.CalcDistance(x, y, xp, yp).LessThanOrEqualTo(m_text_height.Divide(1.5)))
                {
                    m_cur_item = (int)(i);
                    mouseEvent.Handled = true;
                    return;
                }
            }
        }

        public override void OnMouseUp(MouseEventArgs mouseEvent)
        {
        }

        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
        }

        public override void OnKeyDown(KeyEventArgs keyEvent)
        {
            if (m_cur_item >= 0)
            {
                if (keyEvent.KeyCode == Keys.Up
                    || keyEvent.KeyCode == Keys.Right)
                {
                    m_cur_item++;
                    if (m_cur_item >= m_items.Count)
                    {
                        m_cur_item = 0;
                    }
                    keyEvent.Handled = true;
                }

                if (keyEvent.KeyCode == Keys.Down
                    || keyEvent.KeyCode == Keys.Left)
                {
                    m_cur_item--;
                    if (m_cur_item < 0)
                    {
                        m_cur_item = m_items.Count - 1;
                    }
                    keyEvent.Handled = true;
                }
            }
        }


        // Vertex source interface
        public override uint num_paths() { return 5; }
        public override void Rewind(uint idx)
        {
            m_idx = idx;
            m_dy = m_text_height.Multiply(2.0);
            m_draw_item = 0;

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

                case 1:                 // Border
                    m_vertex = 0;
                    m_vx[0] = Bounds.Left;
                    m_vy[0] = Bounds.Bottom;
                    m_vx[1] = Bounds.Right;
                    m_vy[1] = Bounds.Bottom;
                    m_vx[2] = Bounds.Right;
                    m_vy[2] = Bounds.Top;
                    m_vx[3] = Bounds.Left;
                    m_vy[3] = Bounds.Top;
                    m_vx[4] = Bounds.Left.Add(m_border_width);
                    m_vy[4] = Bounds.Bottom.Add(m_border_width);
                    m_vx[5] = Bounds.Left.Add(m_border_width);
                    m_vy[5] = Bounds.Top.Subtract(m_border_width);
                    m_vx[6] = Bounds.Right.Subtract(m_border_width);
                    m_vy[6] = Bounds.Top.Subtract(m_border_width);
                    m_vx[7] = Bounds.Right.Subtract(m_border_width);
                    m_vy[7] = Bounds.Bottom.Add(m_border_width);
                    break;

                case 2:                 // Text
                    m_text.Text = m_items[0];
                    m_text.StartPoint(m_xs1.Add(m_dy.Multiply(1.5)), m_ys1.Add(m_dy.Divide(2.0)));
                    m_text.SetFontSize(m_text_height);
                    m_text_poly.Width = m_text_thickness;
                    m_text_poly.LineJoin = LineJoin.RoundJoin;
                    m_text_poly.LineCap = LineCap.Round;
                    m_text_poly.Rewind(0);
                    break;

                case 3:                 // Inactive items
                    m_ellipse.Init(m_xs1.Add(m_dy.Divide(1.3)),
                                   m_ys1.Add(m_dy.Divide(1.3)),
                                   m_text_height.Divide(1.5),
                                   m_text_height.Divide(1.5), 32);
                    m_ellipse_poly.Width = m_text_thickness;
                    m_ellipse_poly.Rewind(0);
                    break;


                case 4:                 // Active Item
                    if (m_cur_item >= 0)
                    {
                        m_ellipse.Init(m_xs1.Add(m_dy.Divide(1.3)),
                                       m_ys1.Add(m_dy.Multiply(m_cur_item)).Add(m_dy.Divide(1.3)),
                                       m_text_height.Divide(2.0),
                                       m_text_height.Divide(2.0), 32);
                        m_ellipse.Rewind(0);
                    }
                    break;

            }
        }

        public override uint Vertex(out T x, out T y)
        {
            x = M.Zero<T>();
            y = M.Zero<T>();
            uint cmd = (uint)Path.Commands.LineTo;
            switch (m_idx)
            {
                case 0:
                    if (m_vertex == 0) cmd = (uint)Path.Commands.MoveTo;
                    if (m_vertex >= 4) cmd = (uint)Path.Commands.Stop;
                    x = m_vx[m_vertex];
                    y = m_vy[m_vertex];
                    m_vertex++;
                    break;

                case 1:
                    if (m_vertex == 0 || m_vertex == 4) cmd = (uint)Path.Commands.MoveTo;
                    if (m_vertex >= 8) cmd = (uint)Path.Commands.Stop;
                    x = m_vx[m_vertex];
                    y = m_vy[m_vertex];
                    m_vertex++;
                    break;

                case 2:
                    cmd = m_text_poly.Vertex(out x, out y);
                    if (Path.IsStop(cmd))
                    {
                        m_draw_item++;
                        if (m_draw_item >= m_items.Count)
                        {
                            break;
                        }
                        else
                        {
                            m_text.Text = m_items[(int)m_draw_item];
                            m_text.StartPoint(m_xs1.Add(m_dy.Multiply(1.5)),
                                               m_ys1.Add(m_dy.Multiply(m_draw_item + 1)).Subtract(m_dy.Divide(2.0)));

                            m_text_poly.Rewind(0);
                            cmd = m_text_poly.Vertex(out x, out y);
                        }
                    }
                    break;

                case 3:
                    cmd = m_ellipse_poly.Vertex(out x, out y);
                    if (Path.IsStop(cmd))
                    {
                        m_draw_item++;
                        if (m_draw_item >= m_items.Count)
                        {
                            break;
                        }
                        else
                        {
                            m_ellipse.Init(m_xs1.Add(m_dy.Divide(1.3)),
                                           m_ys1.Add(m_dy.Multiply(m_draw_item)).Add(m_dy.Divide(1.3)),
                                           m_text_height.Divide(1.5),
                                           m_text_height.Divide(1.5), 32);
                            m_ellipse_poly.Rewind(0);
                            cmd = m_ellipse_poly.Vertex(out x, out y);
                        }
                    }
                    break;


                case 4:
                    if (m_cur_item >= 0)
                    {
                        cmd = m_ellipse.Vertex(out x, out y);
                    }
                    else
                    {
                        cmd = (uint)Path.Commands.Stop;
                    }
                    break;

                default:
                    cmd = (uint)Path.Commands.Stop;
                    break;
            }

            if (!Path.IsStop(cmd))
            {
                GetTransform().Transform(ref x, ref y);
            }

            return cmd;
        }


        private void calc_rbox()
        {
            m_xs1 = Bounds.Left.Add(m_border_width);
            m_ys1 = Bounds.Bottom.Add(m_border_width);
            m_xs2 = Bounds.Right.Subtract(m_border_width);
            m_ys2 = Bounds.Top.Subtract(m_border_width);
        }
    };



    //------------------------------------------------------------------------
    public class rbox_ctrl<T> : rbox_ctrl_impl<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        IColorType m_background_color;
        IColorType m_border_color;
        IColorType m_text_color;
        IColorType m_inactive_color;
        IColorType m_active_color;


        public rbox_ctrl(double x1, double y1, double x2, double y2)
            : this(M.New<T>(x1), M.New<T>(y1), M.New<T>(x2), M.New<T>(y2))
        { }
        public rbox_ctrl(T x1, T y1, T x2, T y2) :
            base(x1, y1, x2, y2)
        {
            m_background_color = (new RGBA_Doubles(1.0, 1.0, 0.9));
            m_border_color = (new RGBA_Doubles(0.0, 0.0, 0.0));
            m_text_color = (new RGBA_Doubles(0.0, 0.0, 0.0));
            m_inactive_color = (new RGBA_Doubles(0.0, 0.0, 0.0));
            m_active_color = (new RGBA_Doubles(0.4, 0.0, 0.0));
        }


        public void background_color(IColorType c) { m_background_color = c; }
        public void border_color(IColorType c) { m_border_color = c; }
        public void text_color(IColorType c) { m_text_color = c; }
        public void inactive_color(IColorType c) { m_inactive_color = c; }
        public void active_color(IColorType c) { m_active_color = c; }

        public override IColorType color(uint i)
        {
            switch (i)
            {
                case 0:
                    return m_background_color;

                case 1:
                    return m_border_color;

                case 2:
                    return m_text_color;

                case 3:
                    return m_inactive_color;

                case 4:
                    return m_active_color;

                default:
                    throw new System.IndexOutOfRangeException("There is not a color for this index");
            }
        }
    };
}
