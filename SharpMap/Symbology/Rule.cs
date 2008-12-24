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
using System.Xml.Serialization;
using SharpMap.Expressions;

namespace SharpMap.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "RuleType")]
    [XmlRoot("Rule", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class Rule
    {
        private String _name;
        private Description _description;
        private LegendGraphic _legendGraphic;
        private Object _item;
        private Double _minScaleDenominator;
        private Boolean _minScaleDenominatorSpecified;
        private Double _maxScaleDenominator;
        private Boolean _maxScaleDenominatorSpecified;
        private Symbolizer[] _items;

        /// <summary>
        /// Gets or sets the unique name for this <see cref="Rule"/>.
        /// </summary>
        /// <remarks>
        /// Not expected to be human readable. Use the <see cref="Description"/> for 
        /// legends.
        /// </remarks>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the human-readable <see cref="Symbology.Description"/> for
        /// this rule, usually displayed in the legend.
        /// </summary>
        public Description Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets or sets an optional explicit graphic symbolizer 
        /// to be displayed in a legend for this rule.
        /// </summary>
        public LegendGraphic LegendGraphic
        {
            get { return _legendGraphic; }
            set { _legendGraphic = value; }
        }

        [XmlElement("Filter", typeof (FilterExpression), Namespace = "http://www.opengis.net/ogc")]
        [XmlElement("ElseFilter", typeof (ElseFilter))]
        public Object Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public Boolean IsElseFilterRule
        {
            get { return Item is ElseFilter; }
        }

        public FilterExpression Filter
        {
            get { return Item as FilterExpression; }
            set { Item = value; }
        }

        /// <summary>
        /// Gets or sets a value which defines the minimum of a range 
        /// of map-rendering scales for which the rule should be applied.
        /// </summary>
        public Double MinScaleDenominator
        {
            get { return _minScaleDenominator; }
            set { _minScaleDenominator = value; }
        }

        /// <summary>
        /// Gets or sets a value which defines the maximum of a range 
        /// of map-rendering scales for which the rule should be applied.
        /// </summary>
        public Double MaxScaleDenominator
        {
            get { return _maxScaleDenominator; }
            set { _maxScaleDenominator = value; }
        }

        /// <summary>
        /// Used by the XML serializer.
        /// </summary>
        [XmlIgnore]
        public Boolean MinScaleDenominatorSpecified
        {
            get { return _minScaleDenominatorSpecified; }
            set { _minScaleDenominatorSpecified = value; }
        }

        /// <summary>
        /// Used by the XML serializer.
        /// </summary>
        [XmlIgnore]
        public Boolean MaxScaleDenominatorSpecified
        {
            get { return _maxScaleDenominatorSpecified; }
            set { _maxScaleDenominatorSpecified = value; }
        }

        /// <summary>
        /// The set of <see cref="Symbolizer"/>s applied by this <see cref="Rule"/>.
        /// </summary>
        [XmlElement("LineSymbolizer", typeof (LineSymbolizer))]
        [XmlElement("PointSymbolizer", typeof (PointSymbolizer))]
        [XmlElement("PolygonSymbolizer", typeof (PolygonSymbolizer))]
        [XmlElement("RasterSymbolizer", typeof (RasterSymbolizer))]
        [XmlElement("TextSymbolizer", typeof (TextSymbolizer))]
        public Symbolizer[] Symbolizers
        {
            get { return _items; }
            set { _items = value; }
        }
    }
}