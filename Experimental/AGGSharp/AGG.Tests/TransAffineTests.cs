using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NPack.Interfaces;
using NPack;
using AGG.Transform;

namespace NUnitAgg
{
    [TestFixture]
    public class TransAffineTest
    {
        [Test]
        public void InvertTest()
        {
            //Affine a = Affine.NewIdentity();
            IAffineTransformMatrix<DoubleComponent> a =
                MatrixFactory<DoubleComponent>.NewIdentity(VectorDimension.Two);

            a.Translate(
                MatrixFactory<DoubleComponent>.CreateVector2D(
                M.New<DoubleComponent>(10),
                M.New<DoubleComponent>(10)));

            IAffineTransformMatrix<DoubleComponent> b = MatrixFactory<DoubleComponent>.CreateAffine(a);
            b = b.Inverse;



            DoubleComponent x = 100;
            DoubleComponent y = 100;
            DoubleComponent newx = x;
            DoubleComponent newy = y;

            IVector<DoubleComponent> v1 =
                MatrixFactory<DoubleComponent>.CreateVector2D(
                    M.New<DoubleComponent>(100),
                    M.New<DoubleComponent>(100));

            IVector<DoubleComponent> v2 = a.TransformVector(v1);
            IVector<DoubleComponent> v3 = b.TransformVector(v2);


            //a.Transform(ref newx, ref newy);
            //b.Transform(ref newx, ref newy);
            Assert.AreEqual((double)v1[0], (double)v3[0], .001);
            Assert.AreEqual((double)v1[1], (double)v3[1], .001);
        }

        [Test]
        public void TransformTest()
        {
            IAffineTransformMatrix<DoubleComponent> a = MatrixFactory<DoubleComponent>.NewIdentity(VectorDimension.Two);
            a.Translate(MatrixFactory<DoubleComponent>.CreateVector2D(10, 20));

            DoubleComponent x = 10;
            DoubleComponent y = 20;
            DoubleComponent newx = 0;
            DoubleComponent newy = 0;

            a.Transform(ref newx, ref newy);
            Assert.AreEqual((double)x, (double)newx, .001);
            Assert.AreEqual((double)y, (double)newy, .001);
        }
    }
}
