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
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "TemporalCRSType", Namespace = Declarations.SchemaVersion)]
    public class TemporalCRSType : AbstractCRSType
    {
        [XmlIgnore] private TemporalDatumProperty _temporalDatum;
        [XmlIgnore] private TimeCSProperty _timeCS;
        [XmlIgnore] private UsesTemporalCS _usesTemporalCS;

        [XmlElement(Type = typeof (TemporalDatumProperty), ElementName = "temporalDatum", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public TemporalDatumProperty TemporalDatum
        {
            get { return _temporalDatum; }
            set { _temporalDatum = value; }
        }

        [XmlElement(Type = typeof (TimeCSProperty), ElementName = "timeCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public TimeCSProperty TimeCS
        {
            get { return _timeCS; }
            set { _timeCS = value; }
        }

        [XmlElement(Type = typeof (UsesTemporalCS), ElementName = "usesTemporalCS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public UsesTemporalCS UsesTemporalCS
        {
            get { return _usesTemporalCS; }
            set { _usesTemporalCS = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            TimeCS.MakeSchemaCompliant();
            UsesTemporalCS.MakeSchemaCompliant();
            TemporalDatum.MakeSchemaCompliant();
        }
    }
}