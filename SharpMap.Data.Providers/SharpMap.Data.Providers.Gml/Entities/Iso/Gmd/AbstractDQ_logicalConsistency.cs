namespace SharpMap.Entities.Iso.Gmd
{
    using SharpMap.Entities.Ogc.Gml;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot(ElementName="AbstractDQ_logicalConsistency", Namespace="http://www.isotc211.org/2005/gmd", IsNullable=false)]
    public abstract class AbstractDQ_logicalConsistency : AbstractDQ_logicalConsistency_Type
    {
        protected AbstractDQ_logicalConsistency()
        {
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}

