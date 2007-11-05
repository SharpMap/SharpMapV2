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

namespace SharpMap.Data.Providers.ShapeFile
{
	/// <summary>
	/// Represents invariant shapefile values, offsets and lengths
	/// derived from the shapefile specification.
	/// </summary>
	internal class ShapeFileConstants
	{
		/// <summary>
		/// Size, in bytes, of the shapefile header region.
		/// </summary>
		public const int HeaderSizeBytes = 100;

		/// <summary>
		/// The first value in any shapefile.
		/// </summary>
		public const int HeaderStartCode = 9994;

		/// <summary>
		/// The version of any valid shapefile.
		/// </summary>
		public const int VersionCode = 1000;

		/// <summary>
		/// The number of bytes in a shapefile record header 
		/// (per-record preamble).
		/// </summary>
		public const int ShapeRecordHeaderByteLength = 8;

		/// <summary>
		/// The number of bytes in a record header (per-record preamble)
		/// in a shapefile's index file.
		/// </summary>
		public const int IndexRecordByteLength = 8;

		/// <summary>
		/// The number of bytes used to store the BoundingBox, or 
		/// extents for the shapefile.
		/// </summary>
		public const int BoundingBoxFieldByteLength = 32;

		/// <summary>
		/// The name given to the row identifier in a ShapeFileProvider.
		/// </summary>
		public static readonly string IdColumnName = "OID";
	}
}
