/*
*  The attached / following is part of SharpMap.Presentation.Web.SharpLayers
*  SharpMap.Presentation.Web.SharpLayers is free software © 2008 Newgrove Consultants Limited, 
*  www.newgrove.com; you can redistribute it and/or modify it under the terms 
*  of the current GNU Lesser General Public License (LGPL) as published by and 
*  available from the Free Software Foundation, Inc., 
*  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
*  This program is distributed without any warranty; 
*  without even the implied warranty of merchantability or fitness for purpose.  
*  See the GNU Lesser General Public License for the full details. 
*  
*  Author: John Diss 2009
* 
*/
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Controls.Measure');

SharpMap.Presentation.Web.SharpLayers.Controls.Measure.MeasureTool = function(element) {
    SharpMap.Presentation.Web.SharpLayers.Controls.Measure.MeasureTool.initializeBase(this, [element]);
}
SharpMap.Presentation.Web.SharpLayers.Controls.Measure.MeasureTool.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.Measure.MeasureTool.callBaseMethod(this, 'initialize');
    },

    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.Measure.MeasureTool.callBaseMethod(this, 'dispose');
    },
    _toolBuilderDelegate: function() {
        var options = this.get_builderParams();
        if (!(options))
            throw "invalid builder params for measure tool";

        var styleMap;
        var selector;
        var sld;
        var div;

        div = options.div;
        delete options.div;
        if ((options.styleSelector)) {
            selector = eval(options.styleSelector);
            delete options.styleSelector;
        }
        if ((options.sld)) {
            sld = options.sld;
            delete options.sld;
        }

        if ((selector) && (sld))
            styleMap = selector(sld.get_hostedItem());
        options.handlerOptions = { "layerOptions": { "styleMap": styleMap} };

        var handler = eval(options.handler);
        delete options.handler;

        var call = options.onClientMeasure;
        delete options.onClientMeasure;


        options.type = OpenLayers.Control.TYPE_TOOL;

        if (options.mode == 'Area') {
            options.displayClass = (options.displayClass) || "olControlMeasureArea";
            
        }
        else {
            options.displayClass = (options.displayClass) || "olControlMeasure";
        }
        call = (call) ? call : function(e) {
        
        
        if (options.mode == 'Area') {
            alert("Approx " + Math.round(e.measure,2) + " square " + e.units, "Measurement"); 
        }
        else {
            alert("Approx " + Math.round(e.measure,2) + " " + e.units, "Measurement");
        }
        
        
        };



        var m = new SharpMap.Presentation.Web.SharpLayers.Controls.Measure.MeasureToolButton(handler, options);

        m.events.on({ "measure": call });



        var p = new OpenLayers.Control.Panel({ "div": div, "displayClass": "" });

        //jd: following needs to be moved out
        p._destroy = p.destroy;
        p.destroy = function() {
            Array.remove(OpenLayers.Control.Panel.panels, this);
            this._destroy();
        }

        OpenLayers.Control.Panel.panels.push(p);
        p._onClick = p.onClick;
        p.onClick = function(ctl, evt) {
            OpenLayers.Control.Panel.onPanelActivated(this);
            return this._onClick(ctl, evt);
        };

        p.addControls([m]);


        return p;

    }

}
SharpMap.Presentation.Web.SharpLayers.Controls.Measure.MeasureTool.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.Measure.MeasureTool', SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent);
