
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

namespace AGG.Gamma
{
    public interface IGammaFunction
    {
        double GetGamma(double x);
    };

    public struct GammaNone : IGammaFunction
    {
        public double GetGamma(double x) { return x; }
    };


    //==============================================================gamma_power
    public class GammaPower : IGammaFunction
    {
        public GammaPower() { m_gamma = 1.0; }
        public GammaPower(double g) { m_gamma = g; }

        //public void Gamma(double g) { m_gamma = g; }
        public double Gamma
        {
            get
            {
                return m_gamma;
            }
            set
            {
                m_gamma = value;
            }
        }

        public double GetGamma(double x)
        {
            return Math.Pow(x, m_gamma);
        }

        double m_gamma;
    };


    //==========================================================gamma_threshold
    public class GammaThreshold : IGammaFunction
    {
        public GammaThreshold() { m_threshold = 0.5; }
        public GammaThreshold(double t) { m_threshold = t; }

        //public void Threshold(double t) { m_threshold = t; }
        public double Threshold
        {
            get
            {
                return m_threshold;
            }
            set
            {
                m_threshold = value;
            }
        }

        public double GetGamma(double x)
        {
            return (x < m_threshold) ? 0.0 : 1.0;
        }

        double m_threshold;
    };


    //============================================================gamma_linear
    public class GammaLinear : IGammaFunction
    {
        public GammaLinear()
        {
            m_start = (0.0);
            m_end = (1.0);
        }
        public GammaLinear(double s, double e)
        {
            m_start = (s);
            m_end = (e);
        }

        public void Set(double s, double e) { m_start = s; m_end = e; }


        //public void Start(double s) { m_start = s; }
        //public void End(double e) { m_end = e; }
        public double Start { get { return m_start; } set { m_start = value; } }
        public double End { get { return m_end; } set { m_end = value; } }

        public double GetGamma(double x)
        {
            if (x < m_start) return 0.0;
            if (x > m_end) return 1.0;
            double EndMinusStart = m_end - m_start;
            if (EndMinusStart != 0)
                return (x - m_start) / EndMinusStart;
            else
                return 0.0;
        }

        double m_start;
        double m_end;
    };


    //==========================================================gamma_multiply
    public class GammaMultiply : IGammaFunction
    {
        public GammaMultiply()
        {
            m_mul = (1.0);
        }
        public GammaMultiply(double v)
        {
            m_mul = (v);
        }

        //public void ScaleFactor(double v) { m_mul = v; }
        public double ScaleFactor { get { return m_mul; } set { m_mul = value; } }

        public double GetGamma(double x)
        {
            double y = x * m_mul;
            if (y > 1.0) y = 1.0;
            return y;
        }

        double m_mul;
    };
}