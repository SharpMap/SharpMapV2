using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public interface IViewVector : ICloneable, IEquatable<IViewVector>, IEnumerable<double>
    {
        double[] Elements { get; }
        double this[int element] { get; }
    }
}
