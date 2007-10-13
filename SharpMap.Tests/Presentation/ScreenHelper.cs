using System;
using System.Runtime.InteropServices;

namespace SharpMap.Tests.Presentation
{
    public static class ScreenHelper
    {
        public static readonly double Dpi;

        static ScreenHelper()
        {
            IntPtr dc = IntPtr.Zero;

            try
            {
                dc = CreateDC("DISPLAY", null, null, IntPtr.Zero);

                int dpi = GetDeviceCaps(dc, LOGPIXELSX);

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
        private static extern IntPtr CreateDC(string driver, string device, string output, IntPtr initData);

        [DllImport("Gdi32")]
        private static extern void DeleteDC(IntPtr dc);

        [DllImport("Gdi32")]
        private static extern int GetDeviceCaps(IntPtr hdc, int index);

        private const int LOGPIXELSX = 88;
        private const int LOGPIXELSY = 90;
    }
}