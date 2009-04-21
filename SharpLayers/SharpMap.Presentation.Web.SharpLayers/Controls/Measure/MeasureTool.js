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

        // TODO: Add your initalization code here
    },

    dispose: function() {
        // TODO: Add your cleanup code here

        SharpMap.Presentation.Web.SharpLayers.Controls.Measure.MeasureTool.callBaseMethod(this, 'dispose');
    },
    _toolBuilderDelegate: function() {
        var options = this.get_builderParams();
        if (!(options))
            throw "invalid builder params for measure tool";

        var styleMap;
        var selector;
        var sld;
        if ((options.styleSelector)) {
            selector = eval(options.styleSelector);
            delete options.styleSelectror;
        }
        if ((options.sld)) {
            sld = options.sld;
            delete options.sld;
        }

        if ((selector) && (sld))
            styleMap = selector(sld.get_hostedItem());

        options.handlerOptions = { "styleMap": styleMap };

        var handler = eval(options.handler);
        delete options.handler;

        var call = options.onClientMeasure;
        delete options.onClientMeasure;

        options.type = OpenLayers.Control.TYPE_TOOL;

        call = (call) ? call : function(e) { alert(e); };

        var m = new OpenLayers.Control.Measure(handler, options);

        m.events.on({ "measure": call });
        return m;
    }

}
SharpMap.Presentation.Web.SharpLayers.Controls.Measure.MeasureTool.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.Measure.MeasureTool', SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent);
