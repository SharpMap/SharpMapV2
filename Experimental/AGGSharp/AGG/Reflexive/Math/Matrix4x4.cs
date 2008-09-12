



namespace Reflexive.Math
{
    //public class Matrix4X4
    //{
    //    double[] m_pA = new double[16];

    //    public Matrix4X4()
    //    {
    //        SetElement(0, 0, 1.0f);
    //        SetElement(1, 1, 1.0f);
    //        SetElement(2, 2, 1.0f);
    //        SetElement(3, 3, 1.0f);
    //    }

    //    public Matrix4X4(Matrix4X4 CopyFrom)
    //    {
    //        for(int i=0; i<16; i++)
    //        {
    //            m_pA[i] = CopyFrom.m_pA[i];
    //        }
    //    }

    //    public Matrix4X4(double[] CopyFrom)
    //    {
    //        SetElements(CopyFrom);
    //    }

    //    public double GetElement(int Row, int Column) 
    //    {
    //        return m_pA[Row * 4 + Column]; 
    //    }

    //    public void SetElement(int Row, int Column, double Value)
    //    {
    //        m_pA[Row * 4 + Column] = Value; 
    //    }

    //    public void AddElement(int Row, int Column, double Value) 
    //    {
    //        m_pA[Row * 4 + Column] += Value; 
    //    }

    //    public void Identity()
    //    {
    //        Zero();
    //        SetElement(0, 0, 1.0f);
    //        SetElement(1, 1, 1.0f);
    //        SetElement(2, 2, 1.0f);
    //        SetElement(3, 3, 1.0f);
    //    }

    //    public void Zero()
    //    {
    //        for (int i = 0; i < 16; i++)
    //        {
    //            m_pA[i] = 0;
    //        }
    //    }

    //    // A bit of code from Intel LBB [10/29/2003]
    //    /************************************************************
    //    *
    //    * input:
    //    * mat - pointer to array of 16 doubles (source matrix)
    //    * output:
    //    * dst - pointer to array of 16 doubles (invert matrix)
    //    *
    //    Version														Cycles
    //    C code with Cramer's rule									846
    //    C code with Cramer's rule & Streaming SIMD Extensions		210
    //    *************************************************************/
    //    static void IntelInvertC(double[] mat, double[] dst)
    //    {
    //        double[] tmp = new double[12]; /* temp array for pairs */
    //        double[] src = new double[16]; /* array of transpose source matrix */
    //        double det; /* determinant */
    //        /* transpose matrix */
    //        for ( int i = 0; i < 4; i++) {
    //            src[i] = mat[i*4];
    //            src[i + 4] = mat[i*4 + 1];
    //            src[i + 8] = mat[i*4 + 2];
    //            src[i + 12] = mat[i*4 + 3];
    //        }
    //        /* calculate pairs for first 8 elements (cofactors) */
    //        tmp[0] = src[10] * src[15];
    //        tmp[1] = src[11] * src[14];
    //        tmp[2] = src[9] * src[15];
    //        tmp[3] = src[11] * src[13];
    //        tmp[4] = src[9] * src[14];
    //        tmp[5] = src[10] * src[13];
    //        tmp[6] = src[8] * src[15];
    //        tmp[7] = src[11] * src[12];
    //        tmp[8] = src[8] * src[14];
    //        tmp[9] = src[10] * src[12];
    //        tmp[10] = src[8] * src[13];
    //        tmp[11] = src[9] * src[12];
    //        /* calculate first 8 elements (cofactors) */
    //        dst[0] = tmp[0]*src[5] + tmp[3]*src[6] + tmp[4]*src[7];
    //        dst[0] -= tmp[1]*src[5] + tmp[2]*src[6] + tmp[5]*src[7];
    //        dst[1] = tmp[1]*src[4] + tmp[6]*src[6] + tmp[9]*src[7];
    //        dst[1] -= tmp[0]*src[4] + tmp[7]*src[6] + tmp[8]*src[7];
    //        dst[2] = tmp[2]*src[4] + tmp[7]*src[5] + tmp[10]*src[7];
    //        dst[2] -= tmp[3]*src[4] + tmp[6]*src[5] + tmp[11]*src[7];
    //        dst[3] = tmp[5]*src[4] + tmp[8]*src[5] + tmp[11]*src[6];
    //        dst[3] -= tmp[4]*src[4] + tmp[9]*src[5] + tmp[10]*src[6];
    //        dst[4] = tmp[1]*src[1] + tmp[2]*src[2] + tmp[5]*src[3];
    //        dst[4] -= tmp[0]*src[1] + tmp[3]*src[2] + tmp[4]*src[3];
    //        dst[5] = tmp[0]*src[0] + tmp[7]*src[2] + tmp[8]*src[3];
    //        dst[5] -= tmp[1]*src[0] + tmp[6]*src[2] + tmp[9]*src[3];
    //        dst[6] = tmp[3]*src[0] + tmp[6]*src[1] + tmp[11]*src[3];
    //        dst[6] -= tmp[2]*src[0] + tmp[7]*src[1] + tmp[10]*src[3];
    //        dst[7] = tmp[4]*src[0] + tmp[9]*src[1] + tmp[10]*src[2];
    //        dst[7] -= tmp[5]*src[0] + tmp[8]*src[1] + tmp[11]*src[2];
    //        /* calculate pairs for second 8 elements (cofactors) */
    //        tmp[0] = src[2]*src[7];
    //        tmp[1] = src[3]*src[6];
    //        tmp[2] = src[1]*src[7];
    //        tmp[3] = src[3]*src[5];
    //        tmp[4] = src[1]*src[6];
    //        tmp[5] = src[2]*src[5];
    //        tmp[6] = src[0]*src[7];
    //        tmp[7] = src[3]*src[4];
    //        tmp[8] = src[0]*src[6];
    //        tmp[9] = src[2]*src[4];
    //        tmp[10] = src[0]*src[5];
    //        tmp[11] = src[1]*src[4];
    //        /* calculate second 8 elements (cofactors) */
    //        dst[8] = tmp[0]*src[13] + tmp[3]*src[14] + tmp[4]*src[15];
    //        dst[8] -= tmp[1]*src[13] + tmp[2]*src[14] + tmp[5]*src[15];
    //        dst[9] = tmp[1]*src[12] + tmp[6]*src[14] + tmp[9]*src[15];
    //        dst[9] -= tmp[0]*src[12] + tmp[7]*src[14] + tmp[8]*src[15];
    //        dst[10] = tmp[2]*src[12] + tmp[7]*src[13] + tmp[10]*src[15];
    //        dst[10]-= tmp[3]*src[12] + tmp[6]*src[13] + tmp[11]*src[15];
    //        dst[11] = tmp[5]*src[12] + tmp[8]*src[13] + tmp[11]*src[14];
    //        dst[11]-= tmp[4]*src[12] + tmp[9]*src[13] + tmp[10]*src[14];
    //        dst[12] = tmp[2]*src[10] + tmp[5]*src[11] + tmp[1]*src[9];
    //        dst[12]-= tmp[4]*src[11] + tmp[0]*src[9] + tmp[3]*src[10];
    //        dst[13] = tmp[8]*src[11] + tmp[0]*src[8] + tmp[7]*src[10];
    //        dst[13]-= tmp[6]*src[10] + tmp[9]*src[11] + tmp[1]*src[8];
    //        dst[14] = tmp[6]*src[9] + tmp[11]*src[11] + tmp[3]*src[8];
    //        dst[14]-= tmp[10]*src[11] + tmp[2]*src[8] + tmp[7]*src[9];
    //        dst[15] = tmp[10]*src[10] + tmp[4]*src[8] + tmp[9]*src[9];
    //        dst[15]-= tmp[8]*src[9] + tmp[11]*src[10] + tmp[5]*src[8];
    //        /* calculate determinant */
    //        det=src[0]*dst[0]+src[1]*dst[1]+src[2]*dst[2]+src[3]*dst[3];
    //        /* calculate matrix inverse */
    //        det = 1/det;
    //        for ( int j = 0; j < 16; j++)
    //        {
    //            dst[j] *= det;
    //        }  
    //    }

    //    public bool GetInverse(Matrix4X4 OriginalMatrix)
    //    {
    //        IntelInvertC(OriginalMatrix.m_pA, m_pA);
    //        return true;
    //    }

    //    public bool GetInverse()
    //    {
    //        Matrix4X4 Temp = new Matrix4X4(this);
    //        return GetInverse(Temp);
    //    }

    //    private void matrix_swap_mirror(int a, int b)
    //    {
    //        double Temp = GetElement(a, b);
    //        SetElement(a,b, GetElement(b,a));
    //        SetElement(b, a, Temp);
    //    }

    //    public void Transpose3X3()
    //    {
    //        matrix_swap_mirror(0,1);
    //        matrix_swap_mirror(0,2);
    //        matrix_swap_mirror(0,3);
    //        matrix_swap_mirror(1,2);
    //        matrix_swap_mirror(1,3);
    //        matrix_swap_mirror(2,3);
    //    }

    //    public void Translate(double tx, double ty, double tz)
    //    {
    //        int i;

    //        Zero();
    //        for(i=0; i<4; i++)
    //        {
    //            SetElement(i, i, 1.0f);
    //        }

    //        // <Simon 2002/05/02> fixed matrix ordering problem
    //        SetElement(3, 0, tx);
    //        SetElement(3, 1, ty);
    //        SetElement(3, 2, tz);
    //    }

    //    public void Translate(Vector3D Vect)
    //    {
    //        Translate(Vect.x, Vect.y, Vect.z);
    //    }

    //    public void AddTranslate(double x, double y, double z)
    //    {
    //        AddTranslate(new Vector3D(x, y, z));
    //    }

    //    public void AddTranslate(Vector3D Vect)
    //    {
    //        Matrix4X4 Temp = new Matrix4X4();
    //        Temp.Translate(Vect.x, Vect.y, Vect.z);

    //        Multiply(Temp);
    //    }

    //    public void Scale(float sx, float sy, float sz)
    //    {
    //        Scale((double)sx, (double)sy, (double)sz);
    //    }

    //    public void Scale(double sx, double sy, double sz)
    //    {
    //      Zero();
    //      SetElement(0, 0, sx);
    //      SetElement(1, 1, sy);
    //      SetElement(2, 2, sz);
    //      SetElement(3, 3, 1.0f);
    //    }

    //    public void AddRotate(uint Axis, double Theta)
    //    {
    //        Matrix4X4 Temp = new Matrix4X4();
    //        Temp.Rotate(Axis, Theta);

    //        Multiply(Temp);
    //    }

    //    public void Rotate(uint Axis, double Theta)
    //    {
    //        double c, s;

    //        if(Theta != 0)
    //        {
    //            c = (double)System.Math.Cos(Theta);
    //            s = (double)System.Math.Sin(Theta);
    //        }
    //        else
    //        {
    //            c = 1.0f;
    //            s = 0.0f;
    //        }

    //        switch(Axis)
    //        {
    //        case 0:
    //            SetElement(0, 0, 1.0f);
    //            SetElement(0, 1, 0.0f);
    //            SetElement(0, 2, 0.0f);
    //            SetElement(0, 3, 0.0f);

    //            SetElement(1, 0, 0.0f);
    //            SetElement(1, 1, c);
    //            SetElement(1, 2, s);
    //            SetElement(1, 3, 0.0f);

    //            SetElement(2, 0, 0.0f);
    //            SetElement(2, 1, -s);
    //            SetElement(2, 2, c);
    //            SetElement(2, 3, 0.0f);
    //            break;

    //        case 1:
    //            SetElement(0, 0, c);
    //            SetElement(0, 1, 0.0f);
    //            SetElement(0, 2, -s);
    //            SetElement(0, 3, 0.0f);

    //            SetElement(1, 0, 0.0f);
    //            SetElement(1, 1, 1.0f);
    //            SetElement(1, 2, 0.0f);
    //            SetElement(1, 3, 0.0f);

    //            SetElement(2, 0, s);
    //            SetElement(2, 1, 0.0f);
    //            SetElement(2, 2, c);
    //            SetElement(2, 3, 0.0f);
    //            break;

    //        case 2:
    //            SetElement(0, 0, c);
    //            SetElement(0, 1, s);
    //            SetElement(0, 2, 0.0f);
    //            SetElement(0, 3, 0.0f);

    //            SetElement(1, 0, -s);
    //            SetElement(1, 1, c);
    //            SetElement(1, 2, 0.0f);
    //            SetElement(1, 3, 0.0f);

    //            SetElement(2, 0, 0.0f);
    //            SetElement(2, 1, 0.0f);
    //            SetElement(2, 2, 1.0f);
    //            SetElement(2, 3, 0.0f);
    //            break;
    //        }

    //        // set the ones that don't change
    //        SetElement(3, 0, 0.0f);
    //        SetElement(3, 1, 0.0f);
    //        SetElement(3, 2, 0.0f);
    //        SetElement(3, 3, 1.0f);
    //    }

    //    public void Rotate(Vector3D Axis, double AngleRadians)
    //    {
    //        Axis.Normalize();

    //        double Cos = (double)System.Math.Cos(AngleRadians);
    //        double Sin = (double)System.Math.Sin(AngleRadians);

    //        double OneMinusCos = 1.0f - Cos;

    //        m_pA[0 + 4 * 0] = OneMinusCos * Axis.x * Axis.x + Cos;
    //        m_pA[0 + 4 * 1] = OneMinusCos * Axis.x * Axis.y - Sin * Axis.z;
    //        m_pA[0 + 4 * 2] = OneMinusCos * Axis.x * Axis.z + Sin * Axis.y;
    //        m_pA[0 + 4 * 3] = 0.0f;

    //        m_pA[1 + 4 * 0] = OneMinusCos * Axis.x * Axis.y + Sin * Axis.z;
    //        m_pA[1 + 4 * 1] = OneMinusCos * Axis.y * Axis.y + Cos;
    //        m_pA[1 + 4 * 2] = OneMinusCos * Axis.y * Axis.z - Sin * Axis.x;
    //        m_pA[1 + 4 * 3] = 0.0f;

    //        m_pA[2 + 4 * 0] = OneMinusCos * Axis.x * Axis.z - Sin * Axis.y;
    //        m_pA[2 + 4 * 1] = OneMinusCos * Axis.y * Axis.z + Sin * Axis.x;
    //        m_pA[2 + 4 * 2] = OneMinusCos * Axis.z * Axis.z + Cos;
    //        m_pA[2 + 4 * 3] = 0.0f;

    //        m_pA[3 + 4 * 0] = 0.0f;
    //        m_pA[3 + 4 * 1] = 0.0f;
    //        m_pA[3 + 4 * 2] = 0.0f;
    //        m_pA[3 + 4 * 3] = 1.0f;
    //    }

    //    public bool Equals(Matrix4X4 OtherMatrix, double ErrorRange)
    //    {
    //        for(int i=0; i<4; i++)
    //        {
    //            for(int j=0; j<4; j++)
    //            {
    //                if(		GetElement(i, j) < OtherMatrix.GetElement(i, j) - ErrorRange
    //                    ||	GetElement(i, j) > OtherMatrix.GetElement(i, j) + ErrorRange)
    //                {
    //                    return false;
    //                }
    //            }
    //        }

    //        return true;
    //    }

    //    public void PrepareMatrix(double Tx, double Ty, double Tz,
    //                                   double Rx, double Ry, double Rz,
    //                                   double Sx, double Sy, double Sz)
    //    {
    //        bool Initialized = false;

    //        if (Sx != 1.0f || Sy != 1.0f || Sz != 1.0f)
    //        {
    //            if (Initialized)
    //            {
    //                Matrix4X4 Temp = new Matrix4X4();
    //                Temp.Scale(Sx, Sy, Sz);
    //                Multiply(Temp);
    //            }
    //            else
    //            {
    //                Scale(Sx, Sy, Sz);
    //                Initialized = true;
    //            }
    //        }
    //        if (Rx != .0f)
    //        {
    //            if (Initialized)
    //            {
    //                Matrix4X4 Temp = new Matrix4X4();
    //                Temp.Rotate(0, Rx);
    //                Multiply(Temp);
    //            }
    //            else
    //            {
    //                Rotate(0, Rx);
    //                Initialized = true;
    //            }
    //        }
    //        if (Ry != .0f)
    //        {
    //            if (Initialized)
    //            {
    //                Matrix4X4 Temp = new Matrix4X4();
    //                Temp.Rotate(1, Ry);
    //                Multiply(Temp);
    //            }
    //            else
    //            {
    //                Rotate(1, Ry);
    //                Initialized = true;
    //            }
    //        }
    //        if (Rz != .0f)
    //        {
    //            if (Initialized)
    //            {
    //                Matrix4X4 Temp = new Matrix4X4();
    //                Temp.Rotate(2, Rz);
    //                Multiply(Temp);
    //            }
    //            else
    //            {
    //                Rotate(2, Rz);
    //                Initialized = true;
    //            }
    //        }
    //        if (Tx != 0.0f || Ty != 0.0f || Tz != 0.0f)
    //        {
    //            if (Initialized)
    //            {
    //                Matrix4X4 Temp = new Matrix4X4();
    //                Temp.Translate(Tx, Ty, Tz);
    //                Multiply(Temp);
    //            }
    //            else
    //            {
    //                Translate(Tx, Ty, Tz);
    //                Initialized = true;
    //            }

    //            if (!Initialized)
    //            {
    //                Identity();
    //            }
    //        }
    //    }

    //    public void PrepareMatrix(Vector3D pTranslateVector,
    //                                   Vector3D pRotateVector,
    //                                   Vector3D pScaleVector)
    //    {
    //        PrepareMatrix(pTranslateVector.x, pTranslateVector.y, pTranslateVector.z,
    //            pRotateVector.x, pRotateVector.y, pRotateVector.z,
    //            pScaleVector.x, pScaleVector.y, pScaleVector.z);
    //    }

    //    public void PrepareMatrixFromPositionAndDirection(Vector3D Position, Vector3D Direction)
    //    {
    //        // Setup translation part.
    //        Translate(Position);

    //        // Do orientation.
    //        Vector3D YAxis = Direction;
    //        YAxis.Normalize();

    //        // Generate a candidate for the x axis.

    //        // Try the world x axis first.
    //        Vector3D XAxis = new Vector3D(1.0f, 0.0f, 0.0f);

    //        double Threshold = (double)System.Math.Cos(Helper.DegToRad<T>(10.0));

    //        if (YAxis.Dot(XAxis) > Threshold)
    //        {
    //            // Too close so use the world y axis.
    //            XAxis = new Vector3D(0.0f, 1.0f, 0.0f);
    //        }

    //        // Get the z axis from the cross product.
    //        Vector3D ZAxis = new Vector3D();
    //        ZAxis.Cross(XAxis, YAxis);
    //        ZAxis.Normalize();

    //        // Get the true x axis from y and z.
    //        XAxis.Cross(YAxis, ZAxis);

    //        for (int i=0; i<3; i++)
    //        {
    //            SetElement(0, i, XAxis[i]);
    //            SetElement(1, i, YAxis[i]);
    //            SetElement(2, i, ZAxis[i]);
    //        }
        	
    //        SetElement(0, 3, 0.0f);
    //        SetElement(1, 3, 0.0f);
    //        SetElement(2, 3, 0.0f);

    //    }

    //    public void PrepareInvMatrix(double Tx, double Ty, double Tz,
    //                                      double Rx, double Ry, double Rz,
    //                                      double Sx, double Sy, double Sz)
    //    {
    //        Matrix4X4 M0 = new Matrix4X4();
    //        Matrix4X4 M1 = new Matrix4X4();
    //        Matrix4X4 M2 = new Matrix4X4();
    //        Matrix4X4 M3 = new Matrix4X4();
    //        Matrix4X4 M4 = new Matrix4X4();
    //        Matrix4X4 M5 = new Matrix4X4();
    //        Matrix4X4 M6 = new Matrix4X4();
    //        Matrix4X4 M7 = new Matrix4X4();

    //        M0.Scale(Sx, Sy, Sz);
    //        M1.Rotate(0, Rx);
    //        M2.Rotate(1, Ry);
    //        M3.Rotate(2, Rz);
    //        M4.Translate(Tx, Ty, Tz);
    //        // 4 * 3 * 2 * 1 * 0
    //        M5.Multiply(M4, M3);
    //        M6.Multiply(M5, M2);
    //        M7.Multiply(M6, M1);
    //        Multiply(M7, M0);
    //    }

    //    public void PrepareInvMatrix(Vector3D pTranslateVector,
    //                                   Vector3D pRotateVector,
    //                                   Vector3D pScaleVector)
    //    {
    //        PrepareInvMatrix(pTranslateVector.x, pTranslateVector.y, pTranslateVector.z,
    //                         pRotateVector.x, pRotateVector.y, pRotateVector.z,
    //                         pScaleVector.x, pScaleVector.y, pScaleVector.z);
    //    }

    //    public void TransformVector(double[] pChanged)
    //    {
    //        double[] Hold = (double[])pChanged.Clone();
    //        pChanged[0] = GetElement(0, 0) * Hold[0] + GetElement(0, 1) * Hold[1] + GetElement(0, 2) * Hold[2] + GetElement(0, 3) * Hold[3];
    //        pChanged[1] = GetElement(1, 0) * Hold[0] + GetElement(1, 1) * Hold[1] + GetElement(1, 2) * Hold[2] + GetElement(1, 3) * Hold[3];
    //        pChanged[2] = GetElement(2, 0) * Hold[0] + GetElement(2, 1) * Hold[1] + GetElement(2, 2) * Hold[2] + GetElement(2, 3) * Hold[3];
    //        pChanged[3] = GetElement(3, 0) * Hold[0] + GetElement(3, 1) * Hold[1] + GetElement(3, 2) * Hold[2] + GetElement(3, 3) * Hold[3];
    //    }

    //    public void TransformVector(ref Vector3D Changed)
    //    {
    //        Vector3D Original = Changed;
    //        TransformVector(out Changed, Original);
    //    }

    //    public void TransformVector(out Vector3D Changed, Vector3D Original)
    //    {
    //        TransformVector3X3(out Changed, Original);
    //        Changed.x += GetElement(3, 0);
    //        Changed.y += GetElement(3, 1);
    //        Changed.z += GetElement(3, 2);
    //    }

    //    public void TransformVector3X3(ref Vector3D Changed)
    //    {
    //        Vector3D Original = new Vector3D(Changed);
    //        TransformVector3X3(out Changed, Original);
    //    }

    //    public void TransformVector3X3(out Vector3D Changed, Vector3D Original)
    //    {
    //        Changed.x = GetElement(0, 0) * Original.x + GetElement(1, 0) * Original.y + GetElement(2, 0) * Original.z;
    //        Changed.y = GetElement(0, 1) * Original.x + GetElement(1, 1) * Original.y + GetElement(2, 1) * Original.z;
    //        Changed.z = GetElement(0, 2) * Original.x + GetElement(1, 2) * Original.y + GetElement(2, 2) * Original.z;
    //    }

    //    public uint ValidateMatrix()
    //    {
    //        if( GetElement(3, 0) == 0.0f && GetElement(3, 1) == 0.0f && GetElement(3, 2) == 0.0f && GetElement(3, 3) == 1.0f)
    //        {
    //            return 1;
    //        }

    //        return 0;
    //    }

    //    public static Matrix4X4 operator *(Matrix4X4 A, Matrix4X4 B)
    //    {
    //        Matrix4X4 Temp = new Matrix4X4(A);
    //        Temp.Multiply(B);
    //        return Temp;
    //    }

    //    public void Multiply(Matrix4X4 Two)
    //    {
    //        Matrix4X4 Hold = new Matrix4X4(this);
    //        Multiply(Hold, Two);
    //    }

    //    public void Multiply(Matrix4X4 One, Matrix4X4 Two)
    //    {
    //        if (this == One || this == Two)
    //        {
    //            throw new System.FormatException("Neither of the input parameters can be the same Matrix as this.");
    //        }

    //        for(int i = 0; i < 4; i++)
    //        {
    //            for(int j = 0; j < 4; j++)
    //            {
    //                SetElement(i, j, 0);
    //                for(int k = 0; k < 4; k++)
    //                {
    //                    AddElement(i, j, One.GetElement(i, k) * Two.GetElement(k, j));
    //                }
    //            }
    //        }
    //    }

    //    // Returns the X-axis vector from this matrix
    //    public void GetXAxisVector (Vector3D result)
    //    {
    //        // stored as row vectors
    //        result.x = GetElement(0, 0);
    //        result.y = GetElement(0, 1);
    //        result.z = GetElement(0, 2);
    //    }

    //    // Returns the Y-axis vector from this matrix
    //    public void GetYAxisVector (Vector3D result)
    //    {
    //        // stored as row vectors
    //        result.x = GetElement(1, 0);
    //        result.y = GetElement(1, 1);
    //        result.z = GetElement(1, 2);
    //    }

    //    // Returns the Z-axis vector from this matrix
    //    public void GetZAxisVector (Vector3D result)
    //    {
    //        // stored as row vectors
    //        result.x = GetElement(2, 0);
    //        result.y = GetElement(2, 1);
    //        result.z = GetElement(2, 2);
    //    }

    //    // Returns the translation from this matrix
    //    public void GetTranslation (Vector3D result)
    //    {
    //        // stored as row vectors
    //        result.x = GetElement(3, 0);
    //        result.y = GetElement(3, 1);
    //        result.z = GetElement(3, 2);
    //    }

    //    public void SetElements(Matrix4X4 CopyFrom)
    //    {
    //        SetElements(CopyFrom.GetElements());
    //    }

    //    public void SetElements(double[] pElements)
    //    {
    //        for(int i=0; i<16; i++)
    //        {
    //            m_pA[i] = pElements[i];
    //        }
    //    }

    //    public void SetElements(double A00_00, double A00_01, double A00_02, double A00_03,
    //                                 double A01_00, double A01_01, double A01_02, double A01_03,
    //                                 double A02_00, double A02_01, double A02_02, double A02_03,
    //                                 double A03_00, double A03_01, double A03_02, double A03_03)
    //    {
    //        int Offset = 0;
    //        m_pA[Offset++] = A00_00; m_pA[Offset++] = A00_01; m_pA[Offset++] = A00_02; m_pA[Offset++] = A00_03;
    //        m_pA[Offset++] = A01_00; m_pA[Offset++] = A01_01; m_pA[Offset++] = A01_02; m_pA[Offset++] = A01_03;
    //        m_pA[Offset++] = A02_00; m_pA[Offset++] = A02_01; m_pA[Offset++] = A02_02; m_pA[Offset++] = A02_03;
    //        m_pA[Offset++] = A03_00; m_pA[Offset++] = A03_01; m_pA[Offset++] = A03_02; m_pA[Offset++] = A03_03;
    //    }

    //    public void SetElements(float A00_00, float A00_01, float A00_02, float A00_03,
    //                                 float A01_00, float A01_01, float A01_02, float A01_03,
    //                                 float A02_00, float A02_01, float A02_02, float A02_03,
    //                                 float A03_00, float A03_01, float A03_02, float A03_03)
    //    {
    //        SetElements((double)A00_00, (double)A00_01, (double)A00_02, (double)A00_03,
    //                                 (double)A01_00, (double)A01_01, (double)A01_02, (double)A01_03,
    //                                 (double)A02_00, (double)A02_01, (double)A02_02, (double)A02_03,
    //                                 (double)A03_00, (double)A03_01, (double)A03_02, (double)A03_03);
    //    }

    //    public double[] GetElements()
    //    {
    //        return m_pA;
    //    }

    //}
};

//namespace NUnitReflexive
//{
//    [TestFixture]
//    public class Matrix4x4Test
//    {
//        System.Random TempRand = new Random();

//        static bool TestOne(double Tx, double Ty, double Tz)
//        {
//            return TestOne(Tx, Ty, Tz, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f);
//        }

//        static bool TestOne(double Tx, double Ty, double Tz, double Rx, double Ry, double Rz)
//        {
//            return TestOne(Tx, Ty, Tz, Rx, Ry, Rz, 1.0f, 1.0f, 1.0f);
//        }

//        static bool TestOne(double Tx, double Ty, double Tz, double Rx, double Ry, double Rz, double Sx, double Sy, double Sz)
//        {
//            Vector3D UnitVectorY = new Vector3D(0.0f, 1.0f, 0.0f);
//            Vector3D v1 = new Vector3D();
//            v1 = UnitVectorY;
//            Matrix4X4 NormalMatrix = new Matrix4X4();
//            Matrix4X4 InverseMatrixFromNormalMatrix = new Matrix4X4();
//            Matrix4X4 InverseMatrixCalculated = new Matrix4X4();

//            NormalMatrix.PrepareMatrix(Tx, Ty, Tz, Rx, Ry, Rz, Sx, Sy, Sz);
//            NormalMatrix.TransformVector(ref v1);

//            InverseMatrixFromNormalMatrix.GetInverse(NormalMatrix);
//            InverseMatrixFromNormalMatrix.TransformVector(ref v1);

//            // make sure they are the same within an error range
//            Assert.IsTrue(v1.Equals(UnitVectorY, .01f));

//            NormalMatrix.TransformVector(ref v1);

//            InverseMatrixCalculated.PrepareInvMatrix(-Tx, -Ty, -Tz, -Rx, -Ry, -Rz, 1.0f/Sx, 1.0f/Sy, 1.0f/Sz);
//            InverseMatrixCalculated.TransformVector(ref v1);

//            // make sure they are the same within an error range
//            Assert.IsTrue(v1.Equals(UnitVectorY, .001f));

//            // And just a bit more checking [7/26/2001] LBB
//            // and now just check that TransformVector is always working
//            NormalMatrix.PrepareMatrix(Tx, Ty, Tz, Rx, Ry, Rz, Sx, Sy, Sz);
//            NormalMatrix.TransformVector3X3(ref v1);
//            InverseMatrixCalculated.PrepareInvMatrix(-Tx, -Ty, -Tz, -Rx, -Ry, -Rz, 1.0f/Sx, 1.0f/Sy, 1.0f/Sz);
//            InverseMatrixCalculated.TransformVector3X3(ref v1);
//            Assert.IsTrue(v1.Equals(UnitVectorY, .001f));

//            NormalMatrix.PrepareMatrix(Tx, Ty, Tz, Rx, Ry, Rz, Sx, Sy, Sz);
//            NormalMatrix.TransformVector3X3(ref v1);
//            InverseMatrixCalculated.GetInverse(NormalMatrix);
//            InverseMatrixCalculated.TransformVector3X3(ref v1);
//            Assert.IsTrue(v1.Equals(UnitVectorY, .001f));

//            return true;
//        }

//        public double RandomDouble(System.Random Rand, double Min, double Max)
//        {
//            return (double)Rand.NextDouble() * (Max - Min) + Min;
//        }

//        [Test]
//        public void MatrixColumnMajor()
//        {
//            // Make sure our matrix is set up colum major like opengl. LBB [7/11/2003]
//            Matrix4X4 ColumnMajorRotationMatrix = new Matrix4X4();
//            ColumnMajorRotationMatrix.Rotate(1, .2345f);
//            Matrix4X4 ColumnMajorTransLationMatrix = new Matrix4X4();
//            ColumnMajorTransLationMatrix.Translate(.2342f, 234234.734f, 223.324f);
//            Matrix4X4 ColumnMajorAccumulatedMatrix = new Matrix4X4();
//            ColumnMajorAccumulatedMatrix.Multiply(ColumnMajorRotationMatrix, ColumnMajorTransLationMatrix);
//            double[] KnownMatrixFormFloats = 
//            {
//                .972631f,	0.0f,		-.232357f, 0.0f, 
//                0.0f,		1.0f,		0.0f,		0.0f,
//                .232357f,	0.0f,		.972631f,	0.0f, 
//                .2342f,		234234.73f,	223.324f,	1.0f 
//            };
//            Matrix4X4 KnownMatrixForm = new Matrix4X4();
//            KnownMatrixForm.SetElements(KnownMatrixFormFloats);
//            Assert.IsTrue(KnownMatrixForm.Equals(ColumnMajorAccumulatedMatrix, .01f));
//        }

//        [Test]
//        public void RotateAboutXAxis()
//        {
//            Vector3D RotateAboutX = new Vector3D(1.0f, 0.0f, 0.0f);
//            Matrix4X4 RotationMatrix = new Matrix4X4();
//            RotationMatrix.Rotate(RotateAboutX, (double)(System.Math.PI / 2));
//            Vector3D PointToRotate = new Vector3D(0, 40, 0);
//            RotationMatrix.TransformVector(ref PointToRotate);
//            Assert.IsTrue(PointToRotate.Equals(new Vector3D(0, 0, 40), .01f));
//        }
//        [Test]
//        public void RotateAboutYAxis()
//        {
//            Vector3D RotateAboutY = new Vector3D(0.0f, 1.0f, 0.0f);
//            Matrix4X4 RotationMatrix = new Matrix4X4();
//            RotationMatrix.Rotate(RotateAboutY, (double)(System.Math.PI / 2));
//            Vector3D PointToRotate = new Vector3D(40, 0, 0);
//            RotationMatrix.TransformVector(ref PointToRotate);
//            Assert.IsTrue(PointToRotate.Equals(new Vector3D(0, 0, -40), .01f));
//        }
//        [Test]
//        public void RotateAboutZAxis()
//        {
//            Vector3D RotateAboutZ = new Vector3D(0.0f, 0.0f, 1.0f);
//            Matrix4X4 RotationMatrix = new Matrix4X4();
//            RotationMatrix.Rotate(RotateAboutZ, (double)(System.Math.PI / 2));
//            Vector3D PointToRotate = new Vector3D(40, 0, 0);
//            RotationMatrix.TransformVector(ref PointToRotate);
//            Assert.IsTrue(PointToRotate.Equals(new Vector3D(0, 40, 0), .01f));
//        }

//        [Test]
//        public void ConcatenatedMatrixIsSameAsIndividualMatrices ()
//        {
//            // Make sure that pushing a concatenated matrix is the same as through a bunch of individual matrices [7/30/2001] LBB
//            uint NumTransforms = (uint)TempRand.Next(10) + 4;
//            Vector3D UnitVectorY = new Vector3D(0.0f, 1.0f, 0.0f);
//            Vector3D EachMatrixVector = new Vector3D();
//            Vector3D ConcatenatedMatrixVector = new Vector3D();
//            EachMatrixVector = UnitVectorY;
//            Matrix4X4 ConcatenatedMatrix = new Matrix4X4();
//            ConcatenatedMatrix.Identity();
//            Matrix4X4[] pTranforms = new Matrix4X4[NumTransforms];
//            for (int i = 0; i < NumTransforms; i++)
//            {
//                pTranforms[i] = new Matrix4X4();
//            }

//            for (uint CurTransform = 0; CurTransform < NumTransforms; CurTransform++)
//            {
//                uint Axis = (uint)TempRand.Next(3);
//                double Rotation = RandomDouble(TempRand, 0.01f, 2 * (double)System.Math.PI);
//                Vector3D Translation;
//                Translation.x = RandomDouble(TempRand, -10000.0f, 10000.0f);
//                Translation.y = RandomDouble(TempRand, -10000.0f, 10000.0f);
//                Translation.z = RandomDouble(TempRand, -10000.0f, 10000.0f);
//                if (TempRand.Next(2) != 0)
//                {
//                    pTranforms[CurTransform].Rotate(Axis, Rotation);
//                }
//                else
//                {
//                    pTranforms[CurTransform].Translate(Translation);
//                }

//                pTranforms[CurTransform].TransformVector(ref EachMatrixVector);
//                ConcatenatedMatrix.Multiply(pTranforms[CurTransform]); // this is working for rotation
//                ConcatenatedMatrixVector = UnitVectorY;
//                ConcatenatedMatrix.TransformVector(ref ConcatenatedMatrixVector);
//                Assert.IsTrue(ConcatenatedMatrixVector.Equals(EachMatrixVector, .01f));
//            }
//        }

//        [Test]
//        public void PrepareAsInveresAndInverseAreSame()
//        {
//            //***************************************
//            TestOne(0.0f, 0.0f, 0.0f, (double)System.Math.PI / 2, 0, 0);
//            TestOne(0.0f, 0.0f, 0.0f, 0, (double)System.Math.PI / 2, 0);
//            TestOne(0.0f, 0.0f, 0.0f, 0, 0, (double)System.Math.PI / 2);

//            //***************************************
//            TestOne(5.0f, 50.0f, 10.0f);

//            //***************************************
//            TestOne(5.0f, 50.0f, 10.0f, 3.0f, 2.0f, 33333.0f);

//            // Let's just sling a bunch of test [7/26/2001] LBB
//            TestOne(10.0f, 0.0f, 0.0f, 0.0f, 0.0f, (double)System.Math.PI / 2.0f);
//            TestOne(0.0f, 10.0f, 0.0f, 0.0f, 0.0f, (double)System.Math.PI / 2.0f);
//            TestOne(0.0f, 0.0f, 10.0f, 0.0f, 0.0f, (double)System.Math.PI / 2.0f);

//            TestOne(10.0f, 0.0f, 0.0f, 0.0f, (double)System.Math.PI / 2.0f, 0.0f);
//            TestOne(0.0f, 10.0f, 0.0f, 0.0f, (double)System.Math.PI / 2.0f, 0.0f);
//            TestOne(0.0f, 0.0f, 10.0f, 0.0f, (double)System.Math.PI / 2.0f, 0.0f);

//            TestOne(10.0f, 0.0f, 0.0f, (double)System.Math.PI / 2.0f, 0.0f, 0.0f);
//            TestOne(0.0f, 10.0f, 0.0f, (double)System.Math.PI / 2.0f, 0.0f, 0.0f);
//            TestOne(0.0f, 0.0f, 10.0f, (double)System.Math.PI / 2.0f, 0.0f, 0.0f);

//            for (uint i = 0; i < 100; i++)
//            {
//                TestOne(
//                    RandomDouble(TempRand, -1000.0f, 1000.0f),
//                    RandomDouble(TempRand, -1000.0f, 1000.0f),
//                    RandomDouble(TempRand, -1000.0f, 1000.0f),

//                    RandomDouble(TempRand, -2.0f * (double)System.Math.PI, 2.0f * (double)System.Math.PI),
//                    RandomDouble(TempRand, -2.0f * (double)System.Math.PI, 2.0f * (double)System.Math.PI),
//                    RandomDouble(TempRand, -2.0f * (double)System.Math.PI, 2.0f * (double)System.Math.PI),

//                    RandomDouble(TempRand, 0.001f, 1000.0f),
//                    RandomDouble(TempRand, 0.001f, 1000.0f),
//                    RandomDouble(TempRand, 0.001f, 1000.0f));
//            }
//        }

//        [Test]
//        public void PrepareMatrixFromPositionAndDirection()
//        {
//            // Test the PrepareMatrixFromPositionAndDirection function.
//            Matrix4X4 TestA = new Matrix4X4();
//            TestA.PrepareMatrixFromPositionAndDirection(new Vector3D(1.0f, 2.0f, 3.0f), new Vector3D(1.0f, 0.0f, 1.0f));
//            double[] TestACorrectResultFloats = 
//            {
//                .7073f,		0.0f,		-.7073f,	0.0f, 
//                .7072f,		0.0f,		.7072f,		0.0f,
//                0.0f,		-1.0f,		0.0f,		0.0f, 
//                1.0f,		2.0f,		3.0f,		1.0f 
//            };
//            Matrix4X4 TestACorrectResult = new Matrix4X4();
//            TestACorrectResult.SetElements(TestACorrectResultFloats);
//            Assert.IsTrue(TestACorrectResult.Equals(TestA, .01f));

//            Matrix4X4 TestB = new Matrix4X4();
//            TestB.PrepareMatrixFromPositionAndDirection(new Vector3D(1.0f, 2.0f, 3.0f), new Vector3D(1.0f, 0.0f, 0.01f));
//            double[] TestBCorrectResultFloats = 
//            {
//                0.0f,		1.0f,		0.0f,		0.0f, 
//                1.0f,		0.0f,		.0099f,		0.0f,
//                0.0099f,	0.0f,		-1.0f,		0.0f, 
//                1.0f,		2.0f,		3.0f,		1.0f 
//            };
//            Matrix4X4 TestBCorrectResult = new Matrix4X4();
//            TestBCorrectResult.SetElements(TestBCorrectResultFloats);
//            Assert.IsTrue(TestBCorrectResult.Equals(TestB, .01f));
//        }
//    }
//}
