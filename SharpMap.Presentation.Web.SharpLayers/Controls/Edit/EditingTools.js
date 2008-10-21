Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Controls.Edit');

SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools = function(element) {
    SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools.initializeBase(this, [element]);

}
SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools.callBaseMethod(this, 'initialize');

        // TODO: Add your initalization code here
    },

    dispose: function() {
        // TODO: Add your cleanup code here

        SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools.callBaseMethod(this, 'dispose');
    },
    _toolBuilderDelegate: function() {
        var options = this.get_builderParams();
        options.div = typeof options.div == "string" ? $get(options.div) : options.div;

        var layer = options.layer;
        delete options.layer

        if (typeof layer == "string")
            layer = $find(layer).get_hostedItem();

        return new OpenLayers.Control.EditingToolbar(layer, options);
    }
}
SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools', SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent);
