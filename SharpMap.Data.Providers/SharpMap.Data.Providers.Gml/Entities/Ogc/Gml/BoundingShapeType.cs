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
    [Serializable, XmlType(TypeName = "BoundingShapeType", Namespace = Declarations.SchemaVersion)]
    public class BoundingShapeType
    {
        [XmlIgnore] private Envelope _envelope;
        [XmlIgnore] private string _nilReason;
        [XmlIgnore] private string _null;

        public BoundingShapeType()
        {
            Null = string.Empty;
        }

        [XmlElement(Type = typeof (Envelope), ElementName = "Envelope", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public Envelope Envelope
        {
            get { return _envelope; }
            set { _envelope = value; }
        }

        [XmlAttribute(AttributeName = "nilReason", DataType = "anyURI")]
        public string NilReason
        {
            get { return _nilReason; }
            set { _nilReason = value; }
        }

        [XmlElement(ElementName = "Null", IsNullable = false, Form = XmlSchemaForm.Qualified, DataType = "anyURI",
            Namespace = Declarations.SchemaVersion)]
        public string Null
        {
            get { return _null; }
            set { _null = value; }
        }

        public virtual void MakeSchemaCompliant()
        {
            Envelope.MakeSchemaCompliant();
        }
    }
}