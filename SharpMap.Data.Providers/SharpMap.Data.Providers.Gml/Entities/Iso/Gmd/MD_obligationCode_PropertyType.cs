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

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "MD_obligationCode_PropertyType", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_obligationCode_PropertyType
    {
        [XmlIgnore] private MD_obligationCode_Type _mD_ObligationCode;
        [XmlIgnore] private string _nilReason;
        [XmlIgnore] public bool MD_ObligationCodeSpecified = true;

        [XmlElement(ElementName = "MD_obligationCode", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_obligationCode_Type MD_obligationCode
        {
            get { return _mD_ObligationCode; }
            set
            {
                _mD_ObligationCode = value;
                MD_ObligationCodeSpecified = true;
            }
        }

        [XmlAttribute(AttributeName = "nilReason", DataType = "anyURI")]
        public string NilReason
        {
            get { return _nilReason; }
            set { _nilReason = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}