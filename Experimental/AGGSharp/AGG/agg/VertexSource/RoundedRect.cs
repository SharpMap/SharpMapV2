
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
//
// Rounded rectangle vertex generator
//
//----------------------------------------------------------------------------

//#ifndef AGG_ROUNDED_RECT_INCLUDED
//#define AGG_ROUNDED_RECT_INCLUDED

//#include "agg_basics.h"
//#include "agg_arc.h"

using System;
using NPack.Interfaces;
namespace AGG.VertexSource
{
    //------------------------------------------------------------rounded_rect
    //
    // See Implemantation agg_rounded_rect.cpp
    //
    public class RoundedRect<T> : IVertexSource<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        RectDouble<T> m_Bounds;
        T m_rx1;
        T m_ry1;
        T m_rx2;
        T m_ry2;
        T m_rx3;
        T m_ry3;
        T m_rx4;
        T m_ry4;
        uint m_status;
        Arc<T> m_arc = new Arc<T>();


        public RoundedRect(double left, double bottom, double right, double top, double r)
            : this(M.New<T>(left), M.New<T>(bottom), M.New<T>(right), M.New<T>(top), M.New<T>(r))
        {
        }

        public RoundedRect(T left, T bottom, T right, T top, T r)
        {
            m_Bounds.Left = (left); m_Bounds.Bottom = (bottom); m_Bounds.Right = (right); m_Bounds.Top = (top);
            m_rx1 = (r); m_ry1 = (r); m_rx2 = (r); m_ry2 = (r);
            m_rx3 = (r); m_ry3 = (r); m_rx4 = (r); m_ry4 = (r);

            if (left.GreaterThan(right)) { m_Bounds.Left = right; m_Bounds.Right = left; }
            if (bottom.GreaterThan(top)) { m_Bounds.Bottom = top; m_Bounds.Top = bottom; }
        }

        public RoundedRect(RectDouble<T> bounds, T r)
            : this(bounds.Left, bounds.Bottom, bounds.Right, bounds.Top, r)
        {
        }

        public void Rect(T left, T bottom, T right, T top)
        {
            m_Bounds.Left = left;
            m_Bounds.Bottom = bottom;
            m_Bounds.Right = right;
            m_Bounds.Top = top;
            if (left.GreaterThan(right)) { m_Bounds.Left = right; m_Bounds.Right = left; }
            if (bottom.GreaterThan(top)) { m_Bounds.Bottom = top; m_Bounds.Top = bottom; }
        }

        public void Radius(T r)
        {
            m_rx1 = m_ry1 = m_rx2 = m_ry2 = m_rx3 = m_ry3 = m_rx4 = m_ry4 = r;
        }

        public void Radius(T rx, T ry)
        {
            m_rx1 = m_rx2 = m_rx3 = m_rx4 = rx;
            m_ry1 = m_ry2 = m_ry3 = m_ry4 = ry;
        }

        public void Radius(T rx_bottom, T ry_bottom, T rx_top, T ry_top)
        {
            m_rx1 = m_rx2 = rx_bottom;
            m_rx3 = m_rx4 = rx_top;
            m_ry1 = m_ry2 = ry_bottom;
            m_ry3 = m_ry4 = ry_top;
        }

        public void Radius(T rx1, T ry1, T rx2, T ry2,
                              T rx3, T ry3, T rx4, T ry4)
        {
            m_rx1 = rx1; m_ry1 = ry1; m_rx2 = rx2; m_ry2 = ry2;
            m_rx3 = rx3; m_ry3 = ry3; m_rx4 = rx4; m_ry4 = ry4;
        }

        public void NormalizeRadius()
        {
            T dx = m_Bounds.Top.Subtract(m_Bounds.Bottom).Abs();
            T dy = m_Bounds.Right.Subtract(m_Bounds.Left).Abs();

            T k = M.One<T>();
            T t;
            t = dx.Divide(m_rx1.Add(m_rx2)); if (t.LessThan(k)) k = t;
            t = dx.Divide(m_rx3.Add(m_rx4)); if (t.LessThan(k)) k = t;
            t = dy.Divide(m_ry1.Add(m_ry2)); if (t.LessThan(k)) k = t;
            t = dy.Divide(m_ry3.Add(m_ry4)); if (t.LessThan(k)) k = t;

            if (k.LessThan(1.0))
            {
                m_rx1.MultiplyEquals(k);
                m_ry1.MultiplyEquals(k);
                m_rx2.MultiplyEquals(k);
                m_ry2.MultiplyEquals(k);
                m_rx3.MultiplyEquals(k);
                m_ry3.MultiplyEquals(k);
                m_rx4.MultiplyEquals(k);
                m_ry4.MultiplyEquals(k);
            }
        }

        //public void ApproximationScale(double s) { m_arc.ApproximationScale(s); }
        public T ApproximationScale
        {
            get
            {
                return m_arc.ApproximationScale;
            }
            set
            {
                m_arc.ApproximationScale = value;
            }
        }

        public void Rewind(uint unused)
        {
            m_status = 0;
        }

        public uint Vertex(out T x, out  T y)
        {
            x = M.Zero<T>();
            y = M.Zero<T>();
            uint cmd = (uint)Path.Commands.Stop;
            switch (m_status)
            {
                case 0:
                    m_arc.Init(m_Bounds.Left.Add(m_rx1), m_Bounds.Bottom.Add(m_ry1), m_rx1, m_ry1,
                               M.PI<T>(), M.PI<T>().Multiply(M.PI<T>()).Multiply(0.5));
                    m_arc.Rewind(0);
                    m_status++;
                    goto case 1;

                case 1:
                    cmd = m_arc.Vertex(out x, out y);
                    if (Path.IsStop(cmd))
                    {
                        m_status++;
                    }
                    else
                    {
                        return cmd;
                    }
                    goto case 2;

                case 2:
                    m_arc.Init(m_Bounds.Right.Subtract(m_rx2), m_Bounds.Bottom.Add(m_ry2), m_rx2, m_ry2,
                         M.PI<T>().Add(M.PI<T>()).Multiply(0.5), M.Zero<T>());
                    m_arc.Rewind(0);
                    m_status++;
                    goto case 3;

                case 3:
                    cmd = m_arc.Vertex(out x, out y);
                    if (Path.IsStop(cmd))
                    {
                        m_status++;
                    }
                    else
                    {
                        return (uint)Path.Commands.LineTo;
                    }
                    goto case 4;

                case 4:
                    m_arc.Init(m_Bounds.Right.Subtract(m_rx3), m_Bounds.Top.Subtract(m_ry3), m_rx3, m_ry3,
                              M.Zero<T>(), M.PI<T>().Multiply(0.5));
                    m_arc.Rewind(0);
                    m_status++;
                    goto case 5;

                case 5:
                    cmd = m_arc.Vertex(out x, out y);
                    if (Path.IsStop(cmd))
                    {
                        m_status++;
                    }
                    else
                    {
                        return (uint)Path.Commands.LineTo;
                    }
                    goto case 6;

                case 6:
                    m_arc.Init(m_Bounds.Left.Add(m_rx4), m_Bounds.Top.Subtract(m_ry4), m_rx4, m_ry4,
                             M.PI<T>().Multiply(0.5), M.PI<T>());
                    m_arc.Rewind(0);
                    m_status++;
                    goto case 7;

                case 7:
                    cmd = m_arc.Vertex(out x, out y);
                    if (Path.IsStop(cmd))
                    {
                        m_status++;
                    }
                    else
                    {
                        return (uint)Path.Commands.LineTo;
                    }
                    goto case 8;

                case 8:
                    cmd = (uint)Path.Commands.EndPoly
                        | (uint)Path.Flags.Close
                        | (uint)Path.Flags.CCW;
                    m_status++;
                    break;
            }
            return cmd;
        }
    };
}

