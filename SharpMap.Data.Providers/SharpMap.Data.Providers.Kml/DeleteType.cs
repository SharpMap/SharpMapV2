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
    [XmlRoot("Delete", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class DeleteType
    {
        private AbstractFeatureType[] itemsField;

        /// <remarks/>
        [XmlElement("NetworkLink", typeof (NetworkLinkType))]
        [XmlElement("Placemark", typeof (PlacemarkType))]
        public AbstractFeatureType[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }
    }
}