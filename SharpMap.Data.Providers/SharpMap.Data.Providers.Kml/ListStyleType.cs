using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Ogc.Kml
{
    /// <remarks/>
    [GeneratedCode("xsd", "2.0.50727.3038")]
    [Serializable]
    [DebuggerStepThrough]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("ListStyle", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class ListStyleType : AbstractSubStyleType
    {
        private byte[] bgColorField;

        private ItemIconType[] itemIconField;
        private listItemTypeEnumType listItemTypeField;

        private bool listItemTypeFieldSpecified;
        private AbstractObjectType[] listStyleObjectExtensionGroupField;
        private string[] listStyleSimpleExtensionGroupField;

        private int maxSnippetLinesField;

        private bool maxSnippetLinesFieldSpecified;

        public ListStyleType()
        {
            listItemTypeField = listItemTypeEnumType.check;
            maxSnippetLinesField = 2;
        }

        /// <remarks/>
        public listItemTypeEnumType listItemType
        {
            get { return listItemTypeField; }
            set { listItemTypeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool listItemTypeSpecified
        {
            get { return listItemTypeFieldSpecified; }
            set { listItemTypeFieldSpecified = value; }
        }

        /// <remarks/>
        // CODEGEN Warning: 'default' attribute on items of type 'hexBinary' is not supported in this version of the .Net Framework.  Ignoring default='ffffffff' attribute.
        [XmlElement(DataType = "hexBinary")]
        public byte[] bgColor
        {
            get { return bgColorField; }
            set { bgColorField = value; }
        }

        /// <remarks/>
        [XmlElement("ItemIcon")]
        public ItemIconType[] ItemIcon
        {
            get { return itemIconField; }
            set { itemIconField = value; }
        }

        /// <remarks/>
        public int maxSnippetLines
        {
            get { return maxSnippetLinesField; }
            set { maxSnippetLinesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool maxSnippetLinesSpecified
        {
            get { return maxSnippetLinesFieldSpecified; }
            set { maxSnippetLinesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("ListStyleSimpleExtensionGroup")]
        public string[] ListStyleSimpleExtensionGroup
        {
            get { return listStyleSimpleExtensionGroupField; }
            set { listStyleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("ListStyleObjectExtensionGroup")]
        public AbstractObjectType[] ListStyleObjectExtensionGroup
        {
            get { return listStyleObjectExtensionGroupField; }
            set { listStyleObjectExtensionGroupField = value; }
        }
    }
}