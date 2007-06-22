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
			get { throw new Exception("The method or operation is not implemented."); }
		}

		public override object Clone()
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
