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

namespace SharpMap.Presentation.Web.SharpLayers.Layers.Wms
{
    public class WmsLayerBuilderParams : LayerBuilderParamsBase, IGridBasedLayerBuilderParams
    {
        [ExtenderControlProperty]
        [ClientPropertyName("params")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public WmsParameters WmsParameters { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("projection")]
        public string Projection { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("maxResolution")]
        public string MaxResolution { get; set; }

        #region IGridBasedLayerBuilderParams Members

        [ExtenderControlProperty(true, true)]
        [ClientPropertyName("tileSize")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
         PersistenceMode(PersistenceMode.InnerProperty)]
        public Size TileSize { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("transitionEffect")]
        public TransitionEffects TransitionEffect { get; set; }

        [ExtenderControlProperty]
        [ClientPropertyName("buffer")]
        public int Buffer { get; set; }

        #endregion
    }
}