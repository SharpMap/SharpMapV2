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
    public partial class ComparisonOperators
    {

        private ComparisonOperator[] comparisonOperatorField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ComparisonOperator")]
        public ComparisonOperator[] ComparisonOperator
        {
            get
            {
                return this.comparisonOperatorField;
            }
            set
            {
                this.comparisonOperatorField = value;
            }
        }
    }
}
