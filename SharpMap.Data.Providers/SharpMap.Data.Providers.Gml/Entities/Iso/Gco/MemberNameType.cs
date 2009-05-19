namespace SharpMap.Entities.Iso.Gco
{
    using System;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [Serializable, XmlType(TypeName="MemberName_type", Namespace="http://www.isotc211.org/2005/gco")]
    public class MemberNameType : AbstractObjectType
    {
        [XmlIgnore]
        private CharacterStringPropertyType _aName;
        [XmlIgnore]
        private TypeNamePropertyType _attributeType;

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            this.AName.MakeSchemaCompliant();
            this.AttributeType.MakeSchemaCompliant();
        }

        [XmlElement(Type=typeof(CharacterStringPropertyType), ElementName="aName", IsNullable=false, Form=XmlSchemaForm.Qualified, Namespace="http://www.isotc211.org/2005/gco")]
        public CharacterStringPropertyType AName
        {
            get
            {
                return this._aName;
            }
            set
            {
                this._aName = value;
            }
        }

        [XmlElement(Type=typeof(TypeNamePropertyType), ElementName="attributeType", IsNullable=false, Form=XmlSchemaForm.Qualified, Namespace="http://www.isotc211.org/2005/gco")]
        public TypeNamePropertyType AttributeType
        {
            get
            {
                return this._attributeType;
            }
            set
            {
                this._attributeType = value;
            }
        }
    }
}

