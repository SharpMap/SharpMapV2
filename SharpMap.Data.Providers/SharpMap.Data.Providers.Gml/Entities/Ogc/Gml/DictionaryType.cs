// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Gml
//  *  SharpMap.Data.Providers.Gml is free software © 2008 Newgrove Consultants Limited, 
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

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "DictionaryType", Namespace = "http://www.opengis.net/gml/3.2")]
    public class DictionaryType : DefinitionType
    {
        [XmlIgnore] private AggregationType _aggregationType;
        [XmlIgnore] private List<DictionaryEntry> _dictionaryEntry;
        [XmlIgnore] private List<IndirectEntry> _indirectEntry;
        [XmlIgnore] public bool AggregationTypeSpecified;

        [XmlAttribute(AttributeName = "aggregationType")]
        public AggregationType AggregationType
        {
            get { return _aggregationType; }
            set
            {
                _aggregationType = value;
                AggregationTypeSpecified = true;
            }
        }

        [XmlElement(Type = typeof (DictionaryEntry), ElementName = "dictionaryEntry", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<DictionaryEntry> DictionaryEntry
        {
            get
            {
                if (_dictionaryEntry == null)
                {
                    _dictionaryEntry = new List<DictionaryEntry>();
                }
                return _dictionaryEntry;
            }
            set { _dictionaryEntry = value; }
        }

        [XmlElement(Type = typeof (IndirectEntry), ElementName = "indirectEntry", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public List<IndirectEntry> IndirectEntry
        {
            get
            {
                if (_indirectEntry == null)
                {
                    _indirectEntry = new List<IndirectEntry>();
                }
                return _indirectEntry;
            }
            set { _indirectEntry = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            foreach (DictionaryEntry _c in DictionaryEntry)
            {
                _c.MakeSchemaCompliant();
            }
            foreach (IndirectEntry _c in IndirectEntry)
            {
                _c.MakeSchemaCompliant();
            }
        }
    }
}