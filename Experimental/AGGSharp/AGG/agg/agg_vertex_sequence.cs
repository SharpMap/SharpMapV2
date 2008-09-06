
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
// vertex_sequence container and vertex_dist struct
//
//----------------------------------------------------------------------------
//#ifndef AGG_VERTEX_SEQUENCE_INCLUDED
//#define AGG_VERTEX_SEQUENCE_INCLUDED

//#include "agg_basics.h"
//#include "agg_array.h"
//#include "agg_math.h"

using System;
using NPack.Interfaces;
namespace AGG
{

    //----------------------------------------------------------vertex_sequence
    // Modified agg::pod_vector. The data is interpreted as a sequence 
    // of vertices. It means that the type T must expose:
    //
    // bool T::operator() (const T& val)
    // 
    // that is called every time a new vertex is being added. The main purpose
    // of this operator is the possibility to calculate some values during 
    // adding and to return true if the vertex fits some criteria or false if
    // it doesn't. In the last case the new vertex is not added. 
    // 
    // The simple example is filtering coinciding vertices with calculation 
    // of the distance between the current and previous ones:
    //
    //    struct vertex_dist
    //    {
    //        double   x;
    //        double   y;
    //        double   dist;
    //
    //        vertex_dist() {}
    //        vertex_dist(double x_, double y_) :
    //            x(x_),
    //            y(y_),
    //            dist(0.0)
    //        {
    //        }
    //
    //        bool operator () (const vertex_dist& val)
    //        {
    //            return (dist = calc_distance(x, y, val.x, val.y)) > EPSILON;
    //        }
    //    };
    //
    // Function close() calls this operator and removes the last vertex if 
    // necessary.
    //------------------------------------------------------------------------
    //template<class T, unsigned S=6> 
    public class VertexSequence<T> : VectorPOD<VertexDist<T>>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    //public class vertex_sequence<TypeOfVertex> : pod_vector<TypeOfVertex> where TypeOfVertex : 
    {
        //typedef pod_vector<T, S> base_type;

        public override void Add(VertexDist<T> val)
        {
            if (base.Size() > 1)
            {
                if (!Array[base.Size() - 2].IsEqual(Array[base.Size() - 1]))
                {
                    base.RemoveLast();
                }
            }
            base.Add(val);
        }

        public void ModifyLast(VertexDist<T> val)
        {
            base.RemoveLast();
            Add(val);
        }

        public void Close(bool closed)
        {
            while (base.Size() > 1)
            {
                if (Array[base.Size() - 2].IsEqual(Array[base.Size() - 1])) break;
                VertexDist<T> t = this[base.Size() - 1];
                base.RemoveLast();
                ModifyLast(t);
            }

            if (closed)
            {
                while (base.Size() > 1)
                {
                    if (Array[base.Size() - 1].IsEqual(Array[0])) break;
                    base.RemoveLast();
                }
            }
        }

        internal VertexDist<T> Prev(uint idx)
        {
            return this[(idx + m_size - 1) % m_size];
        }

        internal VertexDist<T> Curr(uint idx)
        {
            return this[idx];
        }

        internal VertexDist<T> Next(uint idx)
        {
            return this[(idx + 1) % m_size];
        }
    };

    //-------------------------------------------------------------vertex_dist
    // Vertex (x, y) with the distance to the next one. The last vertex has 
    // distance between the last and the first points if the polygon is closed
    // and 0.0 if it's a polyline.
    public struct VertexDist<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public T X;

        //public double X
        //{
        //    get { return x; }
        //    set { x = value; }
        //}

        public T Y;

        //public double Y
        //{
        //    get { return y; }
        //    set { y = value; }
        //}


        public T Dist;

        //public double Dist
        //{
        //    get { return dist; }
        //    set { dist = value; }
        //}

        public VertexDist(T x_, T y_)
        {
            X = x_;
            Y = y_;
            Dist = M.Zero<T>();
        }

        public bool IsEqual(VertexDist<T> val)
        {
            bool ret = (Dist = MathUtil.CalcDistance(X, Y, val.X, val.Y)).GreaterThan(MathUtil.VertexDistEpsilon);
            if (!ret) Dist = M.One<T>().Divide(MathUtil.VertexDistEpsilon);
            return ret;
        }
    };


    /*
    //--------------------------------------------------------vertex_dist_cmd
    // Save as the above but with additional "command" Value
    struct vertex_dist_cmd : vertex_dist
    {
        unsigned cmd;

        vertex_dist_cmd() {}
        vertex_dist_cmd(double x_, double y_, unsigned cmd_) :
            base (x_, y_)
            
        {
            cmd = cmd;
        }
    };
     */
}

//#endif
