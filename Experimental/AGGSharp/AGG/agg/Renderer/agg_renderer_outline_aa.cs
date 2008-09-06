
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

namespace AGG.Rendering
{
    //===================================================distance_interpolator0
    public class DistanceInterpolator0
    {
        int m_dx;
        int m_dy;
        int m_dist;

        //---------------------------------------------------------------------
        public DistanceInterpolator0() { }
        public DistanceInterpolator0(int x1, int y1, int x2, int y2, int x, int y)
        {
            m_dx = (LineAABasics.LineMr(x2) - LineAABasics.LineMr(x1));
            m_dy = (LineAABasics.LineMr(y2) - LineAABasics.LineMr(y1));
            m_dist = ((LineAABasics.LineMr(x + LineAABasics.LineSubPixelScale / 2) - LineAABasics.LineMr(x2)) * m_dy -
                   (LineAABasics.LineMr(y + LineAABasics.LineSubPixelScale / 2) - LineAABasics.LineMr(y2)) * m_dx);

            m_dx <<= LineAABasics.LineMrSubPixelShift;
            m_dy <<= LineAABasics.LineMrSubPixelShift;
        }

        //---------------------------------------------------------------------
        public void IncX() { m_dist += m_dy; }
        public int Dist { get { return m_dist; } }
    };

    //==================================================distance_interpolator00
    public class distance_interpolator00
    {
        int m_dx1;
        int m_dy1;
        int m_dx2;
        int m_dy2;
        int m_dist1;
        int m_dist2;

        //---------------------------------------------------------------------
        public distance_interpolator00() { }
        public distance_interpolator00(int xc, int yc,
                                int x1, int y1, int x2, int y2,
                                int x, int y)
        {
            m_dx1 = (LineAABasics.LineMr(x1) - LineAABasics.LineMr(xc));
            m_dy1 = (LineAABasics.LineMr(y1) - LineAABasics.LineMr(yc));
            m_dx2 = (LineAABasics.LineMr(x2) - LineAABasics.LineMr(xc));
            m_dy2 = (LineAABasics.LineMr(y2) - LineAABasics.LineMr(yc));
            m_dist1 = ((LineAABasics.LineMr(x + LineAABasics.LineSubPixelScale / 2) - LineAABasics.LineMr(x1)) * m_dy1 -
                    (LineAABasics.LineMr(y + LineAABasics.LineSubPixelScale / 2) - LineAABasics.LineMr(y1)) * m_dx1);
            m_dist2 = ((LineAABasics.LineMr(x + LineAABasics.LineSubPixelScale / 2) - LineAABasics.LineMr(x2)) * m_dy2 -
                    (LineAABasics.LineMr(y + LineAABasics.LineSubPixelScale / 2) - LineAABasics.LineMr(y2)) * m_dx2);

            m_dx1 <<= LineAABasics.LineMrSubPixelShift;
            m_dy1 <<= LineAABasics.LineMrSubPixelShift;
            m_dx2 <<= LineAABasics.LineMrSubPixelShift;
            m_dy2 <<= LineAABasics.LineMrSubPixelShift;
        }

        //---------------------------------------------------------------------
        public void IncX() { m_dist1 += m_dy1; m_dist2 += m_dy2; }
        public int Dist1 { get { return m_dist1; } }
        public int Dist2 { get { return m_dist2; } }
    };

    //===================================================distance_interpolator1
    public class DistanceInterpolator1
    {
        int m_dx;
        int m_dy;
        int m_dist;

        //---------------------------------------------------------------------
        public DistanceInterpolator1() { }
        public DistanceInterpolator1(int x1, int y1, int x2, int y2, int x, int y)
        {
            m_dx = (x2 - x1);
            m_dy = (y2 - y1);
            m_dist = (Basics.RoundInt((double)(x + LineAABasics.LineSubPixelScale / 2 - x2) * (double)(m_dy) -
                          (double)(y + LineAABasics.LineSubPixelScale / 2 - y2) * (double)(m_dx)));

            m_dx <<= LineAABasics.LineSubPixelShift;
            m_dy <<= LineAABasics.LineSubPixelShift;
        }

        //---------------------------------------------------------------------
        public void IncX() { m_dist += m_dy; }
        public void DecX() { m_dist -= m_dy; }
        public void IncY() { m_dist -= m_dx; }
        public void DecY() { m_dist += m_dx; }

        //---------------------------------------------------------------------
        public void IncX(int dy)
        {
            m_dist += m_dy;
            if (dy > 0) m_dist -= m_dx;
            if (dy < 0) m_dist += m_dx;
        }

        //---------------------------------------------------------------------
        public void dec_x(int dy)
        {
            m_dist -= m_dy;
            if (dy > 0) m_dist -= m_dx;
            if (dy < 0) m_dist += m_dx;
        }

        //---------------------------------------------------------------------
        public void inc_y(int dx)
        {
            m_dist -= m_dx;
            if (dx > 0) m_dist += m_dy;
            if (dx < 0) m_dist -= m_dy;
        }

        public void dec_y(int dx)
        //---------------------------------------------------------------------
        {
            m_dist += m_dx;
            if (dx > 0) m_dist += m_dy;
            if (dx < 0) m_dist -= m_dy;
        }

        //---------------------------------------------------------------------
        public int dist() { return m_dist; }
        public int dx() { return m_dx; }
        public int dy() { return m_dy; }
    };





    //===================================================distance_interpolator2
    public class DistanceInterpolator2
    {
        int m_dx;
        int m_dy;
        int m_dx_start;
        int m_dy_start;

        int m_dist;
        int m_dist_start;

        //---------------------------------------------------------------------
        public DistanceInterpolator2() { }
        public DistanceInterpolator2(int x1, int y1, int x2, int y2,
                               int sx, int sy, int x, int y)
        {
            m_dx = (x2 - x1);
            m_dy = (y2 - y1);
            m_dx_start = (LineAABasics.LineMr(sx) - LineAABasics.LineMr(x1));
            m_dy_start = (LineAABasics.LineMr(sy) - LineAABasics.LineMr(y1));

            m_dist = (Basics.RoundInt((double)(x + LineAABasics.LineSubPixelScale / 2 - x2) * (double)(m_dy) -
                          (double)(y + LineAABasics.LineSubPixelScale / 2 - y2) * (double)(m_dx)));

            m_dist_start = ((LineAABasics.LineMr(x + LineAABasics.LineSubPixelScale / 2) - LineAABasics.LineMr(sx)) * m_dy_start -
                         (LineAABasics.LineMr(y + LineAABasics.LineSubPixelScale / 2) - LineAABasics.LineMr(sy)) * m_dx_start);

            m_dx <<= LineAABasics.LineSubPixelShift;
            m_dy <<= LineAABasics.LineSubPixelShift;
            m_dx_start <<= LineAABasics.LineMrSubPixelShift;
            m_dy_start <<= LineAABasics.LineMrSubPixelShift;
        }

        public DistanceInterpolator2(int x1, int y1, int x2, int y2,
                               int ex, int ey, int x, int y, int none)
        {
            m_dx = (x2 - x1);
            m_dy = (y2 - y1);
            m_dx_start = (LineAABasics.LineMr(ex) - LineAABasics.LineMr(x2));
            m_dy_start = (LineAABasics.LineMr(ey) - LineAABasics.LineMr(y2));

            m_dist = (Basics.RoundInt((double)(x + LineAABasics.LineSubPixelScale / 2 - x2) * (double)(m_dy) -
                          (double)(y + LineAABasics.LineSubPixelScale / 2 - y2) * (double)(m_dx)));

            m_dist_start = ((LineAABasics.LineMr(x + LineAABasics.LineSubPixelScale / 2) - LineAABasics.LineMr(ex)) * m_dy_start -
                         (LineAABasics.LineMr(y + LineAABasics.LineSubPixelScale / 2) - LineAABasics.LineMr(ey)) * m_dx_start);

            m_dx <<= LineAABasics.LineSubPixelShift;
            m_dy <<= LineAABasics.LineSubPixelShift;
            m_dx_start <<= LineAABasics.LineMrSubPixelShift;
            m_dy_start <<= LineAABasics.LineMrSubPixelShift;
        }


        //---------------------------------------------------------------------
        public void IncX() { m_dist += m_dy; m_dist_start += m_dy_start; }
        public void DecX() { m_dist -= m_dy; m_dist_start -= m_dy_start; }
        public void IncY() { m_dist -= m_dx; m_dist_start -= m_dx_start; }
        public void DecY() { m_dist += m_dx; m_dist_start += m_dx_start; }

        //---------------------------------------------------------------------
        public void IncX(int dy)
        {
            m_dist += m_dy;
            m_dist_start += m_dy_start;
            if (dy > 0)
            {
                m_dist -= m_dx;
                m_dist_start -= m_dx_start;
            }
            if (dy < 0)
            {
                m_dist += m_dx;
                m_dist_start += m_dx_start;
            }
        }

        //---------------------------------------------------------------------
        public void DecX(int dy)
        {
            m_dist -= m_dy;
            m_dist_start -= m_dy_start;
            if (dy > 0)
            {
                m_dist -= m_dx;
                m_dist_start -= m_dx_start;
            }
            if (dy < 0)
            {
                m_dist += m_dx;
                m_dist_start += m_dx_start;
            }
        }

        //---------------------------------------------------------------------
        public void IncY(int dx)
        {
            m_dist -= m_dx;
            m_dist_start -= m_dx_start;
            if (dx > 0)
            {
                m_dist += m_dy;
                m_dist_start += m_dy_start;
            }
            if (dx < 0)
            {
                m_dist -= m_dy;
                m_dist_start -= m_dy_start;
            }
        }

        //---------------------------------------------------------------------
        public void DecY(int dx)
        {
            m_dist += m_dx;
            m_dist_start += m_dx_start;
            if (dx > 0)
            {
                m_dist += m_dy;
                m_dist_start += m_dy_start;
            }
            if (dx < 0)
            {
                m_dist -= m_dy;
                m_dist_start -= m_dy_start;
            }
        }

        //---------------------------------------------------------------------
        public int Dist { get { return m_dist; } }
        public int DistStart { get { return m_dist_start; } }
        public int DistEnd { get { return m_dist_start; } }

        //---------------------------------------------------------------------
        public int DX { get { return m_dx; } }
        public int DY { get { return m_dy; } }
        public int DXStart { get { return m_dx_start; } }
        public int DYStart { get { return m_dy_start; } }
        public int DXEnd { get { return m_dx_start; } }
        public int DYEnd { get { return m_dy_start; } }
    };





    //===================================================distance_interpolator3
    public class DistanceInterpolator3
    {
        int m_dx;
        int m_dy;
        int m_dx_start;
        int m_dy_start;
        int m_dx_end;
        int m_dy_end;

        int m_dist;
        int m_dist_start;
        int m_dist_end;

        //---------------------------------------------------------------------
        public DistanceInterpolator3() { }
        public DistanceInterpolator3(int x1, int y1, int x2, int y2,
                               int sx, int sy, int ex, int ey,
                               int x, int y)
        {
            m_dx = (x2 - x1);
            m_dy = (y2 - y1);
            m_dx_start = (LineAABasics.LineMr(sx) - LineAABasics.LineMr(x1));
            m_dy_start = (LineAABasics.LineMr(sy) - LineAABasics.LineMr(y1));
            m_dx_end = (LineAABasics.LineMr(ex) - LineAABasics.LineMr(x2));
            m_dy_end = (LineAABasics.LineMr(ey) - LineAABasics.LineMr(y2));

            m_dist = (Basics.RoundInt((double)(x + LineAABasics.LineSubPixelScale / 2 - x2) * (double)(m_dy) -
                          (double)(y + LineAABasics.LineSubPixelScale / 2 - y2) * (double)(m_dx)));

            m_dist_start = ((LineAABasics.LineMr(x + LineAABasics.LineSubPixelScale / 2) - LineAABasics.LineMr(sx)) * m_dy_start -
                         (LineAABasics.LineMr(y + LineAABasics.LineSubPixelScale / 2) - LineAABasics.LineMr(sy)) * m_dx_start);

            m_dist_end = ((LineAABasics.LineMr(x + LineAABasics.LineSubPixelScale / 2) - LineAABasics.LineMr(ex)) * m_dy_end -
                       (LineAABasics.LineMr(y + LineAABasics.LineSubPixelScale / 2) - LineAABasics.LineMr(ey)) * m_dx_end);

            m_dx <<= LineAABasics.LineSubPixelShift;
            m_dy <<= LineAABasics.LineSubPixelShift;
            m_dx_start <<= LineAABasics.LineMrSubPixelShift;
            m_dy_start <<= LineAABasics.LineMrSubPixelShift;
            m_dx_end <<= LineAABasics.LineMrSubPixelShift;
            m_dy_end <<= LineAABasics.LineMrSubPixelShift;
        }

        //---------------------------------------------------------------------
        void IncX() { m_dist += m_dy; m_dist_start += m_dy_start; m_dist_end += m_dy_end; }
        void DecX() { m_dist -= m_dy; m_dist_start -= m_dy_start; m_dist_end -= m_dy_end; }
        void IncY() { m_dist -= m_dx; m_dist_start -= m_dx_start; m_dist_end -= m_dx_end; }
        void DecY() { m_dist += m_dx; m_dist_start += m_dx_start; m_dist_end += m_dx_end; }

        //---------------------------------------------------------------------
        void IncX(int dy)
        {
            m_dist += m_dy;
            m_dist_start += m_dy_start;
            m_dist_end += m_dy_end;
            if (dy > 0)
            {
                m_dist -= m_dx;
                m_dist_start -= m_dx_start;
                m_dist_end -= m_dx_end;
            }
            if (dy < 0)
            {
                m_dist += m_dx;
                m_dist_start += m_dx_start;
                m_dist_end += m_dx_end;
            }
        }

        //---------------------------------------------------------------------
        void DecX(int dy)
        {
            m_dist -= m_dy;
            m_dist_start -= m_dy_start;
            m_dist_end -= m_dy_end;
            if (dy > 0)
            {
                m_dist -= m_dx;
                m_dist_start -= m_dx_start;
                m_dist_end -= m_dx_end;
            }
            if (dy < 0)
            {
                m_dist += m_dx;
                m_dist_start += m_dx_start;
                m_dist_end += m_dx_end;
            }
        }

        //---------------------------------------------------------------------
        void IncY(int dx)
        {
            m_dist -= m_dx;
            m_dist_start -= m_dx_start;
            m_dist_end -= m_dx_end;
            if (dx > 0)
            {
                m_dist += m_dy;
                m_dist_start += m_dy_start;
                m_dist_end += m_dy_end;
            }
            if (dx < 0)
            {
                m_dist -= m_dy;
                m_dist_start -= m_dy_start;
                m_dist_end -= m_dy_end;
            }
        }

        //---------------------------------------------------------------------
        void DecY(int dx)
        {
            m_dist += m_dx;
            m_dist_start += m_dx_start;
            m_dist_end += m_dx_end;
            if (dx > 0)
            {
                m_dist += m_dy;
                m_dist_start += m_dy_start;
                m_dist_end += m_dy_end;
            }
            if (dx < 0)
            {
                m_dist -= m_dy;
                m_dist_start -= m_dy_start;
                m_dist_end -= m_dy_end;
            }
        }

        //---------------------------------------------------------------------
        int Dist { get { return m_dist; } }
        int DistStart { get { return m_dist_start; } }
        int DistEnd { get { return m_dist_end; } }

        //---------------------------------------------------------------------
        int DX { get { return m_dx; } }
        int DY { get { return m_dy; } }
        int DXStart { get { return m_dx_start; } }
        int DYStart { get { return m_dy_start; } }
        int DXEnd { get { return m_dx_end; } }
        int DYEnd { get { return m_dy_end; } }
    };

    /*
    //================================================line_interpolator_aa_base
    public class line_interpolator_aa_base
    {
        line_parameters m_lp;
        public dda2_line_interpolator m_li;
        IRasterizer m_ren;
        int m_len;
        protected int m_x;
        protected int m_y;
        int m_old_x;
        int m_old_y;
        int m_count;
        int m_width;
        int m_max_extent;
        int m_step;
        int[] m_dist = new int[max_half_width + 1];
        byte[] m_covers = new byte[max_half_width * 2 + 4];
        //typedef Renderer renderer_type;

        //---------------------------------------------------------------------
        const int max_half_width = 64;

        //---------------------------------------------------------------------
        public line_interpolator_aa_base(IRasterizer ren, line_parameters lp)
        {
            m_lp=lp;
            m_li = new dda2_line_interpolator(lp.vertical ? LineAABasics.line_dbl_hr(lp.x2 - lp.x1) : LineAABasics.line_dbl_hr(lp.y2 - lp.y1),
                lp.vertical ? Math.Abs(lp.y2 - lp.y1) : Math.Abs(lp.x2 - lp.x1) + 1);
            m_ren=(ren);
            m_len=((lp.vertical == (lp.inc > 0)) ? -lp.len : lp.len);
            m_x=(lp.x1 >> LineAABasics.line_subpixel_shift);
            m_y=(lp.y1 >> LineAABasics.line_subpixel_shift);
            m_old_x=(m_x);
            m_old_y=(m_y);
            m_count = ((lp.vertical ? Math.Abs((lp.y2 >> LineAABasics.line_subpixel_shift) - m_y) :
                                   Math.Abs((lp.x2 >> LineAABasics.line_subpixel_shift) - m_x)));
            m_width=(ren.subpixel_width());
            //m_max_extent(m_width >> (line_subpixel_shift - 2));
            m_max_extent=((m_width + LineAABasics.line_subpixel_mask) >> LineAABasics.line_subpixel_shift);
            m_step=0;

            dda2_line_interpolator li = new dda2_line_interpolator(0, 
                lp.vertical ? (lp.dy << LineAABasics.line_subpixel_shift) : (lp.dx << LineAABasics.line_subpixel_shift),
                lp.len);

            uint i;
            int stop = m_width + LineAABasics.line_subpixel_scale * 2;
            for(i = 0; i < max_half_width; ++i)
            {
                m_dist[i] = li.y();
                if(m_dist[i] >= stop) break;
                li.Next();
            }
            m_dist[i++] = 0x7FFF0000;
        }

        //---------------------------------------------------------------------
        public int step_hor_base(distance_interpolator1 di)
        {
            m_li.Next();
            m_x += m_lp.inc;
            m_y = (m_lp.y1 + m_li.y()) >> LineAABasics.line_subpixel_shift;

            if (m_lp.inc > 0) di.inc_x(m_y - m_old_y);
            else              di.dec_x(m_y - m_old_y);

            m_old_y = m_y;

            return di.dist() / m_len;
        }

        //---------------------------------------------------------------------
        public int step_ver_base(distance_interpolator1 di)
        {
            m_li.Next();
            m_y += m_lp.inc;
            m_x = (m_lp.x1 + m_li.y()) >> LineAABasics.line_subpixel_shift;

            if (m_lp.inc > 0) di.inc_y(m_x - m_old_x);
            else              di.dec_y(m_x - m_old_x);

            m_old_x = m_x;

            return di.dist() / m_len;
        }

        //---------------------------------------------------------------------
        public bool vertical() { return m_lp.vertical; }
        public int  width() { return m_width; }
        public int  count() { return m_count; }

    };

    //====================================================line_interpolator_aa0
    public class line_interpolator_aa0 : line_interpolator_aa_base
    {
        distance_interpolator1 m_di; 
        //typedef Renderer renderer_type;
        //typedef line_interpolator_aa_base<Renderer> base_type;

        //---------------------------------------------------------------------
        public line_interpolator_aa0(IRasterizer ren, line_parameters lp)
            :
            base(ren, lp)
        {
            m_di = new distance_interpolator1(lp.x1, lp.y1, lp.x2, lp.y2,
                 lp.x1 & ~LineAABasics.line_subpixel_mask, lp.y1 & ~LineAABasics.line_subpixel_mask);

            m_li.adjust_forward();
        }

        //---------------------------------------------------------------------
        public bool step_hor()
        {
            int dist;
            int dy;
            int s1 = base.step_hor_base(m_di);
            byte* p0 = base.m_covers + base.max_half_width + 2;
            byte* p1 = p0;

            *p1++ = (byte)base.m_ren.cover(s1);

            dy = 1;
            while((dist = base.m_dist[dy] - s1) <= base.m_width)
            {
                *p1++ = (byte)base.m_ren.cover(dist);
                ++dy;
            }

            dy = 1;
            while((dist = base.m_dist[dy] + s1) <= base.m_width)
            {
                *--p0 = (byte)base.m_ren.cover(dist);
                ++dy;
            }
            base.m_ren.blend_solid_vspan(base.m_x, 
                                               base.m_y - dy + 1, 
                                               (uint)(p1 - p0), 
                                               p0);
            return ++base.m_step < base.m_count;
        }

        //---------------------------------------------------------------------
        public bool step_ver()
        {
            int dist;
            int dx;
            int s1 = base.step_ver_base(m_di);
            byte* p0 = base.m_covers + base.max_half_width + 2;
            byte* p1 = p0;

            *p1++ = (byte)base.m_ren.cover(s1);

            dx = 1;
            while((dist = base.m_dist[dx] - s1) <= base.m_width)
            {
                *p1++ = (byte)base.m_ren.cover(dist);
                ++dx;
            }

            dx = 1;
            while((dist = base.m_dist[dx] + s1) <= base.m_width)
            {
                *--p0 = (byte)base.m_ren.cover(dist);
                ++dx;
            }
            base.m_ren.blend_solid_hspan(base.m_x - dx + 1, 
                                               base.m_y,
                                               uint(p1 - p0), 
                                               p0);
            return ++base.m_step < base.m_count;
        }
    };

    //====================================================line_interpolator_aa1
    public class line_interpolator_aa1 : line_interpolator_aa_base
    {
        distance_interpolator2 m_di; 
        //typedef Renderer renderer_type;
        //typedef line_interpolator_aa_base<Renderer> base_type;

        //---------------------------------------------------------------------
        public line_interpolator_aa1(IRasterizer ren, line_parameters lp, 
                              int sx, int sy) :
            base(ren, lp)
        {
            m_di = new distance_interpolator2(lp.x1, lp.y1, lp.x2, lp.y2, sx, sy,
                 lp.x1 & ~line_subpixel_mask, lp.y1 & ~line_subpixel_mask);

            int dist1_start;
            int dist2_start;

            int npix = 1;

            if(lp.vertical)
            {
                do
                {
                    --base.m_li;
                    base.m_y -= lp.inc;
                    base.m_x = (base.m_lp.x1 + base.m_li.y()) >> LineAABasics.line_subpixel_shift;

                    if(lp.inc > 0) m_di.dec_y(base.m_x - base.m_old_x);
                    else           m_di.inc_y(base.m_x - base.m_old_x);

                    base.m_old_x = base.m_x;

                    dist1_start = dist2_start = m_di.dist_start(); 

                    int dx = 0;
                    if(dist1_start < 0) ++npix;
                    do
                    {
                        dist1_start += m_di.dy_start();
                        dist2_start -= m_di.dy_start();
                        if(dist1_start < 0) ++npix;
                        if(dist2_start < 0) ++npix;
                        ++dx;
                    }
                    while(base.m_dist[dx] <= base.m_width);
                    --base.m_step;
                    if(npix == 0) break;
                    npix = 0;
                }
                while(base.m_step >= -base.m_max_extent);
            }
            else
            {
                do
                {
                    --base.m_li;
                    base.m_x -= lp.inc;
                    base.m_y = (base.m_lp.y1 + base.m_li.y()) >> LineAABasics.line_subpixel_shift;

                    if(lp.inc > 0) m_di.dec_x(base.m_y - base.m_old_y);
                    else           m_di.inc_x(base.m_y - base.m_old_y);

                    base.m_old_y = base.m_y;

                    dist1_start = dist2_start = m_di.dist_start(); 

                    int dy = 0;
                    if(dist1_start < 0) ++npix;
                    do
                    {
                        dist1_start -= m_di.dx_start();
                        dist2_start += m_di.dx_start();
                        if(dist1_start < 0) ++npix;
                        if(dist2_start < 0) ++npix;
                        ++dy;
                    }
                    while(base.m_dist[dy] <= base.m_width);
                    --base.m_step;
                    if(npix == 0) break;
                    npix = 0;
                }
                while(base.m_step >= -base.m_max_extent);
            }
            base.m_li.adjust_forward();
        }

        //---------------------------------------------------------------------
        public bool step_hor()
        {
            int dist_start;
            int dist;
            int dy;
            int s1 = base.step_hor_base(m_di);

            dist_start = m_di.dist_start();
            byte* p0 = base.m_covers + base.max_half_width + 2;
            byte* p1 = p0;

            *p1 = 0;
            if(dist_start <= 0)
            {
                *p1 = (byte)base.m_ren.cover(s1);
            }
            ++p1;

            dy = 1;
            while((dist = base.m_dist[dy] - s1) <= base.m_width)
            {
                dist_start -= m_di.dx_start();
                *p1 = 0;
                if(dist_start <= 0)
                {   
                    *p1 = (byte)base.m_ren.cover(dist);
                }
                ++p1;
                ++dy;
            }

            dy = 1;
            dist_start = m_di.dist_start();
            while((dist = base.m_dist[dy] + s1) <= base.m_width)
            {
                dist_start += m_di.dx_start();
                *--p0 = 0;
                if(dist_start <= 0)
                {   
                    *p0 = (byte)base.m_ren.cover(dist);
                }
                ++dy;
            }

            base.m_ren.blend_solid_vspan(base.m_x, 
                                               base.m_y - dy + 1,
                                               uint(p1 - p0), 
                                               p0);
            return ++base.m_step < base.m_count;
        }

        //---------------------------------------------------------------------
        public bool step_ver()
        {
            int dist_start;
            int dist;
            int dx;
            int s1 = base.step_ver_base(m_di);
            byte* p0 = base.m_covers + base.max_half_width + 2;
            byte* p1 = p0;

            dist_start = m_di.dist_start();

            *p1 = 0;
            if(dist_start <= 0)
            {
                *p1 = (byte)base.m_ren.cover(s1);
            }
            ++p1;

            dx = 1;
            while((dist = base.m_dist[dx] - s1) <= base.m_width)
            {
                dist_start += m_di.dy_start();
                *p1 = 0;
                if(dist_start <= 0)
                {   
                    *p1 = (byte)base.m_ren.cover(dist);
                }
                ++p1;
                ++dx;
            }

            dx = 1;
            dist_start = m_di.dist_start();
            while((dist = base.m_dist[dx] + s1) <= base.m_width)
            {
                dist_start -= m_di.dy_start();
                *--p0 = 0;
                if(dist_start <= 0)
                {   
                    *p0 = (byte)base.m_ren.cover(dist);
                }
                ++dx;
            }
            base.m_ren.blend_solid_hspan(base.m_x - dx + 1, 
                                               base.m_y,
                                               (uint)(p1 - p0), 
                                               p0);

            return ++base.m_step < base.m_count;
        }
    };

    //====================================================line_interpolator_aa2
    public class line_interpolator_aa2 : line_interpolator_aa_base
    {
        distance_interpolator2 m_di; 
        //typedef Renderer renderer_type;
        //typedef line_interpolator_aa_base<Renderer> base_type;

        //---------------------------------------------------------------------
        public line_interpolator_aa2(IRasterizer ren, line_parameters lp, 
                              int ex, int ey) :
            base(ren, lp)
        {
            m_di = new distance_interpolator2(lp.x1, lp.y1, lp.x2, lp.y2, ex, ey, 
                 lp.x1 & ~line_subpixel_mask, lp.y1 & ~line_subpixel_mask,
                 0);
            base.m_li.adjust_forward();
            base.m_step -= base.m_max_extent;
        }

        //---------------------------------------------------------------------
        public bool step_hor()
        {
            int dist_end;
            int dist;
            int dy;
            int s1 = base.step_hor_base(m_di);
            byte* p0 = base.m_covers + base.max_half_width + 2;
            byte* p1 = p0;

            dist_end = m_di.dist_end();

            int npix = 0;
            *p1 = 0;
            if(dist_end > 0)
            {
                *p1 = (byte)base.m_ren.cover(s1);
                ++npix;
            }
            ++p1;

            dy = 1;
            while((dist = base.m_dist[dy] - s1) <= base.m_width)
            {
                dist_end -= m_di.dx_end();
                *p1 = 0;
                if(dist_end > 0)
                {   
                    *p1 = (byte)base.m_ren.cover(dist);
                    ++npix;
                }
                ++p1;
                ++dy;
            }

            dy = 1;
            dist_end = m_di.dist_end();
            while((dist = base.m_dist[dy] + s1) <= base.m_width)
            {
                dist_end += m_di.dx_end();
                *--p0 = 0;
                if(dist_end > 0)
                {   
                    *p0 = (byte)base.m_ren.cover(dist);
                    ++npix;
                }
                ++dy;
            }
            base.m_ren.blend_solid_vspan(base.m_x,
                                               base.m_y - dy + 1, 
                                               uint(p1 - p0), 
                                               p0);
            return npix && ++base.m_step < base.m_count;
        }

        //---------------------------------------------------------------------
        public bool step_ver()
        {
            int dist_end;
            int dist;
            int dx;
            int s1 = base.step_ver_base(m_di);
            byte* p0 = base.m_covers + base.max_half_width + 2;
            byte* p1 = p0;

            dist_end = m_di.dist_end();

            int npix = 0;
            *p1 = 0;
            if(dist_end > 0)
            {
                *p1 = (byte)base.m_ren.cover(s1);
                ++npix;
            }
            ++p1;

            dx = 1;
            while((dist = base.m_dist[dx] - s1) <= base.m_width)
            {
                dist_end += m_di.dy_end();
                *p1 = 0;
                if(dist_end > 0)
                {   
                    *p1 = (byte)base.m_ren.cover(dist);
                    ++npix;
                }
                ++p1;
                ++dx;
            }

            dx = 1;
            dist_end = m_di.dist_end();
            while((dist = base.m_dist[dx] + s1) <= base.m_width)
            {
                dist_end -= m_di.dy_end();
                *--p0 = 0;
                if(dist_end > 0)
                {   
                    *p0 = (byte)base.m_ren.cover(dist);
                    ++npix;
                }
                ++dx;
            }
            base.m_ren.blend_solid_hspan(base.m_x - dx + 1,
                                               base.m_y, 
                                               (uint)(p1 - p0), 
                                               p0);
            return npix && ++base.m_step < base.m_count;
        }
    };

    //====================================================line_interpolator_aa3
    public class line_interpolator_aa3 : line_interpolator_aa_base
    {
        distance_interpolator3 m_di; 
        //typedef Renderer renderer_type;
        //typedef line_interpolator_aa_base<Renderer> base_type;

        //---------------------------------------------------------------------
        public line_interpolator_aa3(IRasterizer ren, line_parameters lp, 
                              int sx, int sy, int ex, int ey) :
            base(ren, lp)
        {
            m_di = new distance_interpolator3(lp.x1, lp.y1, lp.x2, lp.y2, sx, sy, ex, ey, 
                 lp.x1 & ~line_subpixel_mask, lp.y1 & ~line_subpixel_mask);
            int dist1_start;
            int dist2_start;
            int npix = 1;
            if(lp.vertical)
            {
                do
                {
                    --base.m_li;
                    base.m_y -= lp.inc;
                    base.m_x = (base.m_lp.x1 + base.m_li.y()) >> LineAABasics.line_subpixel_shift;

                    if(lp.inc > 0) m_di.dec_y(base.m_x - base.m_old_x);
                    else           m_di.inc_y(base.m_x - base.m_old_x);

                    base.m_old_x = base.m_x;

                    dist1_start = dist2_start = m_di.dist_start(); 

                    int dx = 0;
                    if(dist1_start < 0) ++npix;
                    do
                    {
                        dist1_start += m_di.dy_start();
                        dist2_start -= m_di.dy_start();
                        if(dist1_start < 0) ++npix;
                        if(dist2_start < 0) ++npix;
                        ++dx;
                    }
                    while(base.m_dist[dx] <= base.m_width);
                    if(npix == 0) break;
                    npix = 0;
                }
                while(--base.m_step >= -base.m_max_extent);
            }
            else
            {
                do
                {
                    --base.m_li;
                    base.m_x -= lp.inc;
                    base.m_y = (base.m_lp.y1 + base.m_li.y()) >> LineAABasics.line_subpixel_shift;

                    if(lp.inc > 0) m_di.dec_x(base.m_y - base.m_old_y);
                    else           m_di.inc_x(base.m_y - base.m_old_y);

                    base.m_old_y = base.m_y;

                    dist1_start = dist2_start = m_di.dist_start(); 

                    int dy = 0;
                    if(dist1_start < 0) ++npix;
                    do
                    {
                        dist1_start -= m_di.dx_start();
                        dist2_start += m_di.dx_start();
                        if(dist1_start < 0) ++npix;
                        if(dist2_start < 0) ++npix;
                        ++dy;
                    }
                    while(base.m_dist[dy] <= base.m_width);
                    if(npix == 0) break;
                    npix = 0;
                }
                while(--base.m_step >= -base.m_max_extent);
            }
            base.m_li.adjust_forward();
            base.m_step -= base.m_max_extent;
        }


        //---------------------------------------------------------------------
        public bool step_hor()
        {
            int dist_start;
            int dist_end;
            int dist;
            int dy;
            int s1 = base.step_hor_base(m_di);
            byte* p0 = base.m_covers + base.max_half_width + 2;
            byte* p1 = p0;

            dist_start = m_di.dist_start();
            dist_end   = m_di.dist_end();

            int npix = 0;
            *p1 = 0;
            if(dist_end > 0)
            {
                if(dist_start <= 0)
                {
                    *p1 = (byte)base.m_ren.cover(s1);
                }
                ++npix;
            }
            ++p1;

            dy = 1;
            while((dist = base.m_dist[dy] - s1) <= base.m_width)
            {
                dist_start -= m_di.dx_start();
                dist_end   -= m_di.dx_end();
                *p1 = 0;
                if(dist_end > 0 && dist_start <= 0)
                {   
                    *p1 = (byte)base.m_ren.cover(dist);
                    ++npix;
                }
                ++p1;
                ++dy;
            }

            dy = 1;
            dist_start = m_di.dist_start();
            dist_end   = m_di.dist_end();
            while((dist = base.m_dist[dy] + s1) <= base.m_width)
            {
                dist_start += m_di.dx_start();
                dist_end   += m_di.dx_end();
                *--p0 = 0;
                if(dist_end > 0 && dist_start <= 0)
                {   
                    *p0 = (byte)base.m_ren.cover(dist);
                    ++npix;
                }
                ++dy;
            }
            base.m_ren.blend_solid_vspan(base.m_x,
                                               base.m_y - dy + 1, 
                                               uint(p1 - p0), 
                                               p0);
            return npix && ++base.m_step < base.m_count;
        }

        //---------------------------------------------------------------------
        public bool step_ver()
        {
            int dist_start;
            int dist_end;
            int dist;
            int dx;
            int s1 = base.step_ver_base(m_di);
            byte* p0 = base.m_covers + base.max_half_width + 2;
            byte* p1 = p0;

            dist_start = m_di.dist_start();
            dist_end   = m_di.dist_end();

            int npix = 0;
            *p1 = 0;
            if(dist_end > 0)
            {
                if(dist_start <= 0)
                {
                    *p1 = (byte)base.m_ren.cover(s1);
                }
                ++npix;
            }
            ++p1;

            dx = 1;
            while((dist = base.m_dist[dx] - s1) <= base.m_width)
            {
                dist_start += m_di.dy_start();
                dist_end   += m_di.dy_end();
                *p1 = 0;
                if(dist_end > 0 && dist_start <= 0)
                {   
                    *p1 = (byte)base.m_ren.cover(dist);
                    ++npix;
                }
                ++p1;
                ++dx;
            }

            dx = 1;
            dist_start = m_di.dist_start();
            dist_end   = m_di.dist_end();
            while((dist = base.m_dist[dx] + s1) <= base.m_width)
            {
                dist_start -= m_di.dy_start();
                dist_end   -= m_di.dy_end();
                *--p0 = 0;
                if(dist_end > 0 && dist_start <= 0)
                {   
                    *p0 = (byte)base.m_ren.cover(dist);
                    ++npix;
                }
                ++dx;
            }
            base.m_ren.blend_solid_hspan(base.m_x - dx + 1,
                                               base.m_y, 
                                               (uint)(p1 - p0), 
                                               p0);
            return npix && ++base.m_step < base.m_count;
        }
    };


    //==========================================================line_profile_aa
    //
    // See Implementation agg_line_profile_aa.cpp 
    // 
    public class line_profile_aa
    {
        const int subpixel_shift = 8;
        const int subpixel_scale = 1 << subpixel_shift;
        const int subpixel_mask  = subpixel_scale - 1;

        const int aa_shift = 8;
        const int aa_scale = 1 << aa_shift;
        const int aa_mask  = aa_scale - 1;

        ArrayPOD<byte> m_profile;
        byte[]            m_gamma = new byte[aa_scale];
        int                   m_subpixel_width;
        double                m_min_width;
        double                m_smoother_width;

        //---------------------------------------------------------------------
        
        //---------------------------------------------------------------------
        public line_profile_aa()
        {
            m_subpixel_width=(0);
            m_min_width=(1.0);
            m_smoother_width=(1.0);

            int i;
            for(i = 0; i < aa_scale; i++) m_gamma[i] = (byte)i;
        }

        //---------------------------------------------------------------------
        public line_profile_aa(double w, IGammaFunction gamma_function)
        {
            m_subpixel_width=(0);
            m_min_width=(1.0);
            m_smoother_width=(1.0);
            gamma(gamma_function);
            width(w);
        }

        //---------------------------------------------------------------------
        public void min_width(double w) { m_min_width = w; }
        public void smoother_width(double w) { m_smoother_width = w; }

        //---------------------------------------------------------------------
        public void gamma(IGammaFunction gamma_function)
        { 
            int i;
            for(i = 0; i < aa_scale; i++)
            {
                m_gamma[i] = (byte)(agg_basics.uround(gamma_function.GetGamma((double)(i) / aa_mask) * aa_mask));
            }
        }

        public void width(double w)
        {
            if(w < 0.0) w = 0.0;

            if(w < m_smoother_width) w += w;
            else                     w += m_smoother_width;

            w *= 0.5;

            w -= m_smoother_width;
            double s = m_smoother_width;
            if(w < 0.0) 
            {
                s += w;
                w = 0.0;
            }
            set(w, s);
        }

        public uint profile_size() { return m_profile.size(); }
        public int subpixel_width() { return m_subpixel_width; }

        //---------------------------------------------------------------------
        public double min_width() { return m_min_width; }
        public double smoother_width() { return m_smoother_width; }

        //---------------------------------------------------------------------
        public byte Value(int dist)
        {
            return m_profile.Array[dist + subpixel_scale*2];
        }

        private byte[] profile(double w)
        {
            m_subpixel_width = (int)agg_basics.uround(w * subpixel_scale);
            uint size = (uint)(m_subpixel_width + subpixel_scale * 6);
            if(size > m_profile.size())
            {
                m_profile.resize(size);
            }
            return m_profile.Array;
        }

        private void set(double center_width, double smoother_width)
        {
            double base_val = 1.0;
            if(center_width == 0.0)   center_width = 1.0 / subpixel_scale;
            if(smoother_width == 0.0) smoother_width = 1.0 / subpixel_scale;

            double width = center_width + smoother_width;
            if(width < m_min_width)
            {
                double k = width / m_min_width;
                base_val *= k;
                center_width /= k;
                smoother_width /= k;
            }

            byte[] ch = profile(center_width + smoother_width);
            int chIndex = 0;

            uint subpixel_center_width = (uint)(center_width * subpixel_scale);
            uint subpixel_smoother_width = (uint)(smoother_width * subpixel_scale);

            int ch_center   = subpixel_scale*2;
            int ch_smoother = (int)subpixel_center_width;

            uint i;

            uint val = m_gamma[(uint)(base_val * aa_mask)];
            chIndex = ch_center;
            for(i = 0; i < subpixel_center_width; i++)
            {
                ch[chIndex++] = (byte)val;
            }

            for(i = 0; i < subpixel_smoother_width; i++)
            {
                ch[ch_smoother++] = 
                    m_gamma[(uint)((base_val - 
                                      base_val * 
                                      ((double)(i) / subpixel_smoother_width)) * aa_mask)];
            }

            uint n_smoother = profile_size() - 
                                  subpixel_smoother_width - 
                                  subpixel_center_width - 
                                  subpixel_scale*2;

            val = m_gamma[0];
            for(i = 0; i < n_smoother; i++)
            {
                ch[ch_smoother++] = (byte)val;
            }

            chIndex = ch_center;
            for(i = 0; i < subpixel_scale*2; i++)
            {
                ch[--chIndex] = ch[ch_center++];
            }
        }
    };
     */

    /*
    //======================================================renderer_outline_aa
    public class renderer_outline_aa : IRasterizer
    {
        private IRasterizer m_ren;
        private IColorType m_color;
        line_profile_aa m_profile; 
        rect_i                 m_clip_box;
        bool                   m_clipping;

        //---------------------------------------------------------------------
        public renderer_outline_aa(IRasterizer ren, line_profile_aa prof)
        {
            m_ren=ren;
            m_profile=prof;
            m_clip_box = new rect_i(0,0,0,0);
            m_clipping=false;
        }

        public void Attach(IRasterizer ren) { m_ren = &ren; }

        //---------------------------------------------------------------------
        public void color(IColorType c) { m_color = c; }
        public IColorType color() { return m_color; }

        //---------------------------------------------------------------------
        public void profile(line_profile_aa prof) { m_profile = prof; }
        public line_profile_aa profile() { return m_profile; }

        //---------------------------------------------------------------------
        public int subpixel_width() { return m_profile.subpixel_width(); }

        //---------------------------------------------------------------------
        public void reset_clipping() { m_clipping = false; }
        public void clip_box(double x1, double y1, double x2, double y2)
        {
            m_clip_box.x1 = line_coord_sat::conv(x1);
            m_clip_box.y1 = line_coord_sat::conv(y1);
            m_clip_box.x2 = line_coord_sat::conv(x2);
            m_clip_box.y2 = line_coord_sat::conv(y2);
            m_clipping = true;
        }

        //---------------------------------------------------------------------
        public int cover(int d)
        {
            return m_profile.Value(d);
        }

        //-------------------------------------------------------------------------
        public unsafe void blend_solid_hspan(int x, int y, uint len, byte* covers)
        {
            m_ren.blend_solid_hspan(x, y, len, m_color, covers);
        }

        //-------------------------------------------------------------------------
        public unsafe void blend_solid_vspan(int x, int y, uint len, byte* covers)
        {
            m_ren.blend_solid_vspan(x, y, len, m_color, covers);
        }

        //-------------------------------------------------------------------------
        public static bool accurate_join_only() { return false; }

#if false
        //-------------------------------------------------------------------------
        //template<class Cmp>
        public void semidot_hline(Cmp cmp,
                           int xc1, int yc1, int xc2, int yc2, 
                           int x1,  int y1,  int x2)
        {
            byte covers[line_interpolator_aa_base<renderer_outline_aa>::max_half_width * 2 + 4];
            byte* p0 = covers;
            byte* p1 = covers;
            int x = x1 << LineAABasics.line_subpixel_shift;
            int y = y1 << LineAABasics.line_subpixel_shift;
            int w = subpixel_width();
            distance_interpolator0 di(xc1, yc1, xc2, yc2, x, y);
            x += LineAABasics.line_subpixel_scale/2;
            y += LineAABasics.line_subpixel_scale/2;

            int x0 = x1;
            int dx = x - xc1;
            int dy = y - yc1;
            do
            {
                int d = int(fast_sqrt(dx*dx + dy*dy));
                *p1 = 0;
                if(cmp(di.dist()) && d <= w)
                {
                    *p1 = (byte)cover(d);
                }
                ++p1;
                dx += LineAABasics.line_subpixel_scale;
                di.inc_x();
            }
            while(++x1 <= x2);
            m_ren.blend_solid_hspan(x0, y1, 
                                     uint(p1 - p0), 
                                     color(), 
                                     p0);
        }

        //-------------------------------------------------------------------------
        //template<class Cmp> 
        public void semidot(Cmp cmp, int xc1, int yc1, int xc2, int yc2)
        {
            if(m_clipping && clipping_flags(xc1, yc1, m_clip_box)) return;

            int r = ((subpixel_width() + line_subpixel_mask) >> LineAABasics.line_subpixel_shift);
            if(r < 1) r = 1;
            ellipse_bresenham_interpolator ei(r, r);
            int dx = 0;
            int dy = -r;
            int dy0 = dy;
            int dx0 = dx;
            int x = xc1 >> LineAABasics.line_subpixel_shift;
            int y = yc1 >> LineAABasics.line_subpixel_shift;

            do
            {
                dx += ei.dx();
                dy += ei.dy();

                if(dy != dy0)
                {
                    semidot_hline(cmp, xc1, yc1, xc2, yc2, x-dx0, y+dy0, x+dx0);
                    semidot_hline(cmp, xc1, yc1, xc2, yc2, x-dx0, y-dy0, x+dx0);
                }
                dx0 = dx;
                dy0 = dy;
                ++ei;
            }
            while(dy < 0);
            semidot_hline(cmp, xc1, yc1, xc2, yc2, x-dx0, y+dy0, x+dx0);
        }
#endif

        //-------------------------------------------------------------------------
        public void pie_hline(int xc, int yc, int xp1, int yp1, int xp2, int yp2, 
                       int xh1, int yh1, int xh2)
        {
            if(m_clipping && clipping_flags(xc, yc, m_clip_box)) return;
           
            byte covers[line_interpolator_aa_base<renderer_outline_aa>::max_half_width * 2 + 4];
            byte* p0 = covers;
            byte* p1 = covers;
            int x = xh1 << LineAABasics.line_subpixel_shift;
            int y = yh1 << LineAABasics.line_subpixel_shift;
            int w = subpixel_width();

            distance_interpolator00 di(xc, yc, xp1, yp1, xp2, yp2, x, y);
            x += LineAABasics.line_subpixel_scale/2;
            y += LineAABasics.line_subpixel_scale/2;

            int xh0 = xh1;
            int dx = x - xc;
            int dy = y - yc;
            do
            {
                int d = int(fast_sqrt(dx*dx + dy*dy));
                *p1 = 0;
                if(di.dist1() <= 0 && di.dist2() > 0 && d <= w)
                {
                    *p1 = (byte)cover(d);
                }
                ++p1;
                dx += LineAABasics.line_subpixel_scale;
                di.inc_x();
            }
            while(++xh1 <= xh2);
            m_ren.blend_solid_hspan(xh0, yh1, 
                                     uint(p1 - p0), 
                                     color(), 
                                     p0);
        }


        //-------------------------------------------------------------------------
        public void pie(int xc, int yc, int x1, int y1, int x2, int y2)
        {
            int r = ((subpixel_width() + line_subpixel_mask) >> LineAABasics.line_subpixel_shift);
            if(r < 1) r = 1;
            ellipse_bresenham_interpolator ei(r, r);
            int dx = 0;
            int dy = -r;
            int dy0 = dy;
            int dx0 = dx;
            int x = xc >> LineAABasics.line_subpixel_shift;
            int y = yc >> LineAABasics.line_subpixel_shift;

            do
            {
                dx += ei.dx();
                dy += ei.dy();

                if(dy != dy0)
                {
                    pie_hline(xc, yc, x1, y1, x2, y2, x-dx0, y+dy0, x+dx0);
                    pie_hline(xc, yc, x1, y1, x2, y2, x-dx0, y-dy0, x+dx0);
                }
                dx0 = dx;
                dy0 = dy;
                ++ei;
            }
            while(dy < 0);
            pie_hline(xc, yc, x1, y1, x2, y2, x-dx0, y+dy0, x+dx0);
        }

        //-------------------------------------------------------------------------
        public void line0_no_clip(line_parameters lp)
        {
            if(lp.len > line_max_length)
            {
                line_parameters lp1, lp2;
                lp.divide(lp1, lp2);
                line0_no_clip(lp1);
                line0_no_clip(lp2);
                return;
            }

            line_interpolator_aa0<renderer_outline_aa> li(*this, lp);
            if(li.count())
            {
                if(li.vertical())
                {
                    while(li.step_ver());
                }
                else
                {
                    while(li.step_hor());
                }
            }
        }

        //-------------------------------------------------------------------------
        public void line0(line_parameters lp)
        {
            if(m_clipping)
            {
                int x1 = lp.x1;
                int y1 = lp.y1;
                int x2 = lp.x2;
                int y2 = lp.y2;
                uint flags = clip_line_segment(&x1, &y1, &x2, &y2, m_clip_box);
                if((flags & 4) == 0)
                {
                    if(flags)
                    {
                        line_parameters lp2(x1, y1, x2, y2, 
                                           uround(calc_distance(x1, y1, x2, y2)));
                        line0_no_clip(lp2);
                    }
                    else
                    {
                        line0_no_clip(lp);
                    }
                }
            }
            else
            {
                line0_no_clip(lp);
            }
        }

        //-------------------------------------------------------------------------
        public void line1_no_clip(line_parameters lp, int sx, int sy)
        {
            if(lp.len > line_max_length)
            {
                line_parameters lp1, lp2;
                lp.divide(lp1, lp2);
                line1_no_clip(lp1, (lp.x1 + sx) >> 1, (lp.y1 + sy) >> 1);
                line1_no_clip(lp2, lp1.x2 + (lp1.y2 - lp1.y1), lp1.y2 - (lp1.x2 - lp1.x1));
                return;
            }

            fix_degenerate_bisectrix_start(lp, &sx, &sy);
            line_interpolator_aa1<renderer_outline_aa> li(*this, lp, sx, sy);
            if(li.vertical())
            {
                while(li.step_ver());
            }
            else
            {
                while(li.step_hor());
            }
        }


        //-------------------------------------------------------------------------
        public void line1(line_parameters lp, int sx, int sy)
        {
            if(m_clipping)
            {
                int x1 = lp.x1;
                int y1 = lp.y1;
                int x2 = lp.x2;
                int y2 = lp.y2;
                uint flags = clip_line_segment(&x1, &y1, &x2, &y2, m_clip_box);
                if((flags & 4) == 0)
                {
                    if(flags)
                    {
                        line_parameters lp2(x1, y1, x2, y2, 
                                           uround(calc_distance(x1, y1, x2, y2)));
                        if(flags & 1)
                        {
                            sx = x1 + (y2 - y1); 
                            sy = y1 - (x2 - x1);
                        }
                        else
                        {
                            while(Math.Abs(sx - lp.x1) + Math.Abs(sy - lp.y1) > lp2.len)
                            {
                                sx = (lp.x1 + sx) >> 1;
                                sy = (lp.y1 + sy) >> 1;
                            }
                        }
                        line1_no_clip(lp2, sx, sy);
                    }
                    else
                    {
                        line1_no_clip(lp, sx, sy);
                    }
                }
            }
            else
            {
                line1_no_clip(lp, sx, sy);
            }
        }

        //-------------------------------------------------------------------------
        public void line2_no_clip(line_parameters lp, int ex, int ey)
        {
            if(lp.len > line_max_length)
            {
                line_parameters lp1, lp2;
                lp.divide(lp1, lp2);
                line2_no_clip(lp1, lp1.x2 + (lp1.y2 - lp1.y1), lp1.y2 - (lp1.x2 - lp1.x1));
                line2_no_clip(lp2, (lp.x2 + ex) >> 1, (lp.y2 + ey) >> 1);
                return;
            }

            fix_degenerate_bisectrix_end(lp, &ex, &ey);
            line_interpolator_aa2<renderer_outline_aa> li(*this, lp, ex, ey);
            if(li.vertical())
            {
                while(li.step_ver());
            }
            else
            {
                while(li.step_hor());
            }
        }

        //-------------------------------------------------------------------------
        public void line2(line_parameters lp, int ex, int ey)
        {
            if(m_clipping)
            {
                int x1 = lp.x1;
                int y1 = lp.y1;
                int x2 = lp.x2;
                int y2 = lp.y2;
                uint flags = clip_line_segment(&x1, &y1, &x2, &y2, m_clip_box);
                if((flags & 4) == 0)
                {
                    if(flags)
                    {
                        line_parameters lp2(x1, y1, x2, y2, 
                                           uround(calc_distance(x1, y1, x2, y2)));
                        if(flags & 2)
                        {
                            ex = x2 + (y2 - y1); 
                            ey = y2 - (x2 - x1);
                        }
                        else
                        {
                            while(Math.Abs(ex - lp.x2) + Math.Abs(ey - lp.y2) > lp2.len)
                            {
                                ex = (lp.x2 + ex) >> 1;
                                ey = (lp.y2 + ey) >> 1;
                            }
                        }
                        line2_no_clip(lp2, ex, ey);
                    }
                    else
                    {
                        line2_no_clip(lp, ex, ey);
                    }
                }
            }
            else
            {
                line2_no_clip(lp, ex, ey);
            }
        }

        //-------------------------------------------------------------------------
        public void line3_no_clip(line_parameters lp, 
                           int sx, int sy, int ex, int ey)
        {
            if(lp.len > line_max_length)
            {
                line_parameters lp1, lp2;
                lp.divide(lp1, lp2);
                int mx = lp1.x2 + (lp1.y2 - lp1.y1);
                int my = lp1.y2 - (lp1.x2 - lp1.x1);
                line3_no_clip(lp1, (lp.x1 + sx) >> 1, (lp.y1 + sy) >> 1, mx, my);
                line3_no_clip(lp2, mx, my, (lp.x2 + ex) >> 1, (lp.y2 + ey) >> 1);
                return;
            }

            fix_degenerate_bisectrix_start(lp, &sx, &sy);
            fix_degenerate_bisectrix_end(lp, &ex, &ey);
            line_interpolator_aa3<renderer_outline_aa> li(*this, lp, sx, sy, ex, ey);
            if(li.vertical())
            {
                while(li.step_ver());
            }
            else
            {
                while(li.step_hor());
            }
        }

        //-------------------------------------------------------------------------
        public void line3(line_parameters lp, 
                   int sx, int sy, int ex, int ey)
        {
            if(m_clipping)
            {
                int x1 = lp.x1;
                int y1 = lp.y1;
                int x2 = lp.x2;
                int y2 = lp.y2;
                uint flags = clip_line_segment(&x1, &y1, &x2, &y2, m_clip_box);
                if((flags & 4) == 0)
                {
                    if(flags)
                    {
                        line_parameters lp2(x1, y1, x2, y2, 
                                           uround(calc_distance(x1, y1, x2, y2)));
                        if(flags & 1)
                        {
                            sx = x1 + (y2 - y1); 
                            sy = y1 - (x2 - x1);
                        }
                        else
                        {
                            while(Math.Abs(sx - lp.x1) + Math.Abs(sy - lp.y1) > lp2.len)
                            {
                                sx = (lp.x1 + sx) >> 1;
                                sy = (lp.y1 + sy) >> 1;
                            }
                        }
                        if(flags & 2)
                        {
                            ex = x2 + (y2 - y1); 
                            ey = y2 - (x2 - x1);
                        }
                        else
                        {
                            while(Math.Abs(ex - lp.x2) + Math.Abs(ey - lp.y2) > lp2.len)
                            {
                                ex = (lp.x2 + ex) >> 1;
                                ey = (lp.y2 + ey) >> 1;
                            }
                        }
                        line3_no_clip(lp2, sx, sy, ex, ey);
                    }
                    else
                    {
                        line3_no_clip(lp, sx, sy, ex, ey);
                    }
                }
            }
            else
            {
                line3_no_clip(lp, sx, sy, ex, ey);
            }
        }
    };
     */
}
