using System.Runtime.InteropServices;
using AGG.Color;
using AGG.PixelFormat;

namespace AGG.Buffer
{
    public sealed class RasterBufferAccessorClip 
        : IRasterBufferAccessor
    {
        private IPixelFormat m_pixf;
        unsafe private byte* m_pBackBufferColor;
        private int m_x, m_x0, m_y;
        unsafe private byte* m_pix_ptr;
        uint m_PixelWidthInBytes;

        public RasterBufferAccessorClip(IPixelFormat pixf, RGBA_Doubles bk)
        {
            m_pixf = pixf;
            m_PixelWidthInBytes = m_pixf.PixelWidthInBytes;
            //pixfmt_alpha_blend_bgra32.make_pix(m_bk_buf, bk);
            unsafe
            {
                m_pBackBufferColor = (byte*)Marshal.AllocHGlobal(16);
            }
        }

        ~RasterBufferAccessorClip()
        {
            unsafe
            {
                Marshal.FreeHGlobal((System.IntPtr)m_pBackBufferColor);
            }
        }

        public IPixelFormat PixelFormat
        {
            get
            {
                return m_pixf;
            }
        }

        public void attach(IPixelFormat pixf)
        {
            m_pixf = pixf;
        }

        IPixelFormat GetPixelFormat()
        {
            return m_pixf;
        }

        public void background_color(IColorType bk)
        {
            unsafe { m_pixf.MakePix(m_pBackBufferColor, bk); }
        }

        unsafe private byte* pixel()
        {
            if (m_y >= 0 && m_y < (int)m_pixf.Height &&
               m_x >= 0 && m_x < (int)m_pixf.Width)
            {
                return m_pixf.PixPtr(m_x, m_y);
            }

            return m_pBackBufferColor;
        }

        unsafe public byte* span(int x, int y, uint len)
        {
            m_x = m_x0 = x;
            m_y = y;
            if (y >= 0 && y < (int)m_pixf.Height &&
               x >= 0 && x + (int)len <= (int)m_pixf.Width)
            {
                return m_pix_ptr = m_pixf.PixPtr(x, y);
            }
            m_pix_ptr = null;
            return pixel();
        }

        unsafe public byte* next_x()
        {
            if (m_pix_ptr != null)
            {
                return m_pix_ptr += (int)m_PixelWidthInBytes;
            }

            ++m_x;
            return pixel();
        }

        unsafe public byte* next_y()
        {
            ++m_y;
            m_x = m_x0;
            if (m_pix_ptr != null
                && m_y >= 0
                && m_y < (int)m_pixf.Height)
            {
                return m_pix_ptr = m_pixf.PixPtr(m_x, m_y);
            }
            m_pix_ptr = null;
            return pixel();
        }
    };
}
