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
using GeoAPI.Geometries;

namespace SharpMap.Indexing
{
    /// <summary>
    /// Interface for spatial index nodes.
    /// </summary>
    public interface ISpatialIndexNode
    {
        /// <summary>
        /// Gets the BoundingBox for this node, which minimally bounds all items.
        /// </summary>
        BoundingBox BoundingBox { get; }

        /// <summary>
        /// Node identifier.
        /// </summary>
        UInt32? NodeId { get; }
    }

    /// <summary>
    /// Interface for a searchable spatial index containing 
    /// <typeparamref name="TEntry"/> instances.
    /// </summary>
    /// <typeparam name="TEntry">The type of the index entries.</typeparam>
    public interface ISearchableSpatialIndex<TEntry>
    {
        /// <summary>
        /// Returns a set of entries which intersect the <paramref name="bounds"/>.
        /// </summary>
        /// <param name="bounds">
        /// The bounds to search the index for intersection.
        /// </param>
        /// <returns>
        /// A set of entries which intersect the given <paramref name="bounds"/>.
        /// </returns>
        IEnumerable<TEntry> Search(BoundingBox bounds);

        /// <summary>
        /// Returns a set of entries which intersect the <paramref name="geometry"/>.
        /// </summary>
        /// <param name="geometry">
        /// The bounds to search the index for intersection.
        /// </param>
        /// <returns>
        /// A set of entries which intersect the given <paramref name="geometry"/>.
        /// </returns>
        IEnumerable<TEntry> Search(Geometry geometry);
    }

    /// <summary>
    /// Interface for an insertable spatial index containing <typeparamref name="TEntry"/> instances.
    /// </summary>
    /// <typeparam name="TEntry">The type of the index entries.</typeparam>
    public interface IUpdatableSpatialIndex<TEntry>
    {
        /// <summary>
        /// Inserts an entry into the index.
        /// </summary>
        /// <param name="entry">The entry to insert.</param>
        void Insert(TEntry entry);

        /// <summary>
        /// Removes an entry from the index.
        /// </summary>
        /// <param name="entry">The entry to remove.</param>
        void Remove(TEntry entry);
    }

    /// <summary>
    /// Interface for a node splitting strategy used by an updateable spatial index.
    /// </summary>
    public interface INodeSplitStrategy
    {
        /// <summary>
        /// Splits a given node based on the given <paramref name="heuristic"/>.
        /// </summary>
        /// <param name="node">The node to split.</param>
        /// <param name="heuristic">The heuristic used to compute the node split.</param>
        /// <returns>The new node split off from <paramref name="node"/>.</returns>
        ISpatialIndexNode SplitNode(ISpatialIndexNode node, IndexBalanceHeuristic heuristic);
    }

    /// <summary>
    /// Interface for a strategy to insert new entries into an updatable spatial index.
    /// </summary>
    /// <typeparam name="TEntry">The type of the index entries.</typeparam>
    public interface IEntryInsertStrategy<TEntry>
    {
        /// <summary>
        /// Inserts a new entry into the spatial index.
        /// </summary>
        /// <param name="entry">The entry to insert.</param>
        /// <param name="node">The next node at which to try the insert.</param>
        /// <param name="nodeSplitStrategy">An <see cref="INodeSplitStrategy"/> used to split the node if it overflows.</param>
        /// <param name="heuristic">The heuristic used to balance the insert or compute the node split.</param>
        /// <param name="newSiblingFromSplit">A possible new node from a node-split.</param>
        void InsertEntry(TEntry entry, ISpatialIndexNode node, INodeSplitStrategy nodeSplitStrategy,
                         IndexBalanceHeuristic heuristic, out ISpatialIndexNode newSiblingFromSplit);
    }

    /// <summary>
    /// Interface for a strategy used to restructure a spatial index.
    /// </summary>
    public interface IIndexRestructureStrategy
    {
        /// <summary>
        /// Restructures a node in the index.
        /// </summary>
        /// <param name="node">The spatial index node to restructure.</param>
        void RestructureNode(ISpatialIndexNode node);
    }
}