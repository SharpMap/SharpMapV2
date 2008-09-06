using System;
using System.Collections.Generic;
using AGG.Color;
using AGG.PixelFormat;
using AGG.Rasterizer;
using AGG.Rendering;
using AGG.Scanline;
using AGG.Transform;
using AGG.VertexSource;
using NPack.Interfaces;
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
using Tesselate;
using NPack;

namespace AGG
{
    public abstract class VertexCachedTesselator<T> : Tesselator<T>
           where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public abstract void AddVertex(T x, T y);
    }

    public class RenderToGLTesselator<T> : VertexCachedTesselator<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        List<AddedVertex> m_Vertices = new List<AddedVertex>();

        internal class AddedVertex
        {
            internal AddedVertex(T x, T y)
            {
                m_X = x;
                m_Y = y;
            }
            internal T m_X;
            internal T m_Y;
        };

        public RenderToGLTesselator()
        {
            callBegin += new Tesselator<T>.CallBeginDelegate(BeginCallBack);
            callEnd += new Tesselator<T>.CallEndDelegate(EndCallBack);
            callVertex += new Tesselator<T>.CallVertexDelegate(VertexCallBack);
            //callEdgeFlag += new Tesselator.CallEdgeFlagDelegate(EdgeFlagCallBack);
            callCombine += new Tesselator<T>.CallCombineDelegate(CombineCallBack);
        }

        public override void BeginPolygon()
        {
            m_Vertices.Clear();

            base.BeginPolygon();
        }

        public void BeginCallBack(Tesselator<T>.TriangleListType type)
        {
            switch (type)
            {
                case Tesselator<T>.TriangleListType.Triangles:
                    Gl.glBegin(Gl.GL_TRIANGLES);
                    break;

                case Tesselator<T>.TriangleListType.TriangleFan:
                    Gl.glBegin(Gl.GL_TRIANGLE_FAN);
                    break;

                case Tesselator<T>.TriangleListType.TriangleStrip:
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
            Gl.glVertex2d(m_Vertices[index].m_X.ToDouble(), m_Vertices[index].m_Y.ToDouble());
        }

        public void EdgeFlagCallBack(bool IsEdge)
        {
            Gl.glEdgeFlag(IsEdge);
        }

        public void CombineCallBack(T[] coords3, int[] data4,
            T[] weight4, out int outData)
        {
            outData = AddVertex(coords3[0], coords3[1], false);
        }

        public override void AddVertex(T x, T y)
        {
            AddVertex(x, y, true);
        }

        public int AddVertex(T x, T y, bool passOnToTesselator)
        {
            int index = m_Vertices.Count;
            m_Vertices.Add(new AddedVertex(x, y));
            T[] coords = new T[3];
            coords[0] = x;
            coords[1] = y;
            if (passOnToTesselator)
            {
                AddVertex(coords, index);
            }
            return index;
        }
    };

    public class RenderToGLWithUVTesselator<T> : Tesselator<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        List<AddedVertex> m_Vertices = new List<AddedVertex>();

        internal class AddedVertex
        {
            internal AddedVertex(T x, T y, T u, T v)
            {
                m_X = x;
                m_Y = y;
                m_U = u;
                m_V = v;
            }
            internal T m_X;
            internal T m_Y;
            internal T m_U;
            internal T m_V;
        };

        public RenderToGLWithUVTesselator()
        {
            callBegin += new Tesselator<T>.CallBeginDelegate(BeginCallBack);
            callEnd += new Tesselator<T>.CallEndDelegate(EndCallBack);
            callVertex += new Tesselator<T>.CallVertexDelegate(VertexCallBack);
            callCombine += new Tesselator<T>.CallCombineDelegate(CombineCallBack);
        }

        public override void BeginPolygon()
        {
            m_Vertices.Clear();
            base.BeginPolygon();
        }

        public void BeginCallBack(Tesselator<T>.TriangleListType type)
        {
            switch (type)
            {
                case Tesselator<T>.TriangleListType.Triangles:
                    Gl.glBegin(Gl.GL_TRIANGLES);
                    break;

                case Tesselator<T>.TriangleListType.TriangleFan:
                    Gl.glBegin(Gl.GL_TRIANGLE_FAN);
                    break;

                case Tesselator<T>.TriangleListType.TriangleStrip:
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
            Gl.glTexCoord2d(m_Vertices[index].m_U.ToDouble(), m_Vertices[index].m_V.ToDouble());
            Gl.glVertex2d(m_Vertices[index].m_X.ToDouble(), m_Vertices[index].m_Y.ToDouble());
        }

        public void EdgeFlagCallBack(bool IsEdge)
        {
            throw new Exception();
            //Gl.glEdgeFlag(IsEdge);
        }

        public void CombineCallBack(T[] coords3, int[] data4,
            T[] weight4, out int outData)
        {
            T u = m_Vertices[data4[0]].m_U.Multiply(weight4[0])
                .Add(
                    m_Vertices[data4[1]].m_U.Multiply(weight4[1])
                ).Add(
                    m_Vertices[data4[2]].m_U.Multiply(weight4[2])
                ).Add(
                    m_Vertices[data4[3]].m_U.Multiply(weight4[3])
                );
            T v = M.Zero<T>();
            outData = AddVertex(coords3[0], coords3[1], u, v, false);
        }

        public void AddVertex(T x, T y, T u, T v)
        {
            AddVertex(x, y, u, v, true);
        }

        public int AddVertex(T x, T y, T u, T v, bool passOnToTesselator)
        {
            int index = m_Vertices.Count;
            m_Vertices.Add(new AddedVertex(x, y, u, v));
            T[] coords = new T[3];
            coords[0] = x;
            coords[1] = y;
            if (passOnToTesselator)
            {
                AddVertex(coords, index);
            }
            return index;
        }
    };

    public class GetEdgesTesselator<T> : VertexCachedTesselator<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public class AddedVertex
        {
            internal AddedVertex(T x, T y)
            {
                m_X = x;
                m_Y = y;
            }
            internal T m_X;
            internal T m_Y;
        };

        public List<AddedVertex> m_EdgesVertices = new List<AddedVertex>();
        public List<List<int>> m_CountourIndices = new List<List<int>>();

        public GetEdgesTesselator()
        {
            callBegin += new Tesselator<T>.CallBeginDelegate(BeginCallBack);
            callEnd += new Tesselator<T>.CallEndDelegate(EndCallBack);
            callVertex += new Tesselator<T>.CallVertexDelegate(VertexCallBack);
            callCombine += new Tesselator<T>.CallCombineDelegate(CombineCallBack);
            BoundaryOnly = true;
        }

        public override void BeginPolygon()
        {
            m_EdgesVertices.Clear();
            m_CountourIndices.Clear();
            m_CountourIndices.Add(new List<int>());

            base.BeginPolygon();
        }

        public void BeginCallBack(Tesselator<T>.TriangleListType type)
        {
            if (type != TriangleListType.LineLoop)
            {
                throw new Exception("This only does boundaries.  Why are you getting anything other athn LineLoops?");
            }
        }

        public void EndCallBack()
        {
        }

        public void VertexCallBack(int index)
        {
            m_CountourIndices[m_CountourIndices.Count - 1].Add(index);
        }

        public void CombineCallBack(T[] coords3, int[] data4,
            T[] weight4, out int outData)
        {
            outData = AddVertex(coords3[0], coords3[1], false);
        }

        public override void AddVertex(T x, T y)
        {
            AddVertex(x, y, true);
        }

        public int AddVertex(T x, T y, bool passOnToTesselator)
        {
            int index = m_EdgesVertices.Count;
            m_EdgesVertices.Add(new AddedVertex(x, y));
            T[] coords = new T[3];
            coords[0] = x;
            coords[1] = y;
            if (passOnToTesselator)
            {
                AddVertex(coords, index);
            }
            return index;
        }
    };

    public class RendererOpenGL<T> : RendererBase<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public bool m_ForceTexturedEdgeAntiAliasing;
        RenderToGLTesselator<T> m_RenderNowTesselator = new RenderToGLTesselator<T>();

        public RendererOpenGL(IPixelFormat PixelFormat, RasterizerScanlineAA<T> Rasterizer)
            : base(PixelFormat, Rasterizer)
        {
            TextPath = new GsvText<T>();
            StrockedText = new ConvStroke<T>(TextPath);
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
        void SendShapeToTeselator(VertexCachedTesselator<T> teselator, IVertexSource<T> vertexSource)
        {
            teselator.BeginPolygon();

            uint PathAndFlags = 0;
            T x, y;
            bool haveBegunContour = false;
            while (!Path.IsStop(PathAndFlags = vertexSource.Vertex(out x, out y)))
            {
                if (Path.IsClose(PathAndFlags)
                    || (haveBegunContour && Path.IsMoveTo(PathAndFlags)))
                {
                    teselator.EndContour();
                    haveBegunContour = false;
                }

                if (!Path.IsClose(PathAndFlags))
                {
                    if (!haveBegunContour)
                    {
                        teselator.BeginContour();
                        haveBegunContour = true;
                    }

                    teselator.AddVertex(x, y);
                }
            }

            if (haveBegunContour)
            {
                teselator.EndContour();
            }

#if use_timers
            OpenGLEndPolygonTimer.Start();
#endif
            teselator.EndPolygon();
#if use_timers
            OpenGLEndPolygonTimer.Stop();
#endif
        }

        int m_AATextureHandle = -1;
        void CheckLineImageCache()
        {
            if (m_AATextureHandle == -1)
            {
                // Create the texture handle and display list handle
                int[] textureHandle = new int[1];
                Gl.glGenTextures(1, textureHandle);
                m_AATextureHandle = textureHandle[0];

                // Set up some texture parameters for openGL
                Gl.glBindTexture(Gl.GL_TEXTURE_2D, m_AATextureHandle);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);

                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_CLAMP_TO_EDGE);
                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_CLAMP_TO_EDGE);

                byte[] hardwarePixelBuffer = new byte[8];
                hardwarePixelBuffer[0] = hardwarePixelBuffer[1] = hardwarePixelBuffer[2] = hardwarePixelBuffer[3] = 255;
                hardwarePixelBuffer[4] = hardwarePixelBuffer[5] = hardwarePixelBuffer[6] = 255;
                hardwarePixelBuffer[7] = 0;

                // Create the texture
                Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGBA, 2, 1,
                    0, Gl.GL_RGBA, Gl.GL_UNSIGNED_BYTE, hardwarePixelBuffer);
            }
        }

        void DrawAAShape(IVertexSource<T> vertexSource)
        {
            m_RenderNowTesselator.callEdgeFlag += new Tesselator<T>.CallEdgeFlagDelegate(m_RenderNowTesselator.EdgeFlagCallBack);

            SendShapeToTeselator(m_RenderNowTesselator, vertexSource);

            m_RenderNowTesselator.callEdgeFlag -= new Tesselator<T>.CallEdgeFlagDelegate(m_RenderNowTesselator.EdgeFlagCallBack);

            /*
            // Get the monotone outlines for the shape (push it through with edges only set).
            GetEdgesTesselator edgesTesselator = new GetEdgesTesselator();
            SendShapeToTeselator(edgesTesselator, vertexSource);

            // Get the outline and inside of each monotone shape.
            PathStorage PolgonToDraw = new PathStorage();
            List<int> curIntList = edgesTesselator.m_CountourIndices[0];
            List<GetEdgesTesselator.AddedVertex> vertexList = edgesTesselator.m_EdgesVertices;
            for (int i = 0; i < curIntList.Count; i++)
            {
                if(i==0)
                    PolgonToDraw.move_to(vertexList[curIntList[i]].m_X, vertexList[curIntList[i]].m_Y);
                else
                    PolgonToDraw.line_to(vertexList[curIntList[i]].m_X, vertexList[curIntList[i]].m_Y);
            }
            PolgonToDraw.close_polygon();

            conv_stroke OutLine = new conv_stroke(PolgonToDraw);
            OutLine.width(2);
            
            // Render the outside edge with a border texture to get AA through bilinear filtering.
            List<GetEdgesTesselator.AddedVertex> outsideEdge = new List<GetEdgesTesselator.AddedVertex>();
            List<GetEdgesTesselator.AddedVertex> insideEdge = new List<GetEdgesTesselator.AddedVertex>();

            uint PathAndFlags = 0;
            double x, y;
            bool haveBegunContour = false;
            int edgeIndex = 0;
            OutLine.rewind(0);
            while (!Path.is_stop(PathAndFlags = OutLine.vertex(out x, out y)))
            {
                if (Path.is_close(PathAndFlags)
                    || (haveBegunContour && Path.is_move_to(PathAndFlags)))
                {
                    haveBegunContour = false;
                    edgeIndex++;
                }

                if (!Path.is_close(PathAndFlags))
                {
                    if (edgeIndex == 1)
                    {
                        insideEdge.Add(new GetEdgesTesselator.AddedVertex(x, y));
                    }
                    else
                    {
                        outsideEdge.Add(new GetEdgesTesselator.AddedVertex(x, y));
                    }
                }
            }

            //Gl.glColor4d(0, 0, 1, .5);
            CheckLineImageCache();
            Gl.glEnable(Gl.GL_TEXTURE_2D);
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, m_AATextureHandle);
            RenderToGLWithUVTesselator withUV = new RenderToGLWithUVTesselator();
            {
                withUV.BeginPolygon();
                withUV.BeginContour();
                for (int i = 0; i < insideEdge.Count; i++)
                {
                    withUV.AddVertex(insideEdge[i].m_X, insideEdge[i].m_Y, 0, 0);
                }
                withUV.EndContour();
                withUV.BeginContour();
                for (int i = 0; i < outsideEdge.Count; i++)
                {
                    withUV.AddVertex(outsideEdge[i].m_X, outsideEdge[i].m_Y, 1, 0);
                }
                withUV.EndContour();
                withUV.EndPolygon();
            }
            Gl.glDisable(Gl.GL_TEXTURE_2D);

            // Render the inside polygon normally to let GL draw in the filled area.
            //Gl.glColor4d(1, 0, 0, .5);
            m_RenderNowTesselator.BeginPolygon();
            m_RenderNowTesselator.BeginContour();
            for (int i = 0; i < insideEdge.Count; i++)
            {
                m_RenderNowTesselator.AddVertex(insideEdge[i].m_X, insideEdge[i].m_Y);
            }
            m_RenderNowTesselator.EndContour();
            m_RenderNowTesselator.EndPolygon();
             */
        }

        public override void Render(IVertexSource<T> vertexSource, uint pathIndexToRender, RGBA_Bytes colorBytes)
        {
#if use_timers
            OpenGLRenderTimer.Start();
#endif
            PushOrthoProjection();

            vertexSource.Rewind(pathIndexToRender);

            RGBA_Doubles color = colorBytes.GetAsRGBA_Doubles();

            Gl.glColor4d(color.R, color.G, color.B, color.A);

            IAffineTransformMatrix<T> transform = GetTransform();
            if (!transform.Equals(MatrixFactory<T>.NewIdentity(VectorDimension.Two)))
            {
                vertexSource = new ConvTransform<T>(vertexSource, transform);
            }

            if (m_ForceTexturedEdgeAntiAliasing)
            {
                DrawAAShape(vertexSource);
            }
            else
            {
                SendShapeToTeselator(m_RenderNowTesselator, vertexSource);
            }

            PopOrthoProjection();
#if use_timers
            OpenGLRenderTimer.Stop();
#endif
        }

        public override void Clear(IColorType color)
        {
            RGBA_Doubles colorDoubles = color.GetAsRGBA_Doubles();
            Gl.glClearColor((float)colorDoubles.R, (float)colorDoubles.G, (float)colorDoubles.B, (float)colorDoubles.A);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
        }
    }
}
