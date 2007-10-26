using System;
using System.Runtime.InteropServices;

namespace SharpMap.Tests.Presentation
{
    public static class ScreenHelper
    {
        public static readonly Double Dpi;

        static ScreenHelper()
        {
            IntPtr dc = IntPtr.Zero;

            try
            {
                dc = CreateDC("DISPLAY", null, null, IntPtr.Zero);

                Int32 dpi = GetDeviceCaps(dc, LOGPIXELSX);

                if (dpi == 0)
                {
                    // Can't query the DPI - just guess the most likely
                    Dpi = 96;
                }
                else
                {
                    Dpi = dpi;
                }
            }
            finally
            {
                if (dc != IntPtr.Zero)
                {
                    DeleteDC(dc);
                }
            }
        }

        [DllImport("Gdi32")]
        private static extern IntPtr CreateDC(String driver, String device, String output, IntPtr initData);

        [DllImport("Gdi32")]
        private static extern void DeleteDC(IntPtr dc);

        [DllImport("Gdi32")]
        private static extern Int32 GetDeviceCaps(IntPtr hdc, Int32 index);

        private const Int32 LOGPIXELSX = 88;
        private const Int32 LOGPIXELSY = 90;
    }
}