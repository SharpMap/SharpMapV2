using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SharpMap.Symbology.StyleAdditions;


namespace SharpMap.Symbology.Test
{
    [TestFixture]
    public class OgcSymbologySerializeTests
    {
        [Test]
        public void SerializeLabelStyleName()
        {
            LabelStyle labelStyle = new LabelStyle();
            labelStyle.Name = "Test Style";

            String xml = OgcSymbologySerialize.LabelStyle(labelStyle);

            StringAssert.Contains(@"<Name>Test Style</Name>", xml);
            StringAssert.Contains(@"<FeatureTypeStyle", xml);
        }
        

        [Test]
        public void SerializeLabelStyleDescription()
        {
            LabelStyle labelStyle = new LabelStyle();
            labelStyle.Description = new Description("description","abstract");

            String xml = OgcSymbologySerialize.LabelStyle(labelStyle);

            StringAssert.Contains(@"<Title>description</Title>", xml);
            StringAssert.Contains(@"<Abstract>abstract</Abstract>", xml);
        }
    }
}
