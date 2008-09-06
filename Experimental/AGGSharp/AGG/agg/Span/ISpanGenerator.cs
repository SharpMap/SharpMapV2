using System;
using AGG.Color;
using NPack.Interfaces;

namespace AGG.Span
{
    public interface ISpanGenerator<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable
    {
        void Prepare();
        unsafe void Generate(RGBA_Bytes* span, int x, int y, uint len);
    };
}
