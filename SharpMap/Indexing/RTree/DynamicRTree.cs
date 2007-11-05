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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SharpMap.Geometries;

namespace SharpMap.Indexing.RTree
{
    /// <summary>
    /// An implementation of an updatable R-Tree spatial index.
    /// </summary>
    /// <typeparam name="TValue">
    /// The type of the value used in the entries.
    /// </typeparam>
    /// <remarks>
    /// Depending on the strategy implemented and used in the 
    /// construction of an instance of <see cref="DynamicRTree{TValue}"/>,
    /// the instance could be one of a number of various R-Tree 
    /// variants, such as R*Tree or R+Tree.
    /// </remarks>
    public class DynamicRTree<TValue>
        : RTree<TValue>, IUpdatableSpatialIndex<RTreeIndexEntry<TValue>>, ISerializable
    {
        #region Fields

        private static readonly BinaryFormatter _keyFormatter = new BinaryFormatter();
        private DynamicRTreeBalanceHeuristic _heuristic = new DynamicRTreeBalanceHeuristic(4, 10, UInt16.MaxValue);
        private readonly INodeSplitStrategy _nodeSplitStrategy;
        private readonly IEntryInsertStrategy<RTreeIndexEntry<TValue>> _insertStrategy;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new updatable R-Tree instance
        /// </summary>
        /// <param name="insertStrategy">
        /// Strategy used to insert new entries into the index.
        /// </param>
        /// <param name="nodeSplitStrategy">
        /// Strategy used to split nodes when they are full.
        /// </param>
        /// <param name="heuristic">
        /// Heuristics used to build the index and keep it balanced.
        /// </param>
        public DynamicRTree(
            IEntryInsertStrategy<RTreeIndexEntry<TValue>> insertStrategy,
            INodeSplitStrategy nodeSplitStrategy, DynamicRTreeBalanceHeuristic heuristic)
        {
            _insertStrategy = insertStrategy;
            _nodeSplitStrategy = nodeSplitStrategy;
            _heuristic = heuristic;
        }

        /// <summary>
        /// This constructor is used internally 
        /// for loading a tree from a stream.
        /// </summary>
        protected DynamicRTree() {}

        protected DynamicRTree(SerializationInfo info, StreamingContext context)
        {
            byte[] data = (byte[]) info.GetValue("data", typeof (byte[]));
            MemoryStream buffer = new MemoryStream(data);
            DynamicRTree<TValue> tree = FromStream(buffer);
            Root = tree.Root;
        }

        #endregion

        /// <summary>
        /// The strategy used to insert new entries into the index.
        /// </summary>
        public IEntryInsertStrategy<RTreeIndexEntry<TValue>> EntryInsertStrategy
        {
            get { return _insertStrategy; }
        }

        /// <summary>
        /// Huristic used to create and balance the tree.
        /// </summary>
        public IndexBalanceHeuristic Heuristic
        {
            get { return _heuristic; }
        }

        /// <summary>
        /// Inserts a node into the tree using <see cref="EntryInsertStrategy"/>.
        /// </summary>
        /// <param name="entry">The entry to insert into the index.</param>
        public virtual void Insert(RTreeIndexEntry<TValue> entry)
        {
            ISpatialIndexNode newSiblingFromSplit;

            EntryInsertStrategy.InsertEntry(entry, Root, _nodeSplitStrategy, Heuristic, out newSiblingFromSplit);

            // Add the newly split sibling
            if (newSiblingFromSplit is RTreeLeafNode<TValue>)
            {
                RTreeBranchNode<TValue> node;
                
                if (Root is RTreeLeafNode<TValue>)
                {
                    RTreeLeafNode<TValue> oldRoot = Root as RTreeLeafNode<TValue>;
                    Root = CreateBranchNode();
                    node = Root as RTreeBranchNode<TValue>;
                    Debug.Assert(node != null);
                    node.Add(oldRoot);
                }

                node = Root as RTreeBranchNode<TValue>;
                Debug.Assert(node != null);
                node.Add(newSiblingFromSplit);
            }
            else if (newSiblingFromSplit is RTreeBranchNode<TValue>) // Came from a root split
            {
                ISpatialIndexNode oldRoot = Root;
                Root = CreateBranchNode();
                RTreeBranchNode<TValue> node = Root as RTreeBranchNode<TValue>;
                Debug.Assert(node != null);
                node.Add(oldRoot);
                node.Add(newSiblingFromSplit);
            }
        }

        /// <summary>
        /// Removes an entry from the index.
        /// </summary>
        /// <param name="entry">The entry to remove.</param>
        public virtual void Remove(RTreeIndexEntry<TValue> entry)
        {
            ISpatialIndexNode node = Root;

            if (node is RTreeBranchNode<TValue>)
            {
                RTreeBranchNode<TValue> branch = node as RTreeBranchNode<TValue>;
                branch.Remove(entry);
            }
            else if (node is RTreeLeafNode<TValue>)
            {
                RTreeLeafNode<TValue> leaf = node as RTreeLeafNode<TValue>;
                leaf.Remove(entry);
            }
        }

        #region Read/Write index to/from a file

        private static readonly Version IndexFileVersion = new Version(2, 0, 0, 0);

        /// <summary>
        /// Loads an R-Tree structure from a stream.
        /// </summary>
        /// <param name="data">Stream which holds the R-Tree spatial index.</param>
        /// <returns>
        /// A <see cref="DynamicRTree{TValue}"/> instance which had been 
        /// persisted to the <see cref="System.IO.Stream">stream</see>.
        /// </returns>
        public static DynamicRTree<TValue> FromStream(Stream data)
        {
            DynamicRTree<TValue> tree = new DynamicRTree<TValue>();

            using (BinaryReader br = new BinaryReader(data))
            {
                Version fileVersion = new Version(br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32());

                if (fileVersion != IndexFileVersion) //Check fileindex version
                {
                    throw new ObsoleteIndexFileFormatException(IndexFileVersion, fileVersion,
                                                               "Invalid index file version. Please rebuild the spatial index by deleting the index.");
                }

                uint keyLength = (uint) Marshal.SizeOf(typeof (TValue));

                using (MemoryStream keyBuffer = new MemoryStream((int) keyLength))
                {
                    tree.Root = readNode(tree, br, keyBuffer, keyLength);
                }
            }

            return tree;
        }

        /// <summary>
        /// Saves the spatial index to a stream.
        /// </summary>
        /// <param name="stream">Stream to save index to.</param>
        public void SaveIndex(Stream stream)
        {
            BinaryWriter writer = new BinaryWriter(stream);

            //Save index version
            writer.Write(IndexFileVersion.Major);
            writer.Write(IndexFileVersion.Minor);
            writer.Write(IndexFileVersion.Build);
            writer.Write(IndexFileVersion.Revision);

            uint keyLength = (uint) Marshal.SizeOf(typeof (TValue));

            using (MemoryStream keyBuffer = new MemoryStream((int) keyLength))
            {
                saveNode(Root, writer, keyBuffer, keyLength);
            }
        }

        /// <summary>
        /// Reads a node from a stream recursively.
        /// </summary>
        /// <param name="tree">R-Tree instance.</param>
        /// <param name="reader">Binary reader reference.</param>
        /// <param name="keyBuffer">Buffer to serialze key data into.</param>
        /// <param name="keyLength">Data length of serialized key.</param>
        private static ISpatialIndexNode readNode(DynamicRTree<TValue> tree, BinaryReader reader, 
            MemoryStream keyBuffer, uint keyLength)
        {
            if (reader.BaseStream.Position == reader.BaseStream.Length)
            {
                return null;
            }

            ISpatialIndexNode node;
            bool isLeaf = reader.ReadBoolean();

            BoundingBox bounds = new BoundingBox(
                                        reader.ReadDouble(), 
                                        reader.ReadDouble(), 
                                        reader.ReadDouble(), 
                                        reader.ReadDouble());

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
                Debug.Assert(leaf != null);

                int featureCount = reader.ReadInt32();

                for (int i = 0; i < featureCount; i++)
                {
                    RTreeIndexEntry<TValue> entry = new RTreeIndexEntry<TValue>();

                    entry.BoundingBox = new BoundingBox(
                                                reader.ReadDouble(), 
                                                reader.ReadDouble(), 
                                                reader.ReadDouble(), 
                                                reader.ReadDouble());

                    keyBuffer.Position = 0;
                    keyBuffer.Write(reader.ReadBytes((int) keyLength), 0, (int) keyLength);
                    entry.Value = (TValue) _keyFormatter.Deserialize(keyBuffer);

                    leaf.Add(entry);
                }
            }
            else
            {
                RTreeBranchNode<TValue> index = node as RTreeBranchNode<TValue>;
                Debug.Assert(index != null);

                uint childNodes = reader.ReadUInt32();

                for (int c = 0; c < childNodes; c++)
                {
                    ISpatialIndexNode child = readNode(tree, reader, keyBuffer, keyLength);

                    if (child != null)
                    {
                        index.Add(child);
                    }
                }
            }

            return node;
        }

        /// <summary>
        /// Saves a node to a stream recursively.
        /// </summary>
        /// <param name="node">Node to save.</param>
        /// <param name="writer">BinaryWriter to format values and write to stream.</param>
        /// <param name="keyBuffer">Buffer used to serialize key value.</param>
        /// <param name="keyLength">Number of bytes in the key's memory representation.</param>
        private static void saveNode(ISpatialIndexNode node, BinaryWriter writer, MemoryStream keyBuffer, uint keyLength)
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
                    keyBuffer.Capacity = (int) keyLength;
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