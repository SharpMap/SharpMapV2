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
    [Serializable, XmlType(TypeName = "AbstractFeatureCollectionType", Namespace = Declarations.SchemaVersion)]
    public abstract class AbstractFeatureCollectionType : AbstractFeatureType
    {
        [XmlIgnore] private List<FeatureMember> _featureMember;
        [XmlIgnore] private FeatureMembers _featureMembers;

        [XmlElement(Type = typeof (FeatureMember), ElementName = "featureMember", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public List<FeatureMember> FeatureMember
        {
            get
            {
                if (_featureMember == null)
                {
                    _featureMember = new List<FeatureMember>();
                }
                return _featureMember;
            }
            set { _featureMember = value; }
        }

        [XmlElement(Type = typeof (FeatureMembers), ElementName = "featureMembers", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public FeatureMembers FeatureMembers
        {
            get { return _featureMembers; }
            set { _featureMembers = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
        }
    }
}