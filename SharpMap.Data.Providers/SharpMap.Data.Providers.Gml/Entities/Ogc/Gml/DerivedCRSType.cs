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
    [Serializable, XmlType(TypeName = "DerivedCRSType", Namespace = Declarations.SchemaVersion)]
    public class DerivedCRSType : AbstractGeneralDerivedCRSType
    {
        [XmlIgnore] private BaseCRSProperty _baseCRS;
        [XmlIgnore] private CoordinateSystemProperty _coordinateSystem;
        [XmlIgnore] private DerivedCRSType_E _derivedCRSType;

        [XmlElement(Type = typeof (BaseCRSProperty), ElementName = "baseCRS", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public BaseCRSProperty BaseCRS
        {
            get { return _baseCRS; }
            set { _baseCRS = value; }
        }

        [XmlElement(Type = typeof (CoordinateSystemProperty), ElementName = "coordinateSystem", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public CoordinateSystemProperty CoordinateSystem
        {
            get { return _coordinateSystem; }
            set { _coordinateSystem = value; }
        }

        [XmlElement(Type = typeof (DerivedCRSType_E), ElementName = "derivedCRSType", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public DerivedCRSType_E DerivedCRSType_child
        {
            get { return _derivedCRSType; }
            set { _derivedCRSType = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            BaseCRS.MakeSchemaCompliant();
            DerivedCRSType_child.MakeSchemaCompliant();
            CoordinateSystem.MakeSchemaCompliant();
        }
    }
}