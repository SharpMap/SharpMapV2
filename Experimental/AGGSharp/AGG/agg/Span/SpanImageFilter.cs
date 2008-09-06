using System;
using AGG.Buffer;
using AGG.Color;
using AGG.ImageFilter;
using AGG.Interpolation;
using NPack.Interfaces;
using image_subpixel_scale_e = AGG.ImageFilter.SubPixelScale;

namespace AGG.Span
{
    public abstract class SpanImageFilter<T> : ISpanGenerator<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public SpanImageFilter() { }
        public SpanImageFilter(IRasterBufferAccessor src,
            ISpanInterpolator<T> interpolator)
            : this(src, interpolator, null)
        {

        }

        public SpanImageFilter(IRasterBufferAccessor src,
            ISpanInterpolator<T> interpolator, ImageFilterLookUpTable<T> filter)
        {
            m_src = src;
            m_interpolator = interpolator;
            m_filter = (filter);

            m_dx_dbl.Set(0.5);
            m_dy_dbl.Set(0.5);

            m_dx_int = ((int)image_subpixel_scale_e.Scale / 2);
            m_dy_int = ((int)image_subpixel_scale_e.Scale / 2);
        }
        public void Attach(IRasterBufferAccessor v) { m_src = v; }

        public unsafe abstract void Generate(RGBA_Bytes* span, int x, int y, uint len);

        //--------------------------------------------------------------------
        public IRasterBufferAccessor Source() { return m_src; }
        public ImageFilterLookUpTable<T> Filter { get { return m_filter; } set { m_filter = value; } }
        public int FilterDxInt() { return (int)m_dx_int; }
        public int FilterDyInt() { return (int)m_dy_int; }
        public T FilterDxDbl() { return m_dx_dbl; }
        public T FilterDyDbl() { return m_dy_dbl; }

        //--------------------------------------------------------------------
        //public void Interpolator(ISpanInterpolator v) { m_interpolator = v; }
        //public void Filter(ImageFilterLookUpTable v) { m_filter = v; }
        public void FilterOffset(T dx, T dy)
        {
            m_dx_dbl = dx;
            m_dy_dbl = dy;
            m_dx_int = (uint)Basics.RoundInt(dx.Multiply(default(T).Set((int)image_subpixel_scale_e.Scale)));
            m_dy_int = (uint)Basics.RoundInt(dy.Multiply(default(T).Set((int)image_subpixel_scale_e.Scale)));
        }
        public void FilterOffset(T d) { FilterOffset(d, d); }

        //--------------------------------------------------------------------
        public ISpanInterpolator<T> Interpolator { get { return m_interpolator; } set { m_interpolator = value; } }

        //--------------------------------------------------------------------
        public void Prepare() { }

        //--------------------------------------------------------------------
        private IRasterBufferAccessor m_src;
        private ISpanInterpolator<T> m_interpolator;
        protected ImageFilterLookUpTable<T> m_filter;
        private T m_dx_dbl;
        private T m_dy_dbl;
        private uint m_dx_int;
        private uint m_dy_int;
    };
}
