
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
namespace AGG
{
    public interface IGenerator<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable
    {
        void RemoveAll();
        void AddVertex(T x, T y, uint unknown);
        void Rewind(uint path_id);
        uint Vertex(ref T x, ref T y);

        LineCap LineCap { get; set; }
        LineJoin LineJoin { get; set; }
        InnerJoin InnerJoin { get; set; }

        //void LineCap(LineCap lc);
        //void LineJoin(LineJoin lj);
        //void InnerJoin(InnerJoin ij);

        //void Width(double w);
        //void MiterLimit(double ml);
        //void MiterLimitTheta(double t);
        //void InnerMiterLimit(double ml);
        //void ApproximationScale(double approxScale);

        T Width { get; set; }
        T MiterLimit { get; set; }
        T MiterLimitTheta { set; }
        T InnerMiterLimit { get; set; }
        T ApproximationScale { get; set; }

        //void Shorten(double s);
        T Shorten { get; set; }
    };
}
