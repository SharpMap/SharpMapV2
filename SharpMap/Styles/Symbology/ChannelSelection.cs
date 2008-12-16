using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "ChannelSelectionType")]
    [XmlRoot("ChannelSelection", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    internal class ChannelSelection
    {
        private SelectedChannel[] itemsField;

        private ItemsChoiceType4[] itemsElementNameField;

        /// <remarks/>
        [XmlElement("BlueChannel", typeof (SelectedChannel))]
        [XmlElement("GrayChannel", typeof (SelectedChannel))]
        [XmlElement("GreenChannel", typeof (SelectedChannel))]
        [XmlElement("RedChannel", typeof (SelectedChannel))]
        [XmlChoiceIdentifier("ItemsElementName")]
        public SelectedChannel[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }

        /// <remarks/>
        [XmlElement("ItemsElementName")]
        [XmlIgnore]
        public ItemsChoiceType4[] ItemsElementName
        {
            get { return itemsElementNameField; }
            set { itemsElementNameField = value; }
        }
    }
}