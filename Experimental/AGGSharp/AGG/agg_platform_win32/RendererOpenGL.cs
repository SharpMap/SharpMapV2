//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
using Tao.OpenGl;

using System;
using System.Collections.Generic;

using AGG.PixelFormat;
using AGG.VertexSource;
using AGG.Transform;

using Tesselate;

namespace AGG
{
    public class PolygonTesselator
    {
        internal class Vertex
        {
            internal Vertex(double x, double y)
            {
                m_X = x;
                m_Y = y;
            }
            internal double m_X;
            internal double m_Y;
        };
        List<Vertex> m_Vertices = new List<Vertex>();
        Tesselator m_Tesselator;

        public void BeginCallBack(Tesselator.TriangleListType type)
        {
            switch(type)
            {
                case Tesselator.TriangleListType.Triangles:
                    Gl.glBegin(Gl.GL_TRIANGLES);
                    break;

                case Tesselator.TriangleListType.TriangleFan:
                    Gl.glBegin(Gl.GL_TRIANGLE_FAN);
                    break;

                case Tesselator.TriangleListType.TriangleStrip:
                    Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
                    break;
            }
        }

        public void EndCallBack()
        {
            Gl.glEnd();
        }

        public void VertexCallBack(int index)
        {
            Gl.glVertex2d(m_Vertices[index].m_X, m_Vertices[index].m_Y);
        }

        public void EdgeFlagCallBack(bool IsEdge)
        {
            Gl.glEdgeFlag(IsEdge);
        }

        public void CombineCallBack(double[] coords3, int[] data4,
            double[] weight4, out int outData)
        {
            outData = AddVertex(coords3[0], coords3[1], false);
        }

        public PolygonTesselator()
        {
            m_Tesselator = new Tesselate.Tesselator();
            m_Tesselator.callBegin += new Tesselator.CallBeginDelegate(BeginCallBack);
            m_Tesselator.callEnd += new Tesselator.CallEndDelegate(EndCallBack);
            m_Tesselator.callVertex += new Tesselator.CallVertexDelegate(VertexCallBack);
            //m_Tesselator.callEdgeFlag += new Tesselator.CallEdgeFlagDelegate(EdgeFlagCallBack);
            m_Tesselator.callCombine += new Tesselator.CallCombineDelegate(CombineCallBack);
        }

        ~PolygonTesselator()
        {
        }

        public void BeginPolygon()
        {
            m_Vertices.Clear();
            m_Tesselator.BeginPolygon();
        }

        public void BeginContour()
        {
            m_Tesselator.BeginContour();
        }

        public void AddVertex(double x, double y)
        {
            AddVertex(x, y, true);
        }

        public int AddVertex(double x, double y, bool passOnToTesselator)
        {
            int index = m_Vertices.Count;
            m_Vertices.Add(new Vertex(x, y));
            double[] coords = new double[3];
            coords[0] = x;
            coords[1] = y;
            if (passOnToTesselator)
            {
                m_Tesselator.AddVertex(coords, index);
            }
            return index;
        }

        public void EndContour()
        {
            m_Tesselator.EndContour();
        }

        public void EndPolygon()
        {
            m_Tesselator.EndPolygon();
        }
    };

    public class RendererOpenGL : RendererBase
    {
        PolygonTesselator m_Tesselator = new PolygonTesselator();

        public RendererOpenGL(IPixelFormat PixelFormat, rasterizer_scanline_aa Rasterizer)
            : base(PixelFormat, Rasterizer)
        {
            TextPath = new gsv_text();
            StrockedText = new conv_stroke(TextPath);
        }

        public override IScanlineCache ScanlineCache
        {
            get { return null; }
            set { throw new Exception("There is no scanline cache on a GL surface."); }
        }

        public static void PushOrthoProjection()
        {
            Gl.glPushAttrib(Gl.GL_TRANSFORM_BIT | Gl.GL_ENABLE_BIT);

            Gl.glEnable(Gl.GL_BLEND);

            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);

            int[] viewport = new int[4];
            Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);            
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();
            Gl.glOrtho(viewport[0], viewport[2], viewport[1], viewport[3], 0, 1);
            
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPushMatrix();
            Gl.glLoadIdentity();

            viewport = null;
        }

        public static void PopOrthoProjection()
        {
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glPopMatrix();
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();
            Gl.glPopAttrib();
        }

#if use_timers
        static CNamedTimer OpenGLRenderTimer = new CNamedTimer("OpenGLRenderTimer");
        static CNamedTimer OpenGLEndPolygonTimer = new CNamedTimer("OpenGLEndPolygonTimer");        
#endif
        public override void Render(IVertexSource vertexSource, uint pathIndexToRender, RGBA_Bytes colorBytes)
        {
#if use_timers
            OpenGLRenderTimer.Start();
#endif
            PushOrthoProjection();

            vertexSource.rewind(pathIndexToRender);

            RGBA_Doubles color = colorBytes.GetAsRGBA_Doubles();

            Gl.glColor4d(color.m_r, color.m_g, color.m_b, color.m_a);

            m_Tesselator.BeginPolygon();

            uint PathAndFlags = 0;
            double x, y;
            bool haveBegunContour = false;
            while (!Path.is_stop(PathAndFlags = vertexSource.vertex(out x, out y)))
            {
                if (Path.is_close(PathAndFlags)
                    || (haveBegunContour && Path.is_move_to(PathAndFlags)))
                {
                    m_Tesselator.EndContour();
                    haveBegunContour = false;
                }

                if(!Path.is_close(PathAndFlags))
                {
                    if (!haveBegunContour)
                    {
                        m_Tesselator.BeginContour();
                        haveBegunContour = true;
                    }

                    m_Tesselator.AddVertex(x, y);
                }
            }

            if (haveBegunContour)
            {
                m_Tesselator.EndContour();
            }

#if use_timers
            OpenGLEndPolygonTimer.Start();
#endif
            m_Tesselator.EndPolygon();
#if use_timers
            OpenGLEndPolygonTimer.Stop();
#endif

            PopOrthoProjection();
#if use_timers
            OpenGLRenderTimer.Stop();
#endif
        }

        public override void Clear(IColorType color)
        {
            RGBA_Doubles colorDoubles = color.GetAsRGBA_Doubles();
            Gl.glClearColor((float)colorDoubles.m_r, (float)colorDoubles.m_g, (float)colorDoubles.m_b, (float)colorDoubles.m_a);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
        }
    }
}
