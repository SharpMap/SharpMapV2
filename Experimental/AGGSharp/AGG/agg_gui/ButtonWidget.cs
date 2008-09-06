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
// classes ButtonWidget
//
//----------------------------------------------------------------------------
using System;
using AGG.Color;
using AGG.Transform;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;

namespace AGG.UI
{
    //----------------------------------------------------------cbox_ctrl_impl
    public class ButtonWidget<T> : GUIWidget<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        TextWidget<T> m_ButtonText;
        bool m_MouseDownOnButton = false;
        bool m_MouseOverButton = false;
        bool m_DrawHoverEffect = true;
        T m_X;
        T m_Y;
        T m_BorderWidth;
        T m_TextPadding;
        T m_BorderRadius;

        public delegate void ButtonEventHandler(ButtonWidget<T> button);
        public event ButtonEventHandler ButtonClick;

        protected bool MouseDownOnButton
        {
            get { return m_MouseDownOnButton; }
            set { m_MouseDownOnButton = value; }
        }

        protected bool MouseOverButton
        {
            get { return m_MouseOverButton; }
            set { m_MouseOverButton = value; }
        }

        public T BorderWidth { get { return m_BorderWidth; } set { m_BorderWidth = value; } }
        public T TextPadding { get { return m_TextPadding; } set { m_TextPadding = value; } }
        public T BorderRadius { get { return m_BorderRadius; } set { m_BorderRadius = value; } }


        public ButtonWidget(double x, double y, string lable)
            : this(M.New<T>(x), M.New<T>(y), lable)
        {
        }

        public ButtonWidget(T x, T y, string lable)
            : this(x, y, lable, M.New<T>(16), M.Zero<T>(), M.New<T>(3), M.New<T>(5))
        {
        }

        public ButtonWidget(double x, double y, string lable,
            double textHeight, double textPadding, double borderWidth, double borderRadius)
            : this(M.New<T>(x), M.New<T>(y), lable, M.New<T>(textHeight), M.New<T>(textPadding), M.New<T>(borderWidth), M.New<T>(borderRadius))
        {
        }

        public ButtonWidget(T x, T y, string lable,
            T textHeight, T textPadding, T borderWidth, T borderRadius)
        {
            m_X = x;
            m_Y = y;
            SetTransform(MatrixFactory<T>.NewTranslation(x, y));
            m_ButtonText = new TextWidget<T>(lable, M.Zero<T>(), M.Zero<T>(), textHeight);
            AddChild(m_ButtonText);

            TextPadding = textPadding;
            BorderWidth = borderWidth;
            BorderRadius = borderRadius;

            T totalExtra = BorderWidth.Add(TextPadding);
            m_Bounds.Left = totalExtra.Negative();
            m_Bounds.Bottom = totalExtra.Negative();
            m_Bounds.Right = m_ButtonText.Width.Add(totalExtra);
            m_Bounds.Top = m_ButtonText.Height.Add(totalExtra);
        }

        public enum Alignment { Center, Left, Right };

        public void AlignX(T x, Alignment alignment)
        {
            IAffineTransformMatrix<T> transform = GetTransform();
            switch (alignment)
            {
                case Alignment.Center:
                    SetTransform(MatrixFactory<T>.NewTranslation(x.Subtract(Width.Divide(2)), transform.TranslationY()));
                    break;

                case Alignment.Left:
                    SetTransform(MatrixFactory<T>.NewTranslation(x, transform.TranslationY()));
                    break;

                case Alignment.Right:
                    SetTransform(MatrixFactory<T>.NewTranslation(x.Subtract(Width), transform.TranslationY()));
                    break;
            }
        }

        public override void OnDraw()
        {
            T totalExtra = BorderWidth.Add(TextPadding);
            RectDouble<T> Bounds = new RectDouble<T>(totalExtra.Negative(), totalExtra.Negative(), m_ButtonText.Width.Add(totalExtra), m_ButtonText.Height.Add(totalExtra));
            RoundedRect<T> rectBorder = new RoundedRect<T>(Bounds, m_BorderRadius);
            GetRenderer().Render(rectBorder, new RGBA_Bytes(0, 0, 0));
            RectDouble<T> insideBounds = Bounds;
            insideBounds.Inflate(BorderWidth.Negative());
            RoundedRect<T> rectInside = new RoundedRect<T>(insideBounds, M.New<T>(Math.Max(m_BorderRadius.Subtract(BorderWidth).ToDouble(), 0)));
            RGBA_Bytes insideColor = new RGBA_Bytes(1.0, 1.0, 1.0);
            if (MouseOverButton)
            {
                if (MouseDownOnButton)
                {
                    insideColor = new RGBA_Bytes(255, 110, 110);
                }
                else
                {
                    insideColor = new RGBA_Bytes(225, 225, 255);
                }
            }

            GetRenderer().Render(rectInside, insideColor);

#if false
            double x1, y1, x2, y2;
            m_ButtonText.GetTextBounds(out x1, out y1, out x2, out y2);
            RoundedRect rectText = new RoundedRect(x1, y1, x2, y2, 0);
            conv_stroke rectOutline = new conv_stroke(rectText);
            GetRenderer().Render(rectOutline, new RGBA_Bytes(1.0, 0, 0));
#endif
#if false
            RoundedRect rectText2 = new RoundedRect(m_ButtonText.Bounds, 0);
            conv_stroke rectOutline2 = new conv_stroke(rectText2);
            GetRenderer().Render(rectOutline, new RGBA_Bytes(0, 0, 1.0));
#endif

            base.OnDraw();
        }

        override public void OnMouseDown(MouseEventArgs mouseEvent)
        {
            if (InRect(M.New<T>(mouseEvent.X), M.New<T>(mouseEvent.Y)))
            {
                MouseDownOnButton = true;
                MouseOverButton = true;
                mouseEvent.Handled = true;
            }
            else
            {
                MouseDownOnButton = false;
            }
        }

        override public void OnMouseUp(MouseEventArgs mouseEvent)
        {
            if (MouseDownOnButton
              && InRect(M.New<T>(mouseEvent.X), M.New<T>(mouseEvent.Y)))
            {
                if (ButtonClick != null)
                {
                    ButtonClick(this);
                }
                mouseEvent.Handled = true;
            }

            MouseDownOnButton = false;
        }

        override public void OnMouseMove(MouseEventArgs mouseEvent)
        {
            if (InRect(M.New<T>(mouseEvent.X), M.New<T>(mouseEvent.Y)))
            {
                if (!MouseOverButton)
                {
                    MouseOverButton = true;
                    if (m_DrawHoverEffect)
                    {
                        Invalidate();
                    }
                }
            }
            else
            {
                if (MouseOverButton)
                {
                    MouseOverButton = false;
                    if (m_DrawHoverEffect)
                    {
                        Invalidate();
                    }
                }
            }
        }

        override public void OnKeyDown(KeyEventArgs keyEvent)
        {
            if (keyEvent.KeyCode == Keys.Space)
            {
                if (ButtonClick != null)
                {
                    ButtonClick(this);
                }
                keyEvent.Handled = true;
            }
        }
    };
}
