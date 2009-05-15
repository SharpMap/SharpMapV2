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
    [XmlType(TypeName = "DocumentType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class DocumentType : AbstractContainerType
    {
        [XmlIgnore] private List<AbstractFeatureGroup> _abstractFeatureGroup;
        [XmlIgnore] private List<DocumentObjectExtensionGroup> _documentObjectExtensionGroup;
        [XmlIgnore] private List<string> _documentSimpleExtensionGroup;
        [XmlIgnore] private List<Schema> _schema;

        [XmlElement(Type = typeof (Schema), ElementName = "Schema", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<Schema> Schema
        {
            get
            {
                if (_schema == null) _schema = new List<Schema>();
                return _schema;
            }
            set { _schema = value; }
        }

        [XmlElement(Type = typeof (AbstractFeatureGroup), ElementName = "AbstractFeatureGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AbstractFeatureGroup> AbstractFeatureGroup
        {
            get
            {
                if (_abstractFeatureGroup == null) _abstractFeatureGroup = new List<AbstractFeatureGroup>();
                return _abstractFeatureGroup;
            }
            set { _abstractFeatureGroup = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "DocumentSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> DocumentSimpleExtensionGroup
        {
            get
            {
                if (_documentSimpleExtensionGroup == null) _documentSimpleExtensionGroup = new List<string>();
                return _documentSimpleExtensionGroup;
            }
            set { _documentSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (DocumentObjectExtensionGroup), ElementName = "DocumentObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<DocumentObjectExtensionGroup> DocumentObjectExtensionGroup
        {
            get
            {
                if (_documentObjectExtensionGroup == null)
                    _documentObjectExtensionGroup = new List<DocumentObjectExtensionGroup>();
                return _documentObjectExtensionGroup;
            }
            set { _documentObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}