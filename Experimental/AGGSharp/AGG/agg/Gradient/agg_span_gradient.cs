
using System;
using AGG.Color;
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
using AGG.Span;
using NPack.Interfaces;
namespace AGG.Gradient
{
    public interface IGradient
    {
        int Calculate(int x, int y, int d);
    };

    public interface IColorFunction
    {
        int Size { get; }
        RGBA_Bytes this[int v]
        {
            get;
        }
    };

    //==========================================================span_gradient
   

    //=====================================================gradient_linear_color
    public struct GradientLinearColor : IColorFunction
    {
        RGBA_Bytes m_c1;
        RGBA_Bytes m_c2;
        int m_size;

        public GradientLinearColor(RGBA_Bytes c1, RGBA_Bytes c2)
            : this(c1, c2, 256)
        {
        }

        public GradientLinearColor(RGBA_Bytes c1, RGBA_Bytes c2, int size)
        {
            m_c1 = c1;
            m_c2 = c2;
            m_size = size;
        }

        public int Size { get { return m_size; } }
        public RGBA_Bytes this[int v]
        {
            get
            {
                return m_c1.Gradient(m_c2, (double)(v) / (double)(m_size - 1));
            }
        }

        public void Colors(RGBA_Bytes c1, RGBA_Bytes c2)
        {
            Colors(c1, c2, 256);
        }

        public void Colors(RGBA_Bytes c1, RGBA_Bytes c2, int size)
        {
            m_c1 = c1;
            m_c2 = c2;
            m_size = size;
        }
    };

    //==========================================================gradient_circle
    public class GradientCircle : IGradient
    {
        // Actually the same as radial. Just for compatibility
        public int Calculate(int x, int y, int d)
        {
            return (int)(MathUtil.FastSqrt((uint)(x * x + y * y)));
        }
    };


    //==========================================================gradient_radial
    public class GradientRadial : IGradient
    {
        public int Calculate(int x, int y, int d)
        {
            //return (int)(System.Math.Sqrt((uint)(x * x + y * y)));
            return (int)(MathUtil.FastSqrt((uint)(x * x + y * y)));
        }
    };

    //========================================================gradient_radial_d
    public class GradientRadialDbl : IGradient
    {
        public int Calculate(int x, int y, int d)
        {
            return (int)Basics.RoundUint(System.Math.Sqrt((double)(x) * (double)(x) + (double)(y) * (double)(y)));
        }
    };

    //====================================================gradient_radial_focus
    public class GradientRadialFocus<T> : IGradient
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        int m_r;
        int m_fx;
        int m_fy;
        double m_r2;
        double m_fx2;
        double m_fy2;
        double m_mul;

        //---------------------------------------------------------------------
        public GradientRadialFocus()
        {
            m_r = (100 * SpanGradient<T>.GradientScale);
            m_fx = (0);
            m_fy = (0);
            UpdateValues();
        }

        //---------------------------------------------------------------------
        public GradientRadialFocus(double r, double fx, double fy)
        {
            m_r = (Basics.RoundInt(r * SpanGradient<T>.GradientScale));
            m_fx = (Basics.RoundInt(fx * SpanGradient<T>.GradientScale));
            m_fy = (Basics.RoundInt(fy * SpanGradient<T>.GradientScale));
            UpdateValues();
        }

        //---------------------------------------------------------------------
        public void Init(double r, double fx, double fy)
        {
            m_r = Basics.RoundInt(r * SpanGradient<T>.GradientScale);
            m_fx = Basics.RoundInt(fx * SpanGradient<T>.GradientScale);
            m_fy = Basics.RoundInt(fy * SpanGradient<T>.GradientScale);
            UpdateValues();
        }

        //---------------------------------------------------------------------
        public double Radius { get { return (double)(m_r) / SpanGradient<T>.GradientScale; } }
        public double FocusX { get { return (double)(m_fx) / SpanGradient<T>.GradientScale; } }
        public double FocusY { get { return (double)(m_fy) / SpanGradient<T>.GradientScale; } }

        //---------------------------------------------------------------------
        public int Calculate(int x, int y, int d)
        {
            double dx = x - m_fx;
            double dy = y - m_fy;
            double d2 = dx * m_fy - dy * m_fx;
            double d3 = m_r2 * (dx * dx + dy * dy) - d2 * d2;
            return Basics.RoundInt((dx * m_fx + dy * m_fy + System.Math.Sqrt(System.Math.Abs(d3))) * m_mul);
        }

        //---------------------------------------------------------------------
        private void UpdateValues()
        {
            // Calculate the invariant values. In case the focal center
            // lies exactly on the gradient circle the divisor degenerates
            // into zero. In this case we just move the focal center by
            // one subpixel unit possibly in the direction to the origin (0,0)
            // and calculate the values again.
            //-------------------------
            m_r2 = (double)(m_r) * (double)(m_r);
            m_fx2 = (double)(m_fx) * (double)(m_fx);
            m_fy2 = (double)(m_fy) * (double)(m_fy);
            double d = (m_r2 - (m_fx2 + m_fy2));
            if (d == 0)
            {
                if (m_fx != 0)
                {
                    if (m_fx < 0) ++m_fx; else --m_fx;
                }

                if (m_fy != 0)
                {
                    if (m_fy < 0) ++m_fy; else --m_fy;
                }

                m_fx2 = (double)(m_fx) * (double)(m_fx);
                m_fy2 = (double)(m_fy) * (double)(m_fy);
                d = (m_r2 - (m_fx2 + m_fy2));
            }
            m_mul = m_r / d;
        }
    };


    //==============================================================gradient_x
    public class GradientX : IGradient
    {
        public int Calculate(int x, int y, int d) { return x; }
    };


    //==============================================================gradient_y
    public class GradientY : IGradient
    {
        public int Calculate(int x, int y, int d) { return y; }
    };

    //========================================================gradient_diamond
    public class GradientDiamond : IGradient
    {
        public int Calculate(int x, int y, int d)
        {
            int ax = System.Math.Abs(x);
            int ay = System.Math.Abs(y);
            return ax > ay ? ax : ay;
        }
    };

    //=============================================================gradient_xy
    public class GradientXY : IGradient
    {
        public int Calculate(int x, int y, int d)
        {
            return System.Math.Abs(x) * System.Math.Abs(y) / d;
        }
    };

    //========================================================gradient_sqrt_xy
    public class GradientSqrtXY : IGradient
    {
        public int Calculate(int x, int y, int d)
        {
            //return (int)System.Math.Sqrt((uint)(System.Math.Abs(x) * System.Math.Abs(y)));
            return (int)MathUtil.FastSqrt((uint)(System.Math.Abs(x) * System.Math.Abs(y)));
        }
    };

    //==========================================================gradient_conic
    public class GradientConic : IGradient
    {
        public int Calculate(int x, int y, int d)
        {
            return (int)Basics.RoundUint(System.Math.Abs(System.Math.Atan2((double)(y), (double)(x))) * (double)(d) / System.Math.PI);
        }
    };

    //=================================================gradient_repeat_adaptor
    public class GradientRepeatAdaptor : IGradient
    {
        IGradient m_gradient;

        public GradientRepeatAdaptor(IGradient gradient)
        {
            m_gradient = gradient;
        }


        public int Calculate(int x, int y, int d)
        {
            int ret = m_gradient.Calculate(x, y, d) % d;
            if (ret < 0) ret += d;
            return ret;
        }
    };

    //================================================gradient_reflect_adaptor
    public class GradientReflectAdaptor : IGradient
    {
        IGradient m_gradient;

        public GradientReflectAdaptor(IGradient gradient)
        {
            m_gradient = gradient;
        }

        public int Calculate(int x, int y, int d)
        {
            int d2 = d << 1;
            int ret = m_gradient.Calculate(x, y, d) % d2;
            if (ret < 0) ret += d2;
            if (ret >= d) ret = d2 - ret;
            return ret;
        }
    };

    public class GradientClampAdaptor : IGradient
    {
        IGradient m_gradient;

        public GradientClampAdaptor(IGradient gradient)
        {
            m_gradient = gradient;
        }

        public int Calculate(int x, int y, int d)
        {
            int ret = m_gradient.Calculate(x, y, d);
            if (ret < 0) ret = 0;
            if (ret > d) ret = d;
            return ret;
        }
    };
}