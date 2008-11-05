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
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Strategy.BBox');

SharpMap.Presentation.Web.SharpLayers.Strategy.BBox.BBoxStrategyComponent = function() {
    SharpMap.Presentation.Web.SharpLayers.Strategy.BBox.BBoxStrategyComponent.initializeBase(this);
}

SharpMap.Presentation.Web.SharpLayers.Strategy.BBox.BBoxStrategyComponent.prototype = {

    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Strategy.BBox.BBoxStrategyComponent.callBaseMethod(this, 'initialize');
    },
    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Strategy.BBox.BBoxStrategyComponent.callBaseMethod(this, 'dispose');
    },
    _strategyBuilderDelegate: function() {
        var options = this.get_builderParams();
        return new OpenLayers.Strategy.BBOX(options);
    }
}

SharpMap.Presentation.Web.SharpLayers.Strategy.BBox.BBoxStrategyComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Strategy.BBox.BBoxStrategyComponent', SharpMap.Presentation.Web.SharpLayers.Strategy.StrategyComponent);