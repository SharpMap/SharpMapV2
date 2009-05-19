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
using SharpMap.Entities.Iso.Gco;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable, XmlType(TypeName = "MD_digitalTransferOptions_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_digitalTransferOptions_Type : AbstractObjectType
    {
        [XmlIgnore] private MD_medium_PropertyType _offLine;
        [XmlIgnore] private List<CI_onlineResource_PropertyType> _onLine;
        [XmlIgnore] private RealPropertyType _transferSize;
        [XmlIgnore] private CharacterStringPropertyType _unitsOfDistribution;

        [XmlElement(Type = typeof (MD_medium_PropertyType), ElementName = "offLine", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public MD_medium_PropertyType OffLine
        {
            get { return _offLine; }
            set { _offLine = value; }
        }

        [XmlElement(Type = typeof (CI_onlineResource_PropertyType), ElementName = "onLine", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<CI_onlineResource_PropertyType> OnLine
        {
            get
            {
                if (_onLine == null)
                {
                    _onLine = new List<CI_onlineResource_PropertyType>();
                }
                return _onLine;
            }
            set { _onLine = value; }
        }

        [XmlElement(Type = typeof (RealPropertyType), ElementName = "transferSize", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public RealPropertyType TransferSize
        {
            get { return _transferSize; }
            set { _transferSize = value; }
        }

        [XmlElement(Type = typeof (CharacterStringPropertyType), ElementName = "unitsOfDistribution", IsNullable = false
            , Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CharacterStringPropertyType UnitsOfDistribution
        {
            get { return _unitsOfDistribution; }
            set { _unitsOfDistribution = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}