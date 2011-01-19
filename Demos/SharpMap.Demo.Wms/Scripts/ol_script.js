$(document).ready(function() {
    var options;

    OpenLayers.DOTS_PER_INCH = 25.4 / 0.28;
    OpenLayers.IMAGE_RELOAD_ATTEMPTS = 5;
    OpenLayers.Util.onImageLoadErrorColor = "transparent";
    OpenLayers.Util.onImageLoadError = function() {
        this.src = "/Content/Images/sorry.jpg";
        this.style.backgroundColor = OpenLayers.Util.onImageLoadErrorColor;
    };

    options = {
        osm:{
            url: 'http://full.wms.geofabrik.de/std/demo_key?'
        },
        wms: {
            id: "sharpmapv2",
            type: "WMS",
            version: "1.3.0",
            url: "/Wms.ashx",
            layers: 'poly_landmarks,tiger_roads,poi',
            srs: "4326"
        },
        controls: [],        
        maxExtent: new OpenLayers.Bounds(-180, -90, 180, 90),
        numZoomLevels: 24,           
        projection: new OpenLayers.Projection("EPSG:4326"),
        units: "meters",
        format: "image/png"
    };

    init = function() {
        var lon = -73.9529;
        var lat = 40.7723;
        var zoom = 12;
        var map, osm, layer, init;

        map = new OpenLayers.Map('map', options);
        map.addControl(new OpenLayers.Control.LayerSwitcher());
        map.addControl(new OpenLayers.Control.Navigation());
        map.addControl(new OpenLayers.Control.PanZoomBar());
        map.addControl(new OpenLayers.Control.MousePosition());

        osm = new OpenLayers.Layer.WMS("OpenStreetMap", options.osm.url, {
            layers: "", 
            format: options.format,
            tiled: true
        }, {
            isBaseLayer: true,
            singleTile: false, 
            ratio: 1 
        });
        layer = new OpenLayers.Layer.WMS("SampleWMS", options.wms.url, {
            layers: options.wms.layers,
            srs: options.wms.srs,
            service: options.wms.type,
            version: options.wms.version,
            format: options.format,
            transparent: true,
            tiled: false                
        }, {
            wmsName: options.wms.id,
            isBaseLayer: false,
            transparent: true,
            buffer: 0,
            displayOutsideMaxExtent: true,
            singleTile: true,
            yx: []
        });
        map.addLayers([osm, layer]);        
        map.setCenter(new OpenLayers.LonLat(lon, lat), zoom);
    };
    init();
});
