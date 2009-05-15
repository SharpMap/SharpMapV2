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
    [XmlType(TypeName = "LinkType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public abstract class LinkType : BasicLinkType
    {
        [XmlIgnore] private string _httpQuery;
        [XmlIgnore] private List<LinkObjectExtensionGroup> _LinkObjectExtensionGroup;
        [XmlIgnore] private List<string> _LinkSimpleExtensionGroup;
        [XmlIgnore] private double _refreshInterval;

        [XmlIgnore] public bool _refreshIntervalSpecified;
        [XmlIgnore] private RefreshMode _refreshMode;

        [XmlIgnore] public bool _refreshModeSpecified;
        [XmlIgnore] private double _viewBoundScale;

        [XmlIgnore] public bool _viewBoundScaleSpecified;
        [XmlIgnore] private string _viewFormat;


        [XmlIgnore] private ViewRefreshMode _viewRefreshMode;

        [XmlIgnore] public bool _viewRefreshModeSpecified;


        [XmlIgnore] private double _viewRefreshTime;

        [XmlIgnore] public bool _viewRefreshTimeSpecified;

        public LinkType()
        {
            refreshMode = RefreshMode.OnChange;
            refreshInterval = 4.0;
            viewRefreshMode = ViewRefreshMode.Never;
            viewRefreshTime = 4.0;
            viewBoundScale = 1.0;
        }

        [XmlElement(ElementName = "refreshMode", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public RefreshMode refreshMode
        {
            get { return _refreshMode; }
            set
            {
                _refreshMode = value;
                _refreshModeSpecified = true;
            }
        }

        [XmlElement(ElementName = "refreshInterval", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = Declarations.SchemaVersion)]
        public double refreshInterval
        {
            get { return _refreshInterval; }
            set
            {
                _refreshInterval = value;
                _refreshIntervalSpecified = true;
            }
        }

        [XmlElement(ElementName = "viewRefreshMode", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public ViewRefreshMode viewRefreshMode
        {
            get { return _viewRefreshMode; }
            set
            {
                _viewRefreshMode = value;
                _viewRefreshModeSpecified = true;
            }
        }


        [XmlElement(ElementName = "viewRefreshTime", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = Declarations.SchemaVersion)]
        public double viewRefreshTime
        {
            get { return _viewRefreshTime; }
            set
            {
                _viewRefreshTime = value;
                _viewRefreshTimeSpecified = true;
            }
        }


        [XmlElement(ElementName = "viewBoundScale", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = Declarations.SchemaVersion)]
        public double viewBoundScale
        {
            get { return _viewBoundScale; }
            set
            {
                _viewBoundScale = value;
                _viewBoundScaleSpecified = true;
            }
        }

        [XmlElement(ElementName = "viewFormat", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string viewFormat
        {
            get { return _viewFormat; }
            set { _viewFormat = value; }
        }

        [XmlElement(ElementName = "httpQuery", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string httpQuery
        {
            get { return _httpQuery; }
            set { _httpQuery = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "LinkSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> LinkSimpleExtensionGroup
        {
            get
            {
                if (_LinkSimpleExtensionGroup == null) _LinkSimpleExtensionGroup = new List<string>();
                return _LinkSimpleExtensionGroup;
            }
            set { _LinkSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (LinkObjectExtensionGroup), ElementName = "LinkObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<LinkObjectExtensionGroup> LinkObjectExtensionGroup
        {
            get
            {
                if (_LinkObjectExtensionGroup == null)
                    _LinkObjectExtensionGroup = new List<LinkObjectExtensionGroup>();
                return _LinkObjectExtensionGroup;
            }
            set { _LinkObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}