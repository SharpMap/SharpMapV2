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
    [XmlInclude(typeof (PhotoOverlay))]
    [XmlInclude(typeof (ScreenOverlay))]
    [XmlInclude(typeof (GroundOverlay))]
    [Serializable]
    [XmlType(TypeName = "AbstractOverlayType", Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class OverlayBase : FeatureBase
    {
        private KmlObjectBase[] _kmlOverlayObjectExtensionGroupField;
        private string[] abstractOverlaySimpleExtensionGroupField;
        private byte[] colorField;

        private int drawOrderField;

        private bool drawOrderFieldSpecified;

        private LinkType iconField;

        public OverlayBase()
        {
            drawOrderField = 0;
        }

        /// <remarks/>
        // CODEGEN Warning: 'default' attribute on items of type 'hexBinary' is not supported in this version of the .Net Framework.  Ignoring default='ffffffff' attribute.
        [XmlElement(DataType = "hexBinary")]
        public byte[] color
        {
            get { return colorField; }
            set { colorField = value; }
        }

        /// <remarks/>
        public int drawOrder
        {
            get { return drawOrderField; }
            set { drawOrderField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool drawOrderSpecified
        {
            get { return drawOrderFieldSpecified; }
            set { drawOrderFieldSpecified = value; }
        }

        /// <remarks/>
        public LinkType Icon
        {
            get { return iconField; }
            set { iconField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractOverlaySimpleExtensionGroup")]
        public string[] AbstractOverlaySimpleExtensionGroup
        {
            get { return abstractOverlaySimpleExtensionGroupField; }
            set { abstractOverlaySimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractOverlayObjectExtensionGroup")]
        public KmlObjectBase[] KmlOverlayObjectExtensionGroup
        {
            get { return _kmlOverlayObjectExtensionGroupField; }
            set { _kmlOverlayObjectExtensionGroupField = value; }
        }
    }
}