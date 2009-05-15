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

namespace SharpMap.Entities.xAL
{
    [XmlType(TypeName = "PremiseNumberRangeFrom", Namespace = Declarations.SchemaVersion), Serializable]
    public class PremiseNumberRangeFrom
    {
        [XmlIgnore] private List<AddressLine> _AddressLine;
        [XmlIgnore] private List<PremiseNumber> _PremiseNumber;

        [XmlIgnore] private List<PremiseNumberPrefix> _PremiseNumberPrefix;
        [XmlIgnore] private List<PremiseNumberSuffix> _PremiseNumberSuffix;

        [XmlElement(Type = typeof (AddressLine), ElementName = "AddressLine", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<AddressLine> AddressLine
        {
            get
            {
                if (_AddressLine == null) _AddressLine = new List<AddressLine>();
                return _AddressLine;
            }
            set { _AddressLine = value; }
        }

        [XmlElement(Type = typeof (PremiseNumberPrefix), ElementName = "PremiseNumberPrefix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PremiseNumberPrefix> PremiseNumberPrefix
        {
            get
            {
                if (_PremiseNumberPrefix == null) _PremiseNumberPrefix = new List<PremiseNumberPrefix>();
                return _PremiseNumberPrefix;
            }
            set { _PremiseNumberPrefix = value; }
        }

        [XmlElement(Type = typeof (PremiseNumber), ElementName = "PremiseNumber", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PremiseNumber> PremiseNumber
        {
            get
            {
                if (_PremiseNumber == null) _PremiseNumber = new List<PremiseNumber>();
                return _PremiseNumber;
            }
            set { _PremiseNumber = value; }
        }

        [XmlElement(Type = typeof (PremiseNumberSuffix), ElementName = "PremiseNumberSuffix", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<PremiseNumberSuffix> PremiseNumberSuffix
        {
            get
            {
                if (_PremiseNumberSuffix == null) _PremiseNumberSuffix = new List<PremiseNumberSuffix>();
                return _PremiseNumberSuffix;
            }
            set { _PremiseNumberSuffix = value; }
        }

        public void MakeSchemaCompliant()
        {
            foreach (PremiseNumber _c in PremiseNumber) _c.MakeSchemaCompliant();
        }
    }
}