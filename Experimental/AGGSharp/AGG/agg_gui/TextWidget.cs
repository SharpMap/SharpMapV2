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
//
// classes rbox_ctrl_impl, rbox_ctrl
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
    //------------------------------------------------------------------------
    public class TextWidget<T> : SimpleVertexSourceWidget<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T m_BorderSize;
        T m_Thickness;
        T m_CapsHeight;

        T[] m_vx = new T[32];
        T[] m_vy = new T[32];

        GsvText<T> m_text;
        ConvStroke<T> m_text_poly;
        IColorType m_text_color;

        uint m_idx;

        public TextWidget(string Text, double left, double bottom, double CapitalHeight)
            : this(Text, M.New<T>(left), M.New<T>(bottom), M.New<T>(CapitalHeight))
        { }

        public TextWidget(string Text, T left, T bottom, T CapitalHeight)
            : base(M.Zero<T>(), M.Zero<T>(), M.Zero<T>(), M.Zero<T>())
        {
            m_text_color = (new RGBA_Doubles(0.0, 0.0, 0.0));
            m_BorderSize = CapitalHeight.Multiply(.2);
            m_Thickness = CapitalHeight.Divide(8);
            m_CapsHeight = CapitalHeight;
            m_text = new GsvText<T>();
            m_text.Text = Text;
            m_text_poly = new ConvStroke<T>(m_text);
            m_idx = (0);
            T MinX, MinY, MaxX, MaxY;
            GetTextBounds(out MinX, out MinY, out MaxX, out MaxY);
            T FullWidth = MaxX.Subtract(MinX).Add(m_BorderSize.Multiply(2));
            T FullHeight = m_CapsHeight.Add(m_text.AscenderHeight).Add(m_text.DescenderHeight).Add(m_BorderSize.Multiply(2));
            Bounds = new RectDouble<T>(left, bottom, left.Add(FullWidth), bottom.Add(FullHeight));
        }

        public string Text
        {
            get
            {
                return m_text.Text;
            }
            set
            {
                m_text.Text = value;
            }
        }

        public void GetSize(int characterToMeasureStartIndexInclusive, int characterToMeasureEndIndexInclusive, out IVector<T> offset)
        {
            m_text.GetSize(characterToMeasureStartIndexInclusive, characterToMeasureEndIndexInclusive, out offset);
        }

        // this will return the position to the left of the requested character.
        public IVector<T> GetOffsetLeftOfCharacterIndex(int characterIndex)
        {
            IVector<T> offset = MatrixFactory<T>.CreateVector2D(M.Zero<T>(), M.Zero<T>());
            if (characterIndex > 0)
            {
                m_text.GetSize(0, characterIndex - 1, out offset);
            }
            return offset;
        }

        // If the Text is "TEXT" and the position is less than half the distance to the center
        // of "T" the return Value will be 0 if it is between the center of 'T' and the center of 'E'
        // it will be 1 and so on.
        public int GetCharacterIndex(double xOffset)
        {
            return 0;
        }

        public override bool InRect(T x, T y)
        {
            GetTransform().Inverse.Transform(ref x, ref y);
            return Bounds.HitTest(x, y);
        }

        public void GetTextBounds(out T min_x, out T min_y, out T max_x, out T max_y)
        {
            Rewind(0);
            T VertexX;
            T VertexY;
            uint cmd = Vertex(out VertexX, out VertexY);
            min_x = VertexX;
            min_y = VertexY;
            max_x = VertexX;
            max_y = VertexY;
            while (cmd != (uint)Path.Commands.Stop)
            {
                cmd = Vertex(out VertexX, out VertexY) & (uint)Path.Commands.Mask;
                if (Path.IsVertex(cmd))
                {
                    min_x = M.Min(VertexX, min_x);
                    min_y = M.Min(VertexY, min_y);
                    max_x = M.Max(VertexX, max_x);
                    max_y = M.Max(VertexY, max_y);
                }
            }
        }

        // Vertex source interface
        public override uint num_paths() { return 1; }
        public override void Rewind(uint idx)
        {
            m_idx = idx;

            switch (idx)
            {
                case 0:                 // Text
                    m_text.StartPoint(Bounds.Left.Add(m_BorderSize), Bounds.Bottom.Add(m_BorderSize).Add(m_text.AscenderHeight));
                    m_text.SetFontSize(m_CapsHeight);
                    m_text_poly.Width = m_Thickness;
                    m_text_poly.LineJoin = LineJoin.RoundJoin;
                    m_text_poly.LineCap = LineCap.Round;
                    m_text_poly.Rewind(0);
                    break;
            }
        }

        public override uint Vertex(out T x, out T y)
        {
            x = M.Zero<T>();
            y = M.Zero<T>();
            uint cmd = (uint)Path.Commands.LineTo;
            switch (m_idx)
            {
                case 0:
                    cmd = m_text_poly.Vertex(out x, out y);
                    if (Path.IsStop(cmd))
                    {
                        m_text.StartPoint(Bounds.Left.Add(m_BorderSize),
                                           Bounds.Bottom.Add(m_BorderSize).Add(m_text.AscenderHeight));

                        m_text_poly.Rewind(0);
                        cmd = (uint)Path.Commands.Stop;
                    }
                    break;

                default:
                    cmd = (uint)Path.Commands.Stop;
                    break;
            }

            if (!Path.IsStop(cmd))
            {
                GetTransform().Transform(ref x, ref y);
            }

            return cmd;
        }


        public void text_color(IColorType c) { m_text_color = c; }

        public override IColorType color(uint i)
        {
            switch (i)
            {
                case 0:
                    return m_text_color;

                default:
                    throw new System.IndexOutOfRangeException("There is not a color for this index");
            }
        }
    };
}