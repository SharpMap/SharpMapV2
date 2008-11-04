Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Strategy');

SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent = function() {
    this._builderDelegate = Function.createDelegate(this, this._strategyBuilderDelegate);

    SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent.initializeBase(this);
}

SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent.prototype = {

    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent.callBaseMethod(this, 'initialize');
    },
    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent.callBaseMethod(this, 'dispose');
    },
    buildObject: function() {
        SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent.callBaseMethod(this, 'buildObject');
    }
}

SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent', SharpMap.Presentation.Web.SharpLayers.ComponentBase);