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

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "RuleType")]
    [XmlRoot("Rule", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class Rule
    {
        private string _name;
        private Description _description;
        private LegendGraphic _legendGraphic;
        private Expression _item;
        private double _minScaleDenominator;
        private bool _minScaleDenominatorSpecified;
        private double _maxScaleDenominator;
        private bool _maxScaleDenominatorSpecified;
        private Symbolizer[] _items;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Description Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public LegendGraphic LegendGraphic
        {
            get { return _legendGraphic; }
            set { _legendGraphic = value; }
        }

        [XmlElement("Filter", typeof (FilterExpression), Namespace = "http://www.opengis.net/ogc")]
        [XmlElement("ElseFilter", typeof (ElseFilterExpression))]
        public Expression FilterExpression
        {
            get { return _item; }
            set { _item = value; }
        }

        public double MinScaleDenominator
        {
            get { return _minScaleDenominator; }
            set { _minScaleDenominator = value; }
        }

        [XmlIgnore]
        public bool MinScaleDenominatorSpecified
        {
            get { return _minScaleDenominatorSpecified; }
            set { _minScaleDenominatorSpecified = value; }
        }

        public double MaxScaleDenominator
        {
            get { return _maxScaleDenominator; }
            set { _maxScaleDenominator = value; }
        }

        [XmlIgnore]
        public bool MaxScaleDenominatorSpecified
        {
            get { return _maxScaleDenominatorSpecified; }
            set { _maxScaleDenominatorSpecified = value; }
        }

        [XmlElement("LineSymbolizer", typeof (LineSymbolizer))]
        [XmlElement("PointSymbolizer", typeof (PointSymbolizer))]
        [XmlElement("PolygonSymbolizer", typeof (PolygonSymbolizer))]
        [XmlElement("RasterSymbolizer", typeof (RasterSymbolizer))]
        [XmlElement("TextSymbolizer", typeof (TextSymbolizer))]
        public Symbolizer[] Items
        {
            get { return _items; }
            set { _items = value; }
        }
    }
}