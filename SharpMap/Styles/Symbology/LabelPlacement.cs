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
    [System.Xml.Serialization.XmlRootAttribute("LabelPlacement", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    class LabelPlacement
    {

        private object itemField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("LinePlacement", typeof(LinePlacementType))]
        [System.Xml.Serialization.XmlElementAttribute("PointPlacement", typeof(PointPlacementType))]
        public object Item
        {
            get
            {
                return this.itemField;
            }
            set
            {
                this.itemField = value;
            }
        }
    }
}
