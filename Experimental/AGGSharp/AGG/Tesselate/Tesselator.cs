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
    public class Tesselator<T>
           where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        /* The begin/end calls must be properly nested.  We keep track of
        * the current state to enforce the ordering.
        */
        enum ProcessingState
        {
            Dormant, InPolygon, InContour
        };

        /* We cache vertex data for single-contour polygons so that we can
        * try a quick-and-dirty decomposition first.
        */
        const int MAX_CACHE_SIZE = 100;
        public static readonly T MAX_COORD = M.New<T>(1.0e150);

        public struct Vertex
        {
            public T x;
            public T y;
        }

        public struct VertexAndIndex
        {
            public T x;
            public T y;
            public int vertexIndex;
        }

        public enum TriangleListType
        {
            LineLoop,
            Triangles,
            TriangleStrip,
            TriangleFan
        }

        public enum WindingRuleType
        {
            Odd,
            NonZero,
            Positive,
            Negative,
            ABS_GEQ_Two,
        }

        ProcessingState processingState;		/* what begin/end calls have we seen? */
        HalfEdge<T> lastHalfEdge;	/* lastEdge.Org is the most recent vertex */
        public Mesh<T> mesh;		/* stores the input contours, and eventually the tessellation itself */

        public WindingRuleType windingRule;	// rule for determining polygon interior

        public Dictionary<T> edgeDictionary;		/* edge dictionary for sweep line */
        public C5.IntervalHeap<ContourVertex<T>> vertexPriorityQue = new C5.IntervalHeap<ContourVertex<T>>();
        public ContourVertex<T> currentSweepVertex;		/* current sweep event being processed */

        public delegate void CallCombineDelegate(T[] coords3, int[] data4,
            T[] weight4, out int outData);

        public event CallCombineDelegate callCombine;

        /*** state needed for rendering callbacks (see render.c) ***/

        bool boundaryOnly;	/* Extract contours, not triangles */
        public Face<T> lonelyTriList;
        /* list of triangles which could not be rendered as strips or fans */

        public delegate void CallBeginDelegate(TriangleListType type);
        public event CallBeginDelegate callBegin;

        public delegate void CallEdgeFlagDelegate(bool boundaryEdge);
        public event CallEdgeFlagDelegate callEdgeFlag;

        public delegate void CallVertexDelegate(int data);
        public event CallVertexDelegate callVertex;

        public delegate void CallEndDelegate();
        public event CallEndDelegate callEnd;

        public delegate void CallMeshDelegate(Mesh<T> mesh);
        public event CallMeshDelegate callMesh;


        /*** state needed to cache single-contour polygons for renderCache() */

        bool emptyCache;		/* empty cache on next vertex() call */
        public int cacheCount;		/* number of cached vertices */
        public Tesselator<T>.VertexAndIndex[] simpleVertexCache = new VertexAndIndex[MAX_CACHE_SIZE];	/* the vertex data */

        public Tesselator()
        {
            /* Only initialize fields which can be changed by the api.  Other fields
            * are initialized where they are used.
            */

            this.processingState = ProcessingState.Dormant;

            this.windingRule = Tesselator<T>.WindingRuleType.NonZero;
            this.boundaryOnly = false;
        }

        ~Tesselator()
        {
            RequireState(ProcessingState.Dormant);
        }

        public bool EdgeCallBackSet
        {
            get
            {
                return callEdgeFlag != null;
            }
        }

        public WindingRuleType WindingRule
        {
            get { return this.windingRule; }
            set { this.windingRule = value; }
        }

        public bool BoundaryOnly
        {
            get { return this.boundaryOnly; }
            set { this.boundaryOnly = value; }
        }

        public bool IsWindingInside(int numCrossings)
        {
            switch (this.windingRule)
            {
                case Tesselator<T>.WindingRuleType.Odd:
                    return (numCrossings & 1) != 0;

                case Tesselator<T>.WindingRuleType.NonZero:
                    return (numCrossings != 0);

                case Tesselator<T>.WindingRuleType.Positive:
                    return (numCrossings > 0);

                case Tesselator<T>.WindingRuleType.Negative:
                    return (numCrossings < 0);

                case Tesselator<T>.WindingRuleType.ABS_GEQ_Two:
                    return (numCrossings >= 2) || (numCrossings <= -2);
            }

            throw new Exception();
        }

        public void CallBegin(TriangleListType triangleType)
        {
            if (callBegin != null)
            {
                callBegin(triangleType);
            }
        }

        public void CallVertex(int vertexData)
        {
            if (callVertex != null)
            {
                callVertex(vertexData);
            }
        }

        public void CallEdegFlag(bool edgeState)
        {
            if (callEdgeFlag != null)
            {
                callEdgeFlag(edgeState);
            }
        }

        public void CallEnd()
        {
            if (callEnd != null)
            {
                callEnd();
            }
        }

        public void CallCombine(T[] coords3, int[] data4,
            T[] weight4, out int outData)
        {
            outData = 0;
            if (callCombine != null)
            {
                callCombine(coords3, data4, weight4, out outData);
            }
        }

        void GotoState(ProcessingState newProcessingState)
        {
            while (this.processingState != newProcessingState)
            {
                /* We change the current state one level at a time, to get to
                * the desired state.
                */
                if (this.processingState < newProcessingState)
                {
                    switch (this.processingState)
                    {
                        case ProcessingState.Dormant:
                            throw new Exception("MISSING_BEGIN_POLYGON");

                        case ProcessingState.InPolygon:
                            throw new Exception("MISSING_BEGIN_CONTOUR");

                        default:
                            break;
                    }
                }
                else
                {
                    switch (this.processingState)
                    {
                        case ProcessingState.InContour:
                            throw new Exception("MISSING_END_CONTOUR");

                        case ProcessingState.InPolygon:
                            throw new Exception("MISSING_END_POLYGON");

                        default:
                            break;
                    }
                }
            }
        }

        void RequireState(ProcessingState state)
        {
            if (this.processingState != state)
            {
                GotoState(state);
            }
        }

        public virtual void BeginPolygon()
        {
            RequireState(ProcessingState.Dormant);

            processingState = ProcessingState.InPolygon;
            cacheCount = 0;
            emptyCache = false;
            mesh = null;
        }

        public void BeginContour()
        {
            RequireState(ProcessingState.InPolygon);

            processingState = ProcessingState.InContour;
            lastHalfEdge = null;
            if (cacheCount > 0)
            {
                // Just set a flag so we don't get confused by empty contours
                emptyCache = true;
            }
        }

        bool AddVertex(T x, T y, int data)
        {
            HalfEdge<T> e;

            e = this.lastHalfEdge;
            if (e == null)
            {
                /* Make a self-loop (one vertex, one edge). */
                e = this.mesh.MakeEdge();
                Mesh<T>.meshSplice(e, e.otherHalfOfThisEdge);
            }
            else
            {
                /* Create a new vertex and edge which immediately follow e
                * in the ordering around the left face.
                */
                if (Mesh<T>.meshSplitEdge(e) == null)
                {
                    return false;
                }
                e = e.nextEdgeCCWAroundLeftFace;
            }

            /* The new vertex is now e.Org. */
            e.originVertex.clientIndex = data;
            e.originVertex.coords[0] = x;
            e.originVertex.coords[1] = y;

            /* The winding of an edge says how the winding number changes as we
            * cross from the edge''s right face to its left face.  We add the
            * vertices in such an order that a CCW contour will add +1 to
            * the winding number of the region inside the contour.
            */
            e.winding = 1;
            e.otherHalfOfThisEdge.winding = -1;

            this.lastHalfEdge = e;

            return true;
        }

        void EmptyCache()
        {
            VertexAndIndex[] v = this.simpleVertexCache;

            this.mesh = new Mesh<T>();

            for (int i = 0; i < this.cacheCount; i++)
            {
                this.AddVertex(v[i].x, v[i].y, v[i].vertexIndex);
            }
            this.cacheCount = 0;
            this.emptyCache = false;
        }

        void CacheVertex(T[] coords3, int data)
        {
            this.simpleVertexCache[this.cacheCount].vertexIndex = data;
            this.simpleVertexCache[this.cacheCount].x = coords3[0];
            this.simpleVertexCache[this.cacheCount].y = coords3[1];
            ++this.cacheCount;
        }

        public void AddVertex(T[] coords3, int data)
        {
            int i;
            T x;
            T[] clamped = new T[3];

            RequireState(ProcessingState.InContour);

            if (emptyCache)
            {
                EmptyCache();
                lastHalfEdge = null;
            }

            for (i = 0; i < 3; ++i)
            {
                x = coords3[i];
                if (x.LessThan(MAX_COORD.Negative()))
                {
                    throw new Exception("Your coordinate exceeded -" + MAX_COORD.ToString() + ".");
                }
                if (x.GreaterThan(MAX_COORD))
                {
                    throw new Exception("Your coordinate exceeded " + MAX_COORD.ToString() + ".");
                }
                clamped[i] = x;
            }

            if (mesh == null)
            {
                if (cacheCount < MAX_CACHE_SIZE)
                {
                    CacheVertex(clamped, data);
                    return;
                }
                EmptyCache();
            }

            AddVertex(clamped[0], clamped[1], data);
        }

        public void EndContour()
        {
            RequireState(ProcessingState.InContour);
            processingState = ProcessingState.InPolygon;
        }

        void CheckOrientation()
        {
            T area;
            Face<T> curFace, faceHead = this.mesh.faceHead;
            ContourVertex<T> vHead = this.mesh.vertexHead;
            HalfEdge<T> curHalfEdge;

            /* When we compute the normal automatically, we choose the orientation
             * so that the the sum of the signed areas of all contours is non-negative.
             */
            area = M.Zero<T>();
            for (curFace = faceHead.nextFace; curFace != faceHead; curFace = curFace.nextFace)
            {
                curHalfEdge = curFace.halfEdgeThisIsLeftFaceOf;
                if (curHalfEdge.winding <= 0)
                {
                    continue;
                }

                do
                {
                    area.AddEquals(curHalfEdge.originVertex.x.Subtract(curHalfEdge.directionVertex.x).Multiply(
                        curHalfEdge.originVertex.y.Add(curHalfEdge.directionVertex.y)));
                    curHalfEdge = curHalfEdge.nextEdgeCCWAroundLeftFace;
                } while (curHalfEdge != curFace.halfEdgeThisIsLeftFaceOf);
            }

            if (area.LessThan(0))
            {
                /* Reverse the orientation by flipping all the t-coordinates */
                for (ContourVertex<T> curVertex = vHead.nextVertex; curVertex != vHead; curVertex = curVertex.nextVertex)
                {
                    curVertex.y = curVertex.y.Negative();
                }
            }
        }

        void ProjectPolygon()
        {
            ContourVertex<T> v, vHead = this.mesh.vertexHead;

            // Project the vertices onto the sweep plane
            for (v = vHead.nextVertex; v != vHead; v = v.nextVertex)
            {
                v.x = v.coords[0];
                v.y = v.coords[1].Negative();
            }

            CheckOrientation();
        }

        public void EndPolygon()
        {
            RequireState(ProcessingState.InPolygon);
            processingState = ProcessingState.Dormant;

            if (this.mesh == null)
            {
                if (!this.EdgeCallBackSet && this.callMesh == null)
                {

                    /* Try some special code to make the easy cases go quickly
                    * (eg. convex polygons).  This code does NOT handle multiple contours,
                    * intersections, edge flags, and of course it does not generate
                    * an explicit mesh either.
                    */
                    if (RenderCache())
                    {
                        return;
                    }
                }

                EmptyCache(); /* could've used a label*/
            }

            /* Determine the polygon normal and project vertices onto the plane
            * of the polygon.
            */
            ProjectPolygon();

            /* __gl_computeInterior( this ) computes the planar arrangement specified
            * by the given contours, and further subdivides this arrangement
            * into regions.  Each region is marked "inside" if it belongs
            * to the polygon, according to the rule given by this.windingRule.
            * Each interior region is guaranteed to be monotone.
            */
            ActiveRegion<T>.ComputeInterior(this);

            bool rc = true;

            /* If the user wants only the boundary contours, we throw away all edges
            * except those which separate the interior from the exterior.
            * Otherwise we tessellate all the regions marked "inside".
            */
            if (this.boundaryOnly)
            {
                rc = this.mesh.SetWindingNumber(1, true);
            }
            else
            {
                rc = this.mesh.TessellateInterior();
            }

            this.mesh.CheckMesh();

            if (this.callBegin != null || this.callEnd != null
                || this.callVertex != null || this.callEdgeFlag != null)
            {
                if (this.boundaryOnly)
                {
                    RenderBoundary(mesh);  /* output boundary contours */
                }
                else
                {
                    RenderMesh(mesh);	   /* output strips and fans */
                }
            }
            if (this.callMesh != null)
            {
                /* Throw away the exterior faces, so that all faces are interior.
                * This way the user doesn't have to check the "inside" flag,
                * and we don't need to even reveal its existence.  It also leaves
                * the freedom for an implementation to not generate the exterior
                * faces in the first place.
                */
                mesh.DiscardExterior();
                callMesh(mesh); /* user wants the mesh itself */
                this.mesh = null;
                return;
            }
            this.mesh = null;
        }

        #region CodeFromRender
        class FaceCount
        {
            public FaceCount(int _size, HalfEdge<T> _eStart, RenderDelegate _render)
            {
                size = _size;
                eStart = _eStart;
                render = _render;
            }

            public int size;		/* number of triangles used */
            public HalfEdge<T> eStart;	/* edge where this primitive starts */
            public delegate void RenderDelegate(Tesselator<T> tess, HalfEdge<T> edge, int data);
            event RenderDelegate render;
            // routine to render this primitive

            public void CallRender(Tesselator<T> tess, HalfEdge<T> edge, int data)
            {
                render(tess, edge, data);
            }
        };

        /************************ Strips and Fans decomposition ******************/

        /* __gl_renderMesh( tess, mesh ) takes a mesh and breaks it into triangle
        * fans, strips, and separate triangles.  A substantial effort is made
        * to use as few rendering primitives as possible (ie. to make the fans
        * and strips as large as possible).
        *
        * The rendering output is provided as callbacks (see the api).
        */
        public void RenderMesh(Mesh<T> mesh)
        {
            Face<T> f;

            /* Make a list of separate triangles so we can render them all at once */
            this.lonelyTriList = null;

            for (f = mesh.faceHead.nextFace; f != mesh.faceHead; f = f.nextFace)
            {
                f.marked = false;
            }
            for (f = mesh.faceHead.nextFace; f != mesh.faceHead; f = f.nextFace)
            {

                /* We examine all faces in an arbitrary order.  Whenever we find
                * an unprocessed face F, we output a group of faces including F
                * whose size is maximum.
                */
                if (f.isInterior && !f.marked)
                {
                    RenderMaximumFaceGroup(f);
                    if (!f.marked)
                    {
                        throw new System.Exception();
                    }
                }
            }
            if (this.lonelyTriList != null)
            {
                RenderLonelyTriangles(this.lonelyTriList);
                this.lonelyTriList = null;
            }
        }


        void RenderMaximumFaceGroup(Face<T> fOrig)
        {
            /* We want to find the largest triangle fan or strip of unmarked faces
            * which includes the given face fOrig.  There are 3 possible fans
            * passing through fOrig (one centered at each vertex), and 3 possible
            * strips (one for each CCW permutation of the vertices).  Our strategy
            * is to try all of these, and take the primitive which uses the most
            * triangles (a greedy approach).
            */
            HalfEdge<T> e = fOrig.halfEdgeThisIsLeftFaceOf;
            FaceCount max = new FaceCount(1, e, new FaceCount.RenderDelegate(RenderTriangle));
            FaceCount newFace;

            max.size = 1;
            max.eStart = e;

            if (!this.EdgeCallBackSet)
            {
                newFace = MaximumFan(e); if (newFace.size > max.size) { max = newFace; }
                newFace = MaximumFan(e.nextEdgeCCWAroundLeftFace); if (newFace.size > max.size) { max = newFace; }
                newFace = MaximumFan(e.Lprev); if (newFace.size > max.size) { max = newFace; }

                newFace = MaximumStrip(e); if (newFace.size > max.size) { max = newFace; }
                newFace = MaximumStrip(e.nextEdgeCCWAroundLeftFace); if (newFace.size > max.size) { max = newFace; }
                newFace = MaximumStrip(e.Lprev); if (newFace.size > max.size) { max = newFace; }
            }

            max.CallRender(this, max.eStart, max.size);
        }

        FaceCount MaximumFan(HalfEdge<T> eOrig)
        {
            /* eOrig.Lface is the face we want to render.  We want to find the size
            * of a maximal fan around eOrig.Org.  To do this we just walk around
            * the origin vertex as far as possible in both directions.
            */
            FaceCount newFace = new FaceCount(0, null, new FaceCount.RenderDelegate(RenderFan));
            Face<T> trail = null;
            HalfEdge<T> e;

            for (e = eOrig; !e.leftFace.Marked(); e = e.nextEdgeCCWAroundOrigin)
            {
                Face<T>.AddToTrail(ref e.leftFace, ref trail);
                ++newFace.size;
            }
            for (e = eOrig; !e.rightFace.Marked(); e = e.Oprev)
            {
                Face<T> f = e.rightFace;
                Face<T>.AddToTrail(ref f, ref trail);
                e.rightFace = f;
                ++newFace.size;
            }
            newFace.eStart = e;

            Face<T>.FreeTrail(ref trail);
            return newFace;
        }


        bool IsEven(int n)
        {
            return (((n) & 1) == 0);
        }

        FaceCount MaximumStrip(HalfEdge<T> eOrig)
        {
            /* Here we are looking for a maximal strip that contains the vertices
            * eOrig.Org, eOrig.Dst, eOrig.Lnext.Dst (in that order or the
            * reverse, such that all triangles are oriented CCW).
            *
            * Again we walk forward and backward as far as possible.  However for
            * strips there is a twist: to get CCW orientations, there must be
            * an *even* number of triangles in the strip on one side of eOrig.
            * We walk the strip starting on a side with an even number of triangles;
            * if both side have an odd number, we are forced to shorten one side.
            */
            FaceCount newFace = new FaceCount(0, null, RenderStrip);
            int headSize = 0, tailSize = 0;
            Face<T> trail = null;
            HalfEdge<T> e, eTail, eHead;

            for (e = eOrig; !e.leftFace.Marked(); ++tailSize, e = e.nextEdgeCCWAroundOrigin)
            {
                Face<T>.AddToTrail(ref e.leftFace, ref trail);
                ++tailSize;
                e = e.Dprev;
                if (e.leftFace.Marked()) break;
                Face<T>.AddToTrail(ref e.leftFace, ref trail);
            }
            eTail = e;

            for (e = eOrig; !e.rightFace.Marked(); ++headSize, e = e.Dnext)
            {
                Face<T> f = e.rightFace;
                Face<T>.AddToTrail(ref f, ref trail);
                e.rightFace = f;
                ++headSize;
                e = e.Oprev;
                if (e.rightFace.Marked()) break;
                f = e.rightFace;
                Face<T>.AddToTrail(ref f, ref trail);
                e.rightFace = f;
            }
            eHead = e;

            newFace.size = tailSize + headSize;
            if (IsEven(tailSize))
            {
                newFace.eStart = eTail.otherHalfOfThisEdge;
            }
            else if (IsEven(headSize))
            {
                newFace.eStart = eHead;
            }
            else
            {
                /* Both sides have odd length, we must shorten one of them.  In fact,
                * we must start from eHead to guarantee inclusion of eOrig.Lface.
                */
                --newFace.size;
                newFace.eStart = eHead.nextEdgeCCWAroundOrigin;
            }

            Face<T>.FreeTrail(ref trail);
            return newFace;
        }


        void RenderTriangle(Tesselator<T> tess, HalfEdge<T> e, int size)
        {
            /* Just add the triangle to a triangle list, so we can render all
            * the separate triangles at once.
            */
            if (size != 1)
            {
                throw new Exception();
            }
            Face<T>.AddToTrail(ref e.leftFace, ref this.lonelyTriList);
        }


        void RenderLonelyTriangles(Face<T> f)
        {
            /* Now we render all the separate triangles which could not be
            * grouped into a triangle fan or strip.
            */
            HalfEdge<T> e;
            bool newState = false;
            bool edgeState = false;	/* force edge state output for first vertex */
            bool sentFirstEdge = false;

            this.CallBegin(Tesselator<T>.TriangleListType.Triangles);

            for (; f != null; f = f.trail)
            {
                /* Loop once for each edge (there will always be 3 edges) */

                e = f.halfEdgeThisIsLeftFaceOf;
                do
                {
                    if (this.EdgeCallBackSet)
                    {
                        /* Set the "edge state" to TRUE just before we output the
                        * first vertex of each edge on the polygon boundary.
                        */
                        newState = !e.rightFace.isInterior;
                        if (edgeState != newState || !sentFirstEdge)
                        {
                            sentFirstEdge = true;
                            edgeState = newState;
                            this.CallEdegFlag(edgeState);
                        }
                    }

                    this.CallVertex(e.originVertex.clientIndex);

                    e = e.nextEdgeCCWAroundLeftFace;
                } while (e != f.halfEdgeThisIsLeftFaceOf);
            }

            this.CallEnd();
        }


        static void RenderFan(Tesselator<T> tess, HalfEdge<T> e, int size)
        {
            /* Render as many CCW triangles as possible in a fan starting from
            * edge "e".  The fan *should* contain exactly "size" triangles
            * (otherwise we've goofed up somewhere).
            */
            tess.CallBegin(Tesselator<T>.TriangleListType.TriangleFan);
            tess.CallVertex(e.originVertex.clientIndex);
            tess.CallVertex(e.directionVertex.clientIndex);

            while (!e.leftFace.Marked())
            {
                e.leftFace.marked = true;
                --size;
                e = e.nextEdgeCCWAroundOrigin;
                tess.CallVertex(e.directionVertex.clientIndex);
            }

            if (size != 0)
            {
                throw new Exception();
            }
            tess.CallEnd();
        }


        static void RenderStrip(Tesselator<T> tess, HalfEdge<T> halfEdge, int size)
        {
            /* Render as many CCW triangles as possible in a strip starting from
            * edge "e".  The strip *should* contain exactly "size" triangles
            * (otherwise we've goofed up somewhere).
            */
            tess.CallBegin(Tesselator<T>.TriangleListType.TriangleStrip);
            tess.CallVertex(halfEdge.originVertex.clientIndex);
            tess.CallVertex(halfEdge.directionVertex.clientIndex);

            while (!halfEdge.leftFace.Marked())
            {
                halfEdge.leftFace.marked = true;
                --size;
                halfEdge = halfEdge.Dprev;
                tess.CallVertex(halfEdge.originVertex.clientIndex);
                if (halfEdge.leftFace.Marked()) break;

                halfEdge.leftFace.marked = true;
                --size;
                halfEdge = halfEdge.nextEdgeCCWAroundOrigin;
                tess.CallVertex(halfEdge.directionVertex.clientIndex);
            }

            if (size != 0)
            {
                throw new Exception();
            }
            tess.CallEnd();
        }


        /************************ Boundary contour decomposition ******************/

        /* Takes a mesh, and outputs one
        * contour for each face marked "inside".  The rendering output is
        * provided as callbacks.
        */
        public void RenderBoundary(Mesh<T> mesh)
        {
            for (Face<T> curFace = mesh.faceHead.nextFace; curFace != mesh.faceHead; curFace = curFace.nextFace)
            {
                if (curFace.isInterior)
                {
                    this.CallBegin(Tesselator<T>.TriangleListType.LineLoop);
                    HalfEdge<T> curHalfEdge = curFace.halfEdgeThisIsLeftFaceOf;
                    do
                    {
                        this.CallVertex(curHalfEdge.originVertex.clientIndex);
                        curHalfEdge = curHalfEdge.nextEdgeCCWAroundLeftFace;
                    } while (curHalfEdge != curFace.halfEdgeThisIsLeftFaceOf);
                    this.CallEnd();
                }
            }
        }


        /************************ Quick-and-dirty decomposition ******************/

        const int SIGN_INCONSISTENT = 2;

        int ComputeNormal(double[] norm3)
        /*
        * Check that each triangle in the fan from v0 has a
        * consistent orientation with respect to norm3[].  If triangles are
        * consistently oriented CCW, return 1; if CW, return -1; if all triangles
        * are degenerate return 0; otherwise (no consistent orientation) return
        * SIGN_INCONSISTENT.
        */
        {
            Tesselator<T>.VertexAndIndex[] vCache = this.simpleVertexCache;
            Tesselator<T>.VertexAndIndex v0 = vCache[0];
            int vcIndex;
            T dot, xc, yc, xp, yp;
            T[] n = new T[3];
            int sign = 0;

            /* Find the polygon normal.  It is important to get a reasonable
            * normal even when the polygon is self-intersecting (eg. a bowtie).
            * Otherwise, the computed normal could be very tiny, but perpendicular
            * to the true plane of the polygon due to numerical noise.  Then all
            * the triangles would appear to be degenerate and we would incorrectly
            * decompose the polygon as a fan (or simply not render it at all).
            *
            * We use a sum-of-triangles normal algorithm rather than the more
            * efficient sum-of-trapezoids method (used in CheckOrientation()
            * in normal.c).  This lets us explicitly reverse the signed area
            * of some triangles to get a reasonable normal in the self-intersecting
            * case.
            */
            vcIndex = 1;
            xc = vCache[vcIndex].x.Subtract(v0.x);
            yc = vCache[vcIndex].y.Subtract(v0.y);
            while (++vcIndex < this.cacheCount)
            {
                xp = xc; yp = yc;
                xc = vCache[vcIndex].x.Subtract(v0.x);
                yc = vCache[vcIndex].y.Subtract(v0.y);

                /* Compute (vp - v0) cross (vc - v0) */
                n[0] = M.Zero<T>();
                n[1] = M.Zero<T>();
                n[2] = xp.Multiply(yc).Subtract(yp.Multiply(xc));

                dot = n[0].Multiply(norm3[0]).Add(n[1].Multiply(norm3[1])).Add(n[2].Multiply(norm3[2]));
                if (dot.NotEqual(0))
                {
                    /* Check the new orientation for consistency with previous triangles */
                    if (dot.GreaterThan(0))
                    {
                        if (sign < 0)
                        {
                            return SIGN_INCONSISTENT;
                        }
                        sign = 1;
                    }
                    else
                    {
                        if (sign > 0)
                        {
                            return SIGN_INCONSISTENT;
                        }
                        sign = -1;
                    }
                }
            }

            return sign;
        }

        /* Takes a single contour and tries to render it
        * as a triangle fan.  This handles convex polygons, as well as some
        * non-convex polygons if we get lucky.
        *
        * Returns TRUE if the polygon was successfully rendered.  The rendering
        * output is provided as callbacks (see the api).
        */
        public bool RenderCache()
        {
            Tesselator<T>.VertexAndIndex[] vCache = this.simpleVertexCache;
            Tesselator<T>.VertexAndIndex v0 = vCache[0];
            double[] norm3 = new double[3];
            int sign;

            if (this.cacheCount < 3)
            {
                /* Degenerate contour -- no output */
                return true;
            }

            norm3[0] = 0;
            norm3[1] = 0;
            norm3[2] = 1;

            sign = this.ComputeNormal(norm3);
            if (sign == SIGN_INCONSISTENT)
            {
                // Fan triangles did not have a consistent orientation
                return false;
            }
            if (sign == 0)
            {
                // All triangles were degenerate
                return true;
            }

            /* Make sure we do the right thing for each winding rule */
            switch (this.windingRule)
            {
                case Tesselator<T>.WindingRuleType.Odd:
                case Tesselator<T>.WindingRuleType.NonZero:
                    break;
                case Tesselator<T>.WindingRuleType.Positive:
                    if (sign < 0) return true;
                    break;
                case Tesselator<T>.WindingRuleType.Negative:
                    if (sign > 0) return true;
                    break;
                case Tesselator<T>.WindingRuleType.ABS_GEQ_Two:
                    return true;
            }

            this.CallBegin(this.BoundaryOnly ? Tesselator<T>.TriangleListType.LineLoop
                : (this.cacheCount > 3) ? Tesselator<T>.TriangleListType.TriangleFan
                : Tesselator<T>.TriangleListType.Triangles);

            this.CallVertex(v0.vertexIndex);
            if (sign > 0)
            {
                for (int vcIndex = 1; vcIndex < this.cacheCount; ++vcIndex)
                {
                    this.CallVertex(vCache[vcIndex].vertexIndex);
                }
            }
            else
            {
                for (int vcIndex = this.cacheCount - 1; vcIndex > 0; --vcIndex)
                {
                    this.CallVertex(vCache[vcIndex].vertexIndex);
                }
            }
            this.CallEnd();
            return true;
        }
        #endregion
    };
}

//namespace NUnitTesselate
//{
//    [TestFixture]
//    public class Tesselate_Tests
//    {
//        int m_CurrentInputTest;
//        int m_CurrentOutputTest;
//        int m_CurrentOutput;
//        string m_LastString;

//        public class Vertex
//        {
//            public Vertex(double x, double y)
//            {
//                m_X = x;
//                m_Y = y;
//            }
//            public double m_X;
//            public double m_Y;
//        };

//        public List<Vertex> m_VertexList = new List<Vertex>();

//        public static string[][] m_InsructionStream = new string[][]
//            { 
//                new string[] { "BP", 
//                    "BC", "V", "0", "0", "V", "60", "0", "V", "60", "60", "V", "0", "60", "EC", 
//                    "BC", "V", "10", "10", "V", "50", "10", "V", "50", "50", "V", "10", "50", "EC", 
//                    "BC", "V", "20", "20", "V", "40", "20", "V", "40", "40", "V", "20", "40", "EC", 
//                    "EP",},  // Three boxes CCW winding
//                new string[] { "BP", 
//                    "BC", "V", "0", "0", "V", "60", "0", "V", "60", "60", "V", "0", "60", "EC", 
//                    "BC", "V", "10", "10", "V", "10", "50", "V", "50", "50", "V", "50", "10", "EC", 
//                    "BC", "V", "20", "20", "V", "20", "40", "V", "40", "40", "V", "40", "20", "EC", 
//                    "EP",},  // Three boxes 1. CCW 2. CW 3. CW
//                new string[] { "BP", 
//                    "BC", "V", "10", "0", "V", "50", "0", "V", "50", "40", "V", "10", "40", "EC", 
//                    "BC", "V", "0", "10", "V", "60", "10", "V", "60", "20", "V", "0", "20", "EC", 
//                    "BC", "V", "30", "-10", "V", "40", "30", "V", "20", "30", "EC", 
//                    "EP",},  // Two boxes and a triangle all CCW
//                new string[] { "BP", 
//                    "BC", "V", "0", "0", "V", "70", "0", "V", "70", "40", "V", "30", "40",
//                    "V", "30", "30", "V", "40", "30", "V", "40", "50", "V", "20", "50",
//                    "V", "20", "20", "V", "50", "20", "V", "50", "60", "V", "10", "60",
//                    "V", "10", "10", "V", "60", "10", "V", "60", "70", "V", "0", "70",
//                    "V", "0", "0", "EC", 
//                    "EP",},  // One large CCW loop that makes about 4 boxes.
//            };

//        static string[][] m_TestOutput = new string[][]
//        {
//            // CCW, ODD
//            new string[] {"B", "STRIP", "V", "0", "V", "4", "V", "3", "V", "7", "V", "2", "V", "6", "V", "1", "V", 
//            "5", "V", "4", "E", "B", "FAN", "V", "11", "V", "8", "V", "9", "V", "10", "E", "B", "TRI", "V", "4", 
//            "V", "0", "V", "1", "E", },
//            // CCW, NON-ZERO
//            new string[] {"B", "STRIP", "V", "11", "V", "8", "V", "9", "V", "5", "V", "10", "V", "6", "V", "11", 
//            "V", "7", "V", "8", "V", "4", "V", "5", "V", "1", "V", "6", "V", "2", "V", "7", "V", "3", "V", "4", "V", 
//            "0", "V", "1", "E", "B", "TRI", "V", "11", "V", "9", "V", "10", "E", },
//            // CCW, POSITIVE
//            new string[] {"B", "STRIP", "V", "11", "V", "8", "V", "9", "V", "5", "V", "10", "V", "6", "V", "11", 
//            "V", "7", "V", "8", "V", "4", "V", "5", "V", "1", "V", "6", "V", "2", "V", "7", "V", "3", "V", "4", "V", 
//            "0", "V", "1", "E", "B", "TRI", "V", "11", "V", "9", "V", "10", "E", },
//            // CCW, NEGATIVE
//            new string[] {},
//            // CCW, ABS >= 2
//            new string[] {"B", "STRIP", "V", "6", "V", "7", "V", "11", "V", "8", "V", "9", "V", "5", "V", "10", "V", 
//            "6", "V", "11", "E", "B", "FAN", "V", "8", "V", "7", "V", "4", "V", "5", "E", "B", "TRI", "V", "11", 
//            "V", "9", "V", "10", "E", },
//            // CCW, ODD, EDGE FLAG
//            new string[] {"B", "TRI", "F", "0", "V", "9", "F", "1", "V", "11", "V", "8", "F", "0", "V", "11", "F", 
//            "1", "V", "9", "V", "10", "F", "0", "V", "0", "V", "4", "F", "1", "V", "3", "F", "0", "V", "4", "F", 
//            "1", "V", "0", "F", "0", "V", "1", "V", "4", "V", "1", "F", "1", "V", "5", "F", "0", "V", "5", "V", "1", 
//            "F", "1", "V", "6", "F", "0", "V", "3", "V", "7", "F", "1", "V", "2", "F", "0", "V", "7", "V", "3", "F", 
//            "1", "V", "4", "F", "0", "V", "2", "F", "1", "V", "7", "F", "0", "V", "6", "V", "2", "V", "6", "F", "1", 
//            "V", "1", "E", },
//            // CCW, NON-ZERO, EDGE FLAG
//            new string[] {"B", "TRI", "F", "0", "V", "9", "V", "11", "V", "8", "V", "11", "V", "9", "V", "10", "V", 
//            "4", "V", "8", "V", "7", "V", "8", "V", "4", "V", "5", "V", "8", "V", "5", "V", "9", "V", "9", "V", "5", 
//            "V", "10", "V", "7", "V", "11", "V", "6", "V", "11", "V", "7", "V", "8", "V", "6", "V", "11", "V", "10", 
//            "V", "6", "V", "10", "V", "5", "V", "0", "V", "4", "F", "1", "V", "3", "F", "0", "V", "4", "F", "1", 
//            "V", "0", "F", "0", "V", "1", "V", "4", "V", "1", "V", "5", "V", "5", "V", "1", "V", "6", "V", "3", "V", 
//            "7", "F", "1", "V", "2", "F", "0", "V", "7", "V", "3", "V", "4", "V", "2", "V", "7", "V", "6", "V", "2", 
//            "V", "6", "F", "1", "V", "1", "E", },
//            // CCW, POSITIVE, EDGE FLAG
//            new string[] {"B", "TRI", "F", "0", "V", "9", "V", "11", "V", "8", "V", "11", "V", "9", "V", "10", "V", 
//            "4", "V", "8", "V", "7", "V", "8", "V", "4", "V", "5", "V", "8", "V", "5", "V", "9", "V", "9", "V", "5", 
//            "V", "10", "V", "7", "V", "11", "V", "6", "V", "11", "V", "7", "V", "8", "V", "6", "V", "11", "V", "10", 
//            "V", "6", "V", "10", "V", "5", "V", "0", "V", "4", "F", "1", "V", "3", "F", "0", "V", "4", "F", "1", 
//            "V", "0", "F", "0", "V", "1", "V", "4", "V", "1", "V", "5", "V", "5", "V", "1", "V", "6", "V", "3", "V", 
//            "7", "F", "1", "V", "2", "F", "0", "V", "7", "V", "3", "V", "4", "V", "2", "V", "7", "V", "6", "V", "2", 
//            "V", "6", "F", "1", "V", "1", "E", },
//            // CCW, NEGATIVE, EDGE FLAG
//            new string[] {},
//            // CCW, ABS >= 2, EDGE FLAG
//            new string[] {"B", "TRI", "F", "0", "V", "9", "V", "11", "V", "8", "V", "11", "V", "9", "V", "10", "V", 
//            "4", "V", "8", "F", "1", "V", "7", "F", "0", "V", "8", "F", "1", "V", "4", "F", "0", "V", "5", "V", "8", 
//            "V", "5", "V", "9", "V", "9", "V", "5", "V", "10", "V", "7", "V", "11", "F", "1", "V", "6", "F", "0", 
//            "V", "11", "V", "7", "V", "8", "V", "6", "V", "11", "V", "10", "V", "6", "V", "10", "F", "1", "V", "5", 
//            "E", },
//            // CCW - CW - CW, ODD
//            new string[] {"B", "STRIP", "V", "0", "V", "4", "V", "3", "V", "5", "V", "2", "V", "6", "V", "1", "V", 
//            "7", "V", "4", "E", "B", "FAN", "V", "9", "V", "8", "V", "11", "V", "10", "E", "B", "TRI", "V", "4", 
//            "V", "0", "V", "1", "E", },
//            // CCW - CW - CW, NON-ZERO
//            new string[] {"B", "STRIP", "V", "0", "V", "4", "V", "3", "V", "5", "V", "2", "V", "6", "V", "1", "V", 
//            "7", "V", "4", "E", "B", "FAN", "V", "9", "V", "8", "V", "11", "V", "10", "E", "B", "TRI", "V", "4", 
//            "V", "0", "V", "1", "E", },
//            // CCW - CW - CW, POSITIVE
//            new string[] {"B", "STRIP", "V", "0", "V", "4", "V", "3", "V", "5", "V", "2", "V", "6", "V", "1", "V", 
//            "7", "V", "4", "E", "B", "TRI", "V", "4", "V", "0", "V", "1", "E", },
//            // CCW - CW - CW, NEGATIVE
//            new string[] {"B", "FAN", "V", "9", "V", "8", "V", "11", "V", "10", "E", },
//            // CCW - CW - CW, ABS >= 2
//            new string[] {},
//            // CCW - CW - CW, ODD, EDGE FLAG
//            new string[] {"B", "TRI", "F", "0", "V", "11", "F", "1", "V", "9", "V", "8", "F", "0", "V", "9", "F", 
//            "1", "V", "11", "V", "10", "F", "0", "V", "0", "V", "4", "F", "1", "V", "3", "F", "0", "V", "4", "F", 
//            "1", "V", "0", "F", "0", "V", "1", "V", "4", "V", "1", "F", "1", "V", "7", "F", "0", "V", "7", "V", "1", 
//            "F", "1", "V", "6", "F", "0", "V", "3", "V", "5", "F", "1", "V", "2", "F", "0", "V", "5", "V", "3", "F", 
//            "1", "V", "4", "F", "0", "V", "2", "F", "1", "V", "5", "F", "0", "V", "6", "V", "2", "V", "6", "F", "1", 
//            "V", "1", "E", },
//            // CCW - CW - CW, NON-ZERO, EDGE FLAG
//            new string[] {"B", "TRI", "F", "0", "V", "11", "F", "1", "V", "9", "V", "8", "F", "0", "V", "9", "F", 
//            "1", "V", "11", "V", "10", "F", "0", "V", "0", "V", "4", "F", "1", "V", "3", "F", "0", "V", "4", "F", 
//            "1", "V", "0", "F", "0", "V", "1", "V", "4", "V", "1", "F", "1", "V", "7", "F", "0", "V", "7", "V", "1", 
//            "F", "1", "V", "6", "F", "0", "V", "3", "V", "5", "F", "1", "V", "2", "F", "0", "V", "5", "V", "3", "F", 
//            "1", "V", "4", "F", "0", "V", "2", "F", "1", "V", "5", "F", "0", "V", "6", "V", "2", "V", "6", "F", "1", 
//            "V", "1", "E", },
//            // CCW - CW - CW, POSITIVE, EDGE FLAG
//            new string[] {"B", "TRI", "F", "0", "V", "0", "V", "4", "F", "1", "V", "3", "F", "0", "V", "4", "F", 
//            "1", "V", "0", "F", "0", "V", "1", "V", "4", "V", "1", "F", "1", "V", "7", "F", "0", "V", "7", "V", "1", 
//            "F", "1", "V", "6", "F", "0", "V", "3", "V", "5", "F", "1", "V", "2", "F", "0", "V", "5", "V", "3", "F", 
//            "1", "V", "4", "F", "0", "V", "2", "F", "1", "V", "5", "F", "0", "V", "6", "V", "2", "V", "6", "F", "1", 
//            "V", "1", "E", },
//            // CCW - CW - CW, NEGATIVE, EDGE FLAG
//            new string[] {"B", "TRI", "F", "0", "V", "11", "F", "1", "V", "9", "V", "8", "F", "0", "V", "9", "F", 
//            "1", "V", "11", "V", "10", "E", },
//            // CCW - CW - CW, ABS >= 2
//            new string[] {},
//            // RECTS & TRI, ODD
//            new string[] {"C", "10", "10", "5", "4", "3", "0", "0.0833333", "0.416667", "0.125", "0.375", "C", "10", 
//            "20", "6", "7", "3", "11", "0.0833333", "0.416667", "0.166667", "0.333333", "C", "22.5", "20", "8", "10", 
//            "6", "12", "0.125", "0.375", "0.125", "0.375", "C", "25", "10", "8", "13", "5", "11", "0.166667", "0.333333", 
//            "0.15", "0.35", "C", "27.5", "3.46142e-008", "8", "14", "1", "0", "0.25", "0.25", "0.21875", "0.28125", 
//            "C", "32.5", "1.34611e-008", "1", "15", "9", "8", "0.111111", "0.388889", "0.125", "0.375", "C", "35", 
//            "10", "5", "14", "9", "16", "0.142857", "0.357143", "0.166667", "0.333333", "C", "37.5", "20", "6", "13", 
//            "9", "17", "0.2", "0.3", "0.25", "0.25", "C", "50", "10", "5", "17", "2", "1", "0.3", "0.2", "0.125", 
//            "0.375", "C", "50", "20", "6", "18", "2", "19", "0.277778", "0.222222", "0.166667", "0.333333", "B", 
//            "FAN", "V", "7", "V", "4", "V", "11", "V", "12", "E", "B", "STRIP", "V", "12", "V", "13", "V", "3", "V", 
//            "10", "V", "2", "V", "9", "V", "20", "V", "18", "E", "B", "FAN", "V", "18", "V", "13", "V", "14", "V", 
//            "17", "E", "B", "FAN", "V", "20", "V", "19", "V", "5", "V", "6", "E", "B", "FAN", "V", "17", "V", "16", 
//            "V", "1", "V", "19", "E", "B", "FAN", "V", "11", "V", "0", "V", "15", "V", "14", "E", "B", "TRI", "V", 
//            "16", "V", "15", "V", "8", "E", },
//            // RECTS & TRI, NON-ZERO
//            new string[] {"C", "10", "10", "5", "4", "3", "0", "0.0833333", "0.416667", "0.125", "0.375", "C", "10", 
//            "20", "6", "7", "3", "11", "0.0833333", "0.416667", "0.166667", "0.333333", "C", "22.5", "20", "8", "10", 
//            "6", "12", "0.125", "0.375", "0.125", "0.375", "C", "25", "10", "8", "13", "5", "11", "0.166667", "0.333333", 
//            "0.15", "0.35", "C", "27.5", "3.46142e-008", "8", "14", "1", "0", "0.25", "0.25", "0.21875", "0.28125", 
//            "C", "32.5", "1.34611e-008", "1", "15", "9", "8", "0.111111", "0.388889", "0.125", "0.375", "C", "35", 
//            "10", "5", "14", "9", "16", "0.142857", "0.357143", "0.166667", "0.333333", "C", "37.5", "20", "6", "13", 
//            "9", "17", "0.2", "0.3", "0.25", "0.25", "C", "50", "10", "5", "17", "2", "1", "0.3", "0.2", "0.125", 
//            "0.375", "C", "50", "20", "6", "18", "2", "19", "0.277778", "0.222222", "0.166667", "0.333333", "B", 
//            "STRIP", "V", "7", "V", "11", "V", "12", "V", "14", "V", "13", "V", "18", "V", "9", "V", "20", "V", "2", 
//            "E", "B", "FAN", "V", "10", "V", "3", "V", "13", "V", "9", "V", "2", "V", "3", "E", "B", "STRIP", "V", 
//            "11", "V", "15", "V", "14", "V", "17", "V", "18", "V", "19", "V", "20", "V", "5", "V", "6", "E", "B", 
//            "FAN", "V", "17", "V", "15", "V", "16", "V", "1", "V", "19", "E", "B", "TRI", "V", "15", "V", "11", "V", 
//            "0", "V", "16", "V", "15", "V", "8", "V", "13", "V", "3", "V", "12", "V", "11", "V", "7", "V", "4", "E", 
//            },
//            // RECTS & TRI, POSITIVE
//            new string[] {"C", "10", "10", "5", "4", "3", "0", "0.0833333", "0.416667", "0.125", "0.375", "C", "10", 
//            "20", "6", "7", "3", "11", "0.0833333", "0.416667", "0.166667", "0.333333", "C", "22.5", "20", "8", "10", 
//            "6", "12", "0.125", "0.375", "0.125", "0.375", "C", "25", "10", "8", "13", "5", "11", "0.166667", "0.333333", 
//            "0.15", "0.35", "C", "27.5", "3.46142e-008", "8", "14", "1", "0", "0.25", "0.25", "0.21875", "0.28125", 
//            "C", "32.5", "1.34611e-008", "1", "15", "9", "8", "0.111111", "0.388889", "0.125", "0.375", "C", "35", 
//            "10", "5", "14", "9", "16", "0.142857", "0.357143", "0.166667", "0.333333", "C", "37.5", "20", "6", "13", 
//            "9", "17", "0.2", "0.3", "0.25", "0.25", "C", "50", "10", "5", "17", "2", "1", "0.3", "0.2", "0.125", 
//            "0.375", "C", "50", "20", "6", "18", "2", "19", "0.277778", "0.222222", "0.166667", "0.333333", "B", 
//            "STRIP", "V", "7", "V", "11", "V", "12", "V", "14", "V", "13", "V", "18", "V", "9", "V", "20", "V", "2", 
//            "E", "B", "FAN", "V", "10", "V", "3", "V", "13", "V", "9", "V", "2", "V", "3", "E", "B", "STRIP", "V", 
//            "11", "V", "15", "V", "14", "V", "17", "V", "18", "V", "19", "V", "20", "V", "5", "V", "6", "E", "B", 
//            "FAN", "V", "17", "V", "15", "V", "16", "V", "1", "V", "19", "E", "B", "TRI", "V", "15", "V", "11", "V", 
//            "0", "V", "16", "V", "15", "V", "8", "V", "13", "V", "3", "V", "12", "V", "11", "V", "7", "V", "4", "E", 
//            },
//            // RECTS & TRI, NEGATIVE
//            new string[] {"C", "10", "10", "5", "4", "3", "0", "0.0833333", "0.416667", "0.125", "0.375", "C", "10", 
//            "20", "6", "7", "3", "11", "0.0833333", "0.416667", "0.166667", "0.333333", "C", "22.5", "20", "8", "10", 
//            "6", "12", "0.125", "0.375", "0.125", "0.375", "C", "25", "10", "8", "13", "5", "11", "0.166667", "0.333333", 
//            "0.15", "0.35", "C", "27.5", "3.46142e-008", "8", "14", "1", "0", "0.25", "0.25", "0.21875", "0.28125", 
//            "C", "32.5", "1.34611e-008", "1", "15", "9", "8", "0.111111", "0.388889", "0.125", "0.375", "C", "35", 
//            "10", "5", "14", "9", "16", "0.142857", "0.357143", "0.166667", "0.333333", "C", "37.5", "20", "6", "13", 
//            "9", "17", "0.2", "0.3", "0.25", "0.25", "C", "50", "10", "5", "17", "2", "1", "0.3", "0.2", "0.125", 
//            "0.375", "C", "50", "20", "6", "18", "2", "19", "0.277778", "0.222222", "0.166667", "0.333333", },
//            // RECTS & TRI, ABS >= 2
//            new string[] {"C", "10", "10", "5", "4", "3", "0", "0.0833333", "0.416667", "0.125", "0.375", "C", "10", 
//            "20", "6", "7", "3", "11", "0.0833333", "0.416667", "0.166667", "0.333333", "C", "22.5", "20", "8", "10", 
//            "6", "12", "0.125", "0.375", "0.125", "0.375", "C", "25", "10", "8", "13", "5", "11", "0.166667", "0.333333", 
//            "0.15", "0.35", "C", "27.5", "3.46142e-008", "8", "14", "1", "0", "0.25", "0.25", "0.21875", "0.28125", 
//            "C", "32.5", "1.34611e-008", "1", "15", "9", "8", "0.111111", "0.388889", "0.125", "0.375", "C", "35", 
//            "10", "5", "14", "9", "16", "0.142857", "0.357143", "0.166667", "0.333333", "C", "37.5", "20", "6", "13", 
//            "9", "17", "0.2", "0.3", "0.25", "0.25", "C", "50", "10", "5", "17", "2", "1", "0.3", "0.2", "0.125", 
//            "0.375", "C", "50", "20", "6", "18", "2", "19", "0.277778", "0.222222", "0.166667", "0.333333", "B", 
//            "FAN", "V", "14", "V", "15", "V", "17", "V", "18", "V", "13", "V", "12", "V", "11", "E", "B", "FAN", 
//            "V", "9", "V", "10", "V", "13", "V", "18", "E", "B", "FAN", "V", "18", "V", "17", "V", "19", "V", "20", 
//            "E", "B", "TRI", "V", "17", "V", "15", "V", "16", "E", },
//            // RECTS & TRI, ODD, EDGE FLAG
//            new string[] {"C", "10", "10", "5", "4", "3", "0", "0.0833333", "0.416667", "0.125", "0.375", "C", "10", 
//            "20", "6", "7", "3", "11", "0.0833333", "0.416667", "0.166667", "0.333333", "C", "22.5", "20", "8", "10", 
//            "6", "12", "0.125", "0.375", "0.125", "0.375", "C", "25", "10", "8", "13", "5", "11", "0.166667", "0.333333", 
//            "0.15", "0.35", "C", "27.5", "3.46142e-008", "8", "14", "1", "0", "0.25", "0.25", "0.21875", "0.28125", 
//            "C", "32.5", "1.34611e-008", "1", "15", "9", "8", "0.111111", "0.388889", "0.125", "0.375", "C", "35", 
//            "10", "5", "14", "9", "16", "0.142857", "0.357143", "0.166667", "0.333333", "C", "37.5", "20", "6", "13", 
//            "9", "17", "0.2", "0.3", "0.25", "0.25", "C", "50", "10", "5", "17", "2", "1", "0.3", "0.2", "0.125", 
//            "0.375", "C", "50", "20", "6", "18", "2", "19", "0.277778", "0.222222", "0.166667", "0.333333", "B", 
//            "TRI", "F", "0", "V", "15", "F", "1", "V", "11", "V", "0", "F", "0", "V", "11", "F", "1", "V", "15", 
//            "V", "14", "V", "16", "V", "15", "V", "8", "F", "0", "V", "20", "F", "1", "V", "9", "V", "18", "F", "0", 
//            "V", "1", "F", "1", "V", "17", "V", "16", "F", "0", "V", "17", "F", "1", "V", "1", "V", "19", "F", "0", 
//            "V", "5", "F", "1", "V", "20", "V", "19", "F", "0", "V", "20", "F", "1", "V", "5", "V", "6", "F", "0", 
//            "V", "10", "F", "1", "V", "2", "F", "0", "V", "3", "V", "2", "F", "1", "V", "10", "F", "0", "V", "9", 
//            "V", "2", "V", "9", "F", "1", "V", "20", "F", "0", "V", "14", "F", "1", "V", "18", "V", "13", "F", "0", 
//            "V", "18", "F", "1", "V", "14", "V", "17", "F", "0", "V", "13", "F", "1", "V", "3", "V", "12", "F", "0", 
//            "V", "3", "F", "1", "V", "13", "F", "0", "V", "10", "V", "11", "F", "1", "V", "7", "V", "4", "F", "0", 
//            "V", "7", "F", "1", "V", "11", "V", "12", "E", },
//            // RECTS & TRI, NON-ZERO, EDGE FLAG
//            new string[] {"C", "10", "10", "5", "4", "3", "0", "0.0833333", "0.416667", "0.125", "0.375", "C", "10", 
//            "20", "6", "7", "3", "11", "0.0833333", "0.416667", "0.166667", "0.333333", "C", "22.5", "20", "8", "10", 
//            "6", "12", "0.125", "0.375", "0.125", "0.375", "C", "25", "10", "8", "13", "5", "11", "0.166667", "0.333333", 
//            "0.15", "0.35", "C", "27.5", "3.46142e-008", "8", "14", "1", "0", "0.25", "0.25", "0.21875", "0.28125", 
//            "C", "32.5", "1.34611e-008", "1", "15", "9", "8", "0.111111", "0.388889", "0.125", "0.375", "C", "35", 
//            "10", "5", "14", "9", "16", "0.142857", "0.357143", "0.166667", "0.333333", "C", "37.5", "20", "6", "13", 
//            "9", "17", "0.2", "0.3", "0.25", "0.25", "C", "50", "10", "5", "17", "2", "1", "0.3", "0.2", "0.125", 
//            "0.375", "C", "50", "20", "6", "18", "2", "19", "0.277778", "0.222222", "0.166667", "0.333333", "B", 
//            "TRI", "F", "0", "V", "15", "F", "1", "V", "11", "V", "0", "F", "0", "V", "11", "V", "15", "V", "14", 
//            "V", "15", "V", "17", "V", "14", "V", "17", "V", "15", "V", "16", "V", "16", "F", "1", "V", "15", "V", 
//            "8", "F", "0", "V", "20", "V", "9", "V", "18", "V", "19", "V", "18", "V", "17", "V", "18", "V", "19", 
//            "V", "20", "V", "1", "V", "17", "F", "1", "V", "16", "F", "0", "V", "17", "F", "1", "V", "1", "F", "0", 
//            "V", "19", "V", "5", "V", "20", "F", "1", "V", "19", "F", "0", "V", "20", "F", "1", "V", "5", "V", "6", 
//            "F", "0", "V", "10", "F", "1", "V", "2", "F", "0", "V", "3", "V", "2", "V", "10", "V", "9", "V", "2", 
//            "V", "9", "F", "1", "V", "20", "F", "0", "V", "13", "V", "9", "V", "10", "V", "9", "V", "13", "V", "18", 
//            "V", "14", "V", "18", "V", "13", "V", "18", "V", "14", "V", "17", "V", "14", "V", "12", "V", "11", "V", 
//            "12", "V", "14", "V", "13", "V", "13", "F", "1", "V", "3", "F", "0", "V", "12", "V", "3", "V", "13", 
//            "V", "10", "V", "11", "F", "1", "V", "7", "V", "4", "F", "0", "V", "7", "V", "11", "F", "1", "V", "12", 
//            "E", },
//            // RECTS & TRI, POSITVE, EDGE FLAG
//            new string[] {"C", "10", "10", "5", "4", "3", "0", "0.0833333", "0.416667", "0.125", "0.375", "C", "10", 
//            "20", "6", "7", "3", "11", "0.0833333", "0.416667", "0.166667", "0.333333", "C", "22.5", "20", "8", "10", 
//            "6", "12", "0.125", "0.375", "0.125", "0.375", "C", "25", "10", "8", "13", "5", "11", "0.166667", "0.333333", 
//            "0.15", "0.35", "C", "27.5", "3.46142e-008", "8", "14", "1", "0", "0.25", "0.25", "0.21875", "0.28125", 
//            "C", "32.5", "1.34611e-008", "1", "15", "9", "8", "0.111111", "0.388889", "0.125", "0.375", "C", "35", 
//            "10", "5", "14", "9", "16", "0.142857", "0.357143", "0.166667", "0.333333", "C", "37.5", "20", "6", "13", 
//            "9", "17", "0.2", "0.3", "0.25", "0.25", "C", "50", "10", "5", "17", "2", "1", "0.3", "0.2", "0.125", 
//            "0.375", "C", "50", "20", "6", "18", "2", "19", "0.277778", "0.222222", "0.166667", "0.333333", "B", 
//            "TRI", "F", "0", "V", "15", "F", "1", "V", "11", "V", "0", "F", "0", "V", "11", "V", "15", "V", "14", 
//            "V", "15", "V", "17", "V", "14", "V", "17", "V", "15", "V", "16", "V", "16", "F", "1", "V", "15", "V", 
//            "8", "F", "0", "V", "20", "V", "9", "V", "18", "V", "19", "V", "18", "V", "17", "V", "18", "V", "19", 
//            "V", "20", "V", "1", "V", "17", "F", "1", "V", "16", "F", "0", "V", "17", "F", "1", "V", "1", "F", "0", 
//            "V", "19", "V", "5", "V", "20", "F", "1", "V", "19", "F", "0", "V", "20", "F", "1", "V", "5", "V", "6", 
//            "F", "0", "V", "10", "F", "1", "V", "2", "F", "0", "V", "3", "V", "2", "V", "10", "V", "9", "V", "2", 
//            "V", "9", "F", "1", "V", "20", "F", "0", "V", "13", "V", "9", "V", "10", "V", "9", "V", "13", "V", "18", 
//            "V", "14", "V", "18", "V", "13", "V", "18", "V", "14", "V", "17", "V", "14", "V", "12", "V", "11", "V", 
//            "12", "V", "14", "V", "13", "V", "13", "F", "1", "V", "3", "F", "0", "V", "12", "V", "3", "V", "13", 
//            "V", "10", "V", "11", "F", "1", "V", "7", "V", "4", "F", "0", "V", "7", "V", "11", "F", "1", "V", "12", 
//            "E", },
//            // RECTS & TRI, NEGATIVE, EDGE FLAG
//            new string[] {"C", "10", "10", "5", "4", "3", "0", "0.0833333", "0.416667", "0.125", "0.375", "C", "10", 
//            "20", "6", "7", "3", "11", "0.0833333", "0.416667", "0.166667", "0.333333", "C", "22.5", "20", "8", "10", 
//            "6", "12", "0.125", "0.375", "0.125", "0.375", "C", "25", "10", "8", "13", "5", "11", "0.166667", "0.333333", 
//            "0.15", "0.35", "C", "27.5", "3.46142e-008", "8", "14", "1", "0", "0.25", "0.25", "0.21875", "0.28125", 
//            "C", "32.5", "1.34611e-008", "1", "15", "9", "8", "0.111111", "0.388889", "0.125", "0.375", "C", "35", 
//            "10", "5", "14", "9", "16", "0.142857", "0.357143", "0.166667", "0.333333", "C", "37.5", "20", "6", "13", 
//            "9", "17", "0.2", "0.3", "0.25", "0.25", "C", "50", "10", "5", "17", "2", "1", "0.3", "0.2", "0.125", 
//            "0.375", "C", "50", "20", "6", "18", "2", "19", "0.277778", "0.222222", "0.166667", "0.333333", },
//            // RECTS & TRI, ABS >= 0, EDGE FLAG
//            new string[] {"C", "10", "10", "5", "4", "3", "0", "0.0833333", "0.416667", "0.125", "0.375", "C", "10", 
//            "20", "6", "7", "3", "11", "0.0833333", "0.416667", "0.166667", "0.333333", "C", "22.5", "20", "8", "10", 
//            "6", "12", "0.125", "0.375", "0.125", "0.375", "C", "25", "10", "8", "13", "5", "11", "0.166667", "0.333333", 
//            "0.15", "0.35", "C", "27.5", "3.46142e-008", "8", "14", "1", "0", "0.25", "0.25", "0.21875", "0.28125", 
//            "C", "32.5", "1.34611e-008", "1", "15", "9", "8", "0.111111", "0.388889", "0.125", "0.375", "C", "35", 
//            "10", "5", "14", "9", "16", "0.142857", "0.357143", "0.166667", "0.333333", "C", "37.5", "20", "6", "13", 
//            "9", "17", "0.2", "0.3", "0.25", "0.25", "C", "50", "10", "5", "17", "2", "1", "0.3", "0.2", "0.125", 
//            "0.375", "C", "50", "20", "6", "18", "2", "19", "0.277778", "0.222222", "0.166667", "0.333333", "B", 
//            "TRI", "F", "0", "V", "15", "V", "17", "F", "1", "V", "14", "F", "0", "V", "17", "F", "1", "V", "15", 
//            "V", "16", "F", "0", "V", "19", "V", "18", "F", "1", "V", "17", "F", "0", "V", "18", "F", "1", "V", "19", 
//            "V", "20", "F", "0", "V", "13", "F", "1", "V", "9", "V", "10", "F", "0", "V", "9", "V", "13", "F", "1", 
//            "V", "18", "F", "0", "V", "14", "V", "18", "V", "13", "V", "18", "V", "14", "V", "17", "V", "14", "F", 
//            "1", "V", "12", "V", "11", "F", "0", "V", "12", "V", "14", "F", "1", "V", "13", "E", },
//            // LONG PATH, ODD
//            new string[] {"C", "0", "0", "16", "0", "0", "0", "0.5", "0.5", "0", "0", "C", "40", "40", "2", "3", 
//            "6", "5", "0.125", "0.375", "0.25", "0.25", "C", "50", "40", "2", "18", "10", "9", "0.166667", "0.333333", 
//            "0.25", "0.25", "C", "60", "40", "2", "19", "14", "13", "0.25", "0.25", "0.25", "0.25", "B", "STRIP", 
//            "V", "18", "V", "6", "V", "3", "V", "7", "V", "4", "V", "8", "V", "9", "E", "B", "STRIP", "V", "19", 
//            "V", "20", "V", "10", "V", "14", "V", "11", "V", "15", "V", "12", "V", "17", "V", "1", "E", "B", "FAN", 
//            "V", "1", "V", "2", "V", "20", "V", "13", "V", "12", "E", "B", "FAN", "V", "9", "V", "19", "V", "18", 
//            "V", "5", "V", "4", "E", },
//            // LONG PATH, NON-ZERO
//            new string[] {"C", "0", "0", "16", "0", "0", "0", "0.5", "0.5", "0", "0", "C", "40", "40", "2", "3", 
//            "6", "5", "0.125", "0.375", "0.25", "0.25", "C", "50", "40", "2", "18", "10", "9", "0.166667", "0.333333", 
//            "0.25", "0.25", "C", "60", "40", "2", "19", "14", "13", "0.25", "0.25", "0.25", "0.25", "B", "STRIP", 
//            "V", "15", "V", "17", "V", "12", "V", "1", "V", "13", "V", "20", "V", "19", "V", "10", "V", "6", "V", 
//            "7", "V", "3", "V", "4", "V", "5", "V", "9", "V", "18", "V", "19", "V", "6", "E", "B", "FAN", "V", "11", 
//            "V", "8", "V", "7", "V", "10", "V", "14", "V", "15", "V", "12", "V", "8", "E", "B", "FAN", "V", "9", 
//            "V", "4", "V", "8", "V", "13", "V", "19", "E", "B", "FAN", "V", "3", "V", "5", "V", "18", "V", "6", "E", 
//            "B", "TRI", "V", "8", "V", "4", "V", "7", "V", "8", "V", "12", "V", "13", "V", "20", "V", "1", "V", "2", 
//            "V", "14", "V", "10", "V", "20", "E", },
//            // LONG PATH, POSITIVE
//            new string[] {"C", "0", "0", "16", "0", "0", "0", "0.5", "0.5", "0", "0", "C", "40", "40", "2", "3", 
//            "6", "5", "0.125", "0.375", "0.25", "0.25", "C", "50", "40", "2", "18", "10", "9", "0.166667", "0.333333", 
//            "0.25", "0.25", "C", "60", "40", "2", "19", "14", "13", "0.25", "0.25", "0.25", "0.25", "B", "STRIP", 
//            "V", "15", "V", "17", "V", "12", "V", "1", "V", "13", "V", "20", "V", "19", "V", "10", "V", "6", "V", 
//            "7", "V", "3", "V", "4", "V", "5", "V", "9", "V", "18", "V", "19", "V", "6", "E", "B", "FAN", "V", "11", 
//            "V", "8", "V", "7", "V", "10", "V", "14", "V", "15", "V", "12", "V", "8", "E", "B", "FAN", "V", "9", 
//            "V", "4", "V", "8", "V", "13", "V", "19", "E", "B", "FAN", "V", "3", "V", "5", "V", "18", "V", "6", "E", 
//            "B", "TRI", "V", "8", "V", "4", "V", "7", "V", "8", "V", "12", "V", "13", "V", "20", "V", "1", "V", "2", 
//            "V", "14", "V", "10", "V", "20", "E", },
//            // LONG PATH, NEGATIVE
//            new string[] {"C", "0", "0", "16", "0", "0", "0", "0.5", "0.5", "0", "0", "C", "40", "40", "2", "3", 
//            "6", "5", "0.125", "0.375", "0.25", "0.25", "C", "50", "40", "2", "18", "10", "9", "0.166667", "0.333333", 
//            "0.25", "0.25", "C", "60", "40", "2", "19", "14", "13", "0.25", "0.25", "0.25", "0.25", },
//            // LONG PATH, ABS >= 2
//            new string[] {"C", "0", "0", "16", "0", "0", "0", "0.5", "0.5", "0", "0", "C", "40", "40", "2", "3", 
//            "6", "5", "0.125", "0.375", "0.25", "0.25", "C", "50", "40", "2", "18", "10", "9", "0.166667", "0.333333", 
//            "0.25", "0.25", "C", "60", "40", "2", "19", "14", "13", "0.25", "0.25", "0.25", "0.25", "B", "STRIP", 
//            "V", "10", "V", "11", "V", "7", "V", "8", "V", "4", "V", "9", "V", "5", "V", "18", "V", "3", "V", "6", 
//            "V", "7", "V", "10", "E", "B", "FAN", "V", "3", "V", "7", "V", "4", "V", "5", "E", "B", "FAN", "V", "19", 
//            "V", "10", "V", "6", "V", "18", "V", "9", "V", "13", "V", "20", "E", "B", "FAN", "V", "8", "V", "11", 
//            "V", "12", "V", "13", "V", "9", "E", },
//            // LONG PATH, ODD, EDGE FLAG
//            new string[] {"C", "0", "0", "16", "0", "0", "0", "0.5", "0.5", "0", "0", "C", "40", "40", "2", "3", 
//            "6", "5", "0.125", "0.375", "0.25", "0.25", "C", "50", "40", "2", "18", "10", "9", "0.166667", "0.333333", 
//            "0.25", "0.25", "C", "60", "40", "2", "19", "14", "13", "0.25", "0.25", "0.25", "0.25", "B", "TRI", "F", 
//            "0", "V", "8", "V", "4", "F", "1", "V", "7", "F", "0", "V", "4", "F", "1", "V", "8", "F", "0", "V", "9", 
//            "V", "4", "V", "9", "F", "1", "V", "5", "F", "0", "V", "5", "V", "9", "F", "1", "V", "18", "F", "0", 
//            "V", "18", "F", "1", "V", "9", "V", "19", "F", "0", "V", "20", "F", "1", "V", "10", "V", "19", "F", "0", 
//            "V", "17", "V", "12", "F", "1", "V", "15", "F", "0", "V", "12", "F", "1", "V", "17", "F", "0", "V", "1", 
//            "V", "12", "V", "1", "F", "1", "V", "13", "F", "0", "V", "13", "V", "1", "F", "1", "V", "20", "F", "0", 
//            "V", "20", "F", "1", "V", "1", "V", "2", "F", "0", "V", "15", "V", "11", "F", "1", "V", "14", "F", "0", 
//            "V", "11", "V", "15", "F", "1", "V", "12", "F", "0", "V", "14", "F", "1", "V", "11", "F", "0", "V", "10", 
//            "V", "14", "V", "10", "F", "1", "V", "20", "F", "0", "V", "7", "V", "3", "F", "1", "V", "6", "F", "0", 
//            "V", "3", "V", "7", "F", "1", "V", "4", "F", "0", "V", "6", "F", "1", "V", "3", "V", "18", "E", },
//            // LONG PATH, NON-ZERO, EDGE FLAG
//            new string[] {"C", "0", "0", "16", "0", "0", "0", "0.5", "0.5", "0", "0", "C", "40", "40", "2", "3", 
//            "6", "5", "0.125", "0.375", "0.25", "0.25", "C", "50", "40", "2", "18", "10", "9", "0.166667", "0.333333", 
//            "0.25", "0.25", "C", "60", "40", "2", "19", "14", "13", "0.25", "0.25", "0.25", "0.25", "B", "TRI", "F", 
//            "0", "V", "5", "V", "3", "V", "4", "V", "3", "V", "5", "V", "18", "V", "19", "V", "6", "V", "18", "V", 
//            "8", "V", "4", "V", "7", "V", "4", "V", "8", "V", "9", "V", "4", "V", "9", "V", "5", "V", "5", "V", "9", 
//            "V", "18", "V", "18", "V", "9", "V", "19", "V", "7", "V", "3", "V", "6", "V", "3", "V", "7", "V", "4", 
//            "V", "6", "V", "3", "V", "18", "V", "20", "V", "10", "V", "19", "V", "12", "V", "8", "V", "11", "V", 
//            "8", "V", "12", "V", "13", "V", "8", "V", "13", "V", "9", "V", "9", "V", "13", "V", "19", "V", "19", 
//            "V", "13", "V", "20", "V", "17", "V", "12", "F", "1", "V", "15", "F", "0", "V", "12", "F", "1", "V", 
//            "17", "F", "0", "V", "1", "V", "12", "V", "1", "V", "13", "V", "13", "V", "1", "V", "20", "V", "20", 
//            "F", "1", "V", "1", "V", "2", "F", "0", "V", "15", "V", "11", "F", "1", "V", "14", "F", "0", "V", "11", 
//            "V", "15", "V", "12", "V", "14", "V", "11", "V", "10", "V", "14", "V", "10", "F", "1", "V", "20", "F", 
//            "0", "V", "11", "V", "7", "V", "10", "V", "7", "V", "11", "V", "8", "V", "10", "V", "7", "V", "6", "V", 
//            "10", "V", "6", "V", "19", "E", },
//            // LONG PATH, POSITVE, EDGE FLAG
//            new string[] {"C", "0", "0", "16", "0", "0", "0", "0.5", "0.5", "0", "0", "C", "40", "40", "2", "3", 
//            "6", "5", "0.125", "0.375", "0.25", "0.25", "C", "50", "40", "2", "18", "10", "9", "0.166667", "0.333333", 
//            "0.25", "0.25", "C", "60", "40", "2", "19", "14", "13", "0.25", "0.25", "0.25", "0.25", "B", "TRI", "F", 
//            "0", "V", "5", "V", "3", "V", "4", "V", "3", "V", "5", "V", "18", "V", "19", "V", "6", "V", "18", "V", 
//            "8", "V", "4", "V", "7", "V", "4", "V", "8", "V", "9", "V", "4", "V", "9", "V", "5", "V", "5", "V", "9", 
//            "V", "18", "V", "18", "V", "9", "V", "19", "V", "7", "V", "3", "V", "6", "V", "3", "V", "7", "V", "4", 
//            "V", "6", "V", "3", "V", "18", "V", "20", "V", "10", "V", "19", "V", "12", "V", "8", "V", "11", "V", 
//            "8", "V", "12", "V", "13", "V", "8", "V", "13", "V", "9", "V", "9", "V", "13", "V", "19", "V", "19", 
//            "V", "13", "V", "20", "V", "17", "V", "12", "F", "1", "V", "15", "F", "0", "V", "12", "F", "1", "V", 
//            "17", "F", "0", "V", "1", "V", "12", "V", "1", "V", "13", "V", "13", "V", "1", "V", "20", "V", "20", 
//            "F", "1", "V", "1", "V", "2", "F", "0", "V", "15", "V", "11", "F", "1", "V", "14", "F", "0", "V", "11", 
//            "V", "15", "V", "12", "V", "14", "V", "11", "V", "10", "V", "14", "V", "10", "F", "1", "V", "20", "F", 
//            "0", "V", "11", "V", "7", "V", "10", "V", "7", "V", "11", "V", "8", "V", "10", "V", "7", "V", "6", "V", 
//            "10", "V", "6", "V", "19", "E", },
//            // LONG PATH, NEGATIVE, EDGE FLAG
//            new string[] {"C", "0", "0", "16", "0", "0", "0", "0.5", "0.5", "0", "0", "C", "40", "40", "2", "3", 
//            "6", "5", "0.125", "0.375", "0.25", "0.25", "C", "50", "40", "2", "18", "10", "9", "0.166667", "0.333333", 
//            "0.25", "0.25", "C", "60", "40", "2", "19", "14", "13", "0.25", "0.25", "0.25", "0.25", },
//            // LONG PATH, ABS >= 2, EDGE FLAG
//            new string[] {"C", "0", "0", "16", "0", "0", "0", "0.5", "0.5", "0", "0", "C", "40", "40", "2", "3", 
//            "6", "5", "0.125", "0.375", "0.25", "0.25", "C", "50", "40", "2", "18", "10", "9", "0.166667", "0.333333", 
//            "0.25", "0.25", "C", "60", "40", "2", "19", "14", "13", "0.25", "0.25", "0.25", "0.25", "B", "TRI", "F", 
//            "0", "V", "5", "V", "3", "V", "4", "V", "3", "V", "5", "V", "18", "V", "19", "V", "6", "V", "18", "V", 
//            "8", "V", "4", "V", "7", "V", "4", "V", "8", "V", "9", "V", "4", "V", "9", "V", "5", "V", "5", "V", "9", 
//            "V", "18", "V", "18", "V", "9", "V", "19", "V", "12", "V", "8", "F", "1", "V", "11", "F", "0", "V", "8", 
//            "F", "1", "V", "12", "F", "0", "V", "13", "V", "8", "V", "13", "V", "9", "V", "9", "V", "13", "V", "19", 
//            "V", "19", "F", "1", "V", "13", "V", "20", "F", "0", "V", "11", "V", "7", "F", "1", "V", "10", "F", "0", 
//            "V", "7", "V", "11", "V", "8", "V", "10", "V", "7", "V", "6", "V", "10", "V", "6", "F", "1", "V", "19", 
//            "F", "0", "V", "7", "V", "3", "V", "6", "V", "3", "V", "7", "V", "4", "V", "6", "V", "3", "V", "18", 
//            "E", },
//        };

//        string GetNextOutputAsString()
//        {
//            m_LastString = m_TestOutput[m_CurrentOutputTest][m_CurrentOutput];
//            m_CurrentOutput++;
//            return m_LastString;
//        }

//        double GetNextOutputAsDouble()
//        {
//            return Convert.ToDouble(GetNextOutputAsString());
//        }

//        int GetNextOutputAsInt()
//        {
//            double asDouble = Convert.ToDouble(GetNextOutputAsString());
//            Assert.AreEqual((int)asDouble, asDouble);
//            return (int)asDouble;
//        }

//        bool GetNextOutputAsBool()
//        {
//            double asDouble = Convert.ToDouble(GetNextOutputAsString());
//            if (asDouble == 1)
//            {
//                return true;
//            }

//            Assert.AreEqual(asDouble, 0);

//            return false;
//        }

//        public void BeginCallBack(Tesselator<T>.TriangleListType type)
//        {
//            Assert.IsTrue(GetNextOutputAsString() == "B");
//            switch (type)
//            {
//                case Tesselator.TriangleListType.Triangles:
//                    Assert.IsTrue(GetNextOutputAsString() == "TRI");
//                    break;

//                case Tesselator.TriangleListType.TriangleFan:
//                    Assert.IsTrue(GetNextOutputAsString() == "FAN");
//                    break;

//                case Tesselator.TriangleListType.TriangleStrip:
//                    Assert.IsTrue(GetNextOutputAsString() == "STRIP");
//                    break;

//                default:
//                    throw new Exception("unknown TriangleListType '" + type.ToString() + "'.");
//            }
//        }

//        public void EndCallBack()
//        {
//            Assert.IsTrue(GetNextOutputAsString() == "E");
//        }

//        public void VertexCallBack(int index)
//        {
//            Assert.IsTrue(GetNextOutputAsString() == "V");
//            Assert.AreEqual(GetNextOutputAsInt(), index);
//        }

//        public void EdgeFlagCallBack(bool IsEdge)
//        {
//            Assert.IsTrue(GetNextOutputAsString() == "F");
//            Assert.AreEqual(GetNextOutputAsBool(), IsEdge);
//        }

//        public void CombineCallBack(double[] coords3, int[] data4,
//            double[] weight4, out int outData)
//        {
//            double error = .001;
//            Assert.IsTrue(GetNextOutputAsString() == "C");
//            Assert.AreEqual(GetNextOutputAsDouble(), coords3[0], error);
//            Assert.AreEqual(GetNextOutputAsDouble(), coords3[1], error);
//            Assert.AreEqual(GetNextOutputAsInt(), data4[0]);
//            Assert.AreEqual(GetNextOutputAsInt(), data4[1]);
//            Assert.AreEqual(GetNextOutputAsInt(), data4[2]);
//            Assert.AreEqual(GetNextOutputAsInt(), data4[3]);
//            Assert.AreEqual(GetNextOutputAsDouble(), weight4[0], error);
//            Assert.AreEqual(GetNextOutputAsDouble(), weight4[1], error);
//            Assert.AreEqual(GetNextOutputAsDouble(), weight4[2], error);
//            Assert.AreEqual(GetNextOutputAsDouble(), weight4[3], error);

//            outData = m_VertexList.Count;
//            m_VertexList.Add(new Vertex(coords3[0], coords3[1]));
//        }

//        public void ParseStreamForTesselator(Tesselator<T> tesselator, int instructionStreamIndex)
//        {
//            m_VertexList.Clear();
//            m_CurrentOutput = 0;

//            string[] instructionStream = m_InsructionStream[instructionStreamIndex];
//            for (int curInstruction = 0; curInstruction < instructionStream.Length; curInstruction++)
//            {
//                switch (instructionStream[curInstruction])
//                {
//                    case "BP":
//                        tesselator.BeginPolygon();
//                        break;

//                    case "BC":
//                        tesselator.BeginContour();
//                        break;

//                    case "V":
//                        double x = Convert.ToDouble(instructionStream[curInstruction + 1]);
//                        double y = Convert.ToDouble(instructionStream[curInstruction + 2]);
//                        curInstruction += 2;
//                        double[] coords = new double[3];
//                        coords[0] = x;
//                        coords[1] = y;
//                        tesselator.AddVertex(coords, m_VertexList.Count);
//                        m_VertexList.Add(new Vertex(x, y));
//                        break;

//                    case "EC":
//                        tesselator.EndContour();
//                        break;

//                    case "EP":
//                        tesselator.EndPolygon();
//                        break;

//                    default:
//                        throw new Exception();
//                }
//            }
//        }

//        void RunTest(int instructionStreamIndex, Tesselator<T>.WindingRuleType windingRule, bool setEdgeFlag)
//        {
//            Tesselate.Tesselator tesselator = new Tesselate.Tesselator();
//            tesselator.callBegin += new Tesselate.Tesselator.CallBeginDelegate(BeginCallBack);
//            tesselator.callEnd += new Tesselate.Tesselator.CallEndDelegate(EndCallBack);
//            tesselator.callVertex += new Tesselate.Tesselator.CallVertexDelegate(VertexCallBack);
//            tesselator.callCombine += new Tesselate.Tesselator.CallCombineDelegate(CombineCallBack);

//            tesselator.windingRule = windingRule;
//            if (setEdgeFlag)
//            {
//                tesselator.callEdgeFlag += new Tesselate.Tesselator.CallEdgeFlagDelegate(EdgeFlagCallBack);
//            }

//            ParseStreamForTesselator(tesselator, instructionStreamIndex);
//        }

//        [Test]
//        public void MatchesGLUTesselator()
//        {
//            for (m_CurrentInputTest = 0; m_CurrentInputTest < m_InsructionStream.Length; m_CurrentInputTest++)
//            {
//                RunTest(m_CurrentInputTest, Tesselator.WindingRuleType.Odd, false);
//                m_CurrentOutputTest++;
//                RunTest(m_CurrentInputTest, Tesselator.WindingRuleType.NonZero, false);
//                m_CurrentOutputTest++;
//                RunTest(m_CurrentInputTest, Tesselator.WindingRuleType.Positive, false);
//                m_CurrentOutputTest++;
//                RunTest(m_CurrentInputTest, Tesselator.WindingRuleType.Negative, false);
//                m_CurrentOutputTest++;
//                RunTest(m_CurrentInputTest, Tesselator.WindingRuleType.ABS_GEQ_Two, false);
//                m_CurrentOutputTest++;

//                RunTest(m_CurrentInputTest, Tesselator.WindingRuleType.Odd, true);
//                m_CurrentOutputTest++;
//                RunTest(m_CurrentInputTest, Tesselator.WindingRuleType.NonZero, true);
//                m_CurrentOutputTest++;
//                RunTest(m_CurrentInputTest, Tesselator.WindingRuleType.Positive, true);
//                m_CurrentOutputTest++;
//                RunTest(m_CurrentInputTest, Tesselator.WindingRuleType.Negative, true);
//                m_CurrentOutputTest++;
//                RunTest(m_CurrentInputTest, Tesselator.WindingRuleType.ABS_GEQ_Two, true);
//                m_CurrentOutputTest++;
//            }
//        }
//    }
//}