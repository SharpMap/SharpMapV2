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
using System.Runtime.Serialization.Formatters.Binary;

using SharpMap.Indexing;

namespace SharpMap.Indexing.RTree
{	
	/// <summary>
    /// An implementation of an updatable R-Tree spatial index.
    /// </summary>
    /// <typeparam name="TValue">The type of the value used in the entries.</typeparam>
    /// <remarks>
    /// Depending on the strategy implemented and used in the construction of an instance of <see cref="DynamicRTree"/>,
    /// the instance could be one of a number of various R-Tree variants, such as R*Tree or R+Tree.
    /// </remarks>
    public class DynamicRTree<TValue> : RTree<TValue>, IUpdatableSpatialIndex<RTreeIndexEntry<TValue>>, ISerializable
        where TValue : IEquatable<TValue>
	{
        private static readonly BinaryFormatter _keyFormatter = new BinaryFormatter();
        private DynamicRTreeBalanceHeuristic _heuristic = new DynamicRTreeBalanceHeuristic(4, 10, UInt16.MaxValue);
        private INodeSplitStrategy _nodeSplitStrategy;
        private IEntryInsertStrategy<RTreeIndexEntry<TValue>> _insertStrategy;

		/// <summary>
        /// Creates a new updatable R-Tree instance
		/// </summary>
        /// <param name="insertStrategy">Strategy used to insert new entries into the index.</param>
        /// <param name="nodeSplitStrategy">Strategy used to split nodes when they are full.</param>
        /// <param name="heuristic">Heuristics used to build the index and keep it balanced.</param>
        public DynamicRTree(IEntryInsertStrategy<RTreeIndexEntry<TValue>> insertStrategy, INodeSplitStrategy nodeSplitStrategy, DynamicRTreeBalanceHeuristic heuristic)
            : base()
		{
            _insertStrategy = insertStrategy;
            _nodeSplitStrategy = nodeSplitStrategy;
            _heuristic = heuristic;
		}

		/// <summary>
		/// This constructor is used internally for loading a tree from a stream.
		/// </summary>
        protected DynamicRTree()
		{
		}

        protected DynamicRTree(SerializationInfo info, StreamingContext context)
        {
            byte[] data = (byte[])info.GetValue("data", typeof(byte[]));
            MemoryStream buffer = new MemoryStream(data);
            DynamicRTree<TValue> tree = FromStream(buffer);
            Root = tree.Root;
        }

        /// <summary>
        /// Huristic used to create and balance the tree.
        /// </summary>
        public IndexBalanceHeuristic Heuristic
        {
            get { return _heuristic; }
        }

        /// <summary>
        /// The strategy used to insert new entries into the index.
        /// </summary>
        public IEntryInsertStrategy<RTreeIndexEntry<TValue>> EntryInsertStrategy
        {
            get { return _insertStrategy; }
        }

        /// <summary>
        /// Inserts a node into the tree using <see cref="EntryStrategy"/>.
        /// </summary>
        /// <param name="key">Key for the geometry.</param>
        /// <param name="region">Bounding box of the geometry to enter into the index.</param>
        public virtual void Insert(RTreeIndexEntry<TValue> entry)
        {
            ISpatialIndexNode newSiblingFromSplit;
            
            EntryInsertStrategy.InsertEntry(entry, Root, _nodeSplitStrategy, Heuristic, out newSiblingFromSplit);

            // Add the newly split sibling
            if (newSiblingFromSplit is RTreeLeafNode<TValue>)
            {
                if (Root is RTreeLeafNode<TValue>)
                {
                    RTreeLeafNode<TValue> oldRoot = Root as RTreeLeafNode<TValue>;
                    Root = CreateBranchNode();
                    (Root as RTreeBranchNode<TValue>).Add(oldRoot);
                }

                (Root as RTreeBranchNode<TValue>).Add(newSiblingFromSplit);
                newSiblingFromSplit = null;
            }
            else if (newSiblingFromSplit is RTreeBranchNode<TValue>) // Came from a root split
            {
                ISpatialIndexNode oldRoot = Root;
                Root = CreateBranchNode();
                (Root as RTreeBranchNode<TValue>).Add(oldRoot);
                (Root as RTreeBranchNode<TValue>).Add(newSiblingFromSplit);
            }
        }

        #region Read/Write index to/from a file
        
        private static readonly System.Version IndexFileVersion = new System.Version(2, 0, 0, 0);

        /// <summary>
        /// Loads an R-Tree structure from a stream
        /// </summary>
        /// <param name="filename">Stream which holds the R-Tree spatial index</param>
        /// <returns>A <see cref="DynamicRTree"/> instance which had been persisted to the <see cref="System.IO.Stream">stream</see>.</returns>
        public static DynamicRTree<TValue> FromStream(Stream data)
        {
            DynamicRTree<TValue> tree = new DynamicRTree<TValue>();

            using (System.IO.BinaryReader br = new System.IO.BinaryReader(data))
            {
                System.Version fileVersion = new System.Version(br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32());

                if (fileVersion != IndexFileVersion) //Check fileindex version
                {
                    throw new ObsoleteIndexFileFormatException(IndexFileVersion, fileVersion, "Invalid index file version. Please rebuild the spatial index by deleting the index.");
                }

                uint keyLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(TValue));

                using (MemoryStream keyBuffer = new MemoryStream((int)keyLength))
                {
                    tree.Root = readNode(tree, br, keyBuffer, keyLength);
                }
            }

            return tree;
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

                uint keyLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(TValue));

                using (MemoryStream keyBuffer = new MemoryStream((int)keyLength))
                {
                    saveNode(this.Root, bw, keyBuffer, keyLength);
                }
            }
        }

        /// <summary>
        /// Reads a node from a stream recursively.
        /// </summary>
        /// <param name="tree">R-Tree instance.</param>
        /// <param name="br">Binary reader reference.</param>
        private static ISpatialIndexNode readNode(DynamicRTree<TValue> tree, BinaryReader br, MemoryStream keyBuffer, uint keyLength)
        {
            if (br.BaseStream.Position == br.BaseStream.Length)
            {
                return null;
            }

            ISpatialIndexNode node;
            bool isLeaf = br.ReadBoolean();

            BoundingBox bounds = new BoundingBox(br.ReadDouble(), br.ReadDouble(), br.ReadDouble(), br.ReadDouble());

            if (isLeaf)
            {
                node = tree.CreateLeafNode(bounds);
            }
            else
            {
                node = tree.CreateBranchNode(bounds);
            }

            if (isLeaf)
            {
                RTreeLeafNode<TValue> leaf = node as RTreeLeafNode<TValue>;
                int featureCount = br.ReadInt32();
             
                for (int i = 0; i < featureCount; i++)
                {
                    RTreeIndexEntry<TValue> entry = new RTreeIndexEntry<TValue>();
                    
                    entry.BoundingBox = new BoundingBox(br.ReadDouble(), br.ReadDouble(), br.ReadDouble(), br.ReadDouble());
                    
                    keyBuffer.Position = 0;
                    keyBuffer.Write(br.ReadBytes((int)keyLength), 0, (int)keyLength);
                    entry.Value = (TValue)_keyFormatter.Deserialize(keyBuffer);

                    leaf.Add(entry);
                }
            }
            else
            {
                RTreeBranchNode<TValue> index = node as RTreeBranchNode<TValue>;
                uint childNodes = br.ReadUInt32();

                for (int c = 0; c < childNodes; c++)
                {
                    ISpatialIndexNode child = readNode(tree, br, keyBuffer, keyLength);

                    if (child != null)
                    {
                        index.Add(child);
                    }
                }
            }

            return node;
        }

        /// <summary>
        /// Saves a node to a stream recursively
        /// </summary>
        /// <param name="node">Node to save</param>
        /// <param name="sw">BinaryWriter to write to stream</param>
        private void saveNode(ISpatialIndexNode node, BinaryWriter writer, MemoryStream keyBuffer, uint keyLength)
        {
            //Write node boundingbox
            writer.Write(node.BoundingBox.Left);
            writer.Write(node.BoundingBox.Bottom);
            writer.Write(node.BoundingBox.Right);
            writer.Write(node.BoundingBox.Top);
            writer.Write(node is RTreeLeafNode<TValue>);

            if (node is RTreeLeafNode<TValue>)
            {
                RTreeLeafNode<TValue> leaf = node as RTreeLeafNode<TValue>;
                writer.Write(leaf.Items.Count); //Write number of features at node

                if (keyBuffer.Capacity != keyLength)
                {
                    keyBuffer.SetLength(keyLength);
                    keyBuffer.Capacity = (int)keyLength;
                }

                foreach (RTreeIndexEntry<TValue> entry in leaf.Items) //Write each featurebox
                {
                    writer.Write(entry.BoundingBox.Left);
                    writer.Write(entry.BoundingBox.Bottom);
                    writer.Write(entry.BoundingBox.Right);
                    writer.Write(entry.BoundingBox.Top);

                    keyBuffer.Position = 0;
                    _keyFormatter.Serialize(keyBuffer, entry.Value);
                    writer.Write(keyBuffer.GetBuffer());
                }
            }
            else if (node is RTreeBranchNode<TValue>) //Save next node
            {
                foreach (ISpatialIndexNode child in (node as RTreeBranchNode<TValue>).Items)
                {
                    saveNode(child, writer, keyBuffer, keyLength);
                }
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
}
