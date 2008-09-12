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
// classes polygon_ctrl_impl, polygon_ctrl
//
//----------------------------------------------------------------------------
using System;
using AGG.Color;
using AGG.VertexSource;
using NPack;
using NPack.Interfaces;
namespace AGG.UI
{
    class simple_polygon_vertex_source<T> : IVertexSource<T>
                 where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T[] m_polygon;
        uint m_num_points;
        uint m_vertex;
        bool m_roundoff;
        bool m_close;

        public simple_polygon_vertex_source(T[] polygon, uint np)
            : this(polygon, np, false, true)
        {
        }

        public simple_polygon_vertex_source(T[] polygon, uint np,
                                     bool roundoff)
            : this(polygon, np, roundoff, true)
        {
        }

        public simple_polygon_vertex_source(T[] polygon, uint np, bool roundoff, bool close)
        {
            m_polygon = (polygon);
            m_num_points = (np);
            m_vertex = (0);
            m_roundoff = (roundoff);
            m_close = (close);
        }

        public void close(bool f) { m_close = f; }
        public bool close() { return m_close; }

        public void Rewind(uint idx)
        {
            m_vertex = 0;
        }

        public uint Vertex(out T x, out T y)
        {
            x = M.Zero<T>();
            y = M.Zero<T>();
            if (m_vertex > m_num_points)
            {
                return (uint)Path.Commands.Stop;
            }

            if (m_vertex == m_num_points)
            {
                ++m_vertex;
                return (uint)Path.Commands.EndPoly | (m_close ? (uint)Path.Flags.Close : 0);
            }
            x = m_polygon[m_vertex * 2];
            y = m_polygon[m_vertex * 2 + 1];
            if (m_roundoff)
            {
                x = x.Floor().Add(0.5);
                y = y.Floor().Add(0.5);
            }
            ++m_vertex;
            return (m_vertex == 1) ? (uint)Path.Commands.MoveTo : (uint)Path.Commands.LineTo;
        }
    };

    public class polygon_ctrl_impl<T> : UI.SimpleVertexSourceWidget<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        ArrayPOD<T> m_polygon;
        uint m_num_points;
        int m_node;
        int m_edge;
        simple_polygon_vertex_source<T> m_vs;
        ConvStroke<T> m_stroke;
        Ellipse<T> m_ellipse;
        T m_point_radius;
        uint m_status;
        T m_dx;
        T m_dy;
        bool m_in_polygon_check;

        public polygon_ctrl_impl(uint np) : this(np, M.New<T>(5)) { }


        public polygon_ctrl_impl(uint np, double point_radius)
            : this(np, M.New<T>(point_radius))
        {
        }

        public polygon_ctrl_impl(uint np, T point_radius)
            : base(M.Zero<T>(), M.Zero<T>(), M.One<T>(), M.One<T>())
        {
            m_ellipse = new Ellipse<T>();
            m_polygon = new ArrayPOD<T>(np * 2);
            m_num_points = (np);
            m_node = (-1);
            m_edge = (-1);
            m_vs = new simple_polygon_vertex_source<T>(m_polygon.Array, m_num_points, false);
            m_stroke = new ConvStroke<T>(m_vs);
            m_point_radius = (point_radius);
            m_status = (0);
            m_dx = M.Zero<T>();
            m_dy = M.Zero<T>();
            m_in_polygon_check = (true);
            m_stroke.Width = M.One<T>();
        }

        public uint num_points() { return m_num_points; }
        public T xn(uint n) { return m_polygon.Array[n * 2]; }

        public void SetXN(uint n, double newXN) { SetXN(n, M.New<T>(newXN)); }
        public void SetXN(uint n, T newXN) { m_polygon.Array[n * 2] = newXN; }

        public void AddXN(uint n, double newXN) { AddXN(n, M.New<T>(newXN)); }
        public void AddXN(uint n, T newXN) { m_polygon.Array[n * 2].AddEquals(newXN); }

        public T yn(uint n) { return m_polygon.Array[n * 2 + 1]; }
        public void SetYN(uint n, double newYN) { SetYN(n, M.New<T>(newYN)); }
        public void SetYN(uint n, T newYN) { m_polygon.Array[n * 2 + 1] = newYN; }
        public void AddYN(uint n, double newYN) { AddYN(n, M.New<T>(newYN)); }
        public void AddYN(uint n, T newYN) { m_polygon.Array[n * 2 + 1].AddEquals(newYN); }

        public T[] polygon() { return m_polygon.Array; }

        public void line_width(T w) { m_stroke.Width = w; }
        public T line_width() { return m_stroke.Width; }

        public void point_radius(T r) { m_point_radius = r; }
        public T point_radius() { return m_point_radius; }

        public void in_polygon_check(bool f) { m_in_polygon_check = f; }
        public bool in_polygon_check() { return m_in_polygon_check; }

        public void close(bool f) { m_vs.close(f); }
        public bool close() { return m_vs.close(); }

        // Vertex source interface
        public override uint num_paths() { return 1; }
        public override void Rewind(uint path_id)
        {
            m_status = 0;
            m_stroke.Rewind(0);
        }

        public override uint Vertex(out T x, out T y)
        {
            uint cmd = (uint)Path.Commands.Stop;
            T r = m_point_radius;
            if (m_status == 0)
            {
                cmd = m_stroke.Vertex(out x, out y);
                if (!Path.IsStop(cmd))
                {
                    GetTransform().Transform(ref x, ref y);
                    return cmd;
                }
                if (m_node >= 0 && m_node == (int)(m_status)) r.MultiplyEquals(1.2);
                m_ellipse.Init(xn(m_status), yn(m_status), r, r, 32);
                ++m_status;
            }
            cmd = m_ellipse.Vertex(out x, out y);
            if (!Path.IsStop(cmd))
            {
                GetTransform().Transform(ref x, ref y);
                return cmd;
            }
            if (m_status >= m_num_points) return (uint)Path.Commands.Stop;
            if (m_node >= 0 && m_node == (int)(m_status)) r.MultiplyEquals(1.2);
            m_ellipse.Init(xn(m_status), yn(m_status), r, r, 32);
            ++m_status;
            cmd = m_ellipse.Vertex(out x, out y);
            if (!Path.IsStop(cmd))
            {
                GetTransform().Transform(ref x, ref y);
            }
            return cmd;
        }

        public override bool InRect(T x, T y)
        {
            return false;
        }

        public override void OnMouseDown(MouseEventArgs mouseEvent)
        {
            bool ret = false;
            m_node = -1;
            m_edge = -1;
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            GetTransform().Inverse.Transform(ref x, ref y);
            for (uint i = 0; i < m_num_points; i++)
            {
                T dx = x.Subtract(xn(i)), dy = y.Subtract(yn(i));


                //if (Math.Sqrt((x - xn(i)) * (x - xn(i)) + (y - yn(i)) * (y - yn(i))) < m_point_radius)
                if (M.Length(dx, dy).LessThan(m_point_radius))
                {
                    m_dx = x.Subtract(xn(i));
                    m_dy = y.Subtract(yn(i));
                    m_node = (int)(i);
                    ret = true;
                    break;
                }
            }

            if (!ret)
            {
                for (uint i = 0; i < m_num_points; i++)
                {
                    if (check_edge(i, x, y))
                    {
                        m_dx = x;
                        m_dy = y;
                        m_edge = (int)(i);
                        ret = true;
                        break;
                    }
                }
            }

            if (!ret)
            {
                if (point_in_polygon(x, y))
                {
                    m_dx = x;
                    m_dy = y;
                    m_node = (int)(m_num_points);
                    ret = true;
                }
            }

            if (ret)
            {
                mouseEvent.Handled = true;
            }
        }

        public override void OnMouseUp(MouseEventArgs mouseEvent)
        {
            bool ret = (m_node >= 0) || (m_edge >= 0);
            m_node = -1;
            m_edge = -1;
            if (ret)
            {
                mouseEvent.Handled = ret;
            }
        }

        public override void OnMouseMove(MouseEventArgs mouseEvent)
        {
            bool ret = false;
            T dx;
            T dy;
            T x = M.New<T>(mouseEvent.X);
            T y = M.New<T>(mouseEvent.Y);
            GetTransform().Inverse.Transform(ref x, ref y);
            if (m_node == (int)(m_num_points))
            {
                dx = x.Subtract(m_dx);
                dy = y.Subtract(m_dy);
                uint i;
                for (i = 0; i < m_num_points; i++)
                {
                    SetXN(i, xn(i).Add(dx));
                    SetYN(i, yn(i).Subtract(dy));
                }
                m_dx = x;
                m_dy = y;
                ret = true;
            }
            else
            {
                if (m_edge >= 0)
                {
                    uint n1 = (uint)m_edge;
                    uint n2 = (n1 + m_num_points - 1) % m_num_points;
                    dx = x.Subtract(m_dx);
                    dy = y.Subtract(m_dy);
                    SetXN(n1, xn(n1).Add(dx));
                    SetYN(n1, yn(n1).Add(dy));
                    SetXN(n2, xn(n2).Add(dx));
                    SetYN(n2, yn(n2).Add(dy));
                    m_dx = x;
                    m_dy = y;
                    ret = true;
                }
                else
                {
                    if (m_node >= 0)
                    {
                        SetXN((uint)m_node, x.Subtract(m_dx));
                        SetYN((uint)m_node, y.Subtract(m_dy));
                        ret = true;
                    }
                }
            }
            if (ret)
            {
                mouseEvent.Handled = ret;
            }
        }

        public override void OnKeyDown(KeyEventArgs keyEvent)
        {
        }


        private bool check_edge(uint i, T x, T y)
        {
            bool ret = false;

            uint n1 = i;
            uint n2 = (i + m_num_points - 1) % m_num_points;
            T x1 = xn(n1);
            T y1 = yn(n1);
            T x2 = xn(n2);
            T y2 = yn(n2);

            T dx = x2.Subtract(x1);
            T dy = y2.Subtract(y1);

            if (M.Length(dx, dy).GreaterThan(0.0000001))
            {
                T x3 = x;
                T y3 = y;
                T x4 = x3.Subtract(dy);
                T y4 = y3.Add(dx);

                T den = y4.Subtract(y3).Multiply(x2.Subtract(x1)).Subtract(x4.Subtract(x3).Multiply(y2.Subtract(y1)));
                T u1 = x4.Subtract(x3).Multiply(y1.Subtract(y3)).Subtract(y4.Subtract(y3).Multiply(x1.Subtract(x3)).Divide(den));

                T xi = x1.Add(u1.Multiply(x2.Subtract(x1)));
                T yi = y1.Add(u1.Multiply(y2.Subtract(y1)));

                dx = xi.Subtract(x);
                dy = yi.Subtract(y);

                if (u1.GreaterThan(0.0)
                    && u1.LessThan(1.0)
                    && M.Length(dx, dy).LessThanOrEqualTo(m_point_radius))
                {
                    ret = true;
                }
            }
            return ret;
        }

        //======= Crossings Multiply algorithm of InsideTest ======================== 
        //
        // By Eric Haines, 3D/Eye Inc, erich@eye.com
        //
        // This version is usually somewhat faster than the original published in
        // Graphics Gems IV; by turning the division for testing the X axis crossing
        // into a tricky multiplication test this part of the test became faster,
        // which had the additional effect of making the test for "both to left or
        // both to right" a bit slower for triangles than simply computing the
        // intersection each time.  The main increase is in triangle testing speed,
        // which was about 15% faster; all other polygon complexities were pretty much
        // the same as before.  On machines where division is very expensive (not the
        // case on the HP 9000 series on which I tested) this test should be much
        // faster overall than the old code.  Your mileage may (in fact, will) vary,
        // depending on the machine and the test data, but in general I believe this
        // code is both shorter and faster.  This test was inspired by unpublished
        // Graphics Gems submitted by Joseph Samosky and Mark Haigh-Hutchinson.
        // Related work by Samosky is in:
        //
        // Samosky, Joseph, "SectionView: A system for interactively specifying and
        // visualizing sections through three-dimensional medical image data",
        // M.S. Thesis, Department of Electrical Engineering and Computer Science,
        // Massachusetts Institute of Technology, 1993.
        //
        // Shoot a test ray along +X axis.  The strategy is to compare vertex Y values
        // to the testing point's Y and quickly discard edges which are entirely to one
        // side of the test ray.  Note that CONVEX and WINDING code can be added as
        // for the CrossingsTest() code; it is left out here for clarity.
        //
        // Input 2D polygon _pgon_ with _numverts_ number of vertices and test point
        // _point_, returns 1 if inside, 0 if outside.
        private bool point_in_polygon(T tx, T ty)
        {
            if (m_num_points < 3) return false;
            if (!m_in_polygon_check) return false;

            uint j;
            bool yflag0, yflag1, inside_flag;
            T vtx0, vty0, vtx1, vty1;

            vtx0 = xn(m_num_points - 1);
            vty0 = yn(m_num_points - 1);

            // get test bit for above/below X axis
            yflag0 = (vty0.GreaterThanOrEqualTo(ty));

            vtx1 = xn(0);
            vty1 = yn(0);

            inside_flag = false;
            for (j = 1; j <= m_num_points; ++j)
            {
                yflag1 = (vty1.GreaterThanOrEqualTo(ty));
                // Check if endpoints straddle (are on opposite sides) of X axis
                // (i.e. the Y's differ); if so, +X ray could intersect this edge.
                // The old test also checked whether the endpoints are both to the
                // right or to the left of the test point.  However, given the faster
                // intersection point computation used below, this test was found to
                // be a break-even proposition for most polygons and a loser for
                // triangles (where 50% or more of the edges which survive this test
                // will cross quadrants and so have to have the X intersection computed
                // anyway).  I credit Joseph Samosky with inspiring me to try dropping
                // the "both left or both right" part of my code.
                if (yflag0 != yflag1)
                {
                    // Check intersection of pgon segment with +X ray.
                    // Note if >= point's X; if so, the ray hits it.
                    // The division operation is avoided for the ">=" test by checking
                    // the sign of the first vertex wrto the test point; idea inspired
                    // by Joseph Samosky's and Mark Haigh-Hutchinson's different
                    // polygon inclusion tests.
                    if ((vty1.Subtract(ty).Multiply(vtx0.Subtract(vtx1)).GreaterThanOrEqualTo(
                          vtx1.Subtract(tx).Multiply(vty0.Subtract(vty1))) == yflag1))
                    {
                        inside_flag = !inside_flag;
                    }
                }

                // Move to the next pair of vertices, retaining info as possible.
                yflag0 = yflag1;
                vtx0 = vtx1;
                vty0 = vty1;

                uint k = (j >= m_num_points) ? j - m_num_points : j;
                vtx1 = xn(k);
                vty1 = yn(k);
            }
            return inside_flag;
        }
    };



    //----------------------------------------------------------polygon_ctrl
    //template<class ColorT> 
    public class polygon_ctrl<T> : polygon_ctrl_impl<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        IColorType m_color;

        public polygon_ctrl(uint np) : this(np, M.New<T>(5)) { }

        public polygon_ctrl(uint np, double point_radius)
            : this(np, M.New<T>(point_radius))
        { }
        public polygon_ctrl(uint np, T point_radius)
            : base(np, point_radius)
        {
            m_color = new RGBA_Doubles(0.0, 0.0, 0.0);
        }

        public void line_color(IColorType c) { m_color = c; }
        public override IColorType color(uint i) { return m_color; }
    };
}
