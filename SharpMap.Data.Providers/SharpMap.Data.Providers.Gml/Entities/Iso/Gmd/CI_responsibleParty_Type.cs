using SharpMap.Entities.Iso.Gco;

namespace SharpMap.Entities.Iso.Gmd
{
    using System;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [Serializable, XmlType(TypeName="CI_responsibleParty_Type", Namespace="http://www.isotc211.org/2005/gmd")]
    public class CI_responsibleParty_Type : AbstractObjectType
    {
        [XmlIgnore]
        private CI_contact_PropertyType _contactInfo;
        [XmlIgnore]
        private CharacterStringPropertyType _individualName;
        [XmlIgnore]
        private CharacterStringPropertyType _organisationName;
        [XmlIgnore]
        private CharacterStringPropertyType _positionName;
        [XmlIgnore]
        private CI_roleCode_PropertyType _role;

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            this.Role.MakeSchemaCompliant();
        }

        [XmlElement(Type=typeof(CI_contact_PropertyType), ElementName="contactInfo", IsNullable=false, Form=XmlSchemaForm.Qualified, Namespace="http://www.isotc211.org/2005/gmd")]
        public CI_contact_PropertyType ContactInfo
        {
            get
            {
                return this._contactInfo;
            }
            set
            {
                this._contactInfo = value;
            }
        }

        [XmlElement(Type=typeof(CharacterStringPropertyType), ElementName="individualName", IsNullable=false, Form=XmlSchemaForm.Qualified, Namespace="http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType IndividualName
        {
            get
            {
                return this._individualName;
            }
            set
            {
                this._individualName = value;
            }
        }

        [XmlElement(Type=typeof(CharacterStringPropertyType), ElementName="organisationName", IsNullable=false, Form=XmlSchemaForm.Qualified, Namespace="http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType OrganisationName
        {
            get
            {
                return this._organisationName;
            }
            set
            {
                this._organisationName = value;
            }
        }

        [XmlElement(Type=typeof(CharacterStringPropertyType), ElementName="positionName", IsNullable=false, Form=XmlSchemaForm.Qualified, Namespace="http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType PositionName
        {
            get
            {
                return this._positionName;
            }
            set
            {
                this._positionName = value;
            }
        }

        [XmlElement(Type=typeof(CI_roleCode_PropertyType), ElementName="role", IsNullable=false, Form=XmlSchemaForm.Qualified, Namespace="http://www.isotc211.org/2005/gmd")]
        public CI_roleCode_PropertyType Role
        {
            get
            {
                return this._role;
            }
            set
            {
                this._role = value;
            }
        }
    }
}

