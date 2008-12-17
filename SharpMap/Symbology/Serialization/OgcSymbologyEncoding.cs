using System;
using System.ComponentModel;
using System.Xml.Serialization;


namespace SharpMap.Symbology.Serialization
{
    /// <remarks/>
    [XmlInclude(typeof (UnaryLogicOpType))]
    [XmlInclude(typeof (BinaryLogicOpType))]
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc")]
    public abstract class LogicOpsType {}

    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc")]
    [XmlRoot("And", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public class BinaryLogicOpType : LogicOpsType
    {
        private object[] itemsField;

        private ItemsChoiceType6[] itemsElementNameField;

        /// <remarks/>
        [XmlElement("And", typeof (BinaryLogicOpType))]
        [XmlElement("Not", typeof (UnaryLogicOpType))]
        [XmlElement("Or", typeof (BinaryLogicOpType))]
        [XmlElement("PropertyIsBetween", typeof (PropertyIsBetweenType))]
        [XmlElement("PropertyIsEqualTo", typeof (BinaryComparisonOpType))]
        [XmlElement("PropertyIsGreaterThan", typeof (BinaryComparisonOpType))]
        [XmlElement("PropertyIsGreaterThanOrEqualTo", typeof (BinaryComparisonOpType))]
        [XmlElement("PropertyIsLessThan", typeof (BinaryComparisonOpType))]
        [XmlElement("PropertyIsLessThanOrEqualTo", typeof (BinaryComparisonOpType))]
        [XmlElement("PropertyIsLike", typeof (PropertyIsLikeType))]
        [XmlElement("PropertyIsNotEqualTo", typeof (BinaryComparisonOpType))]
        [XmlElement("PropertyIsNull", typeof (PropertyIsNullType))]
        [XmlElement("comparisonOps", typeof (ComparisonOpsType))]
        [XmlElement("logicOps", typeof (LogicOpsType))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public object[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ItemsChoiceType6[] ItemsElementName
        {
            get { return itemsElementNameField; }
            set { itemsElementNameField = value; }
        }
    }

    public class BinaryOperatorType : ExpressionType
    {
    }


    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc")]
    [XmlRoot("PropertyIsEqualTo", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public class BinaryComparisonOpType : ComparisonOpsType
    {
        private ExpressionType[] itemsField;

        private ItemsChoiceType5[] itemsElementNameField;

        private bool matchCaseField;

        public BinaryComparisonOpType()
        {
            matchCaseField = true;
        }

        /// <remarks/>
        [XmlElement("Add", typeof (BinaryOperatorType))]
        [XmlElement("Div", typeof (BinaryOperatorType))]
        [XmlElement("Function", typeof (FunctionType1))]
        [XmlElement("Literal", typeof (LiteralType))]
        [XmlElement("Mul", typeof (BinaryOperatorType))]
        [XmlElement("PropertyName", typeof (PropertyNameType))]
        [XmlElement("Sub", typeof (BinaryOperatorType))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public ExpressionType[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ItemsChoiceType5[] ItemsElementName
        {
            get { return itemsElementNameField; }
            set { itemsElementNameField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        [DefaultValue(true)]
        public bool matchCase
        {
            get { return matchCaseField; }
            set { matchCaseField = value; }
        }
    }


    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc", IncludeInSchema = false)]
    public enum ItemsChoiceType6
    {
        /// <remarks/>
        And,

        /// <remarks/>
        Not,

        /// <remarks/>
        Or,

        /// <remarks/>
        PropertyIsBetween,

        /// <remarks/>
        PropertyIsEqualTo,

        /// <remarks/>
        PropertyIsGreaterThan,

        /// <remarks/>
        PropertyIsGreaterThanOrEqualTo,

        /// <remarks/>
        PropertyIsLessThan,

        /// <remarks/>
        PropertyIsLessThanOrEqualTo,

        /// <remarks/>
        PropertyIsLike,

        /// <remarks/>
        PropertyIsNotEqualTo,

        /// <remarks/>
        PropertyIsNull,

        /// <remarks/>
        comparisonOps,

        /// <remarks/>
        logicOps,
    }


    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc")]
    [XmlRoot("SortBy", Namespace = "http://www.opengis.net/ogc", IsNullable = false)]
    public class SortByType
    {
        private SortPropertyType[] sortPropertyField;

        /// <remarks/>
        [XmlElement("SortProperty")]
        public SortPropertyType[] SortProperty
        {
            get { return sortPropertyField; }
            set { sortPropertyField = value; }
        }
    }

    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc")]
    public class SortPropertyType
    {
        private PropertyNameType propertyNameField;

        private SortOrderType sortOrderField;

        private bool sortOrderFieldSpecified;

        /// <remarks/>
        public PropertyNameType PropertyName
        {
            get { return propertyNameField; }
            set { propertyNameField = value; }
        }

        /// <remarks/>
        public SortOrderType SortOrder
        {
            get { return sortOrderField; }
            set { sortOrderField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool SortOrderSpecified
        {
            get { return sortOrderFieldSpecified; }
            set { sortOrderFieldSpecified = value; }
        }
    }

    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/ogc")]
    public enum SortOrderType
    {
        /// <remarks/>
        DESC,

        /// <remarks/>
        ASC,
    }
}