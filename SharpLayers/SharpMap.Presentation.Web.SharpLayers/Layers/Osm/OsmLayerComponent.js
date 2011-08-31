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
*  Author: John Diss 2010
* 
*/
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Layers.Osm');

SharpMap.Presentation.Web.SharpLayers.Layers.Osm.OsmLayerComponent = function() {
    SharpMap.Presentation.Web.SharpLayers.Layers.Osm.OsmLayerComponent.initializeBase(this);


}
SharpMap.Presentation.Web.SharpLayers.Layers.Osm.OsmLayerComponent.prototype = {
    initialize: function() {
        this._builderDelegate = Function.createDelegate(this, this._layerBuilderDelegate);
        SharpMap.Presentation.Web.SharpLayers.Layers.Osm.OsmLayerComponent.callBaseMethod(this, 'initialize');
    },
    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Layers.Osm.OsmLayerComponent.callBaseMethod(this, 'dispose');
    },
    _layerBuilderDelegate: function() {
        var options = this.get_builderParams();
        var url = options.url;
        delete options.url;
        return new OpenLayers.Layer.OSM(this.get_name(), url, options);
    }
}
SharpMap.Presentation.Web.SharpLayers.Layers.Osm.OsmLayerComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Layers.Osm.OsmLayerComponent', SharpMap.Presentation.Web.SharpLayers.Layers.LayerComponent);
