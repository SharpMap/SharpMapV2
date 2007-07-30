using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Data.Providers
{
	internal class ShapeFileConstants
	{
		public static readonly int HeaderSizeBytes = 100;
		public static readonly int HeaderStartCode = 9994;
		public static readonly int VersionCode = 1000;
		public static readonly int ShapeRecordHeaderByteLength = 8;
		public static readonly int IndexRecordByteLength = 8;
		public static readonly int BoundingBoxFieldByteLength = 32;
		public static readonly string IdColumnName = "OID";
	}
}
