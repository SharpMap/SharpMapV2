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
using NPack;
using NPack.Interfaces;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;

namespace SharpMap.Rendering.Rendering3D
{
    public struct ViewPoint3D : IVectorD
    {
        private double _x, _y, _z;
        private bool _hasValue;

        public static readonly ViewPoint3D Empty = new ViewPoint3D();
        public static readonly ViewPoint3D Zero = new ViewPoint3D(0, 0, 0);

        public ViewPoint3D(double x, double y, double z)
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

        public double X
        {
            get { return _x; }
        }

        public double Y
        {
            get { return _y; }
        }

        public double Z
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
                   point._x == _x &&
                   point._y == _y &&
                   point._z == _z;
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

        public double Determinant
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public int ColumnCount
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool IsSingular
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IMatrix<DoubleComponent> Inverse
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool IsSquare
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool IsSymmetrical
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public int RowCount
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public IMatrix<DoubleComponent> GetMatrix(int[] rowIndexes, int j0, int j1)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public DoubleComponent this[int row, int column]
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public IMatrix<DoubleComponent> Transpose()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IAddable<IMatrix<DoubleComponent>> Members

        public IMatrix<DoubleComponent> Add(IMatrix<DoubleComponent> a)
        {
            checkRank(a);

            DoubleComponent[] elements = a.ElementArray[0];
            return new ViewPoint3D(elements[0] + _x, elements[1] + _y, elements[2] + _z);
        }

        #endregion

        #region ISubtractable<IMatrix<DoubleComponent>> Members

        public IMatrix<DoubleComponent> Subtract(IMatrix<DoubleComponent> a)
        {
            checkRank(a);

            DoubleComponent[] elements = a.ElementArray[0];
            return new ViewPoint3D(elements[0] - _x, elements[1] - _y, elements[2] - _z);
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
            return new ViewPoint3D(-_x, -_y, -_z);
        }

        #endregion

        #region IMultipliable<IMatrix<DoubleComponent>> Members

        public IMatrix<DoubleComponent> Multiply(IMatrix<DoubleComponent> a)
        {
            throw new Exception("The method or operation is not implemented.");
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
    }
}
