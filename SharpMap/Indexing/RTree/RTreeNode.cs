using System;
using System.Collections.Generic;
using GeoAPI.DataStructures;
using GeoAPI.Geometries;
using GeoAPI.Indexing;

namespace SharpMap.Indexing.RTree
{
    public class RTreeNode<TItem> : SpatialIndexNode<TItem>
        where TItem : IBoundable<IExtents>
    {
        private List<RTreeNode<TItem>> _children;

        protected internal RTreeNode(ISpatialIndex<IExtents, TItem> index, IExtents emptyBounds)
        {
            Index = index;
            Bounds = emptyBounds;
        }

        public override String ToString()
        {
            return String.Format("[{0}] IsLeaf: {1}; Bounds: {2}; " +
                                 "SubNodes Count: {3}; Item Count: {4}",
                                 GetType(),
                                 IsLeaf,
                                 Bounds,
                                 SubNodeCount,
                                 ItemCount);
        }

        public override Boolean IsLeaf
        {
            get
            {
                return _children == null || _children.Count == 0;
            }
        }

        protected override void addSubNode(ISpatialIndexNode<IExtents, TItem> child)
        {
            if (child == null) throw new ArgumentNullException("child");

            RTreeNode<TItem> node = child as RTreeNode<TItem>;

            if (node == null)
            {
                throw new ArgumentException("Parameter must be of type RTreeNode<TItem>.");
            }

            ensureChildren();

            _children.Add(node);

            if (Bounds == null)
            {
                Bounds = node.Bounds.Clone() as IExtents;
            }
            else
            {
                Bounds.ExpandToInclude(child.Bounds);
            }
        }

        protected override void addSubNodes(IEnumerable<ISpatialIndexNode<IExtents, TItem>> children)
        {
            IEnumerable<RTreeNode<TItem>> nodes =
                Caster.Downcast<RTreeNode<TItem>, ISpatialIndexNode<IExtents, TItem>>(children);

            ensureChildren();

            _children.AddRange(nodes);

            foreach (RTreeNode<TItem> child in nodes)
            {
                if (Bounds == null)
                {
                    Bounds = child.Bounds.Clone() as IExtents;
                }
                else
                {
                    Bounds.ExpandToInclude(child.Bounds);
                }
            }
        }

        public override IEnumerable<ISpatialIndexNode<IExtents, TItem>> SubNodes
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

        protected override Boolean removeSubNode(ISpatialIndexNode<IExtents, TItem> child)
        {
            if (child == null) throw new ArgumentNullException("child");

            RTreeNode<TItem> node = child as RTreeNode<TItem>;

            if (node == null)
            {
                throw new ArgumentException("Parameter must be of type RTreeNode<TItem>.");
            }

            ensureChildren();

            return _children.Remove(node);
        }

        public override Int32 SubNodeCount
        {
            get { return _children == null ? 0 : _children.Count; }
        }

        private void ensureChildren()
        {
            if (_children == null)
            {
                _children = new List<RTreeNode<TItem>>();
            }
        }
    }
}
