// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections;
using System.Collections.Generic;
using NPack;
using NPack.Interfaces;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;

namespace SharpMap.Rendering.Rendering3D
{
    public struct ViewPoint3D : IVectorD
    {
        private DoubleComponent _x, _y, _z;
        private bool _hasValue;

        public static readonly ViewPoint3D Empty = new ViewPoint3D();
        public static readonly ViewPoint3D Zero = new ViewPoint3D(0, 0, 0);

        public ViewPoint3D(DoubleComponent x, DoubleComponent y, DoubleComponent z)
        {
            _x = x;
            _y = y;
            _z = z;
            _hasValue = true;
        }

        public ViewPoint3D(double[] elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }

            if (elements.Length != 3)
            {   
                throw new ArgumentException("Elements array must have only 3 components");
            }

            _x = elements[0];
            _y = elements[1];
            _z = elements[2];
            _hasValue = true;
        }

        public override int GetHashCode()
        {
            return unchecked((int)_x ^ (int)_y ^ (int)_z);
        }

        public override string ToString()
        {
            return String.Format("[ViewPoint3D] ({0:N3}, {1:N3}, {2:N3})", _x, _y, _z);
        }

        public DoubleComponent X
        {
            get { return _x; }
        }

        public DoubleComponent Y
        {
            get { return _y; }
        }

        public DoubleComponent Z
        {
            get { return _z; }
        }

        #region IVector<DoubleComponent> Members

        public DoubleComponent[][] ElementArray
        {
            get { return new DoubleComponent[][] { new DoubleComponent[] { _x, _y, _z } }; }
        }

        public int ComponentCount
        {
            get { return 3; }
        }

        public DoubleComponent this[int element]
        {
            get
            {
                if (element == 0)
                {
                    return _x;
                }
                
                if (element == 1)
                {   
                    return _y;
                }

                if (element == 2)
                {
                    return _z;
                }

                throw new IndexOutOfRangeException("The element index must be either 0, 1 or 2 for a 3D point");
            }
            set
            {
                // setting members of a ValueType is not a good idea
                throw new NotSupportedException();
            }
        }

        public bool IsEmpty
        {
            get { return _hasValue; }
        }

        #endregion

        #region Equality Computation

        public override bool Equals(object obj)
        {
            if(obj is ViewPoint3D)
            {
                return Equals((ViewPoint3D) obj);
            }
            
            if (obj is IVectorD)
            {
                return Equals(obj as IVectorD);
            }

            if(obj is IMatrixD)
            {
                return Equals(obj as IMatrixD);
            }

            return false;
        }

        public bool Equals(ViewPoint3D point)
        {
            return point._hasValue == _hasValue &&
                   point._x.Equals(_x) &&
                   point._y.Equals(_y) &&
                   point._z.Equals(_z);
        }

        #region IEquatable<IMatrix<DoubleComponent>> Members

        public bool Equals(IMatrix<DoubleComponent> other)
        {
            if (other == null)
            {
                return false;
            }

            if (ComponentCount != other.ColumnCount)
            {
                return false;
            }

            for (int elementIndex = 0; elementIndex < ComponentCount; elementIndex++)
            {
                if (!this[elementIndex].Equals(other[0, elementIndex]))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        public static bool operator ==(ViewPoint3D vector1, IVectorD vector2)
        {
            return vector1.Equals(vector2);
        }

        public static bool operator !=(ViewPoint3D vector1, IVectorD vector2)
        {
            return !(vector1 == vector2);
        }

        #endregion

        #region IMatrix<DoubleComponent> Members

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.Clone()
        {
            return new Vector<DoubleComponent>(_x, _y, _z);
        }

        /// <summary>
        /// Gets a submatrix.
        /// </summary>
        /// <param name="rowIndexes">The indexes of the rows to include.</param>
        /// <param name="j0">The starting column to include.</param>
        /// <param name="j1">The ending column to include.</param>
        /// <returns>A submatrix with rows given by <paramref name="rowIndexes"/> and columns <paramref name="j0"/> 
        /// through <paramref name="j1"/>.</returns>
        IMatrixD IMatrixD.GetMatrix(int[] rowIndexes, int j0, int j1)
        {
            throw new NotImplementedException();
        }

        public double Determinant
        {
            get { throw new NotImplementedException(); }
        }

        public int ColumnCount
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSingular
        {
            get { throw new NotImplementedException(); }
        }

        public IMatrix<DoubleComponent> Inverse
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSquare
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSymmetrical
        {
            get { throw new NotImplementedException(); }
        }

        public int RowCount
        {
            get { throw new NotImplementedException(); }
        }

        public IMatrix<DoubleComponent> GetMatrix(int[] rowIndexes, int j0, int j1)
        {
            throw new NotImplementedException();
        }

        public DoubleComponent this[int row, int column]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public IMatrix<DoubleComponent> Transpose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IAddable<IMatrix<DoubleComponent>> Members

        public IMatrix<DoubleComponent> Add(IMatrix<DoubleComponent> a)
        {
            checkRank(a);

            DoubleComponent[] elements = a.Elements[0];
            return new ViewPoint3D(elements[0].Add(_x), elements[1].Add(_y), elements[2].Add(_z));
        }

        #endregion

        #region ISubtractable<IMatrix<DoubleComponent>> Members

        public IMatrix<DoubleComponent> Subtract(IMatrix<DoubleComponent> a)
        {
            checkRank(a);

            DoubleComponent[] elements = a.Elements[0];
            return new ViewPoint3D(elements[0].Subtract(_x), elements[1].Subtract(_y), elements[2].Subtract(_z));
        }

        #endregion

        #region IHasZero<IMatrix<DoubleComponent>> Members

        IMatrix<DoubleComponent> IHasZero<IMatrix<DoubleComponent>>.Zero
        {
            get { return Zero; }
        }

        #endregion

        #region INegatable<IMatrix<DoubleComponent>> Members

        public IMatrix<DoubleComponent> Negative()
        {
            return new ViewPoint3D(_x.Negative(), _y.Negative(), _z.Negative());
        }

        #endregion

        #region IMultipliable<IMatrix<DoubleComponent>> Members

        public IMatrix<DoubleComponent> Multiply(IMatrix<DoubleComponent> a)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<IMatrix<DoubleComponent>> Members

        public IMatrix<DoubleComponent> Divide(IMatrix<DoubleComponent> a)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IHasOne<IMatrix<DoubleComponent>> Members

        public IMatrix<DoubleComponent> One
        {
            get { return new ViewPoint3D(1, 1, 1); }
        }

        #endregion

        private void checkRank(IMatrixD a)
        {
            if (a.ColumnCount != ColumnCount)
            {
                throw new ArgumentException("Addend must have the same number of components as this vector.", "a");
            }

            if (a.RowCount != 1)
            {
                throw new ArgumentException("Addend must be a vector.", "a");
            }
        }

        #region IEnumerable<DoubleComponent> Members

        ///<summary>
        ///Returns an enumerator that iterates through the collection.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>1</filterpriority>
        IEnumerator<DoubleComponent> IEnumerable<DoubleComponent>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable Members

        ///<summary>
        ///Returns an enumerator that iterates through a collection.
        ///</summary>
        ///
        ///<returns>
        ///An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<DoubleComponent>) this).GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Creates a component-by-component copy of the vector.
        /// </summary>
        /// <returns>A copy of the vector.</returns>
        IVectorD IVectorD.Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the number of components in the vector.
        /// </summary>
        int IVectorD.ComponentCount
        {
            get { return IsEmpty ? 0 : 3; }
        }

        /// <summary>
        /// Gets or sets the vector component array.
        /// </summary>
        DoubleComponent[] IVectorD.Components
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the vector multiplied by -1.
        /// </summary>
        /// <returns>The vector when multiplied by -1.</returns>
        IVectorD IVectorD.Negative()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets a component in the vector.
        /// </summary>
        /// <param name="index">The index of the component.</param>
        /// <returns>The value of the component at the given <paramref name="index"/>.</returns>
        DoubleComponent IVectorD.this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the determinant for the matrix, if it exists.
        /// </summary>
        double IMatrixD.Determinant
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the number of columns in the matrix.
        /// </summary>
        int IMatrixD.ColumnCount
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the format of the matrix, either row-major or column-major.
        /// </summary>
        MatrixFormat IMatrixD.Format
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets true if the matrix is singular (non-invertable).
        /// </summary>
        bool IMatrixD.IsSingular
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets true if the matrix is invertable (non-singular).
        /// </summary>
        bool IMatrixD.IsInvertible
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the inverse of the matrix, if one exists.
        /// </summary>
        IMatrixD IMatrixD.Inverse
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets true if the matrix is square (<c>RowCount == ColumnCount != 0</c>).
        /// </summary>
        bool IMatrixD.IsSquare
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets true if the matrix is symmetrical.
        /// </summary>
        bool IMatrixD.IsSymmetrical
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the number of rows in the matrix.
        /// </summary>
        int IMatrixD.RowCount
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the elements in the matrix as an array of arrays (jagged array).
        /// </summary>
        DoubleComponent[][] IMatrixD.Elements
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets or sets an element in the matrix.
        /// </summary>
        /// <param name="row">The index of the row of the element.</param>
        /// <param name="column">The index of the column of the element.</param>
        /// <returns>The value of the element at the specified row and column.</returns>
        DoubleComponent IMatrixD.this[int row, int column]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the transpose of the matrix.
        /// </summary>
        /// <returns>The matrix with the rows as columns and columns as rows.</returns>
        IMatrixD IMatrixD.Transpose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the sum of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The sum.</returns>
        IMatrixD IAddable<IMatrixD>.Add(IMatrixD b)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the difference of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The difference.</returns>
        IMatrixD ISubtractable<IMatrixD>.Subtract(IMatrixD b)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the negative of the object. Must not modify the object itself.
        /// </summary>
        /// <returns>The negative.</returns>
        IMatrixD INegatable<IMatrixD>.Negative()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the product of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The product.</returns>
        IMatrixD IMultipliable<IMatrixD>.Multiply(IMatrixD b)
        {
            throw new NotImplementedException();
        }

        ///<summary>
        ///Indicates whether the current object is equal to another object of the same type.
        ///</summary>
        ///
        ///<returns>
        ///true if the current object is equal to the other parameter; otherwise, false.
        ///</returns>
        ///
        ///<param name="other">An object to compare with this object.</param>
        bool IEquatable<IMatrixD>.Equals(IMatrixD other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the multiplicative identity.
        /// </summary>
        /// <value>e</value>
        IMatrixD IHasOne<IMatrixD>.One
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns the quotient of the object and <paramref name="b"/>.
        /// It must not modify the value of the object.
        /// </summary>
        /// <param name="b">The second operand.</param>
        /// <returns>The quotient.</returns>
        IMatrixD IDivisible<IMatrixD>.Divide(IMatrixD b)
        {
            throw new NotImplementedException();
        }

        #region INegatable<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> INegatable<IVector<DoubleComponent>>.Negative()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Subtract(IVector<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasZero<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IHasZero<IVector<DoubleComponent>>.Zero
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IAddable<IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Add(IVector<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Divide(IVector<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasOne<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IHasOne<IVector<DoubleComponent>>.One
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IMultipliable<IVector<DoubleComponent>> Members

        public IVector<DoubleComponent> Multiply(IVector<DoubleComponent> b)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
