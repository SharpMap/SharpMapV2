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
using System.Diagnostics;
using System.Drawing;
using System.Text;

using SharpMap.Indexing.QuadTree;
using SharpMap.Geometries;
using GeoPoint = SharpMap.Geometries.Point;

namespace SharpMap.Presentation.WinForms
{
    internal class ImageTileCache : LinearDynamicQuadTree<Bitmap>
    {
		/// <summary>
		/// Creates a new ImageTileCache.
		/// </summary>
		/// <param name="initialBounds">The extents for the entire index. May grow if tiles are added which exceed these bounds.</param>
		public ImageTileCache(BoundingBox initialBounds)
		{
			// Set the bounds of the index
			BoundingBox = initialBounds;
		}

		public void Add(BoundingBox tileBounds, Bitmap tile)
		{
			QuadTreeNode<Bitmap> tileNode = new QuadTreeNode<Bitmap>();
			tileNode.Value = tile;
#warning This doesn't compile in Orcas B2, but it does in VS2005
			//tileNode.BoundingBox = tileBounds;
			Add(tileNode);
		}

		public override IEnumerable<Bitmap> Search(BoundingBox searchBounds)
		{
			return recursiveNodeSearch(searchBounds, this);
		}

		private IEnumerable<Bitmap> recursiveNodeSearch(BoundingBox searchBounds, QuadTreeNode<Bitmap> node)
		{
			// If search bounds are only partially by node bounds, then return node's value,
			// since successive searches will be incomplete.
			if (node.BoundingBox.Intersects(searchBounds) && !node.BoundingBox.Contains(searchBounds))
			{
				yield return node.Value;
				yield break;
			}

			// The node must cover the search bounds, so see if it has more than 
			// one child node which intersects the search bounds. If so, then that must 
			// be the best resolution tiles to satisfy the search request, as the bitmaps can 
			// be resampled upward. If there is only one child, recurse downward on that child.
			int[] intersectNodes = new int[4];
			int intersectNodeIndex = -1;

			for (int nodeIndex = 0; nodeIndex < node.Items.Count; nodeIndex++)
			{
				QuadTreeNode<Bitmap> testNode = Items[nodeIndex];

                if (testNode.BoundingBox.Intersects(searchBounds))
				{
					intersectNodes[++intersectNodeIndex] = nodeIndex;
				}
			}

			// This should not be possible with the first bounds-check in this function
			Debug.Assert(intersectNodeIndex > -1);

			// If more than one node intersects the searchBounds, we must be at the correct resolution.
			// Otherwise, keep searching down the index.
			if (intersectNodeIndex > 0)
			{
				foreach (int nodeIndex in intersectNodes)
				{
					yield return node.Items[nodeIndex].Value;
				}
			}
			else
			{
				foreach (Bitmap tile in recursiveNodeSearch(searchBounds, node.Items[intersectNodes[intersectNodeIndex]]))
				{
					yield return tile;
				}
			}
		}
    }
}
