using System;
using System.Collections.Generic;
using NPack;
using NPack.Interfaces;

namespace SharpMap.Rendering
{
    public class LinearFactory : ILinearFactory<DoubleComponent, Vector, Matrix>
    {
        #region IMatrixFactory<DoubleComponent,Matrix> Members

        public Matrix CreateMatrix(Matrix matrix)
        {
            Matrix m = new Matrix(matrix);
            return m;
        }

        public Matrix CreateMatrix(MatrixFormat format, Int32 rowCount, Int32 columnCount)
        {
            Matrix m = new Matrix(format, rowCount, columnCount);
            return m;
        }

        public Matrix CreateMatrix(Int32 rowCount, Int32 columnCount, IEnumerable<DoubleComponent> values)
        {
            throw new NotImplementedException();
        }

        public Matrix CreateMatrix(Int32 rowCount, Int32 columnCount)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IVectorFactory<DoubleComponent,Vector> Members

        public Vector CreateVector(params Double[] components)
        {
            throw new NotImplementedException();
        }

        public Vector CreateVector(Double a, Double b, Double c)
        {
            throw new NotImplementedException();
        }

        public Vector CreateVector(Double a, Double b)
        {
            throw new NotImplementedException();
        }

        public Vector CreateVector(params DoubleComponent[] components)
        {
            throw new NotImplementedException();
        }

        public Vector CreateVector(DoubleComponent a, DoubleComponent b, DoubleComponent c)
        {
            throw new NotImplementedException();
        }

        public Vector CreateVector(DoubleComponent a, DoubleComponent b)
        {
            throw new NotImplementedException();
        }

        public Vector CreateVector(IEnumerable<DoubleComponent> values)
        {
            throw new NotImplementedException();
        }

        public Vector CreateVector(Int32 componentCount)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
