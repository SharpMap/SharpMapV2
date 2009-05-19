namespace SharpMap.Entities.Iso.Gmd
{
    using SharpMap.Entities.Iso.Gmd;
    using System;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [Serializable, XmlType(TypeName="CI_presentationFormCode_PropertyType", Namespace="http://www.isotc211.org/2005/gmd")]
    public class CI_presentationFormCode_PropertyType
    {
        [XmlIgnore]
        private SharpMap.Entities.Iso.Gmd.CI_presentationFormCode _cI_PresentationFormCode;
        [XmlIgnore]
        private string _nilReason;

        public virtual void MakeSchemaCompliant()
        {
            this.CI_presentationFormCode.MakeSchemaCompliant();
        }

        [XmlElement(Type=typeof(SharpMap.Entities.Iso.Gmd.CI_presentationFormCode), ElementName="CI_presentationFormCode", IsNullable=false, Form=XmlSchemaForm.Qualified, Namespace="http://www.isotc211.org/2005/gmd")]
        public SharpMap.Entities.Iso.Gmd.CI_presentationFormCode CI_presentationFormCode
        {
            get
            {
                return this._cI_PresentationFormCode;
            }
            set
            {
                this._cI_PresentationFormCode = value;
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
    }
}

