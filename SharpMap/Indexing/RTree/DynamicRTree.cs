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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using GeoAPI.Geometries;
using GeoAPI.Indexing;

namespace SharpMap.Indexing.RTree
{
    /// <summary>
    /// An implementation of an updatable R-Tree spatial index.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of the value used in the entries.
    /// </typeparam>
    /// <remarks>
    /// Depending on the strategy implemented and used in the 
    /// construction of an instance of <see cref="DynamicRTree{TItem}"/>,
    /// the instance could be one of a number of various R-Tree 
    /// variants, such as R*Tree or R+Tree.
    /// </remarks>
    public class DynamicRTree<TItem> : RTree<TItem>,
                                       IUpdatableSpatialIndex<IExtents, TItem>,
                                       ISerializable
            where TItem : IBoundable<IExtents>
    {
        #region Fields

        private static readonly BinaryFormatter _keyFormatter = new BinaryFormatter();
        private readonly INodeSplitStrategy<IExtents, TItem> _nodeSplitStrategy;
        private readonly IItemInsertStrategy<IExtents, TItem> _insertStrategy;
        private readonly DynamicRTreeBalanceHeuristic _heuristic = new DynamicRTreeBalanceHeuristic(4, 10, UInt16.MaxValue);

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new updatable R-Tree instance.
        /// </summary>
        /// <param name="geoFactory">
        /// An <see cref="IGeometryFactory"/> for creating extent instances.
        /// </param>
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
            IGeometryFactory geoFactory,
            IItemInsertStrategy<IExtents, TItem> insertStrategy,
            INodeSplitStrategy<IExtents, TItem> nodeSplitStrategy,
            DynamicRTreeBalanceHeuristic heuristic)
            : base(geoFactory)
        {
            if (insertStrategy == null)
            {
                throw new ArgumentNullException("insertStrategy");
            }

            if (nodeSplitStrategy == null)
            {
                throw new ArgumentNullException("nodeSplitStrategy");
            }

            if (heuristic == null)
            {
                throw new ArgumentNullException("heuristic");
            }

            _insertStrategy = insertStrategy;
            _nodeSplitStrategy = nodeSplitStrategy;
            _heuristic = heuristic;
        }

        /// <summary>
        /// This constructor is used internally 
        /// for loading a tree from a stream.
        /// </summary>
        protected DynamicRTree() : base(null) { }

        protected DynamicRTree(SerializationInfo info, StreamingContext context)
            : base(null)
        {
            Byte[] data = (Byte[])info.GetValue("data", typeof(Byte[]));
            MemoryStream buffer = new MemoryStream(data);
            throw new NotImplementedException();
            //DynamicRTree<TItem> tree = FromStream(buffer);
            //Root = tree.Root;
        }

        #endregion

        /// <summary>
        /// The strategy used to insert new entries into the index.
        /// </summary>
        public IItemInsertStrategy<IExtents, TItem> ItemInsertStrategy
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
        /// Inserts a node into the tree using <see cref="ItemInsertStrategy"/>.
        /// </summary>
        /// <param name="item">The item to insert into the index.</param>
        public override void Insert(TItem item)
        {
            ISpatialIndexNode<IExtents, TItem> newSiblingFromSplit;

            //printItemCount();

            ItemInsertStrategy.Insert(item.Bounds, item, Root,
                                      _nodeSplitStrategy, Heuristic,
                                      out newSiblingFromSplit);

            if (newSiblingFromSplit == null)
            {
                return;
            }

            //Debug.Print("Node was split.");

            // Add the newly split sibling
            if (newSiblingFromSplit.IsLeaf)
            {
                if (Root.IsLeaf)    // handle the first splitting of the root node.
                {
                    RTreeNode<TItem> oldRoot = Root as RTreeNode<TItem>;
                    Root = CreateNode(0);
                    Root.Add(oldRoot);
                }

                Root.Add(newSiblingFromSplit);
            }
            else // Came from a root split
            {
                ISpatialIndexNode<IExtents, TItem> oldRoot = Root;
                Root = CreateNode(0);
                Root.Add(oldRoot);
                Root.Add(newSiblingFromSplit);
            }
        }

        /// <summary>
        /// Removes an item from the index.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public override Boolean Remove(TItem item)
        {
            ISpatialIndexNode<IExtents, TItem> itemNode = findNodeForItem(item, Root);

            bool removed = itemNode != null && itemNode.Remove(item);
            if (itemNode.IsPrunable)
            {
                ISpatialIndexNode<IExtents, TItem> parent = findParentNode(itemNode, Root);
                parent.Remove(itemNode);
            }

            return removed;
        }

        private static ISpatialIndexNode<IExtents, TItem> findParentNode(ISpatialIndexNode<IExtents, TItem> lookFor,
                                                                         ISpatialIndexNode<IExtents, TItem> searchNode)
        {
            foreach (ISpatialIndexNode<IExtents, TItem> node in searchNode.SubNodes)
            {
                if (node.Equals(lookFor))
                {
                    return searchNode;
                }
            }

            IExtents itemBounds = lookFor.Bounds;

            // TODO: I think a breadth-first search would be more efficient here
            foreach (ISpatialIndexNode<IExtents, TItem> child in searchNode.SubNodes)
            {
                if (child.Intersects(itemBounds))
                {
                    ISpatialIndexNode<IExtents, TItem> found = findParentNode(lookFor, child);

                    if (found != null)
                    {
                        return found;
                    }
                }
            }

            return null;
        }

        private static ISpatialIndexNode<IExtents, TItem> findNodeForItem(TItem item,
                                                                          ISpatialIndexNode<IExtents, TItem> node)
        {
            foreach (TItem nodeItem in node.Items)
            {
                if (item.Equals(nodeItem))
                {
                    return node;
                }
            }

            IExtents itemBounds = item.Bounds;

            // TODO: I think a breadth-first search would be more efficient here
            foreach (ISpatialIndexNode<IExtents, TItem> child in node.SubNodes)
            {
                if (child.Intersects(itemBounds))
                {
                    ISpatialIndexNode<IExtents, TItem> found = findNodeForItem(item, child);

                    if (found != null)
                    {
                        return found;
                    }
                }
            }

            return null;
        }

        public Boolean Remove(IExtents bounds, TItem item)
        {
            throw new NotImplementedException();
        }

        #region Read/Write index to/from a file

        private static readonly Version IndexFileVersion = new Version(2, 0, 0, 0);

        /// <summary>
        /// Loads an R-Tree structure from a stream.
        /// </summary>
        /// <param name="data">Stream which holds the R-Tree spatial index.</param>
        /// <returns>
        /// A <see cref="DynamicRTree{TItem}"/> instance which had been 
        /// persisted to the <see cref="System.IO.Stream">stream</see>.
        /// </returns>
        public static DynamicRTree<TItem> FromStream(Stream data, IGeometryFactory geoFactory)
        {
            throw new NotImplementedException();
            DynamicRTree<TItem> tree = new DynamicRTree<TItem>();

            using (BinaryReader br = new BinaryReader(data))
            {
                Version fileVersion = new Version(br.ReadInt32(), br.ReadInt32(), br.ReadInt32(), br.ReadInt32());

                if (fileVersion != IndexFileVersion) //Check fileindex version
                {
                    throw new ObsoleteIndexFileFormatException(IndexFileVersion, fileVersion,
                                                               "Invalid index file version. Please rebuild the spatial index by deleting the index.");
                }

                UInt32 keyLength = (UInt32)Marshal.SizeOf(typeof(TItem));

                using (MemoryStream keyBuffer = new MemoryStream((Int32)keyLength))
                {
                    // tree.Root = readNode(tree, br, keyBuffer, keyLength);
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
            throw new NotImplementedException();

            BinaryWriter writer = new BinaryWriter(stream);

            //Save index version
            writer.Write(IndexFileVersion.Major);
            writer.Write(IndexFileVersion.Minor);
            writer.Write(IndexFileVersion.Build);
            writer.Write(IndexFileVersion.Revision);

            UInt32 keyLength = (UInt32)Marshal.SizeOf(typeof(TItem));

            using (MemoryStream keyBuffer = new MemoryStream((Int32)keyLength))
            {
                // saveNode(Root, writer, keyBuffer, keyLength);
            }
        }

        ///// <summary>
        ///// Reads a node from a stream recursively.
        ///// </summary>
        ///// <param name="tree">R-Tree instance.</param>
        ///// <param name="reader">Binary reader reference.</param>
        ///// <param name="keyBuffer">Buffer to serialze key data into.</param>
        ///// <param name="keyLength">Data length of serialized key.</param>
        //private static ISpatialIndexNode<IExtents, TItem> readNode(DynamicRTree<TItem> tree, BinaryReader reader, 
        //    MemoryStream keyBuffer, UInt32 keyLength, IGeometryFactory geoFactory)
        //{
        //    if (reader.BaseStream.Position == reader.BaseStream.Length)
        //    {
        //        return null;
        //    }

        //    ISpatialIndexNode<IExtents, TItem> node;
        //    Boolean isLeaf = reader.ReadBoolean();

        //    ICoordinateFactory coordFactory = geoFactory.CoordinateFactory;

        //    ICoordinate min = coordFactory.Create(
        //        reader.ReadDouble(),
        //        reader.ReadDouble());

        //    ICoordinate max = coordFactory.Create(
        //        reader.ReadDouble(),
        //        reader.ReadDouble());

        //    IExtents bounds = geoFactory.CreateExtents(min, max);

        //    node = tree.CreateNode(bounds);

        //    if (isLeaf)
        //    {
        //        Int32 featureCount = reader.ReadInt32();

        //        for (Int32 i = 0; i < featureCount; i++)
        //        {
        //            TItem entry = new RTreeIndexEntry<TItem>();

        //            entry.Bounds = new BoundingBox(
        //                                        reader.ReadDouble(), 
        //                                        reader.ReadDouble(), 
        //                                        reader.ReadDouble(), 
        //                                        reader.ReadDouble());

        //            keyBuffer.Position = 0;
        //            keyBuffer.Write(reader.ReadBytes((Int32) keyLength), 0, (Int32) keyLength);
        //            entry.Value = (TItem) _keyFormatter.Deserialize(keyBuffer);

        //            leaf.Add(entry);
        //        }
        //    }
        //    else
        //    {
        //        RTreeBranchNode<TItem> index = node as RTreeBranchNode<TItem>;
        //        Debug.Assert(index != null);

        //        UInt32 childNodes = reader.ReadUInt32();

        //        for (Int32 c = 0; c < childNodes; c++)
        //        {
        //            ISpatialIndexNode child = readNode(tree, reader, keyBuffer, keyLength);

        //            if (child != null)
        //            {
        //                index.Add(child);
        //            }
        //        }
        //    }

        //    return node;
        //}

        ///// <summary>
        ///// Saves a node to a stream recursively.
        ///// </summary>
        ///// <param name="node">Node to save.</param>
        ///// <param name="writer">BinaryWriter to format values and write to stream.</param>
        ///// <param name="keyBuffer">Buffer used to serialize key value.</param>
        ///// <param name="keyLength">Number of bytes in the key's memory representation.</param>
        //private static void saveNode(ISpatialIndexNode node, BinaryWriter writer, MemoryStream keyBuffer, UInt32 keyLength)
        //{
        //    //Write node boundingbox
        //    writer.Write(node.Bounds.Left);
        //    writer.Write(node.Bounds.Bottom);
        //    writer.Write(node.Bounds.Right);
        //    writer.Write(node.Bounds.Top);
        //    writer.Write(node is RTreeLeafNode<TItem>);

        //    if (node is RTreeLeafNode<TItem>)
        //    {
        //        RTreeLeafNode<TItem> leaf = node as RTreeLeafNode<TItem>;
        //        writer.Write(leaf.Items.Count); //Write number of features at node

        //        if (keyBuffer.Capacity != keyLength)
        //        {
        //            keyBuffer.SetLength(keyLength);
        //            keyBuffer.Capacity = (Int32) keyLength;
        //        }

        //        foreach (RTreeIndexEntry<TItem> entry in leaf.Items) //Write each featurebox
        //        {
        //            writer.Write(entry.Bounds.Left);
        //            writer.Write(entry.Bounds.Bottom);
        //            writer.Write(entry.Bounds.Right);
        //            writer.Write(entry.Bounds.Top);

        //            keyBuffer.Position = 0;
        //            _keyFormatter.Serialize(keyBuffer, entry.Value);
        //            writer.Write(keyBuffer.GetBuffer());
        //        }
        //    }
        //    else if (node is RTreeBranchNode<TItem>) //Save next node
        //    {
        //        foreach (ISpatialIndexNode child in (node as RTreeBranchNode<TItem>).Items)
        //        {
        //            saveNode(child, writer, keyBuffer, keyLength);
        //        }
        //    }
        //}

        #endregion

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            MemoryStream buffer = new MemoryStream();
            SaveIndex(buffer);
            info.AddValue("data", buffer.ToArray());
        }

        #endregion

        [Conditional("DEBUG")]
        private void printItemCount()
        {
            Int32 count = Root.TotalItemCount;

            if (count % 100 == 0)
            {
                Debug.Print("Inserting item # {0}", count);
            }
        }
    }
}