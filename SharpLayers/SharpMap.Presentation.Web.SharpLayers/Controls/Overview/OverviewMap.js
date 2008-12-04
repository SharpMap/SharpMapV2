
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Controls.Overview');

SharpMap.Presentation.Web.SharpLayers.Controls.Overview.OverviewMap = function() {
    SharpMap.Presentation.Web.SharpLayers.Controls.Overview.OverviewMap.initializeBase(this);

}
SharpMap.Presentation.Web.SharpLayers.Controls.Overview.OverviewMap.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.Overview.OverviewMap.callBaseMethod(this, 'initialize');
    },

    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.Overview.OverviewMap.callBaseMethod(this, 'dispose');
    },
    _toolBuilderDelegate: function() {
        var options = this.get_builderParams();
        options.div = typeof options.div == "string" ? $get(options.div) : options.div;
        var mapHost = this.get_targetMapHost();
        options.mapOptions = mapHost.get_builderParams();
        return new OpenLayers.Control.OverviewMap(options);
    }
}
SharpMap.Presentation.Web.SharpLayers.Controls.Overview.OverviewMap.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.Overview.OverviewMap', SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent);
