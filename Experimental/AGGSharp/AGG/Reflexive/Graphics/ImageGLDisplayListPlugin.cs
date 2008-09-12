using System;
using System.Collections.Generic;
using NPack.Interfaces;
using Tao.OpenGl;

namespace Reflexive.Graphics
{
    public class ImageGLDisplayListPlugin<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private int glTextureHandle;
        private int glDisplayListHandle;
        private static Dictionary<Image<T>, ImageGLDisplayListPlugin<T>> m_ImagesWithCacheData = new Dictionary<Image<T>, ImageGLDisplayListPlugin<T>>();

        static public String SPluginName
        {
            get
            {
                return "ImageGLDisplayListPlugin";
            }
        }

        static public ImageGLDisplayListPlugin<T> GetImageGLDisplayListPlugin(Image<T> imageToFor)
        {
            if (!m_ImagesWithCacheData.ContainsKey(imageToFor))
            {
                ImageGLDisplayListPlugin<T> plugin = new ImageGLDisplayListPlugin<T>();
                m_ImagesWithCacheData.Add(imageToFor, plugin);
                plugin.BuildDisplayList(imageToFor);
                return plugin;
            }

            return m_ImagesWithCacheData[imageToFor];
        }

        private ImageGLDisplayListPlugin()
        {
            // you can't build one of these you have to call GetImageGLDisplayListPlugin.
        }

        ~ImageGLDisplayListPlugin()
        {
            // TODO: Make this work. // Gl.glDeleteLists(glDisplayListHandle, 1);
            int[] textureList = new int[1];
            textureList[0] = glTextureHandle;
            // TODO: Make this work. // Gl.glDeleteTextures(1, textureList);
        }

        private void CheckGLError()
        {

        }

        public int GLDisplayList
        {
            get
            {
                return glDisplayListHandle;
            }
        }

        public int GLTextureHandle
        {
            get
            {
                return glTextureHandle;
            }
        }

        int SmallestHardwareCompatibleTextureSize(int size)
        {
            return size;
        }

        private void BuildDisplayList(Image<T> bufferedImage)
        {
            //Next we expand the image into an openGL texture
            int imageWidth = bufferedImage.Width;
            int imageHeight = bufferedImage.Height;
            byte[] imageBuffer = bufferedImage.ImageBuffer;
            int hardwareWidth = SmallestHardwareCompatibleTextureSize(imageWidth);
            int hardwareHeight = SmallestHardwareCompatibleTextureSize(imageHeight);
            byte[] hardwareExpandedPixelBuffer = new byte[4 * hardwareWidth * hardwareHeight];
            for (int y = 0; y < hardwareHeight; y++)
            {
                for (int x = 0; x < hardwareWidth; x++)
                {
                    int pixelIndex = 4 * (x + y * hardwareWidth);
                    if (x >= imageWidth || y >= imageHeight)
                    {
                        hardwareExpandedPixelBuffer[pixelIndex + 0] = 0;
                        hardwareExpandedPixelBuffer[pixelIndex + 1] = 0;
                        hardwareExpandedPixelBuffer[pixelIndex + 2] = 0;
                        hardwareExpandedPixelBuffer[pixelIndex + 3] = 0;
                    }
                    else
                    {
                        hardwareExpandedPixelBuffer[pixelIndex + 0] = imageBuffer[4 * (x + y * imageWidth) + 0];
                        hardwareExpandedPixelBuffer[pixelIndex + 1] = imageBuffer[4 * (x + y * imageWidth) + 1];
                        hardwareExpandedPixelBuffer[pixelIndex + 2] = imageBuffer[4 * (x + y * imageWidth) + 2];
                        hardwareExpandedPixelBuffer[pixelIndex + 3] = imageBuffer[4 * (x + y * imageWidth) + 3];
                    }
                }
            }

            // Create the texture handle and display list handle
            glDisplayListHandle = Gl.glGenLists(1);
            int[] textureHandle = new int[1];
            Gl.glGenTextures(1, textureHandle);
            glTextureHandle = textureHandle[0];

            // Set up some texture parameters for openGL
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, glTextureHandle);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);

            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE);

            // Create the texture
            Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, hardwareWidth, hardwareHeight,
                0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, hardwareExpandedPixelBuffer);
            hardwareExpandedPixelBuffer = null;

            //Create a display list and bind a texture to it
            Gl.glNewList((uint)(glDisplayListHandle), Gl.GL_COMPILE);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, glTextureHandle);

            //Account for relative image position (it's local coordinate system)

            float glyphLeftStart = 0;
            Gl.glTranslatef(glyphLeftStart, 0, 0);
            Gl.glPushMatrix();
            float glyphTopStart = 0;
            Gl.glTranslatef(0, glyphTopStart, 0);
            float texCoordX = (float)imageWidth / (float)hardwareWidth;
            float texCoordY = (float)imageHeight / (float)hardwareHeight;

            //Draw the quad
            Gl.glBegin(Gl.GL_QUADS);
            Gl.glTexCoord2d(0, 0); Gl.glVertex2f(0, imageHeight);
            Gl.glTexCoord2d(0, texCoordY); Gl.glVertex2f(0, 0);
            Gl.glTexCoord2d(texCoordX, texCoordY); Gl.glVertex2f(imageWidth, 0);
            Gl.glTexCoord2d(texCoordX, 0); Gl.glVertex2f(imageWidth, imageHeight);
            Gl.glEnd();
            Gl.glPopMatrix();

            Gl.glEndList();
        }
    }
}
