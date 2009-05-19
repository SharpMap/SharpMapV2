namespace SharpMap.Entities.Iso.Gco
{
    using System;
    using System.Xml.Serialization;

    [Serializable, XmlType(TypeName="Binary_type", Namespace="http://www.isotc211.org/2005/gco")]
    public class BinaryType
    {
        [XmlIgnore]
        private string _src;
        [XmlIgnore]
        private string _value;

        public virtual void MakeSchemaCompliant()
        {
        }

        [XmlAttribute(AttributeName="src", DataType="anyURI")]
        public string Src
        {
            get
            {
                return this._src;
            }
            set
            {
                this._src = value;
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

