using System;
using System.Collections.Generic;
using System.Text;
using NPack;
using NPack.Interfaces;
using SharpMap.Rendering;

namespace SharpMap.Utilities
{
    internal class TempAffineFactory : IMatrixFactory<DoubleComponent>
    {
        #region IMatrixFactory<DoubleComponent> Members

        public IMatrix<DoubleComponent> CreateMatrix(int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        public IMatrix<DoubleComponent> CreateMatrix(int rowCount, int columnCount, IEnumerable<DoubleComponent> values)
        {
            throw new NotImplementedException();
        }

        public IMatrix<DoubleComponent> CreateMatrix(MatrixFormat format, int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        public IMatrix<DoubleComponent> CreateMatrix(IMatrix<DoubleComponent> matrix)
        {
            throw new NotImplementedException();
        }

        public ITransformMatrix<DoubleComponent> CreateTransformMatrix(int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        public ITransformMatrix<DoubleComponent> CreateTransformMatrix(MatrixFormat format, int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        public IAffineTransformMatrix<DoubleComponent> CreateAffineMatrix(int rank)
        {
            return new AffineMatrix(MatrixFormat.RowMajor, rank);
        }

        public IAffineTransformMatrix<DoubleComponent> CreateAffineMatrix(MatrixFormat format, int rank)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
