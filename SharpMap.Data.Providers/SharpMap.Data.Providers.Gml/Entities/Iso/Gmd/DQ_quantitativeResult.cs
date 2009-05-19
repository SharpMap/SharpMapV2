namespace SharpMap.Entities.Iso.Gmd
{
    using SharpMap.Entities.Ogc.Gml;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot(ElementName="DQ_quantitativeResult", Namespace="http://www.isotc211.org/2005/gmd", IsNullable=false)]
    public class DQ_quantitativeResult : DQ_quantitativeResult_Type
    {
        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}

