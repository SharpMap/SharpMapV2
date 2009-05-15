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
    [XmlType(TypeName = "TimeStampType", Namespace = Declarations.SchemaVersion), Serializable]
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
    public class TimeStampType : AbstractTimePrimitiveType
    {
        [XmlIgnore] private List<TimeStampObjectExtensionGroup> _timeStampObjectExtensionGroup;
        [XmlIgnore] private List<string> _timeStampSimpleExtensionGroup;
        [XmlIgnore] private DateTime _when;

        [XmlIgnore] public bool _whenSpecified;

        public TimeStampType()
        {
            _when = DateTime.Now;
        }


        [XmlElement(ElementName = "when", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "dateTime",
            Namespace = Declarations.SchemaVersion)]
        public DateTime When
        {
            get { return _when; }
            set
            {
                _when = value;
                _whenSpecified = true;
            }
        }

        [XmlIgnore]
        public DateTime WhenUtc
        {
            get { return _when.ToUniversalTime(); }
            set
            {
                _when = value.ToLocalTime();
                _whenSpecified = true;
            }
        }

        [XmlElement(Type = typeof (string), ElementName = "TimeStampSimpleExtensionGroup", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<string> TimeStampSimpleExtensionGroup
        {
            get
            {
                if (_timeStampSimpleExtensionGroup == null) _timeStampSimpleExtensionGroup = new List<string>();
                return _timeStampSimpleExtensionGroup;
            }
            set { _timeStampSimpleExtensionGroup = value; }
        }

        [XmlElement(Type = typeof (TimeStampObjectExtensionGroup), ElementName = "TimeStampObjectExtensionGroup",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<TimeStampObjectExtensionGroup> TimeStampObjectExtensionGroup
        {
            get
            {
                if (_timeStampObjectExtensionGroup == null)
                    _timeStampObjectExtensionGroup = new List<TimeStampObjectExtensionGroup>();
                return _timeStampObjectExtensionGroup;
            }
            set { _timeStampObjectExtensionGroup = value; }
        }

        public new void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}