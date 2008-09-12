
using System;
/*
 *	Portions of this file are  © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 *
 *  Original notices below.
 * 
 */
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
using System.Collections.Generic;
using AGG.Color;
using AGG.PixelFormat;
using AGG.Rasterizer;
using AGG.Scanline;
using AGG.Transform;
using AGG.VertexSource;
using NPack.Interfaces;

namespace AGG.Rendering
{
    public interface IStyleHandler
    {
        bool IsSolid(uint style);
        RGBA_Bytes Color(uint style);
        unsafe void GenerateSpan(RGBA_Bytes* span, int x, int y, uint len, uint style);
    };

    public abstract class RendererBase<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        const int CoverFull = 255;
        protected IPixelFormat m_PixelFormat;
        protected GsvText<T> TextPath;
        protected ConvStroke<T> StrockedText;
        protected Stack<IAffineTransformMatrix<T>> m_AffineTransformStack = new Stack<IAffineTransformMatrix<T>>();
        protected RasterizerScanlineAA<T> m_Rasterizer;

        public RendererBase()
        {
            TextPath = new GsvText<T>();
            StrockedText = new ConvStroke<T>(TextPath);
            m_AffineTransformStack.Push(MatrixFactory<T>.NewIdentity(VectorDimension.Two));
        }

        public RendererBase(IPixelFormat PixelFormat, RasterizerScanlineAA<T> Rasterizer)
            : this()
        {
            Initialize(PixelFormat, Rasterizer);
        }

        public void Initialize(IPixelFormat PixelFormat, RasterizerScanlineAA<T> Rasterizer)
        {
            m_PixelFormat = PixelFormat;
            m_Rasterizer = Rasterizer;
        }

        public IAffineTransformMatrix<T> PopTransform()
        {
            if (m_AffineTransformStack.Count == 1)
            {
                throw new System.Exception("You cannot remove the last Transform from the stack.");
            }

            return m_AffineTransformStack.Pop();
        }

        public void PushTransform()
        {
            if (m_AffineTransformStack.Count > 1000)
            {
                throw new System.Exception("You seem to be leaking transforms.  You should be poping some of them at some point.");
            }

            m_AffineTransformStack.Push(m_AffineTransformStack.Peek());
        }

        public IAffineTransformMatrix<T> GetTransform()
        {
            return m_AffineTransformStack.Peek();
        }

        public void SetTransform(IAffineTransformMatrix<T> value)
        {
            m_AffineTransformStack.Pop();
            m_AffineTransformStack.Push(value);
        }

        public RasterizerScanlineAA<T> Rasterizer
        {
            get { return m_Rasterizer; }
        }

        public abstract IScanlineCache ScanlineCache
        {
            get;
            set;
        }

        public IPixelFormat PixelFormat
        {
            get
            {
                return m_PixelFormat;
            }
        }

        public abstract void Render(IVertexSource<T> vertexSource, uint pathIndexToRender, RGBA_Bytes colorBytes);

        public void Render(IVertexSource<T> vertexSource, RGBA_Bytes[] colorArray, uint[] pathIdArray, uint numPaths)
        {
            for (uint i = 0; i < numPaths; i++)
            {
                Render(vertexSource, pathIdArray[i], colorArray[i]);
            }
        }

        public void Render(IVertexSource<T> vertexSource, RGBA_Bytes color)
        {
            Render(vertexSource, 0, color);
        }

        public abstract void Clear(IColorType color);

        public void DrawString(string text, T x, T y)
        {
            TextPath.SetFontSize(M.New<T>(10));
            TextPath.StartPoint(x, y);
            TextPath.Text = text;
            Render(StrockedText, new RGBA_Bytes(0, 0, 0, 255));
        }

        public void Line(IVector<T> Start, IVector<T> End, RGBA_Bytes color)
        {
            Line(Start[0], Start[1], End[0], End[1], color);
        }

        public void Line(T x1, T y1, T x2, T y2, RGBA_Bytes color)
        {
            PathStorage<T> m_LinesToDraw = new PathStorage<T>();
            m_LinesToDraw.RemoveAll();
            m_LinesToDraw.MoveTo(x1, y1);
            m_LinesToDraw.LineTo(x2, y2);
            ConvStroke<T> StrockedLineToDraw = new ConvStroke<T>(m_LinesToDraw);
            Render(StrockedLineToDraw, color);
        }
    }
}
