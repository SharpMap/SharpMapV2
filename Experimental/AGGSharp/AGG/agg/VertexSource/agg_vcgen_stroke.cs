
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
using AGG.VertexSource.Stroke;
using NPack.Interfaces;
namespace AGG.VertexSource
{
    namespace Stroke
    {
        enum Status
        {
            Initial,
            Ready,
            Cap1,
            Cap2,
            Outline1,
            CloseFirst,
            Outline2,
            OutVertices,
            EndPoly1,
            EndPoly2,
            Stop
        };
    }

    class PointDVector<T> : VectorPOD<IVector<T>>, IVertexDest<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {

    };
    //============================================================vcgen_stroke
    class VCGenStroke<T> : IGenerator<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        MathStroke<T> m_stroker;

        VertexSequence<T> m_src_vertices;
        PointDVector<T> m_out_vertices;

        T m_shorten;
        uint m_closed;
        Status m_status;
        Status m_prev_status;

        uint m_src_vertex;
        uint m_out_vertex;



        public VCGenStroke()
        {
            m_stroker = new MathStroke<T>();
            m_src_vertices = new VertexSequence<T>();
            m_out_vertices = new PointDVector<T>();
            m_status = Status.Initial;
        }

        //public void LineCap(LineCap lc) { m_stroker.LineCap = lc; }
        //public void LineJoin(LineJoin lj) { m_stroker.LineJoin = lj; }
        //public void InnerJoin(InnerJoin ij) { m_stroker.InnerJoin = ij; }

        public LineCap LineCap { get { return m_stroker.LineCap; } set { m_stroker.LineCap = value; } }
        public LineJoin LineJoin { get { return m_stroker.LineJoin; } set { m_stroker.LineJoin = value; } }
        public InnerJoin InnerJoin { get { return m_stroker.InnerJoin; } set { m_stroker.InnerJoin = value; } }

        //public void Width(double w) { m_stroker.width(w); }
        //public void MiterLimit(double ml) { m_stroker.miter_limit(ml); }
        //public void MiterLimitTheta(double t) { m_stroker.miter_limit_theta(t); }
        //public void InnerMiterLimit(double ml) { m_stroker.inner_miter_limit(ml); }
        //public void ApproximationScale(double approx_scale) { m_stroker.approximation_scale(approx_scale); }

        public T Width { get { return m_stroker.Width; } set { m_stroker.Width = value; } }
        public T MiterLimit
        {
            get { return m_stroker.MiterLimit; }

            set { m_stroker.MiterLimit = value; }
        }
        public T InnerMiterLimit
        {
            get { return m_stroker.InnerMiterLimit; }
            set { m_stroker.InnerMiterLimit = value; }
        }
        public T ApproximationScale
        {
            get
            {
                return m_stroker.ApproximationScale;
            }
            set
            {

                m_stroker.ApproximationScale = value;
            }
        }

        public T MiterLimitTheta { set { } }

        // public void Shorten(double s) { m_shorten = s; }
        public T Shorten { get { return m_shorten; } set { m_shorten = value; } }

        // Vertex Generator Interface
        public void RemoveAll()
        {
            m_src_vertices.RemoveAll();
            m_closed = 0;
            m_status = Status.Initial;
        }
        public void AddVertex(T x, T y, uint cmd)
        {
            m_status = Status.Initial;
            if (Path.IsMoveTo(cmd))
            {
                m_src_vertices.ModifyLast(new VertexDist<T>(x, y));
            }
            else
            {
                if (Path.IsVertex(cmd))
                {
                    m_src_vertices.Add(new VertexDist<T>(x, y));
                }
                else
                {
                    m_closed = (uint)Path.GetCloseFlag(cmd);
                }
            }
        }

        // Vertex Source Interface
        public void Rewind(uint idx)
        {
            if (m_status == Status.Initial)
            {
                m_src_vertices.Close(m_closed != 0);
                Path.ShortenPath(m_src_vertices, m_shorten, m_closed);
                if (m_src_vertices.Size() < 3) m_closed = 0;
            }
            m_status = Status.Ready;
            m_src_vertex = 0;
            m_out_vertex = 0;
        }

        public uint Vertex(ref T x, ref T y)
        {
            uint cmd = (uint)Path.Commands.LineTo;
            while (!Path.IsStop(cmd))
            {
                switch (m_status)
                {
                    case Status.Initial:
                        Rewind(0);
                        goto case Status.Ready;

                    case Status.Ready:
                        if (m_src_vertices.Size() < 2 + (m_closed != 0 ? 1 : 0))
                        {
                            cmd = (uint)Path.Commands.Stop;
                            break;
                        }
                        m_status = (m_closed != 0) ? Status.Outline1 : Status.Cap1;
                        cmd = (uint)Path.Commands.MoveTo;
                        m_src_vertex = 0;
                        m_out_vertex = 0;
                        break;

                    case Status.Cap1:
                        m_stroker.CalcCap(m_out_vertices, m_src_vertices[0], m_src_vertices[1],
                            m_src_vertices[0].Dist);
                        m_src_vertex = 1;
                        m_prev_status = Status.Outline1;
                        m_status = Status.OutVertices;
                        m_out_vertex = 0;
                        break;

                    case Status.Cap2:
                        m_stroker.CalcCap(m_out_vertices,
                            m_src_vertices[m_src_vertices.Size() - 1],
                            m_src_vertices[m_src_vertices.Size() - 2],
                            m_src_vertices[m_src_vertices.Size() - 2].Dist);
                        m_prev_status = Status.Outline2;
                        m_status = Status.OutVertices;
                        m_out_vertex = 0;
                        break;

                    case Status.Outline1:
                        if (m_closed != 0)
                        {
                            if (m_src_vertex >= m_src_vertices.Size())
                            {
                                m_prev_status = Status.CloseFirst;
                                m_status = Status.EndPoly1;
                                break;
                            }
                        }
                        else
                        {
                            if (m_src_vertex >= m_src_vertices.Size() - 1)
                            {
                                m_status = Status.Cap2;
                                break;
                            }
                        }
                        m_stroker.CalcJoin(m_out_vertices,
                            m_src_vertices.Prev(m_src_vertex),
                            m_src_vertices.Curr(m_src_vertex),
                            m_src_vertices.Next(m_src_vertex),
                            m_src_vertices.Prev(m_src_vertex).Dist,
                            m_src_vertices.Curr(m_src_vertex).Dist);
                        ++m_src_vertex;
                        m_prev_status = m_status;
                        m_status = Status.OutVertices;
                        m_out_vertex = 0;
                        break;

                    case Status.CloseFirst:
                        m_status = Status.Outline2;
                        cmd = (uint)Path.Commands.MoveTo;
                        goto case Status.Outline2;

                    case Status.Outline2:
                        if (m_src_vertex <= (m_closed == 0 ? 1 : 0))
                        {
                            m_status = Status.EndPoly2;
                            m_prev_status = Status.Stop;
                            break;
                        }

                        --m_src_vertex;
                        m_stroker.CalcJoin(m_out_vertices,
                            m_src_vertices.Next(m_src_vertex),
                            m_src_vertices.Curr(m_src_vertex),
                            m_src_vertices.Prev(m_src_vertex),
                            m_src_vertices.Curr(m_src_vertex).Dist,
                            m_src_vertices.Prev(m_src_vertex).Dist);

                        m_prev_status = m_status;
                        m_status = Status.OutVertices;
                        m_out_vertex = 0;
                        break;

                    case Status.OutVertices:
                        if (m_out_vertex >= m_out_vertices.Size())
                        {
                            m_status = m_prev_status;
                        }
                        else
                        {
                            IVector<T> c = m_out_vertices[(int)m_out_vertex++];
                            x = c[0];
                            y = c[1];
                            return cmd;
                        }
                        break;

                    case Status.EndPoly1:
                        m_status = m_prev_status;
                        return (uint)Path.Commands.EndPoly
                            | (uint)Path.Flags.Close
                            | (uint)Path.Flags.CCW;

                    case Status.EndPoly2:
                        m_status = m_prev_status;
                        return (uint)Path.Commands.EndPoly
                            | (uint)Path.Flags.Close
                            | (uint)Path.Flags.CW;

                    case Status.Stop:
                        cmd = (uint)Path.Commands.Stop;
                        break;
                }
            }
            return cmd;
        }
    };
}