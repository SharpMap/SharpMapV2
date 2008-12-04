Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Controls.Nav');

SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar = function() {
    SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar.initializeBase(this);
}
SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar.callBaseMethod(this, 'initialize');
    },

    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar.callBaseMethod(this, 'dispose');

    },
    _toolBuilderDelegate: function() {
        var options = this.get_builderParams();
        if (options["div"] && typeof options["div"] == "string")
            options.div = $get(options.div);
        return new OpenLayers.Control.NavToolbar(options);
    }
}
SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar', SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent);
