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
    [XmlType(TypeName = "BalloonStyleType", Namespace = "http://www.opengis.net/kml/2.2")]
    [XmlRoot("BalloonStyle", Namespace = "http://www.opengis.net/kml/2.2", IsNullable = false)]
    public class BalloonStyle : SubStyleBase
    {
        private ColorChoice _colorElementNameField;
        private KmlObjectBase[] balloonStyleObjectExtensionGroupField;
        private string[] balloonStyleSimpleExtensionGroupField;
        private DisplayMode displayModeField;

        private bool displayModeFieldSpecified;
        private byte[] itemField;

        private byte[] textColorField;

        private string textField;

        public BalloonStyle()
        {
            displayModeField = DisplayMode.Default;
        }

        /// <remarks/>
        [XmlElement("bgColor", typeof (byte[]), DataType = "hexBinary")]
        [XmlElement("color", typeof (byte[]), DataType = "hexBinary")]
        [XmlChoiceIdentifier("ItemElementName")]
        public byte[] Item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public ColorChoice ColorElementName
        {
            get { return _colorElementNameField; }
            set { _colorElementNameField = value; }
        }

        /// <remarks/>
        // CODEGEN Warning: 'default' attribute on items of type 'hexBinary' is not supported in this version of the .Net Framework.  Ignoring default='ff000000' attribute.
        [XmlElement(DataType = "hexBinary")]
        public byte[] textColor
        {
            get { return textColorField; }
            set { textColorField = value; }
        }

        /// <remarks/>
        public string text
        {
            get { return textField; }
            set { textField = value; }
        }

        /// <remarks/>
        public DisplayMode displayMode
        {
            get { return displayModeField; }
            set { displayModeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool displayModeSpecified
        {
            get { return displayModeFieldSpecified; }
            set { displayModeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("BalloonStyleSimpleExtensionGroup")]
        public string[] BalloonStyleSimpleExtensionGroup
        {
            get { return balloonStyleSimpleExtensionGroupField; }
            set { balloonStyleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("BalloonStyleObjectExtensionGroup")]
        public KmlObjectBase[] BalloonStyleObjectExtensionGroup
        {
            get { return balloonStyleObjectExtensionGroupField; }
            set { balloonStyleObjectExtensionGroupField = value; }
        }
    }
}