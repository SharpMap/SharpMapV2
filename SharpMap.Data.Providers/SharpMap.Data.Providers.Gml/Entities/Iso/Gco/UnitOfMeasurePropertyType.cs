namespace SharpMap.Entities.Iso.Gco
{
    using System;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [Serializable, XmlType(TypeName="UnitOfMeasure_propertyType", Namespace="http://www.isotc211.org/2005/gco")]
    public class UnitOfMeasurePropertyType
    {
        [XmlIgnore]
        private SharpMap.Entities.Ogc.Gml.Actuate _actuate;
        [XmlIgnore]
        private string _arcrole;
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
        private SharpMap.Entities.Ogc.Gml.UnitDefinition _unitDefinition;
        [XmlIgnore]
        private string _uuidref;
        [XmlIgnore]
        public bool ActuateSpecified;
        [XmlIgnore]
        public bool ShowSpecified;

        public UnitOfMeasurePropertyType()
        {
            this.Type = "simple";
        }

        public virtual void MakeSchemaCompliant()
        {
            this.UnitDefinition.MakeSchemaCompliant();
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

        [XmlElement(Type=typeof(SharpMap.Entities.Ogc.Gml.UnitDefinition), ElementName="UnitDefinition", IsNullable=false, Form=XmlSchemaForm.Qualified, Namespace="http://www.opengis.net/gml/3.2")]
        public SharpMap.Entities.Ogc.Gml.UnitDefinition UnitDefinition
        {
            get
            {
                return this._unitDefinition;
            }
            set
            {
                this._unitDefinition = value;
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

