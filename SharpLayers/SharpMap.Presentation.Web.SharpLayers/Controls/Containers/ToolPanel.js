Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Controls.Containers');

SharpMap.Presentation.Web.SharpLayers.Controls.Containers.ToolPanel = function() {
    SharpMap.Presentation.Web.SharpLayers.Controls.Containers.ToolPanel.initializeBase(this);
    this._childControlHostIds = null;
}
SharpMap.Presentation.Web.SharpLayers.Controls.Containers.ToolPanel.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.Containers.ToolPanel.callBaseMethod(this, 'initialize');
    },

    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.Containers.ToolPanel.callBaseMethod(this, 'dispose');
    },
    get_childControlHostIds: function() {
        return this._childControlHostIds;
    },
    set_childControlHostIds: function(idArr) {
        this._childControlHostIds = idArr;
    },
    _toolBuilderDelegate: function() {
        var opts = this.get_builderParams();
        if (opts.div && typeof opts.div == "string")
            opts.div = $get(opts.div);

        var p = new OpenLayers.Control.Panel(opts);
        var childControls = [];
        var ids = this.get_childControlHostIds();
        for (var ndx in ids) {
            var host = $find(ids[ndx]);
            childControls.push(host.get_hostedItem());
        }
        p.addControls(childControls);
        return p;
    }
}
SharpMap.Presentation.Web.SharpLayers.Controls.Containers.ToolPanel.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.Containers.ToolPanel', SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent);
