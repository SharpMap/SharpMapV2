using System;
using System.Collections.Generic;
using GeoAPI.Coordinates;
using NPack;

namespace SharpMap.Tests
{
    public class EpsilonComparer : IComparer<Double>, IComparer<ICoordinate>
    {
        public static readonly Double DefaultEpsilon = 0.0005;
        public static EpsilonComparer Default = new EpsilonComparer(DefaultEpsilon);
        private readonly Double _epsilon;

        private EpsilonComparer(Double epsilon)
        {
            _epsilon = epsilon;
        }

        public Int32 Compare(Double x, Double y)
        {
            Double difference = Math.Abs(x) - Math.Abs(y);

            Int32 result = _epsilon.CompareTo(difference);

            return result >= 0 ? 0 : x.CompareTo(y);
        }

        public Int32 Compare(ICoordinate x, ICoordinate y)
        {
            DoubleComponent[] xComponents = x.Components;
            DoubleComponent[] yComponents = y.Components;

            Int32 result = xComponents.Length.CompareTo(yComponents.Length);

            if (result != 0)
            {
                return result;
            }

            for (Int32 i = 0; i < xComponents.Length; i++)
            {
                result = Compare((Double)xComponents[i], (Double)yComponents[i]);

                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        }
    }
}
