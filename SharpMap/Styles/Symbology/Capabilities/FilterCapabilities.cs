using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Styles.Symbology.Capabilities
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.opengis.net/ogc")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public partial class Filter_Capabilities
    {

        private Scalar_CapabilitiesType scalar_CapabilitiesField;

        private object[] id_CapabilitiesField;

        /// <remarks/>
        public Scalar_CapabilitiesType Scalar_Capabilities
        {
            get
            {
                return this.scalar_CapabilitiesField;
            }
            set
            {
                this.scalar_CapabilitiesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("EID", typeof(EID), IsNullable = false)]
        [System.Xml.Serialization.XmlArrayItemAttribute("FID", typeof(FID), IsNullable = false)]
        public object[] Id_Capabilities
        {
            get
            {
                return this.id_CapabilitiesField;
            }
            set
            {
                this.id_CapabilitiesField = value;
            }
        }
    }
}
