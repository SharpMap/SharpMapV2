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
    [XmlType(TypeName = "NetworkLinkType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class NetworkLinkType : AbstractFeatureType
    {
        [XmlIgnore] private bool __flyToView;

        [XmlIgnore] public bool __flyToViewSpecified;
        [XmlIgnore] private Link __Link;
        [XmlIgnore] private List<NetworkLinkObjectExtensionGroup> __NetworkLinkObjectExtensionGroup;
        [XmlIgnore] private List<string> __NetworkLinkSimpleExtensionGroup;
        [XmlIgnore] private bool __refreshVisibility;

        [XmlIgnore] public bool __refreshVisibilitySpecified;
        [XmlIgnore] private Url __Url;

        public NetworkLinkType()
        {
            refreshVisibility = false;
            flyToView = false;
        }


        [XmlElement(ElementName = "refreshVisibility", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "boolean", Namespace = Declarations.SchemaVersion)]
        public bool refreshVisibility
        {
            get { return __refreshVisibility; }
            set
            {
                __refreshVisibility = value;
                __refreshVisibilitySpecified = true;
            }
        }


        [XmlElement(ElementName = "flyToView", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "boolean",
            Namespace = Declarations.SchemaVersion)]
        public bool flyToView
        {
            get { return __flyToView; }
            set
            {
                __flyToView = value;
                __flyToViewSpecified = true;
            }
        }

        [XmlElement(Type = typeof (Url), ElementName = "Url", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Url Url
        {
            get
            {
                
                return __Url;
            }
            set { __Url = value; }
        }

        [XmlElement(Type = typeof (Link), ElementName = "Link", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Link link
        {
            get
            {
                
                return __Link;
            }
            set { __Link = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "NetworkLinkSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> NetworkLinkSimpleExtensionGroup
        {
            get
            {
                if (__NetworkLinkSimpleExtensionGroup == null) __NetworkLinkSimpleExtensionGroup = new List<string>();
                return __NetworkLinkSimpleExtensionGroup;
            }
            set { __NetworkLinkSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (NetworkLinkObjectExtensionGroup), ElementName = "NetworkLinkObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<NetworkLinkObjectExtensionGroup> NetworkLinkObjectExtensionGroup
        {
            get
            {
                if (__NetworkLinkObjectExtensionGroup == null)
                    __NetworkLinkObjectExtensionGroup = new List<NetworkLinkObjectExtensionGroup>();
                return __NetworkLinkObjectExtensionGroup;
            }
            set { __NetworkLinkObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}