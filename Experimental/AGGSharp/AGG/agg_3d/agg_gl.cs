using System;
using AGG.Buffer;
using AGG.Color;
using AGG.PixelFormat;
using AGG.Rasterizer;
using AGG.Rendering;
using AGG.Scanline;
using AGG.Transform;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;
using Reflexive.Math;

namespace AGG.agg_3d
{
    // has a gl like interface (you should be able to replace it by simply using tao).
    public class Gl<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public const int GL_FLAT = 0x00001d00;
        public const int GL_SMOOTH = 0x00001d01;
        public const int GL_DEPTH_TEST = 0x00000b71;
        public const int GL_LEQUAL = 0x00000203;
        public const int GL_PERSPECTIVE_CORRECTION_HINT = 0x00000c50;
        public const int GL_NICEST = 0x00001102;
        public const int GL_COLOR_BUFFER_BIT = 0x00004000;
        public const int GL_DEPTH_BUFFER_BIT = 0x00000100;
        public const int GL_TRIANGLES = 0x00000004;
        public const int GL_QUADS = 0x00000007;
        public const int GL_PROJECTION = 0x00001701;
        public const int GL_MODELVIEW = 0x00001700;

        private static FormatRGBA s_PixelFormt;
        private static IPixelFormat s_ClippingPixelFormatProxy;
        private static AGG.SpanAllocator s_SpanAllocator = new SpanAllocator();
        private static SpanGouraudRgba<T> s_GouraudSpanGen = new SpanGouraudRgba<T>();
        private static ScanlinePacked8 s_ScanlineUnpacked = new ScanlinePacked8();
        private static RasterizerScanlineAA<T> s_Rasterizer = new RasterizerScanlineAA<T>();
        private static PathStorage<T> s_SharedPathStorage = new PathStorage<T>();
        private static RasterBuffer s_RenderingBuffer;

        private static int s_ShadeModel;
        private static RGBA_Doubles s_ClearColor = new RGBA_Doubles();
        private static double s_ClearDepth;
        private static int s_BeginMode;

        private static RectInt s_GLViewport = new RectInt();
        //private static Matrix4X4 m_CurEditingMatrix;
        //private static Matrix4X4 m_ModelviewMatrix;
        //private static Matrix4X4 m_ProjectionMatrix;
        //private static Matrix4X4 m_CurAccumulatedMatrix;

        private static IAffineTransformMatrix<T> m_CurEditingMatrix;
        private static IAffineTransformMatrix<T> m_ModelviewMatrix;
        private static IAffineTransformMatrix<T> m_ProjectionMatrix;
        private static IAffineTransformMatrix<T> m_CurAccumulatedMatrix;



        private const int m_MaxVertexCacheItems = 16;
        private static VertexCachItem[] m_pVertexCache;
        private static int m_NumCachedVertex;

        private static RGBA_Doubles m_LastSetColor;
        //private static Vector3D m_LastSetNormal;
        //private static Vector2D m_LastSetTextureCoordinate;
        private static IVector<T> m_LastSetNormal;
        private static IVector<T> m_LastSetTextureCoordinate;

        internal class VertexCachItem
        {
            //internal Vector3D m_Position;
            internal IVector<T> m_Position;

            internal RGBA_Doubles m_Color;
            //internal Vector3D m_Normal;
            //internal Vector2D m_TextureCoordinate;
            internal IVector<T> m_Normal;
            internal IVector<T> m_TextureCoordinate;

        };

        public static void CreateContext(RasterBuffer renderingBuffer)
        {
            // set the rendering buffer and all the GL default states (this is also where you can new anything you need)
            s_RenderingBuffer = renderingBuffer;
            s_Rasterizer.SetVectorClipBox(M.Zero<T>(), M.Zero<T>(), M.New<T>(s_RenderingBuffer.Width), M.New<T>(s_RenderingBuffer.Height));
            s_PixelFormt = new FormatRGBA(s_RenderingBuffer, new BlenderBGRA());
            s_ClippingPixelFormatProxy = new FormatClippingProxy(s_PixelFormt);
            //s_ClippingPixelFormatProxy = s_PixelFormt;

            m_CurAccumulatedMatrix = MatrixFactory<T>.NewIdentity(VectorDimension.Three);// new Matrix4X4();
            m_ModelviewMatrix = MatrixFactory<T>.NewIdentity(VectorDimension.Three);// new Matrix4X4();
            m_ProjectionMatrix = MatrixFactory<T>.NewIdentity(VectorDimension.Three); //new Matrix4X4();

            m_pVertexCache = new VertexCachItem[m_MaxVertexCacheItems];
            for (int i = 0; i < m_MaxVertexCacheItems; i++)
            {
                m_pVertexCache[i] = new VertexCachItem();
            }

            m_LastSetColor = new RGBA_Doubles(1, 1, 1, 1);
            m_LastSetNormal = MatrixFactory<T>.CreateZeroVector(VectorDimension.Three);// new Vector3D();
            m_LastSetTextureCoordinate = MatrixFactory<T>.CreateZeroVector(VectorDimension.Two);// new Vector2D();
        }

        public static void glShadeModel(int mode)
        {
            s_ShadeModel = mode;
        }

        public static void glClearColor(double red, double green, double blue, double alpha)
        {
            s_ClearColor = new RGBA_Doubles(red, green, blue, alpha);
        }

        public static void glClearDepth(double depth)
        {
            s_ClearDepth = Math.Max(0, Math.Min(1, depth));

        }

        public static void glEnable(int cap)
        {
            switch (cap)
            {
                case GL_DEPTH_TEST:
                    break;

                default:
                    throw new System.NotImplementedException();
            }
        }

        public static void glDepthFunc(int func)
        {
            switch (func)
            {
                case GL_LEQUAL:
                    break;

                default:
                    throw new System.NotImplementedException();
            }
        }

        public static void glHint(int target, int mode)
        {
            switch (target)
            {
                case GL_PERSPECTIVE_CORRECTION_HINT:
                    switch (mode)
                    {
                        case GL_NICEST:
                            break;

                        default:
                            throw new System.NotImplementedException();
                    }
                    break;

                default:
                    throw new System.NotImplementedException();
            }
        }

        public static void glClear(int mask)
        {
            if ((mask & GL_COLOR_BUFFER_BIT) != 0)
            {
                FormatRGBA pf = new FormatRGBA(s_RenderingBuffer, new BlenderBGRA());

                FormatClippingProxy renderBase = new FormatClippingProxy(pf);

                RasterizerScanlineAA<T> rasterizer = new RasterizerScanlineAA<T>();
                ScanlinePacked8 scanline = new ScanlinePacked8();

                renderBase.Clear(s_ClearColor);
            }

            if ((mask & GL_DEPTH_BUFFER_BIT) != 0)
            {

            }
        }

        public static void glLoadIdentity()
        {
            //m_CurEditingMatrix.Identity();
            m_CurEditingMatrix.Reset();
        }

        public static void glTranslatef(T x, T y, T z)
        {
            //if (s_BeginMode != 0) throw new System.FormatException("You cannot call this inside a Begin - End section.");
            //double[] NewMatrix =
            //{
            //    1.0f, 0.0f, 0.0f, 0.0f,
            //    0.0f, 1.0f, 0.0f, 0.0f,
            //    0.0f, 0.0f, 1.0f, 0.0f,
            //    x,   y,   z, 1.0f,
            //};

            //glMultMatrixf(NewMatrix);
            m_CurEditingMatrix.Translate(MatrixFactory<T>.CreateVector3D(x, y, z));
        }

        public static void glRotatef(double AngleDegrees, T AxisX, T AxisY, T AxisZ)
        {
            //if (s_BeginMode != 0) throw new System.FormatException("You cannot call this inside a Begin - End section.");
            //Vector3D Axis = new Vector3D(AxisX, AxisY, AxisZ);
            //Matrix4X4 Matrix = new Matrix4X4();
            //double AngleRadians = Helper.DegToRad(AngleDegrees);
            //Matrix.Rotate(Axis, AngleRadians);

            //glMultMatrixf(Matrix.GetElements());
            m_CurEditingMatrix.RotateAlong(MatrixFactory<T>.CreateVector3D(AxisX, AxisY, AxisZ), Helper.DegToRad(AngleDegrees));
        }

        public static void glBegin(int mode)
        {
            //m_CurAccumulatedMatrix.Multiply(m_ModelviewMatrix, m_ProjectionMatrix);

            m_CurAccumulatedMatrix = (IAffineTransformMatrix<T>)m_CurAccumulatedMatrix.Multiply(m_ModelviewMatrix);
            m_CurAccumulatedMatrix = (IAffineTransformMatrix<T>)m_CurAccumulatedMatrix.Multiply(m_ProjectionMatrix);

            switch (mode)
            {
                case GL_TRIANGLES:
                    s_BeginMode = GL_TRIANGLES;
                    break;

                case GL_QUADS:
                    s_BeginMode = GL_QUADS;
                    break;

                default:
                    throw new System.NotImplementedException();
            }
        }

        public static void glEnd()
        {
            s_BeginMode = 0;
        }

        public static void glColor3f(float red, float green, float blue)
        {

            m_LastSetColor = new RGBA_Doubles(red, green, blue, m_LastSetColor.A);
            //m_LastSetColor.R = red;
            //m_LastSetColor.G = green;
            //m_LastSetColor.B = blue;
        }

        public static void glVertex3f(T x, T y, T z)
        {
            IVector<T> WorldPoint = MatrixFactory<T>.CreateVector3D(x, y, z);
            WorldPoint = m_CurAccumulatedMatrix.TransformVector(WorldPoint);
            T OneOverZ = M.One<T>().Divide(WorldPoint[2]);
            WorldPoint[0] = WorldPoint[0].Multiply(OneOverZ).Add(1).Multiply(s_GLViewport.X2 * .5f).Add(s_GLViewport.X1);
            WorldPoint[1] = WorldPoint[1].Multiply(OneOverZ).Add(1).Multiply(s_GLViewport.Y2 * .5f).Add(s_GLViewport.Y1);

            m_pVertexCache[m_NumCachedVertex].m_Position = WorldPoint;
            m_pVertexCache[m_NumCachedVertex].m_Color = m_LastSetColor;
            m_pVertexCache[m_NumCachedVertex].m_Normal = m_LastSetNormal;
            m_pVertexCache[m_NumCachedVertex].m_TextureCoordinate = m_LastSetTextureCoordinate;

            m_NumCachedVertex++;

            //double Dilation = 0;
            T Dilation = M.New<T>(.175);

            switch (s_BeginMode)
            {
                /*
            case GL_POINTS:
                {
                    RenderLine(m_pVertexCache[0].m_Position, m_pVertexCache[0].m_Position, m_LastSetColor);
                    m_NumCachedVertex3fs = 0;
                }
                break;

            case LINES:
                {
                    if(m_NumCachedVertex3fs == 2)
                    {
                        // <WIP> use both vertex colors LBB [1/14/2002]
                        RenderLine(m_pVertexCache[0].m_Position, m_pVertexCache[1].m_Position, m_pVertexCache[0].m_Color);
                        m_NumCachedVertex3fs = 0;
                    }
                }
                break;
                 */

                case GL_TRIANGLES:
                    if (m_NumCachedVertex == 3)
                    {
                        IVector<T> Winding = VectorUtilities<T>.Cross(
                            m_pVertexCache[1].m_Position.Subtract(m_pVertexCache[0].m_Position),
                            m_pVertexCache[2].m_Position.Subtract(m_pVertexCache[0].m_Position));



                        if (Winding[2].GreaterThan(0))
                        {
                            s_GouraudSpanGen.Colors(m_pVertexCache[0].m_Color,
                                            m_pVertexCache[1].m_Color,
                                            m_pVertexCache[2].m_Color);
                            s_GouraudSpanGen.Triangle(m_pVertexCache[0].m_Position[0], m_pVertexCache[0].m_Position[1],
                                m_pVertexCache[1].m_Position[0], m_pVertexCache[1].m_Position[1],
                                m_pVertexCache[2].m_Position[0], m_pVertexCache[2].m_Position[1], Dilation);
                            s_Rasterizer.AddPath(s_GouraudSpanGen);
                            //s_Rasterizer.gamma(new gamma_linear(0.0, 0.809));
                            //renderer_scanlines.render_scanlines_aa_solid(ras, sl, ren_base, m_pVertexCache[0].m_Color.Get_rgba8());
                            Renderer<T>.GenerateAndRender(s_Rasterizer, s_ScanlineUnpacked, s_ClippingPixelFormatProxy, s_SpanAllocator, s_GouraudSpanGen);
                        }

                        m_NumCachedVertex = 0;
                    }
                    break;

                case GL_QUADS:
                    if (m_NumCachedVertex == 4)
                    {
                        IVector<T> Winding = VectorUtilities<T>.Cross(
                            m_pVertexCache[1].m_Position.Subtract(m_pVertexCache[0].m_Position),
                            m_pVertexCache[2].m_Position.Subtract(m_pVertexCache[0].m_Position));

                        if (Winding[2].GreaterThan(0))
                        {
                            switch (s_ShadeModel)
                            {
                                case GL_FLAT:
                                    // this can be faster if we don't try to shade it (no span generator)
                                    s_SharedPathStorage.RemoveAll();
                                    s_SharedPathStorage.MoveTo(m_pVertexCache[0].m_Position[0], m_pVertexCache[0].m_Position[1]);
                                    s_SharedPathStorage.LineTo(m_pVertexCache[1].m_Position[0], m_pVertexCache[1].m_Position[1]);
                                    s_SharedPathStorage.LineTo(m_pVertexCache[2].m_Position[0], m_pVertexCache[2].m_Position[1]);
                                    s_SharedPathStorage.LineTo(m_pVertexCache[3].m_Position[0], m_pVertexCache[3].m_Position[1]);

                                    s_Rasterizer.AddPath(s_SharedPathStorage);
                                    Renderer<T>.RenderSolid(s_ClippingPixelFormatProxy, s_Rasterizer, s_ScanlineUnpacked, m_pVertexCache[0].m_Color.GetAsRGBA_Bytes());
                                    break;

                                case GL_SMOOTH:
                                    s_GouraudSpanGen.Colors(
                                        m_pVertexCache[0].m_Color,
                                        m_pVertexCache[1].m_Color,
                                        m_pVertexCache[2].m_Color);

                                    s_GouraudSpanGen.Triangle(
                                        m_pVertexCache[0].m_Position[0], m_pVertexCache[0].m_Position[1],
                                        m_pVertexCache[1].m_Position[0], m_pVertexCache[1].m_Position[1],
                                        m_pVertexCache[2].m_Position[0], m_pVertexCache[2].m_Position[1], Dilation);

                                    s_Rasterizer.AddPath(s_GouraudSpanGen);
                                    //s_Rasterizer.gamma(new gamma_linear(0.0, 0.809));

                                    Renderer<T>.RenderSolid(s_ClippingPixelFormatProxy, s_Rasterizer, s_ScanlineUnpacked, m_pVertexCache[0].m_Color.GetAsRGBA_Bytes());

                                    s_GouraudSpanGen.Colors(
                                        m_pVertexCache[0].m_Color,
                                        m_pVertexCache[2].m_Color,
                                        m_pVertexCache[3].m_Color);

                                    s_GouraudSpanGen.Triangle(
                                        m_pVertexCache[0].m_Position[0], m_pVertexCache[0].m_Position[1],
                                        m_pVertexCache[2].m_Position[0], m_pVertexCache[2].m_Position[1],
                                        m_pVertexCache[3].m_Position[0], m_pVertexCache[3].m_Position[1], M.Zero<T>());
                                    s_Rasterizer.AddPath(s_GouraudSpanGen);
                                    Renderer<T>.RenderSolid(s_ClippingPixelFormatProxy, s_Rasterizer, s_ScanlineUnpacked, m_pVertexCache[0].m_Color.GetAsRGBA_Bytes());
                                    break;

                                default:
                                    throw new System.NotImplementedException();
                            }
                        }

                        m_NumCachedVertex = 0;
                    }
                    break;

                default:
                    // Be sure to call Begin before you pass Vertexes. LBB [1/12/2002]
                    throw new System.NotImplementedException("You do not have a mode set or you have an invalid mode. LBB [1/12/2002]");
            }
        }

        public static void glFrustum(T left, T right, T bottom, T top, T zNear, T zFar)
        {
            // This is right out of the book (assuming I typed it correctly). LBB [7/6/2003]
            T E = zNear.Divide(right.Subtract(left)).Multiply(2);
            T F = zNear.Divide(top.Subtract(bottom)).Multiply(2);
            T A = right.Add(left).Divide(right.Subtract(left));
            T B = top.Add(bottom).Divide(top.Subtract(bottom));
            T C = zFar.Add(zNear).Divide(zFar.Subtract(zNear)).Negative();
            T D = zFar.Multiply(zNear).Divide(zFar.Subtract(zNear)).Multiply(-2);

            //Matrix4X4 FrustrumMatrix = new Matrix4X4();
            //FrustrumMatrix.SetElements(E, 0, 0, 0,
            //                           0, F, 0, 0,
            //                           A, B, C, -1,
            //                           0, 0, D, 0);

            //glMultMatrixf(FrustrumMatrix.GetElements());

            throw new NotImplementedException();
        }

        public static void glViewport(int x, int y, int width, int height)
        {
            s_GLViewport.X1 = x;
            s_GLViewport.Y1 = y;
            s_GLViewport.X2 = width;
            s_GLViewport.Y2 = height;

            s_Rasterizer.SetVectorClipBox(M.New<T>(s_GLViewport.X1), M.New<T>(s_GLViewport.Y1),
               M.New<T>(s_GLViewport.X1 + s_GLViewport.X2), M.New<T>(s_GLViewport.Y1 + s_GLViewport.Y2));
        }

        public static void glMatrixMode(int mode)
        {
            switch (mode)
            {
                case GL_MODELVIEW:
                    m_CurEditingMatrix = m_ModelviewMatrix;
                    break;

                case GL_PROJECTION:
                    m_CurEditingMatrix = m_ProjectionMatrix;
                    break;

                default:
                    throw new System.NotImplementedException("You have to have one of the matrices we suport");

            }
        }

        //public static void glMultMatrixf(double[] m)
        //{
        //    Matrix4X4 CurEditingMatrixHold = new Matrix4X4(m_CurEditingMatrix);
        //    Matrix4X4 Input = new Matrix4X4(m);
        //    m_CurEditingMatrix.Multiply(Input, CurEditingMatrixHold);
        //}
    };
}
