/*
*  The attached / following is part of SharpMap.Presentation.Web.SharpLayers
*  SharpMap.Presentation.Web.SharpLayers is free software � 2008 Newgrove Consultants Limited, 
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
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Layers.Wms');

SharpMap.Presentation.Web.SharpLayers.Layers.Wms.WmsLayerComponent = function() {
    SharpMap.Presentation.Web.SharpLayers.Layers.Wms.WmsLayerComponent.initializeBase(this);

}
SharpMap.Presentation.Web.SharpLayers.Layers.Wms.WmsLayerComponent.prototype = {
    initialize: function() {
        this._builderDelegate = Function.createDelegate(this, this._layerBuilderDelegate);
        SharpMap.Presentation.Web.SharpLayers.Layers.Wms.WmsLayerComponent.callBaseMethod(this, 'initialize');
    },

    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Layers.Wms.WmsLayerComponent.callBaseMethod(this, 'dispose');
    },
    _layerBuilderDelegate: function() {
        var options = this.get_builderParams();
        var url = options.params.url;
        delete options.params.url;
        var params = options.params;
        delete options.params;
        return new OpenLayers.Layer.WMS(this.get_name(), url, params, options);
    }


}
SharpMap.Presentation.Web.SharpLayers.Layers.Wms.WmsLayerComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Layers.Wms.WmsLayerComponent', SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent);
