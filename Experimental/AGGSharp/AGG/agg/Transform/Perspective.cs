
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
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
//
// Perspective 2D transformations
//
//----------------------------------------------------------------------------
using System;
using NPack;
using NPack.Interfaces;

namespace AGG.Transform
{
    public class Perspective<T>
        : AffineMatrix<T>
         where T : IEquatable<T>, IComparable<T>, IComputable<T>, IConvertible, IFormattable
    {
        public Perspective()
            : base(MatrixFormat.ColumnMajor, 3)
        { }


        // Rectangle to quadrilateral
        public Perspective(T x1, T y1, T x2, T y2, T[] quad)
            : this()
        {
            throw new NotImplementedException();
        }

        // Quadrilateral to rectangle
        public Perspective(T[] quad, T x1, T y1, T x2, T y2)
            : this()
        {
            throw new NotImplementedException();
        }

        public bool IsValid()
        {
            //TODO:fix
            return true;
        }
    }


    //=======================================================trans_perspective
    //public sealed class Perspective : ITransform
    //{
    //    static public double AffineEpsilon = 1e-14;
    //    private double sx, shy, w0, shx, sy, w1, tx, ty, w2;

    //    public double W2
    //    {
    //        get { return w2; }
    //        private set { w2 = value; }
    //    }

    //    public double TY
    //    {
    //        get { return ty; }
    //        private set { ty = value; }
    //    }

    //    public double TX
    //    {
    //        get { return tx; }
    //        private set { tx = value; }
    //    }

    //    public double W1
    //    {
    //        get { return w1; }
    //        private set { w1 = value; }
    //    }

    //    public double SY
    //    {
    //        get { return sy; }
    //        private set { sy = value; }
    //    }

    //    public double SHX
    //    {
    //        get { return shx; }
    //        private set { shx = value; }
    //    }

    //    public double W0
    //    {
    //        get { return w0; }
    //        private set { w0 = value; }
    //    }

    //    public double SHY
    //    {
    //        get { return shy; }
    //        private set { shy = value; }
    //    }

    //    public double SX
    //    {
    //        get { return sx; }
    //        private set { sx = value; }
    //    }

    //    //-------------------------------------------------------ruction
    //    // Identity matrix
    //    public Perspective()
    //    {
    //        sx = (1); shy = (0); w0 = (0);
    //        shx = (0); sy = (1); w1 = (0);
    //        tx = (0); ty = (0); w2 = (1);
    //    }

    //    // Custom matrix
    //    public Perspective(double v0, double v1, double v2,
    //                      double v3, double v4, double v5,
    //                      double v6, double v7, double v8)
    //    {
    //        sx = (v0); shy = (v1); w0 = (v2);
    //        shx = (v3); sy = (v4); w1 = (v5);
    //        tx = (v6); ty = (v7); w2 = (v8);
    //    }

    //    // Custom matrix from m[9]
    //    public Perspective(double[] m)
    //    {
    //        sx = (m[0]); shy = (m[1]); w0 = (m[2]);
    //        shx = (m[3]); sy = (m[4]); w1 = (m[5]);
    //        tx = (m[6]); ty = (m[7]); w2 = (m[8]);
    //    }

    //    // From affine
    //    public Perspective(Affine a)
    //    {
    //        sx = (a.SX); shy = (a.SHY); w0 = (0);
    //        shx = (a.SHX); sy = (a.SY); w1 = (0);
    //        tx = (a.TX); ty = (a.TY); w2 = (1);
    //    }

    //    // From trans_perspective
    //    public Perspective(Perspective a)
    //    {
    //        sx = (a.sx); shy = (a.shy); w0 = a.w0;
    //        shx = (a.shx); sy = (a.sy); w1 = a.w1;
    //        tx = (a.tx); ty = (a.ty); w2 = a.w2;
    //    }

    //    // Rectangle to quadrilateral
    //    public Perspective(double x1, double y1, double x2, double y2, double[] quad)
    //    {
    //        RectToQuad(x1, y1, x2, y2, quad);
    //    }

    //    // Quadrilateral to rectangle
    //    public Perspective(double[] quad, double x1, double y1, double x2, double y2)
    //    {
    //        QuadToRect(quad, x1, y1, x2, y2);
    //    }

    //    // Arbitrary quadrilateral transformations
    //    public Perspective(double[] src, double[] dst)
    //    {
    //        QuadToQuad(src, dst);
    //    }

    //    public void Set(Perspective Other)
    //    {
    //        SX = Other.SX;
    //        SHY = Other.SHY;
    //        W0 = Other.W0;
    //        SHX = Other.SHX;
    //        SY = Other.SY;
    //        W1 = Other.W1;
    //        TX = Other.TX;
    //        TY = Other.TY;
    //        W2 = Other.W2;
    //    }

    //    //-------------------------------------- Quadrilateral transformations
    //    // The arguments are double[8] that are mapped to quadrilaterals:
    //    // x1,y1, x2,y2, x3,y3, x4,y4
    //    public bool QuadToQuad(double[] qs, double[] qd)
    //    {
    //        Perspective p = new Perspective();
    //        if (!QuadToSquare(qs)) return false;
    //        if (!p.SquareToQuad(qd)) return false;
    //        Multiply(p);
    //        return true;
    //    }

    //    public bool RectToQuad(double x1, double y1, double x2, double y2, double[] q)
    //    {
    //        double[] r = new double[8];
    //        r[0] = r[6] = x1;
    //        r[2] = r[4] = x2;
    //        r[1] = r[3] = y1;
    //        r[5] = r[7] = y2;
    //        return QuadToQuad(r, q);
    //    }

    //    public bool QuadToRect(double[] q, double x1, double y1, double x2, double y2)
    //    {
    //        double[] r = new double[8];
    //        r[0] = r[6] = x1;
    //        r[2] = r[4] = x2;
    //        r[1] = r[3] = y1;
    //        r[5] = r[7] = y2;
    //        return QuadToQuad(q, r);
    //    }

    //    // Map square (0,0,1,1) to the quadrilateral and vice versa
    //    public bool SquareToQuad(double[] q)
    //    {
    //        double dx = q[0] - q[2] + q[4] - q[6];
    //        double dy = q[1] - q[3] + q[5] - q[7];
    //        if (dx == 0.0 && dy == 0.0)
    //        {
    //            // Affine case (parallelogram)
    //            //---------------
    //            SX = q[2] - q[0];
    //            SHY = q[3] - q[1];
    //            W0 = 0.0;
    //            SHX = q[4] - q[2];
    //            SY = q[5] - q[3];
    //            W1 = 0.0;
    //            TX = q[0];
    //            TY = q[1];
    //            W2 = 1.0;
    //        }
    //        else
    //        {
    //            double dx1 = q[2] - q[4];
    //            double dy1 = q[3] - q[5];
    //            double dx2 = q[6] - q[4];
    //            double dy2 = q[7] - q[5];
    //            double den = dx1 * dy2 - dx2 * dy1;
    //            if (den == 0.0)
    //            {
    //                // Singular case
    //                //---------------
    //                SX = SHY = W0 = SHX = SY = W1 = TX = TY = W2 = 0.0;
    //                return false;
    //            }
    //            // General case
    //            //---------------
    //            double u = (dx * dy2 - dy * dx2) / den;
    //            double v = (dy * dx1 - dx * dy1) / den;
    //            SX = q[2] - q[0] + u * q[2];
    //            SHY = q[3] - q[1] + u * q[3];
    //            W0 = u;
    //            SHX = q[6] - q[0] + v * q[6];
    //            SY = q[7] - q[1] + v * q[7];
    //            W1 = v;
    //            TX = q[0];
    //            TY = q[1];
    //            W2 = 1.0;
    //        }
    //        return true;
    //    }

    //    public bool QuadToSquare(double[] q)
    //    {
    //        if (!SquareToQuad(q)) return false;
    //        Invert();
    //        return true;
    //    }


    //    //--------------------------------------------------------- Operations
    //    public Perspective FromAffine(Affine a)
    //    {
    //        SX = a.SX; SHY = a.SHY; W0 = 0;
    //        SHX = a.SHX; SY = a.SY; W1 = 0;
    //        TX = a.TX; TY = a.TY; W2 = 1;
    //        return this;
    //    }

    //    // Reset - load an identity matrix
    //    public Perspective Reset()
    //    {
    //        SX = 1; SHY = 0; W0 = 0;
    //        SHX = 0; SY = 1; W1 = 0;
    //        TX = 0; TY = 0; W2 = 1;
    //        return this;
    //    }

    //    // Invert matrix. Returns false in degenerate case
    //    public bool Invert()
    //    {
    //        double d0 = SY * W2 - W1 * TY;
    //        double d1 = W0 * TY - SHY * W2;
    //        double d2 = SHY * W1 - W0 * SY;
    //        double d = SX * d0 + SHX * d1 + TX * d2;
    //        if (d == 0.0)
    //        {
    //            SX = SHY = W0 = SHX = SY = W1 = TX = TY = W2 = 0.0;
    //            return false;
    //        }
    //        d = 1.0 / d;
    //        Perspective a = new Perspective(this);
    //        SX = d * d0;
    //        SHY = d * d1;
    //        W0 = d * d2;
    //        SHX = d * (a.W1 * a.TX - a.SHX * a.W2);
    //        SY = d * (a.SX * a.W2 - a.W0 * a.TX);
    //        W1 = d * (a.W0 * a.SHX - a.SX * a.W1);
    //        TX = d * (a.SHX * a.TY - a.SY * a.TX);
    //        TY = d * (a.SHY * a.TX - a.SX * a.TY);
    //        W2 = d * (a.SX * a.SY - a.SHY * a.SHX);
    //        return true;
    //    }

    //    // Direct transformations operations
    //    public Perspective Translate(double x, double y)
    //    {
    //        TX += x;
    //        TY += y;
    //        return this;
    //    }

    //    public Perspective Rotate(double a)
    //    {
    //        Multiply(Affine.NewRotation(a));
    //        return this;
    //    }

    //    public Perspective Scale(double s)
    //    {
    //        Multiply(Affine.NewScaling(s));
    //        return this;
    //    }

    //    public Perspective Scale(double x, double y)
    //    {
    //        Multiply(Affine.NewScaling(x, y));
    //        return this;
    //    }

    //    public Perspective Multiply(Perspective a)
    //    {
    //        Perspective b = new Perspective(this);
    //        SX = a.SX * b.SX + a.SHX * b.SHY + a.TX * b.W0;
    //        SHX = a.SX * b.SHX + a.SHX * b.SY + a.TX * b.W1;
    //        TX = a.SX * b.TX + a.SHX * b.TY + a.TX * b.W2;
    //        SHY = a.SHY * b.SX + a.SY * b.SHY + a.TY * b.W0;
    //        SY = a.SHY * b.SHX + a.SY * b.SY + a.TY * b.W1;
    //        TY = a.SHY * b.TX + a.SY * b.TY + a.TY * b.W2;
    //        W0 = a.W0 * b.SX + a.W1 * b.SHY + a.W2 * b.W0;
    //        W1 = a.W0 * b.SHX + a.W1 * b.SY + a.W2 * b.W1;
    //        W2 = a.W0 * b.TX + a.W1 * b.TY + a.W2 * b.W2;
    //        return this;
    //    }

    //    //------------------------------------------------------------------------
    //    public Perspective Multiply(Affine a)
    //    {
    //        Perspective b = new Perspective(this);
    //        SX = a.SX * b.SX + a.SHX * b.SHY + a.TX * b.W0;
    //        SHX = a.SX * b.SHX + a.SHX * b.SY + a.TX * b.W1;
    //        TX = a.SX * b.TX + a.SHX * b.TY + a.TX * b.W2;
    //        SHY = a.SHY * b.SX + a.SY * b.SHY + a.TY * b.W0;
    //        SY = a.SHY * b.SHX + a.SY * b.SY + a.TY * b.W1;
    //        TY = a.SHY * b.TX + a.SY * b.TY + a.TY * b.W2;
    //        return this;
    //    }

    //    //------------------------------------------------------------------------
    //    public Perspective Premultiply(Perspective b)
    //    {
    //        Perspective a = new Perspective(this);
    //        SX = a.SX * b.SX + a.SHX * b.SHY + a.TX * b.W0;
    //        SHX = a.SX * b.SHX + a.SHX * b.SY + a.TX * b.W1;
    //        TX = a.SX * b.TX + a.SHX * b.TY + a.TX * b.W2;
    //        SHY = a.SHY * b.SX + a.SY * b.SHY + a.TY * b.W0;
    //        SY = a.SHY * b.SHX + a.SY * b.SY + a.TY * b.W1;
    //        TY = a.SHY * b.TX + a.SY * b.TY + a.TY * b.W2;
    //        W0 = a.W0 * b.SX + a.W1 * b.SHY + a.W2 * b.W0;
    //        W1 = a.W0 * b.SHX + a.W1 * b.SY + a.W2 * b.W1;
    //        W2 = a.W0 * b.TX + a.W1 * b.TY + a.W2 * b.W2;
    //        return this;
    //    }

    //    //------------------------------------------------------------------------
    //    public Perspective Premultiply(Affine b)
    //    {
    //        Perspective a = new Perspective(this);
    //        SX = a.SX * b.SX + a.SHX * b.SHY;
    //        SHX = a.SX * b.SHX + a.SHX * b.SY;
    //        TX = a.SX * b.TX + a.SHX * b.TY + a.TX;
    //        SHY = a.SHY * b.SX + a.SY * b.SHY;
    //        SY = a.SHY * b.SHX + a.SY * b.SY;
    //        TY = a.SHY * b.TX + a.SY * b.TY + a.TY;
    //        W0 = a.W0 * b.SX + a.W1 * b.SHY;
    //        W1 = a.W0 * b.SHX + a.W1 * b.SY;
    //        W2 = a.W0 * b.TX + a.W1 * b.TY + a.W2;
    //        return this;
    //    }

    //    //------------------------------------------------------------------------
    //    public Perspective MultiplyInv(Perspective m)
    //    {
    //        Perspective t = m;
    //        t.Invert();
    //        return Multiply(t);
    //    }

    //    //------------------------------------------------------------------------
    //    public Perspective TransPerspectiveMultiplyInv(Affine m)
    //    {
    //        Affine t = m;
    //        t.Invert();
    //        return Multiply(t);
    //    }

    //    //------------------------------------------------------------------------
    //    public Perspective PreMultiplyInv(Perspective m)
    //    {
    //        Perspective t = m;
    //        t.Invert();
    //        Set(t.Multiply(this));
    //        return this;
    //    }

    //    // Multiply inverse of "m" by "this" and assign the result to "this"
    //    public Perspective PreMultiplyInv(Affine m)
    //    {
    //        Perspective t = new Perspective(m);
    //        t.Invert();
    //        Set(t.Multiply(this));
    //        return this;
    //    }

    //    //--------------------------------------------------------- Load/Store
    //    public void StoreTo(double[] m)
    //    {
    //        m[0] = SX; m[1] = SHY; m[2] = W0;
    //        m[3] = SHX; m[4] = SY; m[5] = W1;
    //        m[6] = TX; m[7] = TY; m[8] = W2;
    //    }

    //    //------------------------------------------------------------------------
    //    public Perspective LoadFrom(double[] m)
    //    {
    //        SX = m[0]; SHY = m[1]; W0 = m[2];
    //        SHX = m[3]; SY = m[4]; W1 = m[5];
    //        TX = m[6]; TY = m[7]; W2 = m[8];
    //        return this;
    //    }

    //    //---------------------------------------------------------- Operators
    //    // Multiply the matrix by another one and return the result in a separate matrix.
    //    public static Perspective operator *(Perspective a, Perspective b)
    //    {
    //        Perspective temp = a;
    //        temp.Multiply(b);

    //        return temp;
    //    }

    //    // Multiply the matrix by another one and return the result in a separate matrix.
    //    public static Perspective operator *(Perspective a, Affine b)
    //    {
    //        Perspective temp = a;
    //        temp.Multiply(b);

    //        return temp;
    //    }

    //    // Multiply the matrix by inverse of another one and return the result in a separate matrix.
    //    public static Perspective operator /(Perspective a, Perspective b)
    //    {
    //        Perspective temp = a;
    //        temp.MultiplyInv(b);

    //        return temp;
    //    }

    //    // Calculate and return the inverse matrix
    //    public static Perspective operator ~(Perspective b)
    //    {
    //        Perspective ret = b;
    //        ret.Invert();
    //        return ret;
    //    }

    //    // Equal operator with default epsilon
    //    public static bool operator ==(Perspective a, Perspective b)
    //    {
    //        return a.IsEqual(b, AffineEpsilon);
    //    }

    //    // Not Equal operator with default epsilon
    //    public static bool operator !=(Perspective a, Perspective b)
    //    {
    //        return !a.IsEqual(b, AffineEpsilon);
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        return base.Equals(obj);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return base.GetHashCode();
    //    }

    //    //---------------------------------------------------- Transformations
    //    // Direct transformation of x and y
    //    public void Transform(ref double px, ref double py)
    //    {
    //        double x = px;
    //        double y = py;
    //        double m = 1.0 / (x * W0 + y * W1 + W2);
    //        px = m * (x * SX + y * SHX + TX);
    //        py = m * (x * SHY + y * SY + TY);
    //    }

    //    // Direct transformation of x and y, affine part only
    //    public void TransformAffine(ref double x, ref double y)
    //    {
    //        double tmp = x;
    //        x = tmp * SX + y * SHX + TX;
    //        y = tmp * SHY + y * SY + TY;
    //    }

    //    // Direct transformation of x and y, 2x2 matrix only, no translation
    //    public void Transform2x2(ref double x, ref double y)
    //    {
    //        double tmp = x;
    //        x = tmp * SX + y * SHX;
    //        y = tmp * SHY + y * SY;
    //    }

    //    // Inverse transformation of x and y. It works slow because
    //    // it explicitly inverts the matrix on every call. For massive 
    //    // operations it's better to invert() the matrix and then use 
    //    // direct transformations. 
    //    public void InverseTransform(ref double x, ref double y)
    //    {
    //        Perspective t = new Perspective(this);
    //        if (t.Invert()) t.Transform(ref x, ref y);
    //    }


    //    //---------------------------------------------------------- Auxiliary
    //    public double Determinant()
    //    {
    //        return SX * (SY * W2 - TY * W1) +
    //               SHX * (TY * W0 - SHY * W2) +
    //               TX * (SHY * W1 - SY * W0);
    //    }
    //    public double DeterminantReciprocal()
    //    {
    //        return 1.0 / Determinant();
    //    }

    //    public bool IsValid() { return IsValid(AffineEpsilon); }
    //    public bool IsValid(double epsilon)
    //    {
    //        return Math.Abs(SX) > epsilon && Math.Abs(SY) > epsilon && Math.Abs(W2) > epsilon;
    //    }

    //    public bool IsIdentity() { return IsIdentity(AffineEpsilon); }
    //    public bool IsIdentity(double epsilon)
    //    {
    //        return Basics.IsEqualEpsilon(SX, 1.0, epsilon) &&
    //               Basics.IsEqualEpsilon(SHY, 0.0, epsilon) &&
    //               Basics.IsEqualEpsilon(W0, 0.0, epsilon) &&
    //               Basics.IsEqualEpsilon(SHX, 0.0, epsilon) &&
    //               Basics.IsEqualEpsilon(SY, 1.0, epsilon) &&
    //               Basics.IsEqualEpsilon(W1, 0.0, epsilon) &&
    //               Basics.IsEqualEpsilon(TX, 0.0, epsilon) &&
    //               Basics.IsEqualEpsilon(TY, 0.0, epsilon) &&
    //               Basics.IsEqualEpsilon(W2, 1.0, epsilon);
    //    }

    //    public bool IsEqual(Perspective m)
    //    {
    //        return IsEqual(m, AffineEpsilon);
    //    }

    //    public bool IsEqual(Perspective m, double epsilon)
    //    {
    //        return Basics.IsEqualEpsilon(SX, m.SX, epsilon) &&
    //               Basics.IsEqualEpsilon(SHY, m.SHY, epsilon) &&
    //               Basics.IsEqualEpsilon(W0, m.W0, epsilon) &&
    //               Basics.IsEqualEpsilon(SHX, m.SHX, epsilon) &&
    //               Basics.IsEqualEpsilon(SY, m.SY, epsilon) &&
    //               Basics.IsEqualEpsilon(W1, m.W1, epsilon) &&
    //               Basics.IsEqualEpsilon(TX, m.TX, epsilon) &&
    //               Basics.IsEqualEpsilon(TY, m.TY, epsilon) &&
    //               Basics.IsEqualEpsilon(W2, m.W2, epsilon);
    //    }

    //    // Determine the major affine parameters. Use with caution 
    //    // considering possible degenerate cases.
    //    public double Scale()
    //    {
    //        double x = 0.707106781 * SX + 0.707106781 * SHX;
    //        double y = 0.707106781 * SHY + 0.707106781 * SY;
    //        return Math.Sqrt(x * x + y * y);
    //    }
    //    public double Rotation()
    //    {
    //        double x1 = 0.0;
    //        double y1 = 0.0;
    //        double x2 = 1.0;
    //        double y2 = 0.0;
    //        Transform(ref x1, ref y1);
    //        Transform(ref x2, ref y2);
    //        return Math.Atan2(y2 - y1, x2 - x1);
    //    }
    //    public void Translation(out double dx, out double dy)
    //    {
    //        dx = TX;
    //        dy = TY;
    //    }
    //    public void Scaling(out double x, out double y)
    //    {
    //        double x1 = 0.0;
    //        double y1 = 0.0;
    //        double x2 = 1.0;
    //        double y2 = 1.0;
    //        Perspective t = new Perspective(this);
    //        t *= Affine.NewRotation(-Rotation());
    //        t.Transform(ref x1, ref y1);
    //        t.Transform(ref x2, ref y2);
    //        x = x2 - x1;
    //        y = y2 - y1;
    //    }
    //    public void ScalingAbs(out double x, out double y)
    //    {
    //        x = Math.Sqrt(SX * SX + SHX * SHX);
    //        y = Math.Sqrt(SHY * SHY + SY * SY);
    //    }

    //    //--------------------------------------------------------------------
    //    public sealed class IteratorX
    //    {
    //        double den;
    //        double den_step;
    //        double nom_x;
    //        double nom_x_step;
    //        double nom_y;
    //        double nom_y_step;

    //        private double x;

    //        public double X
    //        {
    //            get { return x; }
    //            set { x = value; }
    //        }
    //        private double y;

    //        public double Y
    //        {
    //            get { return y; }
    //            set { y = value; }
    //        }

    //        public IteratorX() { }
    //        public IteratorX(double px, double py, double step, Perspective m)
    //        {
    //            den = (px * m.W0 + py * m.W1 + m.W2);
    //            den_step = (m.W0 * step);
    //            nom_x = (px * m.SX + py * m.SHX + m.TX);
    //            nom_x_step = (step * m.SX);
    //            nom_y = (px * m.SHY + py * m.SY + m.TY);
    //            nom_y_step = (step * m.SHY);
    //            X = (nom_x / den);
    //            Y = (nom_y / den);
    //        }

    //        public static IteratorX operator ++(IteratorX a)
    //        {
    //            a.den += a.den_step;
    //            a.nom_x += a.nom_x_step;
    //            a.nom_y += a.nom_y_step;
    //            double d = 1.0 / a.den;
    //            a.X = a.nom_x * d;
    //            a.Y = a.nom_y * d;

    //            return a;
    //        }
    //    };

    //    //--------------------------------------------------------------------
    //    public IteratorX Begin(double x, double y, double step)
    //    {
    //        return new IteratorX(x, y, step, this);
    //    }
    //};
}
