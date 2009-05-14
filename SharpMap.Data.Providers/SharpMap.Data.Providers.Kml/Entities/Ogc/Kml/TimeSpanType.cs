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
    [XmlType(TypeName = "TimeSpanType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class TimeSpanType : AbstractTimePrimitiveType
    {
        [XmlIgnore] private DateTime __begin;

        [XmlIgnore] public bool __beginSpecified;


        [XmlIgnore] private DateTime __end;

        [XmlIgnore] public bool __endSpecified;
        [XmlIgnore] private List<TimeSpanObjectExtensionGroup> __TimeSpanObjectExtensionGroup;
        [XmlIgnore] private List<string> __TimeSpanSimpleExtensionGroup;

        public TimeSpanType()
        {
            __begin = DateTime.Now;
            __end = DateTime.Now;
        }

        [XmlElement(ElementName = "begin", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "dateTime",
            Namespace = Declarations.SchemaVersion)]
        public DateTime begin
        {
            get { return __begin; }
            set
            {
                __begin = value;
                __beginSpecified = true;
            }
        }

        [XmlIgnore]
        public DateTime beginUtc
        {
            get { return __begin.ToUniversalTime(); }
            set
            {
                __begin = value.ToLocalTime();
                __beginSpecified = true;
            }
        }


        [XmlElement(ElementName = "end", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "dateTime",
            Namespace = Declarations.SchemaVersion)]
        public DateTime end
        {
            get { return __end; }
            set
            {
                __end = value;
                __endSpecified = true;
            }
        }

        [XmlIgnore]
        public DateTime endUtc
        {
            get { return __end.ToUniversalTime(); }
            set
            {
                __end = value.ToLocalTime();
                __endSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "TimeSpanSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> TimeSpanSimpleExtensionGroup
        {
            get
            {
                if (__TimeSpanSimpleExtensionGroup == null) __TimeSpanSimpleExtensionGroup = new List<string>();
                return __TimeSpanSimpleExtensionGroup;
            }
            set { __TimeSpanSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (TimeSpanObjectExtensionGroup), ElementName = "TimeSpanObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<TimeSpanObjectExtensionGroup> TimeSpanObjectExtensionGroup
        {
            get
            {
                if (__TimeSpanObjectExtensionGroup == null)
                    __TimeSpanObjectExtensionGroup = new List<TimeSpanObjectExtensionGroup>();
                return __TimeSpanObjectExtensionGroup;
            }
            set { __TimeSpanObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}