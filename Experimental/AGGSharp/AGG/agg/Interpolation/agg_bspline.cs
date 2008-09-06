
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
// class bspline
//
//----------------------------------------------------------------------------

using System;
using NPack.Interfaces;
namespace AGG.Interpolation
{
    //----------------------------------------------------------------bspline
    // A very simple class of Bi-cubic Spline interpolation.
    // First call init(num, x[], y[]) where num - number of source points, 
    // x, y - arrays of X and Y values respectively. Here Y must be a function 
    // of X. It means that all the X-coordinates must be arranged in the ascending
    // order. 
    // Then call get(x) that calculates a Value Y for the respective X. 
    // The class supports extrapolation, i.e. you can call get(x) where x is
    // outside the given with init() X-range. Extrapolation is a simple linear 
    // function.
    //------------------------------------------------------------------------
    public sealed class BSpline<T>
                where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private int m_max;
        private int m_num;
        private int m_xOffset;
        private int m_yOffset;
        private ArrayPOD<T> m_am = new ArrayPOD<T>(16);
        private int m_last_idx;

        //------------------------------------------------------------------------
        public BSpline()
        {
            m_max = (0);
            m_num = (0);
            m_xOffset = (0);
            m_yOffset = (0);
            m_last_idx = -1;
        }

        //------------------------------------------------------------------------
        public BSpline(int num)
        {
            m_max = (0);
            m_num = (0);
            m_xOffset = (0);
            m_yOffset = (0);
            m_last_idx = -1;

            Init(num);
        }

        //------------------------------------------------------------------------
        public BSpline(int num, T[] x, T[] y)
        {
            m_max = (0);
            m_num = (0);
            m_xOffset = (0);
            m_yOffset = (0);
            m_last_idx = -1;
            Init(num, x, y);
        }


        //------------------------------------------------------------------------
        public void Init(int max)
        {
            if (max > 2 && max > m_max)
            {
                m_am.Resize(max * 3);
                m_max = max;
                m_xOffset = m_max;
                m_yOffset = m_max * 2;
            }
            m_num = 0;
            m_last_idx = -1;
        }


        //------------------------------------------------------------------------
        public void AddPoint(T x, T y)
        {
            if (m_num < m_max)
            {
                m_am[m_xOffset + m_num] = x;
                m_am[m_yOffset + m_num] = y;
                ++m_num;
            }
        }


        //------------------------------------------------------------------------
        public void Prepare()
        {
            if (m_num > 2)
            {
                int i, k;
                int r;
                int s;
                T h, p, d, f, e;

                for (k = 0; k < m_num; k++)
                {
                    m_am[k] = M.Zero<T>();
                }

                int n1 = 3 * m_num;

                ArrayPOD<T> al = new ArrayPOD<T>(n1);

                for (k = 0; k < n1; k++)
                {
                    al[k] = M.Zero<T>();
                }

                r = m_num;
                s = m_num * 2;

                n1 = m_num - 1;
                d = m_am[m_xOffset + 1].Subtract(m_am[m_xOffset + 0]);
                e = m_am[m_yOffset + 1].Subtract(m_am[m_yOffset + 0]).Divide(d);

                for (k = 1; k < n1; k++)
                {
                    h = d;
                    d = m_am[m_xOffset + k + 1].Subtract(m_am[m_xOffset + k]);
                    f = e;
                    e = m_am[m_yOffset + k + 1].Subtract(m_am[m_yOffset + k]).Divide(d);
                    al[k] = d.Divide(d.Add(h));
                    al[r + k] = M.One<T>().Subtract(al[k]);
                    al[s + k] = M.New<T>(6.0).Multiply(e.Subtract(f).Divide(h.Add(d)));
                }

                for (k = 1; k < n1; k++)
                {
                    p = M.One<T>().Divide(al[r + k].Multiply(al[k - 1]).Add(2.0));
                    al[k].MultiplyEquals(p.Negative());
                    al[s + k] = al[s + k].Subtract(al[r + k].Multiply(al[s + k - 1])).Multiply(p);
                }

                m_am[n1] = M.Zero<T>();
                al[n1 - 1] = al[s + n1 - 1];
                m_am[n1 - 1] = al[n1 - 1];

                for (k = n1 - 2, i = 0; i < m_num - 2; i++, k--)
                {
                    al[k] = al[k].Multiply(al[k + 1]).Add(al[s + k]);
                    m_am[k] = al[k];
                }
            }
            m_last_idx = -1;
        }



        //------------------------------------------------------------------------
        public void Init(int num, T[] x, T[] y)
        {
            if (num > 2)
            {
                Init(num);
                int i;
                for (i = 0; i < num; i++)
                {
                    AddPoint(x[i], y[i]);
                }
                Prepare();
            }
            m_last_idx = -1;
        }


        //------------------------------------------------------------------------
        void BSearch(int n, int xOffset, T x0, out int i)
        {
            int j = n - 1;
            int k;

            for (i = 0; (j - i) > 1; )
            {
                k = (i + j) >> 1;
                if (x0.LessThan(m_am[xOffset + k])) j = k;
                else i = k;
            }
        }



        //------------------------------------------------------------------------
        T Interpolation(T x, int i)
        {
            int j = i + 1;
            T d = m_am[m_xOffset + i].Subtract(m_am[m_xOffset + j]);
            T h = x.Subtract(m_am[m_xOffset + j]);
            T r = m_am[m_xOffset + i].Subtract(x);
            T p = d.Multiply(d).Divide(6.0);
            return (m_am[j].Multiply(r).Multiply(r).Multiply(r).Add(
                    m_am[i].Multiply(h).Multiply(h).Multiply(h).Divide(6.0).Divide(d)
                ).Add(
                   m_am[m_yOffset + j].Subtract(m_am[j].Multiply(p).Multiply(r).Add(
                        m_am[m_yOffset + i].Subtract(m_am[i].Multiply(p))).Multiply(h)))).Divide(d);
        }


        //------------------------------------------------------------------------
        //what are the chances I haven't battered this statement?
        T ExtrapolationLeft(T x)
        {
            T d = m_am[m_xOffset + 1].Subtract(m_am[m_xOffset + 0]);
            return d.Negative().Multiply(
                    m_am[1]).Divide(6).Add(
                        m_am[m_yOffset + 1].Subtract(m_am[m_yOffset + 0]).Divide(d).Multiply
                   (x.Subtract(m_am[m_xOffset + 0]).Add(
                   m_am[m_yOffset + 0])));
        }

        //------------------------------------------------------------------------
        T ExtrapolationRight(T x)
        {
            T d = m_am[m_xOffset + m_num - 1].Subtract(m_am[m_xOffset + m_num - 2]);
            return (d.Multiply(m_am[m_num - 2]).Divide(6).Add(m_am[m_yOffset + m_num - 1].Subtract(m_am[m_yOffset + m_num - 2])).Divide(d)).Multiply
                   (x.Subtract(m_am[m_xOffset + m_num - 1])).Add(
                   m_am[m_yOffset + m_num - 1]);
        }

        //------------------------------------------------------------------------
        public T Get(T x)
        {
            if (m_num > 2)
            {
                int i;

                // Extrapolation on the left
                if (x.LessThan(m_am[m_xOffset + 0])) return ExtrapolationLeft(x);

                // Extrapolation on the right
                if (x.GreaterThanOrEqualTo(m_am[m_xOffset + m_num - 1])) return ExtrapolationRight(x);

                // Interpolation
                BSearch(m_num, m_xOffset, x, out i);
                return Interpolation(x, i);
            }
            return M.Zero<T>();
        }


        //------------------------------------------------------------------------
        public T GetStateful(T x)
        {
            if (m_num > 2)
            {
                // Extrapolation on the left
                if (x.LessThan(m_am[m_xOffset + 0])) return ExtrapolationLeft(x);

                // Extrapolation on the right
                if (x.GreaterThanOrEqualTo(m_am[m_xOffset + m_num - 1])) return ExtrapolationRight(x);

                if (m_last_idx >= 0)
                {
                    // Check if x is not in current range
                    if (x.LessThan(m_am[m_xOffset + m_last_idx]) || x.GreaterThan(m_am[m_xOffset + m_last_idx + 1]))
                    {
                        // Check if x between next points (most probably)
                        if (m_last_idx < m_num - 2 &&
                           x.GreaterThanOrEqualTo(m_am[m_xOffset + m_last_idx + 1]) &&
                           x.LessThanOrEqualTo(m_am[m_xOffset + m_last_idx + 2]))
                        {
                            ++m_last_idx;
                        }
                        else
                            if (m_last_idx > 0 &&
                               x.GreaterThanOrEqualTo(m_am[m_xOffset + m_last_idx - 1]) &&
                               x.LessThanOrEqualTo(m_am[m_xOffset + m_last_idx]))
                            {
                                // x is between pevious points
                                --m_last_idx;
                            }
                            else
                            {
                                // Else perform full search
                                BSearch(m_num, m_xOffset, x, out m_last_idx);
                            }
                    }
                    return Interpolation(x, m_last_idx);
                }
                else
                {
                    // Interpolation
                    BSearch(m_num, m_xOffset, x, out m_last_idx);
                    return Interpolation(x, m_last_idx);
                }
            }
            return M.Zero<T>();
        }
    };
}
