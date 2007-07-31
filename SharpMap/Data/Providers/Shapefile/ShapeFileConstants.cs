using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Data.Providers
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
