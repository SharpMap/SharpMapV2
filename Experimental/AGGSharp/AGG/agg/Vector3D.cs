//*********************************************************************************************************************
//
// Author:	 Lars Brubaker
// Started:	 01/24/94 	
// $Modtime: 7/07/99 11:23a $
// $Revision: 5 $
// Filename: Vector3D.cs
//																$NoKeywords $
// Description:	The file for my 3d Vector class.
//
//*********************************************************************************************************************

//*********************************************************************************************************************
//namespace AGG
//{
//    public struct Vector3D
//    {
//        public double x, y, z;

//        public Vector3D(double new_x, double new_y, double new_z)
//        {
//            x = new_x;
//            y = new_y;
//            z = new_z;
//        }

//        public Vector3D(float new_x, float new_y, float new_z)
//        {
//            x = (double)new_x;
//            y = (double)new_y;
//            z = (double)new_z;
//        }

//        public Vector3D(Vector3D vector3D)
//        {
//            x = vector3D.x;
//            y = vector3D.y;
//            z = vector3D.z;
//        }

//        public double this[int index]
//        {
//            get
//            {
//                switch (index)
//                {
//                    case 0:
//                        return x;
//                    case 1:
//                        return y;
//                    case 2:
//                        return z;
//                    default:
//                        return 0;
//                }
//            }
//        }

//        public void Set(double InX, double InY, double InZ)
//        {
//            x = InX;
//            y = InY;
//            z = InZ;
//        }

//        public void Set(Vector3D Src)
//        {
//            x = Src.x;
//            y = Src.y;
//            z = Src.z;
//        }

//        public double GetLength() { return GetRadius(); }
//        public double GetRadius()
//        {
//            return (double)((System.Math.Sqrt((x * x) + (y * y) + (z * z))));
//        }

//        public void SetRadius(double len)
//        {
//            Normalize();
//            this *= len;
//        }


//        public double Dot(Vector3D B)
//        {
//            return (x * B.x + y * B.y + z * B.z);
//        }

//        public void Cross(Vector3D A, Vector3D B)
//        {
//            x = A.y * B.z - A.z * B.y;
//            y = A.z * B.x - A.x * B.z;
//            z = A.x * B.y - A.y * B.x;
//        }

//        public Vector3D Cross(Vector3D B)
//        {
//            Vector3D Temp = new Vector3D();
//            Temp.Cross(this, B);
//            return Temp;
//        }

//        public void RotateAboutX(float Radians)
//        {
//            RotateAboutX((double)Radians);
//        }

//        public void RotateAboutX(double Radians)
//        {
//            Vector3D Temp;

//            double Cos, Sin;

//            Cos = (double)System.Math.Cos(Radians);
//            Sin = (double)System.Math.Sin(Radians);

//            Temp.y = y * Cos + z * Sin;
//            Temp.z = z * Cos + y * Sin;

//            y = Temp.y;
//            z = Temp.z;
//        }

//        public void RotateAboutY(float Radians)
//        {
//            RotateAboutY((double)Radians);
//        }

//        public void RotateAboutY(double Radians)
//        {
//            Vector3D Temp;

//            double Cos, Sin;

//            Cos = (double)System.Math.Cos(Radians);
//            Sin = (double)System.Math.Sin(Radians);

//            Temp.z = z * Cos + x * Sin;
//            Temp.x = x * Cos + z * Sin;

//            x = Temp.x;
//            z = Temp.z;
//        }

//        public void RotateAboutZ(double Radians)
//        {
//            RotateAboutZ((double)Radians);
//        }

//        public void RotateAboutZ(float Radians)
//        {
//            Vector3D Temp;

//            double Cos, Sin;

//            Cos = (double)System.Math.Cos(Radians);
//            Sin = (double)System.Math.Sin(Radians);

//            Temp.x = x * Cos + y * Sin;
//            Temp.y = y * Cos + x * Sin;

//            x = Temp.x;
//            y = Temp.y;
//        }

//        public void Normalize()
//        {
//            double dist;

//            dist = GetRadius();		// the point is that we need our own length

//            //assert(dist != 0.0f);

//            if (dist != 0.0f)
//            {
//                double invdist = 1.0f / dist;
//                x *= invdist;
//                y *= invdist;
//                z *= invdist;
//            }
//        }

//        public void Normalize(double Length)
//        {
//            if (Length == 0)
//            {
//                throw new System.InvalidOperationException("You can not Normalize to a 0 length");
//            }

//            if (Length != 0.0f)
//            {
//                double invLength = 1.0f / Length;
//                x *= invLength;
//                y *= invLength;
//                z *= invLength;
//            }
//        }

//        public void Zero()
//        {
//            x = (double)0;
//            y = (double)0;
//            z = (double)0;
//        }

//        public void EquLinComb(double r, Vector3D A, double s, Vector3D B)
//        {
//            // the two Vectors are muliplied by thier own variable scale and then added together
//            x = r * A.x + s * B.x;
//            y = r * A.y + s * B.y;
//            z = r * A.z + s * B.z;
//        }

//        public void ScaleAdd(double r, Vector3D VectorOne)
//        {
//            // The Vector3D is scaled and then another Vector3D is added to it.
//            x = r * x + VectorOne.x;
//            y = r * y + VectorOne.y;
//            z = r * z + VectorOne.z;
//        }

//        public void ScaleMult(double r, Vector3D VectorOne)
//        {
//            // The Vector3D is scaled and then multiplied by another Vector3D.
//            x = r * x * VectorOne.x;
//            y = r * y * VectorOne.y;
//            z = r * z * VectorOne.z;
//        }

//        public void EquMin(Vector3D VectorOne, Vector3D VectorTwo)
//        {
//            // set it equal to the min
//            if (VectorOne.x < VectorTwo.x)
//                x = VectorOne.x;
//            else
//                x = VectorTwo.x;

//            if (VectorOne.y < VectorTwo.y)
//                y = VectorOne.y;
//            else
//                y = VectorTwo.y;

//            if (VectorOne.z < VectorTwo.z)
//                z = VectorOne.z;
//            else
//                z = VectorTwo.z;
//        }

//        public void EquMax(Vector3D VectorOne, Vector3D VectorTwo)
//        {
//            if (VectorOne.x > VectorTwo.x)
//                x = VectorOne.x;
//            else
//                x = VectorTwo.x;

//            if (VectorOne.y > VectorTwo.y)
//                y = VectorOne.y;
//            else
//                y = VectorTwo.y;

//            if (VectorOne.z > VectorTwo.z)
//                z = VectorOne.z;
//            else
//                z = VectorTwo.z;
//        }

//        public void Negate()
//        {
//            x = -x;
//            y = -y;
//            z = -z;
//        }

//        public void Debug()
//        {
//            //DebugStream("3DPoint: x = " << x << " : y = " << y << " : z = " << z);
//        }

//        // are they the same within the error Value?
//        public bool Equals(Vector3D OtherVector, double ErrorValue)
//        {
//            if ((x < OtherVector.x + ErrorValue && x > OtherVector.x - ErrorValue) &&
//                (y < OtherVector.y + ErrorValue && y > OtherVector.y - ErrorValue) &&
//                (z < OtherVector.z + ErrorValue && z > OtherVector.z - ErrorValue))
//            {
//                return true;
//            }

//            return false;
//        }

//        public void GetPerpendicularVector(Vector3D DestVector)
//        {
//            if (x == 0.0f)
//            {
//                DestVector.Set(1.0f, 0.0f, 0.0f);
//            }
//            else if (y == 0.0f)
//            {
//                DestVector.Set(0.0f, 1.0f, 0.0f);
//            }
//            else if (z == 0.0f)
//            {
//                DestVector.Set(0.0f, 0.0f, 1.0f);
//            }
//            else
//            {
//                // pick a vector perpendicular to our original vector
//                // we want ConeDir . PerpVec == 0, so this gives us
//                // -y/x * x + 1 * y + 0 * z ==
//                // -y + y + 0 == 0
//                //
//                // so this vector is perpendicular to our original one
//                if (System.Math.Abs(x) > System.Math.Abs(y))
//                {
//                    if (System.Math.Abs(y) > System.Math.Abs(z))
//                    {
//                        // x is the biggest z is the smallest
//                        DestVector.Set(z / x, 0.0f, -1.0f);
//                    }
//                    else
//                    {
//                        // x is the biggest y is the smallest
//                        DestVector.Set(y / x, -1.0f, 0.0f);
//                    }
//                }
//                else if (System.Math.Abs(z) > System.Math.Abs(y)) // y > x
//                {
//                    // z is the biggest x is the smallest
//                    DestVector.Set(-1.0f, 0.0f, x / z);
//                }
//                else // y > x && z < y
//                {
//                    // y is the biggest
//                    if (x > z)
//                    {
//                        // y is the biggest z is the smallest
//                        DestVector.Set(0.0f, z / y, -1.0f);
//                    }
//                    else
//                    {
//                        // y is the biggest x is the smallest
//                        DestVector.Set(-1.0f, x / y, 0.0f);
//                    }
//                }
//            }
//        }

//        //Vector3D	 	operator-(Vector3D B);
//        // this = pOne - pSubtracted
//        public void Minus(Vector3D One, Vector3D Subtracted)
//        {
//            x = One.x - Subtracted.x;
//            y = One.y - Subtracted.y;
//            z = One.z - Subtracted.z;
//        }


//        public void Plus(Vector3D One, Vector3D Two)
//        {
//            x = One.x + Two.x;
//            y = One.y + Two.y;
//            z = One.z + Two.z;
//        }


//        public static Vector3D operator -(Vector3D B)
//        {
//            return new Vector3D(-B.x, -B.y, -B.z);
//        }

//        public static Vector3D operator *(Vector3D A, int scalar)
//        {
//            return new Vector3D((scalar * A.x), (scalar * A.y), (scalar * A.z));
//        }

//        public static Vector3D operator *(int scalar, Vector3D A)
//        {
//            return new Vector3D((scalar * A.x), (scalar * A.y), (scalar * A.z));
//        }

//        public static Vector3D operator *(Vector3D A, double scalar)
//        {
//            return new Vector3D((scalar * A.x), (scalar * A.y), (scalar * A.z));
//        }

//        public static Vector3D operator *(double scalar, Vector3D A)
//        {
//            return new Vector3D((scalar * A.x), (scalar * A.y), (scalar * A.z));
//        }

//        public static Vector3D operator *(Vector3D A, float scalar)
//        {
//            return new Vector3D((scalar * A.x), (scalar * A.y), (scalar * A.z));
//        }

//        public static Vector3D operator *(float scalar, Vector3D A)
//        {
//            return new Vector3D((scalar * A.x), (scalar * A.y), (scalar * A.z));
//        }

//        public static Vector3D operator /(Vector3D A, double Scale)
//        {
//            double scalar = 1.0f / Scale;
//            return new Vector3D((scalar * A.x), (scalar * A.y), (scalar * A.z));
//        }

//        public static Vector3D operator /(double Scale, Vector3D A)
//        {
//            double scalar = 1.0f / Scale;
//            return new Vector3D((scalar * A.x), (scalar * A.y), (scalar * A.z));
//        }

//        public static Vector3D operator +(Vector3D A, Vector3D B)
//        {
//            return new Vector3D(A.x + B.x, A.y + B.y, A.z + B.z);
//        }

//        public static Vector3D operator -(Vector3D A, Vector3D B)
//        {
//            return new Vector3D(A.x - B.x, A.y - B.y, A.z - B.z);
//        }

//        public override bool Equals(System.Object obj)
//        {
//            // If parameter is null return false.
//            if (obj == null)
//            {
//                return false;
//            }

//            // If parameter cannot be cast to Point return false.
//            Vector3D p = (Vector3D)obj;
//            if ((System.Object)p == null)
//            {
//                return false;
//            }

//            // Return true if the fields match:
//            return (x == p.x) && (y == p.y) && (z == p.z);
//        }

//        public static bool operator ==(Vector3D a, Vector3D b)
//        {
//            return a.Equals(b);
//        }

//        public static bool operator !=(Vector3D a, Vector3D b)
//        {
//            return !a.Equals(b);
//        }

//        public override int GetHashCode()
//        {
//            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
//        }
//    };
//}
