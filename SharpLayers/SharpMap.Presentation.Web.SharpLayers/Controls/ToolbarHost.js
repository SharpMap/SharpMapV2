
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Controls');

SharpMap.Presentation.Web.SharpLayers.Controls.ToolbarHost = function(element) {
    SharpMap.Presentation.Web.SharpLayers.Controls.ToolbarHost.initializeBase(this, [element]);
    this._targetMap = null;

}
SharpMap.Presentation.Web.SharpLayers.Controls.ToolbarHost.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.ToolbarHost.callBaseMethod(this, 'initialize');
        this._builderDelegate = Function.createDelegate(this, this._toolbarBuilderDelegate);

    },

    dispose: function() {
        // TODO: Add your cleanup code here

        SharpMap.Presentation.Web.SharpLayers.Controls.ToolbarHost.callBaseMethod(this, 'dispose');
    },
    _toolbarBuilderDelegate: function() {
        var options = this.get_builderParams();
        options.div = this.get_element();

        return new OpenLayers.Control.Panel(options);
    },
    get_targetMap: function() {
        return this._targetMapId;
    },
    set_targetMap: function(val) {
        this._targetMapId = val;
    },
    buildObject: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.ToolbarHost.callBaseMethod(this, 'buildObject')
        this.get_hostedItem().map = this.get_targetMap().get_hostedItem();
    }
}
SharpMap.Presentation.Web.SharpLayers.Controls.ToolbarHost.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.ToolbarHost', SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior);
