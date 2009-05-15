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
using System.Xml;
using System.Xml.Serialization;

namespace SharpMap.Entities.xAL
{
    [XmlType(TypeName = "SubPremiseNumber", Namespace = Declarations.SchemaVersion), Serializable]
    public class SubPremiseNumber
    {
        [XmlIgnore] private string _Code;
        [XmlIgnore] private string _Indicator;

        [XmlIgnore] private Occurrence _IndicatorOccurrence;

        [XmlIgnore] public bool _IndicatorOccurrenceSpecified;

        [XmlIgnore] public bool _NumberTypeOccurrenceSpecified;

        [XmlIgnore] private string _PremiseNumberSeparator;

        [XmlIgnore] private string _Type;
        [XmlIgnore] private string _Value;
        [XmlIgnore] private Occurrence _occurrence;
        [XmlAnyAttribute] public XmlAttribute[] AnyAttr;

        [XmlAttribute(AttributeName = "Indicator")]
        public string Indicator
        {
            get { return _Indicator; }
            set { _Indicator = value; }
        }

        [XmlAttribute(AttributeName = "IndicatorOccurrence")]
        public Occurrence IndicatorOccurrence
        {
            get { return _IndicatorOccurrence; }
            set
            {
                _IndicatorOccurrence = value;
                _IndicatorOccurrenceSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "NumberTypeOccurrence")]
        public Occurrence Occurrence
        {
            get { return _occurrence; }
            set
            {
                _occurrence = value;
                _NumberTypeOccurrenceSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "PremiseNumberSeparator")]
        public string PremiseNumberSeparator
        {
            get { return _PremiseNumberSeparator; }
            set { _PremiseNumberSeparator = value; }
        }

        [XmlAttribute(AttributeName = "Type")]
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        [XmlAttribute(AttributeName = "Code")]
        public string Code
        {
            get { return _Code; }
            set { _Code = value; }
        }

        [XmlText(DataType = "string")]
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public void MakeSchemaCompliant()
        {
        }
    }
}