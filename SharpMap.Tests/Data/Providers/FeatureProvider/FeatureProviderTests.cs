using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NUnit.Framework;
using FeatProvider = SharpMap.Data.Providers.FeatureProvider.FeatureProvider;

namespace SharpMap.Tests.Data.Providers.FeatureProvider
{
    [TestFixture]
    public class FeatureProviderTests
    {
        [Test]
        [Ignore("Not implemented")]
        public void CreatingFeatureProviderWithNoSchema()
        {
            FeatProvider provider = new FeatProvider();
            DataTable schema = provider.GetSchemaTable();
            Assert.AreEqual(0, schema.Rows);
        }

        [Test]
        [Ignore("Not implemented")]
        public void CreatingFeatureProviderWithSchema()
        {
            DataColumn[] columns = new DataColumn[]
                {
                    new DataColumn("Column1", typeof(int)), 
                    new DataColumn("Column2", typeof(string)),
                    new DataColumn("Column3", typeof(DateTime)),
                };
            FeatProvider provider = new FeatProvider(columns);
            DataTable schema = provider.GetSchemaTable();
            Assert.AreEqual(3, schema.Rows);
        }
    }
}
