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
using GeoAPI.Geometries;

namespace SharpMap.Indexing.RTree
{
    /// <summary>
    /// Implements a common, well accepted R-Tree insertion strategy.
    /// </summary>
    /// <typeparam name="TValue">The type of the value used in the entries.</typeparam>
    /// <remarks>
    /// This strategy follows Antonin Guttman's findings in his paper 
    /// entitled "R-Trees: A Dynamic Index Structure for Spatial Searching", 
    /// Proc. 1984 ACM SIGMOD International Conference on Management of Data, pp. 47-57.
    /// </remarks>
    public class GuttmanQuadraticInsert<TValue> : IEntryInsertStrategy<RTreeIndexEntry<TValue>>
    {
        #region IEntryInsertStrategy Members

        public void InsertEntry(RTreeIndexEntry<TValue> entry, ISpatialIndexNode node, 
            INodeSplitStrategy nodeSplitStrategy, IndexBalanceHeuristic heuristic, 
            out ISpatialIndexNode newSiblingFromSplit)
        {
            newSiblingFromSplit = null;

            if (node is RTreeLeafNode<TValue>)
            {
                RTreeLeafNode<TValue> leaf = node as RTreeLeafNode<TValue>;
                
                leaf.Add(entry);

                if (leaf.Items.Count > heuristic.NodeItemMaximumCount)
                {
                    newSiblingFromSplit = nodeSplitStrategy.SplitNode(node, heuristic);
                }
            }
            else
            {
                RTreeBranchNode<TValue> branch = node as RTreeBranchNode<TValue>;

                Double leastExpandedArea = Double.PositiveInfinity;

                ISpatialIndexNode leastExpandedChild = null;

                foreach (ISpatialIndexNode child in branch.Items)
                {
                    BoundingBox candidateRegion = BoundingBox.Join(child.BoundingBox, entry.BoundingBox);
                    Double expandedArea = candidateRegion.GetArea() - child.BoundingBox.GetArea();

                    if (expandedArea < leastExpandedArea)
                    {
                        leastExpandedChild = child;
                        leastExpandedArea = expandedArea;
                    }
                    else if (leastExpandedChild == null 
                        || (expandedArea == leastExpandedArea 
                                && child.BoundingBox.GetArea() < leastExpandedChild.BoundingBox.GetArea()))
                    {
                        leastExpandedChild = child;
                    }
                }

                // Found least expanded child node - insert into it
                InsertEntry(entry, leastExpandedChild, nodeSplitStrategy, heuristic, out newSiblingFromSplit);

                // Adjust this node...
                branch.BoundingBox = BoundingBox.Join(leastExpandedChild.BoundingBox, node.BoundingBox);
                
                // Check for overflow and add to current node if it occured
                if (newSiblingFromSplit != null)
                {
                    // Add new sibling node to the current node
                    RTreeBranchNode<TValue> indexNode = node as RTreeBranchNode<TValue>;
                    indexNode.Add(newSiblingFromSplit);
                    newSiblingFromSplit = null;

                    // Split the current node, since the child count is too high, and return the split to the caller
                    if (indexNode.Items.Count > heuristic.NodeItemMaximumCount)
                    {
                        newSiblingFromSplit = nodeSplitStrategy.SplitNode(indexNode, heuristic);
                    }
                }
            }
        }
        #endregion
    }
}
