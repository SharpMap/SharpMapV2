
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
// conv_stroke
//
//----------------------------------------------------------------------------
using System;
using NPack.Interfaces;
namespace AGG.VertexSource
{
    //-------------------------------------------------------------conv_stroke
    public sealed class ConvStroke<T> : ConvAdaptorVCGen<T>, IVertexSource<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public ConvStroke(IVertexSource<T> vs)
            : base(vs, new VCGenStroke<T>())
        {
        }

        //public void LineCap(LineCap lc) { base.Generator().LineCap(lc); }
        //public void LineJoin(LineJoin lj) { base.Generator().LineJoin(lj); }
        //public void InnerJoin(InnerJoin ij) { base.Generator().InnerJoin(ij); }

        public LineCap LineCap { get { return base.Generator.LineCap; } set { base.Generator.LineCap = value; } }
        public LineJoin LineJoin { get { return base.Generator.LineJoin; } set { base.Generator.LineJoin = value; } }
        public InnerJoin InnerJoin { get { return base.Generator.InnerJoin; } set { base.Generator.InnerJoin = value; } }

        //public void Width(double w) { base.Generator().Width(w); }
        //public void MiterLimit(double ml) { base.Generator().MiterLimit(ml); }
        //public void MiterLimitTheta(double t) { base.Generator().MiterLimitTheta(t); }
        //public void InnerMiterLimit(double ml) { base.Generator().InnerMiterLimit(ml); }
        //public void ApproximationScale(double approxScale) { base.Generator().ApproximationScale(approxScale); }

        public T Width { get { return base.Generator.Width; } set { base.Generator.Width = value; } }
        public T MiterLimit { get { return base.Generator.MiterLimit; } set { base.Generator.MiterLimit = value; } }
        public T MiterLimitTheta { set { base.Generator.MiterLimitTheta = value; } }
        public T InnerMiterLimit { get { return base.Generator.InnerMiterLimit; } set { base.Generator.InnerMiterLimit = value; } }
        public T ApproximationScale { get { return base.Generator.ApproximationScale; } set { base.Generator.ApproximationScale = value; } }

        //public void Shorten(double s) { base.Generator().Shorten(s); }
        public T Shorten { get { return base.Generator.Shorten; } set { base.Generator.Shorten = value; } }
    };
}