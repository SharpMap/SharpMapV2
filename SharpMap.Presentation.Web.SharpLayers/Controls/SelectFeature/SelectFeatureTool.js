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
        var layer = params.layer.get_hostedItem();
        delete params.layer;

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
