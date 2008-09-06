using System;
using System.Collections.Generic;
using NPack.Interfaces;
using Tao.OpenAl;

namespace Reflexive.Audio
{
    public static class AudioSystem<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private static float[] listenerPosition = { 0, 0, 0 };                // Position of the Listener.
        private static float[] listenerVelocity = { 0, 0, 0 };                // Velocity of the Listener.

        // Orientation of the Listener. (first 3 elements are "at", second 3 are "up")
        // Also note that these should be units of '1'.
        private static float[] listenerOrientation = { 0, 0, -1, 0, 1, 0 };

        public static List<SoundBuffer<T>> s_LoadedSoundBuffers = new List<SoundBuffer<T>>();
        public static List<SoundSource<T>> s_LoadedSoundSources = new List<SoundSource<T>>();

        public static void Startup()
        {
            // Initialize OpenAL and clear the error bit.
            Alut.alutInit();
            Al.alGetError();

            Al.alListenerfv(Al.AL_POSITION, listenerPosition);
            Al.alListenerfv(Al.AL_VELOCITY, listenerVelocity);
            Al.alListenerfv(Al.AL_ORIENTATION, listenerOrientation);
        }

        public static void Shutdown()
        {
            foreach (SoundBuffer<T> loadedSound in s_LoadedSoundBuffers)
            {
                Al.alDeleteBuffers(1, ref loadedSound.m_BufferHandle);
            }
            s_LoadedSoundBuffers.Clear();
            foreach (SoundSource<T> loadedSound in s_LoadedSoundSources)
            {
                Al.alDeleteSources(1, ref loadedSound.m_SourceHandle);
            }
            s_LoadedSoundSources.Clear();
            Alut.alutExit();
        }
    }
}
