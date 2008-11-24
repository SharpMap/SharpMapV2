using System;
using System.Data;
using Xunit;
using FProvider = SharpMap.Data.Providers.FeatureProvider.FeatureProvider;

namespace SharpMap.Tests.Data.Providers.FeatureProvider
{
    
    public class FeatureProviderTests
    {
        [Fact(Skip = "Incomplete")]
        public void CreatingFeatureProviderWithNoSchema()
        {
            FProvider provider = new FProvider(null);
            DataTable schema = provider.GetSchemaTable();
            Assert.Equal(0, schema.Rows.Count);
        }

        [Fact(Skip = "Incomplete")]
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
            Assert.Equal(3, schema.Rows.Count);
        }
    }
}