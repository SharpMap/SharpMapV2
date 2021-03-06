namespace SharpMap.Entities.Iso.Gco
{
    using System;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [Serializable, XmlType(TypeName="TypeName_type", Namespace="http://www.isotc211.org/2005/gco")]
    public class TypeNameType : AbstractObjectType
    {
        [XmlIgnore]
        private CharacterStringPropertyType _aName;

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            this.AName.MakeSchemaCompliant();
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
    }
}

