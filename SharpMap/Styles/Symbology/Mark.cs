using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Styles.Symbology
{
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.opengis.net/se")]
    [System.Xml.Serialization.XmlRootAttribute("Mark", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    class Mark
    {

        private object[] itemsField;

        private ItemsChoiceType3[] itemsElementNameField;

        private FillType fillField;

        private StrokeType strokeField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Format", typeof(string))]
        [System.Xml.Serialization.XmlElementAttribute("InlineContent", typeof(InlineContentType))]
        [System.Xml.Serialization.XmlElementAttribute("MarkIndex", typeof(string), DataType = "integer")]
        [System.Xml.Serialization.XmlElementAttribute("OnlineResource", typeof(OnlineResourceType))]
        [System.Xml.Serialization.XmlElementAttribute("WellKnownName", typeof(string))]
        [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
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
        public ItemsChoiceType3[] ItemsElementName
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
        public FillType Fill
        {
            get
            {
                return this.fillField;
            }
            set
            {
                this.fillField = value;
            }
        }

        /// <remarks/>
        public StrokeType Stroke
        {
            get
            {
                return this.strokeField;
            }
            set
            {
                this.strokeField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.opengis.net/se", IncludeInSchema = false)]
    public enum ItemsChoiceType3
    {

        /// <remarks/>
        Format,

        /// <remarks/>
        InlineContent,

        /// <remarks/>
        MarkIndex,

        /// <remarks/>
        OnlineResource,

        /// <remarks/>
        WellKnownName,
    }
}
