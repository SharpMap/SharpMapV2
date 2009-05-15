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
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Kml
{
    [XmlType(TypeName = "BalloonStyleType", Namespace = Declarations.SchemaVersion), Serializable]
    [XmlInclude(typeof (DataType))]
    [XmlInclude(typeof (AbstractTimePrimitiveType))]
    [XmlInclude(typeof (SchemaDataType))]
    [XmlInclude(typeof (ItemIconType))]
    [XmlInclude(typeof (AbstractLatLonBoxType))]
    [XmlInclude(typeof (OrientationType))]
    [XmlInclude(typeof (AbstractStyleSelectorType))]
    [XmlInclude(typeof (ResourceMapType))]
    [XmlInclude(typeof (LocationType))]
    [XmlInclude(typeof (AbstractSubStyleType))]
    [XmlInclude(typeof (RegionType))]
    [XmlInclude(typeof (AliasType))]
    [XmlInclude(typeof (AbstractViewType))]
    [XmlInclude(typeof (AbstractFeatureType))]
    [XmlInclude(typeof (AbstractGeometryType))]
    [XmlInclude(typeof (BasicLinkType))]
    [XmlInclude(typeof (PairType))]
    [XmlInclude(typeof (ImagePyramidType))]
    [XmlInclude(typeof (ScaleType))]
    [XmlInclude(typeof (LodType))]
    [XmlInclude(typeof (ViewVolumeType))]
    public class BalloonStyleType : AbstractSubStyleType
    {
        [XmlIgnore] private List<BalloonStyleObjectExtensionGroup> _BalloonStyleObjectExtensionGroup;
        [XmlIgnore] private List<string> _BalloonStyleSimpleExtensionGroup;
        [XmlIgnore] private byte[] _bgColor;
        [XmlIgnore] private byte[] _color;
        [XmlIgnore] private DisplayMode _displayMode;

        [XmlIgnore] public bool _displayModeSpecified;
        [XmlIgnore] private string _text;
        [XmlIgnore] private byte[] _textColor;

        public BalloonStyleType()
        {
            displayMode = DisplayMode.Default;
        }

        [XmlElement(ElementName = "color", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "hexBinary",
            Namespace = Declarations.SchemaVersion)]
        public byte[] color
        {
            get { return _color; }
            set { _color = value; }
        }

        [XmlElement(ElementName = "bgColor", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "hexBinary",
            Namespace = Declarations.SchemaVersion)]
        public byte[] bgColor
        {
            get { return _bgColor; }
            set { _bgColor = value; }
        }

        [XmlElement(ElementName = "textColor", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "hexBinary", Namespace = Declarations.SchemaVersion)]
        public byte[] textColor
        {
            get { return _textColor; }
            set { _textColor = value; }
        }

        [XmlElement(ElementName = "text", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string text
        {
            get { return _text; }
            set { _text = value; }
        }


        [XmlElement(ElementName = "displayMode", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public DisplayMode displayMode
        {
            get { return _displayMode; }
            set
            {
                _displayMode = value;
                _displayModeSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "BalloonStyleSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> BalloonStyleSimpleExtensionGroup
        {
            get
            {
                if (_BalloonStyleSimpleExtensionGroup == null) _BalloonStyleSimpleExtensionGroup = new List<string>();
                return _BalloonStyleSimpleExtensionGroup;
            }
            set { _BalloonStyleSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (BalloonStyleObjectExtensionGroup), ElementName = "BalloonStyleObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<BalloonStyleObjectExtensionGroup> BalloonStyleObjectExtensionGroup
        {
            get
            {
                if (_BalloonStyleObjectExtensionGroup == null)
                    _BalloonStyleObjectExtensionGroup = new List<BalloonStyleObjectExtensionGroup>();
                return _BalloonStyleObjectExtensionGroup;
            }
            set { _BalloonStyleObjectExtensionGroup = value; }
        }
    }
}