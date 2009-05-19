namespace SharpMap.Entities.Iso.Gco
{
    using SharpMap.Entities.Ogc.Gml;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot(ElementName="Binary", Namespace="http://www.isotc211.org/2005/gco", IsNullable=false)]
    public class Binary : BinaryType
    {
        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}

