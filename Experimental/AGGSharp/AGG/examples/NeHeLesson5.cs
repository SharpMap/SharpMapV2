#define USE_OPENGL

using System;

//using pix_format_e = AGG.UI.PlatformSupportAbstract.PixelFormats;
#if USE_OPENGL
using Tao.OpenGl;
#else
  using AGG.agg_3d;
#endif

using Reflexive.Game;
using Reflexive.Graphics;

using AGG.Transform;
using AGG.VertexSource;
using AGG.Color;
using NPack;
using NPack.Interfaces;
using AGG.UI;

namespace AGG
{
    public class NeHeLesson5<T> : GamePlatform<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private static float rtri;                                              // Angle For The Triangle ( NEW )
        private static float rquad;                                             // Angle For The Quad ( NEW )

        IVector<T> m_MousePos = MatrixFactory<T>.CreateZeroVector(VectorDimension.Two);

        public NeHeLesson5(PixelFormats format, ERenderOrigin RenderOrigin)
            : base(1.0f / 60.0f, 5, format, RenderOrigin)
        {
        }

        public override void OnInitialize()
        {
            InitGL();
            base.OnInitialize();
        }

        public override void OnResize(int width, int height)
        {
            base.OnResize(width, height);

            Gl.glViewport(0, 0, width, height);                                 // Reset The Current Viewport
            Gl.glMatrixMode(Gl.GL_PROJECTION);                                  // Select The Projection Matrix
            Gl.glLoadIdentity();                                                // Reset The Projection Matrix
            Glu.gluPerspective(45, width / (double)height, 0.1, 100);          // Calculate The Aspect Ratio Of The Window
            Gl.glMatrixMode(Gl.GL_MODELVIEW);                                   // Select The Modelview Matrix
            Gl.glLoadIdentity();                                                // Reset The Modelview Matrix
        }

        private bool InitGL()
        {
#if !USE_OPENGL
            Gl.CreateContext(rbuf_window());
#endif
            Gl.glShadeModel(Gl.GL_SMOOTH);                                      // Enable Smooth Shading
            Gl.glClearColor(1, 1, 1, 0.5f);                                     // Black Background
            Gl.glClearDepth(1);                                                 // Depth Buffer Setup
            Gl.glEnable(Gl.GL_DEPTH_TEST);                                      // Enables Depth Testing
            Gl.glDepthFunc(Gl.GL_LEQUAL);                                       // The Type Of Depth Testing To Do
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);         // Really Nice Perspective Calculations

            OnResize((int)width().ToInt(), (int)height().ToInt());

            return true;
        }

        private bool DrawGLScene()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);        // Clear Screen And Depth Buffer
            Gl.glLoadIdentity();                                                // Reset The Current Modelview Matrix
            Gl.glTranslatef(-1.5f, 0, -6);                                      // Move Left 1.5 Units And Into The Screen 6.0
            Gl.glRotatef(rtri, 0, 1, 0);                                        // Rotate The Triangle On The Y axis ( NEW )
            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glBegin(Gl.GL_TRIANGLES);                                        // Drawing Using Triangles
            {
                Gl.glColor3f(1, 0, 0);                                          // Red
                Gl.glVertex3f(0, 1, 0);                                         // Top Of Triangle (Front)
                Gl.glColor3f(0, 1, 0);                                          // Green
                Gl.glVertex3f(-1, -1, 1);                                       // Left Of Triangle (Front)
                Gl.glColor3f(0, 0, 1);                                          // Blue
                Gl.glVertex3f(1, -1, 1);                                        // Right Of Triangle (Front)
                Gl.glColor3f(1, 0, 0);                                          // Red
                Gl.glVertex3f(0, 1, 0);                                         // Top Of Triangle (Right)
                Gl.glColor3f(0, 0, 1);                                          // Blue
                Gl.glVertex3f(1, -1, 1);                                        // Left Of Triangle (Right)
                Gl.glColor3f(0, 1, 0);                                          // Green
                Gl.glVertex3f(1, -1, -1);                                       // Right Of Triangle (Right)
                Gl.glColor3f(1, 0, 0);                                          // Red
                Gl.glVertex3f(0, 1, 0);                                         // Top Of Triangle (Back)
                Gl.glColor3f(0, 1, 0);                                          // Green
                Gl.glVertex3f(1, -1, -1);                                       // Left Of Triangle (Back)
                Gl.glColor3f(0, 0, 1);                                          // Blue
                Gl.glVertex3f(-1, -1, -1);                                      // Right Of Triangle (Back)
                Gl.glColor3f(1, 0, 0);                                          // Red
                Gl.glVertex3f(0, 1, 0);                                         // Top Of Triangle (Left)
                Gl.glColor3f(0, 0, 1);                                          // Blue
                Gl.glVertex3f(-1, -1, -1);                                      // Left Of Triangle (Left)
                Gl.glColor3f(0, 1, 0);                                          // Green
                Gl.glVertex3f(-1, -1, 1);                                       // Right Of Triangle (Left)
            }
            Gl.glEnd();                                                         // Finished Drawing The Triangle
            Gl.glShadeModel(Gl.GL_FLAT);
            Gl.glLoadIdentity();                                                // Reset The Current Modelview Matrix
            Gl.glTranslatef(1.5f, 0, -7);                                       // Move Right 1.5 Units And Into The Screen 7.0
            Gl.glRotatef(rquad, 1, 1, 1);                                       // Rotate The Quad On The X, Y, and Z Axis ( NEW )
            Gl.glColor3f(0.5f, 0.5f, 1);                                        // Set The Color To Blue One Time Only
            Gl.glBegin(Gl.GL_QUADS);                                            // Draw A Quad
            {
                Gl.glColor3f(0, 1, 0);                                          // Set The Color To Green
                Gl.glVertex3f(1, 1, -1);                                        // Top Right Of The Quad (Top)
                Gl.glVertex3f(-1, 1, -1);                                       // Top Left Of The Quad (Top)
                Gl.glVertex3f(-1, 1, 1);                                        // Bottom Left Of The Quad (Top)
                Gl.glVertex3f(1, 1, 1);                                         // Bottom Right Of The Quad (Top)
                Gl.glColor3f(1, 0.5f, 0);                                       // Set The Color To Orange
                Gl.glVertex3f(1, -1, 1);                                        // Top Right Of The Quad (Bottom)
                Gl.glVertex3f(-1, -1, 1);                                       // Top Left Of The Quad (Bottom)
                Gl.glVertex3f(-1, -1, -1);                                      // Bottom Left Of The Quad (Bottom)
                Gl.glVertex3f(1, -1, -1);                                       // Bottom Right Of The Quad (Bottom)
                Gl.glColor3f(1, 0, 0);                                          // Set The Color To Red
                Gl.glVertex3f(1, 1, 1);                                         // Top Right Of The Quad (Front)
                Gl.glVertex3f(-1, 1, 1);                                        // Top Left Of The Quad (Front)
                Gl.glVertex3f(-1, -1, 1);                                       // Bottom Left Of The Quad (Front)
                Gl.glVertex3f(1, -1, 1);                                        // Bottom Right Of The Quad (Front)
                Gl.glColor3f(1, 1, 0);                                          // Set The Color To Yellow
                Gl.glVertex3f(1, -1, -1);                                       // Top Right Of The Quad (Back)
                Gl.glVertex3f(-1, -1, -1);                                      // Top Left Of The Quad (Back)
                Gl.glVertex3f(-1, 1, -1);                                       // Bottom Left Of The Quad (Back)
                Gl.glVertex3f(1, 1, -1);                                        // Bottom Right Of The Quad (Back)
                Gl.glColor3f(0, 0, 1);                                          // Set The Color To Blue
                Gl.glVertex3f(-1, 1, 1);                                        // Top Right Of The Quad (Left)
                Gl.glVertex3f(-1, 1, -1);                                       // Top Left Of The Quad (Left)
                Gl.glVertex3f(-1, -1, -1);                                      // Bottom Left Of The Quad (Left)
                Gl.glVertex3f(-1, -1, 1);                                       // Bottom Right Of The Quad (Left)
                Gl.glColor3f(1, 0, 1);                                          // Set The Color To Violet
                Gl.glVertex3f(1, 1, -1);                                        // Top Right Of The Quad (Right)
                Gl.glVertex3f(1, 1, 1);                                         // Top Left Of The Quad (Right)
                Gl.glVertex3f(1, -1, 1);                                        // Bottom Left Of The Quad (Right)
                Gl.glVertex3f(1, -1, -1);                                       // Bottom Right Of The Quad (Right)
            }
            Gl.glEnd();                                                         // Done Drawing The Quad

            return true;
        }

        Image<T> m_LineImageCache;
        void CheckLineImageCache()
        {
            if (m_LineImageCache == null)
            {
                m_LineImageCache = new Image<T>(2, 1);
                m_LineImageCache.ImageBuffer[0] = 255;
                m_LineImageCache.ImageBuffer[1] = 255;
                m_LineImageCache.ImageBuffer[2] = 255;
                m_LineImageCache.ImageBuffer[3] = 255;

                m_LineImageCache.ImageBuffer[4] = 255;
                m_LineImageCache.ImageBuffer[5] = 255;
                m_LineImageCache.ImageBuffer[6] = 255;
                m_LineImageCache.ImageBuffer[7] = 0;

                /*
                m_LineImageCache.ImageBuffer[8] = 255;
                m_LineImageCache.ImageBuffer[9] = 255;
                m_LineImageCache.ImageBuffer[10] = 255;
                m_LineImageCache.ImageBuffer[11] = 0;

                m_LineImageCache.ImageBuffer[12] = 255;
                m_LineImageCache.ImageBuffer[13] = 255;
                m_LineImageCache.ImageBuffer[14] = 255;
                m_LineImageCache.ImageBuffer[15] = 0;
                 */

                /*
                PathStorage RectToDraw = new PathStorage();
                RectToDraw.move_to(0, 0);
                RectToDraw.line_to(1, 0);
                RectToDraw.line_to(1, 1);
                RectToDraw.line_to(0, 1);
                RendererBase renderer = m_LineImageCache.GetRenderer();
                renderer.Render(RectToDraw, new RGBA_Bytes(255, 255, 255, 255));
                 */
            }
        }

        public void DrawLine(IVector<T> Start, IVector<T> End, bool DoAA)
        {
            if (Start == End)
            {
                return;
            }
            Gl.glPushAttrib(Gl.GL_ALL_ATTRIB_BITS);

            CheckLineImageCache();

            ImageGLDisplayListPlugin<T> GLBuffer = ImageGLDisplayListPlugin<T>.GetImageGLDisplayListPlugin(m_LineImageCache);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glColor4f(0, 0, 0, 1);

            IVector<T> Normal = (End.Subtract(Start));
            Normal = Normal.Normalize();

            IVector<T> PerpendicularNormal = Normal.GetPerpendicular();
            float Width = 40;
            if (DoAA)
            {
                Width--;
            }
            IVector<T> OffsetPos = PerpendicularNormal.Multiply(Width);
            IVector<T> OffsetNeg = PerpendicularNormal.Multiply(-(Width + 1));

            RendererOpenGL<T>.PushOrthoProjection();

            Gl.glDisable(Gl.GL_TEXTURE_2D);

            IVector<T> StartLeft = Start.Add(OffsetPos);
            IVector<T> StartRight = Start.Add(OffsetNeg);
            IVector<T> EndLeft = End.Add(OffsetPos);
            IVector<T> EndRight = End.Add(OffsetNeg);

            Gl.glBegin(Gl.GL_QUADS);
            {
                Gl.glVertex2d(StartLeft[0].ToDouble(), StartLeft[1].ToDouble());
                Gl.glVertex2d(EndLeft[0].ToDouble(), EndLeft[1].ToDouble());
                Gl.glVertex2d(EndRight[0].ToDouble(), EndRight[1].ToDouble());
                Gl.glVertex2d(StartRight[0].ToDouble(), StartRight[1].ToDouble());
            }
            Gl.glEnd();

            if (DoAA)
            {
                double EdgeWith = 2;
                //IVector<DoubleComponent> StartLeftAA = StartLeft + PerpendicularNormal * EdgeWith - Normal * EdgeWith;
                //IVector<DoubleComponent> StartRightAA = StartRight - PerpendicularNormal * EdgeWith - Normal * EdgeWith;
                //IVector<DoubleComponent> EndLeftAA = EndLeft + PerpendicularNormal * EdgeWith + Normal * EdgeWith;
                //IVector<DoubleComponent> EndRightAA = EndRight - PerpendicularNormal * EdgeWith + Normal * EdgeWith;

                IVector<T> StartLeftAA =
                    StartLeft
                    .Add(PerpendicularNormal)
                    .Multiply(EdgeWith)
                    .Subtract(Normal)
                    .Multiply(EdgeWith);


                IVector<T> StartRightAA =
                    StartRight
                    .Subtract(PerpendicularNormal)
                    .Multiply(EdgeWith)
                    .Subtract(Normal)
                    .Multiply(EdgeWith);


                IVector<T> EndLeftAA =
                    EndLeft
                    .Add(PerpendicularNormal)
                    .Multiply(EdgeWith)
                    .Add(Normal)
                    .Multiply(EdgeWith);


                IVector<T> EndRightAA =
                    EndRight
                    .Subtract(PerpendicularNormal)
                    .Multiply(EdgeWith)
                    .Add(Normal)
                    .Multiply(EdgeWith);



                Gl.glEnable(Gl.GL_TEXTURE_2D);
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, GLBuffer.GLTextureHandle);

                Gl.glBegin(Gl.GL_QUADS);
                {
                    // left edge
                    Gl.glTexCoord2d(0, 0);
                    Gl.glVertex2d(StartLeft[0].ToDouble(), StartLeft[1].ToDouble());
                    Gl.glVertex2d(EndLeft[0].ToDouble(), EndLeft[1].ToDouble());
                    Gl.glTexCoord2d(1, 0);
                    Gl.glVertex2d(EndLeftAA[0].ToDouble(), EndLeftAA[1].ToDouble());
                    Gl.glVertex2d(StartLeftAA[0].ToDouble(), StartLeftAA[1].ToDouble());

                    // right edge
                    Gl.glTexCoord2d(0, 0);
                    Gl.glVertex2d(StartRight[0].ToDouble(), StartRight[1].ToDouble());
                    Gl.glVertex2d(EndRight[0].ToDouble(), EndRight[1].ToDouble());
                    Gl.glTexCoord2d(1, 0);
                    Gl.glVertex2d(EndRightAA[0].ToDouble(), EndRightAA[1].ToDouble());
                    Gl.glVertex2d(StartRightAA[0].ToDouble(), StartRightAA[1].ToDouble());

                    // start edge
                    EdgeWith = 20;
                    Gl.glTexCoord2d(0, 0);
                    Gl.glVertex2d(StartLeft[0].ToDouble(), StartLeft[1].ToDouble());
                    Gl.glVertex2d(StartRight[0].ToDouble(), StartRight[1].ToDouble());
                    Gl.glTexCoord2d(1, 0);
                    Gl.glVertex2d(StartRightAA[0].ToDouble(), StartRightAA[1].ToDouble());
                    Gl.glVertex2d(StartLeftAA[0].ToDouble(), StartLeftAA[1].ToDouble());

                    // end edge
                    Gl.glTexCoord2d(0, 0);
                    Gl.glVertex2d(EndLeft[0].ToDouble(), EndLeft[1].ToDouble());
                    Gl.glVertex2d(EndRight[0].ToDouble(), EndRight[1].ToDouble());
                    Gl.glTexCoord2d(1, 0);
                    Gl.glVertex2d(EndRightAA[0].ToDouble(), EndRightAA[1].ToDouble());
                    Gl.glVertex2d(EndLeftAA[0].ToDouble(), EndLeftAA[1].ToDouble());
                }
                Gl.glEnd();
            }

            RendererOpenGL<T>.PopOrthoProjection();

            Gl.glPopAttrib();
        }

        void DrawAATest()
        {
            PathStorage<T> PolgonToDraw = new PathStorage<T>();
            T Angle = M.Zero<T>();
            bool DrawTrinagle = true;
            if (DrawTrinagle)
            {
                PolgonToDraw.MoveTo(Angle.Cos(), Angle.Sin());
                Angle.AddEquals(120.0 / 180.0 * Math.PI);
                PolgonToDraw.LineTo(Angle.Cos(), Angle.Sin());
                Angle.AddEquals(120.0 / 180.0 * Math.PI);
                PolgonToDraw.LineTo(Angle.Cos(), Angle.Sin());
                Angle.AddEquals(120.0 / 180.0 * Math.PI);
                //Triangle.line_to(Math.Cos(Angle), Math.Sin(Angle));
                PolgonToDraw.ClosePolygon();
            }
            else
            {
                PolgonToDraw.MoveTo(M.Zero<T>(), M.Zero<T>());
                PolgonToDraw.LineTo(M.One<T>(), M.Zero<T>());
                PolgonToDraw.LineTo(M.One<T>(), M.One<T>());
                PolgonToDraw.LineTo(M.Zero<T>(), M.One<T>());
                PolgonToDraw.ClosePolygon();
            }

            IAffineTransformMatrix<T> tran = MatrixFactory<T>.NewScaling(VectorDimension.Two, M.New<T>(80));
            tran.RotateAlong(MatrixFactory<T>.CreateVector2D(M.Zero<T>(), M.Zero<T>()), Math.PI / 8);
            tran.Translate(MatrixFactory<T>.CreateVector2D(M.New<T>(500), M.New<T>(100)));

            ConvTransform<T> TransformedPolygon = new ConvTransform<T>(PolgonToDraw, tran);

            ((RendererOpenGL<T>)GetRenderer()).m_ForceTexturedEdgeAntiAliasing = true;
            GetRenderer().Render(TransformedPolygon, new RGBA_Bytes(0, 0, 0));

            Ellipse<T> testEllipse = new Ellipse<T>(M.New<T>(300), M.New<T>(250), M.New<T>(60), M.New<T>(60));
            GetRenderer().Render(testEllipse, new RGBA_Bytes(205, 23, 12, 120));
            ((RendererOpenGL<T>)GetRenderer()).m_ForceTexturedEdgeAntiAliasing = false;

            //conv_stroke OutLine = new conv_stroke(TransformedPolygon);
            //OutLine.width(2);
            //conv_transform TransformedOutLine = new conv_transform(OutLine, Affine.NewTranslation(100, 0));
            //GetRenderer().Render(TransformedOutLine, new RGBA_Bytes(0, 0, 0));

            //conv_transform TransformedOutLine2 = new conv_transform(OutLine, Affine.NewScaling(6) * Affine.NewTranslation(200, 0));
            //conv_stroke OutLineOutLine = new conv_stroke(TransformedOutLine2);
            //GetRenderer().Render(OutLineOutLine, new RGBA_Bytes(0, 0, 0));
        }

        public override void OnDraw()
        {
            //ShowFrameRate = false;
            DrawGLScene();
            DrawAATest();
            //DrawLine(new Vector2D(20, 20), new Vector2D(20, 400));
            //DrawLine(new Vector2D(40, 40), m_MousePos, true);
            //DrawLine(new Vector2D(40, 140), m_MousePos + new Vector2D(0, 100), false);
            base.OnDraw();
        }

        public override void OnMouseMove(AGG.UI.MouseEventArgs mouseEvent)
        {
            m_MousePos[0] = M.New<T>(mouseEvent.X);
            m_MousePos[1] = M.New<T>(mouseEvent.Y);
            base.OnMouseMove(mouseEvent);
        }

        public override void OnUpdate(double NumSecondsPassed)
        {
            rtri += 0.2f;                                                       // Increase The Rotation Variable For The Triangle ( NEW )
            rquad -= 0.15f;                                                     // Decrease The Rotation Variable For The Quad ( NEW )
            base.OnUpdate(NumSecondsPassed);
        }

        public static void StartDemo()
        {
            NeHeLesson5<T> app = new NeHeLesson5<T>(PixelFormats.pix_format_rgba32, ERenderOrigin.OriginBottomLeft);
            app.Caption = "AGG Example. NeHe Lesson 5 (using a GL like interface)";

#if USE_OPENGL
            if (app.init(640, 480, (uint)WindowFlags.UseOpenGL))
#else
            if (app.init(640, 480, (uint)(WindowFlags.None)))
#endif
            {
                app.run();
            }
        }


    };


    public static class App
    {
        [STAThread]
        public static void Main(string[] args)
        {
            NeHeLesson5<DoubleComponent>.StartDemo();
        }
    }
}
