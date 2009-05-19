namespace SharpMap.Entities.Iso.Gmd
{
    using SharpMap.Entities.Ogc.Gml;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot(ElementName="DQ_griddedDataPositionalAccuracy", Namespace="http://www.isotc211.org/2005/gmd", IsNullable=false)]
    public class DQ_griddedDataPositionalAccuracy : DQ_griddedDataPositionalAccuracy_Type
    {
        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}

