Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Layers');

SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent = function() {
    SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent.initializeBase(this);

    this._targetMapHost = null;
    this._name = null;

}
SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent.prototype = {
    initialize: function() {
        this._builderDelegate = Function.createDelegate(this, this._layerBuilderDelegate);
        SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent.callBaseMethod(this, 'initialize');
    },
    dispose: function() {
        delete this._builderDelegate;
        delete this._layerBuilderDelegate;
        SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent.callBaseMethod(this, 'dispose');
    },
    get_name: function() {
        return this._name;
    },
    set_name: function(name) {
        this._name = name;
    },
    get_targetMapHost: function() {
        return this._targetMapHost;
    },
    set_targetMapHost: function(mapHost) {
        if (typeof mapHost == "string")
            mapHost = $find(mapHost);
        this._targetMapHost = mapHost;
    },
    buildObject: function() {
        SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent.callBaseMethod(this, 'buildObject');
        //add the layer to the map
        var mapHost = this.get_targetMapHost();
        mapHost.get_hostedItem().addLayer(this.get_hostedItem());
    }
}
SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent', SharpMap.Presentation.Web.SharpLayers.ComponentBase);
