using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Layers;
using SharpMap.Geometries;

namespace SharpMap.Layers
{
	public class RasterLayer : Layer
	{
		public override BoundingBox Envelope
		{
			get { throw new NotImplementedException(); }
		}

		public override object Clone()
		{
			throw new NotImplementedException();
		}
	}
}
