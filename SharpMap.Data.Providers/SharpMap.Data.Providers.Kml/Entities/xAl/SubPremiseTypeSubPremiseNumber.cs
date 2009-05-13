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
using System.Xml.Serialization;

namespace SharpMap.Entities.xAl
{
    /// <remarks/>
    [Serializable]
    [XmlType(AnonymousType = true, Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0")]
    public class SubPremiseTypeSubPremiseNumber : xAlTypedElementBase
    {
        private string indicatorField;

        private SubPremiseTypeSubPremiseNumberIndicatorOccurrence indicatorOccurrenceField;

        private bool indicatorOccurrenceFieldSpecified;

        private SubPremiseTypeSubPremiseNumberNumberTypeOccurrence numberTypeOccurrenceField;

        private bool numberTypeOccurrenceFieldSpecified;

        private string premiseNumberSeparatorField;

        /// <remarks/>
        [XmlAttribute]
        public string Indicator
        {
            get { return indicatorField; }
            set { indicatorField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public SubPremiseTypeSubPremiseNumberIndicatorOccurrence IndicatorOccurrence
        {
            get { return indicatorOccurrenceField; }
            set { indicatorOccurrenceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool IndicatorOccurrenceSpecified
        {
            get { return indicatorOccurrenceFieldSpecified; }
            set { indicatorOccurrenceFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public SubPremiseTypeSubPremiseNumberNumberTypeOccurrence NumberTypeOccurrence
        {
            get { return numberTypeOccurrenceField; }
            set { numberTypeOccurrenceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool NumberTypeOccurrenceSpecified
        {
            get { return numberTypeOccurrenceFieldSpecified; }
            set { numberTypeOccurrenceFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string PremiseNumberSeparator
        {
            get { return premiseNumberSeparatorField; }
            set { premiseNumberSeparatorField = value; }
        }
    }
}