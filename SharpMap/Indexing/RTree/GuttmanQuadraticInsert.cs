// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using System.Diagnostics;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using GeoAPI.Indexing;

namespace SharpMap.Indexing.RTree
{
    /// <summary>
    /// Implements a common, well known R-Tree insertion strategy.
    /// </summary>
    /// <typeparam name="TItem">The type of items indexed.</typeparam>
    /// <remarks>
    /// This strategy follows Antonin Guttman's findings in his paper 
    /// entitled "R-Trees: A Dynamic Index Structure for Spatial Searching", 
    /// Proc. 1984 ACM SIGMOD International Conference on Management of Data, pp. 47-57.
    /// </remarks>
    public class GuttmanQuadraticInsert<TItem> : IItemInsertStrategy<IExtents, TItem>
        where TItem : IBoundable<IExtents>
    {
        private IGeometryFactory _geoFactory;

        public GuttmanQuadraticInsert(IGeometryFactory geoFactory)
        {
            _geoFactory = geoFactory;
        }

        #region IItemInsertStrategy Members

        //private Int32 _tempSplitCount = 0;
        public void Insert(IExtents bounds, 
                           TItem entry, 
                           ISpatialIndexNode<IExtents, TItem> node, 
                           INodeSplitStrategy<IExtents, TItem> nodeSplitStrategy, 
                           IndexBalanceHeuristic heuristic, 
                           out ISpatialIndexNode<IExtents, TItem> newSiblingFromSplit)
        {
            newSiblingFromSplit = null;

            // Terminating case
            if (node.IsLeaf)
            {
                node.AddItem(entry);

                // Handle node overflow
                if (node.ItemCount > heuristic.NodeItemMaximumCount)
                {
                    // Split the node using the given strategy
                    newSiblingFromSplit = nodeSplitStrategy.SplitNode(node, heuristic);

                    //if (++_tempSplitCount % 100 == 0)
                    //{
                    //    Debug.Print("Node split # {0}", _tempSplitCount);
                    //}
                }
            }
            else
            {
                // NOTE: Descending the tree recursively here
                // can make for a very expensive build of a tree, 
                // even for moderate amounts of data.

                Double leastExpandedArea = Double.MaxValue;
                Double leastExpandedChildArea = Double.MaxValue;

                ComputationExtents currentBounds = new ComputationExtents(bounds);

                ISpatialIndexNode<IExtents, TItem> leastExpandedChild;
                leastExpandedChild = findLeastExpandedChild(node.Children, 
                                                            currentBounds, 
                                                            leastExpandedArea, 
                                                            leastExpandedChildArea);

                Debug.Assert(leastExpandedChild != null);

                // Found least expanded child node - insert into it
                Insert(bounds, entry, leastExpandedChild, nodeSplitStrategy, 
                    heuristic, out newSiblingFromSplit);

                RTreeNode<TItem> rNode = node as RTreeNode<TItem>;

                // Adjust this node...
                rNode.Bounds.ExpandToInclude(bounds);
                
                // Check for overflow and add to current node if it occured
                if (newSiblingFromSplit != null)
                {
                    // Add new sibling node to the current node
                    node.AddChild(newSiblingFromSplit);
                    newSiblingFromSplit = null;

                    // Split the current node, since the child count is too high, 
                    // and return the split to the caller
                    if (node.ItemCount > heuristic.NodeItemMaximumCount)
                    {
                        newSiblingFromSplit = nodeSplitStrategy.SplitNode(node, heuristic);
                    }
                }
            }
        }

        private static ISpatialIndexNode<IExtents, TItem> findLeastExpandedChild(
            IEnumerable<ISpatialIndexNode<IExtents, TItem>> children, 
            ComputationExtents currentBounds, Double leastExpandedArea, 
            Double leastExpandedChildArea) 
        {
            ISpatialIndexNode<IExtents, TItem> leastExpandedChild = null;

            foreach (ISpatialIndexNode<IExtents, TItem> child in children)
            {
                ComputationExtents childBounds = new ComputationExtents(child.Bounds);
                ComputationExtents candidateRegion = childBounds.Union(currentBounds);
                   
                Double candidateRegionArea = candidateRegion.Area;
                Double childArea = childBounds.Area;
                Double expandedArea = candidateRegionArea - childArea;

                if (expandedArea < leastExpandedArea)
                {
                    leastExpandedChild = child;
                    leastExpandedChildArea = childArea;
                    leastExpandedArea = expandedArea;
                }
                else if (expandedArea == leastExpandedArea 
                         && childArea < leastExpandedChildArea)
                {
                    leastExpandedChild = child;
                    leastExpandedChildArea = childArea;
                }
            }

            return leastExpandedChild;
        }

        #endregion

    }
}
