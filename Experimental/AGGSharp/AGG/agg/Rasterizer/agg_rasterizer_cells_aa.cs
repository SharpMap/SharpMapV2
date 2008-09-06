
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
using poly_subpixel_scale_e = AGG.PolySubPixelScale;

namespace AGG.Rasterizer
{
    //-----------------------------------------------------------------cell_aa
    // A pixel cell. There're no constructors defined and it was done 
    // intentionally in order to avoid extra overhead when allocating an 
    // array of cells.
    public struct CellAA// : IPixelCell
    {
        private int x;

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        private int y;

        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        private int cover;

        public int Cover
        {
            get { return cover; }
            set { cover = value; }
        }
        private int area;

        public int Area
        {
            get { return area; }
            set { area = value; }
        }
        private short left, right;

        public short Right
        {
            get { return right; }
            set { right = value; }
        }

        public short Left
        {
            get { return left; }
            set { left = value; }
        }

        public void Initial()
        {
            X = 0x7FFFFFFF;
            Y = 0x7FFFFFFF;
            Cover = 0;
            Area = 0;
            Left = -1;
            Right = -1;
        }

        public void Set(CellAA cellB)
        {
            X = cellB.X;
            Y = cellB.Y;
            Cover = cellB.Cover;
            Area = cellB.Area;
            Left = cellB.Left;
            Right = cellB.Right;
        }

        public void Style(CellAA cellB)
        {
            Left = cellB.Left;
            Right = cellB.Right;
        }

        public bool NotEqual(int ex, int ey, CellAA cell)
        {
            unchecked
            {
                return ((ex - X) | (ey - Y) | (Left - cell.Left) | (Right - cell.Right)) != 0;
            }
        }
    };

    //-----------------------------------------------------rasterizer_cells_aa
    // An internal class that implements the main rasterization algorithm.
    // Used in the rasterizer. Should not be used directly.
    //template<class Cell> 
    public sealed class RasterizerCellsAA
    {
        private uint m_num_used_cells;
        private VectorPOD<CellAA> m_cells;
        private VectorPOD<CellAA> m_sorted_cells;
        private VectorPOD<sorted_y> m_sorted_y;
        private QuickSortCellAA m_QSorter;

        CellAA m_curr_cell;
        CellAA m_style_cell;
        int m_min_x;
        int m_min_y;
        int m_max_x;
        int m_max_y;
        bool m_sorted;

        private enum cell_block_scale_e
        {
            cell_block_shift = 12,
            cell_block_size = 1 << cell_block_shift,
            cell_block_mask = cell_block_size - 1,
            cell_block_pool = 256,
            cell_block_limit = 1024 * cell_block_size
        };

        private struct sorted_y
        {
            internal uint start;
            internal uint num;
        };

        public RasterizerCellsAA()
        {
            m_QSorter = new QuickSortCellAA();
            m_sorted_cells = new VectorPOD<CellAA>();
            m_sorted_y = new VectorPOD<sorted_y>();
            m_min_x = (0x7FFFFFFF);
            m_min_y = (0x7FFFFFFF);
            m_max_x = (-0x7FFFFFFF);
            m_max_y = (-0x7FFFFFFF);
            m_sorted = (false);

            m_style_cell.Initial();
            m_curr_cell.Initial();
        }

        public void Reset()
        {
            m_num_used_cells = 0;

            m_curr_cell.Initial();
            m_style_cell.Initial();
            m_sorted = false;
            m_min_x = 0x7FFFFFFF;
            m_min_y = 0x7FFFFFFF;
            m_max_x = -0x7FFFFFFF;
            m_max_y = -0x7FFFFFFF;
        }

        public void Style(CellAA style_cell)
        {
            m_style_cell.Style(style_cell);
        }

        enum dx_limit_e { dx_limit = 16384 << PolySubPixelScale.Shift };

        public void Line(int x1, int y1, int x2, int y2)
        {
            int poly_subpixel_shift = (int)PolySubPixelScale.Shift;
            int poly_subpixel_mask = (int)PolySubPixelScale.Mask;
            int poly_subpixel_scale = (int)PolySubPixelScale.Scale;
            int dx = x2 - x1;

            if (dx >= (int)dx_limit_e.dx_limit || dx <= -(int)dx_limit_e.dx_limit)
            {
                int cx = (x1 + x2) >> 1;
                int cy = (y1 + y2) >> 1;
                Line(x1, y1, cx, cy);
                Line(cx, cy, x2, y2);
            }

            int dy = y2 - y1;
            int ex1 = x1 >> poly_subpixel_shift;
            int ex2 = x2 >> poly_subpixel_shift;
            int ey1 = y1 >> poly_subpixel_shift;
            int ey2 = y2 >> poly_subpixel_shift;
            int fy1 = y1 & poly_subpixel_mask;
            int fy2 = y2 & poly_subpixel_mask;

            int x_from, x_to;
            int p, rem, mod, lift, delta, first, incr;

            if (ex1 < m_min_x) m_min_x = ex1;
            if (ex1 > m_max_x) m_max_x = ex1;
            if (ey1 < m_min_y) m_min_y = ey1;
            if (ey1 > m_max_y) m_max_y = ey1;
            if (ex2 < m_min_x) m_min_x = ex2;
            if (ex2 > m_max_x) m_max_x = ex2;
            if (ey2 < m_min_y) m_min_y = ey2;
            if (ey2 > m_max_y) m_max_y = ey2;

            SetCurrCell(ex1, ey1);

            //everything is on a single horizontal line
            if (ey1 == ey2)
            {
                RenderHLine(ey1, x1, fy1, x2, fy2);
                return;
            }

            //Vertical line - we have to calculate start and end cells,
            //and then - the common values of the area and coverage for
            //all cells of the line. We know exactly there's only one 
            //cell, so, we don't have to call render_hline().
            incr = 1;
            if (dx == 0)
            {
                int ex = x1 >> poly_subpixel_shift;
                int two_fx = (x1 - (ex << poly_subpixel_shift)) << 1;
                int area;

                first = poly_subpixel_scale;
                if (dy < 0)
                {
                    first = 0;
                    incr = -1;
                }

                x_from = x1;

                delta = first - fy1;
                m_curr_cell.Cover += delta;
                m_curr_cell.Area += two_fx * delta;

                ey1 += incr;
                SetCurrCell(ex, ey1);

                delta = first + first - poly_subpixel_scale;
                area = two_fx * delta;
                while (ey1 != ey2)
                {
                    m_curr_cell.Cover = delta;
                    m_curr_cell.Area = area;
                    ey1 += incr;
                    SetCurrCell(ex, ey1);
                }
                delta = fy2 - poly_subpixel_scale + first;
                m_curr_cell.Cover += delta;
                m_curr_cell.Area += two_fx * delta;
                return;
            }

            //ok, we have to render several hlines
            p = (poly_subpixel_scale - fy1) * dx;
            first = poly_subpixel_scale;

            if (dy < 0)
            {
                p = fy1 * dx;
                first = 0;
                incr = -1;
                dy = -dy;
            }

            delta = p / dy;
            mod = p % dy;

            if (mod < 0)
            {
                delta--;
                mod += dy;
            }

            x_from = x1 + delta;
            RenderHLine(ey1, x1, fy1, x_from, first);

            ey1 += incr;
            SetCurrCell(x_from >> poly_subpixel_shift, ey1);

            if (ey1 != ey2)
            {
                p = poly_subpixel_scale * dx;
                lift = p / dy;
                rem = p % dy;

                if (rem < 0)
                {
                    lift--;
                    rem += dy;
                }
                mod -= dy;

                while (ey1 != ey2)
                {
                    delta = lift;
                    mod += rem;
                    if (mod >= 0)
                    {
                        mod -= dy;
                        delta++;
                    }

                    x_to = x_from + delta;
                    RenderHLine(ey1, x_from, poly_subpixel_scale - first, x_to, first);
                    x_from = x_to;

                    ey1 += incr;
                    SetCurrCell(x_from >> poly_subpixel_shift, ey1);
                }
            }
            RenderHLine(ey1, x_from, poly_subpixel_scale - first, x2, fy2);
        }

        public int MinX { get { return m_min_x; } }
        public int MinY { get { return m_min_y; } }
        public int MaxX { get { return m_max_x; } }
        public int MaxY { get { return m_max_y; } }

#if use_timers
        static CNamedTimer SortCellsTimer = new CNamedTimer("SortCellsTimer");
        static CNamedTimer QSortTimer = new CNamedTimer("QSortTimer");
#endif
        public void SortCells()
        {
            if (m_sorted) return; //Perform sort only the first time.

            AddCurrCell();
            m_curr_cell.X = 0x7FFFFFFF;
            m_curr_cell.Y = 0x7FFFFFFF;
            m_curr_cell.Cover = 0;
            m_curr_cell.Area = 0;

            if (m_num_used_cells == 0) return;

#if use_timers
            SortCellsTimer.Start();
#endif
            // Allocate the array of cell pointers
            m_sorted_cells.Allocate(m_num_used_cells);

            // Allocate and zero the Y array
            m_sorted_y.Allocate((uint)(m_max_y - m_min_y + 1));
            m_sorted_y.Zero();
            CellAA[] cells = m_cells.Array;
            sorted_y[] sortedYData = m_sorted_y.Array;
            CellAA[] sortedCellsData = m_sorted_cells.Array;

            // Create the Y-histogram (count the numbers of cells for each Y)
            for (uint i = 0; i < m_num_used_cells; i++)
            {
                int Index = cells[i].Y - m_min_y;
                sortedYData[Index].start++;
            }

            // Convert the Y-histogram into the array of starting indexes
            uint start = 0;
            uint SortedYSize = m_sorted_y.Size();
            for (uint i = 0; i < SortedYSize; i++)
            {
                uint v = sortedYData[i].start;
                sortedYData[i].start = start;
                start += v;
            }

            // Fill the cell pointer array sorted by Y
            for (uint i = 0; i < m_num_used_cells; i++)
            {
                int SortedIndex = cells[i].Y - m_min_y;
                uint curr_y_start = sortedYData[SortedIndex].start;
                uint curr_y_num = sortedYData[SortedIndex].num;
                sortedCellsData[curr_y_start + curr_y_num] = cells[i];
                ++sortedYData[SortedIndex].num;
            }

#if use_timers
            QSortTimer.Start();
#endif
            // Finally arrange the X-arrays
            for (uint i = 0; i < SortedYSize; i++)
            {
                if (sortedYData[i].num != 0)
                {
                    m_QSorter.Sort(sortedCellsData, sortedYData[i].start, sortedYData[i].start + sortedYData[i].num - 1);
                }
            }
#if use_timers
            QSortTimer.Stop();
#endif
            m_sorted = true;
#if use_timers
            SortCellsTimer.Stop();
#endif
        }

        public uint TotalCells
        {
            get
            {
                return m_num_used_cells;
            }
        }

        public uint ScanlineNumCells(uint y)
        {
            return (uint)m_sorted_y.Data()[(int)y - m_min_y].num;
        }

        public void ScanlineCells(uint y, out CellAA[] CellData, out uint Offset)
        {
            CellData = m_sorted_cells.Data();
            Offset = m_sorted_y[(int)y - m_min_y].start;
        }

        public bool Sorted() { return m_sorted; }

        private void SetCurrCell(int x, int y)
        {
            if (m_curr_cell.NotEqual(x, y, m_style_cell))
            {
                AddCurrCell();
                m_curr_cell.Style(m_style_cell);
                m_curr_cell.X = x;
                m_curr_cell.Y = y;
                m_curr_cell.Cover = 0;
                m_curr_cell.Area = 0;
            }
        }

        private void AddCurrCell()
        {
            if ((m_curr_cell.Area | m_curr_cell.Cover) != 0)
            {
                if (m_num_used_cells >= (int)cell_block_scale_e.cell_block_limit)
                {
                    return;
                }

                AllocateCellsIfRequired();
                m_cells.Data()[m_num_used_cells].Set(m_curr_cell);
                m_num_used_cells++;

#if false
                if(m_num_used_cells == 281)
                {
                    int a = 12;
                }

                DebugFile.Print(m_num_used_cells.ToString() 
                    + ". x=" + m_curr_cell.m_x.ToString()
                    + " y=" + m_curr_cell.m_y.ToString()
                    + " area=" + m_curr_cell.m_area.ToString()
                    + " cover=" + m_curr_cell.m_cover.ToString()
                    + "\n");
#endif
            }
        }

        private void AllocateCellsIfRequired()
        {
            if (m_cells == null || (m_num_used_cells + 1) >= m_cells.Capacity())
            {
                if (m_num_used_cells >= (int)cell_block_scale_e.cell_block_limit)
                {
                    return;
                }

                uint new_num_allocated_cells = m_num_used_cells + (uint)cell_block_scale_e.cell_block_size;
                VectorPOD<CellAA> new_cells = new VectorPOD<CellAA>(new_num_allocated_cells);
                if (m_cells != null)
                {
                    new_cells.CopyFrom(m_cells);
                }
                m_cells = new_cells;
            }
        }

        private void RenderHLine(int ey, int x1, int y1, int x2, int y2)
        {
            int ex1 = x1 >> (int)poly_subpixel_scale_e.Shift;
            int ex2 = x2 >> (int)poly_subpixel_scale_e.Shift;
            int fx1 = x1 & (int)poly_subpixel_scale_e.Mask;
            int fx2 = x2 & (int)poly_subpixel_scale_e.Mask;

            int delta, p, first, dx;
            int incr, lift, mod, rem;

            //trivial case. Happens often
            if (y1 == y2)
            {
                SetCurrCell(ex2, ey);
                return;
            }

            //everything is located in a single cell.  That is easy!
            if (ex1 == ex2)
            {
                delta = y2 - y1;
                m_curr_cell.Cover += delta;
                m_curr_cell.Area += (fx1 + fx2) * delta;
                return;
            }

            //ok, we'll have to render a run of adjacent cells on the same hline...
            p = ((int)poly_subpixel_scale_e.Scale - fx1) * (y2 - y1);
            first = (int)poly_subpixel_scale_e.Scale;
            incr = 1;

            dx = x2 - x1;

            if (dx < 0)
            {
                p = fx1 * (y2 - y1);
                first = 0;
                incr = -1;
                dx = -dx;
            }

            delta = p / dx;
            mod = p % dx;

            if (mod < 0)
            {
                delta--;
                mod += dx;
            }

            m_curr_cell.Cover += delta;
            m_curr_cell.Area += (fx1 + first) * delta;

            ex1 += incr;
            SetCurrCell(ex1, ey);
            y1 += delta;

            if (ex1 != ex2)
            {
                p = (int)poly_subpixel_scale_e.Scale * (y2 - y1 + delta);
                lift = p / dx;
                rem = p % dx;

                if (rem < 0)
                {
                    lift--;
                    rem += dx;
                }

                mod -= dx;

                while (ex1 != ex2)
                {
                    delta = lift;
                    mod += rem;
                    if (mod >= 0)
                    {
                        mod -= dx;
                        delta++;
                    }

                    m_curr_cell.Cover += delta;
                    m_curr_cell.Area += (int)poly_subpixel_scale_e.Scale * delta;
                    y1 += delta;
                    ex1 += incr;
                    SetCurrCell(ex1, ey);
                }
            }
            delta = y2 - y1;
            m_curr_cell.Cover += delta;
            m_curr_cell.Area += (fx2 + (int)poly_subpixel_scale_e.Scale - first) * delta;
        }

        static void SwapCells(CellAA a, CellAA b)
        {
            CellAA temp = a;
            a = b;
            b = temp;
        }

        enum qsort { qsort_threshold = 9 };
    }

    //------------------------------------------------------scanline_hit_test
    public class ScanlineHitTest : IScanlineCache
    {
        private int m_x;
        private bool m_hit;

        public ScanlineHitTest(int x)
        {
            m_x = x;
            m_hit = false;
        }

        public void ResetSpans() { }
        public void Finalize(int nothing) { }
        public void AddCell(int x, uint nothing)
        {
            if (m_x == x) m_hit = true;
        }
        public void AddSpan(int x, int len, uint nothing)
        {
            if (m_x >= x && m_x < x + len) m_hit = true;
        }
        public uint NumSpans { get { return 1; } }
        public bool Hit { get { return m_hit; } }



        public void Reset(int min_x, int max_x)
        {
            throw new System.NotImplementedException();
        }
        public ScanlineSpan Begin()
        {
            throw new System.NotImplementedException();
        }
        public ScanlineSpan GetNextScanlineSpan()
        {
            throw new System.NotImplementedException();
        }
        public int Y
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }
        public byte[] Covers
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }
    }
}