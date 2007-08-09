using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Layers;
using SharpMap.Geometries;
using SharpMap.Data;

namespace SharpMap.Layers
{
    /// <summary>
    /// A map layer of raster data.
    /// </summary>
    /// <example>
    /// Adding a <see cref="RasterLayer"/> to a map:
    /// </example>
	public class RasterLayer : Layer
	{
        public RasterLayer(IProvider dataSource)
            : base(dataSource)
        {
        }

		public override BoundingBox Envelope
		{
			get { throw new NotImplementedException(); }
		}

		public override object Clone()
		{
			throw new NotImplementedException();
        }

        protected override void OnVisibleRegionChanged()
        {
            throw new NotImplementedException();
        }

        protected override void OnVisibleRegionChanging(BoundingBox value, ref bool cancel)
        {
            throw new NotImplementedException();
        }
	}
}
