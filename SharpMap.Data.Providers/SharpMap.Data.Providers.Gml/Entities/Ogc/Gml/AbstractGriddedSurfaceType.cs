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
    [Serializable, XmlType(TypeName = "AbstractGriddedSurfaceType", Namespace = Declarations.SchemaVersion)]
    public abstract class AbstractGriddedSurfaceType : AbstractParametricCurveSurfaceType
    {
        [XmlIgnore] private string _columns;
        [XmlIgnore] private string _rows;
        [XmlIgnore] private RowsCollection _rowsCollection;

        [XmlAttribute(AttributeName = "columns", DataType = "integer")]
        public string Columns
        {
            get { return _columns; }
            set { _columns = value; }
        }

        [XmlAttribute(AttributeName = "rows", DataType = "integer")]
        public string Rows
        {
            get { return _rows; }
            set { _rows = value; }
        }

        [XmlElement(Type = typeof (RowsCollection), ElementName = "rows", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public RowsCollection RowsCollection
        {
            get { return _rowsCollection; }
            set { _rowsCollection = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            _rowsCollection.MakeSchemaCompliant();
        }
    }
}