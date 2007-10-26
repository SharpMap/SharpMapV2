using System;
using System.Data;
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
                    new DataColumn("Column1", typeof (Int32)),
                    new DataColumn("Column2", typeof (String)),
                    new DataColumn("Column3", typeof (DateTime)),
                };
            FeatProvider provider = new FeatProvider(columns);
            DataTable schema = provider.GetSchemaTable();
            Assert.AreEqual(3, schema.Rows);
        }
    }
}