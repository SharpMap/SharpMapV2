// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

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
        private ParameterValue perpendicularOffsetField;

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
