
using System;
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
using NPack.Interfaces;

namespace AGG.Interpolation
{

    //=================================================span_subdiv_adaptor
    public class SpanSubDivAdaptor<T> : ISpanInterpolator<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        int m_subdiv_shift;
        int m_subdiv_size;
        int m_subdiv_mask;
        ISpanInterpolator<T> m_interpolator;
        int m_src_x;
        T m_src_y;
        uint m_pos;
        uint m_len;

        const int SubPixelShift = 8;
        const int SubPixelScale = 1 << SubPixelShift;


        //----------------------------------------------------------------
        public SpanSubDivAdaptor(ISpanInterpolator<T> interpolator)
            : this(interpolator, 4)
        {
        }

        public SpanSubDivAdaptor(ISpanInterpolator<T> interpolator, int subdiv_shift)
        {
            m_subdiv_shift = subdiv_shift;
            m_subdiv_size = 1 << m_subdiv_shift;
            m_subdiv_mask = m_subdiv_size - 1;
            m_interpolator = interpolator;
        }

        public SpanSubDivAdaptor(ISpanInterpolator<T> interpolator,
                             T x, T y, uint len,
                             int subdiv_shift)
            : this(interpolator, subdiv_shift)
        {
            Begin(x, y, len);
        }

        public void Resynchronize(T xe, T ye, uint len)
        {
            throw new System.NotImplementedException();
        }

        //----------------------------------------------------------------
        public ISpanInterpolator<T> Interpolator { get { return m_interpolator; } set { m_interpolator = value; } }
        //public void Interpolator(ISpanInterpolator intr) { m_interpolator = intr; }

        //----------------------------------------------------------------
        public IAffineTransformMatrix<T> Transformer
        {
            get
            {
                return m_interpolator.Transformer;

            }
            set { m_interpolator.Transformer = value; }
        }
        //public void Transformer(Transform.ITransform trans) 
        //{ 
        //    m_interpolator.Transformer(trans); 
        //}

        //----------------------------------------------------------------
        public int SubDivShift
        {
            get { return m_subdiv_shift; }
            set
            {
                m_subdiv_shift = value;
                m_subdiv_size = 1 << m_subdiv_shift;
                m_subdiv_mask = m_subdiv_size - 1;
            }
        }
        //public void SubDivShift(int shift)
        //{
        //    m_subdiv_shift = shift;
        //    m_subdiv_size = 1 << m_subdiv_shift;
        //    m_subdiv_mask = m_subdiv_size - 1;
        //}

        //----------------------------------------------------------------
        public void Begin(T x, T y, uint len)
        {
            m_pos = 1;
            m_src_x = Basics.RoundInt(x.Multiply((SubPixelScale))) + SubPixelScale;
            m_src_y = y;
            m_len = len;
            if (len > m_subdiv_size) len = (uint)m_subdiv_size;
            m_interpolator.Begin(x, y, len);
        }

        //----------------------------------------------------------------
        public void Next()
        {
            m_interpolator.Next();
            if (m_pos >= m_subdiv_size)
            {
                uint len = m_len;
                if (len > m_subdiv_size) len = (uint)m_subdiv_size;
                m_interpolator.Resynchronize(M.New<T>(m_src_x).Divide((double)SubPixelScale).Add(len),
                                              m_src_y,
                                              len);
                m_pos = 0;
            }
            m_src_x += SubPixelScale;
            ++m_pos;
            --m_len;
        }

        //----------------------------------------------------------------
        public void Coordinates(out int x, out int y)
        {
            m_interpolator.Coordinates(out x, out y);
        }

        //----------------------------------------------------------------
        public void LocalScale(out int x, out int y)
        {
            m_interpolator.LocalScale(out x, out y);
        }
    };
}