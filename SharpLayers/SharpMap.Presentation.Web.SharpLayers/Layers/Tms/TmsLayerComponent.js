Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Layers.Tms');

SharpMap.Presentation.Web.SharpLayers.Layers.Tms.TmsLayerComponent = function() {
    SharpMap.Presentation.Web.SharpLayers.Layers.Tms.TmsLayerComponent.initializeBase(this);


}
SharpMap.Presentation.Web.SharpLayers.Layers.Tms.TmsLayerComponent.prototype = {
    initialize: function() {
        this._builderDelegate = Function.createDelegate(this, this._layerBuilderDelegate);
        SharpMap.Presentation.Web.SharpLayers.Layers.Tms.TmsLayerComponent.callBaseMethod(this, 'initialize');
    },
    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Layers.Tms.TmsLayerComponent.callBaseMethod(this, 'dispose');
    },
    _layerBuilderDelegate: function() {
        var options = this.get_builderParams();
        var url = options.url;
        delete options.url;
        return new OpenLayers.Layer.TMS(this.get_name(), url, options);
    }
}
SharpMap.Presentation.Web.SharpLayers.Layers.Tms.TmsLayerComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Layers.Tms.TmsLayerComponent', SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent);
