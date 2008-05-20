// Copyright 2007-2008 Rory Plaire (codekaizen@gmail.com)

using System;
using System.Collections.Generic;
using NPack;
using NPack.Interfaces;

namespace SharpMap.Rendering
{
    /// <summary>
    /// Provides access to <see cref="Matrix"/> processing operations via a 
    /// configured operations engine.
    /// </summary>
    sealed class MatrixProcessor
    {
        private static volatile MatrixProcessor _instance;
        private static readonly Object _initSync = new Object();
        private readonly IMatrixOperations<DoubleComponent, Vector, Matrix> _ops;

        static MatrixProcessor() { }

        private MatrixProcessor(IMatrixOperations<DoubleComponent, Vector, Matrix> ops)
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
                            LinearFactory factory = new LinearFactory();
                            IMatrixOperations<DoubleComponent, Vector, Matrix> ops
                                = new ClrMatrixOperations<DoubleComponent, Vector, Matrix>(factory);

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
        public IMatrixOperations<DoubleComponent, Vector, Matrix> Operations
        {
            get { return _ops; }
        }

        public static Matrix Add(Matrix lhs, Matrix rhs)
        {
            return Instance.Operations.Add(lhs, rhs);
        }

        public static Vector Add(Vector lhs, Vector rhs)
        {
            return Instance.Operations.Add(lhs, rhs);
        }

        public static Matrix Subtract(Matrix lhs, Matrix rhs)
        {
            return Instance.Operations.Subtract(lhs, rhs);
        }

        public static Vector Subtract(Vector lhs, Vector rhs)
        {
            return Instance.Operations.Subtract(lhs, rhs);
        }

        public static Matrix Multiply(Matrix lhs, Matrix rhs)
        {
            return Instance.Operations.Multiply(lhs, rhs);
        }

        public static Vector Multiply(Matrix multiplier, Vector columnVector)
        {
            return Instance.Operations.Multiply(multiplier, columnVector);
        }

        public static Vector Multiply(Vector rowVector, Matrix multiplier)
        {
            return Instance.Operations.Multiply(rowVector, multiplier);
        }

        public static IEnumerable<Matrix> Multiply(Matrix lhs, IEnumerable<Matrix> rhs)
        {
            return Instance.Operations.Multiply(lhs, rhs);
        }

        public static IEnumerable<Matrix> Multiply(IEnumerable<Matrix> lhs, Matrix rhs)
        {
            return Instance.Operations.Multiply(lhs, rhs);
        }

        public static IEnumerable<Vector> Multiply(Matrix multiplier, IEnumerable<Vector> columnVectors)
        {
            return Instance.Operations.Multiply(multiplier, columnVectors);
        }

        public static IEnumerable<Vector> Multiply(IEnumerable<Vector> rowVectors, Matrix multiplier)
        {
            return Instance.Operations.Multiply(rowVectors, multiplier);
        }

        public static void Multiply(Matrix multiplier, IEnumerable<DoubleComponent[]> columnVectorsComponents)
        {
            Instance.Operations.Multiply(multiplier, columnVectorsComponents);
        }

        public static void Multiply(IEnumerable<DoubleComponent[]> rowVectorsComponents, Matrix multiplier)
        {
            Instance.Operations.Multiply(rowVectorsComponents, multiplier);
        }

        public static void Multiply(Matrix multiplier, DoubleComponent[] columnVector)
        {
            Instance.Operations.Multiply(multiplier, columnVector);
        }

        public static void Multiply(DoubleComponent[] rowVector, Matrix multiplier)
        {
            Instance.Operations.Multiply(rowVector, multiplier);
        }

        public static Matrix ScalarMultiply(Matrix matrix, DoubleComponent scalar)
        {
            return Instance.Operations.ScalarMultiply(matrix, scalar);
        }

        public static Matrix Invert(Matrix matrix)
        {
            return Instance.Operations.Invert(matrix);
        }

        public static Matrix Negate(Matrix matrix)
        {
            return Instance.Operations.Negate(matrix);
        }

        public static void Rotate(Matrix matrix, Vector axis, Double radians)
        {
            Matrix result = Instance.Operations.Rotate(matrix, axis, radians);
            SetMatrix(result, matrix);
        }

        public static void Scale(Matrix matrix, Vector scaleVector)
        {
            Matrix result = Instance.Operations.Scale(matrix, scaleVector);
            SetMatrix(result, matrix);
        }

        public static void Shear(Matrix matrix, Vector shearVector)
        {
            Instance.Operations.Shear(matrix, shearVector);
        }

        public static void Translate(Matrix affineMatrix, Vector translateVector)
        {
            Matrix result = Instance.Operations.Translate(affineMatrix, translateVector);
            SetMatrix(result, affineMatrix);
        }

        public static Double FrobeniusNorm(Matrix matrix)
        {
            return Instance.Operations.FrobeniusNorm(matrix);
        }

        public static Double OneNorm(Matrix matrix)
        {
            return Instance.Operations.OneNorm(matrix);
        }

        public static Double TwoNorm(Matrix matrix)
        {
            return Instance.Operations.TwoNorm(matrix);
        }

        public static Double InfinityNorm(Matrix matrix)
        {
            return Instance.Operations.InfinityNorm(matrix);
        }

        public static Double Determinant(Matrix matrix)
        {
            return Instance.Operations.Determinant(matrix);
        }

        public static Int32 Rank(Matrix matrix)
        {
            return Instance.Operations.Rank(matrix);
        }

        public static Double Condition(Matrix matrix)
        {
            return Instance.Operations.FrobeniusNorm(matrix);
        }

        public static Double Trace(Matrix matrix)
        {
            return Instance.Operations.FrobeniusNorm(matrix);
        }

        public static Matrix Solve(Matrix a, Matrix b)
        {
            return Instance.Operations.Solve(a, b);
        }

        public static Matrix SolveTranspose(Matrix a, Matrix b)
        {
            return Instance.Operations.SolveTranspose(a, b);
        }

        public static Matrix Transpose(Matrix matrix)
        {
            return Instance.Operations.Transpose(matrix);
        }

        public static void SetMatrix(Matrix source, Matrix target)
        {
            Instance.Operations.SetMatrix(source, target);
        }

        public static ILUDecomposition<DoubleComponent, Matrix> GetLuDecomposition(Matrix matrix)
        {
            return Instance.Operations.GetLUDecomposition(matrix);
        }

        public static IQRDecomposition<DoubleComponent, Matrix> GetQrDecomposition(Matrix matrix)
        {
            return Instance.Operations.GetQRDecomposition(matrix);
        }

        public static ICholeskyDecomposition<DoubleComponent, Matrix> GetCholeskyDecomposition(Matrix matrix)
        {
            return Instance.Operations.GetCholeskyDecomposition(matrix);
        }

        public static ISingularValueDecomposition<DoubleComponent, Matrix> GetSingularValueDecomposition(Matrix matrix)
        {
            return Instance.Operations.GetSingularValueDecomposition(matrix);
        }

        public static IEigenvalueDecomposition<DoubleComponent, Matrix> GetEigenvalueDecomposition(Matrix matrix)
        {
            return Instance.Operations.GetEigenvalueDecomposition(matrix);
        }
    }
}