using System;
using System.Collections.Generic;
using System.Text;
using SharpMap.Expressions;

namespace SharpMap.Styles.Symbology
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.opengis.net/se", IncludeInSchema = false)]
    public enum ItemsChoiceType2
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://www.opengis.net/ogc:Add")]
        Add,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://www.opengis.net/ogc:Div")]
        Div,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://www.opengis.net/ogc:Function")]
        Function,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://www.opengis.net/ogc:Literal")]
        Literal,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://www.opengis.net/ogc:Mul")]
        Mul,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://www.opengis.net/ogc:PropertyName")]
        PropertyName,

        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("http://www.opengis.net/ogc:Sub")]
        Sub,
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(SvgParameterType))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.opengis.net/se")]
    [System.Xml.Serialization.XmlRootAttribute("InitialGap", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class ParameterValue
    {

        private Expression[] itemsField;

        private ItemsChoiceType2[] itemsElementNameField;

        private string[] textField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Add", typeof(BinaryOperatorType), Namespace = "http://www.opengis.net/ogc")]
        [System.Xml.Serialization.XmlElementAttribute("Div", typeof(BinaryOperatorType), Namespace = "http://www.opengis.net/ogc")]
        [System.Xml.Serialization.XmlElementAttribute("Function", typeof(FunctionType1), Namespace = "http://www.opengis.net/ogc")]
        [System.Xml.Serialization.XmlElementAttribute("Literal", typeof(LiteralType), Namespace = "http://www.opengis.net/ogc")]
        [System.Xml.Serialization.XmlElementAttribute("Mul", typeof(BinaryOperatorType), Namespace = "http://www.opengis.net/ogc")]
        [System.Xml.Serialization.XmlElementAttribute("PropertyName", typeof(PropertyNameType), Namespace = "http://www.opengis.net/ogc")]
        [System.Xml.Serialization.XmlElementAttribute("Sub", typeof(BinaryOperatorType), Namespace = "http://www.opengis.net/ogc")]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
        public ExpressionType[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType2[] ItemsElementName
        {
            get
            {
                return this.itemsElementNameField;
            }
            set
            {
                this.itemsElementNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }
}
