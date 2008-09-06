
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
namespace AGG.PatternFilter
{
    public interface IPatternFilter
    {
        int Dilation();
        unsafe void PixelHighRes(RasterBuffer buf, RGBA_Bytes* p, int x, int y);
    }

    //=======================================================pattern_filter_nn
    //template<class ColorT> 
    /*
    struct pattern_filter_nn
    {
        typedef ColorT RGBA_Bytes;
        static uint dilation() { return 0; }

        static void pixel_low_res(RGBA_Bytes** buf, 
                                             RGBA_Bytes* p, int x, int y)
        {
            *p = buf[y][x];
        }

        static void pixel_high_res(RGBA_Bytes** buf, 
                                              RGBA_Bytes* p, int x, int y)
        {
            *p = buf[y >> line_subpixel_shift]
                    [x >> line_subpixel_shift];
        }
    };
     */

    //===========================================pattern_filter_bilinear_rgba
    public struct PatternFilterBilinearRgbaBytes : IPatternFilter
    {
        public int Dilation() { return 1; }

        public unsafe void PixelLowRes(RGBA_Bytes** buf, RGBA_Bytes* p, int x, int y)
        {
            *p = buf[y][x];
        }

        public unsafe void PixelHighRes(RasterBuffer buf, RGBA_Bytes* p, int x, int y)
        {
            int r, g, b, a;
            r = g = b = a = LineAABasics.LineSubPixelScale * LineAABasics.LineSubPixelScale / 2;

            int weight;
            int x_lr = x >> LineAABasics.LineSubPixelShift;
            int y_lr = y >> LineAABasics.LineSubPixelShift;

            x &= LineAABasics.LineSubPixelMask;
            y &= LineAABasics.LineSubPixelMask;
            RGBA_Bytes* ptr = (RGBA_Bytes*)buf.GetPixelPointer(x_lr, y_lr);

            weight = (LineAABasics.LineSubPixelScale - x) *
                     (LineAABasics.LineSubPixelScale - y);
            r += weight * ptr->R;
            g += weight * ptr->G;
            b += weight * ptr->B;
            a += weight * ptr->A;

            ++ptr;

            weight = x * (LineAABasics.LineSubPixelScale - y);
            r += weight * ptr->R;
            g += weight * ptr->G;
            b += weight * ptr->B;
            a += weight * ptr->A;

            ptr = (RGBA_Bytes*)buf.GetPixelPointer(x_lr, y_lr + 1);

            weight = (LineAABasics.LineSubPixelScale - x) * y;
            r += weight * ptr->R;
            g += weight * ptr->G;
            b += weight * ptr->B;
            a += weight * ptr->A;

            ++ptr;

            weight = x * y;
            r += weight * ptr->R;
            g += weight * ptr->G;
            b += weight * ptr->B;
            a += weight * ptr->A;
            
            //p->R = (byte)(r >> LineAABasics.LineSubPixelShift * 2);
            //p->G = (byte)(g >> LineAABasics.LineSubPixelShift * 2);
            //p->B = (byte)(b >> LineAABasics.LineSubPixelShift * 2);
            //p->A = (byte)(a >> LineAABasics.LineSubPixelShift * 2);

            (*p) = new RGBA_Bytes(
                (byte)(r >> LineAABasics.LineSubPixelShift * 2),
                (byte)(g >> LineAABasics.LineSubPixelShift * 2),
                (byte)(b >> LineAABasics.LineSubPixelShift * 2),
                (byte)(a >> LineAABasics.LineSubPixelShift * 2));
        }
    };
}
