namespace SharpMap.Entities.Iso.Gmd
{
    using SharpMap.Entities.Ogc.Gml;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot(ElementName="AbstractDQ_result", Namespace="http://www.isotc211.org/2005/gmd", IsNullable=false)]
    public abstract class AbstractDQ_result : AbstractDQ_result_Type
    {
        protected AbstractDQ_result()
        {
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}

