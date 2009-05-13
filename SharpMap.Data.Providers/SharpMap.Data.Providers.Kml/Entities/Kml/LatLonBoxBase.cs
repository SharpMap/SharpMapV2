// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software © 2008 Newgrove Consultants Limited, 
//  *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
//  *  of the current GNU Lesser General Public License (LGPL) as published by and 
//  *  available from the Free Software Foundation, Inc., 
//  *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
//  *  This program is distributed without any warranty; 
//  *  without even the implied warranty of merchantability or fitness for purpose.  
//  *  See the GNU Lesser General Public License for the full details. 
//  *  
//  *  Author: John Diss 2009
//  * 
//  */
using System;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Kml
{
    /// <remarks/>
    [XmlInclude(typeof (LatLonBox))]
    [XmlInclude(typeof (LatLonAltBox))]
    [Serializable]
    [XmlType(TypeName = "AbstractLatLonBoxType", Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class LatLonBoxBase : KmlObjectBase
    {
        private KmlObjectBase[] _kmlLatLonBoxObjectExtensionGroupField;
        private string[] abstractLatLonBoxSimpleExtensionGroupField;
        private double eastField;

        private bool eastFieldSpecified;
        private double northField;

        private bool northFieldSpecified;

        private double southField;

        private bool southFieldSpecified;

        private double westField;

        private bool westFieldSpecified;

        public LatLonBoxBase()
        {
            northField = 180;
            southField = -180;
            eastField = 180;
            westField = -180;
        }

        /// <remarks/>
        public double north
        {
            get { return northField; }
            set { northField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool northSpecified
        {
            get { return northFieldSpecified; }
            set { northFieldSpecified = value; }
        }

        /// <remarks/>
        public double south
        {
            get { return southField; }
            set { southField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool southSpecified
        {
            get { return southFieldSpecified; }
            set { southFieldSpecified = value; }
        }

        /// <remarks/>
        public double east
        {
            get { return eastField; }
            set { eastField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool eastSpecified
        {
            get { return eastFieldSpecified; }
            set { eastFieldSpecified = value; }
        }

        /// <remarks/>
        public double west
        {
            get { return westField; }
            set { westField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool westSpecified
        {
            get { return westFieldSpecified; }
            set { westFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractLatLonBoxSimpleExtensionGroup")]
        public string[] AbstractLatLonBoxSimpleExtensionGroup
        {
            get { return abstractLatLonBoxSimpleExtensionGroupField; }
            set { abstractLatLonBoxSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractLatLonBoxObjectExtensionGroup")]
        public KmlObjectBase[] KmlLatLonBoxObjectExtensionGroup
        {
            get { return _kmlLatLonBoxObjectExtensionGroupField; }
            set { _kmlLatLonBoxObjectExtensionGroupField = value; }
        }
    }
}