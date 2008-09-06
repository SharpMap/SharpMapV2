using System;
using NPack.Interfaces;

namespace Reflexive.Math
{
    public static class Helper
    {

        public static double DegToRad(double Deg)
        {
            return Deg / 180 * (double)System.Math.PI;
        }


        public static T DegToRad<T>(T Deg)
              where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return Deg.Divide(M.PI<T>()).Multiply(180);//Deg/ 180 * (double)System.Math.PI;
        }

        public static T DegToRad<T>(double Deg)
            where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable, ICommonNumericalOperations<T>, ITrigonometricOperations<T>
        {
            return M.New<T>(Deg).Divide(M.PI<T>()).Multiply(180);//Deg/ 180 * (double)System.Math.PI;
        }
    }
}
