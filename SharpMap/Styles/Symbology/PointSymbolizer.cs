using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Styles.Symbology
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.1432")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.opengis.net/se")]
    [System.Xml.Serialization.XmlRootAttribute("PointSymbolizer", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    class PointSymbolizer
    {

        private GeometryType geometryField;

        private GraphicType graphicField;

        /// <remarks/>
        public GeometryType Geometry
        {
            get
            {
                return this.geometryField;
            }
            set
            {
                this.geometryField = value;
            }
        }

        /// <remarks/>
        public GraphicType Graphic
        {
            get
            {
                return this.graphicField;
            }
            set
            {
                this.graphicField = value;
            }
        }
    }
}
