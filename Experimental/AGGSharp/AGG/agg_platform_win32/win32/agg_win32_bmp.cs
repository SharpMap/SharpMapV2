//----------------------------------------------------------------------------
//
//----------------------------------------------------------------------------
// Contact: mcseemagg@yahoo.com
//----------------------------------------------------------------------------
//
// class pixel_map
//
//----------------------------------------------------------------------------

//#include "platform/win32/agg_win32_bmp.h"
//#include "agg_basics.h"
using System.Drawing;
using System.Drawing.Imaging;

namespace AGG.UI.win32
{
    public class pixel_map //: IRenderingBuffer
    {
        Bitmap    m_bmp;
        BitmapData m_bmd;

        unsafe byte* m_buf;
        uint m_BitsPerPixel;

        ~pixel_map()
        {
            destroy();
        }

        public pixel_map()
        {
        }

        unsafe void destroy()
        {
             m_bmp  = null;
             m_buf = null;
        }

        public void create(uint width, uint height, uint BytesPerPixel)
        {
            create(width, height, BytesPerPixel, 255);
        }

        unsafe public void create(uint width, uint height, uint BitsPerPixel, uint clear_val)
        {
            destroy();
            if(width == 0)  width = 1;
            if(height == 0) height = 1;
            m_BitsPerPixel = BitsPerPixel;
            switch (m_BitsPerPixel)
            {
                case 24:
                    m_bmp = new Bitmap((int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    break;

                case 32:
                    m_bmp = new Bitmap((int)width, (int)height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    break;

                default:
                    throw new System.NotImplementedException();
            }

            m_bmd = m_bmp.LockBits(new Rectangle(0, 0, m_bmp.Width, m_bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, m_bmp.PixelFormat);
            m_buf = (byte*)m_bmd.Scan0;
            if (clear_val <= 255)
            {
                clear(clear_val);
            }
        }

        public unsafe void LockBufferAndCallbackFunction()
        {
            m_bmd = m_bmp.LockBits(new Rectangle(0, 0, m_bmp.Width, m_bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, m_bmp.PixelFormat);
            m_buf = (byte*)m_bmd.Scan0;


            m_bmp.UnlockBits(m_bmd);
            m_bmd = null;
            m_buf = null;
        }

        unsafe void clear(uint clear_val)
        {
            if (m_buf != null)
            {
                if(clear_val == 0)
                {
                    Basics.MemClear(m_buf, m_bmd.Height * m_bmd.Stride);
                }
                else
                {
                    Basics.MemSet(m_buf, (byte)clear_val, m_bmd.Height * m_bmd.Stride);
                }
            }
        }

        //------------------------------------------------------------------------
        public void draw(Graphics displayGraphics)
        {
            m_bmp.UnlockBits(m_bmd);
            displayGraphics.DrawImage(m_bmp, 0, 0);
            m_bmd = m_bmp.LockBits(new Rectangle(0, 0, m_bmp.Width, m_bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, m_bmp.PixelFormat);
        }

        public void draw(Graphics displayGraphics, Rectangle device_rect, Rectangle bmp_rect)
        {
            displayGraphics.DrawImage(m_bmp, 0, 0);
        }

        //------------------------------------------------------------------------
        public bool load_from_bmp(string filename)
        {
            if (System.IO.File.Exists(filename))
            {
                m_bmp = new Bitmap(filename);
                if (m_bmp != null)
                {
                    //m_is_internal = 1;
                    m_bmd = m_bmp.LockBits(new Rectangle(0, 0, m_bmp.Width, m_bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, m_bmp.PixelFormat);
                    unsafe
                    {
                        m_buf = (byte*)m_bmd.Scan0;
                    }
                    switch (m_bmp.PixelFormat)
                    {
                        case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                            m_BitsPerPixel = 24;
                            break;

                        default:
                            throw new System.NotImplementedException();
                    }
                    return true;
                }
            }

            return false;
        }

        //------------------------------------------------------------------------
        public unsafe byte* buf()
        {
            return m_buf;
        }

        //------------------------------------------------------------------------
        public uint width()
        {
            return (uint)m_bmp.Width;
        }

        //------------------------------------------------------------------------
        public uint height()
        {
            return (uint)m_bmp.Height;
        }

        //------------------------------------------------------------------------
        public int stride()
        {
            return m_bmd.Stride;
        }

        public uint bpp() { return m_BitsPerPixel; }
    }
}



