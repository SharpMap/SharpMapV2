
/*
 *	Portions of this file are  � 2008 Newgrove Consultants Limited, 
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
namespace AGG
{
    //----------------------------------------------------------span_allocator
    public class SpanAllocator
    {
        private ArrayPOD<RGBA_Bytes> m_span;

        public SpanAllocator()
        {
            m_span = new ArrayPOD<RGBA_Bytes>(255);
        }

        //--------------------------------------------------------------------
        public ArrayPOD<RGBA_Bytes> Allocate(uint span_len)
        {
            if (span_len > m_span.Size())
            {
                // To reduce the number of reallocs we align the 
                // span_len to 256 color elements. 
                // Well, I just like this number and it looks reasonable.
                //-----------------------
                m_span.Resize((((int)span_len + 255) >> 8) << 8);
            }
            return m_span;
        }

        public ArrayPOD<RGBA_Bytes> Span { get { return m_span; } }
        public uint MaxSpanLen { get { return m_span.Size(); } }
    };
}
