
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
// classes conv_curve
//
//----------------------------------------------------------------------------
using System;
using NPack.Interfaces;
namespace AGG.VertexSource
{
    //---------------------------------------------------------------conv_curve
    // Curve converter class. Any path storage can have Bezier curves defined 
    // by their control points. There're two types of curves supported: curve3 
    // and curve4. Curve3 is a conic Bezier curve with 2 endpoints and 1 control
    // point. Curve4 has 2 control points (4 points in total) and can be used
    // to interpolate more complicated curves. Curve4, unlike curve3 can be used 
    // to approximate arcs, both circular and elliptical. Curves are approximated 
    // with straight lines and one of the approaches is just to store the whole 
    // sequence of vertices that approximate our curve. It takes additional 
    // memory, and at the same time the consecutive vertices can be calculated 
    // on demand. 
    //
    // Initially, path storages are not suppose to keep all the vertices of the
    // curves (although, nothing prevents us from doing so). Instead, path_storage
    // keeps only vertices, needed to calculate a curve on demand. Those vertices
    // are marked with special commands. So, if the path_storage contains curves 
    // (which are not real curves yet), and we render this storage directly, 
    // all we will see is only 2 or 3 straight line segments (for curve3 and 
    // curve4 respectively). If we need to see real curves drawn we need to 
    // include this class into the conversion pipeline. 
    //
    // Class conv_curve recognizes commands path_cmd_curve3 and path_cmd_curve4 
    // and converts these vertices into a move_to/line_to sequence. 
    //-----------------------------------------------------------------------
    public class ConvCurve<T> : IVertexSource<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        IVertexSource<T> m_source;
        T m_last_x;
        T m_last_y;
        Curve3<T> m_curve3;
        Curve4<T> m_curve4;

        public ConvCurve(IVertexSource<T> source)
        {
            m_curve3 = new Curve3<T>();
            m_curve4 = new Curve4<T>();
            m_source = (source);
            m_last_x = default(T).Zero;
            m_last_y = default(T).Zero;
        }

        public void Attach(IVertexSource<T> source) { m_source = source; }

        //public void ApproximationMethod(ApproximationMethod v)
        //{
        //    m_curve3.ApproximationMethod(v);
        //    m_curve4.ApproximationMethod(v);
        //}

        public ApproximationMethod ApproximationMethod
        {
            get
            {
                return m_curve4.ApproximationMethod;
            }
            set
            {
                m_curve3.ApproximationMethod = value;
                m_curve4.ApproximationMethod = value;
            }
        }

        //public void ApproximationScale(double s)
        //{
        //    m_curve3.ApproximationScale(s);
        //    m_curve4.ApproximationScale(s);
        //}

        public T ApproximationScale
        {
            get
            {

                return m_curve4.ApproximationScale;
            }
            set
            {
                m_curve3.ApproximationScale = value;
                m_curve4.ApproximationScale = value;
            }
        }

        //public void AngleTolerance(double v)
        //{
        //    m_curve3.AngleTolerance(v);
        //    m_curve4.AngleTolerance(v);
        //}

        public T AngleTolerance
        {
            get
            {
                return m_curve4.AngleTolerance;
            }
            set
            {
                m_curve3.AngleTolerance = value;
                m_curve4.AngleTolerance = value;
            }
        }

        //public void CuspLimit(double v)
        //{
        //    m_curve3.CuspLimit(v);
        //    m_curve4.CuspLimit(v);
        //}

        public T CuspLimit
        {
            get
            {
                return m_curve4.CuspLimit;
            }
            set
            {
                m_curve3.CuspLimit = value;
                m_curve4.CuspLimit = value;
            }
        }

        public void Rewind(uint path_id)
        {
            m_source.Rewind(path_id);
            m_last_x = default(T).Zero;
            m_last_y = default(T).Zero;
            m_curve3.Reset();
            m_curve4.Reset();
        }

        public uint Vertex(out T x, out T y)
        {
            if (!Path.IsStop(m_curve3.Vertex(out x, out y)))
            {
                m_last_x = x;
                m_last_y = y;
                return (uint)Path.Commands.LineTo;
            }

            if (!Path.IsStop(m_curve4.Vertex(out x, out y)))
            {
                m_last_x = x;
                m_last_y = y;
                return (uint)Path.Commands.LineTo;
            }

            T ct2_x;
            T ct2_y;
            T end_x;
            T end_y;

            uint cmd = m_source.Vertex(out x, out y);
            switch (cmd)
            {
                case (uint)Path.Commands.Curve3:
                    m_source.Vertex(out end_x, out end_y);

                    m_curve3.Init(m_last_x, m_last_y, x, y, end_x, end_y);

                    m_curve3.Vertex(out x, out y);    // First call returns path_cmd_move_to
                    m_curve3.Vertex(out x, out y);    // This is the first vertex of the curve
                    cmd = (uint)Path.Commands.LineTo;
                    break;

                case (uint)Path.Commands.Curve4:
                    m_source.Vertex(out ct2_x, out ct2_y);
                    m_source.Vertex(out end_x, out end_y);

                    m_curve4.Init(m_last_x, m_last_y, x, y, ct2_x, ct2_y, end_x, end_y);

                    m_curve4.Vertex(out x, out y);    // First call returns path_cmd_move_to
                    m_curve4.Vertex(out x, out y);    // This is the first vertex of the curve
                    cmd = (uint)Path.Commands.LineTo;
                    break;
            }
            m_last_x = x;
            m_last_y = y;
            return cmd;
        }
    };
}