
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

using AGG.Color;
namespace AGG.PixelFormat
{
    //=======================================================pixfmt_transposer
    public sealed class FormatTransposer : PixelFormatProxy
    {
        //--------------------------------------------------------------------
        public FormatTransposer(IPixelFormat pixelFormat)
            : base(pixelFormat)
        {
        }


        //--------------------------------------------------------------------
        public override uint Width { get { return m_pixf.Height; } }
        public override uint Height { get { return m_pixf.Width; } }

        //--------------------------------------------------------------------
        public override RGBA_Bytes Pixel(int x, int y)
        {
            return m_pixf.Pixel(y, x);
        }

        //--------------------------------------------------------------------
        unsafe public override void CopyPixel(int x, int y, byte* c)
        {
            m_pixf.CopyPixel(y, x, c);
        }

        //--------------------------------------------------------------------
        public override void BlendPixel(int x, int y, RGBA_Bytes c, byte cover)
        {
            m_pixf.BlendPixel(y, x, c, cover);
        }

        //--------------------------------------------------------------------
        public override void CopyHLine(int x, int y, uint len, RGBA_Bytes c)
        {
            m_pixf.CopyVLine(y, x, len, c);
        }

        //--------------------------------------------------------------------
        public override void CopyVLine(int x, int y,
                                   uint len,
                                   RGBA_Bytes c)
        {
            m_pixf.CopyHLine(y, x, len, c);
        }

        //--------------------------------------------------------------------
        public override void BlendHLine(int x1, int y, int x2, RGBA_Bytes c, byte cover)
        {
            m_pixf.BlendVLine(y, x1, x2, c, cover);
        }

        //--------------------------------------------------------------------
        public override void BlendVLine(int x, int y1, int y2, RGBA_Bytes c, byte cover)
        {
            m_pixf.BlendHLine(y1, x, y2, c, cover);
        }

        //--------------------------------------------------------------------
        unsafe public override void BlendSolidHSpan(int x, int y,
                                          uint len,
                                          RGBA_Bytes c,
                                          byte* covers)
        {
            m_pixf.BlendSolidVSpan(y, x, len, c, covers);
        }

        //--------------------------------------------------------------------
        unsafe public override void BlendSolidVSpan(int x, int y,
                                          uint len,
                                          RGBA_Bytes c,
                                          byte* covers)
        {
            m_pixf.BlendSolidHSpan(y, x, len, c, covers);
        }

        public unsafe override void CopyColorHSpan(int x, int y, uint len, RGBA_Bytes* colors)
        {
            m_pixf.CopyColorVSpan(y, x, len, colors);
        }

        public unsafe override void CopyColorVSpan(int x, int y, uint len, RGBA_Bytes* colors)
        {
            m_pixf.CopyColorHSpan(y, x, len, colors);
        }

        //--------------------------------------------------------------------
        unsafe public override void BlendColorHSpan(int x, int y, uint len, RGBA_Bytes* colors, byte* covers, byte cover)
        {
            m_pixf.BlendColorVSpan(y, x, len, colors, covers, cover);
        }

        //--------------------------------------------------------------------
        unsafe public override void BlendColorVSpan(int x, int y, uint len, RGBA_Bytes* colors, byte* covers, byte cover)
        {
            m_pixf.BlendColorHSpan(y, x, len, colors, covers, cover);
        }
    };
}
