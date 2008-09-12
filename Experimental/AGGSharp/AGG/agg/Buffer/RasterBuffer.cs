
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
// class rendering_buffer
//
//----------------------------------------------------------------------------

//#ifndef AGG_RENDERING_BUFFER_INCLUDED
//#define AGG_RENDERING_BUFFER_INCLUDED

//#include "agg_array.h"
//using row_data = const_row_info<T>;

namespace AGG.Buffer
{


    public sealed class RasterBuffer : IRasterBuffer
    {
        unsafe byte* m_BufferPointer;    // Pointer to rendering buffer data
        unsafe byte* m_FirstPixelPointer;  // Pointer to first pixel depending on stride 
        uint m_Width;  // Width in pixels
        uint m_Height; // Height in pixels
        uint m_BitsPerPixel;
        uint m_BytesPerPixel;
        int m_StrideInBytes; // Number of bytes per row. Can be < 0

        //-------------------------------------------------------------------
        public RasterBuffer()
        {
        }

        //--------------------------------------------------------------------
        public unsafe RasterBuffer(byte* buf, uint width, uint height, int stride, uint BytesPerPixel)
        {
            attach(buf, width, height, stride, BytesPerPixel);
        }

        public uint BitsPerPixel
        {
            get
            {
                return m_BitsPerPixel;
            }
        }

        //--------------------------------------------------------------------
        public unsafe void attach(byte* buf, uint width, uint height, int stride, uint BitsPerPixel)
        {
            m_BufferPointer = m_FirstPixelPointer = buf;
            m_Width = width;
            m_Height = height;
            m_StrideInBytes = stride;
            m_BitsPerPixel = BitsPerPixel;
            m_BytesPerPixel = BitsPerPixel / 8;
            if (stride < 0)
            {
                int addAmount = -((int)((int)height - 1) * stride);
                m_FirstPixelPointer = &m_BufferPointer[addAmount];
            }
        }

        public void dettachBuffer()
        {
            unsafe
            {
                m_BitsPerPixel = 0;
                m_BufferPointer = m_FirstPixelPointer = null;
                m_Width = m_Height = 0;
                m_StrideInBytes = 0;
            }
        }

        //--------------------------------------------------------------------
        public unsafe byte* GetBuffer() { return m_BufferPointer; }

        public uint Width { get { return m_Width; } }
        public uint Height { get { return m_Height; } }
        public int StrideInBytes { get { return m_StrideInBytes; } }

        public uint StrideInBytesAbs
        {
            get
            {
                return (m_StrideInBytes < 0) ? (uint)(-m_StrideInBytes) : (uint)(m_StrideInBytes);
            }
        }

        public unsafe byte* GetPixelPointer(int x, int y)
        {
            return &m_FirstPixelPointer[y * m_StrideInBytes + x * m_BytesPerPixel];
        }

        public unsafe byte* GetPixelPointer(int y)
        {
            return &m_FirstPixelPointer[y * m_StrideInBytes];
        }

        public void CopyFrom(IRasterBuffer src)
        {
            uint h = Height;
            if (src.Height < h) h = src.Height;

            uint StrideABS = StrideInBytesAbs;
            if (src.StrideInBytesAbs < StrideABS)
            {
                StrideABS = src.StrideInBytesAbs;
            }

            uint y;
            unsafe
            {
                for (y = 0; y < h; y++)
                {
                    Basics.MemCopy(GetPixelPointer((int)y), src.GetPixelPointer((int)y), (int)StrideABS);
                }
            }
        }
    };
}