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
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Gml
{
    [Serializable, XmlType(TypeName = "ConventionalUnitType", Namespace = Declarations.SchemaVersion)]
    public class ConventionalUnitType : UnitDefinitionType
    {
        [XmlIgnore] private ConversionToPreferredUnit _conversionToPreferredUnit;
        [XmlIgnore] private List<DerivationUnitTerm> _derivationUnitTerm;
        [XmlIgnore] private RoughConversionToPreferredUnit _roughConversionToPreferredUnit;

        [XmlElement(Type = typeof (ConversionToPreferredUnit), ElementName = "conversionToPreferredUnit",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ConversionToPreferredUnit ConversionToPreferredUnit
        {
            get { return _conversionToPreferredUnit; }
            set { _conversionToPreferredUnit = value; }
        }

        [XmlElement(Type = typeof (DerivationUnitTerm), ElementName = "derivationUnitTerm", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<DerivationUnitTerm> DerivationUnitTerm
        {
            get
            {
                if (_derivationUnitTerm == null)
                {
                    _derivationUnitTerm = new List<DerivationUnitTerm>();
                }
                return _derivationUnitTerm;
            }
            set { _derivationUnitTerm = value; }
        }

        [XmlElement(Type = typeof (RoughConversionToPreferredUnit), ElementName = "roughConversionToPreferredUnit",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public RoughConversionToPreferredUnit RoughConversionToPreferredUnit
        {
            get { return _roughConversionToPreferredUnit; }
            set { _roughConversionToPreferredUnit = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            ConversionToPreferredUnit.MakeSchemaCompliant();
            RoughConversionToPreferredUnit.MakeSchemaCompliant();
        }
    }
}