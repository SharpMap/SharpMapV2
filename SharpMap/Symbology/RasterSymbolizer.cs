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

namespace SharpMap.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "RasterSymbolizerType")]
    [XmlRoot("RasterSymbolizer", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    internal class RasterSymbolizer : Symbolizer
    {
        private GeometryPropertyNameExpression _geometry;
        private ParameterValue _opacity;
        private ChannelSelection _channelSelection;
        private OverlapBehavior _overlapBehavior;
        private bool _overlapBehaviorSpecified;
        private ColorMap _colorMap;
        private ContrastEnhancement _contrastEnhancement;
        private ShadedRelief _shadedRelief;
        private ImageOutline _imageOutline;

        public GeometryPropertyNameExpression Geometry
        {
            get { return _geometry; }
            set { _geometry = value; }
        }

        public ParameterValue Opacity
        {
            get { return _opacity; }
            set { _opacity = value; }
        }

        public ChannelSelection ChannelSelection
        {
            get { return _channelSelection; }
            set { _channelSelection = value; }
        }

        public OverlapBehavior OverlapBehavior
        {
            get { return _overlapBehavior; }
            set { _overlapBehavior = value; }
        }

        [XmlIgnore]
        public bool OverlapBehaviorSpecified
        {
            get { return _overlapBehaviorSpecified; }
            set { _overlapBehaviorSpecified = value; }
        }

        public ColorMap ColorMap
        {
            get { return _colorMap; }
            set { _colorMap = value; }
        }

        public ContrastEnhancement ContrastEnhancement
        {
            get { return _contrastEnhancement; }
            set { _contrastEnhancement = value; }
        }

        public ShadedRelief ShadedRelief
        {
            get { return _shadedRelief; }
            set { _shadedRelief = value; }
        }

        public ImageOutline ImageOutline
        {
            get { return _imageOutline; }
            set { _imageOutline = value; }
        }
    }
}