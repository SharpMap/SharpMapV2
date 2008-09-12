using System;
using NPack.Interfaces;
using Tao.OpenAl;

namespace Reflexive.Audio
{
    public class SoundSource<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        // Sources are points of emitting sound.
        public int m_SourceHandle = -1;
        /*
         * These are 3D Cartesian vector coordinates. A structure or class would be
         * a more flexible of handling these, but for the sake of simplicity we will
         * just leave it as is.
         */
        private float[] m_SourcePosition = { 0, 0, 0 };                  // Position of the source sound.
        private float[] m_SourceVelocity = { 0, 0, 0 };                  // Velocity of the source sound.

        public SoundSource()
        {
            // Generate an OpenAL source.
            Al.alGenSources(1, out m_SourceHandle);
            if (Al.alGetError() != Al.AL_NO_ERROR)
            {
                return;
            }

            AudioSystem<T>.s_LoadedSoundSources.Add(this);
        }

        public bool BindToBuffer(SoundBuffer<T> bufferToBindTo)
        {
            if (m_SourceHandle == -1)
            {
                return false;
            }

            // Bind the buffer with the source.
            Al.alSourcei(m_SourceHandle, Al.AL_BUFFER, bufferToBindTo.m_BufferHandle);
            Al.alSourcef(m_SourceHandle, Al.AL_PITCH, 1.0f);
            Al.alSourcef(m_SourceHandle, Al.AL_GAIN, 1.0f);
            Al.alSourcefv(m_SourceHandle, Al.AL_POSITION, m_SourcePosition);
            Al.alSourcefv(m_SourceHandle, Al.AL_VELOCITY, m_SourceVelocity);
            Al.alSourcei(m_SourceHandle, Al.AL_LOOPING, bufferToBindTo.m_Loop);

            // Do a final error check and then return.
            if (Al.alGetError() == Al.AL_NO_ERROR)
            {
                return true;
            }

            return false;
        }

        public void Play()
        {
            Al.alSourcePlay(m_SourceHandle);
        }
    }
}
