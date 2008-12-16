using System;
using System.Xml.Serialization;

namespace SharpMap.Expressions
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", TypeName = "FilterType")]
    [XmlRoot("Filter", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public class FilterExpression
    {
        private object[] itemsField;
        private ItemsChoiceType7[] itemsElementNameField;

        [XmlElement("And", typeof(BinaryLogicExpression))]
        [XmlElement("FeatureId", typeof(FeatureIdExpression))]
        [XmlElement("Not", typeof(UnaryLogicExpression))]
        [XmlElement("Or", typeof(BinaryLogicExpression))]
        [XmlElement("PropertyIsBetween", typeof(PropertyIsBetweenExpression))]
        [XmlElement("PropertyIsEqualTo", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsGreaterThan", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsGreaterThanOrEqualTo", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsLessThan", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsLessThanOrEqualTo", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsLike", typeof(PropertyIsLikeExpression))]
        [XmlElement("PropertyIsNotEqualTo", typeof(BinaryComparisonExpression))]
        [XmlElement("PropertyIsNull", typeof(PropertyIsNullExpression))]
        [XmlElement("_Id", typeof(AbstractIdExpression))]
        [XmlElement("comparisonOps", typeof(ComparisonExpression))]
        [XmlElement("logicOps", typeof(LogicExpression))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
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
        public ItemsChoiceType7[] ItemsElementName
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
    }

    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", IncludeInSchema = false)]
    public enum ItemsChoiceType7
    {
        And,
        FeatureId,
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
        _Id,
        comparisonOps,
        logicOps,
    }
}
