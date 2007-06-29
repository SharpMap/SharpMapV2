
using NPack;
using NPack.Interfaces;

namespace SharpMap.Rendering
{
    public interface IViewRectangle<TViewPoint> : IMatrix<DoubleComponent>
        where TViewPoint : IVector<DoubleComponent>
    {
        TViewPoint LowerBounds { get; }
        TViewPoint UpperBounds { get; }
    }
}
