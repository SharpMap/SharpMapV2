using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NPack.Interfaces;
using NPack;
namespace AGG
{
    public static class RectTranslationUtility
    {
        public static void Transform<T>(this IAffineTransformMatrix<T> m, ref RectDouble<T> rect)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            m.Transform(ref rect.x1, ref rect.y1);
            m.Transform(ref rect.x2, ref rect.y2);
        }
    }
}
