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
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Controls.Edit');

SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools = function(element) {
    SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools.initializeBase(this, [element]);

}
SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools.callBaseMethod(this, 'initialize');

        // TODO: Add your initalization code here
    },

    dispose: function() {
        // TODO: Add your cleanup code here

        SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools.callBaseMethod(this, 'dispose');
    },
    _toolBuilderDelegate: function() {
        var options = this.get_builderParams();
        options.div = typeof options.div == "string" ? $get(options.div) : options.div;

        var layer = options.layer;
        delete options.layer

        if (typeof layer == "string")
            layer = $find(layer).get_hostedItem();

        return new OpenLayers.Control.EditingToolbar(layer, options);
    }
}
SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.Edit.EditingTools', SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent);
