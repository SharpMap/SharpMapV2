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
    public class GuttmanQuadraticSplit : INodeSplitStrategy
    {
        #region INodeSplitStrategy Members

        public Node SplitNode(Node node, DynamicRTreeHeuristic huristic)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            if (node is LeafNode)
                return splitLeafNode(node as LeafNode, huristic);
            else if (node is IndexNode)
                return splitIndexNode(node as IndexNode, huristic);

            throw new ArgumentException("Invalid node type. Must be a LeafNode or an IndexNode.", "node");
        }

        #endregion

        private Node splitLeafNode(LeafNode node, DynamicRTreeHeuristic heuristic)
        {
            List<RTreeIndexEntry> entries = new List<RTreeIndexEntry>(node.Entries);
            List<RTreeIndexEntry> group1 = new List<RTreeIndexEntry>();
            List<RTreeIndexEntry> group2 = new List<RTreeIndexEntry>();
            pickSeeds(entries, group1, group2);
            distribute(entries, group1, group2, heuristic);
            if (entries.Count > 0)
            {
                if (group1.Count < heuristic.NodeItemMinimumCount)
                    group1.AddRange(entries);
                if (group2.Count < heuristic.NodeItemMinimumCount)
                    group2.AddRange(entries);
            }

            node.Clear();
            node.AddRange(group1);
            LeafNode sibling = node.Tree.CreateLeafNode();
            sibling.AddRange(group2);
            return sibling;
        }

        private Node splitIndexNode(IndexNode indexNode, DynamicRTreeHeuristic heuristic)
        {
            List<Node> children = new List<Node>(indexNode.Children);
            List<Node> group1 = new List<Node>();
            List<Node> group2 = new List<Node>();
            pickSeeds(children, group1, group2);
            distribute(children, group1, group2, heuristic);
            if (children.Count > 0)
            {
                if (group1.Count < heuristic.NodeItemMinimumCount)
                    group1.AddRange(children);
                if (group2.Count < heuristic.NodeItemMinimumCount)
                    group2.AddRange(children);
            }

            indexNode.Clear();
            indexNode.AddRange(group1);
            IndexNode sibling = indexNode.Tree.CreateIndexNode();
            sibling.AddRange(group2);
            return sibling;
        }

        private void pickSeeds(IList<RTreeIndexEntry> entries, List<RTreeIndexEntry> group1, List<RTreeIndexEntry> group2)
        {
            double largestWaste = Double.MinValue;
            foreach (RTreeIndexEntry entry1 in entries)
                foreach (RTreeIndexEntry entry2 in entries)
                {
                    if (entry1.Id == entry2.Id)
                        continue;

                    SharpMap.Geometries.BoundingBox minBoundingRectangle = new SharpMap.Geometries.BoundingBox(entry1.Box, entry2.Box);
                    double waste = minBoundingRectangle.GetArea() - entry1.Box.GetArea() - entry2.Box.GetArea() + entry1.Box.GetIntersectingArea(entry2.Box);

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

            entries.Remove(group1[0]);
            entries.Remove(group2[0]);
        }

        private void pickSeeds(IList<Node> nodes, List<Node> group1, List<Node> group2)
        {
            double largestWaste = Double.MinValue;
            foreach (Node node1 in nodes)
                foreach (Node node2 in nodes)
                {
                    if (node1.Id == node2.Id)
                        continue;

                    BoundingBox minBoundingRectangle = new BoundingBox(node1.Box, node2.Box);
                    double waste = minBoundingRectangle.GetArea() - node1.Box.GetArea() - node2.Box.GetArea() + node1.Box.GetIntersectingArea(node2.Box);

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

            nodes.Remove(group1[0]);
            nodes.Remove(group2[0]);
        }

        enum GroupBoxLeastEnlarged
        {
            Tie = 0,
            Group1,
            Group2
        }

        private void distribute(IList<RTreeIndexEntry> entries, List<RTreeIndexEntry> group1, List<RTreeIndexEntry> group2, DynamicRTreeHeuristic huristic)
        {
            if (entries.Count == 0)
                return;

            if (group1.Count == huristic.TargetNodeCount || group2.Count == huristic.TargetNodeCount)
                return;

            BoundingBox group1Box = computeBox(group1);
            BoundingBox group2Box = computeBox(group2);
            RTreeIndexEntry entry;
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
                        group1.Add(entry);
                    else if (group2Box.GetArea() < group1Box.GetArea())
                        group2.Add(entry);
                    else if (group1.Count < group2.Count)
                        group1.Add(entry);
                    else if (group2.Count < group1.Count)
                        group2.Add(entry);
                    else if ((DateTime.Now.Ticks / 10) % 2 == 0)
                        group1.Add(entry);
                    else
                        group2.Add(entry);
                    break;
            }

            distribute(entries, group1, group2, huristic);
        }

        private void distribute(IList<Node> nodes, List<Node> group1, List<Node> group2, DynamicRTreeHeuristic huristic)
        {
            if (nodes.Count == 0)
                return;

            if (group1.Count == huristic.TargetNodeCount || group2.Count == huristic.TargetNodeCount)
                return;

            BoundingBox group1Box = computeBox(group1);
            BoundingBox group2Box = computeBox(group2);
            Node node;
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
                        group1.Add(node);
                    else if (group2Box.GetArea() < group1Box.GetArea())
                        group2.Add(node);
                    else if (group1.Count < group2.Count)
                        group1.Add(node);
                    else if (group2.Count < group1.Count)
                        group2.Add(node);
                    else if ((DateTime.Now.Ticks / 10) % 2 == 0)
                        group1.Add(node);
                    else
                        group2.Add(node);
                    break;
            }

            distribute(nodes, group1, group2, huristic);
        }

        private GroupBoxLeastEnlarged pickNext(IList<RTreeIndexEntry> entries, BoundingBox group1Box, BoundingBox group2Box, out RTreeIndexEntry entry)
        {
            double maxArealDifference = Double.MinValue;
            GroupBoxLeastEnlarged group = GroupBoxLeastEnlarged.Tie;
            RTreeIndexEntry? nextEntry = null;
            foreach (RTreeIndexEntry e in entries)
            {
                double arealDifferenceGroup1 = Math.Abs(BoundingBox.Join(group1Box, e.Box).GetArea() - group1Box.GetArea());
                double arealDifferenceGroup2 = Math.Abs(BoundingBox.Join(group2Box, e.Box).GetArea() - group2Box.GetArea());
                if (Math.Abs(arealDifferenceGroup1 - arealDifferenceGroup2) > maxArealDifference)
                {
                    maxArealDifference = Math.Abs(arealDifferenceGroup1 - arealDifferenceGroup2);
                    nextEntry = e;
                    if (arealDifferenceGroup1 < arealDifferenceGroup2)
                        group = GroupBoxLeastEnlarged.Group1;
                    else if (arealDifferenceGroup2 < arealDifferenceGroup1)
                        group = GroupBoxLeastEnlarged.Group2;
                }
            }

            entries.Remove(nextEntry.Value);
            entry = nextEntry.Value;
            return group;
        }

        private GroupBoxLeastEnlarged pickNext(IList<Node> nodes, BoundingBox group1Box, BoundingBox group2Box, out Node nextNode)
        {
            double maxArealDifference = Double.MinValue;
            GroupBoxLeastEnlarged group = GroupBoxLeastEnlarged.Tie;
            nextNode = null;
            foreach (Node node in nodes)
            {
                double arealDifferenceGroup1 = Math.Abs(BoundingBox.Join(group1Box, node.Box).GetArea() - group1Box.GetArea());
                double arealDifferenceGroup2 = Math.Abs(BoundingBox.Join(group2Box, node.Box).GetArea() - group2Box.GetArea());
                if (Math.Abs(arealDifferenceGroup1 - arealDifferenceGroup2) > maxArealDifference)
                {
                    maxArealDifference = Math.Abs(arealDifferenceGroup1 - arealDifferenceGroup2);
                    nextNode = node;
                    if (arealDifferenceGroup1 < arealDifferenceGroup2)
                        group = GroupBoxLeastEnlarged.Group1;
                    else if (arealDifferenceGroup2 < arealDifferenceGroup1)
                        group = GroupBoxLeastEnlarged.Group2;
                }
            }

            nodes.Remove(nextNode);
            return group;
        }

        private BoundingBox computeBox(IList<RTreeIndexEntry> entries)
        {
            BoundingBox boundingBox = BoundingBox.Empty;
            foreach (RTreeIndexEntry entry in entries)
                boundingBox.ExpandToInclude(entry.Box);

            return boundingBox;
        }

        private BoundingBox computeBox(IList<Node> nodes)
        {
            BoundingBox boundingBox = BoundingBox.Empty;
            foreach (Node node in nodes)
                boundingBox.ExpandToInclude(node.Box);

            return boundingBox;
        }
    }
}
