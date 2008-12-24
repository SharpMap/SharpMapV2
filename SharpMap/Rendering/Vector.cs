// Copyright 2007-2008 Rory Plaire (codekaizen@gmail.com)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NPack;
using NPack.Interfaces;

namespace SharpMap.Rendering
{
    /// <summary>
    /// A vector.
    /// </summary>
    public class Vector : IVector<DoubleComponent, Vector>, 
                          IVector<DoubleComponent>,
                          IEquatable<Vector>,
                          IComparable<Vector>, 
                          IComputable<Double, Vector>
    {
        private DoubleComponent[] _elements;

        /// <summary>
        /// Creates a new vector with the given number of components.
        /// </summary>
        /// <param name="componentCount">The number of components in the vector.</param>
        public Vector(Int32 componentCount)
        {
            _elements = new DoubleComponent[componentCount];
        }

        /// <summary>
        /// Creates a new vector with the given components.
        /// </summary>
        /// <param name="components">The components to initialize the vector to.</param>
        public Vector(params DoubleComponent[] components)
        {
            Components = components;
        }

        /// <summary>
        /// Gets a String representation of the vector.
        /// </summary>
        /// <returns>A String describing the vector.</returns>
        public override String ToString()
        {
            return String.Format("[{0}]", printComponents());
        }

        /// <summary>
        /// Creates a component-by-component copy of the vector.
        /// </summary>
        /// <returns>A copy of the vector.</returns>
        public Vector Clone()
        {
            DoubleComponent[] componetsCopy = new DoubleComponent[Components.Length];
            Array.Copy(Components, componetsCopy, componetsCopy.Length);
            return new Vector(componetsCopy);
        }

        /// <summary>
        /// Returns the vector multiplied by -1.
        /// </summary>
        /// <returns>The vector when multiplied by -1.</returns>
        public Vector Negative()
        {
            Vector negated = Clone();

            for (Int32 i = 0; i < negated.ComponentCount; i++)
            {
                negated[i] = negated[i].Negative();
            }

            return negated;
        }

        /// <summary>
        /// Gets or sets a component in the vector.
        /// </summary>
        /// <param name="index">The index of the component.</param>
        /// <returns>The value of the component at the given <paramref name="index"/>.</returns>
        public DoubleComponent this[Int32 index]
        {
            get
            {
                checkIndex(index);

                return Components[index];
            }
            set
            {
                checkIndex(index);

                Components[index] = value;
            }
        }

        private void checkIndex(Int32 index)
        {
            if (index < 0 || index >= ComponentCount)
            {
                throw new ArgumentOutOfRangeException("index", index,
                                                      "Indexer must be between 0 and ComponentCount.");
            }
        }

        public void GetComponents(out DoubleComponent a, out DoubleComponent b)
        {
            Int32 count = ComponentCount;

            a = b = Double.NaN;

            if (count > 0)
            {
                a = _elements[0];
            }

            if (count > 1)
            {
                b = _elements[1];
            }
        }

        public void GetComponents(out DoubleComponent a, out DoubleComponent b, out DoubleComponent c)
        {
            GetComponents(out a, out b);

            c = Double.NaN;

            if (ComponentCount > 2)
            {
                c = _elements[2];
            }
        }

        public void GetComponents(out DoubleComponent a, out DoubleComponent b, out DoubleComponent c, out DoubleComponent d)
        {
            GetComponents(out a, out b, out c);

            d = Double.NaN;

            if (ComponentCount > 2)
            {
                d = _elements[3];
            }
        }

        /// <summary>
        /// Gets the number of components in the vector.
        /// </summary>
        public Int32 ComponentCount
        {
            get { return _elements.Length; }
        }

        /// <summary>
        /// Gets or sets the vector component array.
        /// </summary>
        public DoubleComponent[] Components
        {
            get { return _elements; }
            set
            {
                _elements = new DoubleComponent[value.Length];
                Array.Copy(value, _elements, value.Length);
            }
        }

        /// <summary>
        /// Gets an enumerator which enumerates over the vector's components.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> which returns the vectors components when iterated.</returns>
        public IEnumerator<DoubleComponent> GetEnumerator()
        {
            for (Int32 i = 0; i < ComponentCount; i++)
            {
                yield return this[i];
            }
        }

        #region Private helper methods

        private String printComponents()
        {
            StringBuilder buffer = new StringBuilder();
            foreach (DoubleComponent component in Components)
            {
                if (buffer.Length > 255)
                {
                    buffer.Append(".....");
                    break;
                }

                buffer.Append(component);
                buffer.Append(", ");
            }

            buffer.Length -= 2;
            return buffer.ToString();
        }

        #endregion

        #region Vector implementation

        #region IAddable<Vector> Members

        public Vector Add(Vector b)
        {
            return MatrixProcessor.Add(this, b);
        }

        #endregion

        #region ISubtractable<Vector> Members

        public Vector Subtract(Vector b)
        {
            return MatrixProcessor.Subtract(this, b);
        }

        #endregion

        #region IMultipliable<Vector> Members

        public Vector Multiply(Vector b)
        {
            throw new NotSupportedException("Vector-Vector multiplication is not supported.");
        }

        #endregion

        #region IDivisible<Vector> Members

        public Vector Divide(Vector b)
        {
            throw new NotSupportedException("Vector-Vector division is not supported.");
        }

        #endregion

        #region IHasOne<Vector> Members

        public Vector One
        {
            get
            {
                Vector v = new Vector(ComponentCount);

                for (Int32 i = 0; i < ComponentCount; i++)
                {
                    v[i] = new DoubleComponent(1);
                }

                return v;
            }
        }

        #endregion

        #region IDivisible<Double, Vector> Members

        public Vector Divide(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable<Vector> Members

        public Boolean Equals(Vector other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region IVector<DoubleComponent,Vector> Members


        public Vector Cross(Vector vector)
        {
            throw new NotImplementedException();
        }

        public Double Dot(Vector vector)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMatrix<DoubleComponent> Members

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.Clone()
        {
            throw new NotImplementedException();
        }

        Int32 IMatrix<DoubleComponent>.ColumnCount
        {
            get { throw new NotImplementedException(); }
        }

        Double IMatrix<DoubleComponent>.Determinant
        {
            get { throw new NotImplementedException(); }
        }

        MatrixFormat IMatrix<DoubleComponent>.Format
        {
            get { throw new NotImplementedException(); }
        }

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.GetMatrix(Int32[] rowIndexes, Int32 startColumn, Int32 endColumn)
        {
            throw new NotImplementedException();
        }

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.Inverse
        {
            get { throw new NotImplementedException(); }
        }

        Boolean IMatrix<DoubleComponent>.IsInvertible
        {
            get { throw new NotImplementedException(); }
        }

        Boolean IMatrix<DoubleComponent>.IsSingular
        {
            get { throw new NotImplementedException(); }
        }

        Boolean IMatrix<DoubleComponent>.IsSquare
        {
            get { throw new NotImplementedException(); }
        }

        Boolean IMatrix<DoubleComponent>.IsSymmetrical
        {
            get { throw new NotImplementedException(); }
        }

        Int32 IMatrix<DoubleComponent>.RowCount
        {
            get { throw new NotImplementedException(); }
        }

        IMatrix<DoubleComponent> IMatrix<DoubleComponent>.Transpose()
        {
            throw new NotImplementedException();
        }

        public DoubleComponent this[Int32 row, Int32 column]
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

        #region IComputable<Double,IVector<DoubleComponent,Vector>> Members

        IVector<DoubleComponent, Vector> IComputable<Double, IVector<DoubleComponent, Vector>>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<IVector<DoubleComponent,Vector>> Members

        IVector<DoubleComponent, Vector> IComputable<IVector<DoubleComponent, Vector>>.Abs()
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent, Vector> IComputable<IVector<DoubleComponent, Vector>>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region INegatable<IVector<DoubleComponent,Vector>> Members

        IVector<DoubleComponent, Vector> INegatable<IVector<DoubleComponent, Vector>>.Negative()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<IVector<DoubleComponent,Vector>> Members

        IVector<DoubleComponent, Vector> ISubtractable<IVector<DoubleComponent,Vector>>.Subtract(IVector<DoubleComponent, Vector> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasZero<IVector<DoubleComponent,Vector>> Members

        IVector<DoubleComponent, Vector> IHasZero<IVector<DoubleComponent, Vector>>.Zero
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IAddable<IVector<DoubleComponent,Vector>> Members

        IVector<DoubleComponent, Vector> IAddable<IVector<DoubleComponent,Vector>>.Add(IVector<DoubleComponent, Vector> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<IVector<DoubleComponent,Vector>> Members

        IVector<DoubleComponent, Vector> IDivisible<IVector<DoubleComponent,Vector>>.Divide(IVector<DoubleComponent, Vector> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasOne<IVector<DoubleComponent,Vector>> Members

        IVector<DoubleComponent, Vector> IHasOne<IVector<DoubleComponent, Vector>>.One
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IMultipliable<IVector<DoubleComponent,Vector>> Members

        IVector<DoubleComponent, Vector> IMultipliable<IVector<DoubleComponent,Vector>>.Multiply(IVector<DoubleComponent, Vector> b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBooleanComparable<IVector<DoubleComponent,Vector>> Members

        Boolean IBooleanComparable<IVector<DoubleComponent, Vector>>.GreaterThan(IVector<DoubleComponent, Vector> value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IVector<DoubleComponent, Vector>>.GreaterThanOrEqualTo(IVector<DoubleComponent, Vector> value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IVector<DoubleComponent, Vector>>.LessThan(IVector<DoubleComponent, Vector> value)
        {
            throw new NotImplementedException();
        }

        Boolean IBooleanComparable<IVector<DoubleComponent, Vector>>.LessThanOrEqualTo(IVector<DoubleComponent, Vector> value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<IVector<DoubleComponent,Vector>> Members

        IVector<DoubleComponent, Vector> IExponential<IVector<DoubleComponent, Vector>>.Exp()
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent, Vector> IExponential<IVector<DoubleComponent, Vector>>.Log()
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent, Vector> IExponential<IVector<DoubleComponent, Vector>>.Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent, Vector> IExponential<IVector<DoubleComponent, Vector>>.Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent, Vector> IExponential<IVector<DoubleComponent, Vector>>.Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IAddable<Double,IVector<DoubleComponent,Vector>> Members

        IVector<DoubleComponent, Vector> IAddable<Double, IVector<DoubleComponent, Vector>>.Add(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<Double,IVector<DoubleComponent,Vector>> Members

        IVector<DoubleComponent, Vector> ISubtractable<Double, IVector<DoubleComponent, Vector>>.Subtract(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultipliable<Double,IVector<DoubleComponent,Vector>> Members

        IVector<DoubleComponent, Vector> IMultipliable<Double, IVector<DoubleComponent, Vector>>.Multiply(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<Double,IVector<DoubleComponent,Vector>> Members

        IVector<DoubleComponent, Vector> IDivisible<Double, IVector<DoubleComponent, Vector>>.Divide(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable<IVector<DoubleComponent,Vector>> Members

        Boolean IEquatable<IVector<DoubleComponent,Vector>>.Equals(IVector<DoubleComponent, Vector> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable<IVector<DoubleComponent,Vector>> Members

        Int32 IComparable<IVector<DoubleComponent,Vector>>.CompareTo(IVector<DoubleComponent, Vector> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<Vector> Members

        public Vector Abs()
        {
            throw new NotImplementedException();
        }

        public Vector Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IHasZero<Vector> Members

        public Vector Zero
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IBooleanComparable<Vector> Members

        public Boolean GreaterThan(Vector value)
        {
            throw new NotImplementedException();
        }

        public Boolean GreaterThanOrEqualTo(Vector value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThan(Vector value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThanOrEqualTo(Vector value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<Vector> Members

        public Vector Exp()
        {
            throw new NotImplementedException();
        }

        public Vector Log()
        {
            throw new NotImplementedException();
        }

        public Vector Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        public Vector Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        public Vector Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IAddable<Double,Vector> Members

        public Vector Add(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<Double,Vector> Members

        public Vector Subtract(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultipliable<Double,Vector> Members

        public Vector Multiply(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable<Vector> Members

        public Int32 CompareTo(Vector other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVector<DoubleComponent> Members

        IVector<DoubleComponent> IVector<DoubleComponent>.Clone()
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent> IVector<DoubleComponent>.Negative()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<Double,IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IComputable<Double, IVector<DoubleComponent>>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComputable<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IComputable<IVector<DoubleComponent>>.Abs()
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent> IComputable<IVector<DoubleComponent>>.Set(Double value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region INegatable<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> INegatable<IVector<DoubleComponent>>.Negative()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> ISubtractable<IVector<DoubleComponent>>.Subtract(IVector<DoubleComponent> b)
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

        #region IBooleanComparable<IVector<DoubleComponent>> Members

        public Boolean GreaterThan(IVector<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        public Boolean GreaterThanOrEqualTo(IVector<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThan(IVector<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        public Boolean LessThanOrEqualTo(IVector<DoubleComponent> value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IExponential<IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IExponential<IVector<DoubleComponent>>.Exp()
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent> IExponential<IVector<DoubleComponent>>.Log()
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent> IExponential<IVector<DoubleComponent>>.Log(Double newBase)
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent> IExponential<IVector<DoubleComponent>>.Power(Double exponent)
        {
            throw new NotImplementedException();
        }

        IVector<DoubleComponent> IExponential<IVector<DoubleComponent>>.Sqrt()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IAddable<Double,IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IAddable<Double, IVector<DoubleComponent>>.Add(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ISubtractable<Double,IVector<DoubleComponent>> Members

        IVector<DoubleComponent> ISubtractable<Double, IVector<DoubleComponent>>.Subtract(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultipliable<Double,IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IMultipliable<Double, IVector<DoubleComponent>>.Multiply(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDivisible<Double,IVector<DoubleComponent>> Members

        IVector<DoubleComponent> IDivisible<Double, IVector<DoubleComponent>>.Divide(Double b)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable<IVector<DoubleComponent>> Members

        public Boolean Equals(IVector<DoubleComponent> other)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable<IVector<DoubleComponent>> Members

        public Int32 CompareTo(IVector<DoubleComponent> other)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}