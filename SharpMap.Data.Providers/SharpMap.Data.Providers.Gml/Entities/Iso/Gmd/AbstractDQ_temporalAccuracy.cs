namespace SharpMap.Entities.Iso.Gmd
{
    using SharpMap.Entities.Ogc.Gml;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot(ElementName="AbstractDQ_temporalAccuracy", Namespace="http://www.isotc211.org/2005/gmd", IsNullable=false)]
    public abstract class AbstractDQ_temporalAccuracy : AbstractDQ_temporalAccuracy_Type
    {
        protected AbstractDQ_temporalAccuracy()
        {
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}

