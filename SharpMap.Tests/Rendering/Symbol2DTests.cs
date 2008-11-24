
using SharpMap.Rendering.Rendering2D;
using Xunit;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;
using IMatrixD = NPack.Interfaces.IMatrix<NPack.DoubleComponent>;
using ITransformMatrixD = NPack.Interfaces.ITransformMatrix<NPack.DoubleComponent>;

namespace SharpMap.Tests.Rendering
{

    #region Symbol2D

    
    public class Symbol2DTests
    {
        [Fact]
        public void SizeTest()
        {
            Symbol2D s1 = new Symbol2D(new Size2D(16, 16));
            Assert.Equal(new Size2D(16, 16), s1.Size);
        }

        [Fact(Skip = "Incomplete")]
        public void DataTest()
        {
        }

        [Fact(Skip = "Incomplete")]
        public void OffsetTest()
        {
        }

        [Fact(Skip = "Incomplete")]
        public void RotationTest()
        {
        }

        [Fact(Skip = "Incomplete")]
        public void ScaleTest()
        {
        }

        [Fact(Skip = "Incomplete")]
        public void CloneTest()
        {
        }
    }

    #endregion
}