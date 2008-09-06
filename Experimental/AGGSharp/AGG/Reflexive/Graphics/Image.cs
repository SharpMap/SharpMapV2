using System;
using System.IO;
using AGG.Buffer;
using AGG.Color;
using AGG.PixelFormat;
using AGG.Rasterizer;
using AGG.Rendering;
using AGG.Scanline;
using AGG.VertexSource;
using NPack.Interfaces;
using Reflexive.Game;

namespace Reflexive.Graphics
{
    public class Image<T> : GameObject<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private Byte[] m_ImageBuffer;
        private int m_Width;
        private int m_Height;
        private int m_BitDepth;
        private int m_ScanWidthInBytes;

        public int Width { get { return m_Width; } }
        public int Height { get { return m_Height; } }
        public int BitDepth { get { return m_BitDepth; } }
        public int ScanWidthInBytes { get { return m_ScanWidthInBytes; } }
        public byte[] ImageBuffer { get { return m_ImageBuffer; } }

        ImageRenderer m_ImageRenderer;

        internal class ImageRenderer : Renderer<T>
        {
            Image<T> m_Owner;
            internal ImageRenderer(Image<T> owner)
                : base()
            {
                m_Owner = owner;

                RasterizerScanlineAA<T> rasterizer = new RasterizerScanlineAA<T>();

                IPixelFormat imagePixelFormat = new FormatRGBA(new RasterBuffer(), new BlenderBGRA());
                FormatClippingProxy imageClippingProxy = new FormatClippingProxy(imagePixelFormat);

                Initialize(imageClippingProxy, rasterizer);
                ScanlineCache = new ScanlinePacked8();
            }

            public override void Render(IVertexSource<T> vertexSource, uint pathIndexToRender, RGBA_Bytes colorBytes)
            {
                unsafe
                {
                    fixed (Byte* pImageBuffer = m_Owner.ImageBuffer)
                    {
                        PixelFormat.GetRenderingBuffer().attach(pImageBuffer,
                            (uint)m_Owner.Width, (uint)m_Owner.Height, m_Owner.ScanWidthInBytes, 32);
                    }
                }
            }

            public override void Clear(IColorType color)
            {

            }
        };

        public Image()
        {
            m_ImageRenderer = new ImageRenderer(this);
        }

        public Image(int inWidth, int inHeight)
            : this()
        {
            Allocate(inWidth, inHeight, 32);
        }

        public RendererBase<T> GetRenderer()
        {
            m_ImageRenderer.Rasterizer.SetVectorClipBox(M.Zero<T>(), M.Zero<T>(), M.New<T>(Width), M.New<T>(Height));

            return m_ImageRenderer;
        }

        public void LoadImageData(Stream streamToLoadImageDataFrom)
        {
            //Bitmap bitmap = (Bitmap)Bitmap.FromStream(streamToLoadImageDataFrom);
            //bitmap.LockBits();
            byte[] ImageData = new byte[streamToLoadImageDataFrom.Length];
            streamToLoadImageDataFrom.Read(ImageData, 0, (int)streamToLoadImageDataFrom.Length);
            ImageTgaIO<T>.ReadBitsFromBuffer(this, ImageData, 32);
        }

        internal void Allocate(int inWidth, int inHeight, int inBitDepth)
        {
            if (inBitDepth != (inBitDepth / 8) * 8)
            {
                throw new System.Exception("Your BitDepth must be a multiple of 8.");
            }

            Allocate(inWidth, inHeight, inWidth * (inBitDepth / 8), inBitDepth);
        }

        internal void Allocate(int inWidth, int inHeight, int inBitDepth, int inScanWidthInBytes)
        {
            if (inBitDepth != (inBitDepth / 8) * 8)
            {
                throw new System.Exception("Your BitDepth must be a multiple of 8.");
            }

            if (inScanWidthInBytes < inWidth * (inBitDepth / 8))
            {
                throw new System.Exception("Your scan width cannot be less than the width * the bit depth.");
            }

            m_Width = inWidth;
            m_Height = inHeight;
            m_BitDepth = inBitDepth;
            m_ScanWidthInBytes = inScanWidthInBytes;
            m_ImageBuffer = new byte[m_ScanWidthInBytes * m_Height];
        }
    }
}
