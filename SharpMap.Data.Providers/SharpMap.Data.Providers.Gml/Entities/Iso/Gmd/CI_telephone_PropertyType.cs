namespace SharpMap.Entities.Iso.Gmd
{
    using SharpMap.Entities.Iso.Gmd;
    using System;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [Serializable, XmlType(TypeName="CI_telephone_PropertyType", Namespace="http://www.isotc211.org/2005/gmd")]
    public class CI_telephone_PropertyType
    {
        [XmlIgnore]
        private SharpMap.Entities.Ogc.Gml.Actuate _actuate;
        [XmlIgnore]
        private string _arcrole;
        [XmlIgnore]
        private SharpMap.Entities.Iso.Gmd.CI_telephone _cI_Telephone;
        [XmlIgnore]
        private string _href;
        [XmlIgnore]
        private string _nilReason;
        [XmlIgnore]
        private string _role;
        [XmlIgnore]
        private SharpMap.Entities.Ogc.Gml.Show _show;
        [XmlIgnore]
        private string _title;
        [XmlIgnore]
        private string _type;
        [XmlIgnore]
        private string _uuidref;
        [XmlIgnore]
        public bool ActuateSpecified;
        [XmlIgnore]
        public bool ShowSpecified;

        public CI_telephone_PropertyType()
        {
            this.Type = "simple";
        }

        public virtual void MakeSchemaCompliant()
        {
            this.CI_telephone.MakeSchemaCompliant();
        }

        [XmlAttribute(AttributeName="actuate")]
        public SharpMap.Entities.Ogc.Gml.Actuate Actuate
        {
            get
            {
                return this._actuate;
            }
            set
            {
                this._actuate = value;
                this.ActuateSpecified = true;
            }
        }

        [XmlAttribute(AttributeName="arcrole", DataType="anyURI")]
        public string Arcrole
        {
            get
            {
                return this._arcrole;
            }
            set
            {
                this._arcrole = value;
            }
        }

        [XmlElement(Type=typeof(SharpMap.Entities.Iso.Gmd.CI_telephone), ElementName="CI_telephone", IsNullable=false, Form=XmlSchemaForm.Qualified, Namespace="http://www.isotc211.org/2005/gmd")]
        public SharpMap.Entities.Iso.Gmd.CI_telephone CI_telephone
        {
            get
            {
                return this._cI_Telephone;
            }
            set
            {
                this._cI_Telephone = value;
            }
        }

        [XmlAttribute(AttributeName="href", DataType="anyURI")]
        public string Href
        {
            get
            {
                return this._href;
            }
            set
            {
                this._href = value;
            }
        }

        [XmlAttribute(AttributeName="nilReason", DataType="anyURI")]
        public string NilReason
        {
            get
            {
                return this._nilReason;
            }
            set
            {
                this._nilReason = value;
            }
        }

        [XmlAttribute(AttributeName="role", DataType="anyURI")]
        public string Role
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

        [XmlAttribute(AttributeName="show")]
        public SharpMap.Entities.Ogc.Gml.Show Show
        {
            get
            {
                return this._show;
            }
            set
            {
                this._show = value;
                this.ShowSpecified = true;
            }
        }

        [XmlAttribute(AttributeName="title", DataType="string")]
        public string Title
        {
            get
            {
                return this._title;
            }
            set
            {
                this._title = value;
            }
        }

        [XmlAttribute(AttributeName="type", DataType="string")]
        public string Type
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }

        [XmlAttribute(AttributeName="uuidref", DataType="string")]
        public string Uuidref
        {
            get
            {
                return this._uuidref;
            }
            set
            {
                this._uuidref = value;
            }
        }
    }
}

