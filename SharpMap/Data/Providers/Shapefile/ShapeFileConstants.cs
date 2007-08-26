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

namespace SharpMap.Data.Providers.ShapeFile
{
	internal class ShapeFileConstants
	{
		public const int HeaderSizeBytes = 100;
		public const int HeaderStartCode = 9994;
		public const int VersionCode = 1000;
		public const int ShapeRecordHeaderByteLength = 8;
		public const int IndexRecordByteLength = 8;
		public const int BoundingBoxFieldByteLength = 32;
		public static readonly string IdColumnName = "OID";
	}
}
