namespace SharpMap.Entities.Iso.Gmd
{
    using SharpMap.Entities.Iso.Gmd;
    using System;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [Serializable, XmlType(TypeName="CI_roleCode_PropertyType", Namespace="http://www.isotc211.org/2005/gmd")]
    public class CI_roleCode_PropertyType
    {
        [XmlIgnore]
        private SharpMap.Entities.Iso.Gmd.CI_roleCode _cI_RoleCode;
        [XmlIgnore]
        private string _nilReason;

        public virtual void MakeSchemaCompliant()
        {
            this.CI_roleCode.MakeSchemaCompliant();
        }

        [XmlElement(Type=typeof(SharpMap.Entities.Iso.Gmd.CI_roleCode), ElementName="CI_roleCode", IsNullable=false, Form=XmlSchemaForm.Qualified, Namespace="http://www.isotc211.org/2005/gmd")]
        public SharpMap.Entities.Iso.Gmd.CI_roleCode CI_roleCode
        {
            get
            {
                return this._cI_RoleCode;
            }
            set
            {
                this._cI_RoleCode = value;
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

