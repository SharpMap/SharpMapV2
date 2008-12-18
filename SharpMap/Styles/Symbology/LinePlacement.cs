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
    [XmlType(Namespace = "http://www.opengis.net/se")]
    [XmlRoot("LinePlacement", Namespace = "http://www.opengis.net/se", IsNullable = false)]
    public class LinePlacement
    {
        private ParameterValue _perpendicularOffset;
        private bool _isRepeated;
        private bool _isRepeatedSpecified;
        private ParameterValue _initialGap;
        private ParameterValue _gap;
        private bool _isAligned;
        private bool _isAlignedSpecified;
        private bool _generalizeLine;
        private bool _generalizeLineSpecified;

        public ParameterValue PerpendicularOffset
        {
            get { return _perpendicularOffset; }
            set { _perpendicularOffset = value; }
        }

        public bool IsRepeated
        {
            get { return _isRepeated; }
            set { _isRepeated = value; }
        }

        [XmlIgnore]
        public bool IsRepeatedSpecified
        {
            get { return _isRepeatedSpecified; }
            set { _isRepeatedSpecified = value; }
        }

        public ParameterValue InitialGap
        {
            get { return _initialGap; }
            set { _initialGap = value; }
        }

        public ParameterValue Gap
        {
            get { return _gap; }
            set { _gap = value; }
        }

        public bool IsAligned
        {
            get { return _isAligned; }
            set { _isAligned = value; }
        }

        [XmlIgnore]
        public bool IsAlignedSpecified
        {
            get { return _isAlignedSpecified; }
            set { _isAlignedSpecified = value; }
        }

        public bool GeneralizeLine
        {
            get { return _generalizeLine; }
            set { _generalizeLine = value; }
        }

        [XmlIgnore]
        public bool GeneralizeLineSpecified
        {
            get { return _generalizeLineSpecified; }
            set { _generalizeLineSpecified = value; }
        }
    }
}