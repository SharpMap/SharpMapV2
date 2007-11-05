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

namespace SharpMap.Indexing.RTree
{
    /// <summary>
    /// Implements a common, well accepted R-Tree node splitting strategy.
    /// </summary>
    /// <typeparam name="TValue">The type of the value used in the entries.</typeparam>
    /// <remarks>
    /// This strategy follows Antonin Guttman's findings in his paper 
    /// entitled "R-Trees: A Dynamic Index Structure for Spatial Searching", 
    /// Proc. 1984 ACM SIGMOD International Conference on Management of Data, pp. 47-57.
    /// </remarks>
    public class GuttmanQuadraticSplit<TValue> : INodeSplitStrategy
    {
        #region INodeSplitStrategy Members

        public ISpatialIndexNode SplitNode(ISpatialIndexNode node, IndexBalanceHeuristic heuristic)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (node is RTreeLeafNode<TValue>)
            {
                return splitLeafNode(node as RTreeLeafNode<TValue>, heuristic as DynamicRTreeBalanceHeuristic);
            }
            else if (node is RTreeBranchNode<TValue>)
            {
                return splitBranchNode(node as RTreeBranchNode<TValue>, heuristic as DynamicRTreeBalanceHeuristic);
            }

            throw new ArgumentException("Invalid node type. Must be a LeafNode or an IndexNode.", "node");
        }

        #endregion

        private RTreeLeafNode<TValue> splitLeafNode(RTreeLeafNode<TValue> node, DynamicRTreeBalanceHeuristic heuristic)
        {
            List<RTreeIndexEntry<TValue>> entries = new List<RTreeIndexEntry<TValue>>(node.Items);
            List<RTreeIndexEntry<TValue>> group1 = new List<RTreeIndexEntry<TValue>>();
            List<RTreeIndexEntry<TValue>> group2 = new List<RTreeIndexEntry<TValue>>();
            
            pickSeeds(entries, group1, group2);
            
            distribute(entries, group1, group2, heuristic);

            if (entries.Count > 0)
            {
                if (group1.Count < heuristic.NodeItemMinimumCount)
                {
                    group1.AddRange(entries);
                }
                if (group2.Count < heuristic.NodeItemMinimumCount)
                {
                    group2.AddRange(entries);
                }
            }

            node.Clear();
            node.AddRange(group1);
            RTreeLeafNode<TValue> sibling = (node.Index as RTree<TValue>).CreateLeafNode();
            sibling.AddRange(group2);
            return sibling;
        }

        private RTreeBranchNode<TValue> splitBranchNode(RTreeBranchNode<TValue> indexNode, IndexBalanceHeuristic heuristic)
        {
            List<ISpatialIndexNode> children = new List<ISpatialIndexNode>(indexNode.Items);
            List<ISpatialIndexNode> group1 = new List<ISpatialIndexNode>();
            List<ISpatialIndexNode> group2 = new List<ISpatialIndexNode>();

            pickSeeds(children, group1, group2);
            distribute(children, group1, group2, heuristic);

            if (children.Count > 0)
            {
                if (group1.Count < heuristic.NodeItemMinimumCount)
                {
                    group1.AddRange(children);
                }
                if (group2.Count < heuristic.NodeItemMinimumCount)
                {
                    group2.AddRange(children);
                }
            }

            indexNode.Clear();
            indexNode.AddRange(group1);
            RTreeBranchNode<TValue> sibling = (indexNode.Index as RTree<TValue>).CreateBranchNode();
            sibling.AddRange(group2);
            return sibling;
        }

        private void pickSeeds(IList<RTreeIndexEntry<TValue>> entries, List<RTreeIndexEntry<TValue>> group1, List<RTreeIndexEntry<TValue>> group2)
        {
            double largestWaste = Double.MinValue;

            foreach (RTreeIndexEntry<TValue> entry1 in entries)
            {
                foreach (RTreeIndexEntry<TValue> entry2 in entries)
                {
                    if (entry1.Value.Equals(entry2.Value))
                    {
                        continue;
                    }

                    BoundingBox minBoundingRectangle = new BoundingBox(entry1.BoundingBox, entry2.BoundingBox);
                    double waste = minBoundingRectangle.GetArea() - entry1.BoundingBox.GetArea() - entry2.BoundingBox.GetArea() + entry1.BoundingBox.GetIntersectingArea(entry2.BoundingBox);

                    if (group1.Count == 0 && group2.Count == 0)
                    {
                        group1.Add(entry1);
                        group2.Add(entry2);
                        largestWaste = waste;
                        continue;
                    }

                    if (waste > largestWaste)
                    {
                        group1[0] = entry1;
                        group2[0] = entry2;
                        largestWaste = waste;
                    }
                }
            }

            entries.Remove(group1[0]);
            entries.Remove(group2[0]);
        }

        private void pickSeeds(IList<ISpatialIndexNode> nodes, List<ISpatialIndexNode> group1, List<ISpatialIndexNode> group2)
        {
            double largestWaste = Double.MinValue;

            // Here's why it's known as a quadratic algorithm - O(n^2)
            foreach (ISpatialIndexNode node1 in nodes)
            {
                foreach (ISpatialIndexNode node2 in nodes)
                {
                    if (node1.NodeId == node2.NodeId)
                    {
                        continue;
                    }

                    BoundingBox minBoundingRectangle = new BoundingBox(node1.BoundingBox, node2.BoundingBox);
                    double waste = minBoundingRectangle.GetArea() - node1.BoundingBox.GetArea() - node2.BoundingBox.GetArea() + node1.BoundingBox.GetIntersectingArea(node2.BoundingBox);

                    if (group1.Count == 0 && group2.Count == 0)
                    {
                        group1.Add(node1);
                        group2.Add(node2);
                        largestWaste = waste;
                        continue;
                    }

                    if (waste > largestWaste)
                    {
                        group1[0] = node1;
                        group2[0] = node2;
                        largestWaste = waste;
                    }
                }
            }

            nodes.Remove(group1[0]);
            nodes.Remove(group2[0]);
        }

        enum GroupBoxLeastEnlarged
        {
            Tie = 0,
            Group1,
            Group2
        }

        private void distribute(IList<RTreeIndexEntry<TValue>> entries, List<RTreeIndexEntry<TValue>> group1, List<RTreeIndexEntry<TValue>> group2, IndexBalanceHeuristic heuristic)
        {
            if (entries.Count == 0)
            {
                return;
            }

            if (group1.Count == heuristic.TargetNodeCount || group2.Count == heuristic.TargetNodeCount)
            {
                return;
            }

            RTreeIndexEntry<TValue> entry;
            BoundingBox group1Box = computeBox(group1);
            BoundingBox group2Box = computeBox(group2);
            GroupBoxLeastEnlarged group = pickNext(entries, group1Box, group2Box, out entry);

            switch (group)
            {
                case GroupBoxLeastEnlarged.Group1:
                    group1.Add(entry);
                    break;

                case GroupBoxLeastEnlarged.Group2:
                    group2.Add(entry);
                    break;

                case GroupBoxLeastEnlarged.Tie:
                default:
                    if (group1Box.GetArea() < group2Box.GetArea())
                    {
                        group1.Add(entry);
                    }
                    else if (group2Box.GetArea() < group1Box.GetArea())
                    {
                        group2.Add(entry);
                    }
                    else if (group1.Count < group2.Count)
                    {
                        group1.Add(entry);
                    }
                    else if (group2.Count < group1.Count)
                    {
                        group2.Add(entry);
                    }
                    else if ((DateTime.Now.Ticks / 10) % 2 == 0)
                    {
                        group1.Add(entry);
                    }
                    else
                    {
                        group2.Add(entry);
                    }

                    break;
            }

            distribute(entries, group1, group2, heuristic);
        }

        private void distribute(IList<ISpatialIndexNode> nodes, List<ISpatialIndexNode> group1, List<ISpatialIndexNode> group2, IndexBalanceHeuristic heuristic)
        {
            if (nodes.Count == 0)
            {
                return;
            }

            if (group1.Count == heuristic.TargetNodeCount || group2.Count == heuristic.TargetNodeCount)
            {
                return;
            }

            ISpatialIndexNode node;
            BoundingBox group1Box = computeBox(group1);
            BoundingBox group2Box = computeBox(group2);
            GroupBoxLeastEnlarged group = pickNext(nodes, group1Box, group2Box, out node);

            switch (group)
            {
                case GroupBoxLeastEnlarged.Group1:
                    group1.Add(node);
                    break;

                case GroupBoxLeastEnlarged.Group2:
                    group2.Add(node);
                    break;

                case GroupBoxLeastEnlarged.Tie:
                default:
                    if (group1Box.GetArea() < group2Box.GetArea())
                    {
                        group1.Add(node);
                    }
                    else if (group2Box.GetArea() < group1Box.GetArea())
                    {
                        group2.Add(node);
                    }
                    else if (group1.Count < group2.Count)
                    {
                        group1.Add(node);
                    }
                    else if (group2.Count < group1.Count)
                    {
                        group2.Add(node);
                    }
                    else if ((DateTime.Now.Ticks / 10) % 2 == 0)
                    {
                        group1.Add(node);
                    }
                    else
                    {
                        group2.Add(node);
                    }

                    break;
            }

            distribute(nodes, group1, group2, heuristic);
        }

        private GroupBoxLeastEnlarged pickNext(IList<RTreeIndexEntry<TValue>> entries, BoundingBox group1Box, BoundingBox group2Box, out RTreeIndexEntry<TValue> entry)
        {
            double maxArealDifference = Double.MinValue;
            GroupBoxLeastEnlarged group = GroupBoxLeastEnlarged.Tie;
            RTreeIndexEntry<TValue>? nextEntry = null;

            foreach (RTreeIndexEntry<TValue> e in entries)
            {
                double arealDifferenceGroup1 = Math.Abs(BoundingBox.Join(group1Box, e.BoundingBox).GetArea() - group1Box.GetArea());
                double arealDifferenceGroup2 = Math.Abs(BoundingBox.Join(group2Box, e.BoundingBox).GetArea() - group2Box.GetArea());
                
                if (Math.Abs(arealDifferenceGroup1 - arealDifferenceGroup2) > maxArealDifference)
                {
                    maxArealDifference = Math.Abs(arealDifferenceGroup1 - arealDifferenceGroup2);
                    nextEntry = e;

                    if (arealDifferenceGroup1 < arealDifferenceGroup2)
                    {
                        group = GroupBoxLeastEnlarged.Group1;
                    }
                    else if (arealDifferenceGroup2 < arealDifferenceGroup1)
                    {
                        group = GroupBoxLeastEnlarged.Group2;
                    }
                }
            }

            entries.Remove(nextEntry.Value);
            entry = nextEntry.Value;
            return group;
        }

        private GroupBoxLeastEnlarged pickNext(IList<ISpatialIndexNode> nodes, BoundingBox group1Box, BoundingBox group2Box, out ISpatialIndexNode nextNode)
        {
            double maxArealDifference = Double.MinValue;
            GroupBoxLeastEnlarged group = GroupBoxLeastEnlarged.Tie;
            nextNode = null;

            foreach (ISpatialIndexNode node in nodes)
            {
                double arealDifferenceGroup1 = Math.Abs(BoundingBox.Join(group1Box, node.BoundingBox).GetArea() - group1Box.GetArea());
                double arealDifferenceGroup2 = Math.Abs(BoundingBox.Join(group2Box, node.BoundingBox).GetArea() - group2Box.GetArea());
                
                if (Math.Abs(arealDifferenceGroup1 - arealDifferenceGroup2) > maxArealDifference)
                {
                    maxArealDifference = Math.Abs(arealDifferenceGroup1 - arealDifferenceGroup2);
                    nextNode = node;

                    if (arealDifferenceGroup1 < arealDifferenceGroup2)
                    {
                        group = GroupBoxLeastEnlarged.Group1;
                    }
                    else if (arealDifferenceGroup2 < arealDifferenceGroup1)
                    {
                        group = GroupBoxLeastEnlarged.Group2;
                    }
                }
            }

            nodes.Remove(nextNode);
            return group;
        }

        private BoundingBox computeBox(IList<RTreeIndexEntry<TValue>> entries)
        {
            BoundingBox boundingBox = BoundingBox.Empty;

            foreach (RTreeIndexEntry<TValue> entry in entries)
            {
                boundingBox.ExpandToInclude(entry.BoundingBox);
            }

            return boundingBox;
        }

        private BoundingBox computeBox(IList<ISpatialIndexNode> nodes)
        {
            BoundingBox boundingBox = BoundingBox.Empty;

            foreach (ISpatialIndexNode node in nodes)
            {
                boundingBox.ExpandToInclude(node.BoundingBox);
            }

            return boundingBox;
        }
    }
}
