
using System;
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
using AGG.Color;
using NPack.Interfaces;
namespace AGG.VertexSource
{

    //============================================================span_gouraud
    public class SpanGouraud<T> : IVertexSource<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        CoordType[] m_coord = new CoordType[3];
        T[] m_x = new T[8];
        T[] m_y = new T[8];
        uint[] m_cmd = new uint[8];
        uint m_vertex;

        public struct CoordType
        {
            public T X;
            public T Y;
            public RGBA_Bytes Color;
        };

        //--------------------------------------------------------------------
        public SpanGouraud()
        {
            m_vertex = (0);
            m_cmd[0] = (uint)Path.Commands.Stop;
        }

        //--------------------------------------------------------------------
        public SpanGouraud(RGBA_Bytes c1,
                     RGBA_Bytes c2,
                     RGBA_Bytes c3,
                     T x1, T y1,
                     T x2, T y2,
                     T x3, T y3,
                     T d)
        {
            m_vertex = (0);
            Colors(c1, c2, c3);
            Triangle(x1, y1, x2, y2, x3, y3, d);
        }

        //--------------------------------------------------------------------
        public void Colors(IColorType c1, IColorType c2, IColorType c3)
        {
            m_coord[0].Color = c1.GetAsRGBA_Bytes();
            m_coord[1].Color = c2.GetAsRGBA_Bytes();
            m_coord[2].Color = c3.GetAsRGBA_Bytes();
        }

        //--------------------------------------------------------------------
        // Sets the triangle and dilates it if needed.
        // The trick here is to calculate beveled joins in the vertices of the 
        // triangle and render it as a 6-vertex polygon. 
        // It's necessary to achieve numerical stability. 
        // However, the coordinates to interpolate colors are calculated
        // as miter joins (calc_intersection).
        public void Triangle(T x1, T y1,
                      T x2, T y2,
                      T x3, T y3,
                      T d)
        {
            m_coord[0].X = m_x[0] = x1;
            m_coord[0].Y = m_y[0] = y1;
            m_coord[1].X = m_x[1] = x2;
            m_coord[1].Y = m_y[1] = y2;
            m_coord[2].X = m_x[2] = x3;
            m_coord[2].Y = m_y[2] = y3;
            m_cmd[0] = (uint)Path.Commands.MoveTo;
            m_cmd[1] = (uint)Path.Commands.LineTo;
            m_cmd[2] = (uint)Path.Commands.LineTo;
            m_cmd[3] = (uint)Path.Commands.Stop;

            if (d.NotEqual(0.0))
            {
                MathUtil.DilateTriangle(m_coord[0].X, m_coord[0].Y,
                                m_coord[1].X, m_coord[1].Y,
                                m_coord[2].X, m_coord[2].Y,
                                m_x, m_y, d);

                MathUtil.CalcIntersection(m_x[4], m_y[4], m_x[5], m_y[5],
                                  m_x[0], m_y[0], m_x[1], m_y[1],
                                  out m_coord[0].X, out m_coord[0].Y);

                MathUtil.CalcIntersection(m_x[0], m_y[0], m_x[1], m_y[1],
                                  m_x[2], m_y[2], m_x[3], m_y[3],
                                  out m_coord[1].X, out m_coord[1].Y);

                MathUtil.CalcIntersection(m_x[2], m_y[2], m_x[3], m_y[3],
                                  m_x[4], m_y[4], m_x[5], m_y[5],
                                  out m_coord[2].X, out m_coord[2].Y);
                m_cmd[3] = (uint)Path.Commands.LineTo;
                m_cmd[4] = (uint)Path.Commands.LineTo;
                m_cmd[5] = (uint)Path.Commands.LineTo;
                m_cmd[6] = (uint)Path.Commands.Stop;
            }
        }

        //--------------------------------------------------------------------
        // Vertex Source Interface to feed the coordinates to the rasterizer
        public void Rewind(uint idx)
        {
            m_vertex = 0;
        }

        //--------------------------------------------------------------------
        public uint Vertex(out T x, out T y)
        {
            x = m_x[m_vertex];
            y = m_y[m_vertex];
            return m_cmd[m_vertex++];
        }

        //--------------------------------------------------------------------
        protected void ArrangeVertices(CoordType[] coord)
        {
            unsafe
            {
                coord[0] = m_coord[0];
                coord[1] = m_coord[1];
                coord[2] = m_coord[2];

                if (m_coord[0].Y.GreaterThan(m_coord[2].Y))
                {
                    coord[0] = m_coord[2];
                    coord[2] = m_coord[0];
                }

                CoordType tmp;
                if (coord[0].Y.GreaterThan(coord[1].Y))
                {
                    tmp = coord[1];
                    coord[1] = coord[0];
                    coord[0] = tmp;
                }

                if (coord[1].Y.GreaterThan(coord[2].Y))
                {
                    tmp = coord[2];
                    coord[2] = coord[1];
                    coord[1] = tmp;
                }
            }
        }
    };
}
