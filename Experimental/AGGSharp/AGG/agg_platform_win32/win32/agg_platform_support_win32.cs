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
// class platform_support
//
//----------------------------------------------------------------------------
#define USE_OPENGL

using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

using AGG.PixelFormat;

#if USE_OPENGL
using Tao.OpenGl;
using Tao.Platform.Windows;
using AGG.Scanline;
using AGG.Buffer;
using AGG.Rendering;
using AGG.Color;
using AGG.Rasterizer;
using NPack.Interfaces;
#endif

namespace AGG.UI.win32
{
    //------------------------------------------------------------------------
    public class PlatformSpecificWindow<T> : Form
                         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
#if USE_OPENGL
        protected static IntPtr hDC;                                              // Private GDI Device Context
        private static IntPtr hRC;                                              // Permanent Rendering Context
#endif

        public PlatformSupportAbstract<T>.PixelFormats m_format;
        public PlatformSupportAbstract<T>.PixelFormats m_sys_format;
        private ERenderOrigin m_RenderOrigin;
        //private HWND          m_hwnd;
        internal pixel_map m_pmap_window;
        internal pixel_map[] m_pmap_img = new pixel_map[(int)PlatformSupportAbstract<T>.max_images_e.max_images];
        private uint[] m_keymap = new uint[256];
        private uint m_last_translated_key;
        //private int           m_cur_x;
        //private int           m_cur_y;
        //private uint      m_input_flags;
        public bool m_WindowContentNeedsRedraw;
        public bool fullscreen;
        internal System.Diagnostics.Stopwatch m_StopWatch;
        PlatformSupportAbstract<T> m_app;

        public PlatformSpecificWindow(PlatformSupportAbstract<T> app, PlatformSupportAbstract<T>.PixelFormats format, ERenderOrigin RenderOrigin)
        {
#if USE_OPENGL
            this.CreateParams.ClassStyle = this.CreateParams.ClassStyle |       // Redraw On Size, And Own DC For Window.
                User.CS_HREDRAW | User.CS_VREDRAW | User.CS_OWNDC;
            //this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);            // No Need To Erase Form Background
            this.SetStyle(ControlStyles.DoubleBuffer, true);                    // Buffer Control
            this.SetStyle(ControlStyles.Opaque, true);                          // No Need To Draw Form Background
            this.SetStyle(ControlStyles.ResizeRedraw, true);                    // Redraw On Resize
            this.SetStyle(ControlStyles.UserPaint, true);                       // We'll Handle Painting Ourselves
#endif

            m_app = app;
            m_format = format;
            m_sys_format = PlatformSupportAbstract<T>.PixelFormats.pix_format_undefined;
            m_RenderOrigin = RenderOrigin;
            m_WindowContentNeedsRedraw = true;
            m_StopWatch = new Stopwatch();

            switch (m_format)
            {
                case PlatformSupportAbstract<T>.PixelFormats.pix_format_bw:
                    m_sys_format = PlatformSupportAbstract<T>.PixelFormats.pix_format_bw;
                    break;

                case PlatformSupportAbstract<T>.PixelFormats.pix_format_gray8:
                    m_sys_format = PlatformSupportAbstract<T>.PixelFormats.pix_format_gray8;
                    break;

                case PlatformSupportAbstract<T>.PixelFormats.pix_format_gray16:
                    m_sys_format = PlatformSupportAbstract<T>.PixelFormats.pix_format_gray8;
                    break;

                case PlatformSupportAbstract<T>.PixelFormats.pix_format_rgb565:
                case PlatformSupportAbstract<T>.PixelFormats.pix_format_rgb555:
                    m_sys_format = PlatformSupportAbstract<T>.PixelFormats.pix_format_rgb555;
                    break;

                case PlatformSupportAbstract<T>.PixelFormats.pix_format_rgbAAA:
                case PlatformSupportAbstract<T>.PixelFormats.pix_format_bgrAAA:
                case PlatformSupportAbstract<T>.PixelFormats.pix_format_rgbBBA:
                case PlatformSupportAbstract<T>.PixelFormats.pix_format_bgrABB:
                    m_sys_format = PlatformSupportAbstract<T>.PixelFormats.pix_format_bgr24;
                    break;

                case PlatformSupportAbstract<T>.PixelFormats.pix_format_rgb24:
                    m_sys_format = PlatformSupportAbstract<T>.PixelFormats.pix_format_rgb24;
                    break;

                case PlatformSupportAbstract<T>.PixelFormats.pix_format_bgr24:
                    m_sys_format = PlatformSupportAbstract<T>.PixelFormats.pix_format_bgr24;
                    break;

                case PlatformSupportAbstract<T>.PixelFormats.pix_format_rgb48:
                case PlatformSupportAbstract<T>.PixelFormats.pix_format_bgr48:
                    m_sys_format = PlatformSupportAbstract<T>.PixelFormats.pix_format_bgr24;
                    break;

                case PlatformSupportAbstract<T>.PixelFormats.pix_format_bgra32:
                    m_sys_format = PlatformSupportAbstract<T>.PixelFormats.pix_format_bgra32;
                    break;
                case PlatformSupportAbstract<T>.PixelFormats.pix_format_abgr32:
                    m_sys_format = PlatformSupportAbstract<T>.PixelFormats.pix_format_abgr32;
                    break;
                case PlatformSupportAbstract<T>.PixelFormats.pix_format_argb32:
                    m_sys_format = PlatformSupportAbstract<T>.PixelFormats.pix_format_argb32;
                    break;
                case PlatformSupportAbstract<T>.PixelFormats.pix_format_rgba32:
                    m_sys_format = PlatformSupportAbstract<T>.PixelFormats.pix_format_rgba32;
                    break;

                case PlatformSupportAbstract<T>.PixelFormats.pix_format_bgra64:
                case PlatformSupportAbstract<T>.PixelFormats.pix_format_abgr64:
                case PlatformSupportAbstract<T>.PixelFormats.pix_format_argb64:
                case PlatformSupportAbstract<T>.PixelFormats.pix_format_rgba64:
                    m_sys_format = PlatformSupportAbstract<T>.PixelFormats.pix_format_bgra32;
                    break;
            }

            m_StopWatch.Reset();
            m_StopWatch.Start();
        }

#if USE_OPENGL
        internal virtual bool CreateGLWindow(int width, int height, int bitsPerPixel, bool fullscreenflag)
        {
            int pixelFormat;                                                    // Holds The Results After Searching For A Match
            fullscreen = fullscreenflag;                                        // Set The Global Fullscreen Flag

            GC.Collect();                                                       // Request A Collection
            // This Forces A Swap
            Kernel.SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle, -1, -1);

            if (fullscreen)
            {
                Gdi.DEVMODE dmScreenSettings = new Gdi.DEVMODE();               // Device Mode
                // Size Of The Devmode Structure
                dmScreenSettings.dmSize = (short)Marshal.SizeOf(dmScreenSettings);
                dmScreenSettings.dmPelsWidth = width;                           // Selected Screen Width
                dmScreenSettings.dmPelsHeight = height;                         // Selected Screen Height
                dmScreenSettings.dmBitsPerPel = bitsPerPixel;                           // Selected Bits Per Pixel
                dmScreenSettings.dmFields = Gdi.DM_BITSPERPEL | Gdi.DM_PELSWIDTH | Gdi.DM_PELSHEIGHT;

                // Try To Set Selected Mode And Get Results.  NOTE: CDS_FULLSCREEN Gets Rid Of Start Bar.
                if (User.ChangeDisplaySettings(ref dmScreenSettings, User.CDS_FULLSCREEN) != User.DISP_CHANGE_SUCCESSFUL)
                {
                    // If The Mode Fails, Offer Two Options.  Quit Or Use Windowed Mode.
                    if (MessageBox.Show("The Requested Fullscreen Mode Is Not Supported By\nYour Video Card.  Use Windowed Mode Instead?", "NeHe GL",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    {
                        fullscreen = false;                                     // Windowed Mode Selected.  Fullscreen = false
                    }
                    else
                    {
                        // Pop up A Message Box Lessing User Know The Program Is Closing.
                        MessageBox.Show("Program Will Now Close.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return false;                                           // Return false
                    }
                }
            }

            if (fullscreen)
            {                                                    // Are We Still In Fullscreen Mode?
                FormBorderStyle = FormBorderStyle.None;                    // No Border
                Cursor.Hide();                                                  // Hide Mouse Pointer
            }
            else
            {                                                              // If Windowed
                FormBorderStyle = FormBorderStyle.Sizable;                 // Sizable
                Cursor.Show();                                                  // Show Mouse Pointer
            }

            Width = width;                                                 // Set Window Width
            Height = height + 40;                                               // Set Window Height

            Gdi.PIXELFORMATDESCRIPTOR pfd = new Gdi.PIXELFORMATDESCRIPTOR();    // pfd Tells Windows How We Want Things To Be
            pfd.nSize = (short)Marshal.SizeOf(pfd);                            // Size Of This Pixel Format Descriptor
            pfd.nVersion = 1;                                                   // Version Number
            pfd.dwFlags = Gdi.PFD_DRAW_TO_WINDOW |                              // Format Must Support Window
                Gdi.PFD_SUPPORT_OPENGL |                                        // Format Must Support OpenGL
                Gdi.PFD_DOUBLEBUFFER;                                           // Format Must Support Double Buffering
            pfd.iPixelType = (byte)Gdi.PFD_TYPE_RGBA;                          // Request An RGBA Format
            pfd.cColorBits = (byte)bitsPerPixel;                                       // Select Our Color Depth
            pfd.cRedBits = 0;                                                   // Color Bits Ignored
            pfd.cRedShift = 0;
            pfd.cGreenBits = 0;
            pfd.cGreenShift = 0;
            pfd.cBlueBits = 0;
            pfd.cBlueShift = 0;
            pfd.cAlphaBits = 0;                                                 // No Alpha Buffer
            pfd.cAlphaShift = 0;                                                // Shift Bit Ignored
            pfd.cAccumBits = 0;                                                 // No Accumulation Buffer
            pfd.cAccumRedBits = 0;                                              // Accumulation Bits Ignored
            pfd.cAccumGreenBits = 0;
            pfd.cAccumBlueBits = 0;
            pfd.cAccumAlphaBits = 0;
            pfd.cDepthBits = 16;                                                // 16Bit Z-Buffer (Depth Buffer)
            pfd.cStencilBits = 0;                                               // No Stencil Buffer
            pfd.cAuxBuffers = 0;                                                // No Auxiliary Buffer
            pfd.iLayerType = (byte)Gdi.PFD_MAIN_PLANE;                         // Main Drawing Layer
            pfd.bReserved = 0;                                                  // Reserved
            pfd.dwLayerMask = 0;                                                // Layer Masks Ignored
            pfd.dwVisibleMask = 0;
            pfd.dwDamageMask = 0;

            hDC = User.GetDC(Handle);                                      // Attempt To Get A Device Context
            if (hDC == IntPtr.Zero)
            {                                            // Did We Get A Device Context?
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Can't Create A GL Device Context.", "ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            pixelFormat = Gdi.ChoosePixelFormat(hDC, ref pfd);                  // Attempt To Find An Appropriate Pixel Format
            if (pixelFormat == 0)
            {                                              // Did Windows Find A Matching Pixel Format?
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Can't Find A Suitable PixelFormat.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!Gdi.SetPixelFormat(hDC, pixelFormat, ref pfd))
            {
                // Are We Able To Set The Pixel Format?
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Can't Set The PixelFormat.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            hRC = Wgl.wglCreateContext(hDC);                                    // Attempt To Get The Rendering Context
            if (hRC == IntPtr.Zero)
            {                                            // Are We Able To Get A Rendering Context?
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Can't Create A GL Rendering Context.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!Wgl.wglMakeCurrent(hDC, hRC))
            {
                // Try To Activate The Rendering Context
                KillGLWindow();                                                 // Reset The Display
                MessageBox.Show("Can't Activate The GL Rendering Context.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Show();                                                        // Show The Window
            TopMost = true;                                                // Topmost Window
            Focus();                                                       // Focus The Window

            if (fullscreen)
            {                                                    // This Shouldn't Be Necessary, But Is
                Cursor.Hide();
            }

            return true;                                                        // Success
        }

        internal void KillGLWindow()
        {
            if (fullscreen)
            {                                                    // Are We In Fullscreen Mode?
                User.ChangeDisplaySettings(IntPtr.Zero, 0);                     // If So, Switch Back To The Desktop
                Cursor.Show();                                                  // Show Mouse Pointer
            }

            if (hRC != IntPtr.Zero)
            {                                            // Do We Have A Rendering Context?
                if (!Wgl.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero))
                {             // Are We Able To Release The DC and RC Contexts?
                    MessageBox.Show("Release Of DC And RC Failed.", "SHUTDOWN ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (!Wgl.wglDeleteContext(hRC))
                {                                // Are We Able To Delete The RC?
                    MessageBox.Show("Release Rendering Context Failed.", "SHUTDOWN ERROR",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                hRC = IntPtr.Zero;                                              // Set RC To Null
            }

            if (hDC != IntPtr.Zero)
            {                                            // Do We Have A Device Context?
                if (!IsDisposed)
                {                          // Do We Have A Window?
                    if (Handle != IntPtr.Zero)
                    {                            // Do We Have A Window Handle?
                        if (!User.ReleaseDC(Handle, hDC))
                        {                 // Are We Able To Release The DC?
                            MessageBox.Show("Release Device Context Failed.", "SHUTDOWN ERROR",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }

                hDC = IntPtr.Zero;                                              // Set DC To Null
            }

            Hide();                                                    // Hide The Window
            Close();                                                   // Close The Form
        }
#endif
        protected override void OnKeyUp(System.Windows.Forms.KeyEventArgs windowsKeyEvent)
        {
            AGG.UI.KeyEventArgs aggKeyEvent = new AGG.UI.KeyEventArgs((AGG.UI.Keys)windowsKeyEvent.KeyData);
            m_app.OnKeyUp(aggKeyEvent);

            windowsKeyEvent.Handled = aggKeyEvent.Handled;
            windowsKeyEvent.SuppressKeyPress = aggKeyEvent.SuppressKeyPress;

            base.OnKeyUp(windowsKeyEvent);
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs windowsKeyEvent)
        {
            AGG.UI.KeyEventArgs aggKeyEvent = new AGG.UI.KeyEventArgs((AGG.UI.Keys)windowsKeyEvent.KeyData);
            m_app.OnKeyDown(aggKeyEvent);

            if (aggKeyEvent.Handled)
            {
                m_app.OnControlChanged();
                m_app.force_redraw();
            }

            windowsKeyEvent.Handled = aggKeyEvent.Handled;
            windowsKeyEvent.SuppressKeyPress = aggKeyEvent.SuppressKeyPress;

            base.OnKeyDown(windowsKeyEvent);
        }

        protected override void OnKeyPress(System.Windows.Forms.KeyPressEventArgs windowsKeyPressEvent)
        {
            AGG.UI.KeyPressEventArgs aggKeyPressEvent = new AGG.UI.KeyPressEventArgs(windowsKeyPressEvent.KeyChar);
            m_app.OnKeyPress(aggKeyPressEvent);
            windowsKeyPressEvent.Handled = aggKeyPressEvent.Handled;

            base.OnKeyPress(windowsKeyPressEvent);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_WindowContentNeedsRedraw)
            {
                m_app.OnDraw();
                m_WindowContentNeedsRedraw = false;
            }

            display_pmap(e.Graphics, m_app.rbuf_window());
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs mouseEvent)
        {
            base.OnMouseDown(mouseEvent);

            int Y = mouseEvent.Y;
            if (m_app.flip_y())
            {
                Y = (int)m_app.rbuf_window().Height - Y;
            }
            AGG.UI.MouseEventArgs aggMouseEvent = new AGG.UI.MouseEventArgs((AGG.UI.MouseButtons)mouseEvent.Button, mouseEvent.Clicks, mouseEvent.X, Y, mouseEvent.Delta);

            m_app.SetChildCurrent(M.New<T>(aggMouseEvent.X), M.New<T>(aggMouseEvent.Y));
            m_app.OnMouseDown(aggMouseEvent);
            if (aggMouseEvent.Handled)
            {
                m_app.OnControlChanged();
                m_app.force_redraw();
            }
            else
            {
                if (m_app.InRect(M.New<T>(aggMouseEvent.X), M.New<T>(aggMouseEvent.Y)))
                {
                    if (m_app.SetChildCurrent(M.New<T>(aggMouseEvent.X), M.New<T>(aggMouseEvent.Y)))
                    {
                        m_app.OnControlChanged();
                        m_app.force_redraw();
                    }
                }
                else
                {
                    m_app.OnMouseDown(aggMouseEvent);
                }
            }
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs mouseEvent)
        {
            base.OnMouseMove(mouseEvent);

            int Y = mouseEvent.Y;
            if (m_app.flip_y())
            {
                Y = (int)m_app.rbuf_window().Height - Y;
            }
            AGG.UI.MouseEventArgs aggMouseEvent = new AGG.UI.MouseEventArgs((AGG.UI.MouseButtons)mouseEvent.Button, mouseEvent.Clicks, mouseEvent.X, Y, mouseEvent.Delta);

            m_app.OnMouseMove(aggMouseEvent);
            if (aggMouseEvent.Handled)
            {
                m_app.OnControlChanged();
                m_app.force_redraw();
            }
            else
            {
                if (!m_app.InRect(M.New<T>(aggMouseEvent.X), M.New<T>(aggMouseEvent.Y)))
                {
                    m_app.OnMouseMove(aggMouseEvent);
                }
            }
        }

        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs mouseEvent)
        {
            base.OnMouseUp(mouseEvent);

            int Y = mouseEvent.Y;
            if (m_app.flip_y())
            {
                Y = (int)m_app.rbuf_window().Height - Y;
            }
            AGG.UI.MouseEventArgs aggMouseEvent = new AGG.UI.MouseEventArgs((AGG.UI.MouseButtons)mouseEvent.Button, mouseEvent.Clicks, mouseEvent.X, Y, mouseEvent.Delta);

            m_app.OnMouseUp(aggMouseEvent);
            if (aggMouseEvent.Handled)
            {
                m_app.OnControlChanged();
                m_app.force_redraw();
            }
            m_app.OnMouseUp(aggMouseEvent);
        }

        public unsafe void create_pmap(uint width, uint height, RasterBuffer wnd)
        {
            m_pmap_window = new pixel_map();
            m_pmap_window.create(width, height, PlatformSupportAbstract<T>.GetBitDepthForPixelFormat(m_format));
            wnd.attach(m_pmap_window.buf(),
                m_pmap_window.width(),
                m_pmap_window.height(),
                m_RenderOrigin == ERenderOrigin.OriginBottomLeft ? -m_pmap_window.stride() : m_pmap_window.stride(),
                m_pmap_window.bpp());
        }

        static void convert_pmap(RasterBuffer dst,
                                 RasterBuffer src,
                                 PlatformSupportAbstract<T>.PixelFormats format)
        {
            /*
            switch(format)
            {
            case pix_format_gray8:
                break;

            case pix_format_gray16:
                color_conv(dst, src, color_conv_gray16_to_gray8());
                break;

            case pix_format_rgb565:
                color_conv(dst, src, color_conv_rgb565_to_rgb555());
                break;

            case pix_format_rgbAAA:
                color_conv(dst, src, color_conv_rgbAAA_to_bgr24());
                break;

            case pix_format_bgrAAA:
                color_conv(dst, src, color_conv_bgrAAA_to_bgr24());
                break;

            case pix_format_rgbBBA:
                color_conv(dst, src, color_conv_rgbBBA_to_bgr24());
                break;

            case pix_format_bgrABB:
                color_conv(dst, src, color_conv_bgrABB_to_bgr24());
                break;

            case pix_format_rgb24:
                color_conv(dst, src, color_conv_rgb24_to_bgr24());
                break;

            case pix_format_rgb48:
                color_conv(dst, src, color_conv_rgb48_to_bgr24());
                break;

            case pix_format_bgr48:
                color_conv(dst, src, color_conv_bgr48_to_bgr24());
                break;

            case pix_format_abgr32:
                color_conv(dst, src, color_conv_abgr32_to_bgra32());
                break;

            case pix_format_argb32:
                color_conv(dst, src, color_conv_argb32_to_bgra32());
                break;

            case pix_format_rgba32:
                color_conv(dst, src, color_conv_rgba32_to_bgra32());
                break;

            case pix_format_bgra64:
                color_conv(dst, src, color_conv_bgra64_to_bgra32());
                break;

            case pix_format_abgr64:
                color_conv(dst, src, color_conv_abgr64_to_bgra32());
                break;

            case pix_format_argb64:
                color_conv(dst, src, color_conv_argb64_to_bgra32());
                break;

            case pix_format_rgba64:
                color_conv(dst, src, color_conv_rgba64_to_bgra32());
                break;
            }
             */
        }

        public void display_pmap(Graphics displayGraphics, RasterBuffer src)
        {
            if (m_sys_format == m_format)
            {
#if USE_OPENGL
                if (hDC != IntPtr.Zero)
                {
                    Gdi.SwapBuffers(hDC);
                }
                else
#endif
                {
                    m_pmap_window.draw(displayGraphics);
                }
            }
            else
            {
                pixel_map pmap_tmp = new pixel_map();
                pmap_tmp.create(m_pmap_window.width(),
                    m_pmap_window.height(), PlatformSupportAbstract<T>.GetBitDepthForPixelFormat(m_sys_format));

                RasterBuffer rbuf_tmp = new RasterBuffer();
                unsafe
                {
                    rbuf_tmp.attach(pmap_tmp.buf(),
                        pmap_tmp.width(),
                        pmap_tmp.height(),
                        m_app.flip_y() ?
                        pmap_tmp.stride() :
                        -pmap_tmp.stride(), pmap_tmp.bpp());
                }

                convert_pmap(rbuf_tmp, src, m_format);
                pmap_tmp.draw(displayGraphics);

                throw new System.NotImplementedException();
            }
        }

        public bool load_pmap(string fn, uint idx, RasterBuffer dst)
        {
            pixel_map pmap_tmp = new pixel_map();
            if (!pmap_tmp.load_from_bmp(fn)) return false;

            RasterBuffer rbuf_tmp = new RasterBuffer();
            unsafe
            {
                rbuf_tmp.attach(pmap_tmp.buf(),
                                pmap_tmp.width(),
                                pmap_tmp.height(),
                                m_app.flip_y() ?
                                  pmap_tmp.stride() :
                                 -pmap_tmp.stride(),
                                 pmap_tmp.bpp());

                m_pmap_img[idx] = new pixel_map();
                m_pmap_img[idx].create(pmap_tmp.width(), pmap_tmp.height(),
                    PlatformSupportAbstract<T>.GetBitDepthForPixelFormat(m_format),
                    0);

                dst.attach(m_pmap_img[idx].buf(),
                    m_pmap_img[idx].width(),
                    m_pmap_img[idx].height(),
                    m_app.flip_y() ?
                    m_pmap_img[idx].stride() :
                    -m_pmap_img[idx].stride(), m_pmap_img[idx].bpp());

                switch (m_format)
                {
                    //                 case platform_support_abstract.pix_format_e.pix_format_gray8:
                    //                 switch(pmap_tmp.bpp())
                    //                 {
                    //                     //case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_gray8()); break;
                    //                     //case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_gray8()); break;
                    //                     //case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_gray8()); break;
                    //                 }
                    //                     break;
                    // 
                    //                 case platform_support_abstract.pix_format_e.pix_format_gray16:
                    //                 switch(pmap_tmp.bpp())
                    //                 {
                    //                 //case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_gray16()); break;
                    //                 //case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_gray16()); break;
                    //                 //case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_gray16()); break;
                    //                 }
                    //                 break;
                    // 
                    //             case platform_support_abstract.pix_format_e.pix_format_rgb555:
                    //                 switch(pmap_tmp.bpp())
                    //                 {
                    // //                 case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_rgb555()); break;
                    // //                 case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_rgb555()); break;
                    // //                 case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_rgb555()); break;
                    //                 }
                    //                 break;
                    // 
                    //             case platform_support_abstract.pix_format_e.pix_format_rgb565:
                    //                 switch(pmap_tmp.bpp())
                    //                 {
                    // //                 case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_rgb565()); break;
                    // //                 case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_rgb565()); break;
                    // //                 case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_rgb565()); break;
                    //                 }
                    //                 break;
                    // 
                    case PlatformSupportAbstract<T>.PixelFormats.pix_format_bgr24:
                        switch (pmap_tmp.bpp())
                        {
                            case 24:
                                unsafe
                                {
                                    for (uint y = 0; y < rbuf_tmp.Height; y++)
                                    {
                                        byte* sourceBuffer = rbuf_tmp.GetPixelPointer((int)rbuf_tmp.Height - (int)y - 1);
                                        byte* destBuffer = dst.GetPixelPointer((int)y);
                                        for (uint x = 0; x < rbuf_tmp.Width; x++)
                                        {
                                            *destBuffer++ = sourceBuffer[0];
                                            *destBuffer++ = sourceBuffer[1];
                                            *destBuffer++ = sourceBuffer[2];
                                            sourceBuffer += 3;
                                        }
                                    }
                                }
                                break;

                            default:
                                throw new System.NotImplementedException();
                        }
                        break;

                    case PlatformSupportAbstract<T>.PixelFormats.pix_format_rgb24:
                        switch (pmap_tmp.bpp())
                        {
                            //                 case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_rgb24()); break;
                            case 24:
                                //color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_rgba32()); 
                                unsafe
                                {
                                    for (uint y = 0; y < rbuf_tmp.Height; y++)
                                    {
                                        byte* sourceBuffer = rbuf_tmp.GetPixelPointer((int)rbuf_tmp.Height - (int)y - 1);
                                        byte* destBuffer = dst.GetPixelPointer((int)y);
                                        for (uint x = 0; x < rbuf_tmp.Width; x++)
                                        {
                                            *destBuffer++ = sourceBuffer[2];
                                            *destBuffer++ = sourceBuffer[1];
                                            *destBuffer++ = sourceBuffer[0];
                                            sourceBuffer += 3;
                                        }
                                    }
                                }
                                break;
                            //                 case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_rgb24()); break;
                            default:
                                throw new System.NotImplementedException();
                        }
                        break;
                    // 
                    //             case platform_support_abstract.pix_format_e.pix_format_bgr24:
                    //                 switch(pmap_tmp.bpp())
                    //                 {
                    // //                 case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_bgr24()); break;
                    // //                 case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_bgr24()); break;
                    // //                 case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_bgr24()); break;
                    //                 }
                    //                 break;
                    // 
                    //             case platform_support_abstract.pix_format_e.pix_format_rgb48:
                    //                 switch(pmap_tmp.bpp())
                    //                 {
                    //                 //case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_rgb48()); break;
                    // //                 case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_rgb48()); break;
                    //                 //case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_rgb48()); break;
                    //                 }
                    //                 break;
                    // 
                    //             case platform_support_abstract.pix_format_e.pix_format_bgr48:
                    //                 switch(pmap_tmp.bpp())
                    //                 {
                    //                 //case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_bgr48()); break;
                    // //                 case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_bgr48()); break;
                    //                 //case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_bgr48()); break;
                    //                 }
                    //                 break;
                    // 
                    //             case platform_support_abstract.pix_format_e.pix_format_abgr32:
                    //                 switch(pmap_tmp.bpp())
                    //                 {
                    // //                 case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_abgr32()); break;
                    // //                 case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_abgr32()); break;
                    // //                 case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_abgr32()); break;
                    //                 }
                    //                 break;
                    // 
                    //             case platform_support_abstract.pix_format_e.pix_format_argb32:
                    //                 switch(pmap_tmp.bpp())
                    //                 {
                    // //                 case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_argb32()); break;
                    // //                 case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_argb32()); break;
                    // //                 case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_argb32()); break;
                    //                 }
                    //                 break;
                    // 
                    //             case platform_support_abstract.pix_format_e.pix_format_bgra32:
                    //                 switch(pmap_tmp.bpp())
                    //                 {
                    // //                 case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_bgra32()); break;
                    // //                 case 24: color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_bgra32()); break;
                    // //                 case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_bgra32()); break;
                    //                 }
                    //                 break;

                    case PlatformSupportAbstract<T>.PixelFormats.pix_format_bgra32:
                        switch (pmap_tmp.bpp())
                        {
                            case 24:
                                unsafe
                                {
                                    for (uint y = 0; y < rbuf_tmp.Height; y++)
                                    {
                                        byte* sourceBuffer = rbuf_tmp.GetPixelPointer((int)rbuf_tmp.Height - (int)y - 1);
                                        byte* destBuffer = dst.GetPixelPointer((int)y);
                                        for (uint x = 0; x < rbuf_tmp.Width; x++)
                                        {
                                            *destBuffer++ = sourceBuffer[0];
                                            *destBuffer++ = sourceBuffer[1];
                                            *destBuffer++ = sourceBuffer[2];
                                            *destBuffer++ = 255;
                                            sourceBuffer += 3;
                                        }
                                    }
                                }
                                break;
                        }
                        break;

                    case PlatformSupportAbstract<T>.PixelFormats.pix_format_rgba32:
                        switch (pmap_tmp.bpp())
                        {
                            //                 case 16: color_conv(dst, &rbuf_tmp, color_conv_rgb555_to_rgba32()); break;
                            case 24:
                                //color_conv(dst, &rbuf_tmp, color_conv_bgr24_to_rgba32()); 
                                unsafe
                                {
                                    for (uint y = 0; y < rbuf_tmp.Height; y++)
                                    {
                                        byte* sourceBuffer = rbuf_tmp.GetPixelPointer((int)rbuf_tmp.Height - (int)y - 1);
                                        byte* destBuffer = dst.GetPixelPointer((int)y);
                                        for (uint x = 0; x < rbuf_tmp.Width; x++)
                                        {
                                            *destBuffer++ = sourceBuffer[2];
                                            *destBuffer++ = sourceBuffer[1];
                                            *destBuffer++ = sourceBuffer[0];
                                            *destBuffer++ = 255;
                                            sourceBuffer += 3;
                                        }
                                    }
                                }
                                break;


                            //                 case 32: color_conv(dst, &rbuf_tmp, color_conv_bgra32_to_rgba32()); break;
                            default:
                                throw new System.NotImplementedException();
                        }
                        break;

                    default:
                        throw new System.NotImplementedException();

                }
            }

            return true;
        }

        public bool save_pmap(string fn, uint idx, RasterBuffer src)
        {
            throw new NotImplementedException();
        }


        public uint translate(uint keycode)
        {
            return m_last_translated_key = (keycode > 255) ? 0 : m_keymap[keycode];
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // we handle this so that we control the background
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.Visible)
            {
                if (m_app.rbuf_window() != null)
                {
                    create_pmap((uint)ClientSize.Width, (uint)ClientSize.Height, m_app.rbuf_window());
                }

                m_app.trans_affine_resizing(ClientSize.Width, ClientSize.Height);
                m_app.OnResize(ClientSize.Width, ClientSize.Height);
                m_app.force_redraw();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Call on closing and check if we can close (a "do you want to save" might cancel the close. :).
            bool CancelClose;
            m_app.OnClosing(out CancelClose);

            if (CancelClose)
            {
                e.Cancel = true;
            }

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            m_app.OnClosed();

            base.OnClosed(e);
        }

        internal void force_redraw()
        {
            m_WindowContentNeedsRedraw = true;
            Invalidate();
        }
    };

    public class PlatformSupport<T> : PlatformSupportAbstract<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private RendererBase<T> m_ScreenRenderer;
        private PlatformSpecificWindow<T> m_specific;

        //------------------------------------------------------------------------
        public PlatformSupport(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(format, RenderOrigin)
        {
            m_specific = new PlatformSpecificWindow<T>(this, format, RenderOrigin);
            m_format = format;
            //m_bpp(m_specific->m_bpp),
            //m_window_flags(0),
            m_wait_mode = true;
            m_RenderOrigin = RenderOrigin;
            m_initial_width = 10;
            m_initial_height = 10;

            Caption = "Anti-Grain Geometry Application";

            Focus();
        }

        //------------------------------------------------------------------------
        public override string Caption
        {
            get
            {
                return m_specific.Text;
            }
            set
            {
                m_specific.Text = value;
            }
        }

        public override void OnIdle() { }
        public override void OnResize(int sx, int sy)
        {
            Bounds = new RectDouble<T>(M.Zero<T>(), M.Zero<T>(), M.New<T>(sx), M.New<T>(sy)); ;
            if (m_ScreenRenderer != null)
            {
                m_ScreenRenderer.Rasterizer.SetVectorClipBox(M.Zero<T>(), M.Zero<T>(), Width, Height);
                ((FormatClippingProxy)m_ScreenRenderer.PixelFormat).SetClippingBox(0, 0, Width.ToInt(), Height.ToInt());
            }

#if USE_OPENGL
            if ((m_window_flags & (int)WindowFlags.UseOpenGL) != 0)
            {
                Gl.glViewport(0, 0, Width.ToInt(), Height.ToInt());					// Reset The Current Viewport


                // The following lines set the screen up for a perspective view. Meaning things in the distance get smaller. 
                // This creates a realistic looking scene. 
                // The perspective is calculated with a 45 degree viewing angle based on the windows width and height. 
                // The 0.1f, 100.0f is the starting point and ending point for how deep we can draw into the screen.

                Gl.glMatrixMode(Gl.GL_PROJECTION);						// Select The Projection Matrix
                Gl.glLoadIdentity();							// Reset The Projection Matrix-

                Gl.glMatrixMode(Gl.GL_MODELVIEW);						// Select The Modelview Matrix
                Gl.glLoadIdentity();
                if (m_ScreenRenderer != null)
                {
                    m_ScreenRenderer.Clear(new RGBA_Doubles(1, 1, 1, 1));
                }
            }
#endif
        }

        public override RendererBase<T> GetRenderer()
        {
            return m_ScreenRenderer;
        }
#if false
        public override void OnDraw() {}
        public override bool OnMouseDown(MouseEventArgs mouseEvent) { return base.OnMouseDown(mouseEvent); }
        public override bool OnMouseMove(MouseEventArgs mouseEvent) { return base.OnMouseMove(mouseEvent); }
        public override bool OnMouseUp(MouseEventArgs mouseEvent) { return base.OnMouseUp(mouseEvent); }
        public override void OnKeyDown(AGG.UI.KeyEventArgs keyEvent) { }
        public override void OnKeyUp(AGG.UI.KeyEventArgs keyEvent) { }
#endif
        public override void OnControlChanged() { }

        public override void Invalidate()
        {
            m_specific.Invalidate();
            m_specific.m_WindowContentNeedsRedraw = true;
        }

        public override void Invalidate(RectDouble<T> rectToInvalidate)
        {
            m_specific.Invalidate(new Rectangle(
                (int)rectToInvalidate.Left.Floor().ToInt(),
                (int)Height.Subtract(rectToInvalidate.Top).Floor().ToInt(),
                (int)rectToInvalidate.Width.Ceiling().ToInt(),
                (int)rectToInvalidate.Height.Ceiling().ToInt()));
            m_specific.m_WindowContentNeedsRedraw = true;
        }

        public override void OnClosing(out bool CancelClose)
        {
            CancelClose = false;
            // your app can implement this is you want to be able to override the close.
        }

        public override void Close()
        {
            m_specific.Close();
        }

        //------------------------------------------------------------------------
        public override void force_redraw()
        {
            m_specific.force_redraw();
        }

        //------------------------------------------------------------------------
        public override void update_window()
        {
            if (!m_specific.IsDisposed)
            {
                Graphics displayGraphics = m_specific.CreateGraphics();
                m_specific.display_pmap(displayGraphics, m_rbuf_window);
            }
        }

        //------------------------------------------------------------------------
        public override bool init(uint width, uint height, uint flags)
        {
            if (m_specific.m_sys_format == PlatformSupportAbstract<T>.PixelFormats.pix_format_undefined)
            {
                return false;
            }

            m_window_flags = flags;

            System.Drawing.Size clientSize = new System.Drawing.Size();
            clientSize.Width = (int)width;
            clientSize.Height = (int)height;
            m_specific.ClientSize = clientSize;

            if ((m_window_flags & (uint)WindowFlags.Risizeable) == 0)
            {
                m_specific.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
                m_specific.MaximizeBox = false;
            }

            m_rbuf_window = new RasterBuffer();
            m_specific.create_pmap(width, height, m_rbuf_window);
            m_initial_width = width;
            m_initial_height = height;

#if USE_OPENGL
            if ((m_window_flags & (int)WindowFlags.UseOpenGL) != 0)
            {
                m_specific.CreateGLWindow((int)m_initial_width, (int)m_initial_height, 32, false);
            }
            else
#endif
            {
                m_specific.Show();
            }

            Bounds = new RectDouble<T>(M.Zero<T>(), M.Zero<T>(), M.New<T>(width), M.New<T>(height));

            OnInitialize();
            m_specific.m_WindowContentNeedsRedraw = true;

            return true;
        }

        public override void OnInitialize()
        {
            IPixelFormat screenPixelFormat;
            FormatClippingProxy screenClippingProxy;

            RasterizerScanlineAA<T> rasterizer = new RasterizerScanlineAA<T>();
            ScanlinePacked8 scanlinePacked = new ScanlinePacked8();

            if (rbuf_window().BitsPerPixel == 24)
            {
                screenPixelFormat = new FormatRGB(rbuf_window(), new BlenderBGR());
            }
            else
            {
                screenPixelFormat = new FormatRGBA(rbuf_window(), new BlenderBGRA());
            }
            screenClippingProxy = new FormatClippingProxy(screenPixelFormat);

#if USE_OPENGL
            if ((m_window_flags & (int)WindowFlags.UseOpenGL) != 0)
            {
                m_ScreenRenderer = new RendererOpenGL<T>(screenClippingProxy, rasterizer);
                m_ScreenRenderer.Clear(new RGBA_Doubles(1, 1, 1, 1));
            }
            else
#endif
            {
                m_ScreenRenderer = new Renderer<T>(screenClippingProxy, rasterizer, scanlinePacked);
            }

            m_ScreenRenderer.Rasterizer.SetVectorClipBox(M.Zero<T>(), M.Zero<T>(), Width, Height);

            base.OnInitialize();
        }

        //------------------------------------------------------------------------
        public override void run()
        {
            while (m_specific.Created)
            {
                Application.DoEvents();
                if (m_wait_mode)
                {
                    System.Threading.Thread.Sleep(1);
                }
                else
                {
                    OnIdle();
                }
            }

#if USE_OPENGL
            if ((m_window_flags & (int)WindowFlags.UseOpenGL) != 0)
            {
                m_specific.KillGLWindow();
            }
#endif
        }

        //------------------------------------------------------------------------
        public override bool load_img(uint idx, string file)
        {
            if (idx < (uint)max_images_e.max_images)
            {
                int len = file.Length;
                if (len < 4 || !file.EndsWith(".BMP"))
                {
                    file += ".bmp";
                }

                RasterBuffer temp = new RasterBuffer();
                bool goodLoad = m_specific.load_pmap(file, idx, temp);
                m_rbuf_img.Add(temp);
                return goodLoad;
            }
            return true;
        }

        //------------------------------------------------------------------------
        public override string img_ext() { return ".bmp"; }

        //------------------------------------------------------------------------
        public void start_timer()
        {
            m_specific.m_StopWatch.Reset();
            m_specific.m_StopWatch.Start();
        }

        //------------------------------------------------------------------------
        public double elapsed_time()
        {
            m_specific.m_StopWatch.Stop();
            return m_specific.m_StopWatch.Elapsed.TotalMilliseconds;
        }

        public override bool create_img(uint idx, uint width, uint height, uint BitsPerPixel)
        {
            if (idx < (uint)max_images_e.max_images)
            {
                if (width == 0) width = m_specific.m_pmap_window.width();
                if (height == 0) height = m_specific.m_pmap_window.height();
                if (m_specific.m_pmap_img[idx] == null)
                {
                    m_specific.m_pmap_img[idx] = new pixel_map();
                    m_rbuf_img.Add(new RasterBuffer());
                }
                m_specific.m_pmap_img[idx].create(width, height, BitsPerPixel);
                unsafe
                {
                    m_rbuf_img[(int)idx].attach(m_specific.m_pmap_img[idx].buf(),
                                           m_specific.m_pmap_img[idx].width(),
                                           m_specific.m_pmap_img[idx].height(),
                                            m_specific.m_pmap_img[idx].stride(),
                                            m_specific.m_pmap_img[idx].bpp());
                }
                return true;
            }
            return false;
        }

        public override void message(String msg)
        {
            System.Windows.Forms.MessageBox.Show(msg, "AggSharp message");
        }
    };
}
