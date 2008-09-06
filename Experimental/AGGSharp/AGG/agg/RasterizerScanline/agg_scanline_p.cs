
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
// Class scanline_p - a general purpose scanline container with packed spans.
//
//----------------------------------------------------------------------------
//
// Adaptation for 32-bit screen coordinates (scanline32_p) has been sponsored by 
// Liberty Technology Systems, Inc., visit http://lib-sys.com
//
// Liberty Technology Systems, Inc. is the provider of
// PostScript and PDF technology for software developers.
// 
//----------------------------------------------------------------------------
//#ifndef AGG_SCANLINE_P_INCLUDED
//#define AGG_SCANLINE_P_INCLUDED

namespace AGG.Scanline
{
    //=============================================================scanline_p8
    // 
    // This is a general purpose scanline container which supports the interface 
    // used in the rasterizer::render(). See description of scanline_u8
    // for details.
    // 
    //------------------------------------------------------------------------
    public sealed class ScanlinePacked8 : IScanlineCache
    {
        private int m_last_x;
        private int m_y;

        public int Y
        {
            get { return m_y; }
            private set { m_y = value; }
        }

        private ArrayPOD<byte> m_covers;
        private uint m_cover_index;
        private ArrayPOD<ScanlineSpan> m_spans;
        private uint m_span_index;
        private uint m_interator_index;

        public ScanlineSpan GetNextScanlineSpan()
        {
            m_interator_index++;
            unsafe
            {
                return m_spans.Array[m_interator_index - 1];
            }
        }

        public ScanlinePacked8()
        {
            m_last_x = 0x7FFFFFF0;
            m_covers = new ArrayPOD<byte>(1000);
            m_spans = new ArrayPOD<ScanlineSpan>(1000);
        }

        //--------------------------------------------------------------------
        public void Reset(int min_x, int max_x)
        {
            int max_len = max_x - min_x + 3;
            if (max_len > m_spans.Size())
            {
                m_spans.Resize(max_len);
                m_covers.Resize(max_len);
            }
            m_last_x = 0x7FFFFFF0;
            m_cover_index = 0;
            m_span_index = 0;
            m_spans.Array[m_span_index].Len = 0;
        }

        //--------------------------------------------------------------------
        public void AddCell(int x, uint cover)
        {
            m_covers.Array[m_cover_index] = (byte)cover;
            if (x == m_last_x + 1 && m_spans.Array[m_span_index].Len > 0)
            {
                m_spans.Array[m_span_index].Len++;
            }
            else
            {
                m_span_index++;
                m_spans.Array[m_span_index].CoverIndex = m_cover_index;
                m_spans.Array[m_span_index].X = (short)x;
                m_spans.Array[m_span_index].Len = 1;
            }
            m_last_x = x;
            m_cover_index++;
        }

        //--------------------------------------------------------------------
        unsafe public void AddCells(int x, int len, byte* covers)
        {
            for (uint i = 0; i < len; i++)
            {
                m_covers.Array[m_cover_index + i] = covers[i];
            }

            if (x == m_last_x + 1 && m_spans.Array[m_span_index].Len > 0)
            {
                m_spans.Array[m_span_index].Len += (short)len;
            }
            else
            {
                m_span_index++;
                m_spans.Array[m_span_index].CoverIndex = m_cover_index;
                m_spans.Array[m_span_index].X = (short)x;
                m_spans.Array[m_span_index].Len = (short)len;
            }

            m_cover_index += (uint)len;
            m_last_x = x + (int)len - 1;
        }

        //--------------------------------------------------------------------
        public void AddSpan(int x, int len, uint cover)
        {
            if (x == m_last_x + 1
                && m_spans.Array[m_span_index].Len < 0
                && cover == m_spans.Array[m_span_index].CoverIndex)
            {
                m_spans.Array[m_span_index].Len -= (short)len;
            }
            else
            {
                m_covers.Array[m_cover_index] = (byte)cover;
                m_span_index++;
                m_spans.Array[m_span_index].CoverIndex = m_cover_index++;
                m_spans.Array[m_span_index].X = (short)x;
                m_spans.Array[m_span_index].Len = (short)(-(int)(len));
            }
            m_last_x = x + (int)len - 1;
        }

        //--------------------------------------------------------------------
        public void Finalize(int y)
        {
            Y = y;
        }

        //--------------------------------------------------------------------
        public void ResetSpans()
        {
            m_last_x = 0x7FFFFFF0;
            m_cover_index = 0;
            m_span_index = 0;
            m_spans.Array[m_span_index].Len = 0;
        }

        //public int Y() { return m_y; }

        public uint NumSpans { get { return (uint)m_span_index; } }
        public ScanlineSpan Begin()
        {
            m_interator_index = 1;
            return GetNextScanlineSpan();
        }

        public byte[] Covers
        {
            get
            {
                return m_covers.Array;
            }
        }
    };
}
