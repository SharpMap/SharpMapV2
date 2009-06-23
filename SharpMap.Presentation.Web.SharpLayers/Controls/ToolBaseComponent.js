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
*  Author: John Diss 2008
* 
*/
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Controls');

SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent = function() {
    SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.initializeBase(this);
    this._targetMapHost = null;

}
SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.prototype = {
    initialize: function() {
        this._builderDelegate = Function.createDelegate(this, this._toolBuilderDelegate);
        SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.callBaseMethod(this, 'initialize');

        var mapHost = this.get_targetMapHost(); //this should be either a maphost or a toolPanel
        var _this = this;
        var f = function() {
            mapHost.addControl(_this.get_hostedItem());
        };

        this.scheduleAddToMap(f);
    },
    scheduleAddToMap: function(func) {
        SharpMap.Presentation.Web.SharpLayers.InitSync.addLoad(func);
    },

    dispose: function() {

        SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.callBaseMethod(this, 'dispose');
    },
    get_targetMapHost: function() {
        return this._targetMapHost;
    },
    set_targetMapHost: function(val) {
        if (typeof val == "string")
            val = $find(val);
        this._targetMapHost = val;
    },
    buildObject: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.callBaseMethod(this, 'buildObject');
        //add the control to the map
    }
}
SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent', SharpMap.Presentation.Web.SharpLayers.ComponentBase);
