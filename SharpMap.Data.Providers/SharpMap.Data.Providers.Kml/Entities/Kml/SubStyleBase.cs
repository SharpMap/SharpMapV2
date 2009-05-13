// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Kml
//  *  SharpMap.Data.Providers.Kml is free software © 2008 Newgrove Consultants Limited, 
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
using System.Xml.Serialization;

namespace SharpMap.Entities.Ogc.Kml
{
    /// <remarks/>
    [XmlInclude(typeof (ListStyle))]
    [XmlInclude(typeof (BalloonStyle))]
    [XmlInclude(typeof (ColorStyleBase))]
    [XmlInclude(typeof (PolyStyle))]
    [XmlInclude(typeof (LineStyle))]
    [XmlInclude(typeof (LabelStyle))]
    [XmlInclude(typeof (IconStyle))]
    [Serializable]
    [XmlType(TypeName = "AbstractSubStyleType", Namespace = "http://www.opengis.net/kml/2.2")]
    public abstract class SubStyleBase : KmlObjectBase
    {
        private KmlObjectBase[] _kmlSubStyleObjectExtensionGroupField;
        private string[] abstractSubStyleSimpleExtensionGroupField;

        /// <remarks/>
        [XmlElement("AbstractSubStyleSimpleExtensionGroup")]
        public string[] AbstractSubStyleSimpleExtensionGroup
        {
            get { return abstractSubStyleSimpleExtensionGroupField; }
            set { abstractSubStyleSimpleExtensionGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("AbstractSubStyleObjectExtensionGroup")]
        public KmlObjectBase[] KmlSubStyleObjectExtensionGroup
        {
            get { return _kmlSubStyleObjectExtensionGroupField; }
            set { _kmlSubStyleObjectExtensionGroupField = value; }
        }
    }
}