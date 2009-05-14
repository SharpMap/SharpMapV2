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
        [XmlIgnore] private string __httpQuery;
        [XmlIgnore] private List<LinkObjectExtensionGroup> __LinkObjectExtensionGroup;
        [XmlIgnore] private List<string> __LinkSimpleExtensionGroup;
        [XmlIgnore] private double __refreshInterval;

        [XmlIgnore] public bool __refreshIntervalSpecified;
        [XmlIgnore] private RefreshMode __refreshMode;

        [XmlIgnore] public bool __refreshModeSpecified;
        [XmlIgnore] private double __viewBoundScale;

        [XmlIgnore] public bool __viewBoundScaleSpecified;
        [XmlIgnore] private string __viewFormat;


        [XmlIgnore] private ViewRefreshMode __viewRefreshMode;

        [XmlIgnore] public bool __viewRefreshModeSpecified;


        [XmlIgnore] private double __viewRefreshTime;

        [XmlIgnore] public bool __viewRefreshTimeSpecified;

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
            get { return __refreshMode; }
            set
            {
                __refreshMode = value;
                __refreshModeSpecified = true;
            }
        }

        [XmlElement(ElementName = "refreshInterval", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = Declarations.SchemaVersion)]
        public double refreshInterval
        {
            get { return __refreshInterval; }
            set
            {
                __refreshInterval = value;
                __refreshIntervalSpecified = true;
            }
        }

        [XmlElement(ElementName = "viewRefreshMode", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public ViewRefreshMode viewRefreshMode
        {
            get { return __viewRefreshMode; }
            set
            {
                __viewRefreshMode = value;
                __viewRefreshModeSpecified = true;
            }
        }


        [XmlElement(ElementName = "viewRefreshTime", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = Declarations.SchemaVersion)]
        public double viewRefreshTime
        {
            get { return __viewRefreshTime; }
            set
            {
                __viewRefreshTime = value;
                __viewRefreshTimeSpecified = true;
            }
        }


        [XmlElement(ElementName = "viewBoundScale", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "double", Namespace = Declarations.SchemaVersion)]
        public double viewBoundScale
        {
            get { return __viewBoundScale; }
            set
            {
                __viewBoundScale = value;
                __viewBoundScaleSpecified = true;
            }
        }

        [XmlElement(ElementName = "viewFormat", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string viewFormat
        {
            get { return __viewFormat; }
            set { __viewFormat = value; }
        }

        [XmlElement(ElementName = "httpQuery", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "string",
            Namespace = Declarations.SchemaVersion)]
        public string httpQuery
        {
            get { return __httpQuery; }
            set { __httpQuery = value; }
        }

        [XmlElement(Type = typeof (string), ElementName = "LinkSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> LinkSimpleExtensionGroup
        {
            get
            {
                if (__LinkSimpleExtensionGroup == null) __LinkSimpleExtensionGroup = new List<string>();
                return __LinkSimpleExtensionGroup;
            }
            set { __LinkSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (LinkObjectExtensionGroup), ElementName = "LinkObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<LinkObjectExtensionGroup> LinkObjectExtensionGroup
        {
            get
            {
                if (__LinkObjectExtensionGroup == null)
                    __LinkObjectExtensionGroup = new List<LinkObjectExtensionGroup>();
                return __LinkObjectExtensionGroup;
            }
            set { __LinkObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}