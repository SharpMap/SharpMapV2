using System;
using System.Xml.Serialization;

namespace SharpMap.Expressions
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", IncludeInSchema = false)]
    public enum ItemChoiceType3
    {
        And,
        Not,
        Or,
        PropertyIsBetween,
        PropertyIsEqualTo,
        PropertyIsGreaterThan,
        PropertyIsGreaterThanOrEqualTo,
        PropertyIsLessThan,
        PropertyIsLessThanOrEqualTo,
        PropertyIsLike,
        PropertyIsNotEqualTo,
        PropertyIsNull,
        comparisonOps,
        logicOps,
    }

    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc")]
    [XmlRoot("Not", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public class UnaryLogicExpression
    {
        private object itemField;

        private ItemChoiceType3 itemElementNameField;

        /// <remarks/>
        [XmlElement("And", typeof (BinaryLogicExpression))]
        [XmlElement("Not", typeof (UnaryLogicExpression))]
        [XmlElement("Or", typeof(BinaryLogicExpression))]
        [XmlElement("PropertyIsBetween", typeof (PropertyIsBetweenExpression))]
        [XmlElement("PropertyIsEqualTo", typeof (BinaryComparisonExpression))]
        [XmlElement("PropertyIsGreaterThan", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsGreaterThanOrEqualTo", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsLessThan", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsLessThanOrEqualTo", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsLike", typeof (PropertyIsLikeExpression))]
        [XmlElement("PropertyIsNotEqualTo", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsNull", typeof (PropertyIsNullExpression))]
        [XmlElement("comparisonOps", typeof (ComparisonExpression))]
        [XmlElement("logicOps", typeof (LogicExpression))]
        [XmlChoiceIdentifier("ItemElementName")]
        public object Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public ItemChoiceType3 ItemElementName
        {
            get { return itemElementNameField; }
            set { itemElementNameField = value; }
        }
    }
}