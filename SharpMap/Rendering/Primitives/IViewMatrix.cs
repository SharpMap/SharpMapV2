using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public interface IViewMatrix : ICloneable, IEquatable<IViewMatrix>
    {
        void Reset();
        void Invert();
        bool IsInvertible { get; }
        double[,] Elements { get; set; }
        void Rotate(double degreesTheta);
        void RotateAt(double degreesTheta, IViewVector center);
        double GetOffset(int dimension);
        void Offset(IViewVector offsetVector);
        void Multiply(IViewMatrix matrix);
        void Scale(double scaleAmount);
        void Scale(IViewVector scaleVector);
        void Translate(double translationAmount);
        void Translate(IViewVector translationVector);
    }
}
