Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Strategy');

SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent = function() {
    SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent.initializeBase(this);
}

SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent.prototype = {

    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent.callBaseMethod(this, 'initialize');
    },
    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent.callBaseMethod(this, 'dispose');
    }
}

SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent', SharpMap.Presentation.Web.SharpLayers.ComponentBase);