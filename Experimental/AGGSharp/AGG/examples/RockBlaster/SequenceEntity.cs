using System;
using AGG.Buffer;
using AGG.PixelFormat;
using AGG.Rendering;
using NPack.Interfaces;
using Reflexive.Game;
using Reflexive.Graphics;

namespace RockBlaster
{
    public class SequenceEntity<T> : Entity<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>

    {
        [GameData("Sequence")]
        public AssetReference<T, ImageSequence<T>> ImageSequenceReference = new AssetReference<T, ImageSequence<T>>("LargFire");

        [GameDataNumberAttribute("Rotation")] // This is for save game
        protected double m_Rotation;

        [GameDataNumberAttribute("Scale")] // This is for save game
        protected double m_Scale;

        double m_TotalSeconds;

        public SequenceEntity(IVector<T> position)
            : base(3)
        {
            Position = position;
        }

        protected override void DoDraw(RendererBase<T> destRenderer)
        {
            Image<T> imageToDraw = ImageSequenceReference.Instance.GetImageByTime(m_TotalSeconds);
            //Image imageToDraw = m_PlayerShipSequence.GetImageByIndex(m_ImageIndex);
            //IBlender blender = new BlenderBGRA();
            IBlender blender = new BlenderAddativeBGR();

            unsafe
            {
                RasterBuffer destBuffer = destRenderer.PixelFormat.GetRenderingBuffer();
                byte* pPixels = destBuffer.GetPixelPointer(200);
                byte[] sourceBuffer = imageToDraw.ImageBuffer;
                for (int y = 0; y < imageToDraw.Height; y++)
                {
                    int SourceYOffset = y * imageToDraw.ScanWidthInBytes;
                    int destYOffset = (int)destBuffer.StrideInBytesAbs * y;
                    for (int x = 0; x < imageToDraw.Width; x++)
                    {
                        int sourceOffset = SourceYOffset + x * 4;
                        blender.BlendPix(&pPixels[destYOffset + x * 4],
                            sourceBuffer[sourceOffset + 2], sourceBuffer[sourceOffset + 1], sourceBuffer[sourceOffset + 0], sourceBuffer[sourceOffset + 3]);
                    }
                }
            }
        }

        public override void Update(double numSecondsPassed)
        {
            m_TotalSeconds += numSecondsPassed;
            base.Update(numSecondsPassed);
        }
    }
}
