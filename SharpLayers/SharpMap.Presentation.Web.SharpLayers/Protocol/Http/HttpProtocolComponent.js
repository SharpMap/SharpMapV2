
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Protocol.Http');

SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent = function() {
    SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent.initializeBase(this);
}
SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent.prototype = {
    initialize: function() {
        this._builderDelegate = Function.createDelegate(this, this._protocolBuilderDelegate);
        SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent.callBaseMethod(this, 'initialize');
    },
    dispose: function() {
        delete this._protocolBuilderDelegate;
        SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent.callBaseMethod(this, 'dispose');
    },
    _protocolBuilderDelegate: function() {
        var options = this.get_builderParams();
        return new OpenLayers.Protocol.HTTP(options);
    }
}
SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent', SharpMap.Presentation.Web.SharpLayers.Protocol.ProtocolComponent);
