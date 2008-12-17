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

namespace SharpMap.Styles.Symbology
{
    [Serializable]
    [XmlType(Namespace = "http://www.opengis.net/se", TypeName = "ContrastEnhancementType")]
    [XmlRoot("ContrastEnhancement", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class ContrastEnhancement
    {
        private object _item;
        private double _gammaValue;
        private bool _gammaValueSpecified;

        [XmlElement("Histogram", typeof (Histogram))]
        [XmlElement("Normalize", typeof (Normalize))]
        public object Item
        {
            get { return _item; }
            set { _item = value; }
        }

        public double GammaValue
        {
            get { return _gammaValue; }
            set { _gammaValue = value; }
        }

        [XmlIgnore]
        public bool GammaValueSpecified
        {
            get { return _gammaValueSpecified; }
            set { _gammaValueSpecified = value; }
        }
    }
}