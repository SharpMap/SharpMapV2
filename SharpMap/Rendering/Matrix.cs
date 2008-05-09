// Copyright 2007-2008 Rory Plaire (codekaizen@gmail.com)
//
// Based initially on JAMA : A Java Matrix Package by 
// National Institute of Standards and Technology (NIST) 
// and The MathWorks, a public-domain work.

using System;
using System.Collections.Generic;
using System.Text;
using NPack;
using NPack.Interfaces;

namespace SharpMap.Rendering
{
    /// <summary>
    /// A matrix. The fundamental unit of computation in linear algebra.
    /// </summary>
    public class Matrix : ITransformMatrix<DoubleComponent, Vector, Matrix>, 
                          ITransformMatrix<DoubleComponent>
    {
        internal readonly DoubleComponent[] _elements;
        private readonly Int32 _rowCount;
        private readonly Int32 _columnCount;
        private readonly MatrixFormat _format;

        #region Matrix Constructors

        /// <summary>
        /// Creates a zero rectangular Matrix of the given <paramref name="rowCount"/> by <paramref name="columnCount"/>.
        /// </summary>
        /// <param name="format">The format of the matrix, either row-major or column-major.</param>
        /// <param name="rowCount">Number of rows.</param>
        /// <param name="columnCount">Number of columns.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when rowCount or columnCount is less than 1.</exception>
        public Matrix(MatrixFormat format, Int32 rowCount, Int32 columnCount)
        {
            if (rowCount <= 0)
            {
                throw new ArgumentOutOfRangeException("rowCount", rowCount, "Cannot create a matrix without rows.");
            }

            if (columnCount <= 0)
            {
                throw new ArgumentOutOfRangeException("columnCount", columnCount,
                                                      "Cannot create a matrix without columns.");
            }

            _rowCount = rowCount;
            _columnCount = columnCount;

            _format = format;
            _elements = new DoubleComponent[rowCount * columnCount];
        }

        /// <summary>
        /// Creates a rectangular Matrix of the given <paramref name="rowCount"/> 
        /// by <paramref name="columnCount"/> with 
        /// <paramref name="value"/> assigned to the diagonal.
        /// </summary>
        /// <param name="format">The format of the matrix, either row-major or column-major.</param>
        /// <param name="rowCount">Number of rows.</param>
        /// <param name="columnCount">Number of columns.</param>
        /// <param name="value">The value to assign to the diagonal.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when rowCount or columnCount is less than 1.</exception>
        public Matrix(MatrixFormat format, Int32 rowCount, Int32 columnCount, DoubleComponent value)
            : this(format, rowCount, columnCount)
        {
            for (Int32 i = 0; i < rowCount; i++)
            {
                _elements[i * rowCount + i] = value;
            }
        }

        /// <summary>
        /// Creates a new matrix with the given <paramref name="elements"/>.
        /// </summary>
        /// <param name="format">The format of the matrix, either row-major or column-major.</param>
        /// <param name="elements">The elements to use to fill the matrix.</param>
        /// <exception cref="ArgumentNullException">Thrown when parameter 'elements' is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when length of 'elements' is 0 
        /// or when the first element of 'elements' is an array of length 0.</exception>
        /// <exception cref="ArgumentException">When the arrays of 'elements' are not all the same length.</exception>
        public Matrix(MatrixFormat format, DoubleComponent[][] elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }

            if (elements.Length == 0 || elements[0] == null || elements[0].Length == 0)
            {
                throw new ArgumentException("Must have at least one element.", "elements");
            }

            _format = format;

            _rowCount = format == MatrixFormat.RowMajor ? elements.Length : elements[0].Length;
            _columnCount = format == MatrixFormat.ColumnMajor ? elements.Length : elements[0].Length;

            _elements = new DoubleComponent[_rowCount * _columnCount];

            for (Int32 i = 0; i < (format == MatrixFormat.RowMajor ? _rowCount : _columnCount); i++)
            {
                if (_columnCount != elements[i].Length)
                {
                    throw new ArgumentException(
                        "Cannot create a matrix which has rows having different numbers of columns.");
                }

                for (Int32 j = 0; j < (format == MatrixFormat.ColumnMajor ? _rowCount : _columnCount); j++)
                {
                    this[i, j] = elements[i][j];
                }
            }
        }

        /// <summary>
        /// Creates a new matrix as a copy of the given matrix.
        /// </summary>
        /// <param name="matrixToCopy">The matrix to copy.</param>
        public Matrix(IMatrix<DoubleComponent> matrixToCopy)
            : this(matrixToCopy.Format, matrixToCopy.RowCount, matrixToCopy.ColumnCount)
        {
            Int32 rowCount = matrixToCopy.RowCount;
            Int32 colCount = matrixToCopy.ColumnCount;

            for (Int32 i = 0; i < rowCount; i++)
            {
                for (Int32 j = 0; j < colCount; j++)
                {
                    this[i, j] = matrixToCopy[i, j];
                }
            }
        }

        #endregion

        #region GetHashCode

        public override Int32 GetHashCode()
        {
            return (Int32)Math.Pow(RowCount, ColumnCount);
        }

        #endregion

        #region ToString

        public override String ToString()
        {
            StringBuilder buffer = new StringBuilder();

            for (Int32 i = 0; i < RowCount; i++)
            {
                buffer.Append("[ ");

                for (Int32 j = 0; j < ColumnCount; j++)
                {
                    buffer.AppendFormat("{0:N}, ", this[i, j]);
                }

                buffer.Length -= 2;
                buffer.Append(" ]  ");
            }

            buffer.Length -= 2;

            return buffer.ToString();
        }

        #endregion

        /// <summary>
        /// Creates an element-by-element copy of the matrix.
        /// </summary>
        public Matrix Clone()
        {
            Matrix clone = new Matrix(Format, RowCount, ColumnCount);

            MatrixProcessor.SetMatrix(this, clone);

            return clone;
        }

        #region Equality Computation

        public override Boolean Equals(Object obj)
        {
            return Equals(obj as Matrix);
        }

        public Boolean Equals(Matrix other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            if (RowCount != other.RowCount || ColumnCount != other.ColumnCount)
            {
                return false;
            }

            for (Int32 i = 0; i < RowCount; i++)
            {
                for (Int32 j = 0; j < ColumnCount; j++)
                {
                    if (!this[i, j].Equals(other[i, j]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Compares two <see cref="Matrix"/> instances for element-by-element
        /// equality.
        /// </summary>
        /// <param name="left">The left hand side of the equality comparison.</param>
        /// <param name="right">The right hand side of the equality comparison.</param>
        /// <returns>
        /// <see langword="true"/> if the matrixes are element-by-element equal, <see langword="false"/> otherwise.
        /// </returns>
        public static Boolean operator ==(Matrix left, Matrix right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares two <see cref="Matrix"/> instances element-by-element for 
        /// inequality.
        /// </summary>
        /// <param name="left">The left hand side of the inequality comparison.</param>
        /// <param name="right">The right hand side of the inequality comparison.</param>
        /// <returns>
        /// <see langword="true"/> if the matrixes are not equal in an element-by-element comparison, 
        /// <see langword="false"/> if they are equal.
        /// </returns>
        public static Boolean operator !=(Matrix left, Matrix right)
        {
            return !Equals(left, right);
        }

        #endregion

        #region Element Access

        /// <summary>
        /// Indexer for the matrix.
        /// </summary>
        /// <param name="row">The row to access.</param>
        /// <param name="column">The column to access.</param>
        /// <returns>The value at the given (row, column) of the matrix.</returns>
        public DoubleComponent this[Int32 row, Int32 column]
        {
            get
            {
                if (row < 0 || row >= RowCount)
                {
                    throw new ArgumentOutOfRangeException("row", row, "Row indexer must be between 0 and RowCount.");
                }

                if (column < 0 || column >= ColumnCount)
                {
                    throw new ArgumentOutOfRangeException("column", column,
                                                          "Column indexer must be between 0 and ColumnCount.");
                }

                Int32 index = computeIndex(row, column);

                return _elements[index];
            }
            set
            {
                if (row < 0 || row >= RowCount)
                {
                    throw new ArgumentOutOfRangeException("row", row, "Row indexer must be between 0 and RowCount.");
                }

                if (column < 0 || column >= ColumnCount)
                {
                    throw new ArgumentOutOfRangeException("column", column,
                                                          "Column indexer must be between 0 and ColumnCount.");
                }

                Int32 index = computeIndex(row, column);

                _elements[index] = value;
            }
        }

        #endregion

        #region Identifying Matrix Properties

        /// <summary>
        /// Gets the number of rows in the matrix.
        /// </summary>
        public Int32 RowCount
        {
            get { return _rowCount; }
        }

        /// <summary>
        /// Gets the number of columns in the matrix.
        /// </summary>
        public Int32 ColumnCount
        {
            get { return _columnCount; }
        }

        /// <summary>
        /// Gets <see langword="true"/> if the <see cref="Matrix"/> is square, false if not.
        /// </summary>
        public Boolean IsSquare
        {
            get { return (RowCount == ColumnCount); }
        }

        /// <summary>
        /// Gets <see langword="true"/> if the <see cref="Matrix"/> is symmetrical, false if not.
        /// </summary>
        public Boolean IsSymmetrical
        {
            get
            {
                if (IsSquare)
                {
                    for (Int32 i = 0; i < _rowCount; i++)
                    {
                        for (Int32 j = 0; j <= i; j++)
                        {
                            if (!this[i, j].Equals(this[j, i]))
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets true if the matrix is singular (non-invertible).
        /// </summary>
        public Boolean IsSingular
        {
            get { return !MatrixProcessor.GetLuDecomposition(this).IsNonsingular; }
        }

        /// <summary>
        /// Gets true if the matrix is invertible (non-singular).
        /// </summary>
        public Boolean IsInvertible
        {
            get { return !IsSingular; }
        }

        /// <summary>
        /// Gets the determinant if the <see cref="Matrix"/> instance is square.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if matrix is not sqare.</exception>
        public Double Determinant
        {
            get { return MatrixProcessor.Determinant(this); }
        }

        /// <summary>
        /// Gets a <see cref="MatrixFormat"/> value indicating how the elements are stored.
        /// </summary>
        public MatrixFormat Format
        {
            get { return _format; }
        }

        #endregion

        #region Matrix Operations

        /// <summary>
        /// Gets a submatrix.
        /// </summary>
        /// <param name="i0">Initial row index.</param>
        /// <param name="i1">Final row index.</param>
        /// <param name="j0">Initial column index.</param>
        /// <param name="j1">Final column index.</param>
        /// <returns>
        /// A submatrix with rows given by the rows <paramref name="i0" />
        /// through <paramref name="i1"/>
        /// and columns <paramref name="j0"/> through <paramref name="j1"/>.
        /// </returns>
        public Matrix GetMatrix(Int32 i0, Int32 i1, Int32 j0, Int32 j1)
        {
            Matrix x = new Matrix(Format, i1 - i0 + 1, j1 - j0 + 1);

            for (Int32 i = i0; i <= i1; i++)
            {
                for (Int32 j = j0; j <= j1; j++)
                {
                    x[i - i0, j - j0] = this[i, j];
                }
            }

            return x;
        }

        /// <summary>
        /// Gets a submatrix.
        /// </summary>
        /// <param name="rowIndicies">The indexes of the rows to include.</param>
        /// <param name="j0">The starting column to include.</param>
        /// <param name="j1">The ending column to include.</param>
        /// <returns>
        /// A submatrix with rows given by <paramref name="rowIndicies"/> 
        /// and columns <paramref name="j0"/> through <paramref name="j1"/>.
        /// </returns>
        public Matrix GetMatrix(Int32[] rowIndicies, Int32 j0, Int32 j1)
        {
            if (j0 > j1)
            {
                throw new ArgumentException("Column index j0 must be less than column index j1");
            }

            if (j0 < 0 || j0 >= ColumnCount)
            {
                throw new ArgumentOutOfRangeException("j0", j0,
                                                      "Parameter must be at least 0 and less than matrix column count.");
            }

            if (j1 < 0 || j1 >= ColumnCount)
            {
                throw new ArgumentOutOfRangeException("j1", j1,
                                                      "Parameter must be at least j0 + 1 and less than matrix column count.");
            }

            Matrix sub = new Matrix(Format, rowIndicies.Length, j1 - j0 + 1);

            for (Int32 i = 0; i < rowIndicies.Length; i++)
            {
                for (Int32 j = j0; j <= j1; j++)
                {
                    if (rowIndicies[i] < 0 || rowIndicies[i] >= RowCount)
                    {
                        throw new ArgumentOutOfRangeException(
                            String.Format("rowIndicies[{0}]", i), rowIndicies[i],
                            "Row index must be at least 0 and less than matrix row count");
                    }

                    sub[i, j - j0] = this[rowIndicies[i], j];
                }
            }

            return sub;
        }

        /// <summary>
        /// Returns a transpose of the <see cref="Matrix"/>.
        /// </summary>
        /// <value>The rows-for-columns, columns-for-rows transpose of A.</value>
        /// <returns>AT, the transpose of matrix A.</returns>
        public Matrix Transpose()
        {
            return MatrixProcessor.Transpose(this);
        }

        /// <summary>
        /// Gets the inverse of the <see cref="Matrix"/> if it is square and non-singular, 
        /// the pseudo-inverse if it is non-square, and null if it is singluar (non-invertible).
        /// </summary>
        public Matrix Inverse
        {
            get { return MatrixProcessor.Invert(this); }
        }

        /// <summary>
        /// Scales the elements in the linear transformation by the given <paramref name="amount"/>.
        /// </summary>
        /// <param name="amount">
        /// Amount of scale to apply uniformly in all dimensions.
        /// </param>
        public void Scale(DoubleComponent amount)
        {
            Scale(amount, MatrixOperationOrder.Default);
        }

        /// <summary>
        /// Scales the elements in the linear transformation by the given <paramref name="amount"/>.
        /// </summary>
        /// <param name="amount">
        /// Amount of scale to apply uniformly in all dimensions.
        /// </param>
        /// <param name="order">The order to apply the transform in.</param>
        public virtual void Scale(DoubleComponent amount, MatrixOperationOrder order)
        {
            Vector scaleVector = new Vector(Format == MatrixFormat.ColumnMajor ? RowCount : ColumnCount);

            for (Int32 i = 0; i < scaleVector.ComponentCount; i++)
            {
                scaleVector[i] = amount;
            }

            Scale(scaleVector, MatrixOperationOrder.Default);
        }

        /// <summary>
        /// Scales the elements in the linear transformation by the given <paramref name="scaleVector"/>.
        /// </summary>
        /// <param name="scaleVector">
        /// Amount of scale to apply on a column-by-column basis.
        /// </param>
        public void Scale(Vector scaleVector)
        {
            Scale(scaleVector, MatrixOperationOrder.Default);
        }

        /// <summary>
        /// Scales the elements in the linear transformation by the given <paramref name="scaleVector"/>.
        /// </summary>
        /// <param name="scaleVector">Amount of scale to apply on a column-by-column basis.</param>
        /// <param name="order">The order to apply the transform in.</param>
        public virtual void Scale(Vector scaleVector, MatrixOperationOrder order)
        {
            Matrix scale = new Matrix(Format, RowCount, ColumnCount, new DoubleComponent(1));
            MatrixProcessor.Scale(scale, scaleVector);

            Matrix result;

            if (order == MatrixOperationOrder.Prepend)
            {
                if (Format == MatrixFormat.RowMajor)
                {
                    result = MatrixProcessor.Multiply(scale, this);
                }
                else
                {
                    result = MatrixProcessor.Multiply(this, scale);
                }
            }
            else
            {
                if (Format == MatrixFormat.RowMajor)
                {
                    result = MatrixProcessor.Multiply(this, scale);
                }
                else
                {
                    result = MatrixProcessor.Multiply(scale, this);
                }
            }

            MatrixProcessor.SetMatrix(result, this);
        }

        /// <summary>
        /// Applies a shear to the transform by appending the shear to the <see cref="AffineMatrix{T}"/>.
        /// </summary>
        /// <param name="shearVector">The vector used to compute the shear.</param>
        public void Shear(Vector shearVector)
        {
            Shear(shearVector, MatrixOperationOrder.Default);
        }

        /// <summary>
        /// Applies a shear to the transform, either before or after this <see cref="AffineMatrix{T}"/>.
        /// </summary>
        /// <param name="shearVector">The vector used to compute the shear.</param>
        /// <param name="order">The order to apply the transform in.</param>
        public virtual void Shear(Vector shearVector, MatrixOperationOrder order)
        {
            Matrix shear = new Matrix(Format, RowCount, ColumnCount, new DoubleComponent(1));
            MatrixProcessor.Shear(shear, shearVector);

            Matrix result;

            if (order == MatrixOperationOrder.Prepend)
            {
                if (Format == MatrixFormat.RowMajor)
                {
                    result = MatrixProcessor.Multiply(shear, this);
                }
                else
                {
                    result = MatrixProcessor.Multiply(this, shear);
                }
            }
            else
            {
                if (Format == MatrixFormat.RowMajor)
                {
                    result = MatrixProcessor.Multiply(this, shear);
                }
                else
                {
                    result = MatrixProcessor.Multiply(shear, this);
                }
            }

            MatrixProcessor.SetMatrix(result, this);
        }

        /// <summary>
        /// Rotates the affine transform around the given <paramref name="axis"/>.
        /// </summary>
        /// <param name="axis">The axis to rotate around.</param>
        /// <param name="radians">Angle to rotate through.</param>
        public virtual void RotateAlong(Vector axis, Double radians)
        {
            MatrixProcessor.Rotate(this, axis, radians);
        }

        /// <summary>
        /// Rotates the affine transform around the given <paramref name="axis"/>.
        /// </summary>
        /// <param name="axis">The axis to rotate around.</param>
        /// <param name="radians">Angle to rotate through.</param>
        /// <param name="order">The order to apply the transform in.</param>
        public virtual void RotateAlong(Vector axis, Double radians, MatrixOperationOrder order)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Applies this transform to the given <paramref name="input"/> matrix.
        /// </summary>
        /// <param name="input">Matrix to transform.</param>
        /// <returns>The multiplication of this transform matrix with the input matrix, 
        /// with the transform on the left-hand side of the operation.</returns>
        public Matrix TransformMatrix(Matrix input)
        {
            return Multiply(input);
        }

        ///// <summary>
        ///// Applies this transform to the given <paramref name="input"/> matrix.
        ///// </summary>
        ///// <param name="input">Matrix to transform.</param>
        ///// <returns>The multiplication of this transform matrix with the input matrix, 
        ///// with the transform on the left-hand side of the operation.</returns>
        //public void TransformMatrix(DoubleComponent[][] input)
        //{
        //    MatrixProcessor.Multiply(this, input);
        //}

        /// <summary>
        /// Applies this transform to the given <paramref name="input"/> vector.
        /// </summary>
        /// <param name="input">Vector to transform.</param>
        /// <returns>
        /// The multiplication of this transform matrix with the input vector.
        /// </returns>
        public Vector TransformVector(Vector input)
        {
            DoubleComponent[] elements = new DoubleComponent[input.ComponentCount];
            Array.Copy(input.Components, elements, input.ComponentCount);

            if (Format == MatrixFormat.RowMajor)
            {
                MatrixProcessor.Multiply(elements, this);
            }
            else
            {
                MatrixProcessor.Multiply(this, elements);
            }

            return new Vector(elements);
        }

        /// <summary>
        /// Applies this transform to the given <paramref name="input"/> vectors.
        /// </summary>
        /// <param name="input">Set of vectors to transform.</param>
        public IEnumerable<Vector> TransformVectors(IEnumerable<Vector> input)
        {
            if (Format == MatrixFormat.RowMajor)
            {
                return MatrixProcessor.Multiply(input, this);
            }
            else
            {
                return MatrixProcessor.Multiply(this, input);
            }
        }

        /// <summary>
        /// Applies this transform to the given <paramref name="input"/> vector.
        /// </summary>
        /// <param name="input">Vector to transform.</param>
        public void TransformVector(DoubleComponent[] input)
        {
            if (Format == MatrixFormat.RowMajor)
            {
                MatrixProcessor.Multiply(input, this);
            }
            else
            {
                MatrixProcessor.Multiply(this, input);
            }
        }

        /// <summary>
        /// Applies this transform to the given <paramref name="input"/> vectors.
        /// </summary>
        /// <param name="input">Set of vectors to transform.</param>
        public void TransformVectors(IEnumerable<DoubleComponent[]> input)
        {
            foreach (DoubleComponent[] v in input)
            {
                TransformVector(v);
            }
        }

        #region Arithmetic Operations

        /// <summary>
        /// Returns a copy of the matrix with all the elements negated.
        /// </summary>
        /// <returns>For matrix A, -A.</returns>
        public Matrix Negative()
        {
            return MatrixProcessor.Negate(this);
        }

        /// <summary>
        /// Returns a matrix which is the result of the instance plus the <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The matrix to add.</param>
        /// <returns>For matrix A, A + value.</returns>
        public Matrix Add(Matrix value)
        {
            return MatrixProcessor.Add(this, value);
        }

        /// <summary>
        /// Returns a matrix which is the result of the instance minus the <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The matrix to subtract.</param>
        /// <returns>For matrix A, A - value.</returns>
        public Matrix Subtract(Matrix value)
        {
            return MatrixProcessor.Subtract(this, value);
        }

        /// <summary>
        /// Scalar multiplication.
        /// </summary>
        /// <param name="value">Scalar to multiply.</param>
        /// <returns>For matrix A, (value)A.</returns>
        public Matrix Multiply(DoubleComponent value)
        {
            return MatrixProcessor.ScalarMultiply(this, value);
        }

        /// <summary>
        /// Matrix multiplication.
        /// </summary>
        /// <param name="value">Matrix to multiply.</param>
        /// <returns>For matrix A, (value)A.</returns>
        public Matrix Multiply(Matrix value)
        {
            return MatrixProcessor.Multiply(this, value);
        }

        #region Arithmetical Operators

        /// <summary>
        /// Negates the values of a matrix.
        /// </summary>
        /// <param name="value">The matrix to negate.</param>
        /// <returns>A negated matrix instance.</returns>
        public static Matrix operator -(Matrix value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            return MatrixProcessor.Negate(value);
        }

        /// <summary>
        /// Adds two matrixes element-by-element.
        /// </summary>
        /// <param name="lhs">The left hand side of the addition operation.</param>
        /// <param name="rhs">The right hand side of the addition operation.</param>
        /// <returns>The sum of <paramref name="lhs"/> and <paramref name="rhs"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the dimensions of <paramref name="lhs"/> and <paramref name="rhs"/>
        /// are not the same.
        /// </exception>
        public static Matrix operator +(Matrix lhs, Matrix rhs)
        {
            if (lhs == null)
            {
                throw new ArgumentNullException("lhs");
            }

            if (rhs == null)
            {
                throw new ArgumentNullException("rhs");
            }

            return MatrixProcessor.Add(lhs, rhs);
        }

        /// <summary>
        /// Subtracts two matrixes element-by-element.
        /// </summary>
        /// <param name="lhs">The left hand side of the subtraction operation.</param>
        /// <param name="rhs">The right hand side of the subtraction operation.</param>
        /// <returns>The difference of <paramref name="lhs"/> and <paramref name="rhs"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the dimensions of <paramref name="lhs"/> and <paramref name="rhs"/>
        /// are not the same.
        /// </exception>
        public static Matrix operator -(Matrix lhs, Matrix rhs)
        {
            if (lhs == null)
            {
                throw new ArgumentNullException("lhs");
            }

            if (rhs == null)
            {
                throw new ArgumentNullException("rhs");
            }

            return MatrixProcessor.Subtract(lhs, rhs);
        }

        /// <summary>
        /// Scalar multiplication.
        /// </summary>
        public static Matrix operator *(Matrix lhs, DoubleComponent rhs)
        {
            if (lhs == null)
            {
                throw new ArgumentNullException("lhs");
            }

            return MatrixProcessor.ScalarMultiply(lhs, rhs);
        }

        /// <summary>
        /// <see cref="Matrix"/> multiplication.
        /// </summary>
        public static Matrix operator *(Matrix lhs, Matrix rhs)
        {
            if (lhs == null)
            {
                throw new ArgumentNullException("lhs");
            }

            if (rhs == null)
            {
                throw new ArgumentNullException("rhs");
            }

            return MatrixProcessor.Multiply(lhs, rhs);
        }

        #endregion

        #endregion

        #endregion

        #region IHasZero<Matrix> Members

        /// <summary>
        /// Gets a zero matrix of the same row and column rank.
        /// </summary>
        /// <remarks>
        /// Uses <c>default(T)</c> as zero.
        /// </remarks>
        public Matrix Zero
        {
            get { return new Matrix(Format, RowCount, ColumnCount, new DoubleComponent()); }
        }

        #endregion

        #region IDivisible<Matrix> Members

        /// <summary>
        /// Matrix division is not defined. Throws a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <exception cref="NotSupportedException"/>
        Matrix IDivisible<Matrix>.Divide(Matrix a)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IHasOne<Matrix> Members

        /// <summary>
        /// Gets an identity matrix with the same row rank.
        /// </summary>
        public Matrix One
        {
            get { return Identity(Format, RowCount); }
        }

        #endregion

        #region Static Matrix Generators

        /// <summary>
        /// Generates a square matrix of the given <paramref name="rank"/> with
        /// the number 1 in each element of the diagonal.
        /// </summary>
        /// <param name="format">The format of the matrix, either row-major or column-major.</param>
        /// <param name="rank">Number of rows and columns of the <see cref="Matrix"/>.</param>
        /// <returns>An identiy matrix of the given rank.</returns>
        public static Matrix Identity(MatrixFormat format, Int32 rank)
        {
            return new Matrix(format, rank, rank, new DoubleComponent(1));
        }

        #endregion

        #region IComputable<Matrix> Members

        public Matrix Abs()
        {
            throw new NotImplementedException();
        }

        public Matrix Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region INegatable<Matrix> Members

        Matrix INegatable<Matrix>.Negative()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasZero<Matrix> Members

        Matrix IHasZero<Matrix>.Zero
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IDivisible<Matrix> Members

        public Matrix Divide(Matrix b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<Matrix> Members

        public Matrix Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        public Matrix Sqrt()
        {
            throw new NotImplementedException();
        }

        public Matrix Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        public Matrix Log()
        {
            throw new NotImplementedException();
        }

        public Matrix Exp()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable<Matrix> Members

        public Int32 CompareTo(Matrix other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<Matrix> Members

        Matrix IComputable<Matrix>.Abs()
        {
            throw new NotImplementedException();
        }

        Matrix IComputable<Matrix>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<Matrix> Members

        public Boolean GreaterThan(Matrix value)
        {
            throw new NotImplementedException();
        }

        public Boolean GreaterThanOrEqualTo(Matrix value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThan(Matrix value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThanOrEqualTo(Matrix value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<Matrix> Members

        Matrix IExponential<Matrix>.Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        Matrix IExponential<Matrix>.Sqrt()
        {
            throw new NotImplementedException();
        }

        Matrix IExponential<Matrix>.Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        Matrix IExponential<Matrix>.Log()
        {
            throw new NotImplementedException();
        }

        Matrix IExponential<Matrix>.Exp()
        {
            throw new NotImplementedException();
        }

        #endregion

        private Int32 computeIndex(Int32 row, Int32 column)
        {
            Int32 index;
            index = _format == MatrixFormat.RowMajor
                        ? row * _columnCount + column
                        : column * _rowCount + row;
            return index;
        }

        #region ITransformMatrix<DoubleComponent> Members

        void ITransformMatrix<DoubleComponent>.RotateAlong(IVector<DoubleComponent> axis, Double radians, MatrixOperationOrder order)
        {
            throw new NotImplementedException();
        }

        void ITransformMatrix<DoubleComponent>.RotateAlong(IVector<DoubleComponent> axis, Double radians)
        {
            throw new NotImplementedException();
        }

        void ITransformMatrix<DoubleComponent>.Scale(IVector<DoubleComponent> scaleVector, MatrixOperationOrder order)
        {
            throw new NotImplementedException();
        }

        void ITransformMatrix<DoubleComponent>.Scale(IVector<DoubleComponent> scaleVector)
        {
            throw new NotImplementedException();
        }

        void ITransformMatrix<DoubleComponent>.Shear(IVector<DoubleComponent> shearVector, MatrixOperationOrder order)
        {
            throw new NotImplementedException();
        }

        void ITransformMatrix<DoubleComponent>.Shear(IVector<DoubleComponent> shearVector)
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> ITransformMatrix<DoubleComponent>.TransformMatrix(IMatrix<DoubleComponent> input)
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent> ITransformMatrix<DoubleComponent>.TransformVector(IVector<DoubleComponent> input)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IVector<DoubleComponent>> ITransformMatrix<DoubleComponent>.TransformVectors(IEnumerable<IVector<DoubleComponent>> input)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMatrix<DoubleComponent> Members

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.Clone()
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.GetMatrix(Int32[] rowIndexes, Int32 startColumn, Int32 endColumn)
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.Inverse
        {
            get { throw new NotImplementedException(); }
        }

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.Transpose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable<IMatrix<DoubleComponent>> Members

        Boolean IEquatable<IMatrix<DoubleComponent>>.Equals(IMatrix<DoubleComponent> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable<IMatrix<DoubleComponent>> Members

        Int32 IComparable<IMatrix<DoubleComponent>>.CompareTo(IMatrix<DoubleComponent> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IComputable<IMatrix<DoubleComponent>>.Abs()
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IComputable<IMatrix<DoubleComponent>>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region INegatable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> INegatable<IMatrix<DoubleComponent>>.Negative()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> ISubtractable<IMatrix<DoubleComponent>>.Subtract(IMatrix<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasZero<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IHasZero<IMatrix<DoubleComponent>>.Zero
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IAddable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IAddable<IMatrix<DoubleComponent>>.Add(IMatrix<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IDivisible<IMatrix<DoubleComponent>>.Divide(IMatrix<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasOne<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IHasOne<IMatrix<DoubleComponent>>.One
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IMultipliable<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IMultipliable<IMatrix<DoubleComponent>>.Multiply(IMatrix<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<IMatrix<DoubleComponent>> Members

        Boolean IBooleanComparable<IMatrix<DoubleComponent>>.GreaterThan(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrix<DoubleComponent>>.GreaterThanOrEqualTo(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrix<DoubleComponent>>.LessThan(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IMatrix<DoubleComponent>>.LessThanOrEqualTo(IMatrix<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IExponential<IMatrix<DoubleComponent>>.Exp()
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IExponential<IMatrix<DoubleComponent>>.Log()
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IExponential<IMatrix<DoubleComponent>>.Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IExponential<IMatrix<DoubleComponent>>.Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IExponential<IMatrix<DoubleComponent>>.Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}