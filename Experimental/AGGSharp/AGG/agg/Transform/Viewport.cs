
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
// Viewport transformer - simple orthogonal conversions from world coordinates
//                        to screen (device) ones.
//
//----------------------------------------------------------------------------
using System;
using NPack.Interfaces;
using NPack;

namespace AGG.Transform
{
    public enum AspectRatio
    {
        Stretch,
        Meet,
        Slice
    };

    //----------------------------------------------------------trans_viewport
    public sealed class Viewport<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T m_world_x1;
        T m_world_y1;
        T m_world_x2;
        T m_world_y2;
        T m_device_x1;
        T m_device_y1;
        T m_device_x2;
        T m_device_y2;
        AspectRatio m_aspect;
        bool m_is_valid;
        T m_align_x;
        T m_align_y;
        T m_wx1;
        T m_wy1;
        T m_wx2;
        T m_wy2;
        T m_dx1;
        T m_dy1;
        T m_kx;
        T m_ky;



        //-------------------------------------------------------------------
        public Viewport()
        {
            m_world_x1 = (M.Zero<T>());
            m_world_y1 = (M.Zero<T>());
            m_world_x2 = (M.One<T>());
            m_world_y2 = (M.One<T>());
            m_device_x1 = (M.Zero<T>());
            m_device_y1 = (M.Zero<T>());
            m_device_x2 = (M.One<T>());
            m_device_y2 = (M.One<T>());
            m_aspect = AspectRatio.Stretch;
            m_is_valid = (true);
            m_align_x = (M.New<T>(0.5));
            m_align_y = (M.New<T>(0.5));
            m_wx1 = (M.Zero<T>());
            m_wy1 = (M.Zero<T>());
            m_wx2 = (M.One<T>());
            m_wy2 = (M.One<T>());
            m_dx1 = (M.Zero<T>());
            m_dy1 = (M.Zero<T>());
            m_kx = (M.One<T>());
            m_ky = (M.One<T>());
        }

        //-------------------------------------------------------------------
        public void PreserveAspectRatio(T alignx,
                                   T aligny,
                                   AspectRatio aspect)
        {
            m_align_x = alignx;
            m_align_y = aligny;
            m_aspect = aspect;
            Update();
        }

        //-------------------------------------------------------------------
        public void DeviceViewport(T x1, T y1, T x2, T y2)
        {
            m_device_x1 = x1;
            m_device_y1 = y1;
            m_device_x2 = x2;
            m_device_y2 = y2;
            Update();
        }

        //-------------------------------------------------------------------
        public void WorldViewport(T x1, T y1, T x2, T y2)
        {
            m_world_x1 = x1;
            m_world_y1 = y1;
            m_world_x2 = x2;
            m_world_y2 = y2;
            Update();
        }

        //-------------------------------------------------------------------
        public void DeviceViewport(out T x1, out T y1, out T x2, out T y2)
        {
            x1 = m_device_x1;
            y1 = m_device_y1;
            x2 = m_device_x2;
            y2 = m_device_y2;
        }

        //-------------------------------------------------------------------
        public void WorldViewport(out T x1, out T y1, out T x2, out T y2)
        {
            x1 = m_world_x1;
            y1 = m_world_y1;
            x2 = m_world_x2;
            y2 = m_world_y2;
        }

        //-------------------------------------------------------------------
        public void WorldViewportActual(out T x1, out T y1,
                                   out T x2, out T y2)
        {
            x1 = m_wx1;
            y1 = m_wy1;
            x2 = m_wx2;
            y2 = m_wy2;
        }

        //-------------------------------------------------------------------
        public bool IsValid { get { return m_is_valid; } }
        public T AlignX { get { return m_align_x; } }
        public T AlignY { get { return m_align_y; } }
        public AspectRatio AspectRatio { get { return m_aspect; } }

        //-------------------------------------------------------------------
        public void Transform(ref T x, ref T y)
        {
            x = x.Subtract(m_wx1).Multiply(m_kx).Add(m_dx1);
            y = y.Subtract(m_wy1).Multiply(m_ky).Add(m_dy1);
        }

        //-------------------------------------------------------------------
        public void TransformScaleOnly(ref T x, ref T y)
        {
            x.MultiplyEquals(m_kx);
            y.MultiplyEquals(m_ky);
        }

        //-------------------------------------------------------------------
        public void InverseTransform(ref T x, ref T y)
        {
            x = x.Subtract(m_dx1).Divide(m_kx).Add(m_wx1);
            y = y.Subtract(m_dy1).Divide(m_ky).Add(m_wy1);
        }

        //-------------------------------------------------------------------
        public void InverseTransformScaleOnly(ref T x, ref T y)
        {
            x.DivideEquals(m_kx);
            y.DivideEquals(m_ky);
        }

        //-------------------------------------------------------------------
        public T DeviceDX { get { return m_dx1.Subtract(m_wx1.Multiply(m_kx)); } }
        public T DeviceDY { get { return m_dy1.Subtract(m_wy1.Multiply(m_ky)); } }

        //-------------------------------------------------------------------
        public T ScaleX
        {
            get
            {
                return m_kx;
            }
        }

        //-------------------------------------------------------------------
        public T ScaleY
        {
            get
            {
                return m_ky;
            }
        }

        //-------------------------------------------------------------------
        public T Scale
        {
            get
            {
                return m_kx.Add(m_ky).Multiply(0.5);
            }
        }

        //-------------------------------------------------------------------
        public IAffineTransformMatrix<T> ToAffine()
        {
            //Affine mtx = Affine.NewTranslation(-m_wx1, -m_wy1);
            //mtx *= Affine.NewScaling(m_kx, m_ky);
            //mtx *= Affine.NewTranslation(m_dx1, m_dy1);
            //return mtx;
            IAffineTransformMatrix<T> a = MatrixFactory<T>.NewIdentity(VectorDimension.Two);
            a.Translate(MatrixFactory<T>.CreateVector2D(m_wx1.Negative(), m_wy1.Negative()));
            a.Scale(MatrixFactory<T>.CreateVector2D(m_kx, m_ky));
            a.Scale(MatrixFactory<T>.CreateVector2D(m_dx1, m_dy1));
            return a;
        }

        //-------------------------------------------------------------------
        public IAffineTransformMatrix<T> ToAffineScaleOnly()
        {
            return MatrixFactory<T>.NewScaling2D(m_kx, m_ky);
        }

        private void Update()
        {
            double epsilon = 1e-30;
            if (m_world_x1.Subtract(m_world_x2).Abs().LessThan(epsilon) ||
               m_world_y1.Subtract(m_world_y2).Abs().LessThan(epsilon) ||
               m_device_x1.Subtract(m_device_x2).Abs().LessThan(epsilon) ||
               m_device_y1.Subtract(m_device_y2).Abs().LessThan(epsilon))
            {
                m_wx1 = m_world_x1;
                m_wy1 = m_world_y1;
                m_wx2 = m_world_x1.Add(1.0);
                m_wy2 = m_world_y2.Add(1.0);
                m_dx1 = m_device_x1;
                m_dy1 = m_device_y1;
                m_kx = M.One<T>();
                m_ky = M.One<T>();
                m_is_valid = false;
                return;
            }

            T world_x1 = m_world_x1;
            T world_y1 = m_world_y1;
            T world_x2 = m_world_x2;
            T world_y2 = m_world_y2;
            T device_x1 = m_device_x1;
            T device_y1 = m_device_y1;
            T device_x2 = m_device_x2;
            T device_y2 = m_device_y2;
            if (m_aspect != AspectRatio.Stretch)
            {
                T d;
                m_kx = device_x2.Subtract(device_x1).Divide(world_x2.Subtract(world_x1));
                m_ky = device_y2.Subtract(device_y1).Divide(world_y2.Subtract(world_y1));

                if (m_aspect.Equals(AspectRatio.Meet).Equals(m_kx.LessThan(m_ky)))
                {
                    d = world_y2.Subtract(world_y1).Multiply(m_ky.Divide(m_kx));
                    world_y1.AddEquals(
                        world_y2.Subtract(world_y1).Subtract(d).Multiply(m_align_y)
                        );
                    world_y2 = world_y1.Add(d);
                }
                else
                {
                    d = world_x2.Subtract(world_x1).Multiply(m_kx.Divide(m_ky));
                    world_x1.AddEquals(world_x2.Subtract(world_x1).Subtract(d).Multiply(m_align_x));
                    world_x2 = world_x1.Add(d);
                }
            }
            m_wx1 = world_x1;
            m_wy1 = world_y1;
            m_wx2 = world_x2;
            m_wy2 = world_y2;
            m_dx1 = device_x1;
            m_dy1 = device_y1;
            m_kx = device_x2.Subtract(device_x1).Divide(world_x2.Subtract(world_x1));
            m_ky = device_y2.Subtract(device_y1).Divide(world_y2.Subtract(world_y1));
            m_is_valid = true;
        }
    };
}
