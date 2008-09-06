using System;
using AGG.PixelFormat;
using AGG.Rasterizer;
using AGG.Scanline;
using AGG.UI;
using NPack.Interfaces;

namespace Reflexive.Editor
{
    public class EditorWindow<T> : AGG.UI.win32.PlatformSupport<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public EditorWindow()
            : base(PixelFormats.pix_format_bgr24, ERenderOrigin.OriginBottomLeft)
        {
        }

        public override bool init(uint width, uint height, uint flags)
        {
            bool good = base.init(width, height, flags);

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

            return good;
        }

        public override void OnDraw()
        {
            base.OnDraw();
        }
    }
}
