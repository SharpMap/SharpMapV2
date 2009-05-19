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
    [Serializable, XmlType(TypeName = "MD_distribution_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_distribution_Type : AbstractObjectType
    {
        [XmlIgnore] private List<MD_format_PropertyType> _distributionFormat;
        [XmlIgnore] private List<MD_distributor_PropertyType> _distributor;
        [XmlIgnore] private List<MD_digitalTransferOptions_PropertyType> _transferOptions;

        [XmlElement(Type = typeof (MD_format_PropertyType), ElementName = "distributionFormat", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_format_PropertyType> DistributionFormat
        {
            get
            {
                if (_distributionFormat == null)
                {
                    _distributionFormat = new List<MD_format_PropertyType>();
                }
                return _distributionFormat;
            }
            set { _distributionFormat = value; }
        }

        [XmlElement(Type = typeof (MD_distributor_PropertyType), ElementName = "distributor", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_distributor_PropertyType> Distributor
        {
            get
            {
                if (_distributor == null)
                {
                    _distributor = new List<MD_distributor_PropertyType>();
                }
                return _distributor;
            }
            set { _distributor = value; }
        }

        [XmlElement(Type = typeof (MD_digitalTransferOptions_PropertyType), ElementName = "transferOptions",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_digitalTransferOptions_PropertyType> TransferOptions
        {
            get
            {
                if (_transferOptions == null)
                {
                    _transferOptions = new List<MD_digitalTransferOptions_PropertyType>();
                }
                return _transferOptions;
            }
            set { _transferOptions = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}