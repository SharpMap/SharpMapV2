
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Controls.Scale');

SharpMap.Presentation.Web.SharpLayers.Controls.Scale.ScaleBar = function() {
    SharpMap.Presentation.Web.SharpLayers.Controls.Scale.ScaleBar.initializeBase(this);

}
SharpMap.Presentation.Web.SharpLayers.Controls.Scale.ScaleBar.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.Scale.ScaleBar.callBaseMethod(this, 'initialize');
    },

    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.Scale.ScaleBar.callBaseMethod(this, 'dispose');
    },
    _toolBuilderDelegate: function() {
        var options = this.get_builderParams();
        options.div = typeof options.div == "string" ? $get(options.div) : options.div;


        return new OpenLayers.Control.ScaleLine(options);
    }
}
SharpMap.Presentation.Web.SharpLayers.Controls.Scale.ScaleBar.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.Scale.ScaleBar', SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent);
