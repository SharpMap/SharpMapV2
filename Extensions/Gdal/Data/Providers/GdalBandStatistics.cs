using System;
using OSGeo.GDAL;

namespace SharpMap.Data.Providers
{
    /// <summary>
    /// 
    /// </summary>
    internal class GdalBandStatistics
    {
        public readonly Int32 BandNumber;
        public Double Minimum { get; protected set; }
        public Double Maximum { get; protected set; }
        public Double Range { get; protected set; }
        public Double Mean { get; protected set; }

        internal GdalBandStatistics(Int32 bandNr)
        {
            BandNumber = bandNr;
        }

        internal static GdalBandStatistics GetGdalBandStatistics(Int32 bandNr, Band band, Boolean approximate, Boolean force)
        {
            if (band == null)
                throw new ArgumentNullException("band");

            Double min, max, range, mean;
            band.GetStatistics(approximate ? 1 : 0, force ? 1:0, out min, out max, out range, out mean);
            GdalBandStatistics bs = new GdalBandStatistics(bandNr)
                                        {Minimum = min, Maximum = max, Range = range, Mean = mean};
            return bs;
        }

    }
}