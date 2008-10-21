Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Controls.SelectFeature');

SharpMap.Presentation.Web.SharpLayers.Controls.SelectFeature.SelectFeatureTool = function() {
    SharpMap.Presentation.Web.SharpLayers.Controls.SelectFeature.SelectFeatureTool.initializeBase(this);


}
SharpMap.Presentation.Web.SharpLayers.Controls.SelectFeature.SelectFeatureTool.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.SelectFeature.SelectFeatureTool.callBaseMethod(this, 'initialize');

    },

    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.SelectFeature.SelectFeatureTool.callBaseMethod(this, 'dispose');
    },
    _toolBuilderDelegate: function() {
        var params = this.get_builderParams();
        var layer = $find(params.layer).get_hostedItem();
        delete params.layer;

        ///hold your nose...
        ///todo: get the handlers sent correctly from the server
        if (params.onSelect != null)
            if (typeof params.onSelect == "string")
            params.onSelect = eval(params.onSelect);

        if (params.onUnselect != null)
            if (typeof params.onUnselect == "string")
            params.onUnselect = eval(params.onUnselect);
        return new OpenLayers.Control.SelectFeature(layer, params);

    }
}
SharpMap.Presentation.Web.SharpLayers.Controls.SelectFeature.SelectFeatureTool.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.SelectFeature.SelectFeatureTool', SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent);
