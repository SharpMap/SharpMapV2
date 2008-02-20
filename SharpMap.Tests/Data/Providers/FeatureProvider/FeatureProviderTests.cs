using System;
using System.Data;
using NUnit.Framework;
using FProvider = SharpMap.Data.Providers.FeatureProvider.FeatureProvider;

namespace SharpMap.Tests.Data.Providers.FeatureProvider
{
    [TestFixture]
    public class FeatureProviderTests
    {
        [Test]
        [Ignore("Not implemented")]
        public void CreatingFeatureProviderWithNoSchema()
        {
            FProvider provider = new FProvider(null);
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

            FProvider provider = new FProvider(null, columns);
            DataTable schema = provider.GetSchemaTable();
            Assert.AreEqual(3, schema.Rows);
        }
    }
}