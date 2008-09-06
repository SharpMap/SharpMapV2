
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

//#ifndef AGG_GAMMA_LUT_INCLUDED
//#define AGG_GAMMA_LUT_INCLUDED

//#include <math.h>
//#include "agg_basics.h"

using System;

namespace AGG.Gamma
{
    //template<class LoResT=int8u, class HiResT=int8u, unsigned GammaShift=8, unsigned HiResShift=8> 
    public class GammaLut
    {
        private double m_gamma;
        private byte[] m_dir_gamma;
        private byte[] m_inv_gamma;

        enum GammaScale
        {
            Shift = 8,
            Size = 1 << Shift,
            Mask = Size - 1
        };

        public GammaLut()
        {
            m_gamma = (1.0);
            m_dir_gamma = new byte[(int)GammaScale.Size];
            m_inv_gamma = new byte[(int)GammaScale.Size];
        }

        public GammaLut(double g)
        {
            m_gamma = g;
            m_dir_gamma = new byte[(int)GammaScale.Size];
            m_inv_gamma = new byte[(int)GammaScale.Size];
            Gamma(m_gamma);
        }

        public void Gamma(double g)
        {
            m_gamma = g;

            for (uint i = 0; i < (uint)GammaScale.Size; i++)
            {
                m_dir_gamma[i] = (byte)Basics.RoundUint(Math.Pow(i / (double)GammaScale.Mask, m_gamma) * (double)GammaScale.Mask);
            }

            double inv_g = 1.0 / g;
            for (uint i = 0; i < (uint)GammaScale.Size; i++)
            {
                m_inv_gamma[i] = (byte)Basics.RoundUint(Math.Pow(i / (double)GammaScale.Mask, inv_g) * (double)GammaScale.Mask);
            }
        }

        public double Gamma()
        {
            return m_gamma;
        }

        public byte Dir(byte v)
        {
            return m_dir_gamma[v];
        }

        public byte Inv(byte v)
        {
            return m_inv_gamma[v];
        }
    };
}

