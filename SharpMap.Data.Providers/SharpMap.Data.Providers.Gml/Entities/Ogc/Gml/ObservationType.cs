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
    [Serializable, XmlType(TypeName = "ObservationType", Namespace = Declarations.SchemaVersion)]
    public class ObservationType : AbstractFeatureType
    {
        [XmlIgnore] private ResultOf _resultOf;
        [XmlIgnore] private Target _target;
        [XmlIgnore] private Using _using;
        [XmlIgnore] private ValidTime _validTime;

        [XmlElement(Type = typeof (ResultOf), ElementName = "resultOf", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ResultOf ResultOf
        {
            get { return _resultOf; }
            set { _resultOf = value; }
        }

        [XmlElement(Type = typeof (Target), ElementName = "target", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Target Target
        {
            get { return _target; }
            set { _target = value; }
        }

        [XmlElement(Type = typeof (Using), ElementName = "using", IsNullable = false, Form = XmlSchemaForm.Qualified,
            Namespace = Declarations.SchemaVersion)]
        public Using Using
        {
            get
            {
                if (_using == null)
                {
                    _using = new Using();
                }
                return _using;
            }
            set { _using = value; }
        }

        [XmlElement(Type = typeof (ValidTime), ElementName = "validTime", IsNullable = false,
            Form = XmlSchemaForm.Qualified, Namespace = Declarations.SchemaVersion)]
        public ValidTime ValidTime
        {
            get { return _validTime; }
            set { _validTime = value; }
        }

        public override void MakeSchemaCompliant()
        {
            base.MakeSchemaCompliant();
            ValidTime.MakeSchemaCompliant();
            ResultOf.MakeSchemaCompliant();
        }
    }
}