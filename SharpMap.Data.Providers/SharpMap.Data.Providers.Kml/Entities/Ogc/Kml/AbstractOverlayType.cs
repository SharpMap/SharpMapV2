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
    [XmlType(TypeName = "AbstractOverlayType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public abstract class AbstractOverlayType : AbstractFeatureType
    {
        [XmlIgnore] private List<AbstractOverlayObjectExtensionGroup> _AbstractOverlayObjectExtensionGroup;
        [XmlIgnore] private List<string> _AbstractOverlaySimpleExtensionGroup;
        [XmlIgnore] private byte[] _color;

        [XmlIgnore] private int _drawOrder;

        [XmlIgnore] public bool _drawOrderSpecified;


        [XmlIgnore] private Icon _Icon;

        public AbstractOverlayType()
        {
            drawOrder = 0;
        }

        [XmlElement(ElementName = "color", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "hexBinary",
            Namespace = Declarations.SchemaVersion)]
        public byte[] color
        {
            get { return _color; }
            set { _color = value; }
        }

        [XmlElement(ElementName = "drawOrder", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "int",
            Namespace = Declarations.SchemaVersion)]
        public int drawOrder
        {
            get { return _drawOrder; }
            set
            {
                _drawOrder = value;
                _drawOrderSpecified = true;
            }
        }

        [XmlElement(Type = typeof (Icon), ElementName = "Icon", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Icon Icon
        {
            get { return _Icon; }
            set { _Icon = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "AbstractOverlaySimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> AbstractOverlaySimpleExtensionGroup
        {
            get
            {
                if (_AbstractOverlaySimpleExtensionGroup == null)
                    _AbstractOverlaySimpleExtensionGroup = new List<string>();
                return _AbstractOverlaySimpleExtensionGroup;
            }
            set { _AbstractOverlaySimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (AbstractOverlayObjectExtensionGroup),
            ElementName = "AbstractOverlayObjectExtensionGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<AbstractOverlayObjectExtensionGroup> AbstractOverlayObjectExtensionGroup
        {
            get
            {
                if (_AbstractOverlayObjectExtensionGroup == null)
                    _AbstractOverlayObjectExtensionGroup = new List<AbstractOverlayObjectExtensionGroup>();
                return _AbstractOverlayObjectExtensionGroup;
            }
            set { _AbstractOverlayObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}