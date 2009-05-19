using SharpMap.Entities.Iso.Gco;
using SharpMap.Entities.Ogc.Gml;

namespace SharpMap.Entities.Iso.Gmd
{
    using System;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [Serializable, XmlType(TypeName="CI_series_Type", Namespace="http://www.isotc211.org/2005/gmd")]
    public class CI_series_Type : AbstractObjectType
    {
        [XmlIgnore]
        private CharacterStringPropertyType _issueIdentification;
        [XmlIgnore]
        private CharacterStringPropertyType _name;
        [XmlIgnore]
        private CharacterStringPropertyType _page;

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }

        [XmlElement(Type=typeof(CharacterStringPropertyType), ElementName="issueIdentification", IsNullable=false, Form=XmlSchemaForm.Qualified, Namespace="http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType IssueIdentification
        {
            get
            {
                return this._issueIdentification;
            }
            set
            {
                this._issueIdentification = value;
            }
        }

        [XmlElement(Type=typeof(CharacterStringPropertyType), ElementName="name", IsNullable=false, Form=XmlSchemaForm.Qualified, Namespace="http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        [XmlElement(Type=typeof(CharacterStringPropertyType), ElementName="page", IsNullable=false, Form=XmlSchemaForm.Qualified, Namespace="http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType Page
        {
            get
            {
                return this._page;
            }
            set
            {
                this._page = value;
            }
        }
    }
}

