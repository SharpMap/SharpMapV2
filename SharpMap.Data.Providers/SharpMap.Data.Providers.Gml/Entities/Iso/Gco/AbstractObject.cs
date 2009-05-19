namespace SharpMap.Entities.Iso.Gco
{
    using SharpMap.Entities.Ogc.Gml;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot(ElementName="AbstractObject", Namespace="http://www.isotc211.org/2005/gco", IsNullable=false)]
    public abstract class AbstractObject : AbstractObjectType
    {
        protected AbstractObject()
        {
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}

