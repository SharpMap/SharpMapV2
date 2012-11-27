using System;
using NUnit.Framework;
using SharpMap.Symbology.Serialization;

namespace SharpMap.Symbology.Test.Serialization
{
    [TestFixture]
    public class FeatureTypeStyleSerializerTests
    {
        [Test]
        public void ConstructSerializer()
        {
            Assert.DoesNotThrow(() => { var styleSerializer = new FeatureTypeStyleSerializer(); });
        }
    
        [Test]
        public void DeserializeDegeneratePointSymbolizer()
        {
            FeatureTypeStyleType featureStyleType =
                FeatureTypeStyleSerializer.Deserialize(RootTag(@"<Rule><PointSymbolizer/></Rule>"));
            
            Assert.IsNotNull(featureStyleType);
            Assert.IsNotNull(featureStyleType.Items);
            Assert.AreEqual(1, featureStyleType.Items.Length);

            RuleType rule = featureStyleType.Items[0] as RuleType;
            Assert.IsNotNull(rule);

            Assert.IsNotNull(rule.Items);
            Assert.AreEqual(1, rule.Items.Length);

            PointSymbolizerType pointSymbolizer = rule.Items[0] as PointSymbolizerType;
            Assert.IsNotNull(pointSymbolizer);
        }

        [Test]
        public void SerializeDegeneratePointSymbolizer()
        {
            FeatureTypeStyleType featureStyleType = new FeatureTypeStyleType();
            RuleType rule = new RuleType();
            featureStyleType.Items = new Object[1];
            featureStyleType.Items[0] = rule;
            PointSymbolizerType pointSymbolizer = new PointSymbolizerType();
            rule.Items = new SymbolizerType[1];
            rule.Items[0] = pointSymbolizer;

            String xml = FeatureTypeStyleSerializer.Serialize(featureStyleType);

            StringAssert.Contains("<Rule>", xml);
            StringAssert.Contains("</Rule>", xml);
            StringAssert.Contains("<PointSymbolizer", xml);
        }
    
        private static String RootTag(String contents)
        {
            return String.Format(@"<FeatureTypeStyle version=""1.1.0"" " +
                                    @"xmlns=""http://www.opengis.net/se"" " +
                                    @"xmlns:ogc=""http://www.opengis.net/ogc""  " +
                                    @"xmlns:xlink=""http://www.w3.org/1999/xlink"" " +
                                    @"xmlns:xsi = ""http://www.w3.org/2001/XMLSchema-instance"">" +
                                    @"{0}</FeatureTypeStyle>",
                                 contents);
        }
    }
}
