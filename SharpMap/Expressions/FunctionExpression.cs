using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SharpMap.Expressions
{
    /// <remarks/>
    [XmlInclude(typeof(RecodeType))]
    [XmlInclude(typeof(InterpolateType))]
    [XmlInclude(typeof(CategorizeType))]
    [XmlInclude(typeof(StringLengthType))]
    [XmlInclude(typeof(StringPositionType))]
    [XmlInclude(typeof(TrimType))]
    [XmlInclude(typeof(ChangeCaseType))]
    [XmlInclude(typeof(ConcatenateType))]
    [XmlInclude(typeof(SubstringType))]
    [XmlInclude(typeof(FormatDateType))]
    [XmlInclude(typeof(FormatNumberType))]
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "FunctionType")]
    public abstract class FunctionExpression : Expression
    {
    }

    [Serializable]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "FunctionType", Namespace = "http://www.opengis.net/ogc")]
    [System.Xml.Serialization.XmlRootAttribute("Function", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public partial class FunctionType1 : Expression
    {

        private String _functionName;
        private ExpressionType[] itemsField;

        private ItemsChoiceType[] itemsElementNameField;

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Add", typeof(BinaryOperatorType), Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("Div", typeof(BinaryOperatorType), Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("Function", typeof(FunctionType1), Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("Literal", typeof(LiteralType), Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("Mul", typeof(BinaryOperatorType), Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("PropertyName", typeof(PropertyNameType), Order = 0)]
        [System.Xml.Serialization.XmlElementAttribute("Sub", typeof(BinaryOperatorType), Order = 0)]
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
        [System.Xml.Serialization.XmlElementAttribute("ItemsElementName", Order = 1)]
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public ItemsChoiceType[] ItemsElementName
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
        [XmlAttribute(AttributeName = "name")]
        public String Name
        {
            get
            {
                return _functionName;
            }
            set
            {
                if (_functionName != null)
                {
                    throw new InvalidOperationException("FunctionExpression is read-only after setting.");
                }

                _functionName = value;
            }
        }
    }
}
