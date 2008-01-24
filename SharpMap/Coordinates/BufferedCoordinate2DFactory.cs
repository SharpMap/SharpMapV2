using System;
using System.Collections.Generic;
using System.ComponentModel;
using GeoAPI.Coordinates;
using NPack;
using NPack.Interfaces;

namespace SharpMap.Coordinates
{
    public class BufferedCoordinate2DFactory 
        : ICoordinateFactory<BufferedCoordinate2D>, IVectorBuffer<BufferedCoordinate2D, DoubleComponent>, 
          IBufferedVectorFactory<BufferedCoordinate2D, DoubleComponent>
    {
        private readonly ManagedVectorBuffer<BufferedCoordinate2D, DoubleComponent> _coordinates;

        public BufferedCoordinate2DFactory()
        {
            _coordinates = new ManagedVectorBuffer<BufferedCoordinate2D, DoubleComponent>(2, true, this);
        }

        #region ICoordinateFactory<BufferedCoordinate2D> Members

        public BufferedCoordinate2D Create(Double x, Double y)
        {
            Int32 existingIndex = findExisting(x, y);

            if (existingIndex >= 0)
            {
                return new BufferedCoordinate2D(this, existingIndex);
            }
            else
            {
                return _coordinates.Add(x, y);
            }
        }

        public BufferedCoordinate2D Create(Double x, Double y, Double m)
        {
            throw new NotSupportedException("Coordinates with 'M' values currently not supported.");
        }

        public BufferedCoordinate2D Create(params Double[] coordinates)
        {
            if (coordinates == null)
            {
                throw new ArgumentNullException("coordinates");
            }

            if (coordinates.Length == 2)
            {
                return Create(coordinates[0], coordinates[1]);
            }

            throw new NotSupportedException("Coordinates with 'M' values currently not supported.");
        }

        public BufferedCoordinate2D Create3D(Double x, Double y, Double z)
        {
            throw new NotSupportedException("Only 2D coordinates are supported.");
        }

        public BufferedCoordinate2D Create3D(Double x, Double y, Double z, Double m)
        {
            throw new NotSupportedException("Only 2D coordinates are supported.");
        }

        public BufferedCoordinate2D Create3D(params Double[] coordinates)
        {
            throw new NotSupportedException("Only 2D coordinates are supported.");
        }

        public BufferedCoordinate2D Create(BufferedCoordinate2D coordinate)
        {
            if (coordinate.Factory == this)
            {
                return coordinate;
            }
            else
            {
                return new BufferedCoordinate2D(this, _coordinates.Add(coordinate));
            }
        }

        public AffineTransformMatrix<BufferedCoordinate2D> CreateTransform(BufferedCoordinate2D scaleVector, Double rotation, BufferedCoordinate2D translateVector)
        {
            throw new NotImplementedException();
        }

        public AffineTransformMatrix<BufferedCoordinate2D> CreateTransform(BufferedCoordinate2D scaleVector, BufferedCoordinate2D rotationAxis, Double rotation, BufferedCoordinate2D translateVector)
        {
            throw new NotImplementedException();
        }

        public BufferedCoordinate2D Homogenize(BufferedCoordinate2D coordinate)
        {
            return BufferedCoordinate2D.Homogenize(coordinate);
        }

        public IEnumerable<BufferedCoordinate2D> Homogenize(IEnumerable<BufferedCoordinate2D> coordinates)
        {
            foreach (BufferedCoordinate2D coordinate in coordinates)
            {
                yield return Homogenize(coordinate);
            }
        }

        public BufferedCoordinate2D Dehomogenize(BufferedCoordinate2D coordinate)
        {
            return BufferedCoordinate2D.Dehomogenize(coordinate);
        }

        public IEnumerable<BufferedCoordinate2D> Dehomogenize(IEnumerable<BufferedCoordinate2D> coordinates)
        {
            foreach (BufferedCoordinate2D coordinate in coordinates)
            {
                yield return Dehomogenize(coordinate);
            }
        }

        #endregion

        #region ICoordinateFactory Members

        ICoordinate ICoordinateFactory.Create(Double x, Double y)
        {
            throw new NotImplementedException();
        }

        ICoordinate ICoordinateFactory.Create(Double x, Double y, Double m)
        {
            throw new NotImplementedException();
        }

        ICoordinate ICoordinateFactory.Create(params Double[] coordinates)
        {
            throw new NotImplementedException();
        }

        ICoordinate ICoordinateFactory.Create3D(Double x, Double y, Double z)
        {
            throw new NotImplementedException();
        }

        ICoordinate ICoordinateFactory.Create3D(Double x, Double y, Double z, Double m)
        {
            throw new NotImplementedException();
        }

        ICoordinate ICoordinateFactory.Create3D(params Double[] coordinates)
        {
            throw new NotImplementedException();
        }

        IAffineTransformMatrix<DoubleComponent> ICoordinateFactory.CreateTransform(ICoordinate scaleVector, ICoordinate rotationAxis, Double rotation, ICoordinate translateVector)
        {
            throw new NotImplementedException();
        }

        ICoordinate ICoordinateFactory.Homogenize(ICoordinate coordinate)
        {
            throw new NotImplementedException();
        }

        ICoordinate ICoordinateFactory.Dehomogenize(ICoordinate coordinate)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVectorBuffer<BufferedCoordinate2D,DoubleComponent> Members

        public Int32 Add(IVector<DoubleComponent> vector)
        {
            return _coordinates.Add(vector);
        }

        public Int32 Add(BufferedCoordinate2D vector)
        {
            return _coordinates.Add(vector);
        }

        public BufferedCoordinate2D Add(params DoubleComponent[] components)
        {
            return _coordinates.Add(components);
        }

        public void Clear()
        {
            _coordinates.Clear();
        }

        public Boolean Contains(IVector<DoubleComponent> item)
        {
            return _coordinates.Contains(item);
        }

        public Boolean Contains(BufferedCoordinate2D item)
        {
            return _coordinates.Contains(item);
        }

        public void CopyTo(BufferedCoordinate2D[] array, Int32 startIndex, Int32 endIndex)
        {
            _coordinates.CopyTo(array, startIndex, endIndex);
        }

        public Int32 Count
        {
            get { return _coordinates.Count; }
        }

        public IVectorFactory<BufferedCoordinate2D, DoubleComponent> Factory
        {
            get { return _coordinates.Factory; }
        }

        public Boolean IsReadOnly
        {
            get { return _coordinates.IsReadOnly; }
        }

        public long MaximumSize
        {
            get
            {
                return _coordinates.MaximumSize;
            }
            set
            {
                _coordinates.MaximumSize = value;
            }
        }

        public void Remove(Int32 index)
        {
            _coordinates.Remove(index);
        }

        public event EventHandler SizeIncreased;

        public event CancelEventHandler SizeIncreasing;

        public event EventHandler<VectorOperationEventArgs<BufferedCoordinate2D, DoubleComponent>> VectorChanged;

        public Int32 VectorLength
        {
            get { return 2; }
        }

        public BufferedCoordinate2D this[Int32 index]
        {
            get
            {
                return _coordinates[index];
            }
            set
            {
                _coordinates[index] = value;
            }
        }

        #endregion

        #region IEnumerable<BufferedCoordinate2D> Members

        public IEnumerator<BufferedCoordinate2D> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IBufferedVectorFactory<BufferedCoordinate2D,DoubleComponent> Members

        public BufferedCoordinate2D CreateBufferedVector(IVectorBuffer<BufferedCoordinate2D, DoubleComponent> vectorBuffer, Int32 index)
        {
            if (vectorBuffer != this)
            {
                throw new ArgumentException("The buffer must be this BufferedCoordinate2DFactory.");
            }

            return new BufferedCoordinate2D(vectorBuffer as BufferedCoordinate2DFactory, index);
        }

        #endregion

        internal Double GetOrdinate(Int32 index, Ordinates ordinate)
        {
            if (ordinate == Ordinates.X)
            {
                return _coordinates[index].X;
            }
            else if(ordinate == Ordinates.Y)
            {
                return _coordinates[index].Y;
            }
            else
            {
                throw new NotSupportedException("Ordinate not supported: " + ordinate);
            }
        }

        internal ICoordinate GetZero()
        {
            return new BufferedCoordinate2D(this, 0);
        }

        internal BufferedCoordinate2D Add(BufferedCoordinate2D a, BufferedCoordinate2D b)
        {
            throw new NotImplementedException();
        }

        internal BufferedCoordinate2D Divide(BufferedCoordinate2D a, BufferedCoordinate2D b)
        {
            throw new NotImplementedException();
        }

        internal BufferedCoordinate2D GetOne()
        {
            throw new NotImplementedException();
        }

        internal BufferedCoordinate2D Multiply(BufferedCoordinate2D a, BufferedCoordinate2D b)
        {
            throw new NotImplementedException();
        }

        internal Boolean GreaterThan(BufferedCoordinate2D a, BufferedCoordinate2D b)
        {
            throw new NotImplementedException();
        }

        internal Boolean GreaterThanOrEqualTo(BufferedCoordinate2D a, BufferedCoordinate2D b)
        {
            throw new NotImplementedException();
        }

        internal Boolean LessThan(BufferedCoordinate2D a, BufferedCoordinate2D b)
        {
            throw new NotImplementedException();
        }

        internal Boolean LessThanOrEqualTo(BufferedCoordinate2D a, BufferedCoordinate2D b)
        {
            throw new NotImplementedException();
        }

        internal BufferedCoordinate2D Divide(BufferedCoordinate2D a, Double b)
        {
            throw new NotImplementedException();
        }

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private Int32 findExisting(Double x, Double y)
        {
            throw new NotImplementedException();
        }
    }
}
