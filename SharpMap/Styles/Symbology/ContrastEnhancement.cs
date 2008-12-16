using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se")]
    [XmlRoot("ContrastEnhancement", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    internal class ContrastEnhancement
    {
        private object _item;

        private double _gammaValue;

        private bool _gammaValueSpecified;

        /// <remarks/>
        [XmlElement("Histogram", typeof (Histogram))]
        [XmlElement("Normalize", typeof (Normalize))]
        public object Item
        {
            get { return _item; }
            set { _item = value; }
        }

        /// <remarks/>
        public double GammaValue
        {
            get { return _gammaValue; }
            set { _gammaValue = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool GammaValueSpecified
        {
            get { return _gammaValueSpecified; }
            set { _gammaValueSpecified = value; }
        }
    }
}