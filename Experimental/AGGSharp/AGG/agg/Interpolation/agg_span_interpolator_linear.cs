
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
using NPack;
using NPack.Interfaces;
namespace AGG.Interpolation
{

    public interface ISpanInterpolator<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable
    {
        void Begin(T x, T y, uint len);
        void Coordinates(out int x, out int y);
        void Next();

        IAffineTransformMatrix<T> Transformer { get; set; }
        //void Transformer(ITransform trans);
        void Resynchronize(T xe, T ye, uint len);
        void LocalScale(out int x, out int y);
    };

    //================================================span_interpolator_linear
    public sealed class SpanInterpolatorLinear<T> : ISpanInterpolator<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private IAffineTransformMatrix<T> m_trans;
        private Dda2LineInterpolator m_li_x;
        private Dda2LineInterpolator m_li_y;

        public enum subpixel_scale_e
        {
            SubpixelShift = 8,
            subpixel_shift = SubpixelShift,
            subpixel_scale = 1 << subpixel_shift
        };

        //--------------------------------------------------------------------
        public SpanInterpolatorLinear() { }
        public SpanInterpolatorLinear(IAffineTransformMatrix<T> trans)
        {
            m_trans = trans;
        }

        public SpanInterpolatorLinear(IAffineTransformMatrix<T> trans, T x, T y, uint len)
        {
            m_trans = trans;
            Begin(x, y, len);
        }

        //----------------------------------------------------------------
        public IAffineTransformMatrix<T> Transformer { get { return m_trans; } set { m_trans = value; } }
        //public void Transformer(Transform.ITransform trans) { m_trans = trans; }

        public void LocalScale(out int x, out int y)
        {
            throw new System.NotImplementedException();
        }

        //----------------------------------------------------------------
        public void Begin(T x, T y, uint len)
        {
            T tx;
            T ty;

            tx = x;
            ty = y;
            m_trans.Transform(ref tx, ref ty);
            int x1 = Basics.RoundInt(tx.Multiply((double)subpixel_scale_e.subpixel_scale));
            int y1 = Basics.RoundInt(ty.Multiply((double)subpixel_scale_e.subpixel_scale));

            tx = x.Add(len);
            ty = y;
            m_trans.Transform(ref tx, ref ty);
            int x2 = Basics.RoundInt(tx.Multiply((double)subpixel_scale_e.subpixel_scale));
            int y2 = Basics.RoundInt(ty.Multiply((double)subpixel_scale_e.subpixel_scale));

            m_li_x = new Dda2LineInterpolator(x1, x2, (int)len);
            m_li_y = new Dda2LineInterpolator(y1, y2, (int)len);
        }

        //----------------------------------------------------------------
        public void Resynchronize(T xe, T ye, uint len)
        {
            m_trans.Transform(ref xe, ref ye);
            m_li_x = new Dda2LineInterpolator(m_li_x.Y, Basics.RoundInt(xe.Multiply((double)subpixel_scale_e.subpixel_scale)), (int)len);
            m_li_y = new Dda2LineInterpolator(m_li_y.Y, Basics.RoundInt(ye.Multiply((double)subpixel_scale_e.subpixel_scale)), (int)len);
        }

        //----------------------------------------------------------------
        //public void operator++()
        public void Next()
        {
            m_li_x.Next();
            m_li_y.Next();
        }

        //----------------------------------------------------------------
        public void Coordinates(out int x, out int y)
        {
            x = m_li_x.Y;
            y = m_li_y.Y;
        }
    };

    /*
        //=====================================span_interpolator_linear_subdiv
        template<class Transformer = ITransformer, uint SubpixelShift = 8> 
        class span_interpolator_linear_subdiv
        {
        public:
            typedef Transformer trans_type;

            enum subpixel_scale_e
            {
                subpixel_shift = SubpixelShift,
                subpixel_scale = 1 << subpixel_shift
            };


            //----------------------------------------------------------------
            span_interpolator_linear_subdiv() :
                m_subdiv_shift(4),
                m_subdiv_size(1 << m_subdiv_shift),
                m_subdiv_mask(m_subdiv_size - 1) {}

            span_interpolator_linear_subdiv(const trans_type& trans, 
                                            uint subdiv_shift = 4) : 
                m_subdiv_shift(subdiv_shift),
                m_subdiv_size(1 << m_subdiv_shift),
                m_subdiv_mask(m_subdiv_size - 1),
                m_trans(&trans) {}

            span_interpolator_linear_subdiv(const trans_type& trans,
                                            double x, double y, uint len,
                                            uint subdiv_shift = 4) :
                m_subdiv_shift(subdiv_shift),
                m_subdiv_size(1 << m_subdiv_shift),
                m_subdiv_mask(m_subdiv_size - 1),
                m_trans(&trans)
            {
                begin(x, y, len);
            }

            //----------------------------------------------------------------
            const trans_type& transformer() const { return *m_trans; }
            void transformer(const trans_type& trans) { m_trans = &trans; }

            //----------------------------------------------------------------
            uint subdiv_shift() const { return m_subdiv_shift; }
            void subdiv_shift(uint shift) 
            {
                m_subdiv_shift = shift;
                m_subdiv_size = 1 << m_subdiv_shift;
                m_subdiv_mask = m_subdiv_size - 1;
            }

            //----------------------------------------------------------------
            void begin(double x, double y, uint len)
            {
                double tx;
                double ty;
                m_pos   = 1;
                m_src_x = iround(x * subpixel_scale) + subpixel_scale;
                m_src_y = y;
                m_len   = len;

                if(len > m_subdiv_size) len = m_subdiv_size;
                tx = x;
                ty = y;
                m_trans->Transform(&tx, &ty);
                int x1 = iround(tx * subpixel_scale);
                int y1 = iround(ty * subpixel_scale);

                tx = x + len;
                ty = y;
                m_trans->Transform(&tx, &ty);

                m_li_x = dda2_line_interpolator(x1, iround(tx * subpixel_scale), len);
                m_li_y = dda2_line_interpolator(y1, iround(ty * subpixel_scale), len);
            }

            //----------------------------------------------------------------
            void operator++()
            {
                ++m_li_x;
                ++m_li_y;
                if(m_pos >= m_subdiv_size)
                {
                    uint len = m_len;
                    if(len > m_subdiv_size) len = m_subdiv_size;
                    double tx = double(m_src_x) / double(subpixel_scale) + len;
                    double ty = m_src_y;
                    m_trans->Transform(&tx, &ty);
                    m_li_x = dda2_line_interpolator(m_li_x.y(), iround(tx * subpixel_scale), len);
                    m_li_y = dda2_line_interpolator(m_li_y.y(), iround(ty * subpixel_scale), len);
                    m_pos = 0;
                }
                m_src_x += subpixel_scale;
                ++m_pos;
                --m_len;
            }

            //----------------------------------------------------------------
            void coordinates(int* x, int* y) const
            {
                *x = m_li_x.y();
                *y = m_li_y.y();
            }

        private:
            uint m_subdiv_shift;
            uint m_subdiv_size;
            uint m_subdiv_mask;
            const trans_type* m_trans;
            dda2_line_interpolator m_li_x;
            dda2_line_interpolator m_li_y;
            int      m_src_x;
            double   m_src_y;
            uint m_pos;
            uint m_len;
        };

     */
}