using System;
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
//
// class gamma_spline
//
//----------------------------------------------------------------------------
using AGG.Interpolation;
using NPack.Interfaces;
namespace AGG.UI
{
    //------------------------------------------------------------------------
    // Class-helper for calculation gamma-correction arrays. A gamma-correction
    // array is an array of 256 unsigned chars that determine the actual values 
    // of Anti-Aliasing for each pixel coverage Value from 0 to 255. If all the 
    // values in the array are equal to its index, i.e. 0,1,2,3,... there's
    // no gamma-correction. Class agg::polyfill allows you to use custom 
    // gamma-correction arrays. You can calculate it using any approach, and
    // class gamma_spline allows you to calculate almost any reasonable shape 
    // of the gamma-curve with using only 4 values - kx1, ky1, kx2, ky2.
    // 
    //                                      kx2
    //        +----------------------------------+
    //        |                 |        |    .  | 
    //        |                 |        | .     | ky2
    //        |                 |       .  ------|
    //        |                 |    .           |
    //        |                 | .              |
    //        |----------------.|----------------|
    //        |             .   |                |
    //        |          .      |                |
    //        |-------.         |                |
    //    ky1 |    .   |        |                |
    //        | .      |        |                |
    //        +----------------------------------+
    //            kx1
    // 
    // Each Value can be in range [0...2]. Value 1.0 means one quarter of the
    // bounding rectangle. Function values() calculates the curve by these
    // 4 values. After calling it one can get the gamma-array with call gamma(). 
    // Class also supports the vertex source interface, i.e rewind() and
    // vertex(). It's made for convinience and used in class gamma_ctrl. 
    // Before calling rewind/vertex one must set the bounding box
    // box() using pixel coordinates. 
    //------------------------------------------------------------------------

    public class gamma_spline<T> : SimpleVertexSourceWidget<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        byte[] m_gamma = new byte[256];
        T[] m_x = new T[4];
        T[] m_y = new T[4];
        BSpline<T> m_spline = new BSpline<T>();
        //double        m_x1;
        //double        m_y1;
        //double        m_x2;
        //double        m_y2;
        T m_cur_x;

        public gamma_spline()
            :
            base(M.Zero<T>(), M.Zero<T>(), M.New<T>(10), M.New<T>(10))
        {
            m_cur_x = M.Zero<T>();
            values(M.One<T>(), M.One<T>(), M.One<T>(), M.One<T>());
        }

        public override uint num_paths()
        {
            throw new System.Exception("The method or operation is not implemented.");
        }


        public void values(T kx1, T ky1, T kx2, T ky2)
        {
            if (kx1.LessThan(0.001)) kx1 = M.New<T>(0.001);
            if (kx1.GreaterThan(1.999)) kx1 = M.New<T>(1.999);
            if (ky1.LessThan(0.001)) ky1 = M.New<T>(0.001);
            if (ky1.GreaterThan(1.999)) ky1 = M.New<T>(1.999);
            if (kx2.LessThan(0.001)) kx2 = M.New<T>(0.001);
            if (kx2.GreaterThan(1.999)) kx2 = M.New<T>(1.999);
            if (ky2.LessThan(0.001)) ky2 = M.New<T>(0.001);
            if (ky2.GreaterThan(1.999)) ky2 = M.New<T>(1.999);

            m_x[0] = M.Zero<T>();
            m_y[0] = M.Zero<T>();
            m_x[1] = kx1.Multiply(0.25);
            m_y[1] = ky1.Multiply(0.25);
            m_x[2] = M.One<T>().Subtract(kx2.Multiply(0.25));
            m_y[2] = M.One<T>().Subtract(ky2.Multiply(0.25));
            m_x[3] = M.One<T>();
            m_y[3] = M.One<T>();

            m_spline.Init(4, m_x, m_y);

            int i;
            for (i = 0; i < 256; i++)
            {
                m_gamma[i] = (byte)(y(M.New<T>(i).Divide(255.0)).ToDouble() * 255.0);
            }
        }

        public byte[] gamma() { return m_gamma; }
        public T y(T x)
        {
            if (x.LessThan(0.0)) x = M.Zero<T>();
            if (x.GreaterThan(1.0)) x = M.One<T>();
            T val = m_spline.Get(x);
            if (val.LessThan(0.0)) val = M.Zero<T>();
            if (val.GreaterThan(1.0)) val = M.One<T>();
            return val;
        }

        public void values(out T kx1, out T ky1, out T kx2, out T ky2)
        {
            kx1 = m_x[1].Multiply(4.0);
            ky1 = m_y[1].Multiply(4.0);
            kx2 = M.One<T>().Subtract(m_x[2]).Multiply(4.0);
            ky2 = M.One<T>().Subtract(m_y[2]).Multiply(4.0);
        }

        public void box(T x1, T y1, T x2, T y2)
        {
            Bounds = new RectDouble<T>(x1, y1, x2, y2);
        }


        public override void Rewind(uint idx)
        {
            m_cur_x = M.Zero<T>();
        }

        public override uint Vertex(out T ox, out T oy)
        {
            ox = M.Zero<T>();
            oy = M.Zero<T>();
            if (m_cur_x.Equals(0.0))
            {
                ox = Bounds.Left;
                oy = Bounds.Bottom;
                m_cur_x.AddEquals(M.One<T>().Divide(Bounds.Right.Subtract(Bounds.Left)));
                return (uint)Path.Commands.MoveTo;
            }

            if (m_cur_x.GreaterThan(1.0))
            {
                return (uint)Path.Commands.Stop;
            }

            ox = Bounds.Left.Add(m_cur_x.Multiply(Bounds.Right.Subtract(Bounds.Left)));
            oy = Bounds.Bottom.Add(y(m_cur_x).Multiply(Bounds.Top.Subtract(Bounds.Bottom)));

            m_cur_x.AddEquals(M.One<T>().Divide(Bounds.Right.Subtract(Bounds.Left)));
            return (uint)Path.Commands.LineTo;
        }
    };
}
