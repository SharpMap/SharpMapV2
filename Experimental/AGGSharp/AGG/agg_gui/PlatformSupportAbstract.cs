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
// It's not a part of the AGG library, it's just a helper class to create 
// interactive demo examples. Since the examples should not be too complex
// this class is provided to support some very basic interactive graphical
// funtionality, such as putting the rendered image to the window, simple 
// keyboard and mouse input, window resizing, setting the window title,
// and catching the "idle" events.
// 
// The idea is to have a single header file that does not depend on any 
// platform (I hate these endless #ifdef/#elif/#elif.../#endif) and a number
// of different implementations depending on the concrete platform. 
// The most popular platforms are:
//
// Windows-32 API
// X-Window API
// SDL library (see http://www.libsdl.org/)
// MacOS C/C++ API
// 
// This file does not include any system dependent .h files such as
// windows.h or X11.h, so, your demo applications do not depend on the
// platform. The only file that can #include system dependend headers
// is the implementation file agg_platform_support.cpp. Different
// implementations are placed in different directories, such as
// ~/agg/src/platform/win32
// ~/agg/src/platform/sdl
// ~/agg/src/platform/X11
// and so on.
//
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using AGG.Buffer;
using AGG.Transform;
using NPack.Interfaces;
using NPack;

namespace AGG.UI
{
    public enum ERenderOrigin { OriginTopLeft, OriginBottomLeft };

    abstract public class PlatformSupportAbstract<T> : GUIWidget<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public enum max_images_e { max_images = 16 };


        //----------------------------------------------------------window_flag_e
        // These are flags used in method init(). Not all of them are
        // applicable on different platforms, for example the win32_api
        // cannot use a hardware buffer (window_hw_buffer).
        // The implementation should simply ignore unsupported flags.
        public enum WindowFlags
        {
            None = 0,
            Risizeable = 1,
            KeepAspectRatio = 4,
            UseOpenGL = 16,
        };

        //-----------------------------------------------------------pix_format_e
        // Possible formats of the rendering buffer. Initially I thought that it's
        // reasonable to create the buffer and the rendering functions in 
        // accordance with the native pixel format of the system because it 
        // would have no overhead for pixel format conversion. 
        // But eventually I came to a conclusion that having a possibility to 
        // convert pixel formats on demand is a good idea. First, it was X11 where 
        // there lots of different formats and visuals and it would be great to 
        // render everything in, say, RGB-24 and display it automatically without
        // any additional efforts. The second reason is to have a possibility to 
        // debug renderers for different pixel formats and colorspaces having only 
        // one computer and one system.
        //
        // This stuff is not included into the basic AGG functionality because the 
        // number of supported pixel formats (and/or colorspaces) can be great and 
        // if one needs to add new format it would be good only to add new 
        // rendering files without having to modify any existing ones (a general 
        // principle of incapsulation and isolation).
        //
        // Using a particular pixel format doesn't obligatory mean the necessity
        // of software conversion. For example, win32 API can natively display 
        // gray8, 15-bit RGB, 24-bit BGR, and 32-bit BGRA formats. 
        // This list can be (and will be!) extended in future.
        public enum PixelFormats
        {
            pix_format_undefined = 0,  // By default. No conversions are applied 
            pix_format_bw,             // 1 bit per color B/W
            pix_format_gray8,          // Simple 256 level grayscale
            pix_format_gray16,         // Simple 65535 level grayscale
            pix_format_rgb555,         // 15 bit rgb. Depends on the byte ordering!
            pix_format_rgb565,         // 16 bit rgb. Depends on the byte ordering!
            pix_format_rgbAAA,         // 30 bit rgb. Depends on the byte ordering!
            pix_format_rgbBBA,         // 32 bit rgb. Depends on the byte ordering!
            pix_format_bgrAAA,         // 30 bit bgr. Depends on the byte ordering!
            pix_format_bgrABB,         // 32 bit bgr. Depends on the byte ordering!
            pix_format_rgb24,          // R-G-B, one byte per color component
            pix_format_bgr24,          // B-G-R, native win32 BMP format.
            pix_format_rgba32,         // R-G-B-A, one byte per color component
            pix_format_argb32,         // A-R-G-B, native MAC format
            pix_format_abgr32,         // A-B-G-R, one byte per color component
            pix_format_bgra32,         // B-G-R-A, native win32 BMP format
            pix_format_rgb48,          // R-G-B, 16 bits per color component
            pix_format_bgr48,          // B-G-R, native win32 BMP format.
            pix_format_rgba64,         // R-G-B-A, 16 bits byte per color component
            pix_format_argb64,         // A-R-G-B, native MAC format
            pix_format_abgr64,         // A-B-G-R, one byte per color component
            pix_format_bgra64,         // B-G-R-A, native win32 BMP format

            end_of_pix_formats
        };

        public static uint GetBitDepthForPixelFormat(PixelFormats pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormats.pix_format_bgr24:
                case PixelFormats.pix_format_rgb24:
                    return 24;

                case PixelFormats.pix_format_bgra32:
                case PixelFormats.pix_format_rgba32:
                    return 32;

                default:
                    throw new System.NotImplementedException();
            }
        }

        // format - see enum pix_format_e {};
        // flip_y - true if you want to have the Y-axis flipped vertically.
        public PlatformSupportAbstract(PixelFormats format, ERenderOrigin RenderOrigin)
        {
            m_format = format;
            m_bpp = GetBitDepthForPixelFormat(format);
            m_RenderOrigin = RenderOrigin;
            m_initial_width = 10;
            m_initial_height = 10;
            m_resize_mtx = MatrixFactory<T>.NewIdentity(VectorDimension.Two);
            m_rbuf_img = new List<RasterBuffer>();
        }

        //~platform_support();

        // Setting the windows caption (title). Should be able
        // to be called at least before calling init(). 
        // It's perfect if they can be called anytime.
        public abstract string Caption
        {
            get;
            set;
        }

        // These 3 methods handle working with images. The image
        // formats are the simplest ones, such as .BMP in Windows or 
        // .ppm in Linux. In the applications the names of the files
        // should not have any file extensions. Method load_img() can
        // be called before init(), so, the application could be able 
        // to determine the initial size of the window depending on 
        // the size of the loaded image. 
        // The argument "idx" is the number of the image 0...max_images-1
        abstract public bool load_img(uint idx, String file);
        abstract public bool create_img(uint idx, uint width, uint height, uint BitsPerPixel);
        /*
        abstract public bool save_img(uint idx, String file);

        public bool create_img(uint idx)
        {
            return create_img(idx, 0, 0);
        }

         */

        public virtual void OnInitialize() { }

        // init() and run(). See description before the class for details.
        // The necessity of calling init() after creation is that it's 
        // impossible to call the overridden virtual function (OnInitialize()) 
        // from the constructor. On the other hand it's very useful to have
        // some OnInitialize() event handler when the window is created but 
        // not yet displayed. The rbuf_window() method (see below) is 
        // accessible from OnInitialize().
        abstract public bool init(uint width, uint height, uint flags);
        abstract public void run();
        // The very same parameters that were used in the constructor
        public PixelFormats format() { return m_format; }
        public ERenderOrigin RenderOrigin
        {
            get
            {
                return m_RenderOrigin;
            }
            set
            {
                m_RenderOrigin = value;
            }
        }
        public bool flip_y()
        {
            return RenderOrigin == ERenderOrigin.OriginBottomLeft;
        }
        public uint bpp() { return m_bpp; }

        // The following provides a very simple mechanism of doing someting
        // in background. It's not multithreading. When wait_mode is true
        // the class waits for the events and it does not ever call on_idle().
        // When it's false it calls on_idle() when the event queue is empty.
        // The mode can be changed anytime. This mechanism is satisfactory
        // to create very simple animations.
        public bool wait_mode() { return m_wait_mode; }
        public void wait_mode(bool wait_mode) { m_wait_mode = wait_mode; }

        // These two functions control updating of the window. 
        // force_redraw() is an analog of the Win32 InvalidateRect() function.
        // Being called it sets a flag (or sends a message) which results
        // in calling OnDraw() and updating the content of the window 
        // when the next event cycle comes.
        // update_window() results in just putting immediately the content 
        // of the currently rendered buffer to the window without calling
        // OnDraw().
        abstract public void force_redraw();
        abstract public void update_window();

        abstract public void Close();
        abstract public void OnClosing(out bool CancelClose);

        // So, finally, how to draw anything with AGG? Very simple.
        // rbuf_window() returns a reference to the main rendering 
        // buffer which can be attached to any rendering class.
        // rbuf_img() returns a reference to the previously created
        // or loaded image buffer (see load_img()). The image buffers 
        // are not displayed directly, they should be copied to or 
        // combined somehow with the rbuf_window(). rbuf_window() is
        // the only buffer that can be actually displayed.
        public RasterBuffer rbuf_window() { return m_rbuf_window; }
        public RasterBuffer rbuf_img(uint idx) { return m_rbuf_img[(int)idx]; }

        // Returns file extension used in the implementation for the particular
        // system.
        abstract public String img_ext();

        /*
        public void copy_img_to_window(uint idx)
        {
            if(idx < max_images && rbuf_img(idx).buf())
            {
                rbuf_window().copy_from(rbuf_img(idx));
            }
        }
        
        public void copy_window_to_img(uint idx)
        {
            if(idx < max_images)
            {
                create_img(idx, rbuf_window().width(), rbuf_window().height());
                rbuf_img(idx).copy_from(rbuf_window());
            }
        }
       
         */

        public void copy_img_to_img(uint idx_to, uint idx_from)
        {
            unsafe
            {
                if (idx_from < (uint)max_images_e.max_images
                    && idx_to < (int)max_images_e.max_images
                    && rbuf_img(idx_from).GetBuffer() != null)
                {
                    create_img(idx_to,
                               rbuf_img(idx_from).Width,
                               rbuf_img(idx_from).Height, rbuf_img(idx_from).BitsPerPixel);
                    rbuf_img(idx_to).CopyFrom(rbuf_img(idx_from));
                }
            }
        }

        public abstract void OnIdle();
        abstract public void OnResize(int sx, int sy);
        abstract public void OnControlChanged();

        // Auxiliary functions. trans_affine_resizing() modifier sets up the resizing 
        // matrix on the basis of the given width and height and the initial
        // width and height of the window. The implementation should simply 
        // call this function every time when it catches the resizing event
        // passing in the new values of width and height of the window.
        // Nothing prevents you from "cheating" the scaling matrix if you
        // call this function from somewhere with wrong arguments. 
        // trans_affine_resizing() accessor simply returns current resizing matrix 
        // which can be used to apply additional scaling of any of your 
        // stuff when the window is being resized.
        // width(), height(), initial_width(), and initial_height() must be
        // clear to understand with no comments :-)
        public void trans_affine_resizing(int width, int height)
        {
            if ((m_window_flags & (uint)WindowFlags.KeepAspectRatio) != 0)
            {
                double sx = (double)(width) / (double)(m_initial_width);
                double sy = (double)(height) / (double)(m_initial_height);
                if (sy < sx) sx = sy;
                m_resize_mtx = MatrixFactory<T>.NewScaling2D(M.New<T>(sx), M.New<T>(sx));
                Viewport<T> vp = new Viewport<T>();
                vp.PreserveAspectRatio(M.New<T>(0.5), M.New<T>(0.5), AspectRatio.Meet);
                vp.DeviceViewport(M.Zero<T>(), M.Zero<T>(), M.New<T>(width), M.New<T>(height));
                vp.WorldViewport(M.Zero<T>(), M.Zero<T>(), M.New<T>(m_initial_width), M.New<T>(m_initial_height));
                m_resize_mtx = vp.ToAffine();
            }
            else
            {
                m_resize_mtx = MatrixFactory<T>.NewScaling2D(
                   M.New<T>((double)(width) / (double)(m_initial_width)),
                  M.New<T>((double)(height) / (double)(m_initial_height)));
            }
        }

        public IAffineTransformMatrix<T> trans_affine_resizing() { return m_resize_mtx; }

        public T width() { return M.New<T>((double)m_rbuf_window.Width); }
        public T height() { return M.New<T>((double)m_rbuf_window.Height); }
        public T initial_width() { return M.New<T>((double)m_initial_width); }
        public T initial_height() { return M.New<T>((double)m_initial_height); }
        public uint window_flags() { return m_window_flags; }

        // Get raw display handler depending on the system. 
        // For win32 its an HDC, for other systems it can be a pointer to some
        // structure. See the implementation files for detals.
        // It's provided "as is", so, first you should check if it's not null.
        // If it's null the raw_display_handler is not supported. Also, there's 
        // no guarantee that this function is implemented, so, in some 
        // implementations you may have simply an unresolved symbol when linking.
        //public void* raw_display_handler();


        // display message box or print the message to the console 
        // (depending on implementation)
        abstract public void message(String msg);

        // Stopwatch functions. Function elapsed_time() returns time elapsed 
        // since the latest start_timer() invocation in millisecods. 
        // The resolutoin depends on the implementation. 
        // In Win32 it uses QueryPerformanceFrequency() / QueryPerformanceCounter().
        //abstract public void start_timer();
        //abstract public double elapsed_time();

        // Get the full file name. In most cases it simply returns
        // file_name. As it's appropriate in many systems if you open
        // a file by its name without specifying the path, it tries to 
        // open it in the current directory. The demos usually expect 
        // all the supplementary files to be placed in the current 
        // directory, that is usually coincides with the directory where
        // the the executable is. However, in some systems (BeOS) it's not so. 
        // For those kinds of systems full_file_name() can help access files 
        // preserving commonly used policy.
        // So, it's a good idea to use in the demos the following:
        // FILE* fd = fopen(full_file_name("some.file"), "r"); 
        // instead of
        // FILE* fd = fopen("some.file", "r"); 
        //abstract public String full_file_name(String file_name);

        // Sorry, I'm too tired to describe the private 
        // data members. See the implementations for different
        // platforms for details.
        //private platform_support(platform_support platform);
        //private platform_support operator = (platform_support platform);

        protected PixelFormats m_format;
        protected uint m_bpp;
        protected RasterBuffer m_rbuf_window;
        protected List<RasterBuffer> m_rbuf_img;
        protected uint m_window_flags;
        protected bool m_wait_mode;
        protected ERenderOrigin m_RenderOrigin;
        protected uint m_initial_width;
        protected uint m_initial_height;
        protected IAffineTransformMatrix<T> m_resize_mtx;
    };
}
