
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
// Class scanline_bin - binary scanline.
//
//----------------------------------------------------------------------------
//
// Adaptation for 32-bit screen coordinates (scanline32_bin) has been sponsored by 
// Liberty Technology Systems, Inc., visit http://lib-sys.com
//
// Liberty Technology Systems, Inc. is the provider of
// PostScript and PDF technology for software developers.
// 
//----------------------------------------------------------------------------

namespace AGG.Scanline
{

    //=============================================================scanline_bin
    // 
    // This is binary scaline container which supports the interface 
    // used in the rasterizer::render(). See description of agg_scanline_u8 
    // for details.
    // 
    //------------------------------------------------------------------------
    public sealed class ScanlineBin : IScanlineCache
    {
        private int m_last_x;
        private int m_y;
        private ArrayPOD<ScanlineSpan> m_spans;
        private uint m_span_index;
        private uint m_interator_index;

        public ScanlineSpan GetNextScanlineSpan()
        {
            m_interator_index++;
            return m_spans.Array[m_interator_index - 1];
        }

        //--------------------------------------------------------------------
        public ScanlineBin()
        {
            m_last_x = (0x7FFFFFF0);
            m_spans = new ArrayPOD<ScanlineSpan>(1000);
            m_span_index = 0;
        }

        //--------------------------------------------------------------------
        public void Reset(int min_x, int max_x)
        {
            int max_len = max_x - min_x + 3;
            if (max_len > m_spans.Size())
            {
                m_spans.Resize(max_len);
            }
            m_last_x = 0x7FFFFFF0;
            m_span_index = 0;
        }

        //--------------------------------------------------------------------
        public void AddCell(int x, uint cover)
        {
            if (x == m_last_x + 1)
            {
                m_spans.Array[m_span_index].Len++;
            }
            else
            {
                m_span_index++;
                m_spans.Array[m_span_index].X = (short)x;
                m_spans.Array[m_span_index].Len = 1;
            }
            m_last_x = x;
        }

        //--------------------------------------------------------------------
        public void AddSpan(int x, int len, uint cover)
        {
            if (x == m_last_x + 1)
            {
                m_spans.Array[m_span_index].Len += (short)len;
            }
            else
            {
                m_span_index++;
                m_spans.Array[m_span_index].X = x;
                m_spans.Array[m_span_index].Len = (int)len;
            }
            m_last_x = x + len - 1;
        }

        /*
        //--------------------------------------------------------------------
        public void add_cells(int x, uint len, void*)
        {
            add_span(x, len, 0);
        }
         */

        //--------------------------------------------------------------------
        public void Finalize(int y)
        {
            m_y = y;
        }

        //--------------------------------------------------------------------
        public void ResetSpans()
        {
            m_last_x = 0x7FFFFFF0;
            m_span_index = 0;
        }

        //--------------------------------------------------------------------
        public int Y { get { return m_y; } }
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
                return null;
            }
        }
    };


    /*
//===========================================================scanline32_bin
class scanline32_bin
{
public:
    typedef int32 coord_type;

    //--------------------------------------------------------------------
    struct span
    {
        span() {}
        span(coord_type x_, coord_type len_) : x(x_), len(len_) {}

        coord_type x;
        coord_type len;
    };
    typedef pod_bvector<span, 4> span_array_type;


    //--------------------------------------------------------------------
    class_iterator
    {
    public:
       _iterator(span_array_type& spans) :
            m_spans(spans),
            m_span_idx(0)
        {}

        span& operator*()  { return m_spans[m_span_idx];  }
        span* operator->() { return &m_spans[m_span_idx]; }

        void operator ++ () { ++m_span_idx; }

    private:
        span_array_type& m_spans;
        uint               m_span_idx;
    };


    //--------------------------------------------------------------------
    scanline32_bin() : m_max_len(0), m_last_x(0x7FFFFFF0) {}

    //--------------------------------------------------------------------
    void reset(int min_x, int max_x)
    {
        m_last_x = 0x7FFFFFF0;
        m_spans.remove_all();
    }

    //--------------------------------------------------------------------
    void add_cell(int x, uint)
    {
        if(x == m_last_x+1)
        {
            m_spans.last().len++;
        }
        else
        {
            m_spans.add(span(coord_type(x), 1));
        }
        m_last_x = x;
    }

    //--------------------------------------------------------------------
    void add_span(int x, uint len, uint)
    {
        if(x == m_last_x+1)
        {
            m_spans.last().len += coord_type(len);
        }
        else
        {
            m_spans.add(span(coord_type(x), coord_type(len)));
        }
        m_last_x = x + len - 1;
    }

    //--------------------------------------------------------------------
    void add_cells(int x, uint len, void*)
    {
        add_span(x, len, 0);
    }

    //--------------------------------------------------------------------
    void finalize(int y) 
    { 
        m_y = y; 
    }

    //--------------------------------------------------------------------
    void reset_spans()
    {
        m_last_x = 0x7FFFFFF0;
        m_spans.remove_all();
    }

    //--------------------------------------------------------------------
    int            y()         { return m_y; }
    uint       num_spans() { return m_spans.size(); }
   _iterator begin()     { return_iterator(m_spans); }

private:
    scanline32_bin(scanline32_bin&);
    scanline32_bin operator = (scanline32_bin&);

    uint        m_max_len;
    int             m_last_x;
    int             m_y;
    span_array_type m_spans;
};
     */
}
