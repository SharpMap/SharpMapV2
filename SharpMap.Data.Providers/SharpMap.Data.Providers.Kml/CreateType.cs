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
    [XmlRoot("Create", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class CreateType
    {
        private AbstractContainerType[] itemsField;

        /// <remarks/>
        [XmlElement("Document", typeof (DocumentType))]
        [XmlElement("Folder", typeof (FolderType))]
        public AbstractContainerType[] Items
        {
            get { return itemsField; }
            set { itemsField = value; }
        }
    }
}