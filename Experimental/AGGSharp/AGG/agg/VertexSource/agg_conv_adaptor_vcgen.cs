
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
using NPack.Interfaces;
namespace AGG.VertexSource
{
    //------------------------------------------------------------null_markers
    public struct NullMarkers<T> : IMarkers<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable
    {
        public void RemoveAll() { }
        public void AddVertex(T x, T y, uint unknown) { }
        public void PrepareSrc() { }

        public void Rewind(uint unknown) { }
        public uint Vertex(ref T x, ref T y) { return (uint)Path.Commands.Stop; }
    };


    //------------------------------------------------------conv_adaptor_vcgen
    public class ConvAdaptorVCGen<T> where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable
    {
        private enum Status
        {
            Initial,
            Accumulate,
            Generate
        };

        public ConvAdaptorVCGen(IVertexSource<T> source, IGenerator<T> generator)
        {
            m_markers = new NullMarkers<T>();
            m_source = source;
            m_generator = generator;
            m_status = Status.Initial;
        }

        public ConvAdaptorVCGen(IVertexSource<T> source, IGenerator<T> generator, IMarkers<T> markers)
            : this(source, generator)
        {
            m_markers = markers;
        }
        void Attach(IVertexSource<T> source) { m_source = source; }

        protected IGenerator<T> Generator { get { return m_generator; } }

        IMarkers<T> Markers() { return m_markers; }

        public void Rewind(uint path_id)
        {
            m_source.Rewind(path_id);
            m_status = Status.Initial;
        }

        public uint Vertex(out T x, out T y)
        {
            x = default(T).Zero;
            y = default(T).Zero;
            uint cmd = (uint)Path.Commands.Stop;
            bool done = false;
            while (!done)
            {
                switch (m_status)
                {
                    case Status.Initial:
                        m_markers.RemoveAll();
                        m_last_cmd = m_source.Vertex(out m_start_x, out m_start_y);
                        m_status = Status.Accumulate;
                        goto case Status.Accumulate;

                    case Status.Accumulate:
                        if (Path.IsStop(m_last_cmd)) return (uint)Path.Commands.Stop;

                        m_generator.RemoveAll();
                        m_generator.AddVertex(m_start_x, m_start_y, (uint)Path.Commands.MoveTo);
                        m_markers.AddVertex(m_start_x, m_start_y, (uint)Path.Commands.MoveTo);

                        for (; ; )
                        {
                            cmd = m_source.Vertex(out x, out y);
                            //DebugFile.Print("x=" + x.ToString() + " y=" + y.ToString() + "\n");
                            if (Path.IsVertex(cmd))
                            {
                                m_last_cmd = cmd;
                                if (Path.IsMoveTo(cmd))
                                {
                                    m_start_x = x;
                                    m_start_y = y;
                                    break;
                                }
                                m_generator.AddVertex(x, y, cmd);
                                m_markers.AddVertex(x, y, (uint)Path.Commands.LineTo);
                            }
                            else
                            {
                                if (Path.IsStop(cmd))
                                {
                                    m_last_cmd = (uint)Path.Commands.Stop;
                                    break;
                                }
                                if (Path.IsEndPoly(cmd))
                                {
                                    m_generator.AddVertex(x, y, cmd);
                                    break;
                                }
                            }
                        }
                        m_generator.Rewind(0);
                        m_status = Status.Generate;
                        goto case Status.Generate;

                    case Status.Generate:
                        cmd = m_generator.Vertex(ref x, ref y);
                        //DebugFile.Print("x=" + x.ToString() + " y=" + y.ToString() + "\n");
                        if (Path.IsStop(cmd))
                        {
                            m_status = Status.Accumulate;
                            break;
                        }
                        done = true;
                        break;
                }
            }
            return cmd;
        }

        private IVertexSource<T> m_source;
        private IGenerator<T> m_generator;
        private IMarkers<T> m_markers;
        private Status m_status;
        private uint m_last_cmd;
        private T m_start_x;
        private T m_start_y;
    };
}