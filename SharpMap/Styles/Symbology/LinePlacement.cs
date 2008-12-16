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
    [System.Xml.Serialization.XmlRootAttribute("LinePlacement", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    class LinePlacement
    {

        private ParameterValueType perpendicularOffsetField;

        private bool isRepeatedField;

        private bool isRepeatedFieldSpecified;

        private ParameterValueType initialGapField;

        private ParameterValueType gapField;

        private bool isAlignedField;

        private bool isAlignedFieldSpecified;

        private bool generalizeLineField;

        private bool generalizeLineFieldSpecified;

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

        /// <remarks/>
        public bool IsRepeated
        {
            get
            {
                return this.isRepeatedField;
            }
            set
            {
                this.isRepeatedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsRepeatedSpecified
        {
            get
            {
                return this.isRepeatedFieldSpecified;
            }
            set
            {
                this.isRepeatedFieldSpecified = value;
            }
        }

        /// <remarks/>
        public ParameterValueType InitialGap
        {
            get
            {
                return this.initialGapField;
            }
            set
            {
                this.initialGapField = value;
            }
        }

        /// <remarks/>
        public ParameterValueType Gap
        {
            get
            {
                return this.gapField;
            }
            set
            {
                this.gapField = value;
            }
        }

        /// <remarks/>
        public bool IsAligned
        {
            get
            {
                return this.isAlignedField;
            }
            set
            {
                this.isAlignedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsAlignedSpecified
        {
            get
            {
                return this.isAlignedFieldSpecified;
            }
            set
            {
                this.isAlignedFieldSpecified = value;
            }
        }

        /// <remarks/>
        public bool GeneralizeLine
        {
            get
            {
                return this.generalizeLineField;
            }
            set
            {
                this.generalizeLineField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool GeneralizeLineSpecified
        {
            get
            {
                return this.generalizeLineFieldSpecified;
            }
            set
            {
                this.generalizeLineFieldSpecified = value;
            }
        }
    }
}
