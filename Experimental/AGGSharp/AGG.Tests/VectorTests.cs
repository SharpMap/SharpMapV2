using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NPack;
using NPack.Interfaces;
using AGG.Transform;


namespace NUnitReflexive
{
    [TestFixture]
    public class Vector3DTests
    {
        [Test]
        public void VectorAdditionAndSubtraction()
        {
            IVector<DoubleComponent> Point1 = MatrixFactory<DoubleComponent>.CreateVector3D(1, 1, 1);   //new Vector3D();
            //Point1.Set(1, 1, 1);

            IVector<DoubleComponent> Point2 = MatrixFactory<DoubleComponent>.CreateVector3D(2, 2, 2);
            //Point2.Set(2, 2, 2);

            IVector<DoubleComponent> Point3; ;
            Point3 = Point1.Add(Point2);

            Assert.IsTrue(Point3.Equals(MatrixFactory<DoubleComponent>.CreateVector3D(3, 3, 3)));

            Point3 = Point1.Subtract(Point2);
            Assert.IsTrue(Point3.Equals(MatrixFactory<DoubleComponent>.CreateVector3D(-1, -1, -1)));

            Point3.AddEquals(Point1);

            Assert.IsTrue(Point3.Equals(MatrixFactory<DoubleComponent>.CreateVector3D(0, 0, 0)));

            Point3.AddEquals(Point2);
            Assert.IsTrue(Point3.Equals(MatrixFactory<DoubleComponent>.CreateVector3D(2, 2, 2)));

            Point3.SetFrom(MatrixFactory<DoubleComponent>.CreateVector3D(3, -4, 5));
            Assert.IsTrue(Point3.GetMagnitude().GreaterThan(7.07) && Point3.GetMagnitude().LessThan(7.08));

            IVector<DoubleComponent> InlineOpLeftSide = MatrixFactory<DoubleComponent>.CreateVector3D(5.0f, -3.0f, .0f);
            IVector<DoubleComponent> InlineOpRightSide = MatrixFactory<DoubleComponent>.CreateVector3D(-5.0f, 4.0f, 1.0f);

            Assert.IsTrue(
                InlineOpLeftSide.Add(InlineOpRightSide)
                .Equals(
                    MatrixFactory<DoubleComponent>.CreateVector3D(.0f, 1.0f, 1.0f)
                    ));

            Assert.IsTrue(
                InlineOpLeftSide.Subtract(InlineOpRightSide)
                .Equals(
                    MatrixFactory<DoubleComponent>.CreateVector3D(10.0f, -7.0f, -1.0f))
                    );
        }

        [Test]
        public void ScalarMultiplication()
        {
            IVector<DoubleComponent> ScalarMultiplicationArgument = MatrixFactory<DoubleComponent>.CreateVector3D(5.0f, 4.0f, 3.0f);
            Assert.IsTrue(ScalarMultiplicationArgument.Multiply(-.5)
                .Equals(
                    MatrixFactory<DoubleComponent>.CreateVector3D(2.5f, 2.0f, 1.5f).Negative())
                    );

            Assert.IsTrue(
                ScalarMultiplicationArgument.Multiply(-.5)
                .Equals(
                    MatrixFactory<DoubleComponent>.CreateVector3D(2.5f, 2.0f, 1.5f).Negative())
                    );


            Assert.IsTrue(
                ScalarMultiplicationArgument.Multiply(5)
                .Equals(
                    MatrixFactory<DoubleComponent>.CreateVector3D(25.0f, 20.0f, 15.0f))
                    );

            IVector<DoubleComponent> Point3 = MatrixFactory<DoubleComponent>.CreateVector3D(2, 3, 4);
            Point3.MultiplyEquals(6);
            Assert.IsTrue(IComparable<IVector<DoubleComponent>>.Equals(Point3, MatrixFactory<DoubleComponent>.CreateVector3D(12, 18, 24)));
        }

        [Test]
        public void ScalarDivision()
        {
            IVector<DoubleComponent> ScalarMultiplicationArgument = MatrixFactory<DoubleComponent>.CreateVector3D(5.0f, 4.0f, 3.0f);
            Assert.IsTrue(ScalarMultiplicationArgument.Divide(2).Equals(MatrixFactory<DoubleComponent>.CreateVector3D(2.5f, 2.0f, 1.5f)));
            Assert.IsTrue(ScalarMultiplicationArgument.Multiply(.5).Equals(MatrixFactory<DoubleComponent>.CreateVector3D(2.5f, 2.0f, 1.5f)));

            IVector<DoubleComponent> Point3 = MatrixFactory<DoubleComponent>.CreateVector3D(12, 18, 24);
            Point3.DivideEquals(6);
            Assert.IsTrue(IComparable<IVector<DoubleComponent>>.Equals(Point3, MatrixFactory<DoubleComponent>.CreateVector3D(2, 3, 4)));
        }

        [Test]
        public void DotProduct()
        {
            IVector<DoubleComponent> Test1 = MatrixFactory<DoubleComponent>.CreateVector3D(10, 1, 2);
            IVector<DoubleComponent> Test2 = MatrixFactory<DoubleComponent>.CreateVector3D(1, 0, 0);
            DoubleComponent DotResult = Test2.Dot(Test1);
            Assert.IsTrue(DotResult.Equals(10));
        }

        [Test]
        public void CrossProduct()
        {
            IVector<DoubleComponent> Test1 = MatrixFactory<DoubleComponent>.CreateVector3D(10, 0, 0);
            IVector<DoubleComponent> Test2 = MatrixFactory<DoubleComponent>.CreateVector3D(1, 1, 0);
            IVector<DoubleComponent> CrossResult = Test2.Cross(Test1);
            Assert.IsTrue(CrossResult[0].Equals(0));
            Assert.IsTrue(CrossResult[1].Equals(0));
            Assert.IsTrue(CrossResult[2].LessThan(0));
        }


        [Test]
        public void Normalize()
        {
            IVector<DoubleComponent> Point3 = MatrixFactory<DoubleComponent>.CreateVector3D(3, -4, 5);
            Point3 = Point3.Normalize();
            Assert.IsTrue(Point3.GetMagnitude().GreaterThan(0.99) && Point3.GetMagnitude().LessThan(1.01));
        }

        [Test]
        public void Rotate()
        {
            //IVector<DoubleComponent> Test = MatrixFactory<DoubleComponent>.CreateVector3D(0, 1, 0);
            //Test.RotateAboutX(System.Math.PI / 2);
            //Assert.IsTrue(Test.Equals(new Vector3D(0, 0, 1), 0.001f));
            //Test.RotateAboutY(System.Math.PI / 2);
            //Assert.IsTrue(Test.Equals(new Vector3D(1, 0, 0), 0.001f));
            //Test.RotateAboutZ(System.Math.PI / 2);
            //Assert.IsTrue(Test.Equals(new Vector3D(0, 1, 0), 0.001f));
        }
    }
}

