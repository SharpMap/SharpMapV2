using System;
using System.Collections.Generic;
using GeoAPI.Coordinates;

namespace SharpMap.Coordinates
{
    public class BufferedCoordinate2DFactory : ICoordinateFactory<BufferedCoordinate2D>
    {

        #region ICoordinateFactory<BufferedCoordinate2D> Members

        public BufferedCoordinate2D Create(double x, double y)
        {
            throw new NotImplementedException();
        }

        public BufferedCoordinate2D Create(double x, double y, double m)
        {
            throw new NotImplementedException();
        }

        public BufferedCoordinate2D Create(params double[] coordinates)
        {
            throw new NotImplementedException();
        }

        public BufferedCoordinate2D Create3D(double x, double y, double z)
        {
            throw new NotImplementedException();
        }

        public BufferedCoordinate2D Create3D(double x, double y, double z, double m)
        {
            throw new NotImplementedException();
        }

        public BufferedCoordinate2D Create3D(params double[] coordinates)
        {
            throw new NotImplementedException();
        }

        public BufferedCoordinate2D Create(BufferedCoordinate2D coordinate)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public IEnumerable<BufferedCoordinate2D> Homogenize(IEnumerable<BufferedCoordinate2D> coordinate)
        {
            throw new NotImplementedException();
        }

        public BufferedCoordinate2D Dehomogenize(BufferedCoordinate2D coordinate)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BufferedCoordinate2D> Dehomogenize(IEnumerable<BufferedCoordinate2D> coordinate)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICoordinateFactory Members

        ICoordinate ICoordinateFactory.Create(double x, double y)
        {
            throw new NotImplementedException();
        }

        ICoordinate ICoordinateFactory.Create(double x, double y, double m)
        {
            throw new NotImplementedException();
        }

        ICoordinate ICoordinateFactory.Create(params double[] coordinates)
        {
            throw new NotImplementedException();
        }

        ICoordinate ICoordinateFactory.Create3D(double x, double y, double z)
        {
            throw new NotImplementedException();
        }

        ICoordinate ICoordinateFactory.Create3D(double x, double y, double z, double m)
        {
            throw new NotImplementedException();
        }

        ICoordinate ICoordinateFactory.Create3D(params double[] coordinates)
        {
            throw new NotImplementedException();
        }

        public NPack.Interfaces.IAffineTransformMatrix<NPack.DoubleComponent> CreateTransform(ICoordinate scaleVector, ICoordinate rotationAxis, double rotation, ICoordinate translateVector)
        {
            throw new NotImplementedException();
        }

        public ICoordinate Homogenize(ICoordinate coordinate)
        {
            throw new NotImplementedException();
        }

        public ICoordinate Dehomogenize(ICoordinate coordinate)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
