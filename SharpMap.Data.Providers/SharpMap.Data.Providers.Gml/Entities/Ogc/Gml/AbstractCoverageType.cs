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
    [Serializable, XmlType(TypeName = "AbstractCoverageType", Namespace = "http://www.opengis.net/gml/3.2")]
    public abstract class AbstractCoverageType : AbstractFeatureType
    {
        [XmlIgnore] private DomainSet _domainSet;
        [XmlIgnore] private RangeSet _rangeSet;

        [XmlElement(Type = typeof (DomainSet), ElementName = "domainSet", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public DomainSet DomainSet
        {
            get { return _domainSet; }
            set { _domainSet = value; }
        }

        [XmlElement(Type = typeof (RangeSet), ElementName = "rangeSet", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.opengis.net/gml/3.2")]
        public RangeSet RangeSet
        {
            get { return _rangeSet; }
            set { _rangeSet = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            DomainSet.MakeSchemaCompliant();
            RangeSet.MakeSchemaCompliant();
        }
    }
}