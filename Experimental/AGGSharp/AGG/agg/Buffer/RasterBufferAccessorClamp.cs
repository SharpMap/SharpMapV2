
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
using AGG.PixelFormat;

namespace AGG.Buffer
{


    //-----------------------------------------------------image_accessor_clip
   

    /*
        //--------------------------------------------------image_accessor_no_clip
        template<class PixFmt> class image_accessor_no_clip
        {
        public:
            typedef PixFmt   pixfmt_type;
            typedef typename pixfmt_type::color_type color_type;
            typedef typename pixfmt_type::order_type order_type;
            typedef typename pixfmt_type::value_type value_type;
            enum pix_width_e { pix_width = pixfmt_type::pix_width };

            image_accessor_no_clip() {}
            explicit image_accessor_no_clip(pixfmt_type& pixf) : 
                m_pixf(&pixf) 
            {}

            void Attach(pixfmt_type& pixf)
            {
                m_pixf = &pixf;
            }

            byte* span(int x, int y, uint)
            {
                m_x = x;
                m_y = y;
                return m_pix_ptr = m_pixf->pix_ptr(x, y);
            }

            byte* next_x()
            {
                return m_pix_ptr += pix_width;
            }

            byte* next_y()
            {
                ++m_y;
                return m_pix_ptr = m_pixf->pix_ptr(m_x, m_y);
            }

        private:
            pixfmt_type* m_pixf;
            int                m_x, m_y;
            byte*       m_pix_ptr;
        };
     */

        public sealed class RasterBufferAccessorClamp : IRasterBufferAccessor
        {
            private IPixelFormat m_pixf;
            private int m_x, m_x0, m_y;
            unsafe private byte* m_pix_ptr;
            uint m_PixelWidthInBytes;

            public RasterBufferAccessorClamp(IPixelFormat pixf)
            {
                m_pixf = pixf;
                m_PixelWidthInBytes = m_pixf.PixelWidthInBytes;
            }

            void attach(IPixelFormat pixf)
            {
                m_pixf = pixf;
            }

            public IPixelFormat PixelFormat
            {
                get
                {
                    return m_pixf;
                }
            }

            private unsafe byte* pixel()
            {
                int x = m_x;
                int y = m_y;
                if(x < 0) x = 0;
                if(y < 0) y = 0;
                if (x >= (int)m_pixf.Width)
                {
                    x = (int)m_pixf.Width - 1;
                }

                if (y >= (int)m_pixf.Height)
                {
                    y = (int)m_pixf.Height - 1;
                }

                return m_pixf.PixPtr(x, y);
            }

            public unsafe byte* span(int x, int y, uint len)
            {
                m_x = m_x0 = x;
                m_y = y;
                if(y >= 0 && y < (int)m_pixf.Height &&
                   x >= 0 && x+len <= (int)m_pixf.Width)
                {
                    return m_pix_ptr = m_pixf.PixPtr(x, y);
                }
                m_pix_ptr = null;
                return pixel();
            }

            public unsafe byte* next_x()
            {
                if (m_pix_ptr != null)
                {
                    return m_pix_ptr += m_PixelWidthInBytes;
                }
                ++m_x;
                return pixel();
            }

            public unsafe byte* next_y()
            {
                ++m_y;
                m_x = m_x0;
                if(m_pix_ptr != null 
                    && m_y >= 0 
                    && m_y < (int)m_pixf.Height)
                {
                    return m_pix_ptr = m_pixf.PixPtr(m_x, m_y);
                }
                m_pix_ptr = null;
                return pixel();
            }
        };
    /*

        //-----------------------------------------------------image_accessor_wrap
        template<class PixFmt, class WrapX, class WrapY> class image_accessor_wrap
        {
        public:
            typedef PixFmt   pixfmt_type;
            typedef typename pixfmt_type::color_type color_type;
            typedef typename pixfmt_type::order_type order_type;
            typedef typename pixfmt_type::value_type value_type;
            enum pix_width_e { pix_width = pixfmt_type::pix_width };

            image_accessor_wrap() {}
            explicit image_accessor_wrap(pixfmt_type& pixf) : 
                m_pixf(&pixf), 
                m_wrap_x(pixf.width()), 
                m_wrap_y(pixf.height())
            {}

            void Attach(pixfmt_type& pixf)
            {
                m_pixf = &pixf;
            }

            byte* span(int x, int y, uint)
            {
                m_x = x;
                m_row_ptr = m_pixf->row_ptr(m_wrap_y(y));
                return m_row_ptr + m_wrap_x(x) * pix_width;
            }

            byte* next_x()
            {
                int x = ++m_wrap_x;
                return m_row_ptr + x * pix_width;
            }

            byte* next_y()
            {
                m_row_ptr = m_pixf->row_ptr(++m_wrap_y);
                return m_row_ptr + m_wrap_x(m_x) * pix_width;
            }

        private:
            pixfmt_type* m_pixf;
            byte*       m_row_ptr;
            int                m_x;
            WrapX              m_wrap_x;
            WrapY              m_wrap_y;
        };




        //--------------------------------------------------------wrap_mode_repeat
        class wrap_mode_repeat
        {
        public:
            wrap_mode_repeat() {}
            wrap_mode_repeat(uint size) : 
                m_size(size), 
                m_add(size * (0x3FFFFFFF / size)),
                m_value(0)
            {}

            uint operator() (int v)
            { 
                return m_value = (uint(v) + m_add) % m_size; 
            }

            uint operator++ ()
            {
                ++m_value;
                if(m_value >= m_size) m_value = 0;
                return m_value;
            }
        private:
            uint m_size;
            uint m_add;
            uint m_value;
        };


        //---------------------------------------------------wrap_mode_repeat_pow2
        class wrap_mode_repeat_pow2
        {
        public:
            wrap_mode_repeat_pow2() {}
            wrap_mode_repeat_pow2(uint size) : m_value(0)
            {
                m_mask = 1;
                while(m_mask < size) m_mask = (m_mask << 1) | 1;
                m_mask >>= 1;
            }
            uint operator() (int v)
            { 
                return m_value = uint(v) & m_mask;
            }
            uint operator++ ()
            {
                ++m_value;
                if(m_value > m_mask) m_value = 0;
                return m_value;
            }
        private:
            uint m_mask;
            uint m_value;
        };


        //----------------------------------------------wrap_mode_repeat_auto_pow2
        class wrap_mode_repeat_auto_pow2
        {
        public:
            wrap_mode_repeat_auto_pow2() {}
            wrap_mode_repeat_auto_pow2(uint size) :
                m_size(size),
                m_add(size * (0x3FFFFFFF / size)),
                m_mask((m_size & (m_size-1)) ? 0 : m_size-1),
                m_value(0)
            {}

            uint operator() (int v) 
            { 
                if(m_mask) return m_value = uint(v) & m_mask;
                return m_value = (uint(v) + m_add) % m_size;
            }
            uint operator++ ()
            {
                ++m_value;
                if(m_value >= m_size) m_value = 0;
                return m_value;
            }

        private:
            uint m_size;
            uint m_add;
            uint m_mask;
            uint m_value;
        };


        //-------------------------------------------------------wrap_mode_reflect
        class wrap_mode_reflect
        {
        public:
            wrap_mode_reflect() {}
            wrap_mode_reflect(uint size) : 
                m_size(size), 
                m_size2(size * 2),
                m_add(m_size2 * (0x3FFFFFFF / m_size2)),
                m_value(0)
            {}

            uint operator() (int v)
            { 
                m_value = (uint(v) + m_add) % m_size2;
                if(m_value >= m_size) return m_size2 - m_value - 1;
                return m_value;
            }

            uint operator++ ()
            {
                ++m_value;
                if(m_value >= m_size2) m_value = 0;
                if(m_value >= m_size) return m_size2 - m_value - 1;
                return m_value;
            }
        private:
            uint m_size;
            uint m_size2;
            uint m_add;
            uint m_value;
        };



        //--------------------------------------------------wrap_mode_reflect_pow2
        class wrap_mode_reflect_pow2
        {
        public:
            wrap_mode_reflect_pow2() {}
            wrap_mode_reflect_pow2(uint size) : m_value(0)
            {
                m_mask = 1;
                m_size = 1;
                while(m_mask < size) 
                {
                    m_mask = (m_mask << 1) | 1;
                    m_size <<= 1;
                }
            }
            uint operator() (int v)
            { 
                m_value = uint(v) & m_mask;
                if(m_value >= m_size) return m_mask - m_value;
                return m_value;
            }
            uint operator++ ()
            {
                ++m_value;
                m_value &= m_mask;
                if(m_value >= m_size) return m_mask - m_value;
                return m_value;
            }
        private:
            uint m_size;
            uint m_mask;
            uint m_value;
        };



        //---------------------------------------------wrap_mode_reflect_auto_pow2
        class wrap_mode_reflect_auto_pow2
        {
        public:
            wrap_mode_reflect_auto_pow2() {}
            wrap_mode_reflect_auto_pow2(uint size) :
                m_size(size),
                m_size2(size * 2),
                m_add(m_size2 * (0x3FFFFFFF / m_size2)),
                m_mask((m_size2 & (m_size2-1)) ? 0 : m_size2-1),
                m_value(0)
            {}

            uint operator() (int v) 
            { 
                m_value = m_mask ? uint(v) & m_mask : 
                                  (uint(v) + m_add) % m_size2;
                if(m_value >= m_size) return m_size2 - m_value - 1;
                return m_value;            
            }
            uint operator++ ()
            {
                ++m_value;
                if(m_value >= m_size2) m_value = 0;
                if(m_value >= m_size) return m_size2 - m_value - 1;
                return m_value;
            }

        private:
            uint m_size;
            uint m_size2;
            uint m_add;
            uint m_mask;
            uint m_value;
        };
     */
}