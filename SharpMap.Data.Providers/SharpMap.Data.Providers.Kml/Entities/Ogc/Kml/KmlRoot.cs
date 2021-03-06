// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software � 2008 Newgrove Consultants Limited, 
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
    [XmlRoot(ElementName = "kml", Namespace = Declarations.SchemaVersion, IsNullable = false), Serializable]
    [XmlType(TypeName = "KmlType", Namespace = Declarations.SchemaVersion)]
    [XmlInclude(typeof (AbstractContainerType))]
    [XmlInclude(typeof (PlacemarkType))]
    [XmlInclude(typeof (NetworkLinkType))]
    [XmlInclude(typeof (AbstractOverlayType))]
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
    public class KmlRoot
    {
        [XmlIgnore] private AbstractFeatureGroup _abstractFeatureGroup;
        [XmlIgnore] private string _hint;
        [XmlIgnore] private List<KmlObjectExtensionGroup> _kmlObjectExtensionGroup;
        [XmlIgnore] private List<string> _kmlSimpleExtensionGroup;

        [XmlIgnore] private NetworkLinkControl _networkLinkControl;

        [XmlAttribute(AttributeName = "hint", DataType = "string")]
        public string Hint
        {
            get { return _hint; }
            set { _hint = value; }
        }

        [XmlElement(Type = typeof (NetworkLinkControl), ElementName = "NetworkLinkControl", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public NetworkLinkControl NetworkLinkControl
        {
            get { return _networkLinkControl; }
            set { _networkLinkControl = value; }
        }

        [XmlElement(Type = typeof (AbstractFeatureGroup), ElementName = "AbstractFeatureGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AbstractFeatureGroup AbstractFeatureGroup
        {
            get { return _abstractFeatureGroup; }
            set { _abstractFeatureGroup = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "KmlSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> KmlSimpleExtensionGroup
        {
            get
            {
                if (_kmlSimpleExtensionGroup == null) _kmlSimpleExtensionGroup = new List<string>();
                return _kmlSimpleExtensionGroup;
            }
            set { _kmlSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (KmlObjectExtensionGroup), ElementName = "KmlObjectExtensionGroup", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<KmlObjectExtensionGroup> KmlObjectExtensionGroup
        {
            get
            {
                if (_kmlObjectExtensionGroup == null) _kmlObjectExtensionGroup = new List<KmlObjectExtensionGroup>();
                return _kmlObjectExtensionGroup;
            }
            set { _kmlObjectExtensionGroup = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}