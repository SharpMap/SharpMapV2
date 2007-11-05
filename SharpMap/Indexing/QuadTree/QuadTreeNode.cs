// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

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
