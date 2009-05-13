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
    [XmlInclude(typeof (PolyStyle))]
    [XmlInclude(typeof (LineStyle))]
    [XmlInclude(typeof (LabelStyle))]
    [XmlInclude(typeof (IconStyle))]
    [Serializable]
    [XmlType(TypeName = "AbstractColorStyleType", Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class ColorStyleBase : SubStyleBase
    {
        private KmlObjectBase[] _kmlColorStyleObjectExtensionGroupField;
        private string[] abstractColorStyleSimpleExtensionGroupField;
        private byte[] colorField;

        private ColorMode colorModeField;

        private bool colorModeFieldSpecified;

        public ColorStyleBase()
        {
            colorModeField = ColorMode.Normal;
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
        public ColorMode colorMode
        {
            get { return colorModeField; }
            set { colorModeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool colorModeSpecified
        {
            get { return colorModeFieldSpecified; }
            set { colorModeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractColorStyleSimpleExtensionGroup")]
        public string[] AbstractColorStyleSimpleExtensionGroup
        {
            get { return abstractColorStyleSimpleExtensionGroupField; }
            set { abstractColorStyleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractColorStyleObjectExtensionGroup")]
        public KmlObjectBase[] KmlColorStyleObjectExtensionGroup
        {
            get { return _kmlColorStyleObjectExtensionGroupField; }
            set { _kmlColorStyleObjectExtensionGroupField = value; }
        }
    }
}