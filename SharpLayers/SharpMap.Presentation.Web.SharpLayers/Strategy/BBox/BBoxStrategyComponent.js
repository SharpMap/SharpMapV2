Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Strategy.BBox');

SharpMap.Presentation.Web.SharpLayers.Strategy.BBox.BBoxStrategyComponent = function() {
    SharpMap.Presentation.Web.SharpLayers.Strategy.BBox.BBoxStrategyComponent.initializeBase(this);
}

SharpMap.Presentation.Web.SharpLayers.Strategy.BBox.BBoxStrategyComponent.prototype = {

    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Strategy.BBox.BBoxStrategyComponent.callBaseMethod(this, 'initialize');
    },
    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Strategy.BBox.BBoxStrategyComponent.callBaseMethod(this, 'dispose');
    }
}

SharpMap.Presentation.Web.SharpLayers.Strategy.BBox.BBoxStrategyComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Strategy.BBox.BBoxStrategyComponent', SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent);