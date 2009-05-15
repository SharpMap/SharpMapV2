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
    [XmlType(TypeName = "PairType", Namespace = Declarations.SchemaVersion), Serializable]
    [XmlInclude(typeof (StyleMapType))]
    [XmlInclude(typeof (StyleType))]
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
    public class PairType : AbstractObjectType
    {
        [XmlIgnore] private AbstractStyleSelectorGroup _abstractStyleSelectorGroup;
        [XmlIgnore] private StyleState _key;

        [XmlIgnore] public bool _keySpecified;
        [XmlIgnore] private List<PairObjectExtensionGroup> _pairObjectExtensionGroup;
        [XmlIgnore] private List<string> _pairSimpleExtensionGroup;


        [XmlIgnore] private string _styleUrl;

        public PairType()
        {
            key = StyleState.Normal;
        }

        [XmlElement(ElementName = "key", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public StyleState key
        {
            get { return _key; }
            set
            {
                _key = value;
                _keySpecified = true;
            }
        }

        [XmlElement(ElementName = "styleUrl", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI",
            Namespace = Declarations.SchemaVersion)]
        public string styleUrl
        {
            get { return _styleUrl; }
            set { _styleUrl = value; }
        }

        [XmlElement(Type = typeof (AbstractStyleSelectorGroup), ElementName = "AbstractStyleSelectorGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AbstractStyleSelectorGroup AbstractStyleSelectorGroup
        {
            get { return _abstractStyleSelectorGroup; }
            set { _abstractStyleSelectorGroup = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "PairSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> PairSimpleExtensionGroup
        {
            get
            {
                if (_pairSimpleExtensionGroup == null) _pairSimpleExtensionGroup = new List<string>();
                return _pairSimpleExtensionGroup;
            }
            set { _pairSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (PairObjectExtensionGroup), ElementName = "PairObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PairObjectExtensionGroup> PairObjectExtensionGroup
        {
            get
            {
                if (_pairObjectExtensionGroup == null)
                    _pairObjectExtensionGroup = new List<PairObjectExtensionGroup>();
                return _pairObjectExtensionGroup;
            }
            set { _pairObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}