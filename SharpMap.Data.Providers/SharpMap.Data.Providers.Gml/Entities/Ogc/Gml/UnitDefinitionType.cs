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
    [Serializable, XmlType(TypeName = "UnitDefinitionType", Namespace = Declarations.SchemaVersion)]
    public class UnitDefinitionType : DefinitionType
    {
        [XmlIgnore] private CatalogSymbol _catalogSymbol;
        [XmlIgnore] private QuantityType _quantityType;
        [XmlIgnore] private QuantityTypeReference _quantityTypeReference;

        [XmlElement(Type = typeof (CatalogSymbol), ElementName = "catalogSymbol", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public CatalogSymbol CatalogSymbol
        {
            get { return _catalogSymbol; }
            set { _catalogSymbol = value; }
        }

        [XmlElement(Type = typeof (QuantityType), ElementName = "quantityType", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public QuantityType QuantityType
        {
            get { return _quantityType; }
            set { _quantityType = value; }
        }

        [XmlElement(Type = typeof (QuantityTypeReference), ElementName = "quantityTypeReference", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public QuantityTypeReference QuantityTypeReference
        {
            get { return _quantityTypeReference; }
            set { _quantityTypeReference = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}