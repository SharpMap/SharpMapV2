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
    [XmlRoot("outerBoundaryIs", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class BoundaryType
    {
        private AbstractObjectType[] boundaryObjectExtensionGroupField;
        private string[] boundarySimpleExtensionGroupField;
        private LinearRingType linearRingField;

        /// <remarks/>
        public LinearRingType LinearRing
        {
            get { return linearRingField; }
            set { linearRingField = value; }
        }

        /// <remarks/>
        [XmlElement("BoundarySimpleExtensionGroup")]
        public string[] BoundarySimpleExtensionGroup
        {
            get { return boundarySimpleExtensionGroupField; }
            set { boundarySimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("BoundaryObjectExtensionGroup")]
        public AbstractObjectType[] BoundaryObjectExtensionGroup
        {
            get { return boundaryObjectExtensionGroupField; }
            set { boundaryObjectExtensionGroupField = value; }
        }
    }
}