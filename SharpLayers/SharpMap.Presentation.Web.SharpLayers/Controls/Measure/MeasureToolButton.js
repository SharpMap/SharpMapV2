SharpMap.Presentation.Web.SharpLayers.Controls.Measure.MeasureToolButton = OpenLayers.Class(OpenLayers.Control, {
    type: OpenLayers.Control.TYPE_TOOL,
    innerTool: null,

    initialize: function(innerTool, options) {
        OpenLayers.Control.prototype.initialize.apply(this, [options]);
        this.innerTool = innerTool;
    },
    
    destroy: function() {
        this.innerTool.destroy();
        OpenLayers.Control.prototype.destroy.apply(this, arguments);
    },

    activate: function() {
        this.innerTool.activate();
        return OpenLayers.Control.prototype.activate.apply(this, arguments);
    },

    deactivate: function() {
        this.innerTool.deactivate();
        return OpenLayers.Control.prototype.deactivate.apply(this, arguments);
    },

    setMap: function(map) {
        this.innerTool.setMap(map);
        OpenLayers.Control.prototype.setMap.apply(this, arguments);
    },
    
    CLASS_NAME: "HowlingAtTheMoon"
});