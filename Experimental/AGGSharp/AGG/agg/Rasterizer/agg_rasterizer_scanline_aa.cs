
using System;
using AGG.Gamma;
using AGG.Scanline;
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
//
// The author gratefully acknowleges the support of David Turner, 
// Robert Wilhelm, and Werner Lemberg - the authors of the FreeType 
// libray - in producing this work. See http://www.freetype.org for details.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
//
// Adaptation for 32-bit screen coordinates has been sponsored by 
// Liberty Technology Systems, Inc., visit http://lib-sys.com
//
// Liberty Technology Systems, Inc. is the provider of
// PostScript and PDF technology for software developers.
// 
//----------------------------------------------------------------------------
using AGG.VertexSource;
using NPack.Interfaces;
using filling_rule_e = AGG.FillingRule;
using poly_subpixel_scale_e = AGG.PolySubPixelScale;

namespace AGG.Rasterizer
{
    //==================================================rasterizer_scanline_aa
    // Polygon rasterizer that is used to render filled polygons with 
    // high-quality Anti-Aliasing. Internally, by default, the class uses 
    // integer coordinates in format 24.8, i.e. 24 bits for integer part 
    // and 8 bits for fractional - see poly_subpixel_shift. This class can be 
    // used in the following  way:
    //
    // 1. filling_rule(filling_rule_e ft) - optional.
    //
    // 2. gamma() - optional.
    //
    // 3. reset()
    //
    // 4. move_to(x, y) / line_to(x, y) - make the polygon. One can create 
    //    more than one contour, but each contour must consist of at least 3
    //    vertices, i.e. move_to(x1, y1); line_to(x2, y2); line_to(x3, y3);
    //    is the absolute minimum of vertices that define a triangle.
    //    The algorithm does not check either the number of vertices nor
    //    coincidence of their coordinates, but in the worst case it just 
    //    won't draw anything.
    //    The orger of the vertices (clockwise or counterclockwise) 
    //    is important when using the non-zero filling rule (fill_non_zero).
    //    In this case the vertex order of all the contours must be the same
    //    if you want your intersecting polygons to be without "holes".
    //    You actually can use different vertices order. If the contours do not 
    //    intersect each other the order is not important anyway. If they do, 
    //    contours with the same vertex order will be rendered without "holes" 
    //    while the intersecting contours with different orders will have "holes".
    //
    // filling_rule() and gamma() can be called anytime before "sweeping".
    //------------------------------------------------------------------------
    public interface IRasterizer<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable
    {
        int MinX { get; }
        int MinY { get; }
        int MaxX { get; }
        int MaxY { get; }

        void Gamma(IGammaFunction gamma_function);

        bool SweepScanline(IScanlineCache sl);
        void Reset();
        void AddPath(IVertexSource<T> vs);
        void AddPath(IVertexSource<T> vs, uint path_id);
        bool RewindScanlines();
    }

    public sealed class RasterizerScanlineAA<T> : IRasterizer<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private RasterizerCellsAA m_outline;
        private IVectorClipper<T> m_VectorClipper;
        private int[] m_gamma = new int[(int)AAScale.Scale];
        private FillingRule m_filling_rule;
        private bool m_auto_close;
        private int m_start_x;
        private int m_start_y;
        private ScanlineStatus m_status;
        private int m_scan_y;

        public enum ScanlineStatus
        {
            Initial,
            MoveTo,
            LineTo,
            Closed
        };

        public enum AAScale
        {
            Shift = 8,
            Scale = 1 << Shift,
            Mask = Scale - 1,
            Scale2 = Scale * 2,
            Mask2 = Scale2 - 1
        };

        public RasterizerScanlineAA()
            : this(new VectorClipper<T>())
        {
        }

        //--------------------------------------------------------------------
        public RasterizerScanlineAA(IVectorClipper<T> rasterizer_sl_clip)
        {
            m_outline = new RasterizerCellsAA();
            m_VectorClipper = rasterizer_sl_clip;
            m_filling_rule = filling_rule_e.NonZero;
            m_auto_close = true;
            m_start_x = 0;
            m_start_y = 0;
            m_status = ScanlineStatus.Initial;

            int i;
            for (i = 0; i < (int)AAScale.Scale; i++) m_gamma[i] = i;
        }

        /*
        //--------------------------------------------------------------------
        public rasterizer_scanline_aa(IClipper rasterizer_sl_clip, IGammaFunction gamma_function)
        {
            m_outline = new rasterizer_cells_aa();
            m_clipper = rasterizer_sl_clip;
            m_filling_rule = filling_rule_e.fill_non_zero;
            m_auto_close = true;
            m_start_x = 0;
            m_start_y = 0;
            m_status = status.status_initial;

            gamma(gamma_function);
        }*/

        //--------------------------------------------------------------------
        public void Reset()
        {
            m_outline.Reset();
            m_status = ScanlineStatus.Initial;
        }

        public void reset_clipping()
        {
            Reset();
            m_VectorClipper.ResetClipping();
        
        }

        public void SetVectorClipBox(double x1, double y1, double x2, double y2)
        {
            SetVectorClipBox(M.New<T>(x1), M.New<T>(y1), M.New<T>(x2), M.New<T>(y2));    
        }


        public void SetVectorClipBox(T x1, T y1, T x2, T y2)
        {
            Reset();
            m_VectorClipper.ClipBox(m_VectorClipper.Upscale(x1), m_VectorClipper.Upscale(y1),
                               m_VectorClipper.Upscale(x2), m_VectorClipper.Upscale(y2));
        }

        public FillingRule FillingRule
        {
            set
            {
                m_filling_rule = value;
            }
        }

        public bool AutoClose { set { m_auto_close = value; } }

        //--------------------------------------------------------------------
        public void Gamma(IGammaFunction gamma_function)
        {
            for (int i = 0; i < (int)AAScale.Scale; i++)
            {
                m_gamma[i] = (int)Basics.RoundUint(gamma_function.GetGamma((double)(i) / (int)AAScale.Mask) * (int)AAScale.Mask);
            }
        }

        /*
        //--------------------------------------------------------------------
        public uint apply_gamma(uint cover) 
        { 
        	return (uint)m_gamma[cover];
        }
         */

        //--------------------------------------------------------------------
        void MoveToInt(int x, int y)
        {
            if (m_outline.Sorted()) Reset();
            if (m_auto_close) ClosePolygon();
            m_VectorClipper.MoveTo(m_start_x = m_VectorClipper.Downscale(x),
                              m_start_y = m_VectorClipper.Downscale(y));
            m_status = ScanlineStatus.MoveTo;
        }

        //------------------------------------------------------------------------
        void LineToInt(int x, int y)
        {
            m_VectorClipper.LineTo(m_outline,
                              m_VectorClipper.Downscale(x),
                              m_VectorClipper.Downscale(y));
            m_status = ScanlineStatus.LineTo;
        }

        //------------------------------------------------------------------------
        public void MoveToDbl(T x, T y)
        {
            if (m_outline.Sorted()) Reset();
            if (m_auto_close) ClosePolygon();
            m_VectorClipper.MoveTo(m_start_x = m_VectorClipper.Upscale(x),
                              m_start_y = m_VectorClipper.Upscale(y));
            m_status = ScanlineStatus.MoveTo;
        }

        //------------------------------------------------------------------------
        public void LineToDbl(T x, T y)
        {
            m_VectorClipper.LineTo(m_outline,
                              m_VectorClipper.Upscale(x),
                              m_VectorClipper.Upscale(y));
            m_status = ScanlineStatus.LineTo;
            //DebugFile.Print("x=" + x.ToString() + " y=" + y.ToString() + "\n");
        }

        public void ClosePolygon()
        {
            if (m_status == ScanlineStatus.LineTo)
            {
                m_VectorClipper.LineTo(m_outline, m_start_x, m_start_y);
                m_status = ScanlineStatus.Closed;
            }
        }

        void add_vertex(T x, T y, uint PathAndFlags)
        {
            if (Path.IsMoveTo(PathAndFlags))
            {
                MoveToDbl(x, y);
            }
            else
            {
                if (Path.IsVertex(PathAndFlags))
                {
                    LineToDbl(x, y);
                }
                else
                {
                    if (Path.IsClose(PathAndFlags))
                    {
                        ClosePolygon();
                    }
                }
            }
        }
        //------------------------------------------------------------------------
        void EdgeInt(int x1, int y1, int x2, int y2)
        {
            if (m_outline.Sorted()) Reset();
            m_VectorClipper.MoveTo(m_VectorClipper.Downscale(x1), m_VectorClipper.Downscale(y1));
            m_VectorClipper.LineTo(m_outline,
                              m_VectorClipper.Downscale(x2),
                              m_VectorClipper.Downscale(y2));
            m_status = ScanlineStatus.MoveTo;
        }

        //------------------------------------------------------------------------
        void EdgeDbl(T x1, T y1, T x2, T y2)
        {
            if (m_outline.Sorted()) Reset();
            m_VectorClipper.MoveTo(m_VectorClipper.Upscale(x1), m_VectorClipper.Upscale(y1));
            m_VectorClipper.LineTo(m_outline,
                              m_VectorClipper.Upscale(x2),
                              m_VectorClipper.Upscale(y2));
            m_status = ScanlineStatus.MoveTo;
        }

        //-------------------------------------------------------------------
        public void AddPath(IVertexSource<T> vs)
        {
            AddPath(vs, 0);
        }

        public void AddPath(IVertexSource<T> vs, uint path_id)
        {
            T x = M.Zero<T>();
            T y = M.Zero<T>();

            uint PathAndFlags;
            vs.Rewind(path_id);
            if (m_outline.Sorted())
            {
                Reset();
            }

            while (!Path.IsStop(PathAndFlags = vs.Vertex(out x, out y)))
            {
                add_vertex(x, y, PathAndFlags);
            }

            //DebugFile.Print("Test");
        }

        //--------------------------------------------------------------------
        public int MinX { get { return m_outline.MinX; } }
        public int MinY { get { return m_outline.MinY; } }
        public int MaxX { get { return m_outline.MaxX; } }
        public int MaxY { get { return m_outline.MaxY; } }

        //--------------------------------------------------------------------
        void Sort()
        {
            if (m_auto_close) ClosePolygon();
            m_outline.SortCells();
        }

        //------------------------------------------------------------------------
        public bool RewindScanlines()
        {
            if (m_auto_close) ClosePolygon();
            m_outline.SortCells();
            if (m_outline.TotalCells == 0)
            {
                return false;
            }
            m_scan_y = m_outline.MinY;
            return true;
        }

        //------------------------------------------------------------------------
        bool NavigateScanline(int y)
        {
            if (m_auto_close) ClosePolygon();
            m_outline.SortCells();
            if (m_outline.TotalCells == 0 ||
               y < m_outline.MinY ||
               y > m_outline.MaxY)
            {
                return false;
            }
            m_scan_y = y;
            return true;
        }

        //--------------------------------------------------------------------
        public uint CalculateAlpha(int area)
        {
            int cover = area >> ((int)poly_subpixel_scale_e.Shift * 2 + 1 - (int)AAScale.Shift);

            if (cover < 0) cover = -cover;
            if (m_filling_rule == filling_rule_e.EvenOdd)
            {
                cover &= (int)AAScale.Mask2;
                if (cover > (int)AAScale.Scale)
                {
                    cover = (int)AAScale.Scale2 - cover;
                }
            }
            if (cover > (int)AAScale.Mask) cover = (int)AAScale.Mask;
            unsafe
            {
                return (uint)m_gamma[cover];
            }
        }

#if use_timers
        static CNamedTimer SweepSacanLine = new CNamedTimer("SweepSacanLine");
#endif
        //--------------------------------------------------------------------
        public bool SweepScanline(IScanlineCache sl)
        {
#if use_timers
            SweepSacanLine.Start();
#endif
            for (; ; )
            {
                if (m_scan_y > m_outline.MaxY)
                {
#if use_timers
                    SweepSacanLine.Stop();
#endif
                    return false;
                }

                sl.ResetSpans();
                uint scan_y_uint = 0; // it is going to get initialize to 0 anyway so make it clear.
                if (m_scan_y > 0)
                {
                    scan_y_uint = (uint)m_scan_y;
                }
                uint num_cells = m_outline.ScanlineNumCells(scan_y_uint);
                CellAA[] cells;
                uint Offset;
                m_outline.ScanlineCells(scan_y_uint, out cells, out Offset);
                int cover = 0;

                while (num_cells != 0)
                {
                    CellAA cur_cell = cells[Offset];
                    int x = cur_cell.X;
                    int area = cur_cell.Area;
                    uint alpha;

                    cover += cur_cell.Cover;

                    //accumulate all cells with the same X
                    while (--num_cells != 0)
                    {
                        Offset++;
                        cur_cell = cells[Offset];
                        if (cur_cell.X != x)
                        {
                            break;
                        }

                        area += cur_cell.Area;
                        cover += cur_cell.Cover;
                    }

                    if (area != 0)
                    {
                        alpha = CalculateAlpha((cover << ((int)poly_subpixel_scale_e.Shift + 1)) - area);
                        if (alpha != 0)
                        {
                            sl.AddCell(x, alpha);
                        }
                        x++;
                    }

                    if ((num_cells != 0) && (cur_cell.X > x))
                    {
                        alpha = CalculateAlpha(cover << ((int)poly_subpixel_scale_e.Shift + 1));
                        if (alpha != 0)
                        {
                            sl.AddSpan(x, (cur_cell.X - x), alpha);
                        }
                    }
                }

                if (sl.NumSpans != 0) break;
                ++m_scan_y;
            }

            sl.Finalize(m_scan_y);
            ++m_scan_y;
#if use_timers
            SweepSacanLine.Stop();
#endif
            return true;
        }

        //--------------------------------------------------------------------
        bool HitTest(int tx, int ty)
        {
            if (!NavigateScanline(ty)) return false;
            //scanline_hit_test sl(tx);
            //sweep_scanline(sl);
            //return sl.hit();
            return true;
        }
    };
}
