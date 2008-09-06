using System;
using NPack.Interfaces;

namespace AGG.agg_3d
{
    public class Glu<T>
        where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
    {
        public static void gluPerspective(T FieldOfViewYDeg, T XOverYAspectRatio, T zNear, T zFar)
        {
            T xmin, xmax, ymin, ymax;

            //ymax = (double)(zNear * (double)System.Math.Tan(FieldOfViewYDeg * System.Math.PI / 360.0));
            //ymin = -ymax;
            //xmin = (double)(ymin * XOverYAspectRatio);
            //xmax = (double)(ymax * XOverYAspectRatio);

            ymax = zNear.Multiply(FieldOfViewYDeg.Multiply(System.Math.PI / 360.0).Tan());
            ymin = ymax.Negative();
            xmin = ymin.Multiply(XOverYAspectRatio);
            xmax = ymax.Multiply(XOverYAspectRatio);


            Gl<T>.glFrustum(xmin, xmax, ymin, ymax, zNear, zFar);
        }
    };
}
