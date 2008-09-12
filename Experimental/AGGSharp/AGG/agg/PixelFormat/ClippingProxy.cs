
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
// class ClippingPixelFormtProxy
//
//----------------------------------------------------------------------------

using AGG.Buffer;
using AGG.Color;
namespace AGG.PixelFormat
{
    //-----------------------------------------------------------ClippingPixelFormtProxy
    public class FormatClippingProxy : PixelFormatProxy
    {
        private RectInt m_clip_box;

        public const byte CoverFull = 255;

        //--------------------------------------------------------------------
        public FormatClippingProxy(IPixelFormat ren)
            : base(ren)
        {
            m_clip_box = new RectInt(0, 0, (int)ren.Width - 1, (int)ren.Height - 1);
        }

        public override void Attach(IPixelFormat ren)
        {
            base.Attach(ren);
            m_clip_box = new RectInt(0, 0, (int)ren.Width - 1, (int)ren.Height - 1);
        }

        //--------------------------------------------------------------------
        //public IPixelFormat ren() { return m_ren; }
          
        //--------------------------------------------------------------------
        public bool SetClippingBox(int x1, int y1, int x2, int y2)
        {
            RectInt cb = new RectInt(x1, y1, x2, y2);
            cb.Normalize();
            if (cb.Clip(new RectInt(0, 0, (int)Width - 1, (int)Height - 1)))
            {
                m_clip_box = cb;
                return true;
            }
            m_clip_box.X1 = 1;
            m_clip_box.Y1 = 1;
            m_clip_box.X2 = 0;
            m_clip_box.Y2 = 0;
            return false;
        }

        //--------------------------------------------------------------------
        public void ResetClipping(bool visibility)
        {
            if(visibility)
            {
                m_clip_box.X1 = 0;
                m_clip_box.Y1 = 0;
                m_clip_box.X2 = (int)Width - 1;
                m_clip_box.Y2 = (int)Height - 1;
            }
            else
            {
                m_clip_box.X1 = 1;
                m_clip_box.Y1 = 1;
                m_clip_box.X2 = 0;
                m_clip_box.Y2 = 0;
            }
        }

        //--------------------------------------------------------------------
        public void ClipBoxNaked(int x1, int y1, int x2, int y2)
        {
            m_clip_box.X1 = x1;
            m_clip_box.Y1 = y1;
            m_clip_box.X2 = x2;
            m_clip_box.Y2 = y2;
        }

        //--------------------------------------------------------------------
        public bool Inbox(int x, int y)
        {
            return x >= m_clip_box.X1 && y >= m_clip_box.Y1 &&
                   x <= m_clip_box.X2 && y <= m_clip_box.Y2;
        }

        //--------------------------------------------------------------------
        public RectInt ClipBox() { return m_clip_box;    }
        int XMin() { return m_clip_box.X1; }
        int YMin() { return m_clip_box.Y1; }
        int XMax() { return m_clip_box.X2; }
        int YMax() { return m_clip_box.Y2; }

        //--------------------------------------------------------------------
        public RectInt BoundingClipBox() { return m_clip_box;    }
        public int BoundingXMin() { return m_clip_box.X1; }
        public int BoundingYMin() { return m_clip_box.Y1; }
        public int BoundingXMax() { return m_clip_box.X2; }
        public int BoundingYMax() { return m_clip_box.Y2; }

        //--------------------------------------------------------------------
        public void Clear(IColorType in_c)
        {
            uint y;
            RGBA_Bytes c = new RGBA_Bytes(in_c.R_Byte, in_c.G_Byte, in_c.B_Byte, in_c.A_Byte);
            if(Width != 0)
            {
                for(y = 0; y < Height; y++)
                {
                    base.CopyHLine(0, (int)y, Width, c);
                }
            }
        }
          

        //--------------------------------------------------------------------
        public override unsafe void CopyPixel(int x, int y, byte* c)
        {
            if(Inbox(x, y))
            {
                base.CopyPixel(x, y, c);
            }
        }

        //--------------------------------------------------------------------
        public override void BlendPixel(int x, int y, RGBA_Bytes c, byte cover)
        {
            if(Inbox(x, y))
            {
                base.BlendPixel(x, y, c.GetAsRGBA_Bytes(), cover);
            }
        }

        //--------------------------------------------------------------------
        public override RGBA_Bytes Pixel(int x, int y)
        {
            return Inbox(x, y) ? base.Pixel(x, y) : new RGBA_Bytes();
        }

        //--------------------------------------------------------------------
        public override void CopyHLine(int x1, int y, uint x2, RGBA_Bytes c)
        {
            if(x1 > x2) { int t = (int)x2; x2 = (uint)x1; x1 = t; }
            if(y  > YMax()) return;
            if(y  < YMin()) return;
            if(x1 > XMax()) return;
            if(x2 < XMin()) return;

            if(x1 < XMin()) x1 = XMin();
            if(x2 > XMax()) x2 = (uint)XMax();

            base.CopyHLine(x1, y, (uint)(x2 - x1 + 1), c);
        }

        //--------------------------------------------------------------------
        public override void CopyVLine(int x, int y1, uint y2, RGBA_Bytes c)
        {
            if(y1 > y2) { int t = (int)y2; y2 = (uint)y1; y1 = t; }
            if(x  > XMax()) return;
            if(x  < XMin()) return;
            if(y1 > YMax()) return;
            if(y2 < YMin()) return;

            if(y1 < YMin()) y1 = YMin();
            if(y2 > YMax()) y2 = (uint)YMax();

            base.CopyVLine(x, y1, (uint)(y2 - y1 + 1), c);
        }

        //--------------------------------------------------------------------
        public override void BlendHLine(int x1, int y, int x2, RGBA_Bytes c, byte cover)
        {
            if (x1 > x2) 
            {
                int t = (int)x2;
                x2 = x1;
                x1 = t; 
            }
            if(y  > YMax()) 
                return;
            if(y  < YMin()) 
                return;
            if(x1 > XMax()) 
                return;
            if(x2 < XMin()) 
                return;

            if(x1 < XMin()) 
                x1 = XMin();
            if(x2 > XMax()) 
                x2 = XMax();

            base.BlendHLine(x1, y, x2, c, cover);
        }

        //--------------------------------------------------------------------
        public override void BlendVLine(int x, int y1, int y2, RGBA_Bytes c, byte cover)
        {
            if(y1 > y2) { int t = y2; y2 = y1; y1 = t; }
            if(x  > XMax()) return;
            if(x  < XMin()) return;
            if(y1 > YMax()) return;
            if(y2 < YMin()) return;

            if(y1 < YMin()) y1 = YMin();
            if(y2 > YMax()) y2 = YMax();

            base.BlendVLine(x, y1, y2, c, cover);
        }

        /*
        //--------------------------------------------------------------------
        public void copy_bar(int x1, int y1, int x2, int y2, IColorType c)
        {
            rect_i rc(x1, y1, x2, y2);
            rc.normalize();
            if(rc.clip(clip_box()))
            {
                int y;
                for(y = rc.y1; y <= rc.y2; y++)
                {
                    m_ren->copy_hline(rc.x1, y, uint(rc.x2 - rc.x1 + 1), c);
                }
            }
        }

        //--------------------------------------------------------------------
        public void blend_bar(int x1, int y1, int x2, int y2, 
                       IColorType c, byte cover)
        {
            rect_i rc(x1, y1, x2, y2);
            rc.normalize();
            if(rc.clip(clip_box()))
            {
                int y;
                for(y = rc.y1; y <= rc.y2; y++)
                {
                    m_ren->blend_hline(rc.x1,
                                       y,
                                       uint(rc.x2 - rc.x1 + 1), 
                                       c, 
                                       cover);
                }
            }
        }
         */

        //--------------------------------------------------------------------
        public override unsafe void BlendSolidHSpan(int x, int y, uint in_len, RGBA_Bytes c, byte* covers)
        {
            int len = (int)in_len;
            if (y > YMax()) return;
            if(y < YMin()) return;

            if(x < XMin())
            {
                len -= XMin() - x;
                if(len <= 0) return;
                covers += XMin() - x;
                x = XMin();
            }
            if(x + len > XMax())
            {
                len = XMax() - x + 1;
                if(len <= 0) return;
            }
            base.BlendSolidHSpan(x, y, (uint)len, c, covers);
        }

        //--------------------------------------------------------------------
        public override unsafe void BlendSolidVSpan(int x, int y, uint len, RGBA_Bytes c, byte* covers)
        {
            if(x > XMax()) return;
            if(x < XMin()) return;

            if(y < YMin())
            {
                len -= (uint)(YMin() - y);
                if(len <= 0) return;
                covers += YMin() - y;
                y = YMin();
            }
            if(y + len > YMax())
            {
                len = (uint)(YMax() - y + 1);
                if(len <= 0) return;
            }
            base.BlendSolidVSpan(x, y, len, c, covers);
        }


        //--------------------------------------------------------------------
        public override unsafe void CopyColorHSpan(int x, int y, uint len, RGBA_Bytes* colors)
        {
            if(y > YMax()) return;
            if(y < YMin()) return;

            if(x < XMin())
            {
                int d = XMin() - x;
                len -= (uint)d;
                if(len <= 0) return;
                colors += d;
                x = XMin();
            }
            if(x + len > XMax())
            {
                len = (uint)(XMax() - x + 1);
                if(len <= 0) return;
            }
            base.CopyColorHSpan(x, y, len, colors);
        }


        //--------------------------------------------------------------------
        public override unsafe void CopyColorVSpan(int x, int y, uint len, RGBA_Bytes* colors)
        {
            if(x > XMax()) return;
            if(x < XMin()) return;

            if(y < YMin())
            {
                int d = YMin() - y;
                len -= (uint)d;
                if(len <= 0) return;
                colors += d;
                y = YMin();
            }
            if(y + len > YMax())
            {
                len = (uint)(YMax() - y + 1);
                if(len <= 0) return;
            }
            base.CopyColorVSpan(x, y, len, colors);
        }

        //--------------------------------------------------------------------
        public unsafe void BlendColorHSpan(int x, int y, uint len, 
                               RGBA_Bytes* colors, byte* covers)
        {
            throw new System.NotImplementedException();
            //blend_color_hspan(x, y, len, colors, covers, cover_full);
        }

        public override unsafe void BlendColorHSpan(int x, int y, uint in_len, RGBA_Bytes* colors, byte* covers, byte cover)
        {
            int len = (int)in_len;
            if(y > YMax()) 
                return;
            if(y < YMin()) 
                return;

            if(x < XMin())
            {
                int d = XMin() - x;
                len -= d;
                if(len <= 0) return;
                if(covers != null) covers += d;
                colors += d;
                x = XMin();
            }
            if(x + len - 1 > XMax())
            {
                len = XMax() - x + 1;
                if(len <= 0) return;
            }
            
            base.BlendColorHSpan(x, y, (uint)len, colors, covers, cover);
        }

        public void CopyFrom(RasterBuffer src)
        {
            CopyFrom(src, new RectInt(0, 0, (int)src.Width, (int)src.Height), 0, 0);
        }

        public override void CopyFrom(RasterBuffer from, int xdst, int ydst, int xsrc, int ysrc, uint len)
        {
            throw new System.NotImplementedException();
        }

        public override unsafe void MakePix(byte* p, IColorType c)
        {
            throw new System.NotImplementedException();
        }

        public void CopyFrom(RasterBuffer src,
                       RectInt rect_src_ptr,
                       int dx,
                       int dy)
        {
            RectInt rsrc = new RectInt(rect_src_ptr.X1, rect_src_ptr.Y1, rect_src_ptr.X2 + 1, rect_src_ptr.Y2 + 1);

            // Version with xdst, ydst (absolute positioning)
            //rect_i rdst(xdst, ydst, xdst + rsrc.x2 - rsrc.x1, ydst + rsrc.y2 - rsrc.y1);

            // Version with dx, dy (relative positioning)
            RectInt rdst = new RectInt(rsrc.X1 + dx, rsrc.Y1 + dy, rsrc.X2 + dx, rsrc.Y2 + dy);

            RectInt rc = ClipRectArea(ref rdst, ref rsrc, (int)src.Width, (int)src.Height);

            if(rc.X2 > 0)
            {
                int incy = 1;
                if(rdst.Y1 > rsrc.Y1)
                {
                    rsrc.Y1 += rc.Y2 - 1;
                    rdst.Y1 += rc.Y2 - 1;
                    incy = -1;
                }
                while(rc.Y2 > 0)
                {
                    base.CopyFrom(src, 
                                     rdst.X1, rdst.Y1,
                                     rsrc.X1, rsrc.Y1,
                                     (uint)rc.X2);
                    rdst.Y1 += incy;
                    rsrc.Y1 += incy;
                    --rc.Y2;
                }
            }
        }

        public RectInt ClipRectArea(ref RectInt dst, ref RectInt src, int wsrc, int hsrc)
        {
            RectInt rc = new RectInt(0,0,0,0);
            RectInt cb = ClipBox();
            ++cb.X2;
            ++cb.Y2;

            if(src.X1 < 0)
            {
                dst.X1 -= src.X1;
                src.X1 = 0;
            }
            if(src.Y1 < 0)
            {
                dst.Y1 -= src.Y1;
                src.Y1 = 0;
            }

            if(src.X2 > wsrc) src.X2 = wsrc;
            if(src.Y2 > hsrc) src.Y2 = hsrc;

            if(dst.X1 < cb.X1)
            {
                src.X1 += cb.X1 - dst.X1;
                dst.X1 = cb.X1;
            }
            if(dst.Y1 < cb.Y1)
            {
                src.Y1 += cb.Y1 - dst.Y1;
                dst.Y1 = cb.Y1;
            }

            if(dst.X2 > cb.X2) dst.X2 = cb.X2;
            if(dst.Y2 > cb.Y2) dst.Y2 = cb.Y2;

            rc.X2 = dst.X2 - dst.X1;
            rc.Y2 = dst.Y2 - dst.Y1;

            if(rc.X2 > src.X2 - src.X1) rc.X2 = src.X2 - src.X1;
            if(rc.Y2 > src.Y2 - src.Y1) rc.Y2 = src.Y2 - src.Y1;
            return rc;
        }

        //--------------------------------------------------------------------
        public override unsafe void BlendColorVSpan(int x, int y, uint len, RGBA_Bytes* colors, byte* covers, byte cover)
        {
            if(x > XMax()) return;
            if(x < XMin()) return;

            if(y < YMin())
            {
                int d = YMin() - y;
                len -= (uint)d;
                if(len <= 0) return;
                if(covers != null) covers += d;
                colors += d;
                y = YMin();
            }
            if(y + len > YMax())
            {
                len = (uint)(YMax() - y + 1);
                if(len <= 0) return;
            }
            base.BlendColorVSpan(x, y, len, colors, covers, cover);
        }

        /*
        //--------------------------------------------------------------------
        //template<class SrcPixelFormatRenderer>
        public void blend_from(rendering_buffer src)
        {
            blend_from(src, 0, 0, 0, agg::cover_full)

        }

        public void blend_from(rendering_buffer src, 
                        ref rect_i rect_src_ptr, 
                        int dx, 
                        int dy,
                        byte cover)
        {
            rect_i rsrc(0, 0, src.width(), src.height());
            if(rect_src_ptr)
            {
                rsrc.x1 = rect_src_ptr->x1; 
                rsrc.y1 = rect_src_ptr->y1;
                rsrc.x2 = rect_src_ptr->x2 + 1;
                rsrc.y2 = rect_src_ptr->y2 + 1;
            }

            // Version with xdst, ydst (absolute positioning)
            //rect_i rdst(xdst, ydst, xdst + rsrc.x2 - rsrc.x1, ydst + rsrc.y2 - rsrc.y1);

            // Version with dx, dy (relative positioning)
            rect_i rdst(rsrc.x1 + dx, rsrc.y1 + dy, rsrc.x2 + dx, rsrc.y2 + dy);
            rect_i rc = clip_rect_area(rdst, rsrc, src.width(), src.height());

            if(rc.x2 > 0)
            {
                int incy = 1;
                if(rdst.y1 > rsrc.y1)
                {
                    rsrc.y1 += rc.y2 - 1;
                    rdst.y1 += rc.y2 - 1;
                    incy = -1;
                }
                while(rc.y2 > 0)
                {
                    typename SrcPixelFormatRenderer::row_data rw = src.row(rsrc.y1);
                    if(rw.ptr)
                    {
                        int x1src = rsrc.x1;
                        int x1dst = rdst.x1;
                        int len   = rc.x2;
                        if(rw.x1 > x1src)
                        {
                            x1dst += rw.x1 - x1src;
                            len   -= rw.x1 - x1src;
                            x1src  = rw.x1;
                        }
                        if(len > 0)
                        {
                            if(x1src + len-1 > rw.x2)
                            {
                                len -= x1src + len - rw.x2 - 1;
                            }
                            if(len > 0)
                            {
                                m_ren->blend_from(src,
                                                  x1dst, rdst.y1,
                                                  x1src, rsrc.y1,
                                                  len,
                                                  cover);
                            }
                        }
                    }
                    rdst.y1 += incy;
                    rsrc.y1 += incy;
                    --rc.y2;
                }
            }
        }

        //--------------------------------------------------------------------
        //template<class SrcPixelFormatRenderer>
        public void blend_from_color(rendering_buffer src, 
                              IColorType color)
        {
            blend_from_color(src, color, h0, 0, 0, agg::cover_full)
        }
        public void blend_from_color(rendering_buffer src, 
                              IColorType color,
                              ref rect_i rect_src_ptr, 
                              int dx, 
                              int dy,
                              byte cover)
        {
            rect_i rsrc(0, 0, src.width(), src.height());
            if(rect_src_ptr)
            {
                rsrc.x1 = rect_src_ptr->x1; 
                rsrc.y1 = rect_src_ptr->y1;
                rsrc.x2 = rect_src_ptr->x2 + 1;
                rsrc.y2 = rect_src_ptr->y2 + 1;
            }

            // Version with xdst, ydst (absolute positioning)
            //rect_i rdst(xdst, ydst, xdst + rsrc.x2 - rsrc.x1, ydst + rsrc.y2 - rsrc.y1);

            // Version with dx, dy (relative positioning)
            rect_i rdst(rsrc.x1 + dx, rsrc.y1 + dy, rsrc.x2 + dx, rsrc.y2 + dy);
            rect_i rc = clip_rect_area(rdst, rsrc, src.width(), src.height());

            if(rc.x2 > 0)
            {
                int incy = 1;
                if(rdst.y1 > rsrc.y1)
                {
                    rsrc.y1 += rc.y2 - 1;
                    rdst.y1 += rc.y2 - 1;
                    incy = -1;
                }
                while(rc.y2 > 0)
                {
                    typename SrcPixelFormatRenderer::row_data rw = src.row(rsrc.y1);
                    if(rw.ptr)
                    {
                        int x1src = rsrc.x1;
                        int x1dst = rdst.x1;
                        int len   = rc.x2;
                        if(rw.x1 > x1src)
                        {
                            x1dst += rw.x1 - x1src;
                            len   -= rw.x1 - x1src;
                            x1src  = rw.x1;
                        }
                        if(len > 0)
                        {
                            if(x1src + len-1 > rw.x2)
                            {
                                len -= x1src + len - rw.x2 - 1;
                            }
                            if(len > 0)
                            {
                                m_ren->blend_from_color(src,
                                                        color,
                                                        x1dst, rdst.y1,
                                                        x1src, rsrc.y1,
                                                        len,
                                                        cover);
                            }
                        }
                    }
                    rdst.y1 += incy;
                    rsrc.y1 += incy;
                    --rc.y2;
                }
            }
        }

    /*
        //--------------------------------------------------------------------
        //template<class SrcPixelFormatRenderer>
        public void blend_from_lut(rendering_buffer src, IColorType color_lut)
        {
            blend_from_lut(rendering_buffer src, IColorType color_lut, 0, 0, 0, agg::cover_full);
        }
        public void blend_from_lut(rendering_buffer src, 
                            IColorType color_lut,
                            ref rect_i rect_src_ptr, 
                            int dx, 
                            int dy,
                            byte cover)
        {
            rect_i rsrc(0, 0, src.width(), src.height());
            if(rect_src_ptr)
            {
                rsrc.x1 = rect_src_ptr->x1; 
                rsrc.y1 = rect_src_ptr->y1;
                rsrc.x2 = rect_src_ptr->x2 + 1;
                rsrc.y2 = rect_src_ptr->y2 + 1;
            }

            // Version with xdst, ydst (absolute positioning)
            //rect_i rdst(xdst, ydst, xdst + rsrc.x2 - rsrc.x1, ydst + rsrc.y2 - rsrc.y1);

            // Version with dx, dy (relative positioning)
            rect_i rdst(rsrc.x1 + dx, rsrc.y1 + dy, rsrc.x2 + dx, rsrc.y2 + dy);
            rect_i rc = clip_rect_area(rdst, rsrc, src.width(), src.height());

            if(rc.x2 > 0)
            {
                int incy = 1;
                if(rdst.y1 > rsrc.y1)
                {
                    rsrc.y1 += rc.y2 - 1;
                    rdst.y1 += rc.y2 - 1;
                    incy = -1;
                }
                while(rc.y2 > 0)
                {
                    typename SrcPixelFormatRenderer::row_data rw = src.row(rsrc.y1);
                    if(rw.ptr)
                    {
                        int x1src = rsrc.x1;
                        int x1dst = rdst.x1;
                        int len   = rc.x2;
                        if(rw.x1 > x1src)
                        {
                            x1dst += rw.x1 - x1src;
                            len   -= rw.x1 - x1src;
                            x1src  = rw.x1;
                        }
                        if(len > 0)
                        {
                            if(x1src + len-1 > rw.x2)
                            {
                                len -= x1src + len - rw.x2 - 1;
                            }
                            if(len > 0)
                            {
                                m_ren->blend_from_lut(src,
                                                      color_lut,
                                                      x1dst, rdst.y1,
                                                      x1src, rsrc.y1,
                                                      len,
                                                      cover);
                            }
                        }
                    }
                    rdst.y1 += incy;
                    rsrc.y1 += incy;
                    --rc.y2;
                }
            }
        }
     */
    }
}
