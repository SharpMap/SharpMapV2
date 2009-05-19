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
    [Serializable, XmlType(TypeName = "MD_distributor_Type", Namespace = "http://www.isotc211.org/2005/gmd")]
    public class MD_distributor_Type : AbstractObjectType
    {
        [XmlIgnore] private List<MD_standardOrderProcess_PropertyType> _distributionOrderProcess;
        [XmlIgnore] private CI_responsibleParty_PropertyType _distributorContact;
        [XmlIgnore] private List<MD_format_PropertyType> _distributorFormat;
        [XmlIgnore] private List<MD_digitalTransferOptions_PropertyType> _distributorTransferOptions;

        [XmlElement(Type = typeof (MD_standardOrderProcess_PropertyType), ElementName = "distributionOrderProcess",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_standardOrderProcess_PropertyType> DistributionOrderProcess
        {
            get
            {
                if (_distributionOrderProcess == null)
                {
                    _distributionOrderProcess = new List<MD_standardOrderProcess_PropertyType>();
                }
                return _distributionOrderProcess;
            }
            set { _distributionOrderProcess = value; }
        }

        [XmlElement(Type = typeof (CI_responsibleParty_PropertyType), ElementName = "distributorContact",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public CI_responsibleParty_PropertyType DistributorContact
        {
            get { return _distributorContact; }
            set { _distributorContact = value; }
        }

        [XmlElement(Type = typeof (MD_format_PropertyType), ElementName = "distributorFormat", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_format_PropertyType> DistributorFormat
        {
            get
            {
                if (_distributorFormat == null)
                {
                    _distributorFormat = new List<MD_format_PropertyType>();
                }
                return _distributorFormat;
            }
            set { _distributorFormat = value; }
        }

        [XmlElement(Type = typeof (MD_digitalTransferOptions_PropertyType), ElementName = "distributorTransferOptions",
            IsNullable = false, Form = XmlSchemaForm.Qualified, Namespace = "http://www.isotc211.org/2005/gmd")]
        public List<MD_digitalTransferOptions_PropertyType> DistributorTransferOptions
        {
            get
            {
                if (_distributorTransferOptions == null)
                {
                    _distributorTransferOptions = new List<MD_digitalTransferOptions_PropertyType>();
                }
                return _distributorTransferOptions;
            }
            set { _distributorTransferOptions = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            DistributorContact.MakeSchemaCompliant();
        }
    }
}