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
    [XmlType(TypeName = "NetworkLinkControlType", Namespace = Declarations.SchemaVersion), Serializable]
    [XmlInclude(typeof (CameraType))]
    [XmlInclude(typeof (LookAtType))]
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
    public class NetworkLinkControlType
    {
        [XmlIgnore] private AbstractViewGroup __AbstractViewGroup;
        [XmlIgnore] private string __cookie;
        [XmlIgnore] private DateTime __expires;

        [XmlIgnore] public bool __expiresSpecified;
        [XmlIgnore] private string __linkDescription;
        [XmlIgnore] private string __linkName;
        [XmlIgnore] private linkSnippet __linkSnippet;
        [XmlIgnore] private double __maxSessionLength;

        [XmlIgnore] public bool __maxSessionLengthSpecified;
        [XmlIgnore] private string __message;
        [XmlIgnore] private double __minRefreshPeriod;

        [XmlIgnore] public bool __minRefreshPeriodSpecified;
        [XmlIgnore] private List<NetworkLinkControlObjectExtensionGroup> __NetworkLinkControlObjectExtensionGroup;
        [XmlIgnore] private List<string> __NetworkLinkControlSimpleExtensionGroup;
        [XmlIgnore] private Update __Update;

        public NetworkLinkControlType()
        {
            __expires = DateTime.Now;
            minRefreshPeriod = 0.0;
            maxSessionLength = -1.0;
        }


        [XmlElement(ElementName = "minRefreshPeriod", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = Declarations.SchemaVersion)]
        public double minRefreshPeriod
        {
            get { return __minRefreshPeriod; }
            set
            {
                __minRefreshPeriod = value;
                __minRefreshPeriodSpecified = true;
            }
        }


        [XmlElement(ElementName = "maxSessionLength", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = Declarations.SchemaVersion)]
        public double maxSessionLength
        {
            get { return __maxSessionLength; }
            set
            {
                __maxSessionLength = value;
                __maxSessionLengthSpecified = true;
            }
        }

        [XmlElement(ElementName = "cookie", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string cookie
        {
            get { return __cookie; }
            set { __cookie = value; }
        }

        [XmlElement(ElementName = "message", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string message
        {
            get { return __message; }
            set { __message = value; }
        }

        [XmlElement(ElementName = "linkName", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string linkName
        {
            get { return __linkName; }
            set { __linkName = value; }
        }

        [XmlElement(ElementName = "linkDescription", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "string", Namespace = Declarations.SchemaVersion)]
        public string linkDescription
        {
            get { return __linkDescription; }
            set { __linkDescription = value; }
        }

        [XmlElement(Type = typeof (linkSnippet), ElementName = "linkSnippet", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public linkSnippet linkSnippet
        {
            get
            {
                if (__linkSnippet == null) __linkSnippet = new linkSnippet();
                return __linkSnippet;
            }
            set { __linkSnippet = value; }
        }


        [XmlElement(ElementName = "expires", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "dateTime",
            Namespace = Declarations.SchemaVersion)]
        public DateTime expires
        {
            get { return __expires; }
            set
            {
                __expires = value;
                __expiresSpecified = true;
            }
        }

        [XmlIgnore]
        public DateTime expiresUtc
        {
            get { return __expires.ToUniversalTime(); }
            set
            {
                __expires = value.ToLocalTime();
                __expiresSpecified = true;
            }
        }

        [XmlElement(Type = typeof (Update), ElementName = "Update", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Update Update
        {
            get
            {
                if (__Update == null) __Update = new Update();
                return __Update;
            }
            set { __Update = value; }
        }

        [XmlElement(Type = typeof (AbstractViewGroup), ElementName = "AbstractViewGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AbstractViewGroup AbstractViewGroup
        {
            get { return __AbstractViewGroup; }
            set { __AbstractViewGroup = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "NetworkLinkControlSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> NetworkLinkControlSimpleExtensionGroup
        {
            get
            {
                if (__NetworkLinkControlSimpleExtensionGroup == null)
                    __NetworkLinkControlSimpleExtensionGroup = new List<string>();
                return __NetworkLinkControlSimpleExtensionGroup;
            }
            set { __NetworkLinkControlSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (NetworkLinkControlObjectExtensionGroup),
            ElementName = "NetworkLinkControlObjectExtensionGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<NetworkLinkControlObjectExtensionGroup> NetworkLinkControlObjectExtensionGroup
        {
            get
            {
                if (__NetworkLinkControlObjectExtensionGroup == null)
                    __NetworkLinkControlObjectExtensionGroup = new List<NetworkLinkControlObjectExtensionGroup>();
                return __NetworkLinkControlObjectExtensionGroup;
            }
            set { __NetworkLinkControlObjectExtensionGroup = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}