using System;
using System.Xml.Serialization;

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "DescriptionType")]
    [XmlRoot("Description", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class Description
    {
        private String _title;
        private String _abstract;

        public String Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public String Abstract
        {
            get { return _abstract; }
            set { _abstract = value; }
        }
    }
}