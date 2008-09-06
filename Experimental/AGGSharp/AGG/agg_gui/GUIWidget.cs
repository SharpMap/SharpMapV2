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
using System;
using System.Collections.Generic;
using AGG.Color;
using AGG.Rendering;
using AGG.Transform;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;

namespace AGG.UI
{
    public class GUIWidget<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private bool m_Visible = true;
        internal RectDouble<T> m_Bounds = new RectDouble<T>();
        private GUIWidget<T> m_Parrent = null;

        protected IAffineTransformMatrix<T> m_Transform = MatrixFactory<T>.NewIdentity(VectorDimension.Two);
        public List<GUIWidget<T>> m_Children = new List<GUIWidget<T>>();

        private bool m_ContainsFocus = false;

        //private int m_CurrentChildIndex;

        public GUIWidget()
        {
        }

        public virtual IVector<T> Location
        {
            get
            {
                //Vector2D thisOrigin =  Affine.CreateVector(0, 0);
                //Parrent.GetTransform().Inverse.Transform(ref thisOrigin);
                //return thisOrigin;

                IVector<T> thisOrigin = MatrixFactory<T>.CreateVector2D(M.Zero<T>(), M.Zero<T>());
                Parrent.GetTransform().Inverse.Transform(ref thisOrigin);
                return thisOrigin;

            }
            set
            {
                //m_Transform.translate();
                Bounds = new RectDouble<T>(value[0], value[1], value[0].Add(Bounds.Width), value[1].Add(Bounds.Height));
            }
        }

        public virtual RectDouble<T> Bounds
        {
            get
            {
                return m_Bounds;
            }
            set
            {
                m_Bounds = value;
            }
        }

        public bool Visible
        {
            get
            {
                return m_Visible;

            }
            set
            {
                m_Visible = value;
            }
        }

        public GUIWidget<T> Parrent
        {
            get
            {
                return m_Parrent;
            }
        }

        public T Height
        {
            get
            {
                return Bounds.Height;
            }
        }

        public T Width
        {
            get
            {
                return Bounds.Width;
            }
        }

        public virtual RendererBase<T> GetRenderer()
        {
            if (m_Parrent != null)
            {
                return m_Parrent.GetRenderer();
            }

            return null;
        }

        public void AddChild(GUIWidget<T> child)
        {
            child.m_Parrent = this;
            m_Children.Add(child);
            child.OnParentChanged();
        }

        public void RemoveChild(GUIWidget<T> child)
        {
            child.m_Parrent = null;
            m_Children.Remove(child);
            child.OnParentChanged();
        }

        public virtual bool InRect(T x, T y)
        {
            PointToClient(ref x, ref y);
            if (Bounds.HitTest(x, y))
            {
                return true;
            }
            return false;
        }

        public virtual void Invalidate()
        {
            Parrent.Invalidate(Bounds);
        }

        public virtual void Invalidate(RectDouble<T> rectToInvalidate)
        {
            Parrent.Invalidate(rectToInvalidate);
        }

        protected void Unfocus()
        {
            m_ContainsFocus = false;
            foreach (GUIWidget<T> child in m_Children)
            {
                child.Unfocus();
            }
        }

        public void Focus()
        {
            if (m_Parrent != null)
            {
                m_Parrent.Focus();
            }

            // make sure none of the children have focus.
            Unfocus();

            // now say that we do
            m_ContainsFocus = true;
        }

        public bool CanFocus
        {
            get
            {
                return true;
            }
        }

        public bool Focused
        {
            get
            {
                if (ContainsFocus)
                {
                    foreach (GUIWidget<T> child in m_Children)
                    {
                        if (child.ContainsFocus)
                        {
                            return false;
                        }
                    }

                    // we contain focus and none of our children do so we are focused.
                    return true;
                }

                return false;
            }
        }

        public bool ContainsFocus
        {
            get
            {
                return m_ContainsFocus;
            }
        }

        protected GUIWidget<T> GetChildContainingFocus()
        {
            foreach (GUIWidget<T> child in m_Children)
            {
                if (child.ContainsFocus)
                {
                    return child;
                }
            }

            return null;
        }

        public virtual void OnParentChanged()
        {

        }

        public virtual void OnDraw()
        {
            for (int i = 0; i < m_Children.Count; i++)
            {
                GUIWidget<T> child = m_Children[i];
                if (child.Visible)
                {
                    GetRenderer().PushTransform();
                    //Affine transform = GetRenderer().GetTransform();
                    //transform *= child.GetTransform();

                    IAffineTransformMatrix<T> transform = GetRenderer().GetTransform();
                    transform = MatrixFactory<T>.CreateAffine(transform.Multiply(child.GetTransform()));

                    //transform *= child.GetTransform(); 

                    GetRenderer().SetTransform(transform);
                    child.OnDraw();
#if false
                    if (Focused)
                    {
                        RoundedRect rect = new RoundedRect(-5, -5, 5, 5, 0);
                        GetRenderer().Render(rect, new RGBA_Bytes(1.0, 0, 0, .5));
                    }
#endif

                    GetRenderer().PopTransform();
                }
            }
        }

        public virtual void OnClosed()
        {

        }

        public void PointToClient(ref T screenPointX, ref T screenPointY)
        {
            GUIWidget<T> curGUIWidget = this;
            while (curGUIWidget != null)
            {
                curGUIWidget.GetTransform().Inverse.Transform(ref screenPointX, ref screenPointY);
                curGUIWidget = curGUIWidget.Parrent;
            }
        }

        public void PointToClient(ref IVector<T> screenPoint)
        {
            T x = screenPoint[0], y = screenPoint[1];

            PointToClient(ref x, ref y);
            screenPoint = MatrixFactory<T>.CreateVector2D(x, y);
        }

        public void PointToScreen(ref IVector<T> clientPoint)
        {
            GUIWidget<T> prevGUIWidget = this;
            while (prevGUIWidget != null)
            {
                prevGUIWidget.GetTransform().Transform(ref clientPoint);
                prevGUIWidget = prevGUIWidget.Parrent;
            }
        }

        public void RectToScreen(ref RectDouble<T> clientRect)
        {
            GUIWidget<T> prevGUIWidget = this;
            while (prevGUIWidget != null)
            {
                prevGUIWidget.GetTransform().Transform(ref clientRect);
                prevGUIWidget = prevGUIWidget.Parrent;
            }
        }

        public virtual void OnMouseDown(MouseEventArgs mouseEvent)
        {
            int i = 0;
            foreach (GUIWidget<T> child in m_Children)
            {
                if (child.Visible)
                {
                    child.OnMouseDown(mouseEvent);
                    if (child.InRect(M.New<T>(mouseEvent.X), M.New<T>(mouseEvent.Y)))
                    {
                        if (child.CanFocus)
                        {
                            child.Focus();
                        }
                        return;
                    }
                }
                i++;
            }
        }

        public virtual void OnMouseMove(MouseEventArgs mouseEvent)
        {
            foreach (GUIWidget<T> child in m_Children)
            {
                if (child.Visible)
                {
                    child.OnMouseMove(mouseEvent);
                }
            }
        }

        public virtual void OnMouseUp(MouseEventArgs mouseEvent)
        {
            foreach (GUIWidget<T> child in m_Children)
            {
                if (child.Visible)
                {
                    child.OnMouseUp(mouseEvent);
                }
            }
        }

        public virtual void OnKeyPress(KeyPressEventArgs keyPressEvent)
        {
            GUIWidget<T> childWithFocus = GetChildContainingFocus();
            if (childWithFocus != null && childWithFocus.Visible)
            {
                childWithFocus.OnKeyPress(keyPressEvent);
            }
        }

        protected void FocusNext()
        {
            GUIWidget<T> childWithFocus = GetChildContainingFocus();
            for (int i = 0; i < m_Children.Count; i++)
            {
                GUIWidget<T> child = m_Children[i];
                if (child.Visible)
                {
                }
            }
        }

        protected void FocusPrevious()
        {

        }

        public virtual void OnKeyDown(KeyEventArgs keyEvent)
        {
            GUIWidget<T> childWithFocus = GetChildContainingFocus();
            if (childWithFocus != null && childWithFocus.Visible)
            {
                childWithFocus.OnKeyDown(keyEvent);
            }

            if (!keyEvent.Handled && keyEvent.KeyCode == Keys.Tab)
            {
                if (keyEvent.Shift)
                {
                    FocusPrevious();
                }
                else
                {
                    FocusNext();
                }
            }
        }

        public virtual void OnKeyUp(KeyEventArgs keyEvent)
        {
            GUIWidget<T> childWithFocus = GetChildContainingFocus();
            if (childWithFocus != null && childWithFocus.Visible)
            {
                childWithFocus.OnKeyUp(keyEvent);
            }
        }

        public bool SetChildCurrent(T x, T y)
        {
            for (int i = 0; i < m_Children.Count; i++)
            {
                GUIWidget<T> child = m_Children[i];
                if (child.Visible)
                {
                    if (child.InRect(x, y))
                    {
                        if (!child.Focused)
                        {
                            child.Focus();
                            return true;
                        }

                        return false;
                    }
                }
            }

            return false;
        }

        public IAffineTransformMatrix<T> GetTransform()
        {
            return m_Transform;
        }

        public void SetTransform(IAffineTransformMatrix<T> value)
        {
            m_Transform = value;
        }

        public T scale()
        {

            throw new NotImplementedException();
            //return GetTransform().GetScale(); 
        }
    };

    abstract public class SimpleVertexSourceWidget<T> : GUIWidget<T>, IVertexSource<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public SimpleVertexSourceWidget(T x1, T y1, T x2, T y2)
        {
            Bounds = new RectDouble<T>(x1, y1, x2, y2);
        }

        public abstract uint num_paths();
        public abstract void Rewind(uint path_id);
        public abstract uint Vertex(out T x, out T y);

        public virtual IColorType color(uint i) { return (IColorType)new RGBA_Doubles(); }

        public override void OnDraw()
        {
            RendererBase<T> rendererForSurfaceThisIsOn = Parrent.GetRenderer();

            for (uint i = 0; i < num_paths(); i++)
            {
                rendererForSurfaceThisIsOn.Render(this, i, color(i).GetAsRGBA_Bytes());
            }
            base.OnDraw();
        }

#if false
        public override void Render(Renderer renderer)
        {
            for (uint i = 0; i < num_paths(); i++)
            {
                renderer.Render(this, i, color(i).Get_rgba8());
            }
        }

        public void Render(IRasterizer rasterizer, IScanline scanline, IPixelFormat rendererBase)
        {
            uint i;
            for(i = 0; i < num_paths(); i++)
            {
                rasterizer.reset();
                rasterizer.add_path(this, i);
                Renderer.RenderSolid(rendererBase, rasterizer, scanline, color(i).Get_rgba8());
            }
        }
#endif
    }
}
