using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Geometries;

namespace SharpMap.Indexing.QuadTree
{
    public class QuadTree<TValue> : QuadTreeNode<TValue>, ISearchableSpatialIndex<TValue>
    {
    }
}