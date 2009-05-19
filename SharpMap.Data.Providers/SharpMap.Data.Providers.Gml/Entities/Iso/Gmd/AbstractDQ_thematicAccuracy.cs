namespace SharpMap.Entities.Iso.Gmd
{
    using SharpMap.Entities.Ogc.Gml;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot(ElementName="AbstractDQ_thematicAccuracy", Namespace="http://www.isotc211.org/2005/gmd", IsNullable=false)]
    public abstract class AbstractDQ_thematicAccuracy : AbstractDQ_thematicAccuracy_Type
    {
        protected AbstractDQ_thematicAccuracy()
        {
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}

