
using NPack;
using NPack.Interfaces;

namespace SharpMap.Rendering
{
    public interface IViewRectangle<TViewPoint> : IAffineTransformMatrix<DoubleComponent>
        where TViewPoint : IVector<DoubleComponent>
    {
        TViewPoint LowerBounds { get; }
        TViewPoint UpperBounds { get; }
    }
}
