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
using System.Xml.Serialization;

namespace SharpMap.Entities.Iso.Gco
{
    [Serializable, XmlType(TypeName = "UnlimitedInteger_type", Namespace = "http://www.isotc211.org/2005/gco")]
    public class UnlimitedIntegerType
    {
        [XmlIgnore] private Boolean _isInfinite;
        [XmlIgnore] private string _value;
        [XmlIgnore] public bool IsInfiniteSpecified;

        [XmlAttribute(AttributeName = "isInfinite", DataType = "boolean")]
        public Boolean IsInfinite
        {
            get { return _isInfinite; }
            set
            {
                _isInfinite = value;
                IsInfiniteSpecified = true;
            }
        }

        [XmlText(DataType = "string")]
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}