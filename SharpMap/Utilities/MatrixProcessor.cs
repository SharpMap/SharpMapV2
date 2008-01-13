// Copyright 2007 Rory Plaire (codekaizen@gmail.com)

using System;
using System.Collections.Generic;
using NPack;
using NPack.Interfaces;

namespace SharpMap.Utilities
{
    /// <summary>
    /// Provides linear algebra processing methods
    /// </summary>
    sealed class MatrixProcessor
    {
        private static volatile MatrixProcessor _instance = null;
        private static readonly Object _initSync = new Object();
        private readonly IMatrixOperations<DoubleComponent, IVector<DoubleComponent>, IMatrix<DoubleComponent>> _ops;

        static MatrixProcessor() { }

        private MatrixProcessor(IMatrixOperations<DoubleComponent, IVector<DoubleComponent>, IMatrix<DoubleComponent>> ops)
        {
            _ops = ops;
        }

        /// <summary>
        /// Gets the singleton instance of a MatrixProcessor.
        /// </summary>
        public static MatrixProcessor Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_initSync)
                    {
                        if (_instance == null)
                        {
                            // and this is where the configuration should go...
                            // but we hard code it for now.
                            LinearFactory<DoubleComponent> factory = new LinearFactory<DoubleComponent>();
                            IMatrixOperations<DoubleComponent, IVector<DoubleComponent>, IMatrix<DoubleComponent>> ops
                                = new ClrMatrixOperations<DoubleComponent, IVector<DoubleComponent>, IMatrix<DoubleComponent>>(factory);

                            _instance = new MatrixProcessor(ops);
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Gets the configured operations engine.
        /// </summary>
        public IMatrixOperations<DoubleComponent, IVector<DoubleComponent>, IMatrix<DoubleComponent>> Operations
        {
            get { return _ops; }
        }

        public static IMatrix<DoubleComponent> Add(IMatrix<DoubleComponent> lhs, IMatrix<DoubleComponent> rhs)
        {
            return Instance.Operations.Add(lhs, rhs);
        }

        public static IVector<DoubleComponent> Add(IVector<DoubleComponent> lhs, IVector<DoubleComponent> rhs)
        {
            return Instance.Operations.Add(lhs, rhs);
        }

        public static IMatrix<DoubleComponent> Subtract(IMatrix<DoubleComponent> lhs, IMatrix<DoubleComponent> rhs)
        {
            return Instance.Operations.Subtract(lhs, rhs);
        }

        public static IVector<DoubleComponent> Subtract(IVector<DoubleComponent> lhs, IVector<DoubleComponent> rhs)
        {
            return Instance.Operations.Subtract(lhs, rhs);
        }

        public static IMatrix<DoubleComponent> Multiply(IMatrix<DoubleComponent> lhs, IMatrix<DoubleComponent> rhs)
        {
            return Instance.Operations.Multiply(lhs, rhs);
        }

        public static IVector<DoubleComponent> Multiply(IMatrix<DoubleComponent> multiplier, IVector<DoubleComponent> columnVector)
        {
            return Instance.Operations.Multiply(multiplier, columnVector);
        }

        public static IVector<DoubleComponent> Multiply(IVector<DoubleComponent> rowVector, IMatrix<DoubleComponent> multiplier)
        {
            return Instance.Operations.Multiply(rowVector, multiplier);
        }

        public static IEnumerable<IMatrix<DoubleComponent>> Multiply(IMatrix<DoubleComponent> lhs, IEnumerable<IMatrix<DoubleComponent>> rhs)
        {
            return Instance.Operations.Multiply(lhs, rhs);
        }

        public static IEnumerable<IMatrix<DoubleComponent>> Multiply(IEnumerable<IMatrix<DoubleComponent>> lhs, IMatrix<DoubleComponent> rhs)
        {
            return Instance.Operations.Multiply(lhs, rhs);
        }

        public static IEnumerable<IVector<DoubleComponent>> Multiply(IMatrix<DoubleComponent> multiplier, IEnumerable<IVector<DoubleComponent>> columnVectors)
        {
            return Instance.Operations.Multiply(multiplier, columnVectors);
        }

        public static IEnumerable<IVector<DoubleComponent>> Multiply(IEnumerable<IVector<DoubleComponent>> rowVectors, IMatrix<DoubleComponent> multiplier)
        {
            return Instance.Operations.Multiply(rowVectors, multiplier);
        }

        public static void Multiply(IMatrix<DoubleComponent> multiplier, IEnumerable<DoubleComponent[]> columnVectorsComponents)
        {
            Instance.Operations.Multiply(multiplier, columnVectorsComponents);
        }

        public static void Multiply(IEnumerable<DoubleComponent[]> rowVectorsComponents, IMatrix<DoubleComponent> multiplier)
        {
            Instance.Operations.Multiply(rowVectorsComponents, multiplier);
        }

        public static void Multiply(IMatrix<DoubleComponent> multiplier, DoubleComponent[] columnVector)
        {
            Instance.Operations.Multiply(multiplier, columnVector);
        }

        public static void Multiply(DoubleComponent[] rowVector, IMatrix<DoubleComponent> multiplier)
        {
            Instance.Operations.Multiply(rowVector, multiplier);
        }

        public static IMatrix<DoubleComponent> ScalarMultiply(IMatrix<DoubleComponent> matrix, DoubleComponent scalar)
        {
            return Instance.Operations.ScalarMultiply(matrix, scalar);
        }

        public static IMatrix<DoubleComponent> Invert(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.Invert(matrix);
        }

        public static IMatrix<DoubleComponent> Negate(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.Negate(matrix);
        }

        public static void Rotate(IMatrix<DoubleComponent> matrix, IVector<DoubleComponent> axis, Double radians)
        {
            Instance.Operations.Rotate(matrix, axis, radians);
        }

        public static void Scale(IMatrix<DoubleComponent> matrix, IVector<DoubleComponent> scaleVector)
        {
            Instance.Operations.Scale(matrix, scaleVector);
        }

        public static void Shear(IMatrix<DoubleComponent> matrix, IVector<DoubleComponent> shearVector)
        {
            Instance.Operations.Shear(matrix, shearVector);
        }

        public static void Translate(IMatrix<DoubleComponent> affineMatrix, IVector<DoubleComponent> translateVector)
        {
            Instance.Operations.Translate(affineMatrix, translateVector);
        }

        public static Double FrobeniusNorm(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.FrobeniusNorm(matrix);
        }

        public static Double OneNorm(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.OneNorm(matrix);
        }

        public static Double TwoNorm(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.TwoNorm(matrix);
        }

        public static Double InfinityNorm(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.InfinityNorm(matrix);
        }

        public static Double Determinant(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.Determinant(matrix);
        }

        public static Int32 Rank(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.Rank(matrix);
        }

        public static Double Condition(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.FrobeniusNorm(matrix);
        }

        public static Double Trace(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.FrobeniusNorm(matrix);
        }

        public static IMatrix<DoubleComponent> Solve(IMatrix<DoubleComponent> a, IMatrix<DoubleComponent> b)
        {
            return Instance.Operations.Solve(a, b);
        }

        public static IMatrix<DoubleComponent> SolveTranspose(IMatrix<DoubleComponent> a, IMatrix<DoubleComponent> b)
        {
            return Instance.Operations.SolveTranspose(a, b);
        }

        public static IMatrix<DoubleComponent> Transpose(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.Transpose(matrix);
        }

        public static void SetMatrix(IMatrix<DoubleComponent> source, IMatrix<DoubleComponent> target)
        {
            Instance.Operations.SetMatrix(source, target);
        }

        public static ILUDecomposition<DoubleComponent, IMatrix<DoubleComponent>> GetLuDecomposition(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.GetLUDecomposition(matrix);
        }

        public static IQRDecomposition<DoubleComponent, IMatrix<DoubleComponent>> GetQrDecomposition(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.GetQRDecomposition(matrix);
        }

        public static ICholeskyDecomposition<DoubleComponent, IMatrix<DoubleComponent>> GetCholeskyDecomposition(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.GetCholeskyDecomposition(matrix);
        }

        public static ISingularValueDecomposition<DoubleComponent, IMatrix<DoubleComponent>> GetSingularValueDecomposition(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.GetSingularValueDecomposition(matrix);
        }

        public static IEigenvalueDecomposition<DoubleComponent, IMatrix<DoubleComponent>> GetEigenvalueDecomposition(IMatrix<DoubleComponent> matrix)
        {
            return Instance.Operations.GetEigenvalueDecomposition(matrix);
        }
    }
}