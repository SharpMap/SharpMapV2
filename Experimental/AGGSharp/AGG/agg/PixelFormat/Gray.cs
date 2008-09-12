
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
// Adaptation for high precision colors has been sponsored by 
// Liberty Technology Systems, Inc., visit http://lib-sys.com
//
// Liberty Technology Systems, Inc. is the provider of
// PostScript and PDF technology for software developers.
// 
//----------------------------------------------------------------------------

using AGG.Buffer;
using AGG.Color;
namespace AGG.PixelFormat
{
    public interface IBlenderGray : IBlender
    {
        unsafe void BlendPix(byte* p, uint cv, uint alpha);
        unsafe void BlendPix(byte* p, uint cv, uint alpha, uint cover);
    }

    //============================================================blender_gray
    //template<class ColorT> 
    public struct BlenderGray : IBlenderGray
    {
        public uint NumPixelBits { get { return 8; } }

        public int OrderR { get { return 0; } }
        public int OrderG { get { return 0; } }
        public int OrderB { get { return 0; } }
        public int OrderA { get { return 0; } }

        const int BaseShift = 8;

        public unsafe void BlendPix(byte* p, uint cr, uint cg, uint cb, uint alpha)
        {
            BlendPix(p, cr, alpha);
        }
        public unsafe void BlendPix(byte* p, uint cr, uint cg, uint cb, uint alpha, uint cover)
        {
            BlendPix(p, cr, alpha);
        }

        public unsafe void BlendPix(byte* p, uint cv, uint alpha)
        {
            unchecked
            {
                *p = (byte)((((cv - (uint)(*p)) * alpha) + ((int)(*p) << BaseShift)) >> BaseShift);
            }
        }

        public unsafe void BlendPix(byte* p, uint cv, uint alpha, uint cover)
        {
            BlendPix(p, cv, alpha);
        }
    };

    /*

    //======================================================blender_gray_pre
    //template<class ColorT> 
    struct blender_gray_pre
    {
        typedef ColorT color_type;
        typedef typename color_type::value_type value_type;
        typedef typename color_type::calc_type calc_type;
        enum base_scale_e { base_shift = color_type::base_shift };

        static void blend_pix(value_type* p, uint cv,
                                         uint alpha, uint cover)
        {
            alpha = color_type::base_mask - alpha;
            cover = (cover + 1) << (base_shift - 8);
            *p = (value_type)((*p * alpha + cv * cover) >> base_shift);
        }

        static void blend_pix(value_type* p, uint cv,
                                         uint alpha)
        {
            *p = (value_type)(((*p * (color_type::base_mask - alpha)) >> base_shift) + cv);
        }
    };
    


    //=====================================================apply_gamma_dir_gray
    //template<class ColorT, class GammaLut> 
    class apply_gamma_dir_gray
    {
    public:
        typedef typename ColorT::value_type value_type;

        apply_gamma_dir_gray(GammaLut& gamma) : m_gamma(gamma) {}

        void operator () (byte* p)
        {
            *p = m_gamma.dir(*p);
        }

    private:
        GammaLut& m_gamma;
    };



    //=====================================================apply_gamma_inv_gray
    //template<class ColorT, class GammaLut> 
    class apply_gamma_inv_gray
    {
    public:
        typedef typename ColorT::value_type value_type;

        apply_gamma_inv_gray(GammaLut& gamma) : m_gamma(gamma) {}

        void operator () (byte* p)
        {
            *p = m_gamma.inv(*p);
        }

    private:
        GammaLut& m_gamma;
    };

     */


    //=================================================pixfmt_alpha_blend_gray
    //template<class Blender, class RenBuf, uint Step=1, uint Offset=0>
    public sealed class FormatGray : IPixelFormat
    {
        RasterBuffer m_rbuf;
        uint m_Step;
        uint m_Offset;

        IBlenderGray m_Blender;

        const int BaseShift = 8;
        const uint BaseScale = (uint)(1 << BaseShift);
        const uint BaseMask = BaseScale - 1;

        //--------------------------------------------------------------------
        //--------------------------------------------------------------------
        public FormatGray(RasterBuffer rb, IBlenderGray blender, uint step, uint offset)
        {
            m_rbuf = rb;
            Blender = blender;
            m_Step = step;
            m_Offset = offset;
        }

        public IBlender Blender
        {
            get
            {
                return m_Blender;
            }

            set
            {
                if (value.NumPixelBits != 8)
                {
                    throw new System.NotSupportedException("pixfmt_alpha_blend_gray requires your Blender to be 8 bit. Change the Blender or the Pixel Format");
                }
                m_Blender = (IBlenderGray)value;
            }
        }

        public RasterBuffer GetRenderingBuffer()
        {
            return m_rbuf;
        }

        public uint PixelWidthInBytes
        {
            get
            {
                return 1;
            }
        }

        public void Attach(RasterBuffer rb) { m_rbuf = rb; }
        //--------------------------------------------------------------------

        /*
        //template<class PixFmt>
        public bool Attach(PixFmt& pixf, int x1, int y1, int x2, int y2)
        {
            rect_i r(x1, y1, x2, y2);
            if(r.clip(rect_i(0, 0, pixf.width()-1, pixf.height()-1)))
            {
                int stride = pixf.stride();
                m_rbuf.Attach(pixf.pix_ptr(r.x1, stride < 0 ? r.y2 : r.y1), 
                               (r.x2 - r.x1) + 1,
                               (r.y2 - r.y1) + 1,
                               stride);
                return true;
            }
            return false;
        }
         */

        //--------------------------------------------------------------------
        public uint Width { get { return m_rbuf.Width; } }
        public uint Height { get { return m_rbuf.Height; } }
        public int Stride { get { return m_rbuf.StrideInBytes; } }

        //--------------------------------------------------------------------
        unsafe public byte* RowPtr(int y) { return m_rbuf.GetPixelPointer(y); }
        //public row_data     row(int y)     { return m_rbuf.row(y); }

        unsafe public byte* PixPtr(int x, int y)
        {
            return m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;
        }

        //--------------------------------------------------------------------
        unsafe public void MakePix(byte* p, IColorType c)
        {
            p[0] = (byte)c.A_Byte;
        }

        public RGBA_Bytes Pixel(int x, int y)
        {
            unsafe
            {
                byte* p = m_rbuf.GetPixelPointer(y);
                if (p != null)
                {
                    p += x * m_Step + m_Offset;
                    return new RGBA_Bytes(*p, *p, *p, 255);
                }
                return new RGBA_Bytes();
            }
        }
        /*
       //--------------------------------------------------------------------
       public color_type pixel(int x, int y)
       {
           byte* p = (byte*)m_rbuf.row_ptr(y) + x * Step + Offset;
           return color_type(*p);
       }
         */

        //--------------------------------------------------------------------
        unsafe public void CopyPixel(int x, int y, byte* c)
        {
            *((byte*)m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset) = c[0];
        }

        //--------------------------------------------------------------------
        public void BlendPixel(int x, int y, RGBA_Bytes c, byte cover)
        {
            throw new System.NotImplementedException();
            /*
            unsafe
            {
                copy_or_blend_pix(m_rbuf.row_ptr(x, y, 1) + x * Step + Offset,
                                  c,
                                  cover);
            }
             */
        }

        //--------------------------------------------------------------------
        public void CopyHLine(int x, int y, uint len, RGBA_Bytes c)
        {
            unsafe
            {
                byte* p = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;

                do
                {
                    *p = c.R;
                    p += m_Step;
                }
                while (--len != 0);
            }
        }

        //--------------------------------------------------------------------
        public void CopyVLine(int x, int y, uint len, RGBA_Bytes c)
        {
            int ScanWidth = m_rbuf.StrideInBytes;
            unsafe
            {
                byte* p = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;

                do
                {
                    *p = c.R;
                    p = &p[ScanWidth];
                }
                while (--len != 0);
            }
        }

        //--------------------------------------------------------------------
        public void BlendHLine(int x1, int y, int x2, RGBA_Bytes c, byte cover)
        {
            unsafe
            {
                if (c.A != 0)
                {
                    int len = x2 - x1 + 1;
                    byte* p = m_rbuf.GetPixelPointer(y) + x1 * m_Step + m_Offset;

                    uint alpha = (uint)((c.A) * (cover + 1)) >> 8;
                    if (alpha == BaseMask)
                    {
                        do
                        {
                            *p = c.R;
                            p += m_Step;
                        }
                        while (--len != 0);
                    }
                    else
                    {
                        do
                        {
                            m_Blender.BlendPix(p, c.R, alpha, cover);
                            p += m_Step;
                        }
                        while (--len != 0);
                    }
                }
            }
        }


        //--------------------------------------------------------------------
        public void BlendVLine(int x, int y1, int y2, RGBA_Bytes c, byte cover)
        {
            unsafe
            {
                if (c.A != 0)
                {
                    int len = y2 - y1 + 1;
                    int ScanWidth = m_rbuf.StrideInBytes;
                    byte* p = m_rbuf.GetPixelPointer(y1) + x * m_Step + m_Offset;

                    uint alpha = (uint)((c.A) * (cover + 1)) >> 8;
                    if (alpha == BaseMask)
                    {
                        do
                        {
                            *p = c.R;
                            p = &p[ScanWidth];
                        }
                        while (--len != 0);
                    }
                    else
                    {
                        do
                        {
                            m_Blender.BlendPix(p, c.R, alpha, cover);
                            p = &p[ScanWidth];
                        }
                        while (--len != 0);
                    }
                }
            }
        }

        //--------------------------------------------------------------------
        unsafe public void BlendSolidHSpan(int x, int y, uint len, RGBA_Bytes c, byte* covers)
        {
            if (c.A != 0)
            {
                byte* p = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;

                do
                {
                    uint alpha = (uint)((c.A) * (uint)((*covers) + 1)) >> 8;
                    if (alpha == BaseMask)
                    {
                        *p = c.R;
                    }
                    else
                    {
                        m_Blender.BlendPix(p, c.R, alpha, *covers);
                    }
                    p += m_Step;
                    ++covers;
                }
                while (--len != 0);
            }
        }

        public unsafe void BlendSolidVSpan(int x, int y, uint len, RGBA_Bytes c, byte* covers)
        {
            if (c.A != 0)
            {
                int ScanWidth = m_rbuf.StrideInBytes;
                byte* p = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;

                do
                {
                    uint alpha = (uint)((c.A) * (uint)((*covers) + 1)) >> 8;
                    if (alpha == BaseMask)
                    {
                        *p = c.R;
                    }
                    else
                    {
                        m_Blender.BlendPix(p, c.R, alpha, *covers);
                    }
                    p = &p[ScanWidth];
                    ++covers;
                }
                while (--len != 0);
            }
        }

        unsafe public void CopyColorHSpan(int x, int y, uint len, RGBA_Bytes* colors)
        {
            byte* p = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;

            do
            {
                *p = colors[0].R;
                p += m_Step;
                ++colors;
            }
            while (--len != 0);
        }

        //--------------------------------------------------------------------
        unsafe public void CopyColorVSpan(int x, int y, uint len, RGBA_Bytes* colors)
        {
            byte* p = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;
            int ScanWidth = m_rbuf.StrideInBytes;
            do
            {
                *p = colors[0].R;
                p = &p[ScanWidth];
                ++colors;
            }
            while (--len != 0);
        }

        //--------------------------------------------------------------------
        unsafe public void BlendColorHSpan(int x, int y, uint len, RGBA_Bytes* colors, byte* covers, byte cover)
        {
            byte* p = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;

            if (covers != null)
            {
                do
                {
                    CopyOrBlendPix(p, *colors++, *covers++);
                    p += m_Step;
                }
                while (--len != 0);
            }
            else
            {
                if (cover == 255)
                {
                    do
                    {
                        if (colors[0].A == (byte)BaseMask)
                        {
                            *p = colors[0].A;
                        }
                        else
                        {
                            CopyOrBlendPix(p, *colors);
                        }
                        p += m_Step;
                        ++colors;
                    }
                    while (--len != 0);
                }
                else
                {
                    do
                    {
                        CopyOrBlendPix(p, *colors++, cover);
                        p += m_Step;
                    }
                    while (--len != 0);
                }
            }
        }

        //--------------------------------------------------------------------
        private unsafe void CopyOrBlendPix(byte* p, RGBA_Bytes c, uint cover)
        {
            if (c.A != 0)
            {
                uint alpha = (uint)((c.A) * (cover + 1)) >> 8;
                if (alpha == BaseMask)
                {
                    *p = c.A;
                }
                else
                {
                    m_Blender.BlendPix(p, c.A, alpha, cover);
                }
            }
        }


        private unsafe void CopyOrBlendPix(byte* p, RGBA_Bytes c)
        {
            if (c.A != 0)
            {
                if (c.A == BaseMask)
                {
                    *p = c.A;
                }
                else
                {
                    m_Blender.BlendPix(p, c.A, c.A_Byte);
                }
            }
        }

        public unsafe void BlendColorVSpan(int x, int y, uint len, RGBA_Bytes* colors, byte* covers, byte cover)
        {
            int ScanWidth = m_rbuf.StrideInBytes;
            byte* p = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;

            if (covers != null)
            {
                do
                {
                    CopyOrBlendPix(p, *colors++, *covers++);
                    p = &p[ScanWidth];
                }
                while (--len != 0);
            }
            else
            {
                if (cover == 255)
                {
                    do
                    {
                        if (colors[0].A == (byte)BaseMask)
                        {
                            *p = colors[0].A;
                        }
                        else
                        {
                            CopyOrBlendPix(p, *colors);
                        }
                        p = &p[ScanWidth];
                        ++colors;
                    }
                    while (--len != 0);
                }
                else
                {
                    do
                    {
                        CopyOrBlendPix(p, *colors++, cover);
                        p = &p[ScanWidth];
                    }
                    while (--len != 0);
                }
            }
        }

        public void CopyFrom(RasterBuffer sourceBuffer, int xdst, int ydst, int xsrc, int ysrc, uint len)
        {
            unsafe
            {
                byte* pSource = sourceBuffer.GetPixelPointer(ysrc);
                if (pSource != null)
                {
                    int BytesPerScanLine = Stride;
                    Basics.MemMove(m_rbuf.GetPixelPointer(ydst) + xdst * BytesPerScanLine,
                            pSource + xsrc * BytesPerScanLine,
                            (int)len * BytesPerScanLine);
                }
            }
        }
        /*

                                //--------------------------------------------------------------------
                                //template<class Function> 
                                public void for_each_pixel(Function f)
                                {
                                    uint y;
                                    for(y = 0; y < height(); ++y)
                                    {
                                        row_data r = m_rbuf.row(y);
                                        if(r.ptr)
                                        {
                                            uint len = r.x2 - r.x1 + 1;

                                            byte* p = (byte*)
                                                m_rbuf.row_ptr(r.x1, y, len) + r.x1 * Step + Offset;

                                            do
                                            {
                                                f(p);
                                                p += Step;
                                            }
                                            while(--len);
                                        }
                                    }
                                }

                                //--------------------------------------------------------------------
                                //template<class GammaLut> 
                                public void apply_gamma_dir(GammaLut& g)
                                {
                                    for_each_pixel(apply_gamma_dir_gray<color_type, GammaLut>(g));
                                }

                                //--------------------------------------------------------------------
                                //template<class GammaLut> 
                                public void apply_gamma_inv(GammaLut& g)
                                {
                                    for_each_pixel(apply_gamma_inv_gray<color_type, GammaLut>(g));
                                }

                                //--------------------------------------------------------------------
                                //template<class RenBuf2>

                                //--------------------------------------------------------------------
                                //template<class SrcPixelFormatRenderer>
                                public void blend_from_color(SrcPixelFormatRenderer& from, 
                                                      color_type& color,
                                                      int xdst, int ydst,
                                                      int xsrc, int ysrc,
                                                      uint len,
                                                      byte cover)
                                {
                                    typedef typename SrcPixelFormatRenderer::value_type src_value_type;
                                    src_value_type* psrc = (src_value_type*)from.row_ptr(ysrc);
                                    if(psrc)
                                    {
                                        byte* pdst = 
                                            (byte*)m_rbuf.row_ptr(xdst, ydst, len) + xdst;
                                        do 
                                        {
                                            copy_or_blend_pix(pdst, color, (*psrc * cover + base_mask) >> base_shift);
                                            ++psrc;
                                            ++pdst;
                                        }
                                        while(--len);
                                    }
                                }

                                //--------------------------------------------------------------------
                                //template<class SrcPixelFormatRenderer>
                                public void blend_from_lut(SrcPixelFormatRenderer& from, 
                                                    color_type* color_lut,
                                                    int xdst, int ydst,
                                                    int xsrc, int ysrc,
                                                    uint len,
                                                    byte cover)
                                {
                                    typedef typename SrcPixelFormatRenderer::value_type src_value_type;
                                    src_value_type* psrc = (src_value_type*)from.row_ptr(ysrc);
                                    if(psrc)
                                    {
                                        byte* pdst = 
                                            (byte*)m_rbuf.row_ptr(xdst, ydst, len) + xdst;
                                        do 
                                        {
                                            copy_or_blend_pix(pdst, color_lut[*psrc], cover);
                                            ++psrc;
                                            ++pdst;
                                        }
                                        while(--len);
                                    }
                                }

                                 */
    };
}
