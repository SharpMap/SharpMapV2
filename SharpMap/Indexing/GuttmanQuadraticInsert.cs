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

namespace SharpMap.Indexing
{
    internal class GuttmanQuadraticInsert : IEntryInsertStrategy
    {
        #region IEntryInsertStrategy Members

        public void InsertEntry(RTreeIndexEntry entry, Node node, INodeSplitStrategy nodeSplitStrategy, DynamicRTreeHeuristic heuristic, out Node newSiblingFromSplit)
        {
            newSiblingFromSplit = null;

            if (node is LeafNode)
            {
                LeafNode leaf = node as LeafNode;
                leaf.Add(entry);
                if (leaf.Entries.Count > heuristic.NodeItemMaximumCount)
                    newSiblingFromSplit = nodeSplitStrategy.SplitNode(node, heuristic);
            }
            else
            {
                double leastExpandedArea = Double.PositiveInfinity;
                Node leastExpandedChild = null;
                foreach(Node child in (node as IndexNode).Children)
                {
                    BoundingBox candidateRegion = BoundingBox.Join(child.Box, entry.Box);
                    double expandedArea = candidateRegion.GetArea() - child.Box.GetArea();
                    if (expandedArea < leastExpandedArea)
                    {
                        leastExpandedChild = child;
                        leastExpandedArea = expandedArea;
                    }
                    else if (leastExpandedChild == null || (expandedArea == leastExpandedArea && child.Box.GetArea() < leastExpandedChild.Box.GetArea()))
                    {
                        leastExpandedChild = child;
                    }
                }

                // Found least expanded child node - insert into it
                InsertEntry(entry, leastExpandedChild, nodeSplitStrategy, heuristic, out newSiblingFromSplit);

                // Adjust this node...
                node.Box = BoundingBox.Join(leastExpandedChild.Box, node.Box);
                
                // Check for overflow and add to current node if it occured
                if (newSiblingFromSplit != null)
                {
                    // Add new sibling node to the current node
                    IndexNode indexNode = node as IndexNode;
                    indexNode.Add(newSiblingFromSplit);
                    newSiblingFromSplit = null;

                    // Split the current node, since the child count is too high, and return the split to the caller
                    if (indexNode.Children.Count > heuristic.NodeItemMaximumCount)
                        newSiblingFromSplit = nodeSplitStrategy.SplitNode(indexNode, heuristic);
                }
            }
        }
        #endregion
    }
}
