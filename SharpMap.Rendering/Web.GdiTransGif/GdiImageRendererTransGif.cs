/*
 *	This file is part of SharpMap.Rendering.Web.Gdi
 *  SharpMap.Rendering.Web.Gdi is free software © 2008 Newgrove Consultants Limited, 
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
 */
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SharpMap.Presentation.AspNet.MVP;
using SharpMap.Rendering.Gdi;
using SharpMap.Styles;

namespace SharpMap.Rendering.Web
{
    /// <summary>
    /// Same as GdiImageRenderer but with some extra processing to cater for producing Transparent Gifs.. 
    /// You've got to love GDI+ and IE6 :(
    /// Requires Unsafe compilation.
    /// </summary>
    public class GdiImageRendererTransGif : GdiImageRenderer
    {
        public GdiImageRendererTransGif(PixelFormat pixelFormat)
            : base(pixelFormat)
        {

        }

        public GdiImageRendererTransGif()
            : base()
        {

        }

        public int? TransparentColorIndex { get; set; }

        protected override Stream RenderStreamInternal(WebMapView map, out string mimeType)
        {
            Stream stream = base.RenderStreamInternal(map, out mimeType);
            if (MapView.BackgroundColor == StyleColor.Transparent
                && TransparentColorIndex.HasValue
                && ImageCodec.Clsid == FindCodec("image/gif").Clsid)
            {


                Color trans = ViewConverter.Convert(MapView.BackgroundColor);

                using (Bitmap bmpSrc = new Bitmap(stream))
                {
                    using (Bitmap bmpTrgt = new Bitmap(bmpSrc.Width, bmpSrc.Height, PixelFormat.Format8bppIndexed))
                    {
                        ColorPalette cp = bmpSrc.Palette;
                        ColorPalette ncp = bmpTrgt.Palette;

                        int n = 0;
                        foreach (Color c in cp.Entries)
                            ncp.Entries[n++] = Color.FromArgb(255, c);


                        ncp.Entries[TransparentColorIndex.Value] = Color.FromArgb(0, ncp.Entries[TransparentColorIndex.Value]);
                        bmpTrgt.Palette = ncp;


                        BitmapData src =
                            bmpSrc.LockBits(new Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height),
                                            ImageLockMode.ReadOnly, bmpSrc.PixelFormat);

                        BitmapData dst =
                            bmpTrgt.LockBits(new Rectangle(0, 0, bmpTrgt.Width, bmpTrgt.Height), ImageLockMode.WriteOnly,
                                             bmpTrgt.PixelFormat);


                        unsafe
                        {


                            for (int y = 0; y < bmpSrc.Height; y++)

                                for (int x = 0; x < bmpSrc.Width; x++)
                                {

                                    ((byte*)dst.Scan0.ToPointer())[(dst.Stride * y) + x] =
                                        ((byte*)src.Scan0.ToPointer())[(src.Stride * y) + x];

                                }

                        }

                        bmpSrc.UnlockBits(src);
                        bmpTrgt.UnlockBits(dst);

                        MemoryStream ms = new MemoryStream();
                        bmpTrgt.Save(ms, ImageFormat.Gif);

                        return ms;
                    }
                }
            }


            return stream;
        }
    }
}