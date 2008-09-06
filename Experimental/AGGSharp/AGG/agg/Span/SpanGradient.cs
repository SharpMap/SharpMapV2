using System;
using AGG.Color;
using AGG.Gradient;
using AGG.Interpolation;
using NPack.Interfaces;

namespace AGG.Span
{
    public class SpanGradient<T> : ISpanGenerator<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public const int GradientShift = 4;                              //-----gradient_subpixel_shift
        public const int GradientScale = 1 << GradientShift;   //-----gradient_subpixel_scale
        public const int GradientMask = GradientScale - 1;    //-----gradient_subpixel_mask

        public const int SubpixelShift = 8;

        public const int DownscaleShift = SubpixelShift - GradientShift;

        ISpanInterpolator<T> m_interpolator;
        IGradient m_gradient_function;
        IColorFunction m_color_function;
        int m_d1;
        int m_d2;

        //--------------------------------------------------------------------
        public SpanGradient() { }

        //--------------------------------------------------------------------
        public SpanGradient(ISpanInterpolator<T> inter,
                      IGradient gradient_function,
                      IColorFunction color_function,
                      double d1, double d2)
        {
            m_interpolator = inter;
            m_gradient_function = gradient_function;
            m_color_function = color_function;
            m_d1 = (Basics.RoundInt(d1 * GradientScale));
            m_d2 = (Basics.RoundInt(d2 * GradientScale));
        }

        //--------------------------------------------------------------------
        public ISpanInterpolator<T> Interpolator
        {
            get { return m_interpolator; }
            set
            {
                m_interpolator = value;
            }
        }
        public IGradient GradientFunction
        {
            get { return m_gradient_function; }
            set
            {
                m_gradient_function = value;
            }
        }
        public IColorFunction ColorFunction
        {
            get { return m_color_function; }
            set
            {
                m_color_function = value;
            }
        }
        public double D1
        {
            get { return (double)(m_d1) / GradientScale; }
            set { m_d1 = Basics.RoundInt(value * GradientScale); }
        }
        public double D2
        {
            get { return (double)(m_d2) / GradientScale; }
            set { m_d2 = Basics.RoundInt(value * GradientScale); }
        }

        //--------------------------------------------------------------------
        //public void interpolator(ISpanInterpolator i) { m_interpolator = i; }
        //public void gradient_function(IGradient gf) { m_gradient_function = gf; }
        //public void color_function(IColorFunction cf) { m_color_function = cf; }
        //public void d1(double v) { m_d1 = Basics.RoundInt(v * GradientScale); }
        //public void d2(double v) { m_d2 = Basics.RoundInt(v * GradientScale); }

        //--------------------------------------------------------------------
        public void Prepare() { }

        //--------------------------------------------------------------------
        public unsafe void Generate(RGBA_Bytes* span, int x, int y, uint len)
        {
            int dd = m_d2 - m_d1;
            if (dd < 1) dd = 1;
            m_interpolator.Begin(M.New<T>(x).Add(0.5), M.New<T>(y).Add(0.5), len);
            do
            {
                m_interpolator.Coordinates(out x, out y);
                int d = m_gradient_function.Calculate(x >> DownscaleShift,
                                                       y >> DownscaleShift, m_d2);
                d = ((d - m_d1) * (int)m_color_function.Size) / dd;
                if (d < 0) d = 0;
                if (d >= (int)m_color_function.Size)
                {
                    d = m_color_function.Size - 1;
                }

                *span++ = m_color_function[d];
                m_interpolator.Next();
            }
            while (--len != 0);
        }
    };
}
