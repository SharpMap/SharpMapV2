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
        [XmlIgnore] private AbstractViewGroup _abstractViewGroup;
        [XmlIgnore] private string _cookie;
        [XmlIgnore] private DateTime _expires;

        [XmlIgnore] public bool _expiresSpecified;
        [XmlIgnore] private string _linkDescription;
        [XmlIgnore] private string _linkName;
        [XmlIgnore] private linkSnippet _linkSnippet;
        [XmlIgnore] private double _maxSessionLength;

        [XmlIgnore] public bool _maxSessionLengthSpecified;
        [XmlIgnore] private string _message;
        [XmlIgnore] private double _minRefreshPeriod;

        [XmlIgnore] public bool _minRefreshPeriodSpecified;
        [XmlIgnore] private List<NetworkLinkControlObjectExtensionGroup> _networkLinkControlObjectExtensionGroup;
        [XmlIgnore] private List<string> _networkLinkControlSimpleExtensionGroup;
        [XmlIgnore] private Update _update;

        public NetworkLinkControlType()
        {
            _expires = DateTime.Now;
            minRefreshPeriod = 0.0;
            maxSessionLength = -1.0;
        }


        [XmlElement(ElementName = "minRefreshPeriod", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = Declarations.SchemaVersion)]
        public double minRefreshPeriod
        {
            get { return _minRefreshPeriod; }
            set
            {
                _minRefreshPeriod = value;
                _minRefreshPeriodSpecified = true;
            }
        }


        [XmlElement(ElementName = "maxSessionLength", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = Declarations.SchemaVersion)]
        public double maxSessionLength
        {
            get { return _maxSessionLength; }
            set
            {
                _maxSessionLength = value;
                _maxSessionLengthSpecified = true;
            }
        }

        [XmlElement(ElementName = "cookie", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string cookie
        {
            get { return _cookie; }
            set { _cookie = value; }
        }

        [XmlElement(ElementName = "message", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string message
        {
            get { return _message; }
            set { _message = value; }
        }

        [XmlElement(ElementName = "linkName", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string linkName
        {
            get { return _linkName; }
            set { _linkName = value; }
        }

        [XmlElement(ElementName = "linkDescription", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "string", Namespace = Declarations.SchemaVersion)]
        public string linkDescription
        {
            get { return _linkDescription; }
            set { _linkDescription = value; }
        }

        [XmlElement(Type = typeof (linkSnippet), ElementName = "linkSnippet", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public linkSnippet linkSnippet
        {
            get { return _linkSnippet; }
            set { _linkSnippet = value; }
        }


        [XmlElement(ElementName = "expires", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "dateTime",
            Namespace = Declarations.SchemaVersion)]
        public DateTime expires
        {
            get { return _expires; }
            set
            {
                _expires = value;
                _expiresSpecified = true;
            }
        }

        [XmlIgnore]
        public DateTime expiresUtc
        {
            get { return _expires.ToUniversalTime(); }
            set
            {
                _expires = value.ToLocalTime();
                _expiresSpecified = true;
            }
        }

        [XmlElement(Type = typeof (Update), ElementName = "Update", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Update Update
        {
            get { return _update; }
            set { _update = value; }
        }

        [XmlElement(Type = typeof (AbstractViewGroup), ElementName = "AbstractViewGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public AbstractViewGroup AbstractViewGroup
        {
            get { return _abstractViewGroup; }
            set { _abstractViewGroup = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "NetworkLinkControlSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> NetworkLinkControlSimpleExtensionGroup
        {
            get
            {
                if (_networkLinkControlSimpleExtensionGroup == null)
                    _networkLinkControlSimpleExtensionGroup = new List<string>();
                return _networkLinkControlSimpleExtensionGroup;
            }
            set { _networkLinkControlSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (NetworkLinkControlObjectExtensionGroup),
            ElementName = "NetworkLinkControlObjectExtensionGroup", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public List<NetworkLinkControlObjectExtensionGroup> NetworkLinkControlObjectExtensionGroup
        {
            get
            {
                if (_networkLinkControlObjectExtensionGroup == null)
                    _networkLinkControlObjectExtensionGroup = new List<NetworkLinkControlObjectExtensionGroup>();
                return _networkLinkControlObjectExtensionGroup;
            }
            set { _networkLinkControlObjectExtensionGroup = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}