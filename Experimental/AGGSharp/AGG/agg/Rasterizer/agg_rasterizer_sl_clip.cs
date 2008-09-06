
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
//#ifndef AGG_RASTERIZER_SL_CLIP_INCLUDED
//#define AGG_RASTERIZER_SL_CLIP_INCLUDED

//#include "agg_clip_liang_barsky.h"

using System;
using AGG.Clipping;
using NPack.Interfaces;
using poly_subpixel_scale_e = AGG.PolySubPixelScale;

namespace AGG.Rasterizer
{
    //--------------------------------------------------------poly_max_coord_e
    enum PolyMaxCoord
    {
        poly_max_coord = (1 << 30) - 1 //----poly_max_coord
    };

    public interface IVectorClipper<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable
    {
        int Upscale(T v);
        int Downscale(int v);

        void ClipBox(int x1, int y1, int x2, int y2);

        void ResetClipping();

        void MoveTo(int x1, int y1);
        void LineTo(RasterizerCellsAA ras, int x2, int y2);
    };

    //------------------------------------------------------rasterizer_sl_clip
    class VectorClipper<T> : IVectorClipper<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        int MulDiv(double a, double b, double c)
        {
            return Basics.RoundInt(a * b / c);
        }
        int XI(int v) { return v; }
        int YI(int v) { return v; }
        public int Upscale(T v) { return Basics.RoundInt(v.Multiply((int)poly_subpixel_scale_e.Scale)); }
        public int Downscale(int v) { return v; }

        //--------------------------------------------------------------------
        public VectorClipper()
        {
            m_clip_box = new RectInt(0, 0, 0, 0);
            m_x1 = (0);
            m_y1 = (0);
            m_f1 = (0);
            m_clipping = (false);
        }

        //--------------------------------------------------------------------
        public void ResetClipping()
        {
            m_clipping = false;
        }

        //--------------------------------------------------------------------
        public void ClipBox(int x1, int y1, int x2, int y2)
        {
            m_clip_box = new RectInt(x1, y1, x2, y2);
            m_clip_box.Normalize();
            m_clipping = true;
        }

        //--------------------------------------------------------------------
        public void MoveTo(int x1, int y1)
        {
            m_x1 = x1;
            m_y1 = y1;
            if (m_clipping) m_f1 = LiangBarskyClipper.GetClippingFlags(x1, y1, m_clip_box);
        }

        //------------------------------------------------------------------------
        private void LineClipY(RasterizerCellsAA ras,
                                    int x1, int y1,
                                    int x2, int y2,
                                    uint f1, uint f2)
        {
            f1 &= 10;
            f2 &= 10;
            if ((f1 | f2) == 0)
            {
                // Fully visible
                ras.Line(x1, y1, x2, y2);
            }
            else
            {
                if (f1 == f2)
                {
                    // Invisible by Y
                    return;
                }

                int tx1 = x1;
                int ty1 = y1;
                int tx2 = x2;
                int ty2 = y2;

                if ((f1 & 8) != 0) // y1 < clip.y1
                {
                    tx1 = x1 + MulDiv(m_clip_box.Y1 - y1, x2 - x1, y2 - y1);
                    ty1 = m_clip_box.Y1;
                }

                if ((f1 & 2) != 0) // y1 > clip.y2
                {
                    tx1 = x1 + MulDiv(m_clip_box.Y2 - y1, x2 - x1, y2 - y1);
                    ty1 = m_clip_box.Y2;
                }

                if ((f2 & 8) != 0) // y2 < clip.y1
                {
                    tx2 = x1 + MulDiv(m_clip_box.Y1 - y1, x2 - x1, y2 - y1);
                    ty2 = m_clip_box.Y1;
                }

                if ((f2 & 2) != 0) // y2 > clip.y2
                {
                    tx2 = x1 + MulDiv(m_clip_box.Y2 - y1, x2 - x1, y2 - y1);
                    ty2 = m_clip_box.Y2;
                }

                ras.Line(tx1, ty1, tx2, ty2);
            }
        }

        //--------------------------------------------------------------------
        public void LineTo(RasterizerCellsAA ras, int x2, int y2)
        {
            if (m_clipping)
            {
                uint f2 = LiangBarskyClipper.GetClippingFlags(x2, y2, m_clip_box);

                if ((m_f1 & 10) == (f2 & 10) && (m_f1 & 10) != 0)
                {
                    // Invisible by Y
                    m_x1 = x2;
                    m_y1 = y2;
                    m_f1 = f2;
                    return;
                }

                int x1 = m_x1;
                int y1 = m_y1;
                uint f1 = m_f1;
                int y3, y4;
                uint f3, f4;

                switch (((f1 & 5) << 1) | (f2 & 5))
                {
                    case 0: // Visible by X
                        LineClipY(ras, x1, y1, x2, y2, f1, f2);
                        break;

                    case 1: // x2 > clip.x2
                        y3 = y1 + MulDiv(m_clip_box.X2 - x1, y2 - y1, x2 - x1);
                        f3 = LiangBarskyClipper.GetClippingFlagsY(y3, m_clip_box);
                        LineClipY(ras, x1, y1, m_clip_box.X2, y3, f1, f3);
                        LineClipY(ras, m_clip_box.X2, y3, m_clip_box.X2, y2, f3, f2);
                        break;

                    case 2: // x1 > clip.x2
                        y3 = y1 + MulDiv(m_clip_box.X2 - x1, y2 - y1, x2 - x1);
                        f3 = LiangBarskyClipper.GetClippingFlagsY(y3, m_clip_box);
                        LineClipY(ras, m_clip_box.X2, y1, m_clip_box.X2, y3, f1, f3);
                        LineClipY(ras, m_clip_box.X2, y3, x2, y2, f3, f2);
                        break;

                    case 3: // x1 > clip.x2 && x2 > clip.x2
                        LineClipY(ras, m_clip_box.X2, y1, m_clip_box.X2, y2, f1, f2);
                        break;

                    case 4: // x2 < clip.x1
                        y3 = y1 + MulDiv(m_clip_box.X1 - x1, y2 - y1, x2 - x1);
                        f3 = LiangBarskyClipper.GetClippingFlagsY(y3, m_clip_box);
                        LineClipY(ras, x1, y1, m_clip_box.X1, y3, f1, f3);
                        LineClipY(ras, m_clip_box.X1, y3, m_clip_box.X1, y2, f3, f2);
                        break;

                    case 6: // x1 > clip.x2 && x2 < clip.x1
                        y3 = y1 + MulDiv(m_clip_box.X2 - x1, y2 - y1, x2 - x1);
                        y4 = y1 + MulDiv(m_clip_box.X1 - x1, y2 - y1, x2 - x1);
                        f3 = LiangBarskyClipper.GetClippingFlagsY(y3, m_clip_box);
                        f4 = LiangBarskyClipper.GetClippingFlagsY(y4, m_clip_box);
                        LineClipY(ras, m_clip_box.X2, y1, m_clip_box.X2, y3, f1, f3);
                        LineClipY(ras, m_clip_box.X2, y3, m_clip_box.X1, y4, f3, f4);
                        LineClipY(ras, m_clip_box.X1, y4, m_clip_box.X1, y2, f4, f2);
                        break;

                    case 8: // x1 < clip.x1
                        y3 = y1 + MulDiv(m_clip_box.X1 - x1, y2 - y1, x2 - x1);
                        f3 = LiangBarskyClipper.GetClippingFlagsY(y3, m_clip_box);
                        LineClipY(ras, m_clip_box.X1, y1, m_clip_box.X1, y3, f1, f3);
                        LineClipY(ras, m_clip_box.X1, y3, x2, y2, f3, f2);
                        break;

                    case 9:  // x1 < clip.x1 && x2 > clip.x2
                        y3 = y1 + MulDiv(m_clip_box.X1 - x1, y2 - y1, x2 - x1);
                        y4 = y1 + MulDiv(m_clip_box.X2 - x1, y2 - y1, x2 - x1);
                        f3 = LiangBarskyClipper.GetClippingFlagsY(y3, m_clip_box);
                        f4 = LiangBarskyClipper.GetClippingFlagsY(y4, m_clip_box);
                        LineClipY(ras, m_clip_box.X1, y1, m_clip_box.X1, y3, f1, f3);
                        LineClipY(ras, m_clip_box.X1, y3, m_clip_box.X2, y4, f3, f4);
                        LineClipY(ras, m_clip_box.X2, y4, m_clip_box.X2, y2, f4, f2);
                        break;

                    case 12: // x1 < clip.x1 && x2 < clip.x1
                        LineClipY(ras, m_clip_box.X1, y1, m_clip_box.X1, y2, f1, f2);
                        break;
                }
                m_f1 = f2;
            }
            else
            {
                ras.Line(m_x1, m_y1,
                         x2, y2);
            }
            m_x1 = x2;
            m_y1 = y2;
        }

        private RectInt m_clip_box;
        private int m_x1;
        private int m_y1;
        private uint m_f1;
        private bool m_clipping;
    };

    //---------------------------------------------------rasterizer_sl_no_clip
    class VectorClipper_NoClip<T> : IVectorClipper<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public VectorClipper_NoClip()
        {

        }

        public int Upscale(T v) { return Basics.RoundInt(v.Multiply((int)poly_subpixel_scale_e.Scale)); }
        public int Downscale(int v) { return v; }

        public void ResetClipping() { }
        public void ClipBox(int x1, int y1, int x2, int y2) { }
        public void MoveTo(int x1, int y1) { m_x1 = x1; m_y1 = y1; }

        public void LineTo(RasterizerCellsAA ras, int x2, int y2)
        {
            ras.Line(m_x1, m_y1, x2, y2);
            m_x1 = x2;
            m_y1 = y2;
        }

        private int m_x1;
        private int m_y1;
    };
}