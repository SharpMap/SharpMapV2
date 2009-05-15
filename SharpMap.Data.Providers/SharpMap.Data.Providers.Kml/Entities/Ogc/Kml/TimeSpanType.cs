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
        [XmlIgnore] private DateTime _begin;

        [XmlIgnore] public bool _beginSpecified;


        [XmlIgnore] private DateTime _end;

        [XmlIgnore] public bool _endSpecified;
        [XmlIgnore] private List<TimeSpanObjectExtensionGroup> _timeSpanObjectExtensionGroup;
        [XmlIgnore] private List<string> _timeSpanSimpleExtensionGroup;

        public TimeSpanType()
        {
            _begin = DateTime.Now;
            _end = DateTime.Now;
        }

        [XmlElement(ElementName = "begin", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "dateTime",
            Namespace = Declarations.SchemaVersion)]
        public DateTime Begin
        {
            get { return _begin; }
            set
            {
                _begin = value;
                _beginSpecified = true;
            }
        }

        [XmlIgnore]
        public DateTime BeginUtc
        {
            get { return _begin.ToUniversalTime(); }
            set
            {
                _begin = value.ToLocalTime();
                _beginSpecified = true;
            }
        }


        [XmlElement(ElementName = "end", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "dateTime",
            Namespace = Declarations.SchemaVersion)]
        public DateTime End
        {
            get { return _end; }
            set
            {
                _end = value;
                _endSpecified = true;
            }
        }

        [XmlIgnore]
        public DateTime EndUtc
        {
            get { return _end.ToUniversalTime(); }
            set
            {
                _end = value.ToLocalTime();
                _endSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "TimeSpanSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> TimeSpanSimpleExtensionGroup
        {
            get
            {
                if (_timeSpanSimpleExtensionGroup == null) _timeSpanSimpleExtensionGroup = new List<string>();
                return _timeSpanSimpleExtensionGroup;
            }
            set { _timeSpanSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (TimeSpanObjectExtensionGroup), ElementName = "TimeSpanObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<TimeSpanObjectExtensionGroup> TimeSpanObjectExtensionGroup
        {
            get
            {
                if (_timeSpanObjectExtensionGroup == null)
                    _timeSpanObjectExtensionGroup = new List<TimeSpanObjectExtensionGroup>();
                return _timeSpanObjectExtensionGroup;
            }
            set { _timeSpanObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}