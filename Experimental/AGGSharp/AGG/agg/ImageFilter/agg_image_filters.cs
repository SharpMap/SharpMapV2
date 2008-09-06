
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
// Image transformation filters,
// Filtering classes (ImageFilterLookUpTable, image_filter),
// Basic filter shape classes
//----------------------------------------------------------------------------
using System;
using NPack.Interfaces;

namespace AGG.ImageFilter
{
    public interface IImageFilterFunction<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T Radius();
        T CalcWeight(T x);
    };
    public enum FilterScale
    {
        Shift = 14,                      //----image_filter_shift
        Scale = 1 << Shift, //----image_filter_scale 
        Mask = Scale - 1   //----image_filter_mask 
    };

    public enum SubPixelScale
    {
        Shift = 8,                         //----image_subpixel_shift
        Scale = 1 << Shift, //----image_subpixel_scale 
        Mask = Scale - 1   //----image_subpixel_mask 
    };
    //-----------------------------------------------------ImageFilterLookUpTable
    public class ImageFilterLookUpTable<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T m_radius;
        uint m_diameter;
        int m_start;
        ArrayPOD<short> m_weight_array;



        public void Calculate(IImageFilterFunction<T> filter)
        {
            Calculate(filter, true);
        }

        public void Calculate(IImageFilterFunction<T> filter, bool normalization)
        {
            T r = filter.Radius();
            ReAllocLut(r);
            uint i;
            uint pivot = Diameter() << ((int)SubPixelScale.Shift - 1);
            for (i = 0; i < pivot; i++)
            {
                T x = M.New<T>(i).Divide((double)SubPixelScale.Scale);

                T y = filter.CalcWeight(x);
                m_weight_array.Array[pivot + i] =
                m_weight_array.Array[pivot - i] = (short)Basics.RoundInt(y.Multiply((int)FilterScale.Scale));
            }
            uint end = (Diameter() << (int)SubPixelScale.Shift) - 1;
            m_weight_array.Array[0] = m_weight_array.Array[end];
            if (normalization)
            {
                Normalize();
            }
        }

        public ImageFilterLookUpTable()
        {
            m_weight_array = new ArrayPOD<short>(256);
            m_radius = M.Zero<T>();
            m_diameter = (0);
            m_start = (0);
        }

        public ImageFilterLookUpTable(IImageFilterFunction<T> filter)
            : this(filter, true)
        {

        }
        public ImageFilterLookUpTable(IImageFilterFunction<T> filter, bool normalization)
        {
            m_weight_array = new ArrayPOD<short>(256);
            Calculate(filter, normalization);
        }

        public T Radius() { return m_radius; }
        public uint Diameter() { return m_diameter; }
        public int Start() { return m_start; }
        public unsafe short[] WeightArray() { return m_weight_array.Array; }

        //--------------------------------------------------------------------
        // This function normalizes integer values and corrects the rounding 
        // errors. It doesn't do anything with the source floating point values
        // (m_weight_array_dbl), it corrects only integers according to the rule 
        // of 1.0 which means that any sum of pixel weights must be equal to 1.0.
        // So, the filter function must produce a graph of the proper shape.
        //--------------------------------------------------------------------
        public void Normalize()
        {
            uint i;
            int flip = 1;

            for (i = 0; i < (int)SubPixelScale.Scale; i++)
            {
                for (; ; )
                {
                    int sum = 0;
                    uint j;
                    for (j = 0; j < m_diameter; j++)
                    {
                        sum += m_weight_array.Array[j * (int)SubPixelScale.Scale + i];
                    }

                    if (sum == (int)FilterScale.Scale) break;

                    double k = (double)((int)FilterScale.Scale) / (double)(sum);
                    sum = 0;
                    for (j = 0; j < m_diameter; j++)
                    {
                        sum += m_weight_array.Array[j * (int)SubPixelScale.Scale + i] =
                            (short)Basics.RoundInt(m_weight_array.Array[j * (int)SubPixelScale.Scale + i] * k);
                    }

                    sum -= (int)FilterScale.Scale;
                    int inc = (sum > 0) ? -1 : 1;

                    for (j = 0; j < m_diameter && sum != 0; j++)
                    {
                        flip ^= 1;
                        uint idx = flip != 0 ? m_diameter / 2 + j / 2 : m_diameter / 2 - j / 2;
                        int v = m_weight_array.Array[idx * (int)SubPixelScale.Scale + i];
                        if (v < (int)FilterScale.Scale)
                        {
                            m_weight_array.Array[idx * (int)SubPixelScale.Scale + i] += (short)inc;
                            sum += inc;
                        }
                    }
                }
            }

            uint pivot = m_diameter << ((int)SubPixelScale.Shift - 1);

            for (i = 0; i < pivot; i++)
            {
                m_weight_array.Array[pivot + i] = m_weight_array.Array[pivot - i];
            }
            uint end = (Diameter() << (int)SubPixelScale.Shift) - 1;
            m_weight_array.Array[0] = m_weight_array.Array[end];
        }

        private void ReAllocLut(T radius)
        {
            m_radius = radius;
            m_diameter = Basics.CeilingUint(radius) * 2;
            m_start = -(int)(m_diameter / 2 - 1);
            int size = (int)m_diameter << (int)SubPixelScale.Shift;
            if (size > m_weight_array.Size())
            {
                m_weight_array.Resize(size);
            }
        }
    };

    /*

    //--------------------------------------------------------image_filter
    public class image_filter : ImageFilterLookUpTable
    {
        public image_filter()
        {
            calculate(m_filter_function);
        }
    
        private IImageFilter m_filter_function;
    };
     */


    //-----------------------------------------------image_filter_bilinear
    public struct ImageFilterBilinear<T> : IImageFilterFunction<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public T Radius() { return M.New<T>(1); }

        public T CalcWeight(T x)
        {
            return M.New<T>(1.0).Subtract(x);
        }
    };

    //-----------------------------------------------image_filter_hanning
    public struct ImageFilterHanning<T> : IImageFilterFunction<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public T Radius() { return M.New<T>(1.0); }
        public T CalcWeight(T x)
        {
            //return 0.5 + 0.5 * Math.Cos(Math.PI * x);

            return M.New<T>(0.5)
                .Add(M.New<T>(0.5))
                .Multiply(
                    x.Multiply(M.PI<T>()).Cos()
                    );
        }
    };

    //-----------------------------------------------image_filter_hamming
    public struct ImageFilterHamming<T> : IImageFilterFunction<T>
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public T Radius() { return M.One<T>(); }
        public T CalcWeight(T x)
        {
            //return 0.54 + 0.46 * Math.Cos(Math.PI * x);
            return M.New<T>(0.54 + 0.46)
                .Multiply(
                    x.Multiply(M.PI<T>()
                    )
                    .Cos());
        }
    };

    //-----------------------------------------------image_filter_hermite
    public struct ImageFilterHermite<T> : IImageFilterFunction<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public T Radius() { return M.One<T>(); }
        public T CalcWeight(T x)
        {
            //return (2.0 * x - 3.0) * x * x + 1.0;

            return M.New<T>(2)
                .Multiply(x)
                .Subtract(3)
                .Multiply(x)
                .Multiply(x)
                .Add(1);
        }
    };

    //------------------------------------------------image_filter_quadric
    public struct ImageFilterQuadric<T> : IImageFilterFunction<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public T Radius() { return M.New<T>(1.5); }
        public T CalcWeight(T x)
        {
            T t;
            if (x.LessThan(0.5))
                return M.New<T>(0.75)
                    .Subtract(x)
                    .Multiply(x);

            if (x.LessThan(1.5)) { t = x.Subtract(1.5); return M.New<T>(0.5).Multiply(t).Multiply(t); }
            return M.Zero<T>();
        }
    };

    //------------------------------------------------image_filter_bicubic
    public class ImageFilterBicubic<T> : IImageFilterFunction<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private T pow3(T x)
        {
            return (x.LessThanOrEqualTo(0.0)) ? M.Zero<T>() : x.Multiply(x).Multiply(x);
        }

        public T Radius() { return M.New<T>(2.0); }
        public T CalcWeight(T x)
        {
            //return
            //    (1.0 / 6.0) *
            //    (pow3(x + 2) - 4 * pow3(x + 1) + 6 * pow3(x) - 4 * pow3(x - 1));

            return M.New<T>((1.0 / 6.0)).Multiply
               (
                (pow3(x.Add(2)).Subtract(4).Multiply(pow3(x.Add(1))).Add(6).Multiply(pow3(x)).Subtract(4).Multiply(pow3(x.Subtract(1))))
                );

        }
    };

    //-------------------------------------------------image_filter_kaiser
    public class ImageFilterKaiser<T> : IImageFilterFunction<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private T a;
        private T i0a;
        private T epsilon;

        public ImageFilterKaiser()
            : this(6.33)
        {

        }

        public ImageFilterKaiser(double b)
            : this(M.New<T>(b)) { }

        public ImageFilterKaiser(T b)
        {
            a = (b);
            epsilon = M.New<T>(1e-12);
            i0a = M.One<T>().Divide(
                bessel_i0(b));
        }

        public T Radius() { return M.One<T>(); }

        public T CalcWeight(T x)
        {
            //return bessel_i0(a * Math.Sqrt(1.0 - x * x)) * i0a;

            return bessel_i0
                (
                    a.Multiply
                    (
                        M.New<T>(1.0).Subtract(x).Multiply(x).Sqrt()
                     )
                 )
                 .Multiply(i0a);

        }

        private T bessel_i0(T x)
        {
            int i;
            T sum, y, t;

            sum = M.One<T>();

            y = x.Multiply(x)
                .Divide(4.0);

            t = y;

            for (i = 2; t.GreaterThan(epsilon); i++)
            {
                sum.AddEquals(t);
                t.MultiplyEquals
                    (
                        y.Divide(i * i)
                    );
            }
            return sum;
        }
    };

    //----------------------------------------------image_filter_catrom
    public struct ImageFilterCatrom<T> : IImageFilterFunction<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public T Radius() { return M.New<T>(2.0); }

        public T CalcWeight(T x)
        {
            if (x.LessThan(1.0))
            {
                //return 0.5 * (2.0 + x * x * (-5.0 + x * 3.0));
                return M.New<T>(0.5).Multiply(
                    M.New<T>(2.0)
                    .Add(x)
                    .Multiply(x)
                    .Multiply(
                        M.New<T>(-5.0)
                        .Add(x)
                        .Multiply(3.0)
                        )
                    );

            } if (x.LessThan(2.0))
            {
                //return 0.5 * (4.0 + x * (-8.0 + x * (5.0 - x)));
                return M.New<T>(0.5).Multiply
                    (
                        M.New<T>(4.0)
                        .Add(x)
                        .Multiply(
                            M.New<T>(-8.0)
                            .Add(x)
                            .Multiply(
                                M.New<T>(5.0)
                                .Subtract(x)
                                ))
                     );

            }
            return M.Zero<T>();
        }
    };

    //---------------------------------------------image_filter_mitchell
    public class ImageFilterMitchell<T> : IImageFilterFunction<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private T p0, p2, p3;
        private T q0, q1, q2, q3;

        public ImageFilterMitchell()
            : this(M.New<T>(1.0 / 3.0), M.New<T>(1.0 / 3.0))
        {

        }

        public ImageFilterMitchell(T b, T c)
        {
            //p0 = ((6.0 - 2.0 * b) / 6.0);
            //p2 = ((-18.0 + 12.0 * b + 6.0 * c) / 6.0);
            //p3 = ((12.0 - 9.0 * b - 6.0 * c) / 6.0);
            //q0 = ((8.0 * b + 24.0 * c) / 6.0);
            //q1 = ((-12.0 * b - 48.0 * c) / 6.0);
            //q2 = ((6.0 * b + 30.0 * c) / 6.0);
            //q3 = ((-b - 6.0 * c) / 6.0);


            p0 = M.New<T>(6.0 - 2.0)
                    .Multiply(b)
                    .Divide(6.0);


            p2 = M.New<T>(-18.0 + 12.0)
                    .Multiply(b)
                    .Add(6.0)
                    .Multiply(c)
                    .Divide(6.0);

            p3 = M.New<T>(12.0 - 9.0)
                    .Multiply(b)
                    .Subtract(6.0)
                    .Multiply(c)
                    .Divide(6.0);

            q0 = M.New<T>(8.0)
                    .Multiply(b)
                    .Add(24.0)
                    .Multiply(c)
                    .Divide(6.0);

            q1 = M.New<T>(-12.0)
                    .Multiply(b)
                    .Subtract(48.0)
                    .Multiply(c)
                    .Divide(6.0);

            q2 = M.New<T>(6.0)
                    .Multiply(b)
                    .Add(30.0)
                    .Multiply(c)
                    .Divide(6.0);

            q3 = b.Negative()
                    .Subtract(6.0)
                    .Multiply(c)
                    .Divide(6.0);

        }

        public T Radius() { return M.New<T>(2.0); }
        public T CalcWeight(T x)
        {
            if (x.LessThan(1.0))
                return p0.Add(x)
                        .Multiply(x)
                        .Multiply(
                            p2.Add(x).Multiply(p3)
                         );
            // if (x.LessThan(2.0)) return q0 + x * (q1 + x * (q2 + x * q3));

            if (x.LessThan(2.0))
                return q0.Add(x)
                         .Multiply(
                            q1.Add(x)
                            .Multiply(
                                q2.Add(x).Multiply(q3)
                                ));

            return M.Zero<T>();
        }
    };


    //----------------------------------------------image_filter_spline16
    public struct ImageFilterSpline16<T> : IImageFilterFunction<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public T Radius() { return M.New<T>(2.0); }
        public T CalcWeight(T x)
        {
            if (x.LessThan(1.0))
            {
                return x.Subtract(9.0)
                        .Divide(5.0)
                        .Multiply(x)
                        .Subtract(1.0)
                        .Divide(5.0)
                        .Multiply(x)
                        .Add(1.0);
            }
            return M.New<T>(-1.0 / 3.0)
                .Multiply(x.Subtract(1))
                .Add(4.0)
                .Divide(5.0)
                .Multiply(
                    x.Subtract(1)
                ).Subtract(7.0)
                .Divide(15.0)
                .Multiply(
                    x.Subtract(1)
                    );
        }
    };


    //---------------------------------------------image_filter_spline36
    public struct ImageFilterSpline36<T> : IImageFilterFunction<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public T Radius() { return M.New<T>(3.0); }
        public T CalcWeight(T x)
        {
            if (x.LessThan(1.0))
            {
                //return ((13.0 / 11.0 * x - 453.0 / 209.0) * x - 3.0 / 209.0) * x + 1.0;

                return M.New<T>(13.0 / 11.0)
                            .Multiply(x)
                            .Subtract(453.0)
                            .Divide(209.0)
                            .Multiply(x)
                            .Subtract(3.0)
                            .Divide(209.0)
                            .Multiply(x).Add(1.0);

            }
            if (x.LessThan(2.0))
            {
                //return ((-6.0 / 11.0 * (x - 1) + 270.0 / 209.0) * (x - 1) - 156.0 / 209.0) * (x - 1);

                return M.New<T>(-6.0 / 11.0)
                    .Multiply(
                        x.Subtract(1))
                    .Add(270.0)
                    .Divide(209.0)
                    .Multiply(
                        x.Subtract(1)
                    ).Subtract(156.0)
                    .Divide(209.0)
                    .Multiply(
                        x.Subtract(1)
                      );

            }
            return M.New<T>(1.0 / 11.0)
                    .Multiply(
                        x.Subtract(2)
                    ).Subtract(45.0)
                    .Divide(209.0)
                    .Multiply(
                        x.Subtract(2)
                    ).Add(26.0)
                    .Divide(209.0)
                    .Multiply(
                        x.Subtract(2)
                        );
        }
    };


    //----------------------------------------------image_filter_gaussian
    public struct ImageFilterGaussian<T> : IImageFilterFunction<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public T Radius() { return M.New<T>(2.0); }
        public T CalcWeight(T x)
        {
            //return Math.Exp(-2.0 * x * x) * Math.Sqrt(2.0 / Math.PI);
            return x.Multiply(-2)
                .Multiply(x)
                .Exp()
                .Multiply(
                    M.New<T>(2.0)
                    .Divide(
                        M.PI<T>()
                    ).Sqrt()
                );
        }
    };


    //------------------------------------------------image_filter_bessel
    public struct ImageFilterBessel<T> : IImageFilterFunction<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public T Radius() { return M.New<T>(3.2383); }
        public T CalcWeight(T x)
        {
            return x.Equals(0.0) ? M.PI<T>().Divide(4.0) : MathUtil.Besj(M.PI<T>().Multiply(x), 1).Divide(x.Multiply(2));
        }
    };


    //-------------------------------------------------image_filter_sinc
    public class ImageFilterSinc<T> : IImageFilterFunction<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public ImageFilterSinc(T r)
        {
            m_radius = (r.LessThan(2.0) ? M.New<T>(2.0) : r);
        }
        public T Radius() { return m_radius; }
        public T CalcWeight(T x)
        {
            if (x.Equals(0.0)) return M.One<T>();
            x.MultiplyEquals(M.PI<T>());
            return x.Sin().Divide(x);
        }

        private T m_radius;
    };


    //-----------------------------------------------image_filter_lanczos
    public class ImageFilterLanczos<T> : IImageFilterFunction<T>
       where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public ImageFilterLanczos(T r)
        {
            m_radius = (r.LessThan(2.0) ? M.New<T>(2.0) : r);
        }
        public T Radius() { return m_radius; }
        public T CalcWeight(T x)
        {
            if (x.Equals(0.0)) return M.One<T>();
            if (x.GreaterThan(m_radius)) return M.Zero<T>();

            x.MultiplyEquals(M.PI<T>());
            T xr = x.Divide(m_radius);
            return x.Sin()
                .Divide(x)
                .Multiply(
                    xr.Sin()
                    .Divide(xr)
                );
        }
        private T m_radius;
    };

    //----------------------------------------------image_filter_blackman
    public class ImageFilterBlackman<T> : IImageFilterFunction<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public ImageFilterBlackman(T r)
        {
            m_radius = (r.LessThan(2.0) ? M.New<T>(2.0) : r);
        }

        public T Radius() { return m_radius; }

        public T CalcWeight(T x)
        {
            if (x.Equals(0.0))
            {
                return M.One<T>();
            }

            if (x.GreaterThan(m_radius))
            {
                return M.Zero<T>();
            }

            x.MultiplyEquals(M.PI<T>());
            T xr = x.Divide(m_radius);

            //return (Math.Sin(x) / x) * (0.42 + 0.5 * Math.Cos(xr) + 0.08 * Math.Cos(2 * xr));

            return x.Sin()
                    .Divide(x)
                    .Multiply(
                        M.New<T>(0.42 + 0.5)
                        .Multiply(
                            xr.Cos()
                        ).Add(0.08)
                        .Multiply(xr.Multiply(2).Cos()));

        }

        private T m_radius;
    };
}