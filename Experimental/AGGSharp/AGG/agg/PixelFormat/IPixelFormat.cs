
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

using AGG.Buffer;
using AGG.Color;
namespace AGG.PixelFormat
{
    public interface IPixelFormat
    {
        uint Width { get; }
        uint Height { get; }
        int Stride { get; }

        IBlender Blender
        {
            get;
        }

        RGBA_Bytes Pixel(int x, int y);
        unsafe void CopyPixel(int x, int y, byte* c);
        void CopyFrom(RasterBuffer from, int xdst, int ydst, int xsrc, int ysrc, uint len);

        unsafe void MakePix(byte* p, IColorType c);
        void BlendPixel(int x, int y, RGBA_Bytes c, byte cover);

        // line stuff
        void CopyHLine(int x, int y, uint len, RGBA_Bytes c);
        void CopyVLine(int x, int y, uint len, RGBA_Bytes c);

        void BlendHLine(int x, int y, int x2, RGBA_Bytes c, byte cover);
        void BlendVLine(int x, int y1, int y2, RGBA_Bytes c, byte cover);

        // color stuff
        unsafe void CopyColorHSpan(int x, int y, uint len, RGBA_Bytes* colors);
        unsafe void CopyColorVSpan(int x, int y, uint len, RGBA_Bytes* colors);

        unsafe void BlendSolidHSpan(int x, int y, uint len, RGBA_Bytes c, byte* covers);
        unsafe void BlendSolidVSpan(int x, int y, uint len, RGBA_Bytes c, byte* covers);

        unsafe void BlendColorHSpan(int x, int y, uint len, RGBA_Bytes* colors, byte* covers, byte cover);
        unsafe void BlendColorVSpan(int x, int y, uint len, RGBA_Bytes* colors, byte* covers, byte cover);

        RasterBuffer GetRenderingBuffer();

        unsafe byte* RowPtr(int y);
        unsafe byte* PixPtr(int x, int y);

        uint PixelWidthInBytes
        {
            get;
        }
    };

    public abstract class PixelFormatProxy : IPixelFormat
    {
        protected IPixelFormat m_pixf;

        public PixelFormatProxy(IPixelFormat pixf)
        {
            m_pixf = pixf;
        }

        public virtual void Attach(IPixelFormat pixf)
        {
            m_pixf = pixf;
        }

        public virtual uint Width
        {
            get
            {
                return m_pixf.Width;
            }
        }

        public virtual uint Height
        {
            get
            {
                return m_pixf.Height;
            }
        }

        public virtual int Stride
        {
            get
            {
                return m_pixf.Stride;
            }
        }

        public IBlender Blender
        {
            get
            {
                return m_pixf.Blender;
            }
        }

        public virtual RGBA_Bytes Pixel(int x, int y)
        {
            return m_pixf.Pixel(y, x);
        }

        public unsafe virtual void CopyPixel(int x, int y, byte* c)
        {
            m_pixf.CopyPixel(x, y, c);
        }

        public virtual void CopyFrom(RasterBuffer from, int xdst, int ydst, int xsrc, int ysrc, uint len)
        {
            m_pixf.CopyFrom(from, xdst, ydst, xsrc, ysrc, len);
        }

        public unsafe virtual void MakePix(byte* p, IColorType c)
        {
            m_pixf.MakePix(p, c);
        }

        public virtual void BlendPixel(int x, int y, RGBA_Bytes c, byte cover)
        {
            m_pixf.BlendPixel(x, y, c, cover);
        }

        public virtual void CopyHLine(int x, int y, uint len, RGBA_Bytes c)
        {
            m_pixf.CopyHLine(x, y, len, c);
        }

        public virtual void CopyVLine(int x, int y, uint len, RGBA_Bytes c)
        {
            m_pixf.CopyVLine(x, y, len, c);
        }

        public virtual void BlendHLine(int x1, int y, int x2, RGBA_Bytes c, byte cover)
        {
            m_pixf.BlendHLine(x1, y, x2, c, cover);
        }

        public virtual void BlendVLine(int x, int y1, int y2, RGBA_Bytes c, byte cover)
        {
            m_pixf.BlendVLine(x, y1, y2, c, cover);
        }

        public unsafe virtual void BlendSolidHSpan(int x, int y, uint len, RGBA_Bytes c, byte* covers)
        {
            m_pixf.BlendSolidHSpan(x, y, len, c, covers);
        }

        public unsafe virtual void BlendSolidVSpan(int x, int y, uint len, RGBA_Bytes c, byte* covers)
        {
            m_pixf.BlendSolidVSpan(x, y, len, c, covers);
        }

        public unsafe virtual void CopyColorHSpan(int x, int y, uint len, RGBA_Bytes* colors)
        {
            m_pixf.CopyColorHSpan(x, y, len, colors);
        }

        public unsafe virtual void CopyColorVSpan(int x, int y, uint len, RGBA_Bytes* colors)
        {
            m_pixf.CopyColorVSpan(x, y, len, colors);
        }

        public unsafe virtual void BlendColorHSpan(int x, int y, uint len, RGBA_Bytes* colors, byte* covers, byte cover)
        {
            m_pixf.BlendColorHSpan(x, y, len, colors, covers, cover);
        }

        public unsafe virtual void BlendColorVSpan(int x, int y, uint len, RGBA_Bytes* colors, byte* covers, byte cover)
        {
            m_pixf.BlendColorVSpan(x, y, len, colors, covers, cover);
        }

        public virtual RasterBuffer GetRenderingBuffer()
        {
            return m_pixf.GetRenderingBuffer();
        }

        public unsafe byte* RowPtr(int y)
        {
            return m_pixf.RowPtr(y);
        }

        public unsafe virtual byte* PixPtr(int x, int y)
        {
            return m_pixf.PixPtr(x, y);
        }

        public virtual uint PixelWidthInBytes
        {
            get
            {
                return m_pixf.PixelWidthInBytes;
            }
        }
    };
}
