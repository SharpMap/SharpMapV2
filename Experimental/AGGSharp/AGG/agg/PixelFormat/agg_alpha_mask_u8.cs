
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
// scanline_u8 class
//
//----------------------------------------------------------------------------

using AGG.Buffer;
namespace AGG.PixelFormat
{
    public interface IAlphaMask
    {
        byte Pixel(int x, int y);
        byte CombinePixel(int x, int y, byte val);
        unsafe void FillHSpan(int x, int y, byte* dst, int num_pix);
        unsafe void FillVSpan(int x, int y, byte* dst, int num_pix);
        unsafe void CombineHSpanFullCover(int x, int y, byte* dst, int num_pix);
        unsafe void CombineHSpan(int x, int y, byte* dst, int num_pix);
        unsafe void CombineVSpan(int x, int y, byte* dst, int num_pix);
    };

    public sealed class AlphaMaskByteUnclipped : IAlphaMask
    {
        RasterBuffer m_rbuf;
        uint m_Step;
        uint m_Offset;

        public static int cover_shift = 8;
        public static int cover_none = 0;
        public static int cover_full = 255;

        public AlphaMaskByteUnclipped(RasterBuffer rbuf, uint Step, uint Offset)
        {
            m_Step = Step;
            m_Offset = Offset;
            m_rbuf = rbuf;
        }

        public void Attach(RasterBuffer rbuf) { m_rbuf = rbuf; }

        //--------------------------------------------------------------------
        public byte Pixel(int x, int y)
        {
            unsafe
            {
                return *(m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset);
            }
        }

        //--------------------------------------------------------------------
        public byte CombinePixel(int x, int y, byte val)
        {
            unsafe
            {
                return (byte)((cover_full + val * *(m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset)) >> cover_shift);
            }
        }


        //--------------------------------------------------------------------
        public unsafe void FillHSpan(int x, int y, byte* dst, int num_pix)
        {
            byte* mask = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;
            do
            {
                *dst++ = *mask;
                mask += m_Step;
            }
            while (--num_pix != 0);
        }

        public unsafe void CombineHSpanFullCover(int x, int y, byte* dst, int num_pix)
        {
            byte* mask = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;
            do
            {
                *dst = *mask;
                ++dst;
                mask += m_Step;
            }
            while (--num_pix != 0);
        }

        //--------------------------------------------------------------------
        public unsafe void CombineHSpan(int x, int y, byte* dst, int num_pix)
        {
            byte* mask = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;
            do
            {
                *dst = (byte)((cover_full + (*dst) * (*mask)) >> cover_shift);
                ++dst;
                mask += m_Step;
            }
            while (--num_pix != 0);
        }


        //--------------------------------------------------------------------
        public unsafe void FillVSpan(int x, int y, byte* dst, int num_pix)
        {
            byte* mask = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;
            do
            {
                *dst++ = *mask;
                mask += m_rbuf.StrideInBytes;
            }
            while (--num_pix != 0);
        }


        //--------------------------------------------------------------------
        public unsafe void CombineVSpan(int x, int y, byte* dst, int num_pix)
        {
            byte* mask = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;
            do
            {
                *dst = (byte)((cover_full + (*dst) * (*mask)) >> cover_shift);
                ++dst;
                mask += m_rbuf.StrideInBytes;
            }
            while (--num_pix != 0);
        }
    };

    public sealed class AlphaMaskByteClipped : IAlphaMask
    {
        RasterBuffer m_rbuf;
        uint m_Step;
        uint m_Offset;

        public static int CoverShift = 8;
        public static int CoverNone = 0;
        public static int CoverFull = 255;

        public AlphaMaskByteClipped(RasterBuffer rbuf, uint Step, uint Offset)
        {
            m_Step = Step;
            m_Offset = Offset;
            m_rbuf = rbuf;
        }

        public void Attach(RasterBuffer rbuf) { m_rbuf = rbuf; }


        //--------------------------------------------------------------------
        public byte Pixel(int x, int y)
        {
            if (x >= 0 && y >= 0 &&
               x < (int)m_rbuf.Width &&
               y < (int)m_rbuf.Height)
            {
                unsafe
                {
                    return *(m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset);
                }
            }
            return 0;
        }

        //--------------------------------------------------------------------
        public byte CombinePixel(int x, int y, byte val)
        {
            if (x >= 0 && y >= 0 &&
               x < (int)m_rbuf.Width &&
               y < (int)m_rbuf.Height)
            {
                unsafe
                {
                    return (byte)((CoverFull + val * *(m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset)) >> CoverShift);
                }
            }
            return 0;
        }

        //--------------------------------------------------------------------
        public unsafe void FillHSpan(int x, int y, byte* dst, int num_pix)
        {
            int xmax = (int)m_rbuf.Width - 1;
            int ymax = (int)m_rbuf.Height - 1;

            int count = num_pix;
            byte* covers = dst;

            if (y < 0 || y > ymax)
            {
                Basics.MemClear(dst, num_pix);
                return;
            }

            if (x < 0)
            {
                count += x;
                if (count <= 0)
                {
                    Basics.MemClear(dst, num_pix);
                    return;
                }
                Basics.MemClear(covers, -x);
                covers -= x;
                x = 0;
            }

            if (x + count > xmax)
            {
                int rest = x + count - xmax - 1;
                count -= rest;
                if (count <= 0)
                {
                    Basics.MemClear(dst, num_pix);
                    return;
                }
                Basics.MemClear(covers + count, rest);
            }

            byte* mask = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;
            do
            {
                *covers++ = *(mask);
                mask += m_Step;
            }
            while (--count != 0);
        }


        public unsafe void CombineHSpanFullCover(int x, int y, byte* dst, int num_pix)
        {
            int xmax = (int)m_rbuf.Width - 1;
            int ymax = (int)m_rbuf.Height - 1;

            int count = num_pix;
            byte* covers = dst;

            if (y < 0 || y > ymax)
            {
                Basics.MemClear(dst, num_pix);
                return;
            }

            if (x < 0)
            {
                count += x;
                if (count <= 0)
                {
                    Basics.MemClear(dst, num_pix);
                    return;
                }
                Basics.MemClear(covers, -x);
                covers -= x;
                x = 0;
            }

            if (x + count > xmax)
            {
                int rest = x + count - xmax - 1;
                count -= rest;
                if (count <= 0)
                {
                    Basics.MemClear(dst, num_pix);
                    return;
                }
                Basics.MemClear(covers + count, rest);
            }

            byte* mask = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;
            do
            {
                *covers = *mask;
                ++covers;
                mask += m_Step;
            }
            while (--count != 0);
        }

        //--------------------------------------------------------------------
        public unsafe void CombineHSpan(int x, int y, byte* dst, int num_pix)
        {
            int xmax = (int)m_rbuf.Width - 1;
            int ymax = (int)m_rbuf.Height - 1;

            int count = num_pix;
            byte* covers = dst;

            if (y < 0 || y > ymax)
            {
                Basics.MemClear(dst, num_pix);
                return;
            }

            if (x < 0)
            {
                count += x;
                if (count <= 0)
                {
                    Basics.MemClear(dst, num_pix);
                    return;
                }
                Basics.MemClear(covers, -x);
                covers -= x;
                x = 0;
            }

            if (x + count > xmax)
            {
                int rest = x + count - xmax - 1;
                count -= rest;
                if (count <= 0)
                {
                    Basics.MemClear(dst, num_pix);
                    return;
                }
                Basics.MemClear(covers + count, rest);
            }

            byte* mask = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;
            do
            {
                *covers = (byte)((CoverFull + (*covers) *
                                       *mask) >>
                                       CoverShift);
                ++covers;
                mask += m_Step;
            }
            while (--count != 0);
        }

        //--------------------------------------------------------------------
        public unsafe void FillVSpan(int x, int y, byte* dst, int num_pix)
        {
            int xmax = (int)m_rbuf.Width - 1;
            int ymax = (int)m_rbuf.Height - 1;

            int count = num_pix;
            byte* covers = dst;

            if (x < 0 || x > xmax)
            {
                Basics.MemClear(dst, num_pix);
                return;
            }

            if (y < 0)
            {
                count += y;
                if (count <= 0)
                {
                    Basics.MemClear(dst, num_pix);
                    return;
                }
                Basics.MemClear(covers, -y);
                covers -= y;
                y = 0;
            }

            if (y + count > ymax)
            {
                int rest = y + count - ymax - 1;
                count -= rest;
                if (count <= 0)
                {
                    Basics.MemClear(dst, num_pix);
                    return;
                }
                Basics.MemClear(covers + count, rest);
            }

            byte* mask = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;
            do
            {
                *covers++ = *mask;
                mask += m_rbuf.StrideInBytes;
            }
            while (--count != 0);
        }

        //--------------------------------------------------------------------
        public unsafe void CombineVSpan(int x, int y, byte* dst, int num_pix)
        {
            int xmax = (int)m_rbuf.Width - 1;
            int ymax = (int)m_rbuf.Height - 1;

            int count = num_pix;
            byte* covers = dst;

            if (x < 0 || x > xmax)
            {
                Basics.MemClear(dst, num_pix);
                return;
            }

            if (y < 0)
            {
                count += y;
                if (count <= 0)
                {
                    Basics.MemClear(dst, num_pix);
                    return;
                }
                Basics.MemClear(covers, -y);
                covers -= y;
                y = 0;
            }

            if (y + count > ymax)
            {
                int rest = y + count - ymax - 1;
                count -= rest;
                if (count <= 0)
                {
                    Basics.MemClear(dst, num_pix);
                    return;
                }
                Basics.MemClear(covers + count, rest);
            }

            byte* mask = m_rbuf.GetPixelPointer(y) + x * m_Step + m_Offset;
            do
            {
                *covers = (byte)((CoverFull + (*covers) * (*mask)) >> CoverShift);
                ++covers;
                mask += m_rbuf.StrideInBytes;
            }
            while (--count != 0);
        }
    };
}