using System;
using System.Collections.Generic;
using GeoAPI.Geometries;
using GeoAPI.Indexing;
using GeoAPI.Utilities;

namespace SharpMap.Indexing.RTree
{
    public class RTreeNode<TItem> : SpatialIndexNode<TItem>
    {
        public List<RTreeNode<TItem>> _children;

        protected internal RTreeNode(ISpatialIndex<IExtents, TItem> index, Func<TItem, IExtents> bounder)
            : base(bounder)
        {
            Index = index;
        }

        public override String ToString()
        {
            return String.Format("[{0}] IsLeaf: {1}; Bounds: {2}; Children Count: {3}; Item Count: {4}",
                                 GetType(), IsLeaf, Bounds, _children == null ? 0 : _children.Count, Items.Count);
        }

        public override Boolean IsLeaf
        {
            get
            {
                return _children == null || _children.Count == 0;
            }
        }

        public override void AddChild(ISpatialIndexNode<IExtents, TItem> child)
        {
            if (!(child is RTreeNode<TItem>))
            {
                throw new ArgumentException("Parameter must be of type RTreeNode<TItem>.");
            }

            _children.Add(child as RTreeNode<TItem>);
        }

        public override void AddChildren(IEnumerable<ISpatialIndexNode<IExtents, TItem>> children)
        {
            IEnumerable<RTreeNode<TItem>> nodes =
                Enumerable.Downcast<RTreeNode<TItem>, ISpatialIndexNode<IExtents, TItem>>(children);

            _children.AddRange(nodes);
        }

        public override IEnumerable<ISpatialIndexNode<IExtents, TItem>> Children
        {
            get
            {
                if (_children == null)
                {
                    yield break;
                }

                foreach (RTreeNode<TItem> child in _children)
                {
                    yield return child;
                }
            }
        }

        public override Boolean RemoveChild(ISpatialIndexNode<IExtents, TItem> child)
        {
            if (!(child is RTreeNode<TItem>))
            {
                throw new ArgumentException("Parameter must be of type RTreeNode<TItem>.");
            }

            return _children.Remove(child as RTreeNode<TItem>);
        }
    }
}
