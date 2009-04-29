SharpMap.Presentation.Web.SharpLayers.Controls.Measure.MeasureToolButton = OpenLayers.Class(OpenLayers.Control.Measure, {
    type: OpenLayers.Control.TYPE_TOOL,

    draw: function(px) {
        if (this.div == null) {
            this.div = OpenLayers.Util.createDiv(this.id);
        }

        this.div.className = this.displayClass + (this.active ? "Active" : "Inactive");
        if (!this.allowSelection) {
            this.div.className += " olControlNoSelect";
            this.div.setAttribute("unselectable", "on", 0);
            this.div.onselectstart = function() { return (false); };
        }
        if (this.title != "") {
            this.div.title = this.title;
        }

        if (px != null) {
            this.position = px.clone();
        }
        this.moveTo(this.position);
        return this.div;
    },

    CLASS_NAME: "SharpMap.Presentation.Web.SharpLayers.Controls.Measure.MeasureToolButton"
});