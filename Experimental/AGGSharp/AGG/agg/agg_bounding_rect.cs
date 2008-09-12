
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
//
// bounding_rect function template
//
//----------------------------------------------------------------------------
using AGG.VertexSource;
using NPack.Interfaces;

namespace AGG
{
    static public class BoundingRect<T> where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        //-----------------------------------------------------------bounding_rect
        //template<class VertexSource, class GetId, class CoordT>
        public static bool GetBoundingRect(PathStorage<T> vs, uint[] gi,
                           uint start, uint num,
                           out T x1, out T y1, out T x2, out T y2)
        {
            uint i;
            T x = default(T).Zero;
            T y = default(T).Zero;
            bool first = true;

            x1 = default(T).One;
            y1 = default(T).One;
            x2 = default(T).Zero;
            y2 = default(T).Zero;

            for (i = 0; i < num; i++)
            {
                vs.Rewind(gi[start + i]);
                uint PathAndFlags;
                while (!Path.IsStop(PathAndFlags = vs.Vertex(out x, out y)))
                {
                    if (Path.IsVertex(PathAndFlags))
                    {
                        if (first)
                        {
                            x1 = (x);
                            y1 = (y);
                            x2 = (x);
                            y2 = (y);
                            first = false;
                        }
                        else
                        {
                            if (x.LessThan(x1)) x1 = x;
                            if (y.LessThan(y1)) y1 = y;
                            if (x.GreaterThan(x2)) x2 = x;
                            if (y.GreaterThan(y2)) y2 = y;
                        }
                    }
                }
            }
            return x1.LessThanOrEqualTo(x2) && y1.LessThanOrEqualTo(y2);
        }

        public static bool BoundingRectSingle(IVertexSource<T> vs, uint path_id, ref RectDouble<T> rect)
        {
            T x1, y1, x2, y2;
            bool rValue = BoundingRectSingle(vs, path_id, out x1, out y1, out x2, out y2);
            rect.Left = x1;
            rect.Bottom = y1;
            rect.Right = x2;
            rect.Top = y2;
            return rValue;
        }

        //-----------------------------------------------------bounding_rect_single
        //template<class VertexSource, class CoordT> 
        public static bool BoundingRectSingle(IVertexSource<T> vs, uint path_id,
                                  out T x1, out T y1, out T x2, out T y2)
        {
            T x = default(T).Zero;
            T y = default(T).Zero;
            bool first = true;

            x1 = default(T).One;
            y1 = default(T).One;
            x2 = default(T).Zero;
            y2 = default(T).Zero;

            vs.Rewind(path_id);
            uint PathAndFlags;
            while (!Path.IsStop(PathAndFlags = vs.Vertex(out x, out y)))
            {
                if (Path.IsVertex(PathAndFlags))
                {
                    if (first)
                    {
                        x1 = x;
                        y1 = y;
                        x2 = x;
                        y2 = y;
                        first = false;
                    }
                    else
                    {
                        if (x.LessThan(x1)) x1 = x;
                        if (y.LessThan(y1)) y1 = y;
                        if (x.GreaterThan(x2)) x2 = x;
                        if (y.GreaterThan(y2)) y2 = y;
                    }
                }
            }
            return x1.LessThanOrEqualTo(x2) && y1.LessThanOrEqualTo(y2);
        }
    };
}

//#endif
