
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
using AGG.Transform;
using NPack.Interfaces;
using NPack;

namespace AGG.VertexSource
{
    //---------------------------------------------------------------path_base
    // A container to store vertices with their flags. 
    // A path consists of a number of contours separated with "move_to" 
    // commands. The path storage can keep and maintain more than one
    // path. 
    // To navigate to the beginning of a particular path, use rewind(path_id);
    // Where path_id is what start_new_path() returns. So, when you call
    // start_new_path() you need to store its return Value somewhere else
    // to navigate to the path afterwards.
    //
    // See also: vertex_source concept
    //------------------------------------------------------------------------
    public class PathStorage<T> : IVertexSource<T>, IVertexDest<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {

        #region InternalVertexStarge
        private class VertexStorage
        {
            uint m_num_vertices;
            uint m_allocated_vertices;
            T[] m_coord_x;
            T[] m_coord_y;
            uint[] m_CommandAndFlags;

            public void FreeAll()
            {
                m_coord_x = null;
                m_coord_y = null;
                m_CommandAndFlags = null;

                m_num_vertices = 0;
            }

            public uint Size()
            {
                return m_num_vertices;
            }

            public VertexStorage()
            {
            }

            public void RemoveAll()
            {
                m_num_vertices = 0;
            }

            public void AddVertex(T x, T y, uint CommandAndFlags)
            {
                AllocateIfRequired(m_num_vertices);
                m_coord_x[m_num_vertices] = x;
                m_coord_y[m_num_vertices] = y;
                m_CommandAndFlags[m_num_vertices] = CommandAndFlags;

                m_num_vertices++;
            }

            public void ModifyVertex(uint idx, T x, T y)
            {
                m_coord_x[idx] = x;
                m_coord_y[idx] = y;
            }

            public void ModifyVertex(uint idx, T x, T y, uint CommandAndFlags)
            {
                m_coord_x[idx] = x;
                m_coord_y[idx] = y;
                m_CommandAndFlags[idx] = CommandAndFlags;
            }

            public void ModifyCommand(uint idx, uint CommandAndFlags)
            {
                m_CommandAndFlags[idx] = CommandAndFlags;
            }

            public void SwapVertices(uint v1, uint v2)
            {
                T val;

                val = m_coord_x[v1]; m_coord_x[v1] = m_coord_x[v2]; m_coord_x[v2] = val;
                val = m_coord_y[v1]; m_coord_y[v1] = m_coord_y[v2]; m_coord_y[v2] = val;
                uint cmd = m_CommandAndFlags[v1]; m_CommandAndFlags[v1] = m_CommandAndFlags[v2]; m_CommandAndFlags[v2] = cmd;
            }

            public uint LastCommand()
            {
                if (m_num_vertices != 0)
                {
                    return Command(m_num_vertices - 1);
                }

                return (uint)Path.Commands.Stop;
            }

            public uint LastVertex(out T x, out T y)
            {
                if (m_num_vertices != 0)
                {
                    return Vertex((uint)(m_num_vertices - 1), out x, out y);
                }

                x = M.Zero<T>();
                y = M.Zero<T>();
                return (uint)Path.Commands.Stop;
            }

            public uint PrevVertex(out T x, out T y)
            {
                if (m_num_vertices > 1)
                {
                    return Vertex((uint)(m_num_vertices - 2), out x, out y);
                }

                x = M.Zero<T>();
                y = M.Zero<T>();
                return (uint)Path.Commands.Stop;
            }

            public T LastX()
            {
                if (m_num_vertices > 1)
                {
                    uint idx = (uint)(m_num_vertices - 1);
                    return m_coord_x[idx];
                }

                return M.Zero<T>();
            }

            public T LastY()
            {
                if (m_num_vertices > 1)
                {
                    uint idx = (uint)(m_num_vertices - 1);
                    return m_coord_y[idx];
                }
                return M.Zero<T>();
            }

            public uint TotalVertices()
            {
                return m_num_vertices;
            }

            public uint Vertex(uint idx, out T x, out T y)
            {
                x = m_coord_x[idx];
                y = m_coord_y[idx];
                return m_CommandAndFlags[idx];
            }

            public uint Command(uint idx)
            {
                return m_CommandAndFlags[idx];
            }

            private void AllocateIfRequired(uint indexToAdd)
            {
                if (indexToAdd < m_num_vertices)
                {
                    return;
                }

                while (indexToAdd >= m_allocated_vertices)
                {
                    uint newSize = m_allocated_vertices + 256;
                    T[] newX = new T[newSize];
                    T[] newY = new T[newSize];
                    uint[] newCmd = new uint[newSize];

                    if (m_coord_x != null)
                    {
                        for (uint i = 0; i < m_num_vertices; i++)
                        {
                            newX[i] = m_coord_x[i];
                            newY[i] = m_coord_y[i];
                            newCmd[i] = m_CommandAndFlags[i];
                        }
                    }

                    m_coord_x = newX;
                    m_coord_y = newY;
                    m_CommandAndFlags = newCmd;

                    m_allocated_vertices = newSize;
                }
            }
        };
        #endregion

        private VertexStorage m_vertices;
        private uint m_iterator;

        public PathStorage()
        {
            m_vertices = new VertexStorage();
        }

        public void Add(IVector<T> vertex)
        {
            throw new System.NotImplementedException();
        }

        public uint Size()
        {
            return m_vertices.Size();
        }

        public IVector<T> this[int i]
        {
            get
            {
                throw new NotImplementedException("make this work");
            }
        }

        public void RemoveAll() { m_vertices.RemoveAll(); m_iterator = 0; }
        public void FreeAll() { m_vertices.FreeAll(); m_iterator = 0; }

        // Make path functions
        //--------------------------------------------------------------------
        public uint StartNewPath()
        {
            if (!Path.IsStop(m_vertices.LastCommand()))
            {
                m_vertices.AddVertex(M.Zero<T>(), M.Zero<T>(), (uint)Path.Commands.Stop);
            }
            return m_vertices.TotalVertices();
        }


        public void RelToAbs(ref T x, ref T y)
        {
            if (m_vertices.TotalVertices() != 0)
            {
                T x2;
                T y2;
                if (Path.IsVertex(m_vertices.LastVertex(out x2, out y2)))
                {
                    x.AddEquals(x2);
                    y.AddEquals(y2);
                }
            }
        }
        public void MoveTo(double x, double y)
        {
            MoveTo(M.New<T>(x), M.New<T>(y));
        }

        public void MoveTo(T x, T y)
        {
            m_vertices.AddVertex(x, y, (uint)Path.Commands.MoveTo);
        }

        public void MoveRel(double dx, double dy)
        {
            MoveRel(M.New<T>(dx), M.New<T>(dy));   
        }


        public void MoveRel(T dx, T dy)
        {
            RelToAbs(ref dx, ref dy);
            m_vertices.AddVertex(dx, dy, (uint)Path.Commands.MoveTo);
        }

        public void LineTo(double x, double y)
        {
            LineTo(M.New<T>(x), M.New<T>(y));
        }

        public void LineTo(T x, T y)
        {
            m_vertices.AddVertex(x, y, (uint)Path.Commands.LineTo);
        }


        public void LineRel(double dx, double dy)
        {
            LineRel(M.New<T>(dx), M.New<T>(dy));
        }

        public void LineRel(T dx, T dy)
        {
            RelToAbs(ref dx, ref dy);
            m_vertices.AddVertex(dx, dy, (uint)Path.Commands.LineTo);
        }

        public void HLineTo(T x)
        {
            m_vertices.AddVertex(x, LastY(), (uint)Path.Commands.LineTo);
        }

        public void HLineRel(T dx)
        {
            T dy = M.Zero<T>();
            RelToAbs(ref dx, ref dy);
            m_vertices.AddVertex(dx, dy, (uint)Path.Commands.LineTo);
        }

        public void VLineTo(T y)
        {
            m_vertices.AddVertex(LastX(), y, (uint)Path.Commands.LineTo);
        }

        public void VLineRel(T dy)
        {
            T dx = M.Zero<T>();
            RelToAbs(ref dx, ref dy);
            m_vertices.AddVertex(dx, dy, (uint)Path.Commands.LineTo);
        }

        /*
        public void arc_to(double rx, double ry,
                                   double angle,
                                   bool large_arc_flag,
                                   bool sweep_flag,
                                   double x, double y)
        {
            if(m_vertices.total_vertices() && is_vertex(m_vertices.last_command()))
            {
                double epsilon = 1e-30;
                double x0 = 0.0;
                double y0 = 0.0;
                m_vertices.last_vertex(&x0, &y0);

                rx = fabs(rx);
                ry = fabs(ry);

                // Ensure radii are valid
                //-------------------------
                if(rx < epsilon || ry < epsilon) 
                {
                    line_to(x, y);
                    return;
                }

                if(calc_distance(x0, y0, x, y) < epsilon)
                {
                    // If the endpoints (x, y) and (x0, y0) are identical, then this
                    // is equivalent to omitting the elliptical arc segment entirely.
                    return;
                }
                bezier_arc_svg a(x0, y0, rx, ry, angle, large_arc_flag, sweep_flag, x, y);
                if(a.radii_ok())
                {
                    join_path(a);
                }
                else
                {
                    line_to(x, y);
                }
            }
            else
            {
                move_to(x, y);
            }
        }

        public void arc_rel(double rx, double ry,
                                    double angle,
                                    bool large_arc_flag,
                                    bool sweep_flag,
                                    double dx, double dy)
        {
            rel_to_abs(&dx, &dy);
            arc_to(rx, ry, angle, large_arc_flag, sweep_flag, dx, dy);
        }
         */

        public void Curve3(double x_ctrl, double y_ctrl,
                                 double x_to, double y_to)
        {
            Curve3(M.New<T>(x_ctrl), M.New<T>(y_ctrl), M.New<T>(x_to), M.New<T>(y_to));
        }

        public void Curve3(T x_ctrl, T y_ctrl,
                                   T x_to, T y_to)
        {
            m_vertices.AddVertex(x_ctrl, y_ctrl, (uint)Path.Commands.Curve3);
            m_vertices.AddVertex(x_to, y_to, (uint)Path.Commands.Curve3);
        }

        public void Curve3Rel(T dx_ctrl, T dy_ctrl, T dx_to, T dy_to)
        {
            RelToAbs(ref dx_ctrl, ref dy_ctrl);
            RelToAbs(ref dx_to, ref dy_to);
            m_vertices.AddVertex(dx_ctrl, dy_ctrl, (uint)Path.Commands.Curve3);
            m_vertices.AddVertex(dx_to, dy_to, (uint)Path.Commands.Curve3);
        }

        public void Curve3(double x_to, double y_to)
        {
            Curve3(M.New<T>(x_to), M.New<T>(y_to));
        }

        public void Curve3(T x_to, T y_to)
        {
            T x0;
            T y0;
            if (Path.IsVertex(m_vertices.LastVertex(out x0, out y0)))
            {
                T x_ctrl;
                T y_ctrl;
                uint cmd = m_vertices.PrevVertex(out x_ctrl, out y_ctrl);
                if (Path.IsCurve(cmd))
                {
                    x_ctrl = x0.Add(x0).Subtract(x_ctrl);
                    y_ctrl = y0.Add(y0).Subtract(y_ctrl);
                }
                else
                {
                    x_ctrl = x0;
                    y_ctrl = y0;
                }
                Curve3(x_ctrl, y_ctrl, x_to, y_to);
            }
        }

        public void Curve3Rel(T dx_to, T dy_to)
        {
            RelToAbs(ref dx_to, ref dy_to);
            Curve3(dx_to, dy_to);
        }

        public void Curve4(T x_ctrl1, T y_ctrl1,
                                   T x_ctrl2, T y_ctrl2,
                                   T x_to, T y_to)
        {
            m_vertices.AddVertex(x_ctrl1, y_ctrl1, (uint)Path.Commands.Curve4);
            m_vertices.AddVertex(x_ctrl2, y_ctrl2, (uint)Path.Commands.Curve4);
            m_vertices.AddVertex(x_to, y_to, (uint)Path.Commands.Curve4);
        }

        public void Curve4Rel(T dx_ctrl1, T dy_ctrl1,
                                       T dx_ctrl2, T dy_ctrl2,
                                       T dx_to, T dy_to)
        {
            RelToAbs(ref dx_ctrl1, ref dy_ctrl1);
            RelToAbs(ref dx_ctrl2, ref dy_ctrl2);
            RelToAbs(ref dx_to, ref dy_to);
            m_vertices.AddVertex(dx_ctrl1, dy_ctrl1, (uint)Path.Commands.Curve4);
            m_vertices.AddVertex(dx_ctrl2, dy_ctrl2, (uint)Path.Commands.Curve4);
            m_vertices.AddVertex(dx_to, dy_to, (uint)Path.Commands.Curve4);
        }

        public void Curve4(T x_ctrl2, T y_ctrl2,
                                   T x_to, T y_to)
        {
            T x0;
            T y0;
            if (Path.IsVertex(LastVertex(out x0, out y0)))
            {
                T x_ctrl1;
                T y_ctrl1;
                uint cmd = PrevVertex(out x_ctrl1, out y_ctrl1);
                if (Path.IsCurve(cmd))
                {
                    x_ctrl1 = x0.Add(x0).Subtract(x_ctrl1);
                    y_ctrl1 = y0.Add(y0).Subtract(y_ctrl1);
                }
                else
                {
                    x_ctrl1 = x0;
                    y_ctrl1 = y0;
                }
                Curve4(x_ctrl1, y_ctrl1, x_ctrl2, y_ctrl2, x_to, y_to);
            }
        }

        public void Curve4Rel(T dx_ctrl2, T dy_ctrl2,
                                       T dx_to, T dy_to)
        {
            RelToAbs(ref dx_ctrl2, ref dy_ctrl2);
            RelToAbs(ref dx_to, ref dy_to);
            Curve4(dx_ctrl2, dy_ctrl2, dx_to, dy_to);
        }

        public uint TotalVertices()
        {
            return m_vertices.TotalVertices();
        }

        public uint LastVertex(out T x, out T y)
        {
            return m_vertices.LastVertex(out x, out y);
        }

        public uint PrevVertex(out T x, out T y)
        {
            return m_vertices.PrevVertex(out x, out y);
        }

        public T LastX()
        {
            return m_vertices.LastX();
        }

        public T LastY()
        {
            return m_vertices.LastY();
        }

        public uint Vertex(uint idx, out T x, out T y)
        {
            return m_vertices.Vertex(idx, out x, out y);
        }

        public uint Command(uint idx)
        {
            return m_vertices.Command(idx);
        }

        public void ModifyVertex(uint idx, T x, T y)
        {
            m_vertices.ModifyVertex(idx, x, y);
        }

        public void ModifyVertex(uint idx, T x, T y, uint PathAndFlags)
        {
            m_vertices.ModifyVertex(idx, x, y, PathAndFlags);
        }

        public void ModifyCommand(uint idx, uint PathAndFlags)
        {
            m_vertices.ModifyCommand(idx, PathAndFlags);
        }

        public virtual void Rewind(uint path_id)
        {
            m_iterator = path_id;
        }

        public uint Vertex(out T x, out T y)
        {
            if (m_iterator >= m_vertices.TotalVertices())
            {
                x = M.Zero<T>();
                y = M.Zero<T>();
                return (uint)Path.Commands.Stop;
            }

            return m_vertices.Vertex(m_iterator++, out x, out y);
        }

        // Arrange the orientation of a polygon, all polygons in a path, 
        // or in all paths. After calling arrange_orientations() or 
        // arrange_orientations_all_paths(), all the polygons will have 
        // the same orientation, i.e. path_flags_cw or path_flags_ccw
        //--------------------------------------------------------------------
        public uint ArrangePolygonOrientation(uint start, Path.Flags orientation)
        {
            if (orientation == Path.Flags.None) return start;

            // Skip all non-vertices at the beginning
            while (start < m_vertices.TotalVertices() &&
                  !Path.IsVertex(m_vertices.Command(start))) ++start;

            // Skip all insignificant move_to
            while (start + 1 < m_vertices.TotalVertices() &&
                  Path.IsMoveTo(m_vertices.Command(start)) &&
                  Path.IsMoveTo(m_vertices.Command(start + 1))) ++start;

            // Find the last vertex
            uint end = start + 1;
            while (end < m_vertices.TotalVertices() &&
                  !Path.IsNextPoly(m_vertices.Command(end))) ++end;

            if (end - start > 2)
            {
                if (PerceivePolygonOrientation(start, end) != orientation)
                {
                    // Invert polygon, set orientation flag, and skip all end_poly
                    InvertPolygon(start, end);
                    uint PathAndFlags;
                    while (end < m_vertices.TotalVertices() &&
                          Path.IsEndPoly(PathAndFlags = m_vertices.Command(end)))
                    {
                        m_vertices.ModifyCommand(end++, PathAndFlags | (uint)orientation);// Path.set_orientation(cmd, orientation));
                    }
                }
            }
            return end;
        }

        public uint ArrangeOrientations(uint start, Path.Flags orientation)
        {
            if (orientation != Path.Flags.None)
            {
                while (start < m_vertices.TotalVertices())
                {
                    start = ArrangePolygonOrientation(start, orientation);
                    if (Path.IsStop(m_vertices.Command(start)))
                    {
                        ++start;
                        break;
                    }
                }
            }
            return start;
        }

        public void ArrangeOrientationsAllPaths(Path.Flags orientation)
        {
            if (orientation != Path.Flags.None)
            {
                uint start = 0;
                while (start < m_vertices.TotalVertices())
                {
                    start = ArrangeOrientations(start, orientation);
                }
            }
        }

        // Flip all vertices horizontally or vertically, 
        // between x1 and x2, or between y1 and y2 respectively
        //--------------------------------------------------------------------
        public void FlipX(T x1, T x2)
        {
            uint i;
            T x, y;
            for (i = 0; i < m_vertices.TotalVertices(); i++)
            {
                uint PathAndFlags = m_vertices.Vertex(i, out x, out y);
                if (Path.IsVertex(PathAndFlags))
                {
                    m_vertices.ModifyVertex(i, x2.Subtract(x).Add(x1), y);
                }
            }
        }

        public void FlipY(T y1, T y2)
        {
            uint i;
            T x, y;
            for (i = 0; i < m_vertices.TotalVertices(); i++)
            {
                uint PathAndFlags = m_vertices.Vertex(i, out x, out y);
                if (Path.IsVertex(PathAndFlags))
                {
                    m_vertices.ModifyVertex(i, x, y2.Subtract(y).Add(y1));
                }
            }
        }

        public void EndPoly()
        {
            ClosePolygon((uint)Path.Flags.Close);
        }

        public void EndPoly(uint flags)
        {
            if (Path.IsVertex(m_vertices.LastCommand()))
            {
                m_vertices.AddVertex(M.Zero<T>(), M.Zero<T>(), (uint)Path.Commands.EndPoly | flags);
            }
        }


        public void ClosePolygon()
        {
            ClosePolygon((uint)Path.Flags.None);
        }

        public void ClosePolygon(uint flags)
        {
            EndPoly((uint)Path.Flags.Close | flags);
        }

        // Concatenate path. The path is added as is.
        public void ConcatPath(IVertexSource<T> vs)
        {
            ConcatPath(vs, 0);
        }

        public void ConcatPath(IVertexSource<T> vs, uint path_id)
        {
            T x, y;
            uint PathAndFlags;
            vs.Rewind(path_id);
            while (!Path.IsStop(PathAndFlags = vs.Vertex(out x, out y)))
            {
                m_vertices.AddVertex(x, y, PathAndFlags);
            }
        }

        //--------------------------------------------------------------------
        // Join path. The path is joined with the existing one, that is, 
        // it behaves as if the pen of a plotter was always down (drawing)
        //template<class VertexSource> 
        public void JoinPath(PathStorage<T> vs)
        {
            JoinPath(vs, 0);

        }

        public void JoinPath(PathStorage<T> vs, uint path_id)
        {
            T x, y;
            vs.Rewind(path_id);
            uint PathAndFlags = vs.Vertex(out x, out y);
            if (!Path.IsStop(PathAndFlags))
            {
                if (Path.IsVertex(PathAndFlags))
                {
                    T x0, y0;
                    uint PathAndFlags0 = LastVertex(out x0, out y0);
                    if (Path.IsVertex(PathAndFlags0))
                    {
                        if (MathUtil.CalcDistance(x, y, x0, y0).GreaterThan(MathUtil.VertexDistEpsilon))
                        {
                            if (Path.IsMoveTo(PathAndFlags)) PathAndFlags = (uint)Path.Commands.LineTo;
                            m_vertices.AddVertex(x, y, PathAndFlags);
                        }
                    }
                    else
                    {
                        if (Path.IsStop(PathAndFlags0))
                        {
                            PathAndFlags = (uint)Path.Commands.MoveTo;
                        }
                        else
                        {
                            if (Path.IsMoveTo(PathAndFlags)) PathAndFlags = (uint)Path.Commands.LineTo;
                        }
                        m_vertices.AddVertex(x, y, PathAndFlags);
                    }
                }
                while (!Path.IsStop(PathAndFlags = vs.Vertex(out x, out y)))
                {
                    m_vertices.AddVertex(x, y, Path.IsMoveTo(PathAndFlags) ?
                                                    (uint)Path.Commands.LineTo :
                                                    PathAndFlags);
                }
            }
        }

        /*
        // Concatenate polygon/polyline. 
        //--------------------------------------------------------------------
        void concat_poly(T* data, uint num_points, bool closed)
        {
            poly_plain_adaptor<T> poly(data, num_points, closed);
            concat_path(poly);
        }

        // Join polygon/polyline continuously.
        //--------------------------------------------------------------------
        void join_poly(T* data, uint num_points, bool closed)
        {
            poly_plain_adaptor<T> poly(data, num_points, closed);
            join_path(poly);
        }
         */

        //--------------------------------------------------------------------
        public void Translate(T dx, T dy)
        {
            Translate(dx, dy, 0);
        }

        public void Translate(T dx, T dy, uint path_id)
        {
            uint num_ver = m_vertices.TotalVertices();
            for (; path_id < num_ver; path_id++)
            {
                T x, y;
                uint PathAndFlags = m_vertices.Vertex(path_id, out x, out y);
                if (Path.IsStop(PathAndFlags)) break;
                if (Path.IsVertex(PathAndFlags))
                {
                    x.AddEquals(dx);
                    y.AddEquals(dy);
                    m_vertices.ModifyVertex(path_id, x, y);
                }
            }
        }

        public void TranslateAllPaths(T dx, T dy)
        {
            uint idx;
            uint num_ver = m_vertices.TotalVertices();
            for (idx = 0; idx < num_ver; idx++)
            {
                T x, y;
                if (Path.IsVertex(m_vertices.Vertex(idx, out x, out y)))
                {
                    x.AddEquals(dx);
                    y.AddEquals(dy);
                    m_vertices.ModifyVertex(idx, x, y);
                }
            }
        }

        //--------------------------------------------------------------------
        public void Transform(IAffineTransformMatrix<T> trans)
        {
            Transform(trans, 0);
        }

        public void Transform(IAffineTransformMatrix<T> trans, uint path_id)
        {
            uint num_ver = m_vertices.TotalVertices();
            for (; path_id < num_ver; path_id++)
            {
                T x, y;
                uint PathAndFlags = m_vertices.Vertex(path_id, out x, out y);
                if (Path.IsStop(PathAndFlags)) break;
                if (Path.IsVertex(PathAndFlags))
                {
                    //trans.Transform(ref x, ref y);
                    //m_vertices.ModifyVertex(path_id, x, y);
                    IVector<T> v1 = trans.TransformVector(MatrixFactory<T>.CreateVector2D(x, y));
                    m_vertices.ModifyVertex(path_id, v1[0], v1[1]);

                }
            }
        }

        //--------------------------------------------------------------------
        public void TransformAllPaths(IAffineTransformMatrix<T> trans)
        {
            uint idx;
            uint num_ver = m_vertices.TotalVertices();
            for (idx = 0; idx < num_ver; idx++)
            {
                T x, y;
                if (Path.IsVertex(m_vertices.Vertex(idx, out x, out y)))
                {
                    //trans.Transform(ref x, ref y);
                    //m_vertices.ModifyVertex(idx, x, y);

                    IVector<T> v = trans.TransformVector(MatrixFactory<T>.CreateVector2D(x, y));
                    m_vertices.ModifyVertex(idx, v[0], v[1]);
                }
            }
        }

        public void InvertPolygon(uint start)
        {
            // Skip all non-vertices at the beginning
            while (start < m_vertices.TotalVertices() &&
                  !Path.IsVertex(m_vertices.Command(start))) ++start;

            // Skip all insignificant move_to
            while (start + 1 < m_vertices.TotalVertices() &&
                  Path.IsMoveTo(m_vertices.Command(start)) &&
                  Path.IsMoveTo(m_vertices.Command(start + 1))) ++start;

            // Find the last vertex
            uint end = start + 1;
            while (end < m_vertices.TotalVertices() &&
                  !Path.IsNextPoly(m_vertices.Command(end))) ++end;

            InvertPolygon(start, end);
        }

        private Path.Flags PerceivePolygonOrientation(uint start, uint end)
        {
            // Calculate signed area (double area to be exact)
            //---------------------
            uint np = end - start;
            T area = M.Zero<T>();
            uint i;
            for (i = 0; i < np; i++)
            {
                T x1, y1, x2, y2;
                m_vertices.Vertex(start + i, out x1, out y1);
                m_vertices.Vertex(start + (i + 1) % np, out x2, out y2);
                area.AddEquals(x1.Multiply(y2).Subtract(y1.Multiply(x2)));
            }
            return (area.LessThan(0.0)) ? Path.Flags.CW : Path.Flags.CCW;
        }

        private void InvertPolygon(uint start, uint end)
        {
            uint i;
            uint tmp_PathAndFlags = m_vertices.Command(start);

            --end; // Make "end" inclusive

            // Shift all commands to one position
            for (i = start; i < end; i++)
            {
                m_vertices.ModifyCommand(i, m_vertices.Command(i + 1));
            }

            // Assign starting command to the ending command
            m_vertices.ModifyCommand(end, tmp_PathAndFlags);

            // Reverse the polygon
            while (end > start)
            {
                m_vertices.SwapVertices(start++, end--);
            }
        }
    };
}