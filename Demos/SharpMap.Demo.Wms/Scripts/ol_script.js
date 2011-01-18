$(document).ready(function() {
    var lon = -73.9529;
    var lat = 40.7723;
    var zoom = 10;
    
    var options, map, layer, init;

    OpenLayers.DOTS_PER_INCH = 25.4 / 0.28;
    OpenLayers.IMAGE_RELOAD_ATTEMPTS = 5;
    OpenLayers.Util.onImageLoadErrorColor = "transparent";
    OpenLayers.Util.onImageLoadError = function() {
        this.src = "/Content/Images/sorry.jpg";
        this.style.backgroundColor = OpenLayers.Util.onImageLoadErrorColor;
    };

    options = {
        wms: {
            id: "sharpmapv2",
            type: "WMS",           
            version: "1.3.0",
            url: "/Wms.ashx",
            layers: 'poly_landmarks,tiger_roads,poi'
        },
        controls: [],
        projection: new OpenLayers.Projection("EPSG:4326"),
        displayProjection: new OpenLayers.Projection("EPSG:4326"),
        units: "meters",
        format: "image/png"
    };
    init = function() {
        map = new OpenLayers.Map('map', options);
        layer = new OpenLayers.Layer.WMS("Sample WMS", options.wms.url, {
            layers: options.wms.layers,
            service: options.wms.type,
            version: options.wms.version,
            format: options.format,
            transparent: true,
            tiled: true
        }, {
            wmsName: options.wms.id,
            isBaseLayer: true,
            transparent: true,
            buffer: 0,
            displayOutsideMaxExtent: true,
            singleTile: false
        });
        map.addLayer(layer);
        map.setCenter(new OpenLayers.LonLat(lon, lat), zoom);
        map.addControl(new OpenLayers.Control.LayerSwitcher());
        map.addControl(new OpenLayers.Control.PanZoom({
            position: new OpenLayers.Pixel(2, 10)
        }));
        map.addControl(new OpenLayers.Control.MousePosition());
    };
    init();
});
