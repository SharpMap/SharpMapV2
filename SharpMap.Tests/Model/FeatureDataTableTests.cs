using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SharpMap;
using System.Data;

namespace SharpMap.Tests.Model
{
    [TestFixture]
    public class FeatureDataTableTests
    {
        [Test]
        public void CreateTable()
        {
            FeatureDataTable table = new FeatureDataTable();
            Assert.IsNotNull(table);
            Assert.IsNull(table.DataSet);
            Assert.IsNull(table.DefaultView);
        }

        [Test]
        public void NewRowReturnsDetachedFeatureDataRow()
        {
            FeatureDataTable table = new FeatureDataTable();
            FeatureDataRow row = table.NewRow();
            Assert.IsNotNull(row);
            Assert.AreEqual(0, table.Rows.Count);
            Assert.IsNull(row.Table);
        }
    }
}
