using System;
using System.Collections.Generic;
using System.Text;
using GeoAPI.Coordinates;

namespace SharpMap.Coordinates
{
    public class BufferedCoordinateSequenceFactory : ICoordinateSequenceFactory<BufferedCoordinate2D>
    {
        #region ICoordinateSequenceFactory<BufferedCoordinate2D> Members

        public ICoordinateFactory<BufferedCoordinate2D> CoordinateFactory
        {
            get { throw new NotImplementedException(); }
        }

        public ICoordinateSequence<BufferedCoordinate2D> Create(int dimension)
        {
            throw new NotImplementedException();
        }

        public ICoordinateSequence<BufferedCoordinate2D> Create(int size, int dimension)
        {
            throw new NotImplementedException();
        }

        public ICoordinateSequence<BufferedCoordinate2D> Create(ICoordinateSequence<BufferedCoordinate2D> coordSeq)
        {
            throw new NotImplementedException();
        }

        public ICoordinateSequence<BufferedCoordinate2D> Create(Func<double, double> componentTransform, IEnumerable<BufferedCoordinate2D> coordinates, bool allowRepeated, bool direction)
        {
            throw new NotImplementedException();
        }

        public ICoordinateSequence<BufferedCoordinate2D> Create(IEnumerable<BufferedCoordinate2D> coordinates, bool allowRepeated, bool direction)
        {
            throw new NotImplementedException();
        }

        public ICoordinateSequence<BufferedCoordinate2D> Create(Func<double, double> componentTransform, IEnumerable<BufferedCoordinate2D> coordinates, bool allowRepeated)
        {
            throw new NotImplementedException();
        }

        public ICoordinateSequence<BufferedCoordinate2D> Create(IEnumerable<BufferedCoordinate2D> coordinates, bool allowRepeated)
        {
            throw new NotImplementedException();
        }

        public ICoordinateSequence<BufferedCoordinate2D> Create(Func<double, double> componentTransform, IEnumerable<BufferedCoordinate2D> coordinates)
        {
            throw new NotImplementedException();
        }

        public ICoordinateSequence<BufferedCoordinate2D> Create(IEnumerable<BufferedCoordinate2D> coordinates)
        {
            throw new NotImplementedException();
        }

        public ICoordinateSequence<BufferedCoordinate2D> Create(params BufferedCoordinate2D[] coordinates)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICoordinateSequenceFactory Members

        ICoordinateSequence ICoordinateSequenceFactory.Create(int dimension)
        {
            throw new NotImplementedException();
        }

        ICoordinateSequence ICoordinateSequenceFactory.Create(int size, int dimension)
        {
            throw new NotImplementedException();
        }

        public ICoordinateSequence Create(ICoordinateSequence coordSeq)
        {
            throw new NotImplementedException();
        }

        public ICoordinateSequence Create(IEnumerable<ICoordinate> coordinates)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
