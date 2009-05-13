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
    [XmlRoot(Namespace = "urn:oasis:names:tc:ciq:xsdschema:xAL:2.0", IsNullable = false)]
    public class ThoroughfareNumber : xAlTypedElementBase
    {
        private string indicatorField;

        private ThoroughfareNumberIndicatorOccurrence indicatorOccurrenceField;

        private bool indicatorOccurrenceFieldSpecified;

        private ThoroughfareNumberNumberOccurrence numberOccurrenceField;

        private bool numberOccurrenceFieldSpecified;
        private ThoroughfareNumberNumberType numberTypeField;

        private bool numberTypeFieldSpecified;


        /// <remarks/>
        [XmlAttribute]
        public ThoroughfareNumberNumberType NumberType
        {
            get { return numberTypeField; }
            set { numberTypeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool NumberTypeSpecified
        {
            get { return numberTypeFieldSpecified; }
            set { numberTypeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public string Indicator
        {
            get { return indicatorField; }
            set { indicatorField = value; }
        }

        /// <remarks/>
        [XmlAttribute]
        public ThoroughfareNumberIndicatorOccurrence IndicatorOccurrence
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
        public ThoroughfareNumberNumberOccurrence NumberOccurrence
        {
            get { return numberOccurrenceField; }
            set { numberOccurrenceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool NumberOccurrenceSpecified
        {
            get { return numberOccurrenceFieldSpecified; }
            set { numberOccurrenceFieldSpecified = value; }
        }
    }
}