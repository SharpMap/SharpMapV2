using System;
using System.Collections.Generic;

namespace SharpMap.Data.Providers.GdalPreview
{
    public class RasterTransparency
    {
        private struct TransparencyThreeComponentPixel
        {
            public Double Red, Green, Blue;
            internal Double _percentInternal;
            public Double Percent
            {
                get { return 100d*(1d - _percentInternal); }
                set
                {
                    if (0d <= value && value <= 100d)
                        _percentInternal = 1d - (value/100d);
                }
            }

        }

        public struct TransparencyOneComponentPixel
        {
            public Double Value;
            internal Double _percentInternal;
            public Double Percent
            {
                get { return 100d * (1d - _percentInternal); }
                set
                {
                    if (0d <= value && value <= 100d)
                        _percentInternal = 1d - (value / 100d);
                }
            }
        }

        private readonly List<TransparencyOneComponentPixel> _one;
        private readonly List<TransparencyThreeComponentPixel> _three;

        public RasterTransparency()
        {
            _one = new List<TransparencyOneComponentPixel>();
            _three = new List<TransparencyThreeComponentPixel>();
        }

        public Int32 Alpha(Double pixelValue, Int32 globalTransparency)
        {
            if (Double.IsNaN(pixelValue))
                return 0;

            foreach (TransparencyOneComponentPixel oneComponentPixel in _one)
            {
                if (oneComponentPixel.Value == pixelValue)
                    return Convert.ToInt32(globalTransparency * (oneComponentPixel._percentInternal));
            }
            return globalTransparency;
        }

        public Int32 Alpha(Double redValue, Double greenValue, Double blueValue, Int32 globalTransparency)
        {
            if (Double.IsNaN(redValue) ||
                Double.IsNaN(greenValue) ||
                Double.IsNaN(blueValue)) return 0;

            foreach (TransparencyThreeComponentPixel threeComponentPixel in _three)
            {
                if (threeComponentPixel.Red == redValue &&
                    threeComponentPixel.Green == greenValue &&
                    threeComponentPixel.Blue == blueValue)
                {
                    return Convert.ToInt32(globalTransparency * (threeComponentPixel._percentInternal));
                }
            }
            return globalTransparency;
        }

        public void Clear()
        {
            _three.Clear();
            _one.Clear();
        }

        public void Add(Double value, Double percent)
        {
            TransparencyOneComponentPixel ocp = 
                new TransparencyOneComponentPixel {Value = value, Percent = percent};
            if (_one.Contains(ocp)) _one.Remove(ocp);
            _one.Add(ocp);
        }
        public void Add(Double red, Double green, Double blue, Double percent)
        {
            TransparencyThreeComponentPixel tcp = 
                new TransparencyThreeComponentPixel
                  {
                      Red = red,
                      Blue = blue,
                      Green = green,
                      Percent = percent
                  };
            if (_three.Contains(tcp)) _three.Remove(tcp);
            _three.Add(tcp);
        }
    }
}
