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
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Layers');

SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent = function() {
    SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent.initializeBase(this);

    this._targetMapHost = null;
    this._name = null;

}
SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent.prototype = {
    initialize: function() {
        this._builderDelegate = Function.createDelegate(this, this._layerBuilderDelegate);
        SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent.callBaseMethod(this, 'initialize');
    },
    dispose: function() {
        delete this._builderDelegate;
        delete this._layerBuilderDelegate;
        SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent.callBaseMethod(this, 'dispose');
    },
    get_name: function() {
        return this._name;
    },
    set_name: function(name) {
        this._name = name;
    },
    get_targetMapHost: function() {
        return this._targetMapHost;
    },
    set_targetMapHost: function(mapHost) {
        if (typeof mapHost == "string")
            mapHost = $find(mapHost);
        this._targetMapHost = mapHost;
    },
    buildObject: function() {
        SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent.callBaseMethod(this, 'buildObject');
        //add the layer to the map
        var mapHost = this.get_targetMapHost();
        mapHost.get_hostedItem().addLayer(this.get_hostedItem());
    }
}
SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent', SharpMap.Presentation.Web.SharpLayers.ComponentBase);
