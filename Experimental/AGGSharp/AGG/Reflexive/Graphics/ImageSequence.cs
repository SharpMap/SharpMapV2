using System;
using System.Collections.Generic;
using System.IO;
using NPack.Interfaces;
using Reflexive.Game;

namespace Reflexive.Graphics
{
    public class ImageSequence<T> : GameObject<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        [GameDataNumber("FramesPerSecond", 
            Description="Stored so that an object using this sequence knows how fast it should play.",
            Min = 0, Max = 120)]
        double m_FramesPerSecond = 30;
        [GameDataBool("AnimationIsLooping",
            Description = "Does the last frame loop back to the first frame.  Used when asking for a frame out of range.")]
        bool m_Looping = false;

        Image<T>[] m_Images;

        public ImageSequence()
        {
        }

        private static ImageSequence<T> LoadSerializationFileForFolder(String gameDataObjectXMLPath)
        {
            ImageSequence<T> sequenceLoaded;

            sequenceLoaded = (ImageSequence<T>)GameObject<T>.Load(gameDataObjectXMLPath);

            if (sequenceLoaded == null)
            {
                sequenceLoaded = new ImageSequence<T>();
                sequenceLoaded.SaveXML(gameDataObjectXMLPath);
            }

            return sequenceLoaded;
        }

        public new static GameObject<T> Load(String PathName)
        {
            // First we load up the Data In the Serialization file.
            String gameDataObjectXMLPath = Path.Combine(PathName, "ImageSequence");
            ImageSequence<T> sequenceLoaded = LoadSerializationFileForFolder(gameDataObjectXMLPath);

            // Now lets look for and load up any images that we find.
            String[] tgaFilesArray = Directory.GetFiles(PathName, "*.tga");
            List<String> sortedTgaFiles = new List<string>(tgaFilesArray);
            // Make sure they are sorted.
            sortedTgaFiles.Sort();
            sequenceLoaded.m_Images = new Image<T>[sortedTgaFiles.Count];
            int imageIndex = 0;
            foreach (String tgaFile in sortedTgaFiles)
            {
                sequenceLoaded.m_Images[imageIndex] = new Image<T>();
                Stream imageStream = File.Open(tgaFile, FileMode.Open);
                sequenceLoaded.m_Images[imageIndex].LoadImageData(imageStream);
                imageIndex++;
            }

            return sequenceLoaded;
        }

        public Image<T> GetImageByTime(double NumSeconds)
        {
            double TotalSeconds = NumFrames / FramePerSecond;
            return GetImageByRatio(NumSeconds / TotalSeconds);
        }

        public Image<T> GetImageByRatio(double FractionOfTotalLength)
        {
            return GetImageByIndex(FractionOfTotalLength * (NumFrames - 1));
        }

        public Image<T> GetImageByIndex(double ImageIndex)
        {
            return GetImageByIndex((int)(ImageIndex + .5));
        }

        public Image<T> GetImageByIndex(int ImageIndex)
        {
            if (m_Looping)
            {
                return m_Images[ImageIndex % NumFrames];
            }

            if(ImageIndex < 0)
            {
                return m_Images[0];
            }
            else if (ImageIndex > NumFrames - 1)
            {
                return m_Images[NumFrames - 1];
            }

            return m_Images[ImageIndex];
        }

        public int NumFrames
        {
            get
            {
                return m_Images.Length;
            }
        }

        public double FramePerSecond
        {
            get
            {
                return m_FramesPerSecond;
            }
        }
    }
}
