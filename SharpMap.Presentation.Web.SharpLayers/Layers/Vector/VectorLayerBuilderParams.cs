/*
 *  The attached / following is part of SharpMap.Presentation.Web.SharpLayers
 *  SharpMap.Presentation.Web.SharpLayers is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit;
using SharpMap.Presentation.Web.SharpLayers.Strategy;

namespace SharpMap.Presentation.Web.SharpLayers.Layers.Vector
{
    public class VectorLayerBuilderParams : LayerBuilderParamsBase
    {
        private readonly CollectionBase<IStrategy> _strategies =
            new CollectionBase<IStrategy>(
                delegate { return false; });

        private string sld;

        [ExtenderControlProperty]
        [ClientPropertyName("geometryType")]
        public VectorGeometryType? LimitToGeometryType { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("strategies")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public CollectionBase<IStrategy> Strategies
        {
            get { return _strategies; }
        }

        [ExtenderControlProperty]
        [ComponentReference]
        [ClientPropertyName("protocol")]
        public string Protocol { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("sld")]
        [ComponentReference]
        public string Sld
        {
            get { return sld; }
            set { sld = value; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("styleSelector")]
        public string StyleSelector { get; set; }
    }
}