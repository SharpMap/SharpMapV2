
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Controls.LayerSwitcher');

SharpMap.Presentation.Web.SharpLayers.Controls.LayerSwitcher.LayerSwitcherTool = function(element) {
    SharpMap.Presentation.Web.SharpLayers.Controls.LayerSwitcher.LayerSwitcherTool.initializeBase(this, [element]);
}
SharpMap.Presentation.Web.SharpLayers.Controls.LayerSwitcher.LayerSwitcherTool.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.LayerSwitcher.LayerSwitcherTool.callBaseMethod(this, 'initialize');

        // TODO: Add your initalization code here
    },

    dispose: function() {
        // TODO: Add your cleanup code here

        SharpMap.Presentation.Web.SharpLayers.Controls.LayerSwitcher.LayerSwitcherTool.callBaseMethod(this, 'dispose');
    },
    _toolBuilderDelegate: function() {
        var options = this.get_builderParams();
        if (options["div"] && typeof options["div"] == "string")
            options.div = $get(options.div);
        return new OpenLayers.Control.LayerSwitcher(options);
    }

}
SharpMap.Presentation.Web.SharpLayers.Controls.LayerSwitcher.LayerSwitcherTool.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.LayerSwitcher.LayerSwitcherTool', SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent);
