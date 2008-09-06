/*
** License Applicability. Except to the extent portions of this file are
** made subject to an alternative license as permitted in the SGI Free
** Software License B, Version 1.1 (the "License"), the contents of this
** file are subject only to the provisions of the License. You may not use
** this file except in compliance with the License. You may obtain a copy
** of the License at Silicon Graphics, Inc., attn: Legal Services, 1600
** Amphitheatre Parkway, Mountain View, CA 94043-1351, or at:
** 
** http://oss.sgi.com/projects/FreeB
** 
** Note that, as provided in the License, the Software is distributed on an
** "AS IS" basis, with ALL EXPRESS AND IMPLIED WARRANTIES AND CONDITIONS
** DISCLAIMED, INCLUDING, WITHOUT LIMITATION, ANY IMPLIED WARRANTIES AND
** CONDITIONS OF MERCHANTABILITY, SATISFACTORY QUALITY, FITNESS FOR A
** PARTICULAR PURPOSE, AND NON-INFRINGEMENT.
** 
** Original Code. The Original Code is: OpenGL Sample Implementation,
** Version 1.2.1, released January 26, 2000, developed by Silicon Graphics,
** Inc. The Original Code is Copyright (c) 1991-2000 Silicon Graphics, Inc.
** Copyright in any portions created by third parties is as indicated
** elsewhere herein. All Rights Reserved.
** 
** Additional Notice Provisions: The application programming interfaces
** established by SGI in conjunction with the Original Code are The
** OpenGL(R) Graphics System: A Specification (Version 1.2.1), released
** April 1, 1999; The OpenGL(R) Graphics System Utility Library (Version
** 1.3), released November 4, 1998; and OpenGL(R) Graphics with the X
** Window System(R) (Version 1.3), released October 19, 1998. This software
** was created using the OpenGL(R) version 1.2.1 Sample Implementation
** published by SGI, but has not been independently verified as being
** compliant with the OpenGL(R) version 1.2.1 Specification.
**
*/
/*
** Author: Eric Veach, July 1994.
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
**
*/

using System;
using NPack.Interfaces;

namespace Tesselate
{
    public class HalfEdge<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>

    {
        public int debugIndex;
        public bool isFirstHalfEdge;
        public HalfEdge<T> nextHalfEdge;		/* doubly-linked list (prev==Sym.next) */
        public HalfEdge<T> otherHalfOfThisEdge;		/* same edge, opposite direction */
        public HalfEdge<T> nextEdgeCCWAroundOrigin;		/* next edge CCW around origin */
        public HalfEdge<T> nextEdgeCCWAroundLeftFace;		/* next edge CCW around left face */
        public ContourVertex<T> originVertex;		/* origin vertex */
        public Face<T> leftFace;		/* left face */

        /* Internal data (keep hidden) */
        public ActiveRegion<T> regionThisIsUpperEdgeOf;	/* a region with this upper edge (sweep.c) */

        public int winding;	// change in winding number when crossing from the right face to the left face

        public Face<T> rightFace { get { return otherHalfOfThisEdge.leftFace; } set { otherHalfOfThisEdge.leftFace = value; } }
        public HalfEdge<T> Lprev { get { return nextEdgeCCWAroundOrigin.otherHalfOfThisEdge; } set { nextEdgeCCWAroundOrigin.otherHalfOfThisEdge = value; } }
        public HalfEdge<T> Oprev { get { return otherHalfOfThisEdge.nextEdgeCCWAroundLeftFace; } }
        public ContourVertex<T> directionVertex { get { return otherHalfOfThisEdge.originVertex; } set { otherHalfOfThisEdge.originVertex = value; } }
        public HalfEdge<T> Dnext { get { return Rprev.otherHalfOfThisEdge; } }
        public HalfEdge<T> Dprev { get { return nextEdgeCCWAroundLeftFace.otherHalfOfThisEdge; } }
        public HalfEdge<T> Rprev { get { return otherHalfOfThisEdge.nextEdgeCCWAroundOrigin; } }

        public bool EdgeGoesLeft()
        {
            return this.directionVertex.VertLeq(this.originVertex);
        }

        public bool EdgeGoesRight()
        {
            return this.originVertex.VertLeq(this.directionVertex);
        }
    };
}