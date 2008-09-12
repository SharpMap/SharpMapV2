
/*
 *	Portions of this file are  © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 *
 *  Original notices below.
 * 
 */

namespace AGG
{
    //public class Vector2D
    //    : Vector<DoubleComponent>
    //{
    //    public Vector2D() : base(2) { }

    //    public Vector2D(double x, double y)
    //        : base(new DoubleComponent[] { x, y, 0 })
    //    {

    //    }

    //    public DoubleComponent x
    //    {
    //        get
    //        {
    //            return this[0];
    //        }
    //        set
    //        {
    //            this[0] = value;
    //        }
    //    }

    //    public DoubleComponent y
    //    {
    //        get
    //        {
    //            return this[1];
    //        }
    //        set
    //        {
    //            this[1] = value;
    //        }
    //    }
    //}

    //public struct Vector2D
    //{
    //    public double x, y;

    //    public Vector2D(double newX, double newY)
    //    {
    //        x = newX;
    //        y = newY;
    //    }

    //    public Vector2D(float newX, float newY)
    //        : this((double)newX, (double)newY)
    //    {
    //    }

    //    public void Set(double inX, double inY)
    //    {
    //        x = inX;
    //        y = inY;
    //    }

    //    //bool operator==(Vector2D B);
    //    //bool operator!=(Vector2D B);
    //    //double operator[](int Index) { return Index == 0 ? x : y; }
    //    //double operator[](int Index) { return Index == 0 ? x : y; }

    //    static public Vector2D operator +(Vector2D A, Vector2D B)
    //    {
    //        Vector2D temp =  MatrixFactory<T>.CreateVector2D ();
    //        temp.x = A.x + B.x;
    //        temp.y = A.y + B.y;
    //        return temp;
    //    }

    //    static public Vector2D operator -(Vector2D A, Vector2D B)
    //    {
    //        Vector2D temp =  MatrixFactory<T>.CreateVector2D ();
    //        temp.x = A.x - B.x;
    //        temp.y = A.y - B.y;
    //        return temp;
    //    }

    //    static public Vector2D operator *(Vector2D A, Vector2D B)
    //    {
    //        Vector2D temp =  MatrixFactory<T>.CreateVector2D ();
    //        temp.x = A.x * B.x;
    //        temp.y = A.y * B.y;
    //        return temp;
    //    }

    //    static public Vector2D operator *(Vector2D A, double B)
    //    {
    //        Vector2D temp =  MatrixFactory<T>.CreateVector2D ();
    //        temp.x = A.x * B;
    //        temp.y = A.y * B;
    //        return temp;
    //    }

    //    static public Vector2D operator *(double B, Vector2D A)
    //    {
    //        Vector2D temp =  MatrixFactory<T>.CreateVector2D ();
    //        temp.x = A.x * B;
    //        temp.y = A.y * B;
    //        return temp;
    //    }

    //    static public Vector2D operator *(Vector2D A, float B)
    //    {
    //        Vector2D temp =  MatrixFactory<T>.CreateVector2D ();
    //        temp.x = A.x * (double)B;
    //        temp.y = A.y * (double)B;
    //        return temp;
    //    }

    //    static public Vector2D operator *(float B, Vector2D A)
    //    {
    //        Vector2D temp =  MatrixFactory<T>.CreateVector2D ();
    //        temp.x = A.x * (double)B;
    //        temp.y = A.y * (double)B;
    //        return temp;
    //    }

    //    static public Vector2D operator /(Vector2D A, Vector2D B)
    //    {
    //        Vector2D temp =  MatrixFactory<T>.CreateVector2D ();
    //        temp.x = A.x / B.x;
    //        temp.y = A.y / B.y;
    //        return temp;
    //    }

    //    static public Vector2D operator /(Vector2D A, double B)
    //    {
    //        Vector2D temp =  MatrixFactory<T>.CreateVector2D ();
    //        temp.x = A.x / B;
    //        temp.y = A.y / B;
    //        return temp;
    //    }

    //    static public Vector2D operator /(double B, Vector2D A)
    //    {
    //        Vector2D temp =  MatrixFactory<T>.CreateVector2D ();
    //        temp.x = A.x / B;
    //        temp.y = A.y / B;
    //        return temp;
    //    }

    //    // are they the same within the error Value?
    //    public bool Equals(Vector2D OtherVector, double ErrorValue)
    //    {
    //        if ((x < OtherVector.x + ErrorValue && x > OtherVector.x - ErrorValue) &&
    //            (y < OtherVector.y + ErrorValue && y > OtherVector.y - ErrorValue))
    //        {
    //            return true;
    //        }

    //        return false;
    //    }

    //    public override bool Equals(System.Object obj)
    //    {
    //        // If parameter is null return false.
    //        if (obj == null)
    //        {
    //            return false;
    //        }

    //        // If parameter cannot be cast to Point return false.
    //        Vector2D p = (Vector2D)obj;
    //        if ((System.Object)p == null)
    //        {
    //            return false;
    //        }

    //        // Return true if the fields match:
    //        return (x == p.x) && (y == p.y);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return x.GetHashCode() ^ y.GetHashCode();
    //    }

    //    public static bool operator ==(Vector2D a, Vector2D b)
    //    {
    //        return a.Equals(b);
    //    }

    //    public static bool operator !=(Vector2D a, Vector2D b)
    //    {
    //        return !a.Equals(b);
    //    }

    //    public Vector2D GetPerpendicular()
    //    {
    //        Vector2D temp =  MatrixFactory<T>.CreateVector2D (y, -x);

    //        return temp;
    //    }

    //    public Vector2D GetPerpendicularNormal()
    //    {
    //        Vector2D Perpendicular = GetPerpendicular();
    //        Perpendicular.Normalize();
    //        return Perpendicular;
    //    }

    //    public double GetLength()
    //    {
    //        return (double)System.Math.Sqrt((x * x) + (y * y));
    //    }

    //    public double GetLengthSquared()
    //    {
    //        return Dot(this);
    //    }

    //    public static double GetDistanceBetween(Vector2D A, Vector2D B)
    //    {
    //        return (double)System.Math.Sqrt(GetDistanceBetweenSquared(A, B));
    //    }

    //    public static double GetDistanceBetweenSquared(Vector2D A, Vector2D B)
    //    {
    //        return ((A.x - B.x) * (A.x - B.x) + (A.y - B.y) * (A.y - B.y));
    //    }

    //    public double GetSquaredDistanceTo(Vector2D Other)
    //    {
    //        return ((x - Other.x) * (x - Other.x) + (y - Other.y) * (y - Other.y));
    //    }

    //    static public double Range0To2PI(double Value)
    //    {
    //        if (Value < 0)
    //        {
    //            Value += 2 * (double)System.Math.PI;
    //        }

    //        if (Value >= 2 * (double)System.Math.PI)
    //        {
    //            Value -= 2 * (double)System.Math.PI;
    //        }

    //        if (Value < 0 || Value > 2 * System.Math.PI) throw new Exception("Value >= 0 && Value <= 2 * PI");

    //        return Value;
    //    }

    //    static public double GetDeltaAngle(double StartAngle, double EndAngle)
    //    {
    //        if (StartAngle != Range0To2PI(StartAngle)) throw new Exception("StartAngle == Range0To2PI(StartAngle)");
    //        if (EndAngle != Range0To2PI(EndAngle)) throw new Exception("EndAngle   == Range0To2PI(EndAngle)");

    //        double DeltaAngle = EndAngle - StartAngle;
    //        if (DeltaAngle > System.Math.PI)
    //        {
    //            DeltaAngle -= 2 * (double)System.Math.PI;
    //        }

    //        if (DeltaAngle < -System.Math.PI)
    //        {
    //            DeltaAngle += 2 * (double)System.Math.PI;
    //        }

    //        return DeltaAngle;
    //    }

    //    public double GetAngle0To2PI()
    //    {
    //        return (double)Range0To2PI((double)System.Math.Atan2((double)y, (double)x));
    //    }

    //    public double GetDeltaAngle(Vector2D A)
    //    {
    //        return (double)GetDeltaAngle(GetAngle0To2PI(), A.GetAngle0To2PI());
    //    }

    //    public void Normalize()
    //    {
    //        double Length;

    //        Length = GetLength();

    //        if (Length == 0) throw new Exception("Length != 0.f");

    //        if (Length != 0.0f)
    //        {
    //            double InversLength = 1.0f / Length;
    //            x *= InversLength;
    //            y *= InversLength;
    //        }
    //    }

    //    public void Normalize(double Length)
    //    {
    //        if (Length == 0) throw new Exception("Length == 0.f");

    //        if (Length != 0.0f)
    //        {
    //            double InversLength = 1.0f / Length;
    //            x *= InversLength;
    //            y *= InversLength;
    //        }
    //    }

    //    public double NormalizeAndReturnLength()
    //    {
    //        double Length;

    //        Length = GetLength();

    //        if (Length != 0.0f)
    //        {
    //            double InversLength = 1.0f / Length;
    //            x *= InversLength;
    //            y *= InversLength;
    //        }

    //        return Length;
    //    }

    //    public void Rotate(double Radians)
    //    {
    //        Vector2D Temp;

    //        double Cos, Sin;

    //        Cos = (double)System.Math.Cos(Radians);
    //        Sin = (double)System.Math.Sin(Radians);

    //        Temp.x = x * Cos + y * Sin;
    //        Temp.y = y * Cos + x * Sin;

    //        x = Temp.x;
    //        y = Temp.y;
    //    }

    //    public void Zero()
    //    {
    //        x = (double)0;
    //        y = (double)0;
    //    }

    //    public void Negate()
    //    {
    //        x = -x;
    //        y = -y;
    //    }

    //    public double Dot(Vector2D B)
    //    {
    //        return (x * B.x + y * B.y);
    //    }

    //    public double Cross(Vector2D B)
    //    {
    //        return x * B.y - y * B.x;
    //    }
    //};
}

//namespace NUnitReflexive
//{
//    [TestFixture]
//    public class Vector2DTests
//    {
//        [Test]
//        public void ArithmaticOperations()
//        {
//            Vector2D Point1 =  MatrixFactory<T>.CreateVector2D ();
//            Point1.Set(1, 1);

//            Vector2D Point2 =  MatrixFactory<T>.CreateVector2D ();
//            Point2.Set(2, 2);

//            Vector2D Point3 = Point1 + Point2;
//            Assert.IsTrue(Point3 ==  MatrixFactory<T>.CreateVector2D (3, 3));

//            Point3 = Point1 - Point2;
//            Assert.IsTrue(Point3 ==  MatrixFactory<T>.CreateVector2D (-1, -1));

//            Point3 += Point1;
//            Assert.IsTrue(Point3 ==  MatrixFactory<T>.CreateVector2D (0, 0));

//            Point3 += Point2;
//            Assert.IsTrue(Point3 ==  MatrixFactory<T>.CreateVector2D (2, 2));

//            Point3 *= 6;
//            Assert.IsTrue(Point3 ==  MatrixFactory<T>.CreateVector2D (12, 12));

//            Vector2D InlineOpLeftSide =  MatrixFactory<T>.CreateVector2D (5, -3);
//            Vector2D InlineOpRightSide =  MatrixFactory<T>.CreateVector2D (-5, 4);
//            Assert.IsTrue(InlineOpLeftSide + InlineOpRightSide ==  MatrixFactory<T>.CreateVector2D (.0f, 1));

//            Assert.IsTrue(InlineOpLeftSide - InlineOpRightSide ==  MatrixFactory<T>.CreateVector2D (10.0f, -7));
//        }

//        [Test]
//        public void GetLengthAndNormalize()
//        {
//            Vector2D Point3 =  MatrixFactory<T>.CreateVector2D (3, -4);
//            Assert.IsTrue(Point3.GetLength() > 4.999f && Point3.GetLength() < 5.001f);

//            Point3.Normalize();
//            Assert.IsTrue(Point3.GetLength() > 0.99f && Point3.GetLength() < 1.01f);
//        }

//        [Test]
//        public void ScalerOperations()
//        {
//            Vector2D ScalarMultiplicationArgument =  MatrixFactory<T>.CreateVector2D (5.0f, 4.0f);
//            Assert.IsTrue(ScalarMultiplicationArgument * -.5 ==  MatrixFactory<T>.CreateVector2D (-2.5f, -2));
//            Assert.IsTrue(ScalarMultiplicationArgument / 2 ==  MatrixFactory<T>.CreateVector2D (2.5f, 2));
//            Assert.IsTrue(2 / ScalarMultiplicationArgument ==  MatrixFactory<T>.CreateVector2D (2.5f, 2));
//            Assert.IsTrue(5 * ScalarMultiplicationArgument ==  MatrixFactory<T>.CreateVector2D (25, 20));
//        }

//        [Test]
//        public void Rotate()
//        {
//            Vector2D TestVector =  MatrixFactory<T>.CreateVector2D (1, 0);
//            TestVector.Rotate((double)System.Math.PI / 2);
//            Assert.IsTrue(TestVector.Equals( MatrixFactory<T>.CreateVector2D (0, 1), .001f));
//        }

//        [Test]
//        public void CrossProduct()
//        {
//            Random Rand = new Random();
//            Vector2D TestVector2D1 =  MatrixFactory<T>.CreateVector2D (Rand.NextDouble() * 1000, Rand.NextDouble() * 1000);
//            Vector2D TestVector2D2 =  MatrixFactory<T>.CreateVector2D (Rand.NextDouble() * 1000, Rand.NextDouble() * 1000);
//            double Cross2D = TestVector2D1.Cross(TestVector2D2);

//            Vector3D TestVector3D1 = new Vector3D(TestVector2D1.x, TestVector2D1.y, 0);
//            Vector3D TestVector3D2 = new Vector3D(TestVector2D2.x, TestVector2D2.y, 0);
//            Vector3D Cross3D = TestVector3D1.Cross(TestVector3D2);

//            Assert.IsTrue(Cross3D.z == Cross2D);
//        }

//        [Test]
//        public void DotProduct()
//        {
//            Random Rand = new Random();
//            Vector2D TestVector2D1 =  MatrixFactory<T>.CreateVector2D (Rand.NextDouble() * 1000, Rand.NextDouble() * 1000);
//            Vector2D TestVector2D2 =  MatrixFactory<T>.CreateVector2D (Rand.NextDouble() * 1000, Rand.NextDouble() * 1000);
//            double Cross2D = TestVector2D1.Dot(TestVector2D2);

//            Vector3D TestVector3D1 = new Vector3D(TestVector2D1.x, TestVector2D1.y, 0);
//            Vector3D TestVector3D2 = new Vector3D(TestVector2D2.x, TestVector2D2.y, 0);
//            double Cross3D = TestVector3D1.Dot(TestVector3D2);

//            Assert.IsTrue(Cross3D == Cross2D);
//        }

//        [Test]
//        public void LengthAndDistance()
//        {
//            Random Rand = new Random();
//            Vector2D Test1 =  MatrixFactory<T>.CreateVector2D (Rand.NextDouble() * 1000, Rand.NextDouble() * 1000);
//            Vector2D Test2 =  MatrixFactory<T>.CreateVector2D (Rand.NextDouble() * 1000, Rand.NextDouble() * 1000);
//            Vector2D Test3 = Test1 + Test2;
//            double Distance1 = Test2.GetLength();
//            double Distance2 = Vector2D.GetDistanceBetween(Test1, Test3);

//            Assert.IsTrue(Distance1 < Distance2 + .001f && Distance1 > Distance2 - .001f);
//        }
//    }
//}
