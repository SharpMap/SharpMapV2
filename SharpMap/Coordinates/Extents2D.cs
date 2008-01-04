using System;
using GeoAPI.Coordinates;
using GeoAPI.Geometries;
using NPack;
using NPack.Interfaces;

namespace SharpMap.Coordinates
{
    public class Extents2D : IExtents
    {
        public static Extents2D Empty = new Extents2D(Double.NaN, Double.NaN, Double.NaN, Double.NaN);

        public Extents2D(ICoordinate min, ICoordinate max)
        {

        }

        public Extents2D(IPoint min, IPoint max)
        {

        }

        public Extents2D(Double xMin, Double yMin, Double xMax, Double yMax)
        {

        }

        public Extents2D(IExtents union1, IExtents union2)
        {

        }

        #region IExtents Members

        public bool Borders(IExtents other)
        {
            throw new NotImplementedException();
        }

        public bool Borders(IExtents other, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        public ICoordinate Center
        {
            get { throw new NotImplementedException(); }
        }

        public bool Contains(params double[] coordinate)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IExtents other)
        {
            throw new NotImplementedException();
        }

        public bool Contains(ICoordinate other)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Tolerance tolerance, params double[] coordinate)
        {
            throw new NotImplementedException();
        }

        public bool Contains(IExtents other, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        public bool Contains(ICoordinate other, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        public double Distance(IExtents extents)
        {
            throw new NotImplementedException();
        }

        public void ExpandToInclude(params double[] coordinate)
        {
            throw new NotImplementedException();
        }

        public void ExpandToInclude(IExtents other)
        {
            throw new NotImplementedException();
        }

        public void ExpandToInclude(IGeometry other)
        {
            throw new NotImplementedException();
        }

        public IExtents Intersection(IExtents extents)
        {
            throw new NotImplementedException();
        }

        public bool Intersects(params double[] coordinate)
        {
            throw new NotImplementedException();
        }

        public bool Intersects(IExtents other)
        {
            throw new NotImplementedException();
        }

        public bool Intersects(Tolerance tolerance, params double[] coordinate)
        {
            throw new NotImplementedException();
        }

        public bool Intersects(IExtents other, Tolerance tolerance)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty
        {
            get { throw new NotImplementedException(); }
        }

        public ICoordinate Max
        {
            get { throw new NotImplementedException(); }
        }

        public ICoordinate Min
        {
            get { throw new NotImplementedException(); }
        }

        public double GetMax(Ordinates ordinate)
        {
            throw new NotImplementedException();
        }

        public double GetMin(Ordinates ordinate)
        {
            throw new NotImplementedException();
        }

        public double GetSize(Ordinates axis)
        {
            throw new NotImplementedException();
        }

        public double GetSize(Ordinates axis1, Ordinates axis2)
        {
            throw new NotImplementedException();
        }

        public double GetSize(Ordinates axis1, Ordinates axis2, Ordinates axis3)
        {
            throw new NotImplementedException();
        }

        public double GetSize(params Ordinates[] axes)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(params double[] coordinate)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(ICoordinate other)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IExtents other)
        {
            throw new NotImplementedException();
        }

        public void Scale(params double[] vector)
        {
            throw new NotImplementedException();
        }

        public void Scale(double factor)
        {
            throw new NotImplementedException();
        }

        public void Scale(double factor, Ordinates axis)
        {
            throw new NotImplementedException();
        }

        public void SetToEmpty()
        {
            throw new NotImplementedException();
        }

        public IGeometry ToGeometry()
        {
            throw new NotImplementedException();
        }

        public void Translate(params double[] vector)
        {
            throw new NotImplementedException();
        }

        public void Transform(IMatrix<DoubleComponent> transformMatrix)
        {
            throw new NotImplementedException();
        }

        public IExtents Union(IPoint point)
        {
            throw new NotImplementedException();
        }

        public IExtents Union(IExtents box)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable<IExtents> Members

        public bool Equals(IExtents other)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}