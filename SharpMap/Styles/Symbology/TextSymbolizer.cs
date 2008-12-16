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
    [System.Xml.Serialization.XmlRootAttribute("TextSymbolizer", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    class TextSymbolizer
    {

        private GeometryType geometryField;

        private ParameterValueType labelField;

        private SvgParameterType[] fontField;

        private LabelPlacementType labelPlacementField;

        private HaloType haloField;

        private FillType fillField;

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
        public ParameterValueType Label
        {
            get
            {
                return this.labelField;
            }
            set
            {
                this.labelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("SvgParameter", IsNullable = false)]
        public SvgParameterType[] Font
        {
            get
            {
                return this.fontField;
            }
            set
            {
                this.fontField = value;
            }
        }

        /// <remarks/>
        public LabelPlacementType LabelPlacement
        {
            get
            {
                return this.labelPlacementField;
            }
            set
            {
                this.labelPlacementField = value;
            }
        }

        /// <remarks/>
        public HaloType Halo
        {
            get
            {
                return this.haloField;
            }
            set
            {
                this.haloField = value;
            }
        }

        /// <remarks/>
        public FillType Fill
        {
            get
            {
                return this.fillField;
            }
            set
            {
                this.fillField = value;
            }
        }
    }
}
