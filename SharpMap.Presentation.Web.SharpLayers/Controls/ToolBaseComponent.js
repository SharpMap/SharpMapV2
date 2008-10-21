Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Controls');

SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent = function() {
    SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.initializeBase(this);
    this._targetMapHost = null;

}
SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.prototype = {
    initialize: function() {
        this._builderDelegate = Function.createDelegate(this, this._toolBuilderDelegate);
        SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.callBaseMethod(this, 'initialize');
    },

    dispose: function() {

        SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.callBaseMethod(this, 'dispose');
    },
    get_targetMapHost: function() {
        return this._targetMapHost;
    },
    set_targetMapHost: function(val) {
        if (typeof val == "string")
            val = $find(val);
        this._targetMapHost = val;
    },
    buildObject: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.callBaseMethod(this, 'buildObject');
        //add the control to the map
        var mapHost = this.get_targetMapHost();
        mapHost.get_hostedItem().addControl(this.get_hostedItem());
    }
}
SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent', SharpMap.Presentation.Web.SharpLayers.ComponentBase);
