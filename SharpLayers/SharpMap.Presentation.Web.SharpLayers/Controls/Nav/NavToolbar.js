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
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Controls.Nav');

SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar = function() {
    SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar.initializeBase(this);
}
SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar.callBaseMethod(this, 'initialize');
    },

    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar.callBaseMethod(this, 'dispose');

    },
    _toolBuilderDelegate: function() {
        var options = this.get_builderParams();
        if (options["div"] && typeof options["div"] == "string")
            options.div = $get(options.div);
        return new OpenLayers.Control.NavToolbar(options);
    }
}
SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.Nav.NavToolbar', SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent);
