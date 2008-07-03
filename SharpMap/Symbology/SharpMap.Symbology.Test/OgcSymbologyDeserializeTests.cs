using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SharpMap.Symbology.StyleAdditions;


namespace SharpMap.Symbology.Test
{
    [TestFixture]
    public class OgcSymbologyDeserializeTests
    {
        [Test]
        public void DeserializeLabelStyleName()
        {
            String xml = addToRoot(@"<Name>Test Style</Name>" +
                                   @"<Rule><TextSymbolizer/></Rule>");
            LabelStyle labelStyle = OgcSymbologyDeserialize.LabelStyle(xml);

            Assert.AreEqual("Test Style", labelStyle.Name);
        }

        [Test]
        public void DeserializeLabelStyleDescription()
        {
            String xml = addToRoot(@"<Description><Title>title</Title><Abstract>abstract</Abstract></Description>" +
                                   @"<Rule><TextSymbolizer/></Rule>");
            LabelStyle labelStyle = OgcSymbologyDeserialize.LabelStyle(xml);

            Assert.IsNotNull(labelStyle.Description);
            Assert.AreEqual("title", labelStyle.Description.Title);
            Assert.AreEqual("abstract", labelStyle.Description.Abstract);
        }

        private String addToRoot(String contents)
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
