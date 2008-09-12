
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
// Bilinear 2D transformations
//
//----------------------------------------------------------------------------
using NPack.Interfaces;
namespace AGG.Transform
{

    //==========================================================trans_bilinear
    public sealed class Bilinear<T> : ITransform<T>
     where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T[,] m_mtx = new T[4, 2];
        bool m_valid;

        //--------------------------------------------------------------------
        public Bilinear()
        {
            m_valid = (false);
        }

        //--------------------------------------------------------------------
        // Arbitrary quadrangle transformations
        public Bilinear(T[] src, T[] dst)
        {
            QuadToQuad(src, dst);
        }


        //--------------------------------------------------------------------
        // Direct transformations 
        public Bilinear(T x1, T y1, T x2, T y2, T[] quad)
        {
            RectToQuad(x1, y1, x2, y2, quad);
        }


        //--------------------------------------------------------------------
        // Reverse transformations 
        public Bilinear(T[] quad,
                       T x1, T y1, T x2, T y2)
        {
            QuadToRect(quad, x1, y1, x2, y2);
        }


        //--------------------------------------------------------------------
        // Set the transformations using two arbitrary quadrangles.
        public void QuadToQuad(T[] src, T[] dst)
        {
            T[,] left = new T[4, 4];
            T[,] right = new T[4, 2];

            uint i;
            for (i = 0; i < 4; i++)
            {
                uint ix = i * 2;
                uint iy = ix + 1;
                left[i, 0] = M.One<T>();
                left[i, 1] = src[ix].Multiply(src[iy]);
                left[i, 2] = src[ix];
                left[i, 3] = src[iy];

                right[i, 0] = dst[ix];
                right[i, 1] = dst[iy];
            }
            m_valid = SimulEq<T>.Solve(left, right, m_mtx);
        }


        //--------------------------------------------------------------------
        // Set the direct transformations, i.e., rectangle -> quadrangle
        public void RectToQuad(T x1, T y1, T x2, T y2,
                          T[] quad)
        {
            T[] src = new T[8];
            src[0] = src[6] = x1;
            src[2] = src[4] = x2;
            src[1] = src[3] = y1;
            src[5] = src[7] = y2;
            QuadToQuad(src, quad);
        }


        //--------------------------------------------------------------------
        // Set the reverse transformations, i.e., quadrangle -> rectangle
        public void QuadToRect(T[] quad,
                          T x1, T y1, T x2, T y2)
        {
            T[] dst = new T[8];
            dst[0] = dst[6] = x1;
            dst[2] = dst[4] = x2;
            dst[1] = dst[3] = y1;
            dst[5] = dst[7] = y2;
            QuadToQuad(quad, dst);
        }

        //--------------------------------------------------------------------
        // Check if the equations were solved successfully
        public bool IsValid() { return m_valid; }

        //--------------------------------------------------------------------
        // Transform a point (x, y)
        public void Transform(ref T x, ref T y)
        {
            T tx = x;
            T ty = y;
            T xy = tx.Multiply(ty);
            x = m_mtx[0, 0].Add(m_mtx[1, 0].Multiply(xy)).Add(m_mtx[2, 0].Multiply(tx)).Add(m_mtx[3, 0].Multiply(ty));
            y = m_mtx[0, 1].Add(m_mtx[1, 1].Multiply(xy)).Add(m_mtx[2, 1].Multiply(tx)).Add(m_mtx[3, 1].Multiply(ty));
        }


        //--------------------------------------------------------------------
        public sealed class IteratorX
        {
            T inc_x;
            T inc_y;

            public T X;
            public T Y;

            public IteratorX() { }
            public IteratorX(T tx, T ty, T step, T[,] m)
            {
                inc_x = m[1, 0].Multiply(step).Multiply(ty).Add(m[2, 0].Multiply(step));
                inc_y = m[1, 1].Multiply(step).Multiply(ty).Add(m[2, 1].Multiply(step));

                X = m[0, 0].Add(m[1, 0].Multiply(tx).Multiply(ty)).Add(m[2, 0].Multiply(tx)).Add(m[3, 0].Multiply(ty));
                Y = m[0, 1].Add(m[1, 1].Multiply(tx).Multiply(ty)).Add(m[2, 1].Multiply(tx)).Add(m[3, 1].Multiply(ty));
            }

            public static IteratorX operator ++(IteratorX a)
            {
                a.X.AddEquals(a.inc_x);
                a.Y.AddEquals(a.inc_y);

                return a;
            }
        };

        public IteratorX Begin(T x, T y, T step)
        {
            return new IteratorX(x, y, step, m_mtx);
        }
    };
}