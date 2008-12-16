using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Styles.Symbology
{
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.opengis.net/se")]
    [System.Xml.Serialization.XmlRootAttribute("Font", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    class Font
    {
        private SvgParameter[] _svgParameter;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SvgParameter")]
        public SvgParameter[] SvgParameter
        {
            get
            {
                return this._svgParameter;
            }
            set
            {
                this._svgParameter = value;
            }
        }
    }
}
