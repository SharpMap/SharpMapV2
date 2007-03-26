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
