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

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "MeasureType", Namespace = Declarations.SchemaVersion)]
    public class MeasureType
    {
        [XmlIgnore] private string _uom;
        [XmlIgnore] private double _value;
        [XmlIgnore] public bool ValueSpecified;

        [XmlAttribute(AttributeName = "uom")]
        public string Uom
        {
            get { return _uom; }
            set { _uom = value; }
        }

        [XmlText(typeof (double))]
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                ValueSpecified = true;
            }
        }

        public virtual void MakeSchemaCompliant()
        {
        }
    }
}