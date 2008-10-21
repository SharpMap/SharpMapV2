Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Layers.Wms');

SharpMap.Presentation.Web.SharpLayers.Layers.Wms.WmsLayerComponent = function(element) {
    SharpMap.Presentation.Web.SharpLayers.Layers.Wms.WmsLayerComponent.initializeBase(this, [element]);

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
