using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Geometries;

namespace SharpMap.Indexing.QuadTree
{
    public class QuadTreeNode<TValue> : SpatialIndexNode<QuadTreeNode<TValue>, TValue>
    {
        private TValue _value;

        public TValue Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public bool IsLeaf
        {
            get { return Items.Count == 0; }
        }

        public virtual IEnumerable<TValue> Search(BoundingBox searchBounds)
        {
            if (BoundingBox == BoundingBox.Empty)
            {
                yield break;
            }

            foreach (QuadTreeNode<TValue> node in Items)
            {
                if (node.BoundingBox.Intersects(searchBounds))
                {
                    if (node.IsLeaf)
                    {
                        yield return node.Value;
                    }
                    else
                    {
                        node.Search(searchBounds);
                    }
                }
            }
        }

        public virtual IEnumerable<TValue> Search(Geometry geometry)
        {
            throw new NotImplementedException();
        }

        protected override BoundingBox GetItemBoundingBox(QuadTreeNode<TValue> item)
        {
            return item.BoundingBox;
        }
    }
}
