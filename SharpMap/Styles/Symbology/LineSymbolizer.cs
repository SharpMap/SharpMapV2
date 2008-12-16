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
    [System.Xml.Serialization.XmlRootAttribute("LineSymbolizer", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    class LineSymbolizer
    {

        private GeometryType geometryField;

        private StrokeType strokeField;

        private ParameterValueType perpendicularOffsetField;

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

        /// <remarks/>
        public ParameterValueType PerpendicularOffset
        {
            get
            {
                return this.perpendicularOffsetField;
            }
            set
            {
                this.perpendicularOffsetField = value;
            }
        }
    }
}
