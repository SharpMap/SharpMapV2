
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
using System;
using System.Linq;
using AGG.Transform;
using NPack;
using NPack.Interfaces;

namespace AGG.Interpolation
{
    /*
    //===========================================span_interpolator_persp_exact
    //template<uint SubpixelShift = 8> 
    class span_interpolator_persp_exact
    {
    public:
        typedef trans_perspective trans_type;
        typedef trans_perspective::iterator_x iterator_type;
        enum subpixel_scale_e
        {
            subpixel_shift = SubpixelShift,
            subpixel_scale = 1 << subpixel_shift
        };

        //--------------------------------------------------------------------
        span_interpolator_persp_exact() {}

        //--------------------------------------------------------------------
        // Arbitrary quadrangle transformations
        span_interpolator_persp_exact(double[] src, double[] dst) 
        {
            QuadToQuad(src, dst);
        }

        //--------------------------------------------------------------------
        // Direct transformations 
        span_interpolator_persp_exact(double x1, double y1, 
                                      double x2, double y2, 
                                      double[] quad)
        {
            RectToQuad(x1, y1, x2, y2, quad);
        }

        //--------------------------------------------------------------------
        // Reverse transformations 
        span_interpolator_persp_exact(double[] quad, 
                                      double x1, double y1, 
                                      double x2, double y2)
        {
            QuadToRect(quad, x1, y1, x2, y2);
        }

        //--------------------------------------------------------------------
        // Set the transformations using two arbitrary quadrangles.
        void QuadToQuad(double[] src, double[] dst)
        {
            m_trans_dir.QuadToQuad(src, dst);
            m_trans_inv.QuadToQuad(dst, src);
        }

        //--------------------------------------------------------------------
        // Set the direct transformations, i.e., rectangle -> quadrangle
        void RectToQuad(double x1, double y1, double x2, double y2, 
                          double[] quad)
        {
            double src[8];
            src[0] = src[6] = x1;
            src[2] = src[4] = x2;
            src[1] = src[3] = y1;
            src[5] = src[7] = y2;
            QuadToQuad(src, quad);
        }


        //--------------------------------------------------------------------
        // Set the reverse transformations, i.e., quadrangle -> rectangle
        void QuadToRect(double[] quad, 
                          double x1, double y1, double x2, double y2)
        {
            double dst[8];
            dst[0] = dst[6] = x1;
            dst[2] = dst[4] = x2;
            dst[1] = dst[3] = y1;
            dst[5] = dst[7] = y2;
            QuadToQuad(quad, dst);
        }

        //--------------------------------------------------------------------
        // Check if the equations were solved successfully
        bool is_valid() { return m_trans_dir.is_valid(); }

        //----------------------------------------------------------------
        void begin(double x, double y, uint len)
        {
            m_iterator = m_trans_dir.begin(x, y, 1.0);
            double xt = m_iterator.x;
            double yt = m_iterator.y;

            double dx;
            double dy;
            double delta = 1/(double)subpixel_scale;
            dx = xt + delta;
            dy = yt;
            m_trans_inv.Transform(&dx, &dy);
            dx -= x;
            dy -= y;
            int sx1 = agg_basics.uround(subpixel_scale/Math.Sqrt(dx*dx + dy*dy)) >> subpixel_shift;
            dx = xt;
            dy = yt + delta;
            m_trans_inv.Transform(&dx, &dy);
            dx -= x;
            dy -= y;
            int sy1 = agg_basics.uround(subpixel_scale/Math.Sqrt(dx*dx + dy*dy)) >> subpixel_shift;

            x += len;
            xt = x;
            yt = y;
            m_trans_dir.Transform(&xt, &yt);

            dx = xt + delta;
            dy = yt;
            m_trans_inv.Transform(&dx, &dy);
            dx -= x;
            dy -= y;
            int sx2 = agg_basics.uround(subpixel_scale/Math.Sqrt(dx*dx + dy*dy)) >> subpixel_shift;
            dx = xt;
            dy = yt + delta;
            m_trans_inv.Transform(&dx, &dy);
            dx -= x;
            dy -= y;
            int sy2 = agg_basics.uround(subpixel_scale/Math.Sqrt(dx*dx + dy*dy)) >> subpixel_shift;

            m_scale_x = dda2_line_interpolator(sx1, sx2, len);
            m_scale_y = dda2_line_interpolator(sy1, sy2, len);
        }


        //----------------------------------------------------------------
        void resynchronize(double xe, double ye, uint len)
        {
            // Assume x1,y1 are equal to the ones at the previous end point 
            int sx1 = m_scale_x.y();
            int sy1 = m_scale_y.y();

            // Calculate transformed coordinates at x2,y2 
            double xt = xe;
            double yt = ye;
            m_trans_dir.Transform(&xt, &yt);

            double delta = 1/(double)subpixel_scale;
            double dx;
            double dy;

            // Calculate scale by X at x2,y2
            dx = xt + delta;
            dy = yt;
            m_trans_inv.Transform(&dx, &dy);
            dx -= xe;
            dy -= ye;
            int sx2 = agg_basics.uround(subpixel_scale/Math.Sqrt(dx*dx + dy*dy)) >> subpixel_shift;

            // Calculate scale by Y at x2,y2
            dx = xt;
            dy = yt + delta;
            m_trans_inv.Transform(&dx, &dy);
            dx -= xe;
            dy -= ye;
            int sy2 = agg_basics.uround(subpixel_scale/Math.Sqrt(dx*dx + dy*dy)) >> subpixel_shift;

            // Initialize the interpolators
            m_scale_x = dda2_line_interpolator(sx1, sx2, len);
            m_scale_y = dda2_line_interpolator(sy1, sy2, len);
        }



        //----------------------------------------------------------------
        void operator++()
        {
            ++m_iterator;
            ++m_scale_x;
            ++m_scale_y;
        }

        //----------------------------------------------------------------
        void coordinates(int* x, int* y)
        {
            *x = agg_basics.iround(m_iterator.x * subpixel_scale);
            *y = agg_basics.iround(m_iterator.y * subpixel_scale);
        }

        //----------------------------------------------------------------
        void local_scale(int* x, int* y)
        {
            *x = m_scale_x.y();
            *y = m_scale_y.y();
        }

        //----------------------------------------------------------------
        void Transform(double[] x, double[] y)
        {
            m_trans_dir.Transform(x, y);
        }
        
    private:
        trans_type             m_trans_dir;
        trans_type             m_trans_inv;
        iterator_type          m_iterator;
        dda2_line_interpolator m_scale_x;
        dda2_line_interpolator m_scale_y;
    };
     */


    //============================================span_interpolator_persp_lerp
    //template<uint SubpixelShift = 8> 
    public class SpanInterpolatorPerspLerp<T> : ISpanInterpolator<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        IAffineTransformMatrix<T> m_trans_dir;
        IAffineTransformMatrix<T> m_trans_inv;
        Dda2LineInterpolator m_coord_x;
        Dda2LineInterpolator m_coord_y;
        Dda2LineInterpolator m_scale_x;
        Dda2LineInterpolator m_scale_y;

        const int subpixel_shift = 8;
        const int subpixel_scale = 1 << subpixel_shift;

        //--------------------------------------------------------------------
        public SpanInterpolatorPerspLerp()
        {
            m_trans_dir = MatrixFactory<T>.NewIdentity(VectorDimension.Two);// new Transform.Perspective();
            m_trans_inv = MatrixFactory<T>.NewIdentity(VectorDimension.Two);//new Transform.Perspective();
        }

        //--------------------------------------------------------------------
        // Arbitrary quadrangle transformations
        public SpanInterpolatorPerspLerp(double[] src, double[] dst)
            : this()
        {
            QuadToQuad(src, dst);

        }

        public SpanInterpolatorPerspLerp(T[] src, T[] dst)
            : this()
        {
            QuadToQuad(src, dst);
        }

        //--------------------------------------------------------------------
        // Direct transformations 

        public SpanInterpolatorPerspLerp(double x1, double y1,
                                     double x2, double y2,
                                     double[] quad)
            : this()
        {
            RectToQuad(x1, y1, x2, y2, quad);
        }

        public SpanInterpolatorPerspLerp(T x1, T y1,
                                     T x2, T y2,
                                     T[] quad)
            : this()
        {
            RectToQuad(x1, y1, x2, y2, quad);
        }

        //--------------------------------------------------------------------
        // Reverse transformations 
        public SpanInterpolatorPerspLerp(T[] quad,
                                     T x1, T y1,
                                     T x2, T y2)
            : this()
        {
            QuadToRect(quad, x1, y1, x2, y2);
        }

        //--------------------------------------------------------------------
        // Set the transformations using two arbitrary quadrangles.

        public void QuadToQuad(double[] src, double[] dst)
        {
        }
        public void QuadToQuad(T[] src, T[] dst)
        {
            //TODO:fix
            throw new NotImplementedException();
            //m_trans_dir.QuadToQuad(src, dst);
            //m_trans_inv.QuadToQuad(dst, src);
        }

        //--------------------------------------------------------------------
        // Set the direct transformations, i.e., rectangle -> quadrangle

        public void RectToQuad(double x1, double y1, double x2, double y2, double[] quad)
        {
            RectToQuad(M.New<T>(x1), M.New<T>(y1), M.New<T>(x2), M.New<T>(y2), quad.Select(o => M.New<T>(o)).ToArray());
        }

        public void RectToQuad(T x1, T y1, T x2, T y2, T[] quad)
        {
            T[] src = new T[8];
            src[0] = src[6] = x1;
            src[2] = src[4] = x2;
            src[1] = src[3] = y1;
            src[5] = src[7] = y2;
            QuadToQuad(src, quad);
        }


        //--------------------------------------------------------------------
        // Set the reverse transformations, i.e., quadrangle -> rectangle
        public void QuadToRect(double[] quad,
                         double x1, double y1, double x2, double y2)
        {
            QuadToRect(quad.Select(o => M.New<T>(o)).ToArray(), M.New<T>(x1), M.New<T>(y1), M.New<T>(x2), M.New<T>(y2));

        }

        public void QuadToRect(T[] quad,
                          T x1, T y1, T x2, T y2)
        {
            T[] dst = new T[8];
            dst[0] = dst[6] = x1;
            dst[2] = dst[4] = x2;
            dst[1] = dst[3] = y1;
            dst[5] = dst[7] = y2;
            QuadToQuad(quad, dst);
        }

        //--------------------------------------------------------------------
        // Check if the equations were solved successfully
        public bool IsValid() { return m_trans_dir.IsValid(); }

        //----------------------------------------------------------------

        public void Begin(double x, double y, uint len)
        {
            Begin(M.New<T>(x), M.New<T>(y), len);
        }

        public void Begin(T x, T y, uint len)
        {
            // Calculate transformed coordinates at x1,y1 
            //double xt = x;
            //double yt = y;
            //m_trans_dir.Transform(ref xt, ref yt);
            IVector<T> v1 = m_trans_dir.TransformVector(MatrixFactory<T>.CreateVector2D(x, y));

            T xt = v1[0], yt = v1[1];

            int x1 = Basics.RoundInt(xt.Multiply((double)subpixel_scale));
            int y1 = Basics.RoundInt(yt.Multiply((double)subpixel_scale));

            T dx;
            T dy;
            double delta = 1 / (double)subpixel_scale;

            // Calculate scale by X at x1,y1
            dx = xt.Add(delta);
            dy = yt;

            //m_trans_inv.Transform(ref dx, ref dy);
            IVector<T> v2 = m_trans_inv.TransformVector(MatrixFactory<T>.CreateVector2D(dx, dy));
            dx = v2[0];
            dy = v2[1];

            dx.SubtractEquals(x);
            dy.SubtractEquals(y);
            int sx1 = (int)Basics.RoundUint(M.New<T>(subpixel_scale).Divide(M.Length(dx, dy)).ToInt()) >> subpixel_shift;

            // Calculate scale by Y at x1,y1
            dx = xt;
            dy = yt.Add(delta);
            // m_trans_inv.Transform(ref dx, ref dy);
            IVector<T> v3 = m_trans_inv.TransformVector(MatrixFactory<T>.CreateVector2D(dx, dy));
            dx = v3[0];
            dy = v3[1];
            dx.SubtractEquals(x);
            dy.SubtractEquals(y);
            int sy1 = (int)Basics.RoundUint(M.New<T>((double)subpixel_scale).Divide(M.Length(dx, dy)).ToInt()) >> subpixel_shift;

            // Calculate transformed coordinates at x2,y2 
            x.AddEquals(len);
            xt = x;
            yt = y;
            //m_trans_dir.Transform(ref xt, ref yt);
            IVector<T> v4 = m_trans_dir.TransformVector(MatrixFactory<T>.CreateVector2D(xt, yt));
            xt = v4[0];
            yt = v4[1];

            int x2 = Basics.RoundInt(xt.Multiply((double)subpixel_scale));
            int y2 = Basics.RoundInt(yt.Multiply((double)subpixel_scale));

            // Calculate scale by X at x2,y2
            dx = xt.Add(delta);
            dy = yt;

            //m_trans_inv.Transform(ref dx, ref dy);
            IVector<T> v5 = m_trans_inv.TransformVector(MatrixFactory<T>.CreateVector2D(dx, dy));
            dx = v5[0];
            dy = v5[1];

            dx.SubtractEquals(x);
            dy.SubtractEquals(y);
            int sx2 = (int)Basics.RoundUint(M.New<T>((double)subpixel_scale).Divide(M.Length(dx, dy)).ToInt()) >> subpixel_shift;

            // Calculate scale by Y at x2,y2
            dx = xt;
            dy = yt.Add(delta);
            //m_trans_inv.Transform(ref dx, ref dy);
            IVector<T> v6 = m_trans_inv.TransformVector(MatrixFactory<T>.CreateVector2D(dx, dy));
            dx = v6[0];
            dy = v6[1];

            dx.SubtractEquals(x);
            dy.SubtractEquals(y);
            int sy2 = (int)Basics.RoundUint(M.New<T>((double)subpixel_scale).Divide(M.Length(dx, dy)).ToInt()) >> subpixel_shift;

            // Initialize the interpolators
            m_coord_x = new Dda2LineInterpolator(x1, x2, (int)len);
            m_coord_y = new Dda2LineInterpolator(y1, y2, (int)len);
            m_scale_x = new Dda2LineInterpolator(sx1, sx2, (int)len);
            m_scale_y = new Dda2LineInterpolator(sy1, sy2, (int)len);
        }


        //----------------------------------------------------------------
        public void Resynchronize(double xe, double ye, uint len)
        {
            Resynchronize(M.New<T>(xe), M.New<T>(ye), len);
        }
        public void Resynchronize(T xe, T ye, uint len)
        {
            // Assume x1,y1 are equal to the ones at the previous end point 
            int x1 = m_coord_x.Y;
            int y1 = m_coord_y.Y;
            int sx1 = m_scale_x.Y;
            int sy1 = m_scale_y.Y;

            // Calculate transformed coordinates at x2,y2 
            T xt = xe;
            T yt = ye;
            //m_trans_dir.Transform(ref xt, ref yt);
            IVector<T> v1 = m_trans_dir.TransformVector(MatrixFactory<T>.CreateVector2D(xt, yt));
            xt = v1[0];
            yt = v1[1];

            int x2 = Basics.RoundInt(xt.Multiply((double)subpixel_scale));
            int y2 = Basics.RoundInt(yt.Multiply((double)subpixel_scale));

            double delta = 1 / (double)subpixel_scale;
            T dx;
            T dy;

            // Calculate scale by X at x2,y2
            dx = xt.Add(delta);
            dy = yt;
            //m_trans_inv.Transform(ref dx, ref dy);
            IVector<T> v2 = m_trans_inv.TransformVector(MatrixFactory<T>.CreateVector2D(dx, dy));
            dx = v2[0];
            dy = v2[1];

            dx.SubtractEquals(xe);
            dy.SubtractEquals(ye);
            int sx2 = (int)Basics.RoundUint(M.New<T>((double)subpixel_scale).Divide(M.Length(dx, dy)).ToInt()) >> subpixel_shift;

            // Calculate scale by Y at x2,y2
            dx = xt;
            dy = yt.Add(delta);
            //m_trans_inv.Transform(ref dx, ref dy);
            IVector<T> v3 = m_trans_inv.TransformVector(MatrixFactory<T>.CreateVector2D(dx, dy));
            dx = v3[0];
            dy = v3[1];
            dx.SubtractEquals(xe);
            dy.SubtractEquals(ye);
            int sy2 = (int)Basics.RoundUint(M.New<T>((double)subpixel_scale).Divide(M.Length(dx, dy)).ToInt()) >> subpixel_shift;

            // Initialize the interpolators
            m_coord_x = new Dda2LineInterpolator(x1, x2, (int)len);
            m_coord_y = new Dda2LineInterpolator(y1, y2, (int)len);
            m_scale_x = new Dda2LineInterpolator(sx1, sx2, (int)len);
            m_scale_y = new Dda2LineInterpolator(sy1, sy2, (int)len);
        }

        public IAffineTransformMatrix<T> Transformer
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set { throw new System.NotImplementedException(); }
        }

        //public void Transformer(Transform.ITransform trans) 
        //{
        //    throw new System.NotImplementedException();
        //}

        //----------------------------------------------------------------
        public void Next()
        {
            m_coord_x.Next();
            m_coord_y.Next();
            m_scale_x.Next();
            m_scale_y.Next();
        }

        //----------------------------------------------------------------
        public void Coordinates(out int x, out int y)
        {
            x = m_coord_x.Y;
            y = m_coord_y.Y;
        }

        //----------------------------------------------------------------
        public void LocalScale(out int x, out int y)
        {
            x = m_scale_x.Y;
            y = m_scale_y.Y;
        }

        //----------------------------------------------------------------
        public void Transform(ref T x, ref T y)
        {
            IVector<T> v = m_trans_dir.TransformVector(MatrixFactory<T>.CreateVector2D(x, y));
            x = v[0];
            y = v[1];
            //m_trans_dir.Transform(ref x, ref y);
        }
    };
}