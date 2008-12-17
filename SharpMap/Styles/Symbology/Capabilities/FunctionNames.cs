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
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.opengis.net/ogc")]
    public partial class FunctionNamesType
    {

        private FunctionNameType[] functionNameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("FunctionName")]
        public FunctionNameType[] FunctionName
        {
            get
            {
                return this.functionNameField;
            }
            set
            {
                this.functionNameField = value;
            }
        }
    }
}
