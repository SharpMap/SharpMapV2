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
    /// <summary>
    /// An entry in the <see cref="DynamicRTree"/>.
    /// </summary>
    public struct RTreeIndexEntry
    {
        public RTreeIndexEntry(UInt32 key, BoundingBox box)
        {
            _id = key;
            _box = box;
        }

        /// <summary>
        /// Feature Id
        /// </summary>
        private uint _id;
        public UInt32 Id
        {
            get { return _id; }
            internal set { _id = value; }
        }

        /// <summary>
        /// <see cref="BoundingBox"/>
        /// </summary>
        private BoundingBox _box;
        public BoundingBox Box
        {
            get { return _box; }
            internal set { _box = value; }
        }

        public override string ToString()
        {
            return String.Format("Type: {0}; Id: {1}; Box: {2}", GetType().Name, _id, _box);
        }
    }

    /// <summary>
    /// Abstract representation of a node in a <see cref="DynamicRTree"/>
    /// </summary>
    public abstract class Node
    {
        private BoundingBox _box = BoundingBox.Empty;
        private uint? _id;
        private RTree _tree;

        /// <summary>
        /// Gets/sets the Bounding Box for this node, which minimally bounds all children nodes
        /// </summary>
        public BoundingBox Box
        {
            get { return _box; }
            set { _box = value; }
        }

        /// <summary>
        /// Node identifier
        /// </summary>
        public uint? Id
        {
            get { return _id; }
            internal set { _id = value; }
        }

        /// <summary>
        /// Reference to the containing tree
        /// </summary>
        public RTree Tree
        {
            get { return _tree; }
            protected set { _tree = value; }
        }

        public override string ToString()
        {
            return String.Format("Node Id: {0}; Box: {1}", _id, _box);
        }
    }

    /// <summary>
    /// Node type used for branch nodes in the <see cref="DynamicRTree"/>
    /// </summary>
    public class IndexNode : Node
    {
        private List<Node> _children = new List<Node>();

        private IndexNode(RTree tree)
        {
            Tree = tree;
        }

        /// <summary>
        /// Child nodes in the <see cref="DynamicRTree"/>
        /// </summary>
        public IList<Node> Children
        {
            get { return _children.AsReadOnly(); }
        }

        /// <summary>
        /// Adds child node to <see cref="Children"/> node collection and expands node's
        /// <see cref="Box">bounding box</see> to contain <paramref name="child"/> node
        /// bounding box.
        /// </summary>
        /// <param name="child">Node to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="child"/> is null</exception>
        public void Add(Node child)
        {
            if (child == null)
                throw new ArgumentNullException("child");

            _children.Add(child);
            Box = Box.Join(child.Box);
        }

        /// <summary>
        /// Adds a range of child nodes to <see cref="Children"/> node collection and expands node's
        /// <see cref="Box">bounding box</see> to contain all <paramref name="children"/> nodes'
        /// bounding boxes.
        /// </summary>
        /// <param name="children">Nodes to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="children"/> is null, 
        /// or if any node in <paramref name="children"/> is null</exception>
        public void AddRange(IEnumerable<Node> children)
        {
            if (children == null)
                throw new ArgumentNullException("children");

            foreach (Node child in children)
                if (child == null)
                    throw new ArgumentException("Entry in children parameter cannot be null", "children");
                else
                    Box = Box.Join(child.Box);

            _children.AddRange(children);
        }

        /// <summary>
        /// Removes a child node and adjusts <see cref="Node.Box">bounding box</see> to accomodate new 
        /// set of child nodes.
        /// </summary>
        /// <param name="child">Child node to remove</param>
        /// <returns>True if the node was found and removed, false if not found or not removed.</returns>
        public bool Remove(Node child)
        {
            bool removed = _children.Remove(child);
            if (removed && Box.Borders(child.Box))
            {
                Box = BoundingBox.Empty;
                foreach (Node c in _children)
                    Box = Box.Join(c.Box);
            }

            return removed;
        }

        /// <summary>
        /// Removes all children nodes and sets the <see cref="Node.Box">bounding box</see> to <see cref="BoundingBox.Empty"/>
        /// </summary>
        public void Clear()
        {
            _children.Clear();
            Box = BoundingBox.Empty;
        }

        public override string ToString()
        {
            return String.Format("Index: {0}; Box: {1}; Children: {2}", Id, Box, _children.Count);
        }

        internal static IndexNode CreateNode(RTree tree)
        {
            return new IndexNode(tree);
        }
    }

    /// <summary>
    /// Node type used for the leaf nodes in the <see cref="DynamicRTree"/>
    /// </summary>
    public class LeafNode : Node
    {
        private List<RTreeIndexEntry> _entries = new List<RTreeIndexEntry>();

        private LeafNode(RTree tree)
        {
            Tree = tree;
        }

        /// <summary>
        /// Gets the list of entries in the leaf node
        /// </summary>
        public IList<RTreeIndexEntry> Entries
        {
            get { return _entries.AsReadOnly(); }
            private set { _entries = new List<RTreeIndexEntry>(value); }
        }

        /// <summary>
        /// Adds entry to <see cref="Entries"/> collection and expands node's
        /// <see cref="Box">bounding box</see> to contain <paramref name="entry"/>
        /// bounding box.
        /// </summary>
        /// <param name="entry">Entry to add</param>
        public void Add(RTreeIndexEntry entry)
        {
            _entries.Add(entry);
            Box = Box.Join(entry.Box);
        }

        /// <summary>
        /// Adds entries to <see cref="Entries"/> collection and expands node's
        /// <see cref="Box">bounding box</see> to contain all <paramref name="entries">entries'</paramref>
        /// bounding boxes
        /// </summary>
        /// <param name="entries">Entries to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="entries"/> is null</exception>
        public void AddRange(IEnumerable<RTreeIndexEntry> entries)
        {
            if (entries == null)
                throw new ArgumentNullException("entries");

            _entries.AddRange(entries);
            foreach (RTreeIndexEntry entry in entries)
                Box = Box.Join(entry.Box);
        }

        /// <summary>
        /// Removes an entry and adjusts <see cref="Node.Box">bounding box</see> to accomodate new 
        /// set of entries.
        /// </summary>
        /// <param name="entry">Entry to remove</param>
        /// <returns>True if the entry was found and removed, false if not found or not removed.</returns>
        public bool Remove(RTreeIndexEntry entry)
        {
            bool removed = _entries.Remove(entry);
            if (removed && Box.Borders(entry.Box))
            {
                Box = BoundingBox.Empty;
                foreach (RTreeIndexEntry e in _entries)
                    Box = Box.Join(e.Box);
            }

            return removed;
        }

        /// <summary>
        /// Removes all entries and sets the <see cref="Node.Box">bounding box</see> to <see cref="BoundingBox.Empty"/>
        /// </summary>
        public void Clear()
        {
            _entries.Clear();
            Box = BoundingBox.Empty;
        }

        public override string ToString()
        {
            return String.Format("Leaf: {0}; Box: {1}; Entries: {2}", Id, Box, _entries.Count);
        }

        internal static LeafNode CreateNode(RTree tree)
        {
            return new LeafNode(tree);
        }
    }

    public abstract class RTree : IDisposable
    {
        private Node _root;
        private bool _disposed;
        private long _nextNodeId = 0;

        public RTree()
        {
            Root = CreateLeafNode();
        }

        /// <summary>
        /// Root node in the R-Tree
        /// </summary>
        public Node Root
        {
            get { return _root; }
            protected set { _root = value; }
        }

        /// <summary>
        /// Searches the tree and looks for intersections with the boundingbox 'bbox'
        /// </summary>
        /// <param name="box">Boundingbox to intersect with</param>
        public virtual List<UInt32> Search(BoundingBox box)
        {
            List<uint> objectlist = new List<uint>();
            IntersectTreeRecursive(box, Root, objectlist);
            return objectlist;
        }

        /// <summary>
        /// Disposes the node
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                Disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        protected bool Disposed
        {
            get { return _disposed; }
            set { _disposed = value; }
        }

        protected internal virtual IndexNode CreateIndexNode()
        {
            IndexNode node = IndexNode.CreateNode(this);
            node.Id = getNewNodeId();
            return node;
        }

        protected internal virtual LeafNode CreateLeafNode()
        {
            LeafNode node = LeafNode.CreateNode(this);
            node.Id = getNewNodeId();
            return node;
        }

        private uint getNewNodeId()
        {
            return (uint)System.Threading.Interlocked.Increment(ref _nextNodeId);
        }

        /// <summary>
        /// Recursive function that traverses the tree and looks for intersections with a given <see cref="BoundingBox">region</see>.
        /// </summary>
        /// <remarks>
        /// Nothing is added to the list if <paramref name="box"/> equals <see cref="BoundingBox.Empty"/>.
        /// </remarks>
        /// <param name="box">Boundingbox to intersect with</param>
        /// <param name="node">Node to search from</param>
        /// <param name="list">List of found intersections</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is null or if <paramref name="list"/> is null.</exception>
        protected void IntersectTreeRecursive(BoundingBox box, Node node, List<uint> list)
        {
            if (node == null)
                throw new ArgumentNullException("node");
            if (list == null)
                throw new ArgumentNullException("list");
            
            if (box == BoundingBox.Empty)
                return;

            if (node is LeafNode) //Leaf has been reached
            {
                foreach (RTreeIndexEntry entry in (node as LeafNode).Entries)
                    if (entry.Box.Intersects(box))
                        list.Add(entry.Id);
            }
            else if (node is IndexNode)
            {
                if (box.Intersects(node.Box))
                    foreach (Node child in (node as IndexNode).Children)
                        IntersectTreeRecursive(box, child, list);
            }
            else
                throw new InvalidOperationException("Unknown node type: " + node.GetType());
        }
    }
}
