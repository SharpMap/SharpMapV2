
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
//
// Arc vertex generator
//
//----------------------------------------------------------------------------

//#ifndef AGG_ARC_INCLUDED
//#define AGG_ARC_INCLUDED

//#include <math.h>
//#include "agg_basics.h"
using System;
using NPack.Interfaces;
namespace AGG.VertexSource
{

    //=====================================================================arc
    //
    // See Implementation agg_arc.cpp 
    //
    public class Arc<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        T m_OriginX;
        T m_OriginY;

        T m_RadiusX;
        T m_RadiusY;

        T m_StartAngle;
        T m_EndAngle;
        T m_Scale;
        EDirection m_Direction;

        T m_CurrentFlatenAngle;
        T m_FlatenDeltaAngle;

        bool m_IsInitialized;
        uint m_NextPathCommand;

        public enum EDirection
        {
            ClockWise,
            CounterClockWise,
        }

        public Arc()
        {
            m_Scale = default(T).One;
            m_IsInitialized = false;
        }

        public Arc(T OriginX, T OriginY,
             T RadiusX, T RadiusY,
             T Angle1, T Angle2)
            : this(OriginX, OriginY, RadiusX, RadiusY, Angle1, Angle2, EDirection.CounterClockWise)
        {

        }

        public Arc(T OriginX, T OriginY,
             T RadiusX, T RadiusY,
             T Angle1, T Angle2,
             EDirection Direction)
        {
            m_OriginX = OriginX;
            m_OriginY = OriginY;
            m_RadiusX = RadiusX;
            m_RadiusY = RadiusY;
            m_Scale = default(T).One;
            Normalize(Angle1, Angle2, Direction);
        }

        public void Init(T OriginX, T OriginY,
                  T RadiusX, T RadiusY,
                  T Angle1, T Angle2)
        {
            Init(OriginX, OriginY, RadiusX, RadiusY, Angle1, Angle2, EDirection.CounterClockWise);
        }

        public void Init(T OriginX, T OriginY,
                   T RadiusX, T RadiusY,
                   T Angle1, T Angle2,
                   EDirection Direction)
        {
            m_OriginX = OriginX;
            m_OriginY = OriginY;
            m_RadiusX = RadiusX;
            m_RadiusY = RadiusY;
            Normalize(Angle1, Angle2, Direction);
        }


        public T ApproximationScale
        {
            get { return m_Scale; }
            set
            {
                m_Scale = value;
                if (m_IsInitialized)
                {
                    Normalize(m_StartAngle, m_EndAngle, m_Direction);
                }
            }
        }

        public void Rewind(uint unused)
        {
            m_NextPathCommand = (uint)Path.Commands.MoveTo;
            m_CurrentFlatenAngle = m_StartAngle;
        }

        public uint Vertex(out T x, out T y)
        {
            x = default(T).Zero;
            y = default(T).Zero;

            if (Path.IsStop(m_NextPathCommand))
            {
                return (uint)Path.Commands.Stop;
            }

            if ((m_CurrentFlatenAngle.LessThan(m_EndAngle.Subtract(m_FlatenDeltaAngle.Divide(default(T).Set(4))))) != ((int)EDirection.CounterClockWise == 1))
            {
                //x = m_OriginX + Math.Cos(m_EndAngle) * m_RadiusX;
                //y = m_OriginY + Math.Sin(m_EndAngle) * m_RadiusY;

                x = m_OriginX.Add(m_EndAngle.Cos()).Multiply(m_RadiusX);
                y = m_OriginY.Add(m_EndAngle.Sin()).Multiply(m_RadiusY);

                m_NextPathCommand = (uint)Path.Commands.Stop;

                return (uint)Path.Commands.LineTo;
            }

            //x = m_OriginX + Math.Cos(m_CurrentFlatenAngle) * m_RadiusX;
            //y = m_OriginY + Math.Sin(m_CurrentFlatenAngle) * m_RadiusY;

            x = m_OriginX.Add(m_EndAngle.Cos()).Multiply(m_RadiusX);
            y = m_OriginY.Add(m_EndAngle.Sin()).Multiply(m_RadiusY);


            m_CurrentFlatenAngle = m_CurrentFlatenAngle.Add(m_FlatenDeltaAngle);

            uint CurrentPathCommand = m_NextPathCommand;
            m_NextPathCommand = (uint)Path.Commands.LineTo;
            return CurrentPathCommand;
        }

        private void Normalize(T Angle1, T Angle2, EDirection Direction)
        {
            //double ra = (Math.Abs(m_RadiusX) + Math.Abs(m_RadiusY)) / 2;

            T ra = (m_RadiusX.Abs().Add(m_RadiusY.Abs())).Divide(default(T).Set(2));


            //m_FlatenDeltaAngle = Math.Acos(ra / (ra + 0.125 / m_Scale)) * 2;
            m_FlatenDeltaAngle = ra.Divide(ra.Add(default(T).Set(0.125)).Divide(m_Scale)).Acos().Multiply(M.New<T>(2));


            if (Direction == EDirection.CounterClockWise)
            {
                while (Angle2.LessThan(Angle1))
                {
                    //Angle2 += Math.PI * 2.0;
                    Angle2.AddEquals(M.PI<T>().Multiply(2));
                }
            }
            else
            {
                while (Angle1.LessThan(Angle2))
                {
                    //Angle1 += Math.PI * 2.0;

                    Angle1.AddEquals(M.PI<T>().Multiply(2));
                }
                m_FlatenDeltaAngle = m_FlatenDeltaAngle.Negative();
            }
            m_Direction = Direction;
            m_StartAngle = Angle1;
            m_EndAngle = Angle2;
            m_IsInitialized = true;
        }
    };
}
