using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Styles.Symbology
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.opengis.net/se")]
    [System.Xml.Serialization.XmlRootAttribute("RedChannel", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    class SelectedChannel
    {
        private string sourceChannelNameField;

        private ContrastEnhancementType contrastEnhancementField;

        /// <remarks/>
        public string SourceChannelName
        {
            get
            {
                return this.sourceChannelNameField;
            }
            set
            {
                this.sourceChannelNameField = value;
            }
        }

        /// <remarks/>
        public ContrastEnhancementType ContrastEnhancement
        {
            get
            {
                return this.contrastEnhancementField;
            }
            set
            {
                this.contrastEnhancementField = value;
            }
        }
    }
}
