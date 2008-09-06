//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2007 Lars Brubaker
//                  larsbrubaker@gmail.com
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
using System;
using AGG.Color;
using AGG.Transform;
using AGG.VertexSource;
using NPack.Interfaces;
using NPack;

namespace AGG.UI
{
    public class WindowWidget<T> : GUIWidget<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public WindowWidget(RectDouble<T> InBounds)
        {
            SetTransform(MatrixFactory<T>.NewTranslation(InBounds.Left, InBounds.Bottom));
            Bounds = new RectDouble<T>(M.Zero<T>(), M.Zero<T>(), InBounds.Width, InBounds.Height);
        }

        public override void OnDraw()
        {
            RectDouble<T> boundsRect = Bounds;
            RoundedRect<T> roundRect = new RoundedRect<T>(Bounds, M.Zero<T>());
            RectToScreen(ref boundsRect);
            GetRenderer().Rasterizer.SetVectorClipBox(boundsRect.Left, boundsRect.Top, boundsRect.Right, boundsRect.Bottom);
            GetRenderer().Render(roundRect, new RGBA_Bytes(0, 0, 0, 30));
            base.OnDraw();
            GetRenderer().Rasterizer.SetVectorClipBox(M.Zero<T>(), M.Zero<T>(), M.New<T>(640), M.New<T>(480));
        }

        override public void OnMouseDown(MouseEventArgs mouseEvent)
        {
            base.OnMouseDown(mouseEvent);
        }

        override public void OnMouseUp(MouseEventArgs mouseEvent)
        {
            base.OnMouseUp(mouseEvent);
        }

        override public void OnMouseMove(MouseEventArgs mouseEvent)
        {
            //if(Capture || 
            base.OnMouseMove(mouseEvent);
        }
    };

    public class DragBarWidget<T> : GUIWidget<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        bool m_MouseDownOnBar = false;
        IVector<T> m_DownPosition;

        public DragBarWidget(RectDouble<T> InBounds)
        {
            SetTransform(MatrixFactory<T>.NewTranslation(InBounds.Left, InBounds.Bottom));
            Bounds = new RectDouble<T>(M.Zero<T>(), M.Zero<T>(), InBounds.Width, InBounds.Height);
        }

        protected bool MouseDownOnBar
        {
            get { return m_MouseDownOnBar; }
            set { m_MouseDownOnBar = value; }
        }

        public override void OnDraw()
        {
            RoundedRect<T> roundRect = new RoundedRect<T>(Bounds, M.Zero<T>());
            GetRenderer().Render(roundRect, new RGBA_Bytes(0, 0, 0, 30));
            base.OnDraw();
        }

        override public void OnMouseDown(MouseEventArgs mouseEvent)
        {
            if (InRect(M.New<T>(mouseEvent.X), M.New<T>(mouseEvent.Y)))
            {
                MouseDownOnBar = true;
                mouseEvent.Handled = true;
                IVector<T> mouseRelClient = MatrixFactory<T>.CreateVector2D(M.New<T>(mouseEvent.X), M.New<T>(mouseEvent.Y));
                PointToClient(ref mouseRelClient);
                m_DownPosition = mouseRelClient;
            }
            else
            {
                MouseDownOnBar = false;
            }
        }

        override public void OnMouseUp(MouseEventArgs mouseEvent)
        {
            if (MouseDownOnBar
              && InRect(M.New<T>(mouseEvent.X), M.New<T>(mouseEvent.Y)))
            {
                mouseEvent.Handled = true;
            }

            MouseDownOnBar = false;
        }

        override public void OnMouseMove(MouseEventArgs mouseEvent)
        {
            if (MouseDownOnBar)
            {
                IVector<T> mouseRelClient = MatrixFactory<T>.CreateVector2D(M.New<T>(mouseEvent.X), M.New<T>(mouseEvent.Y));
                PointToClient(ref mouseRelClient);

                IVector<T> newLocation = Parrent.Location;
                newLocation[0].AddEquals(mouseRelClient[0].Subtract(m_DownPosition[0]));
                newLocation[1].AddEquals(mouseRelClient[1].Subtract(m_DownPosition[1]));
                Parrent.Location = newLocation;
                // TODO: invalidate the parents current position and where it is going.
                //Invalidate();
            }
        }
    };
}
