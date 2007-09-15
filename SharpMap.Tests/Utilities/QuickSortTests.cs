
using System;
using System.Collections.Generic;
using NUnit.Framework;
using SharpMap.Utilities;

namespace SharpMap.Tests.Utilities
{
    [TestFixture]
    public class QuickSortTests
    {
        [Test]
        public void SortingDoubleArrayWithEvenNumberOfElementsSortsCorrectly()
        {
            Random rnd = new Random();
            List<double> list = new List<double>(100000);
            for (int i = 0; i < 100000; i++)
            {
                list.Add(rnd.NextDouble());
            }

            QuickSort.Sort(list, delegate(double lhs, double rhs)
                { return lhs < rhs ? -1 : lhs > rhs ? 1 : 0; });

            for(int i = 1; i< list.Count; i++)
            {
                Assert.GreaterOrEqual(list[i], list[i-1]);
            }
        }

        [Test]
        public void SortingDoubleArrayWithOddNumberOfElementsSortsCorrectly()
        {
            Random rnd = new Random();
            List<double> list = new List<double>(100001);
            for (int i = 0; i < 100001; i++)
            {
                list.Add(rnd.NextDouble());
            }

            QuickSort.Sort(list, delegate(double lhs, double rhs)
            { return lhs < rhs ? -1 : lhs > rhs ? 1 : 0; });

            for (int i = 1; i < list.Count; i++)
            {
                Assert.GreaterOrEqual(list[i], list[i - 1]);
            }
        }
    }
}
