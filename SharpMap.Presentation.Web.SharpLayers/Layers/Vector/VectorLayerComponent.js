
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Layers.Vector');

SharpMap.Presentation.Web.SharpLayers.Layers.Vector.VectorLayerComponent = function() {
    SharpMap.Presentation.Web.SharpLayers.Layers.Vector.VectorLayerComponent.initializeBase(this);


}
SharpMap.Presentation.Web.SharpLayers.Layers.Vector.VectorLayerComponent.prototype = {
    initialize: function() {
        this._builderDelegate = Function.createDelegate(this, this._layerBuilderDelegate);
        SharpMap.Presentation.Web.SharpLayers.Layers.Vector.VectorLayerComponent.callBaseMethod(this, 'initialize');
    },

    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Layers.Vector.VectorLayerComponent.callBaseMethod(this, 'dispose');
    },
    _layerBuilderDelegate: function() {
        var options = this.get_builderParams();
        return new OpenLayers.Layer.Vector(this.get_name(), options);
    }

}
SharpMap.Presentation.Web.SharpLayers.Layers.Vector.VectorLayerComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Layers.Vector.VectorLayerComponent', SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent);
