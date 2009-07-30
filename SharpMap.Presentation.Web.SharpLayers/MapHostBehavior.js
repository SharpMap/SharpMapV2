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
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers');

SharpMap.Presentation.Web.SharpLayers.MapHostBehavior = function(element) {
    SharpMap.Presentation.Web.SharpLayers.MapHostBehavior.initializeBase(this, [element]);
    _initialExtent = null;


}
SharpMap.Presentation.Web.SharpLayers.MapHostBehavior.prototype = {
    initialize: function() {
        this._builderDelegate = Function.createDelegate(this, this._mapBuilderDelegate);
        SharpMap.Presentation.Web.SharpLayers.MapHostBehavior.callBaseMethod(this, 'initialize');
    },

    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.MapHostBehavior.callBaseMethod(this, 'dispose');
        delete this._mapBuilderDelegate;
    },
    get_initialExtent: function() {
        return this._initialExtent;
    },
    set_initialExtent: function(value) {
        this._initialExtent = value;
    },
    _mapBuilderDelegate: function() {
        var options = this.get_builderParams();
        options.controls = [new OpenLayers.Control.Navigation(),
                            new OpenLayers.Control.ArgParser(),
                            new OpenLayers.Control.Attribution(),
                            new OpenLayers.Control.KeyboardDefaults()];

        this.set_initialExtent(options.initialExtent);
        delete options.initialExtent;

        var map = new OpenLayers.Map(this.get_element(), options);
        map.options = options; //hack to aid serialization
        return map;
    },
    addControl: function(cntrl) {
        this.get_hostedItem().addControl(cntrl);
    },
    zoomToInitialExtent: function() {
        var bounds = this.get_initialExtent();
        if ((bounds))
            this.get_hostedItem().zoomToMaxExtent();
        else
            this.get_hostedItem().zoomTo(bounds, true);
    }


}

SharpMap.Presentation.Web.SharpLayers.MapHostBehavior.zoomToFeature = function(mapHost, feature) {

    var map = mapHost.get_hostedItem();
    SharpMap.Presentation.Web.SharpLayers.Utility.zoomToFeature(map, feature);
}

SharpMap.Presentation.Web.SharpLayers.MapHostBehavior.panToFeature = function(mapHost, feature) {

    var map = mapHost.get_hostedItem();
    SharpMap.Presentation.Web.SharpLayers.Utility.panToFeature(mapHost, feature);
}

SharpMap.Presentation.Web.SharpLayers.MapHostBehavior.drawFeatureOnMap = function(mapHost, vectorLayerHost, feature) {
    var layer = vectorLayerHost.get_hostedItem();
    layer.addFeatures(feature, null);
    feature.layer = layer;
}

SharpMap.Presentation.Web.SharpLayers.MapHostBehavior.registerClass('SharpMap.Presentation.Web.SharpLayers.MapHostBehavior', SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior);
