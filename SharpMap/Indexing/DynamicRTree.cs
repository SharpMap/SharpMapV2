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
using System.IO;
using SharpMap.Geometries;
using System.Runtime.Serialization;

namespace SharpMap.Indexing
{
	/// <summary>
	/// Heuristics used for tree generation
	/// </summary>
	public struct DynamicRTreeHeuristic
	{
        private readonly int _nodeItemMin;
        private readonly int _nodeItemMax;

        public DynamicRTreeHeuristic(int nodeItemMinimumCount, int nodeItemMaximumCount)
        {
            _nodeItemMin = nodeItemMinimumCount;
            _nodeItemMax = nodeItemMaximumCount;
        }

		/// <summary>
		/// Minimum number of <see cref="QuadTreeIndexEntry">entries</see> in a node before it is a candiate for splitting
        /// the node.
		/// </summary>
        public int NodeItemMinimumCount
        {
            get { return _nodeItemMin; }
        }

		/// <summary>
		/// Number of <see cref="QuadTreeIndexEntry">entries</see> in a node to target. More than this will cause a split
        /// if <see cref="MaxTreeDepth"/> is not reached.
		/// </summary>
        public int NodeItemMaximumCount
        {
            get { return _nodeItemMax; }
        }

        /// <summary>
        /// The target number of nodes for a split node.
        /// </summary>
        public int TargetNodeCount
        {
            get { return NodeItemMaximumCount - NodeItemMinimumCount + 1; }
        }
    }
	
	/// <summary>
    /// An implementation of an updatable R-Tree spatial index.
	/// </summary>
    /// <remarks>
    /// Depending on the strategy implemented and used in the construction of an instance of <see cref="DynamicRTree"/>,
    /// the instance could be one of a number of various R-Tree variants, such as R*Tree or R+Tree.
    /// </remarks>
	public class DynamicRTree : RTree, System.Runtime.Serialization.ISerializable
	{
        private DynamicRTreeHeuristic _heuristic = new DynamicRTreeHeuristic(4, 10);
        private INodeSplitStrategy _nodeSplitStrategy;
        private IEntryInsertStrategy _insertStrategy;

		/// <summary>
        /// Creates a new updatable R-Tree instance
		/// </summary>
        /// <param name="insertStrategy">Strategy used to insert new entries into the index.</param>
        /// <param name="nodeSplitStrategy">Strategy used to split nodes when they are full.</param>
        /// <param name="heuristic">Heuristics used to build the index and keep it balanced.</param>
		public DynamicRTree(IEntryInsertStrategy insertStrategy, INodeSplitStrategy nodeSplitStrategy, DynamicRTreeHeuristic heuristic)
            : base()
		{
            _insertStrategy = insertStrategy;
            _nodeSplitStrategy = nodeSplitStrategy;
            _heuristic = heuristic;
		}

		/// <summary>
		/// This constructor is used internally for loading a tree from a stream
		/// </summary>
        protected DynamicRTree()
		{
		}

        protected DynamicRTree(SerializationInfo info, StreamingContext context)
        {
            byte[] data = (byte[])info.GetValue("data", typeof(byte[]));
            MemoryStream buffer = new MemoryStream(data);
            DynamicRTree tree = FromStream(buffer);
            Root = tree.Root;
        }

        /// <summary>
        /// Huristic used to create and balance the tree
        /// </summary>
        public DynamicRTreeHeuristic Heuristic
        {
            get { return _heuristic; }
        }

        /// <summary>
        /// The strategy used to insert new entries into the index
        /// </summary>
        public IEntryInsertStrategy EntryInsertStrategy
        {
            get { return _insertStrategy; }
        }

        /// <summary>
        /// Inserts a node into the tree using <see cref="EntryStrategy"/>
        /// </summary>
        /// <param name="key">Key for the geometry</param>
        /// <param name="region">Bounding box of the geometry to enter into the index</param>
        public virtual void Insert(UInt32 key, BoundingBox region)
        {
            RTreeIndexEntry entry = new RTreeIndexEntry(key, region);
            Node newSiblingFromSplit;
            
            EntryInsertStrategy.InsertEntry(entry, Root, _nodeSplitStrategy, Heuristic, out newSiblingFromSplit);

            // Add the newly split sibling
            if (newSiblingFromSplit is LeafNode)
            {
                if (Root is LeafNode)
                {
                    LeafNode oldRoot = Root as LeafNode;
                    Root = CreateIndexNode();
                    (Root as IndexNode).Add(oldRoot);
                }

                (Root as IndexNode).Add(newSiblingFromSplit);
                newSiblingFromSplit = null;
            }
            else if (newSiblingFromSplit is IndexNode) // Came from a root split
            {
                Node oldRoot = Root;
                Root = CreateIndexNode();
                (Root as IndexNode).Add(oldRoot);
                (Root as IndexNode).Add(newSiblingFromSplit);
            }
        }

        #region Read/Write index to/from a file

        private static readonly System.Version IndexFileVersion = new System.Version(1, 1, 0, 0);

        /// <summary>
        /// Loads an R-Tree structure from a stream
        /// </summary>
        /// <param name="filename">Stream which holds the R-Tree spatial index</param>
        /// <returns>A <see cref="DynamicRTree"/> instance which had been persisted to the <see cref="System.IO.Stream">stream</see>.</returns>
        public static DynamicRTree FromStream(Stream data)
        {
            DynamicRTree tree = new DynamicRTree();
            using (System.IO.BinaryReader br = new System.IO.BinaryReader(data))
            {
                System.Version fileVersion = new System.Version(br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32());
                if (fileVersion != IndexFileVersion) //Check fileindex version
                    throw new ObsoleteFileFormatException(IndexFileVersion, fileVersion, "Invalid index file version. Please rebuild the spatial index by deleting the index.");

                tree.Root = ReadNode(tree, br);
            }

            return tree;
        }

        /// <summary>
        /// Reads a node from a stream recursively
        /// </summary>
        /// <param name="tree">R-Tree instance</param>
        /// <param name="br">Binary reader reference</param>
        private static Node ReadNode(DynamicRTree tree, BinaryReader br)
        {
            if (br.BaseStream.Position == br.BaseStream.Length)
                return null;

            Node node;
            bool isLeaf = br.ReadBoolean();
            
            if (isLeaf)
                node = tree.CreateLeafNode();
            else
                node = tree.CreateIndexNode();

            node.Box = new BoundingBox(br.ReadDouble(), br.ReadDouble(), br.ReadDouble(), br.ReadDouble());

            if (isLeaf)
            {
                LeafNode leaf = node as LeafNode;
                int featureCount = br.ReadInt32();
                for (int i = 0; i < featureCount; i++)
                {
                    RTreeIndexEntry entry = new RTreeIndexEntry();
                    entry.Box = new BoundingBox(br.ReadDouble(), br.ReadDouble(), br.ReadDouble(), br.ReadDouble());
                    entry.Id = br.ReadUInt32();
                    leaf.Add(entry);
                }
            }
            else
            {
                IndexNode index = node as IndexNode;
                uint childNodes = br.ReadUInt32();
                for (int c = 0; c < childNodes; c++)
                {
                    Node child = ReadNode(tree, br);
                    if(child != null)
                        index.Add(child);
                }
            }

            return node;
        }

        /// <summary>
        /// Saves the spatial index to a stream
        /// </summary>
        /// <param name="filename"></param>
        public void SaveIndex(Stream buffer)
        {
            using (System.IO.BinaryWriter bw = new BinaryWriter(buffer))
            {
                //Save index version
                bw.Write(IndexFileVersion.Major);
                bw.Write(IndexFileVersion.Minor);
                bw.Write(IndexFileVersion.Build);
                bw.Write(IndexFileVersion.Revision); 
                SaveNode(this.Root, bw);
            }
        }

        /// <summary>
        /// Saves a node to a stream recursively
        /// </summary>
        /// <param name="node">Node to save</param>
        /// <param name="sw">BinaryWriter to write to stream</param>
        private void SaveNode(Node node, BinaryWriter writer)
        {
            //Write node boundingbox
            writer.Write(node.Box.Left);
            writer.Write(node.Box.Bottom);
            writer.Write(node.Box.Right);
            writer.Write(node.Box.Top);
            writer.Write(node is LeafNode);
            if (node is LeafNode)
            {
                LeafNode leaf = node as LeafNode;
                writer.Write(leaf.Entries.Count); //Write number of features at node
                foreach (RTreeIndexEntry entry in leaf.Entries) //Write each featurebox
                {
                    writer.Write(entry.Box.Left);
                    writer.Write(entry.Box.Bottom);
                    writer.Write(entry.Box.Right);
                    writer.Write(entry.Box.Top);
                    writer.Write(entry.Id);
                }
            }
            else if (node is IndexNode) //Save next node
            {
                foreach (Node child in (node as IndexNode).Children)
                    SaveNode(child, writer);
            }
        }

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            MemoryStream buffer = new MemoryStream();
            SaveIndex(buffer);
            info.AddValue("data", buffer.ToArray());
        }

        #endregion
    }

    /// <summary>
    /// Exception thrown when loading spatial index file fails due to imcompatible version
    /// </summary>
    public class ObsoleteFileFormatException : Exception
    {
        private readonly System.Version _versionExpected;
        private readonly System.Version _versionEncountered;

        public ObsoleteFileFormatException(System.Version versionExpected, System.Version versionEncountered)
            : this(versionExpected, versionEncountered, null) { }

        public ObsoleteFileFormatException(System.Version versionExpected, System.Version versionEncountered, string message) 
            : base(message) 
        {
            _versionExpected = versionExpected;
            _versionEncountered = versionEncountered;
        }

        /// <summary>
        /// Exception thrown when layer rendering has failed. This constructor is used by the runtime during serialization.
        /// </summary>
        /// <param name="info">Data used to reconstruct instance</param>
        /// <param name="context">Serialization context</param>
        public ObsoleteFileFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <summary>
        /// The version of the file expected
        /// </summary>
        public System.Version VersionExpected
        {
            get { return _versionExpected; }
        }

        /// <summary>
        /// The version of the file actually encountered
        /// </summary>
        public System.Version VersionEncountered
        {
            get { return _versionEncountered; }
        }

        public override string ToString()
        {
            return String.Format("Spatial index file version was not compatible with the version expected. Expected: {0}; Encountered: {1}.", VersionExpected, VersionEncountered);
        }
    }
}
