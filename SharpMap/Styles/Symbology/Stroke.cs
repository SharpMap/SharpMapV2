using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Styles.Symbology
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.opengis.net/se", TypeName = "StrokeType")]
    [System.Xml.Serialization.XmlRootAttribute("Stroke", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class Stroke
    {
        private object itemField;

        private SvgParameterType[] svgParameterField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("GraphicFill", typeof(GraphicFillType))]
        [System.Xml.Serialization.XmlElementAttribute("GraphicStroke", typeof(GraphicStrokeType))]
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SvgParameter")]
        public SvgParameterType[] SvgParameter
        {
            get
            {
                return this.svgParameterField;
            }
            set
            {
                this.svgParameterField = value;
            }
        }
    }
}
