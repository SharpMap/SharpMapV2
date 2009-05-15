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
using System.Xml;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAL
{
    [XmlType(TypeName = "SubPremiseNumber", Namespace = Declarations.SchemaVersion), Serializable]
    public class SubPremiseNumber
    {
        [XmlIgnore] private string _code;
        [XmlIgnore] private string _indicator;

        [XmlIgnore] private Occurrence _indicatorOccurrence;

        [XmlIgnore] public bool _indicatorOccurrenceSpecified;

        [XmlIgnore] public bool _numberTypeOccurrenceSpecified;

        [XmlIgnore] private string _premiseNumberSeparator;

        [XmlIgnore] private string _type;
        [XmlIgnore] private string _value;
        [XmlIgnore] private Occurrence _occurrence;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Indicator")]
        public string Indicator
        {
            get { return _indicator; }
            set { _indicator = value; }
        }

        [XmlAttribute(AttributeName = "IndicatorOccurrence")]
        public Occurrence IndicatorOccurrence
        {
            get { return _indicatorOccurrence; }
            set
            {
                _indicatorOccurrence = value;
                _indicatorOccurrenceSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "NumberTypeOccurrence")]
        public Occurrence Occurrence
        {
            get { return _occurrence; }
            set
            {
                _occurrence = value;
                _numberTypeOccurrenceSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "PremiseNumberSeparator")]
        public string PremiseNumberSeparator
        {
            get { return _premiseNumberSeparator; }
            set { _premiseNumberSeparator = value; }
        }

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [XmlAttribute(AttributeName = "Code")]
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        [XmlText(DataType = "string")]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}