
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
// classes dda_line_interpolator, dda2_line_interpolator
//
//----------------------------------------------------------------------------
using System;

namespace AGG.Interpolation
{
    //===================================================dda_line_interpolator
    public sealed class DdaLineInterpolator
    {
        int m_y;
        int m_inc;
        int m_dy;
        //int m_YShift;
        int m_FractionShift;

        //--------------------------------------------------------------------
        public DdaLineInterpolator(int FractionShift)
        {
            m_FractionShift = FractionShift;
        }

        //--------------------------------------------------------------------
        public DdaLineInterpolator(int y1, int y2, uint count, int FractionShift)
        {
            m_FractionShift = FractionShift;
            m_y = (y1);
            m_inc = (((y2 - y1) << m_FractionShift) / (int)(count));
            m_dy = (0);
        }

        //--------------------------------------------------------------------
        //public void operator ++ ()
        public void Next()
        {
            m_dy += m_inc;
        }

        //--------------------------------------------------------------------
        //public void operator -- ()
        public void Prev()
        {
            m_dy -= m_inc;
        }

        //--------------------------------------------------------------------
        //public void operator += (uint n)
        public void Next(uint n)
        {
            m_dy += m_inc * (int)n;
        }

        //--------------------------------------------------------------------
        //public void operator -= (uint n)
        public void Prev(int n)
        {
            m_dy -= m_inc * (int)n;
        }


        //--------------------------------------------------------------------
        public int Y { get { return m_y + (m_dy >> (m_FractionShift)); } } // - m_YShift)); }
        public int DY { get { return m_dy; } }
    };

    //=================================================dda2_line_interpolator
    public sealed class Dda2LineInterpolator
    {
        enum save_size_e { save_size = 2 };

        //--------------------------------------------------------------------
        public Dda2LineInterpolator() { }

        //-------------------------------------------- Forward-adjusted line
        public Dda2LineInterpolator(int y1, int y2, int count)
        {
            m_cnt = (count <= 0 ? 1 : count);
            m_lft = ((y2 - y1) / m_cnt);
            m_rem = ((y2 - y1) % m_cnt);
            m_mod = (m_rem);
            m_y = (y1);

            if (m_mod <= 0)
            {
                m_mod += count;
                m_rem += count;
                m_lft--;
            }
            m_mod -= count;
        }

        //-------------------------------------------- Backward-adjusted line
        public Dda2LineInterpolator(int y1, int y2, int count, int unused)
        {
            m_cnt = (count <= 0 ? 1 : count);
            m_lft = ((y2 - y1) / m_cnt);
            m_rem = ((y2 - y1) % m_cnt);
            m_mod = (m_rem);
            m_y = (y1);

            if (m_mod <= 0)
            {
                m_mod += count;
                m_rem += count;
                m_lft--;
            }
        }

        //-------------------------------------------- Backward-adjusted line
        public Dda2LineInterpolator(int y, int count)
        {
            m_cnt = (count <= 0 ? 1 : count);
            m_lft = ((y) / m_cnt);
            m_rem = ((y) % m_cnt);
            m_mod = (m_rem);
            m_y = (0);

            if (m_mod <= 0)
            {
                m_mod += count;
                m_rem += count;
                m_lft--;
            }
        }

        /*
        //--------------------------------------------------------------------
        public void save(save_data_type* data)
        {
            data[0] = m_mod;
            data[1] = m_y;
        }

        //--------------------------------------------------------------------
        public void load(save_data_type* data)
        {
            m_mod = data[0];
            m_y   = data[1];
        }
         */

        //--------------------------------------------------------------------
        //public void operator++()
        public void Next()
        {
            m_mod += m_rem;
            m_y += m_lft;
            if (m_mod > 0)
            {
                m_mod -= m_cnt;
                m_y++;
            }
        }

        //--------------------------------------------------------------------
        //public void operator--()
        public void Prev()
        {
            if (m_mod <= m_rem)
            {
                m_mod += m_cnt;
                m_y--;
            }
            m_mod -= m_rem;
            m_y -= m_lft;
        }

        //--------------------------------------------------------------------
        public void AdjustForward()
        {
            m_mod -= m_cnt;
        }

        //--------------------------------------------------------------------
        public void AdjustBackward()
        {
            m_mod += m_cnt;
        }

        //--------------------------------------------------------------------
        public int Mod { get { return m_mod; } }
        public int Rem { get { return m_rem; } }
        public int Lft { get { return m_lft; } }

        //--------------------------------------------------------------------
        public int Y { get { return m_y; } }

        private int m_cnt;
        private int m_lft;
        private int m_rem;
        private int m_mod;
        private int m_y;
    };


    //---------------------------------------------line_bresenham_interpolator
    public sealed class LineBresenhamInterpolator
    {
        int m_x1_lr;
        int m_y1_lr;
        int m_x2_lr;
        int m_y2_lr;
        bool m_ver;
        uint m_len;
        int m_inc;
        Dda2LineInterpolator m_interpolator;

        public enum SubPixelScale
        {
            Shift = 8,
            Scale = 1 << Shift,
            Mask = Scale - 1
        };

        //--------------------------------------------------------------------
        public static int LineLr(int v) { return v >> (int)SubPixelScale.Shift; }

        //--------------------------------------------------------------------
        public LineBresenhamInterpolator(int x1, int y1, int x2, int y2)
        {
            m_x1_lr = (LineLr(x1));
            m_y1_lr = (LineLr(y1));
            m_x2_lr = (LineLr(x2));
            m_y2_lr = (LineLr(y2));
            m_ver = (Math.Abs(m_x2_lr - m_x1_lr) < Math.Abs(m_y2_lr - m_y1_lr));
            if (m_ver)
            {
                m_len = (uint)Math.Abs(m_y2_lr - m_y1_lr);
            }
            else
            {
                m_len = (uint)Math.Abs(m_x2_lr - m_x1_lr);
            }

            m_inc = (m_ver ? ((y2 > y1) ? 1 : -1) : ((x2 > x1) ? 1 : -1));
            m_interpolator = new Dda2LineInterpolator(m_ver ? x1 : y1,
                           m_ver ? x2 : y2,
                           (int)m_len);
        }

        //--------------------------------------------------------------------
        public bool IsVer { get { return m_ver; } }
        public uint Len { get { return m_len; } }
        public int Inc { get { return m_inc; } }

        //--------------------------------------------------------------------
        public void HStep()
        {
            m_interpolator.Next();
            m_x1_lr += m_inc;
        }

        //--------------------------------------------------------------------
        public void VStep()
        {
            m_interpolator.Next();
            m_y1_lr += m_inc;
        }

        //--------------------------------------------------------------------
        public int X1 { get { return m_x1_lr; } }
        public int Y1 { get { return m_y1_lr; } }
        public int X2 { get { return LineLr(m_interpolator.Y); } }
        public int Y2 { get { return LineLr(m_interpolator.Y); } }
        public int X2Hr { get { return m_interpolator.Y; } }
        public int Y2Hr { get { return m_interpolator.Y; } }
    };
}
