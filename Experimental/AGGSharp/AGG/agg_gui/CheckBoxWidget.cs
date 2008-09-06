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
// classes cbox_ctrl
//
//----------------------------------------------------------------------------
using System;
using AGG.Color;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;
namespace AGG.UI
{
    //----------------------------------------------------------cbox_ctrl_impl
    public class cbox_ctrl<T> : AGG.UI.SimpleVertexSourceWidget<T>
                 where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private T m_text_thickness;
        private T m_FontSize;
        private String m_label;
        private bool m_status;
        private T[] m_vx = new T[32];
        private T[] m_vy = new T[32];

        private GsvText<T> m_text;
        private ConvStroke<T> m_text_poly;

        private uint m_idx;
        private uint m_vertex;

        private RGBA_Doubles m_text_color;
        private RGBA_Doubles m_inactive_color;
        private RGBA_Doubles m_active_color;

        public cbox_ctrl(double x, double y, string label)
            : this(M.New<T>(x), M.New<T>(y), label)
        { }

        public cbox_ctrl(T x, T y, string label)
            : base(x, y, x.Add(9.0 * 1.5), y.Add(9.0 * 1.5))
        {
            m_text_thickness = M.New<T>(1.5);
            m_FontSize = M.New<T>(9.0);
            m_status = (false);
            m_text = new GsvText<T>();
            m_text_poly = new ConvStroke<T>(m_text);
            m_label = label;

            m_text_color = new RGBA_Doubles(0.0, 0.0, 0.0);
            m_inactive_color = new RGBA_Doubles(0.0, 0.0, 0.0);
            m_active_color = new RGBA_Doubles(0.4, 0.0, 0.0);
        }

        override public uint num_paths() { return 3; }
        override public void Rewind(uint idx)
        {
            m_idx = idx;

            T d2;
            T t;

            switch (idx)
            {
                default:
                case 0:                 // Border
                    m_vertex = 0;
                    m_vx[0] = Bounds.Left;
                    m_vy[0] = Bounds.Bottom;
                    m_vx[1] = Bounds.Right;
                    m_vy[1] = Bounds.Bottom;
                    m_vx[2] = Bounds.Right;
                    m_vy[2] = Bounds.Top;
                    m_vx[3] = Bounds.Left;
                    m_vy[3] = Bounds.Top;
                    m_vx[4] = Bounds.Left.Add(m_text_thickness);
                    m_vy[4] = Bounds.Bottom.Add(m_text_thickness);
                    m_vx[5] = Bounds.Left.Add(m_text_thickness);
                    m_vy[5] = Bounds.Top.Subtract(m_text_thickness);
                    m_vx[6] = Bounds.Right.Subtract(m_text_thickness);
                    m_vy[6] = Bounds.Top.Subtract(m_text_thickness);
                    m_vx[7] = Bounds.Right.Subtract(m_text_thickness);
                    m_vy[7] = Bounds.Bottom.Add(m_text_thickness);
                    break;

                case 1:
                    m_text.Text = m_label;
                    m_text.StartPoint(Bounds.Left.Add(m_FontSize.Multiply(2.0)), Bounds.Bottom.Add(m_FontSize.Divide(5.0)));

                    m_text.SetFontSize(m_FontSize);
                    m_text_poly.Width = m_text_thickness;
                    m_text_poly.LineJoin = LineJoin.RoundJoin;
                    m_text_poly.LineCap = LineCap.Round;
                    m_text_poly.Rewind(0);
                    break;

                case 2:                 // Active item
                    m_vertex = 0;
                    d2 = Bounds.Top.Subtract(Bounds.Bottom).Divide(2.0);
                    t = m_text_thickness.Multiply(1.5);
                    m_vx[0] = Bounds.Left.Add(m_text_thickness);
                    m_vy[0] = Bounds.Bottom.Add(m_text_thickness);
                    m_vx[1] = Bounds.Left.Add(d2);
                    m_vy[1] = Bounds.Bottom.Add(d2).Subtract(t);
                    m_vx[2] = Bounds.Right.Subtract(m_text_thickness);
                    m_vy[2] = Bounds.Bottom.Add(m_text_thickness);
                    m_vx[3] = Bounds.Left.Add(d2).Add(t);
                    m_vy[3] = Bounds.Bottom.Add(d2);
                    m_vx[4] = Bounds.Right.Subtract(m_text_thickness);
                    m_vy[4] = Bounds.Top.Subtract(m_text_thickness);
                    m_vx[5] = Bounds.Left.Add(d2);
                    m_vy[5] = Bounds.Bottom.Add(d2).Add(t);
                    m_vx[6] = Bounds.Left.Add(m_text_thickness);
                    m_vy[6] = Bounds.Top.Subtract(m_text_thickness);
                    m_vx[7] = Bounds.Left.Add(d2).Subtract(t);
                    m_vy[7] = Bounds.Bottom.Add(d2);
                    break;

            }
        }

        override public uint Vertex(out T x, out T y)
        {
            x = M.Zero<T>();
            y = M.Zero<T>();
            uint cmd = (uint)Path.Commands.LineTo;
            switch (m_idx)
            {
                case 0:
                    if (m_vertex == 0 || m_vertex == 4)
                    {
                        cmd = (uint)Path.Commands.MoveTo;
                    }
                    if (m_vertex >= 8)
                    {
                        cmd = (uint)Path.Commands.Stop;
                    }
                    x = m_vx[m_vertex];
                    y = m_vy[m_vertex];
                    m_vertex++;
                    break;

                case 1:
                    cmd = m_text_poly.Vertex(out x, out y);
                    break;

                case 2:
                    if (m_status)
                    {
                        if (m_vertex == 0) cmd = (uint)Path.Commands.MoveTo;
                        if (m_vertex >= 8) cmd = (uint)Path.Commands.Stop;
                        x = m_vx[m_vertex];
                        y = m_vy[m_vertex];
                        m_vertex++;
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

        public void text_thickness(T t) { m_text_thickness = t; }


        public void SetFontSize(double fontSize)
        {
            SetFontSize(M.New<T>(fontSize));
        }
        public void SetFontSize(T fontSize)
        {
            SetFontSizeAndWidthRatio(fontSize, 1);
        }

        public void SetFontSizeAndWidthRatio(T fontSize, double widthRatioOfHeight)
        {
            if (fontSize.Equals(0) || widthRatioOfHeight == 0)
            {
                throw new System.Exception("You can't have a font with 0 width or height.  Nothing will render.");
            }
            m_FontSize = fontSize;
        }

        public String label() { return m_label; }
        public void label(String in_label)
        {
            m_label = in_label;
        }

        public bool status() { return m_status; }
        public void status(bool st) { m_status = st; }

        override public bool InRect(T x, T y)
        {
            GetTransform().Inverse.Transform(ref x, ref y);
            return Bounds.HitTest(x, y);
        }

        override public void OnMouseDown(MouseEventArgs mouseEvent)
        {
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            GetTransform().Inverse.Transform(ref x, ref y);
            if (Bounds.HitTest(x, y))
            {
                m_status = !m_status;
                mouseEvent.Handled = true;
            }
        }

        override public void OnMouseUp(MouseEventArgs mouseEvent)
        {
        }

        override public void OnMouseMove(MouseEventArgs mouseEvent)
        {
        }

        override public void OnKeyDown(KeyEventArgs keyEvent)
        {
            if (keyEvent.KeyCode == Keys.Space)
            {
                m_status = !m_status;
                keyEvent.Handled = true;
            }
        }

        public void text_color(IColorType c) { m_text_color = c.GetAsRGBA_Doubles(); }
        public void inactive_color(IColorType c) { m_inactive_color = c.GetAsRGBA_Doubles(); }
        public void active_color(IColorType c) { m_active_color = c.GetAsRGBA_Doubles(); }

        override public IColorType color(uint i)
        {
            switch (i)
            {
                case 0:
                    return m_inactive_color;

                case 1:
                    return m_text_color;

                case 2:
                    return m_active_color;

                default:
                    return m_inactive_color;
            }
        }
    };
}
