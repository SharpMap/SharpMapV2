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

namespace SharpMap.Entities.Iso.Gts
{
    [Serializable, XmlType(TypeName = "TM_periodDuration_PropertyType", Namespace = "http://www.isotc211.org/2005/gts")]
    public class TM_periodDuration_PropertyType
    {
        [XmlIgnore] private string _nilReason;
        [XmlIgnore] private string _tM_PeriodDuration;

        public TM_periodDuration_PropertyType()
        {
            TM_periodDuration = string.Empty;
        }

        [XmlAttribute(AttributeName = "nilReason", DataType = "anyURI")]
        public string NilReason
        {
            get { return _nilReason; }
            set { _nilReason = value; }
        }

        [XmlElement(ElementName = "TM_periodDuration", IsNullable = false, Form = XmlSchemaForm.Qualified,
            DataType = "duration", Namespace = "http://www.isotc211.org/2005/gts")]
        public string TM_periodDuration
        {
            get { return _tM_PeriodDuration; }
            set { _tM_PeriodDuration = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}