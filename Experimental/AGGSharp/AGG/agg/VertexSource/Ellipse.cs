
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
// class ellipse
//
//----------------------------------------------------------------------------
using System;
using NPack.Interfaces;
using PathCommands = AGG.Path.Commands;
using PathFlags = AGG.Path.Flags;

namespace AGG.VertexSource
{

    //----------------------------------------------------------------ellipse
    public class Ellipse<T> : IVertexSource<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private T m_x;

        public T X
        {
            get { return m_x; }
            set { m_x = value; }
        }
        private T m_y;

        public T Y
        {
            get { return m_y; }
            set { m_y = value; }
        }
        private T m_rx;

        public T RX
        {
            get { return m_rx; }
            set { m_rx = value; }
        }
        private T m_ry;

        public T RY
        {
            get { return m_ry; }
            set { m_ry = value; }
        }
        private T m_scale;
        private uint m_num;
        private uint m_step;
        private bool m_cw;

        public Ellipse()
        {
            m_x = M.Zero<T>();
            m_y = M.Zero<T>();
            m_rx = M.One<T>();
            m_ry = M.One<T>();
            m_scale = M.One<T>();
            m_num = 4;
            m_step = 0;
            m_cw = false;
        }

        public Ellipse(double OriginX, double OriginY, double RadiusX, double RadiusY)
            : this(M.New<T>(OriginX), M.New<T>(OriginY), M.New<T>(RadiusX), M.New<T>(RadiusY))
        { }

        public Ellipse(T OriginX, T OriginY, T RadiusX, T RadiusY)
            : this(OriginX, OriginY, RadiusX, RadiusY, 0, false)
        {

        }

        public Ellipse(double OriginX, double OriginY, double RadiusX, double RadiusY, uint num_steps)
            : this(M.New<T>(OriginX), M.New<T>(OriginY), M.New<T>(RadiusX), M.New<T>(RadiusY), num_steps)
        {
        }

        public Ellipse(T OriginX, T OriginY, T RadiusX, T RadiusY, uint num_steps)
            : this(OriginX, OriginY, RadiusX, RadiusY, num_steps, false)
        {

        }

        public Ellipse(double OriginX, double OriginY, double RadiusX, double RadiusY,
               uint num_steps, bool cw)
            : this(M.New<T>(OriginX), M.New<T>(OriginY), M.New<T>(RadiusX), M.New<T>(RadiusY), num_steps, cw)
        { }

        public Ellipse(T OriginX, T OriginY, T RadiusX, T RadiusY,
                uint num_steps, bool cw)
        {
            m_x = OriginX;
            m_y = OriginY;
            m_rx = RadiusX;
            m_ry = RadiusY;
            m_scale = M.One<T>();
            m_num = num_steps;
            m_step = 0;
            m_cw = cw;
            if (m_num == 0) CalcNumSteps();
        }
        public void Init(double OriginX, double OriginY, double RadiusX, double RadiusY)
        {
            Init(M.New<T>(OriginX), M.New<T>(OriginY), M.New<T>(RadiusX), M.New<T>(RadiusY));
        }
        public void Init(T OriginX, T OriginY, T RadiusX, T RadiusY)
        {
            Init(OriginX, OriginY, RadiusX, RadiusY, 0, false);
        }

        public void Init(double OriginX, double OriginY, double RadiusX, double RadiusY, uint num_steps)
        {
            Init(M.New<T>(OriginX), M.New<T>(OriginY), M.New<T>(RadiusX), M.New<T>(RadiusY), num_steps);
        }

        public void Init(T OriginX, T OriginY, T RadiusX, T RadiusY, uint num_steps)
        {
            Init(OriginX, OriginY, RadiusX, RadiusY, num_steps, false);
        }

        public void Init(double OriginX, double OriginY, double RadiusX, double RadiusY,
                  uint num_steps, bool cw)
        {
            Init(M.New<T>(OriginX), M.New<T>(OriginY), M.New<T>(RadiusX), M.New<T>(RadiusY), num_steps, cw);
        }

        public void Init(T OriginX, T OriginY, T RadiusX, T RadiusY,
                  uint num_steps, bool cw)
        {
            X = OriginX;
            Y = OriginY;
            RX = RadiusX;
            RY = RadiusY;
            m_num = num_steps;
            m_step = 0;
            m_cw = cw;
            if (m_num == 0) CalcNumSteps();
        }

        public T ApproximationScale
        {
            set
            {
                m_scale = value;
                CalcNumSteps();
            }
        }

        public void Rewind(uint path_id)
        {
            m_step = 0;
        }

        public uint Vertex(out T x, out T y)
        {
            x = M.Zero<T>();
            y = M.Zero<T>();
            if (m_step == m_num)
            {
                ++m_step;
                return (int)PathCommands.EndPoly | (int)PathFlags.Close | (int)PathFlags.CCW;
            }
            if (m_step > m_num) return (uint)PathCommands.Stop;

            T angle = M.New<T>(m_step).Divide(m_num).Multiply(2.0).Multiply(M.PI<T>());
            if (m_cw) angle = M.PI<T>().Multiply(2.0).Subtract(angle);
            x = X.Add(angle.Cos()).Multiply(RX);
            y = Y.Add(angle.Sin()).Multiply(RY);
            m_step++;
            return ((m_step == 1) ? (uint)PathCommands.MoveTo : (uint)PathCommands.LineTo);
        }

        private void CalcNumSteps()
        {
            //double ra = (Math.Abs(RX) + Math.Abs(RY)) / 2;
            T ra = RX.Abs().Add(RY.Abs()).Divide(2);

            //double da = Math.Acos(ra / (ra + 0.125 / m_scale)) * 2;

            T da = ra.Divide(ra.Add(0.125).Divide(m_scale)).Acos().Multiply(2);

            m_num = (uint)M.PI<T>().Multiply(2).Divide(da).Round().ToInt();
        }
    };
}



//#endif


