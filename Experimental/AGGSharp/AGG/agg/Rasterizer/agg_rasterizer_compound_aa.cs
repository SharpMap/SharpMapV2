
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
// Anti-Grain Geometry - Version 2.3
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

namespace AGG.Rasterizer
{
    //===========================================================layer_order_e
    public enum LayerOrder
    {
        Unsorted, //------layer_unsorted
        Direct,   //------layer_direct
        Inverse   //------layer_inverse
    };


    //==================================================rasterizer_compound_aa
    //template<class Clip=rasterizer_sl_clip_int> 
    sealed public class RasterizerCompoundAA<T> : IRasterizer<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        RasterizerCellsAA m_Rasterizer;
        IVectorClipper<T> m_VectorClipper;
        FillingRule m_filling_rule;
        LayerOrder m_layer_order;
        VectorPOD<StyleInfo> m_styles;  // Active Styles
        VectorPOD<uint> m_ast;     // Active Style Table (unique values)
        VectorPOD<byte> m_asm;     // Active Style Mask 
        VectorPOD<CellAA> m_cells;
        VectorPOD<byte> m_cover_buf;
        VectorPOD<uint> m_master_alpha;

        int m_min_style;
        int m_max_style;
        int m_start_x;
        int m_start_y;
        int m_scan_y;
        int m_sl_start;
        uint m_sl_len;

        struct StyleInfo
        {
            internal uint start_cell;
            internal uint num_cells;
            internal int last_x;
        };

        private const int AAShift = 8;
        private const int AAScale = 1 << AAShift;
        private const int AAMask = AAScale - 1;
        private const int AAScale2 = AAScale * 2;
        private const int AAMask2 = AAScale2 - 1;

        private const int PolySubpixelShift = (int)PolySubPixelScale.Shift;

        public RasterizerCompoundAA()
        {
            m_Rasterizer = new RasterizerCellsAA();
            m_VectorClipper = new VectorClipper<T>();
            m_filling_rule = FillingRule.NonZero;
            m_layer_order = LayerOrder.Direct;
            m_styles = new VectorPOD<StyleInfo>();  // Active Styles
            m_ast = new VectorPOD<uint>();     // Active Style Table (unique values)
            m_asm = new VectorPOD<byte>();     // Active Style Mask 
            m_cells = new VectorPOD<CellAA>();
            m_cover_buf = new VectorPOD<byte>();
            m_master_alpha = new VectorPOD<uint>();
            m_min_style = (0x7FFFFFFF);
            m_max_style = (-0x7FFFFFFF);
            m_start_x = (0);
            m_start_y = (0);
            m_scan_y = (0x7FFFFFFF);
            m_sl_start = (0);
            m_sl_len = (0);
        }

        public void Gamma(IGammaFunction gamma_function)
        {
            throw new System.NotImplementedException();
        }


        public void Reset()
        {
            m_Rasterizer.Reset();
            m_min_style = 0x7FFFFFFF;
            m_max_style = -0x7FFFFFFF;
            m_scan_y = 0x7FFFFFFF;
            m_sl_start = 0;
            m_sl_len = 0;
        }

        FillingRule FillingRule
        {
            set
            {
                m_filling_rule = value;
            }
        }

        LayerOrder LayerOrder
        {
            set
            {

                m_layer_order = value;
            }
        }

        void ClipBox(T x1, T y1, T x2, T y2)
        {
            Reset();
            m_VectorClipper.ClipBox(m_VectorClipper.Upscale(x1), m_VectorClipper.Upscale(y1),
                               m_VectorClipper.Upscale(x2), m_VectorClipper.Upscale(y2));
        }

        void ResetClipping()
        {
            Reset();
            m_VectorClipper.ResetClipping();
        }

        public void Styles(int left, int right)
        {
            CellAA cell = new CellAA();
            cell.Initial();
            cell.Left = (short)left;
            cell.Right = (short)right;
            m_Rasterizer.Style(cell);
            if (left >= 0 && left < m_min_style) m_min_style = left;
            if (left >= 0 && left > m_max_style) m_max_style = left;
            if (right >= 0 && right < m_min_style) m_min_style = right;
            if (right >= 0 && right > m_max_style) m_max_style = right;
        }

        public void MoveTo(int x, int y)
        {
            if (m_Rasterizer.Sorted()) Reset();
            m_VectorClipper.MoveTo(m_start_x = m_VectorClipper.Downscale(x),
                              m_start_y = m_VectorClipper.Downscale(y));
        }

        public void LineTo(int x, int y)
        {
            m_VectorClipper.LineTo(m_Rasterizer,
                              m_VectorClipper.Downscale(x),
                              m_VectorClipper.Downscale(y));
        }

        public void MoveToDbl(T x, T y)
        {
            if (m_Rasterizer.Sorted()) Reset();
            m_VectorClipper.MoveTo(m_start_x = m_VectorClipper.Upscale(x),
                              m_start_y = m_VectorClipper.Upscale(y));
        }

        public void LineToDbl(T x, T y)
        {
            m_VectorClipper.LineTo(m_Rasterizer,
                              m_VectorClipper.Upscale(x),
                              m_VectorClipper.Upscale(y));
        }

        void AddVertex(T x, T y, uint cmd)
        {
            if (Path.IsMoveTo(cmd))
            {
                MoveToDbl(x, y);
            }
            else
                if (Path.IsVertex(cmd))
                {
                    LineToDbl(x, y);
                }
                else
                    if (Path.IsClose(cmd))
                    {
                        m_VectorClipper.LineTo(m_Rasterizer, m_start_x, m_start_y);
                    }
        }

        void EdgeInt(int x1, int y1, int x2, int y2)
        {
            if (m_Rasterizer.Sorted()) Reset();
            m_VectorClipper.MoveTo(m_VectorClipper.Downscale(x1), m_VectorClipper.Downscale(y1));
            m_VectorClipper.LineTo(m_Rasterizer,
                              m_VectorClipper.Downscale(x2),
                              m_VectorClipper.Downscale(y2));
        }

        void EdgeDbl(T x1, T y1,
                                                  T x2, T y2)
        {
            if (m_Rasterizer.Sorted()) Reset();
            m_VectorClipper.MoveTo(m_VectorClipper.Upscale(x1), m_VectorClipper.Upscale(y1));
            m_VectorClipper.LineTo(m_Rasterizer,
                              m_VectorClipper.Upscale(x2),
                              m_VectorClipper.Upscale(y2));
        }

        void Sort()
        {
            m_Rasterizer.SortCells();
        }

        public bool RewindScanlines()
        {
            m_Rasterizer.SortCells();
            if (m_Rasterizer.TotalCells == 0)
            {
                return false;
            }
            if (m_max_style < m_min_style)
            {
                return false;
            }
            m_scan_y = m_Rasterizer.MinY;
            m_styles.Allocate((uint)(m_max_style - m_min_style + 2), 128);
            AllocateMasterAlpha();
            return true;
        }

        // Returns the number of styles
        public uint SweepStyles()
        {
            for (; ; )
            {
                if (m_scan_y > m_Rasterizer.MaxY) return 0;
                int num_cells = (int)m_Rasterizer.ScanlineNumCells((uint)m_scan_y);
                CellAA[] cells;
                uint cellOffset = 0;
                int curCellOffset;
                m_Rasterizer.ScanlineCells((uint)m_scan_y, out cells, out cellOffset);
                uint num_styles = (uint)(m_max_style - m_min_style + 2);
                uint style_id;
                int styleOffset = 0;

                m_cells.Allocate((uint)num_cells * 2, 256); // Each cell can have two styles
                m_ast.Capacity(num_styles, 64);
                m_asm.Allocate((num_styles + 7) >> 3, 8);
                m_asm.Zero();

                if (num_cells > 0)
                {
                    // Pre-add zero (for no-fill style, that is, -1).
                    // We need that to ensure that the "-1 style" would go first.
                    m_asm.Array[0] |= 1;
                    m_ast.Add(0);
                    m_styles.Array[styleOffset].start_cell = 0;
                    m_styles.Array[styleOffset].num_cells = 0;
                    m_styles.Array[styleOffset].last_x = -0x7FFFFFFF;

                    m_sl_start = cells[0].X;
                    m_sl_len = (uint)(cells[num_cells - 1].X - m_sl_start + 1);
                    while (num_cells-- != 0)
                    {
                        curCellOffset = (int)cellOffset++;
                        AddStyle(cells[curCellOffset].Left);
                        AddStyle(cells[curCellOffset].Right);
                    }

                    // Convert the Y-histogram into the array of starting indexes
                    uint i;
                    uint start_cell = 0;
                    StyleInfo[] stylesArray = m_styles.Array;
                    for (i = 0; i < m_ast.Size(); i++)
                    {
                        int IndexToModify = (int)m_ast[i];
                        uint v = stylesArray[IndexToModify].start_cell;
                        stylesArray[IndexToModify].start_cell = start_cell;
                        start_cell += v;
                    }

                    num_cells = (int)m_Rasterizer.ScanlineNumCells((uint)m_scan_y);
                    m_Rasterizer.ScanlineCells((uint)m_scan_y, out cells, out cellOffset);

                    while (num_cells-- > 0)
                    {
                        curCellOffset = (int)cellOffset++;
                        style_id = (uint)((cells[curCellOffset].Left < 0) ? 0 :
                                    cells[curCellOffset].Left - m_min_style + 1);

                        styleOffset = (int)style_id;
                        if (cells[curCellOffset].X == stylesArray[styleOffset].last_x)
                        {
                            cellOffset = stylesArray[styleOffset].start_cell + stylesArray[styleOffset].num_cells - 1;
                            unchecked
                            {
                                cells[cellOffset].Area += cells[curCellOffset].Area;
                                cells[cellOffset].Cover += cells[curCellOffset].Cover;
                            }
                        }
                        else
                        {
                            cellOffset = stylesArray[styleOffset].start_cell + stylesArray[styleOffset].num_cells;
                            cells[cellOffset].X = cells[curCellOffset].X;
                            cells[cellOffset].Area = cells[curCellOffset].Area;
                            cells[cellOffset].Cover = cells[curCellOffset].Cover;
                            stylesArray[styleOffset].last_x = cells[curCellOffset].X;
                            stylesArray[styleOffset].num_cells++;
                        }

                        style_id = (uint)((cells[curCellOffset].Right < 0) ? 0 :
                                    cells[curCellOffset].Right - m_min_style + 1);

                        styleOffset = (int)style_id;
                        if (cells[curCellOffset].X == stylesArray[styleOffset].last_x)
                        {
                            cellOffset = stylesArray[styleOffset].start_cell + stylesArray[styleOffset].num_cells - 1;
                            unchecked
                            {
                                cells[cellOffset].Area -= cells[curCellOffset].Area;
                                cells[cellOffset].Cover -= cells[curCellOffset].Cover;
                            }
                        }
                        else
                        {
                            cellOffset = stylesArray[styleOffset].start_cell + stylesArray[styleOffset].num_cells;
                            cells[cellOffset].X = cells[curCellOffset].X;
                            cells[cellOffset].Area = -cells[curCellOffset].Area;
                            cells[cellOffset].Cover = -cells[curCellOffset].Cover;
                            stylesArray[styleOffset].last_x = cells[curCellOffset].X;
                            stylesArray[styleOffset].num_cells++;
                        }
                    }
                }
                if (m_ast.Size() > 1) break;
                ++m_scan_y;
            }
            ++m_scan_y;

            if (m_layer_order != LayerOrder.Unsorted)
            {
                VectorPODRangeAdaptor ra = new VectorPODRangeAdaptor(m_ast, 1, m_ast.Size() - 1);
                if (m_layer_order == LayerOrder.Direct)
                {
                    QuickSortRangeAdaptorUint m_QSorter = new QuickSortRangeAdaptorUint();
                    m_QSorter.Sort(ra);
                    //quick_sort(ra, uint_greater);
                }
                else
                {
                    throw new System.NotImplementedException();
                    //QuickSort_range_adaptor_uint m_QSorter = new QuickSort_range_adaptor_uint();
                    //m_QSorter.Sort(ra);
                    //quick_sort(ra, uint_less);
                }
            }

            return m_ast.Size() - 1;
        }

        // Returns style ID depending of the existing style index
        public uint Style(uint style_idx)
        {
            return m_ast[style_idx + 1] + (uint)m_min_style - 1;
        }

        bool NavigateScanline(int y)
        {
            m_Rasterizer.SortCells();
            if (m_Rasterizer.TotalCells == 0)
            {
                return false;
            }
            if (m_max_style < m_min_style)
            {
                return false;
            }
            if (y < m_Rasterizer.MinY || y > m_Rasterizer.MaxY)
            {
                return false;
            }
            m_scan_y = y;
            m_styles.Allocate((uint)(m_max_style - m_min_style + 2), 128);
            AllocateMasterAlpha();
            return true;
        }

        bool HitTest(int tx, int ty)
        {
            if (!NavigateScanline(ty))
            {
                return false;
            }

            uint num_styles = SweepStyles();
            if (num_styles <= 0)
            {
                return false;
            }

            ScanlineHitTest sl = new ScanlineHitTest(tx);
            SweepScanline(sl, -1);
            return sl.Hit;
        }

        byte[] AllocateCoverBuffer(uint len)
        {
            m_cover_buf.Allocate(len, 256);
            return m_cover_buf.Array;
        }

        void MasterAlpha(int style, double alpha)
        {
            if (style >= 0)
            {
                while ((int)m_master_alpha.Size() <= style)
                {
                    m_master_alpha.Add(AAMask);
                }
                m_master_alpha.Array[style] = Basics.RoundUint(alpha * AAMask);
            }
        }

        public void AddPath(IVertexSource<T> vs)
        {
            AddPath(vs, 0);
        }

        public void AddPath(IVertexSource<T> vs, uint path_id)
        {
            T x;
            T y;

            uint cmd;
            vs.Rewind(path_id);
            if (m_Rasterizer.Sorted()) Reset();
            while (!Path.IsStop(cmd = vs.Vertex(out x, out y)))
            {
                AddVertex(x, y, cmd);
            }
        }

        public int MinX { get { return m_Rasterizer.MinX; } }
        public int MinY { get { return m_Rasterizer.MinY; } }
        public int MaxX { get { return m_Rasterizer.MaxX; } }
        public int MaxY { get { return m_Rasterizer.MaxY; } }
        public int MinStyle { get { return m_min_style; } }
        public int MaxStyle { get { return m_max_style; } }

        public int ScanlineStart { get { return m_sl_start; } }
        public uint ScanlineLength { get { return m_sl_len; } }

        public uint CalculateAlpha(int area, uint master_alpha)
        {
            int cover = area >> (PolySubpixelShift * 2 + 1 - AAShift);
            if (cover < 0) cover = -cover;
            if (m_filling_rule == FillingRule.EvenOdd)
            {
                cover &= AAMask2;
                if (cover > AAScale)
                {
                    cover = AAScale2 - cover;
                }
            }
            if (cover > AAMask) cover = AAMask;
            return (uint)((cover * master_alpha + AAMask) >> AAShift);
        }

        public bool SweepScanline(IScanlineCache sl)
        {
            throw new System.NotImplementedException();
        }

        // Sweeps one scanline with one style index. The style ID can be 
        // determined by calling style(). 
        //template<class Scanline> 
        public bool SweepScanline(IScanlineCache sl, int style_idx)
        {
            int scan_y = m_scan_y - 1;
            if (scan_y > m_Rasterizer.MaxY) return false;

            sl.ResetSpans();

            uint master_alpha = AAMask;

            if (style_idx < 0)
            {
                style_idx = 0;
            }
            else
            {
                style_idx++;
                master_alpha = m_master_alpha[(uint)(m_ast[(uint)style_idx] + m_min_style - 1)];
            }

            StyleInfo st = m_styles[m_ast[style_idx]];

            int num_cells = (int)st.num_cells;
            uint CellOffset = st.start_cell;
            CellAA cell = m_cells[CellOffset];

            int cover = 0;
            while (num_cells-- != 0)
            {
                uint alpha;
                int x = cell.X;
                int area = cell.Area;

                cover += cell.Cover;

                cell = m_cells[++CellOffset];

                if (area != 0)
                {
                    alpha = CalculateAlpha((cover << (PolySubpixelShift + 1)) - area,
                                            master_alpha);
                    sl.AddCell(x, alpha);
                    x++;
                }

                if (num_cells != 0 && cell.X > x)
                {
                    alpha = CalculateAlpha(cover << (PolySubpixelShift + 1),
                                            master_alpha);
                    if (alpha != 0)
                    {
                        sl.AddSpan(x, cell.X - x, alpha);
                    }
                }
            }

            if (sl.NumSpans == 0) return false;
            sl.Finalize(scan_y);
            return true;
        }

        private void AddStyle(int style_id)
        {
            if (style_id < 0) style_id = 0;
            else style_id -= m_min_style - 1;

            uint nbyte = (uint)((int)style_id >> 3);
            uint mask = (uint)(1 << (style_id & 7));

            StyleInfo[] stylesArray = m_styles.Array;
            if ((m_asm[nbyte] & mask) == 0)
            {
                m_ast.Add((uint)style_id);
                m_asm.Array[nbyte] |= (byte)mask;
                stylesArray[style_id].start_cell = 0;
                stylesArray[style_id].num_cells = 0;
                stylesArray[style_id].last_x = -0x7FFFFFFF;
            }
            ++stylesArray[style_id].start_cell;
        }

        private void AllocateMasterAlpha()
        {
            while ((int)m_master_alpha.Size() <= m_max_style)
            {
                m_master_alpha.Add(AAMask);
            }
        }
    };
}
