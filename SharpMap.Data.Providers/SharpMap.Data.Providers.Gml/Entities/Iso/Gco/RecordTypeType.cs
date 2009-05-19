namespace SharpMap.Entities.Iso.Gco
{
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlType(TypeName="RecordType_type", Namespace="http://www.isotc211.org/2005/gco")]
    public class RecordTypeType
    {
        [XmlIgnore]
        private SharpMap.Entities.Ogc.Gml.Actuate _actuate;
        [XmlIgnore]
        private string _arcrole;
        [XmlIgnore]
        private string _href;
        [XmlIgnore]
        private string _role;
        [XmlIgnore]
        private SharpMap.Entities.Ogc.Gml.Show _show;
        [XmlIgnore]
        private string _title;
        [XmlIgnore]
        private string _type;
        [XmlIgnore]
        private string _value;
        [XmlIgnore]
        public bool ActuateSpecified;
        [XmlIgnore]
        public bool ShowSpecified;

        public RecordTypeType()
        {
            this.Type = "simple";
        }

        public virtual void MakeSchemaCompliant()
        {
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

        [XmlText(DataType="string")]
        public string Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }
    }
}

