
using System;
using AGG.Buffer;
using AGG.Interpolation;
using AGG.Span;
using NPack.Interfaces;
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
//
// Image transformations with filtering. Span generator base class
//
//----------------------------------------------------------------------------
using image_subpixel_scale_e = AGG.ImageFilter.SubPixelScale;

namespace AGG.ImageFilter
{


    //-------------------------------------------------------span_image_filter


    /*


    //==============================================span_image_resample_affine
    //template<class Source> 
    public class span_image_resample_affine : 
        span_image_filter//<Source, span_interpolator_linear<trans_affine> >
    {
        //typedef Source IImageAccessor;
        //typedef span_interpolator_linear<trans_affine> ISpanInterpolator;
        //typedef span_image_filter<source_type, ISpanInterpolator> base_type;

        //--------------------------------------------------------------------
        public span_image_resample_affine()
        {
            m_scale_limit=(200.0);
            m_blur_x=(1.0);
            m_blur_y=(1.0);
        }

        //--------------------------------------------------------------------
        public span_image_resample_affine(IImageAccessor src, 
                                   ISpanInterpolator inter,
                                   ImageFilterLookUpTable filter) : base(src, inter, filter)
        {
            m_scale_limit(200.0);
            m_blur_x(1.0);
            m_blur_y(1.0);
        }


        //--------------------------------------------------------------------
        public int  scale_limit() { return uround(m_scale_limit); }
        public void scale_limit(int v)  { m_scale_limit = v; }

        //--------------------------------------------------------------------
        public double blur_x() { return m_blur_x; }
        public double blur_y() { return m_blur_y; }
        public void blur_x(double v) { m_blur_x = v; }
        public void blur_y(double v) { m_blur_y = v; }
        public void blur(double v) { m_blur_x = m_blur_y = v; }

        //--------------------------------------------------------------------
        public void prepare() 
        {
            double scale_x;
            double scale_y;

            base_type::interpolator().transformer().scaling_abs(&scale_x, &scale_y);

            if(scale_x * scale_y > m_scale_limit)
            {
                scale_x = scale_x * m_scale_limit / (scale_x * scale_y);
                scale_y = scale_y * m_scale_limit / (scale_x * scale_y);
            }

            if(scale_x < 1) scale_x = 1;
            if(scale_y < 1) scale_y = 1;

            if(scale_x > m_scale_limit) scale_x = m_scale_limit;
            if(scale_y > m_scale_limit) scale_y = m_scale_limit;

            scale_x *= m_blur_x;
            scale_y *= m_blur_y;

            if(scale_x < 1) scale_x = 1;
            if(scale_y < 1) scale_y = 1;

            m_rx     = uround(    scale_x * (double)(image_subpixel_scale));
            m_rx_inv = uround(1.0/scale_x * (double)(image_subpixel_scale));

            m_ry     = uround(    scale_y * (double)(image_subpixel_scale));
            m_ry_inv = uround(1.0/scale_y * (double)(image_subpixel_scale));
        }

        protected int m_rx;
        protected int m_ry;
        protected int m_rx_inv;
        protected int m_ry_inv;

        private double m_scale_limit;
        private double m_blur_x;
        private double m_blur_y;
    };

     */


    //=====================================================span_image_resample
    public abstract class SpanImageResample<T>
        : SpanImageFilter<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public SpanImageResample(IRasterBufferAccessor src,
                            ISpanInterpolator<T> inter,
                            ImageFilterLookUpTable<T> filter)
            : base(src, inter, filter)
        {
            m_scale_limit = (20);
            m_blur_x = ((int)image_subpixel_scale_e.Scale);
            m_blur_y = ((int)image_subpixel_scale_e.Scale);
        }

        //public abstract void prepare();
        //public abstract unsafe void generate(rgba8* span, int x, int y, uint len);

        //--------------------------------------------------------------------
        int ScaleLimit { get { return m_scale_limit; } set { m_scale_limit = value; } }
        //void ScaleLimit(int v) { m_scale_limit = v; }

        //--------------------------------------------------------------------
        double BlurX { get { return (double)(m_blur_x) / (double)((int)image_subpixel_scale_e.Scale); } set { m_blur_x = (int)Basics.RoundUint(value * (double)((int)image_subpixel_scale_e.Scale)); } }
        double BlurY { get { return (double)(m_blur_y) / (double)((int)image_subpixel_scale_e.Scale); } set { m_blur_y = (int)Basics.RoundUint(value * (double)((int)image_subpixel_scale_e.Scale)); } }
        //void BlurX(double v) { m_blur_x = (int)Basics.RoundUint(v * (double)((int)image_subpixel_scale_e.Scale)); }
        //void BlurY(double v) { m_blur_y = (int)Basics.RoundUint(v * (double)((int)image_subpixel_scale_e.Scale)); }
        public double Blur
        {
            set
            {
                m_blur_x = m_blur_y = (int)Basics.RoundUint(value * (double)((int)image_subpixel_scale_e.Scale));
            }
        }

        protected void AdjustScale(ref int rx, ref int ry)
        {
            if (rx < (int)image_subpixel_scale_e.Scale) rx = (int)image_subpixel_scale_e.Scale;
            if (ry < (int)image_subpixel_scale_e.Scale) ry = (int)image_subpixel_scale_e.Scale;
            if (rx > (int)image_subpixel_scale_e.Scale * m_scale_limit)
            {
                rx = (int)image_subpixel_scale_e.Scale * m_scale_limit;
            }
            if (ry > (int)image_subpixel_scale_e.Scale * m_scale_limit)
            {
                ry = (int)image_subpixel_scale_e.Scale * m_scale_limit;
            }
            rx = (rx * m_blur_x) >> (int)image_subpixel_scale_e.Shift;
            ry = (ry * m_blur_y) >> (int)image_subpixel_scale_e.Shift;
            if (rx < (int)image_subpixel_scale_e.Scale) rx = (int)image_subpixel_scale_e.Scale;
            if (ry < (int)image_subpixel_scale_e.Scale) ry = (int)image_subpixel_scale_e.Scale;
        }

        int m_scale_limit;
        int m_blur_x;
        int m_blur_y;
    };
}
