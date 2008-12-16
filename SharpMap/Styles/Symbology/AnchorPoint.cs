using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "AnchorPoint")]
    [XmlRoot("AnchorPoint", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class AnchorPoint
    {
        private ParameterValue _anchorPointX;
        private ParameterValue _anchorPointY;

        public ParameterValue AnchorPointX
        {
            get { return _anchorPointX; }
            set { _anchorPointX = value; }
        }

        public ParameterValue AnchorPointY
        {
            get { return _anchorPointY; }
            set { _anchorPointY = value; }
        }
    }
}