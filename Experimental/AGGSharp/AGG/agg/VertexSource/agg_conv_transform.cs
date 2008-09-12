
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
using AGG.Transform;
using NPack.Interfaces;
using NPack;
namespace AGG.VertexSource
{
    //----------------------------------------------------------conv_transform
    public class ConvTransform<T> : IVertexSource<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        private IVertexSource<T> m_VertexSource;
        private IAffineTransformMatrix<T> m_Transform;

        public ConvTransform(IVertexSource<T> VertexSource, ITransform<T> InTransform)
        {
            ///todo
            throw new NotImplementedException();
        }


        public ConvTransform(IVertexSource<T> VertexSource, IAffineTransformMatrix<T> InTransform)
        {
            m_VertexSource = VertexSource;
            m_Transform = InTransform;
        }

        public void Attach(IVertexSource<T> VertexSource) { m_VertexSource = VertexSource; }

        public void Rewind(uint path_id)
        {
            m_VertexSource.Rewind(path_id);
        }

        public uint Vertex(out T x, out T y)
        {
            uint cmd = m_VertexSource.Vertex(out x, out y);
            if (Path.IsVertex(cmd))
            {
                IVector<T> v = m_Transform.TransformVector(MatrixFactory<T>.CreateVector2D(x, y));
                //m_Transform.Transform(ref x, ref y);
                x = v[0];
                y = v[1];
            }
            return cmd;
        }

        public void Transformer(IAffineTransformMatrix<T> InTransform)
        {
            m_Transform = InTransform;
        }
    };
}