namespace SharpMap.Entities.Iso.Gmd
{
    using SharpMap.Entities.Ogc.Gml;
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlRoot(ElementName="CI_responsibleParty", Namespace="http://www.isotc211.org/2005/gmd", IsNullable=false)]
    public class CI_responsibleParty : CI_responsibleParty_Type
    {
        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}

