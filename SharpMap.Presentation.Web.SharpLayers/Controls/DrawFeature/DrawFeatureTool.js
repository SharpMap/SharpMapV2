Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Controls.DrawFeature');

SharpMap.Presentation.Web.SharpLayers.Controls.DrawFeature.DrawFeatureTool = function(element) {
    SharpMap.Presentation.Web.SharpLayers.Controls.DrawFeature.DrawFeatureTool.initializeBase(this);

}
SharpMap.Presentation.Web.SharpLayers.Controls.DrawFeature.DrawFeatureTool.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.DrawFeature.DrawFeatureTool.callBaseMethod(this, 'initialize');

    },

    dispose: function() {
        // TODO: Add your cleanup code here

        SharpMap.Presentation.Web.SharpLayers.Controls.DrawFeature.DrawFeatureTool.callBaseMethod(this, 'dispose');
    },
    _toolBuilderDelegate: function() {
        var options = this.get_builderParams();
        options.div = typeof options.div == "string" ? $get(options.div) : options.div;
        return new OpenLayers.Control.DrawFeature(options);
    }
}
SharpMap.Presentation.Web.SharpLayers.Controls.DrawFeature.DrawFeatureTool.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.DrawFeature.DrawFeatureTool', SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent);
