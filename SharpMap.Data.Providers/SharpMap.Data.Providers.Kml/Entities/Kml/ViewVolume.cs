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
    [Serializable]
    [XmlType(TypeName = "ViewVolumeType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("ViewVolume", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class ViewVolume : KmlObjectBase
    {
        private double bottomFovField;

        private bool bottomFovFieldSpecified;
        private double leftFovField;

        private bool leftFovFieldSpecified;
        private double nearField;

        private bool nearFieldSpecified;

        private double rightFovField;

        private bool rightFovFieldSpecified;

        private double topFovField;

        private bool topFovFieldSpecified;

        private KmlObjectBase[] viewVolumeObjectExtensionGroupField;
        private string[] viewVolumeSimpleExtensionGroupField;

        public ViewVolume()
        {
            leftFovField = 0;
            rightFovField = 0;
            bottomFovField = 0;
            topFovField = 0;
            nearField = 0;
        }

        /// <remarks/>
        public double leftFov
        {
            get { return leftFovField; }
            set { leftFovField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool leftFovSpecified
        {
            get { return leftFovFieldSpecified; }
            set { leftFovFieldSpecified = value; }
        }

        /// <remarks/>
        public double rightFov
        {
            get { return rightFovField; }
            set { rightFovField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool rightFovSpecified
        {
            get { return rightFovFieldSpecified; }
            set { rightFovFieldSpecified = value; }
        }

        /// <remarks/>
        public double bottomFov
        {
            get { return bottomFovField; }
            set { bottomFovField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool bottomFovSpecified
        {
            get { return bottomFovFieldSpecified; }
            set { bottomFovFieldSpecified = value; }
        }

        /// <remarks/>
        public double topFov
        {
            get { return topFovField; }
            set { topFovField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool topFovSpecified
        {
            get { return topFovFieldSpecified; }
            set { topFovFieldSpecified = value; }
        }

        /// <remarks/>
        public double near
        {
            get { return nearField; }
            set { nearField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool nearSpecified
        {
            get { return nearFieldSpecified; }
            set { nearFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("ViewVolumeSimpleExtensionGroup")]
        public string[] ViewVolumeSimpleExtensionGroup
        {
            get { return viewVolumeSimpleExtensionGroupField; }
            set { viewVolumeSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("ViewVolumeObjectExtensionGroup")]
        public KmlObjectBase[] ViewVolumeObjectExtensionGroup
        {
            get { return viewVolumeObjectExtensionGroupField; }
            set { viewVolumeObjectExtensionGroupField = value; }
        }
    }
}