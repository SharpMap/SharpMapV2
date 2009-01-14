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
