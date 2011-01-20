$(document).ready(function() {
    var options, create, osm, init;

    OpenLayers.DOTS_PER_INCH = 25.4 / 0.28;
    OpenLayers.IMAGE_RELOAD_ATTEMPTS = 5;
    OpenLayers.Util.onImageLoadErrorColor = 'transparent';
    OpenLayers.Util.onImageLoadError = function() {
        this.src = '/Content/Images/sorry.jpg';
        this.style.backgroundColor = OpenLayers.Util.onImageLoadErrorColor;
    };

    options = {
        osm: {
            url: 'http://full.wms.geofabrik.de/std/demo_key?',
            type: 'WMS',
            version: '1.1.1',
            format: 'image/jpeg',
            layers: '',
            srs: '4326'
        },
        wms: {
            url: '/Wms.ashx',
            type: 'WMS',
            version: '1.3.0',
            format: 'image/png',
            layers: ['poly_landmarks',
                     'tiger_roads',
                     'poi'].join(''),
            srs: '4326'
        },
        controls: [],
        maxExtent: new OpenLayers.Bounds(-180, -90, 180, 90),
        numZoomLevels: 24,
        projection: new OpenLayers.Projection('EPSG:4326'),
        displayProjection: new OpenLayers.Projection('EPSG:4326'),
        units: 'meters',
        format: 'image/png'
    };

    create = function(name, tiled) {
        return new OpenLayers.Layer.WMS(name, options.wms.url, {
            layers: options.wms.layers,
            srs: options.wms.srs,
            service: options.wms.type,
            version: options.wms.version,
            format: options.wms.format,
            transparent: true
        }, {
            isBaseLayer: false,
            transparent: true,
            buffer: 0,
            singleTile: !tiled,
            ratio: 1,
            yx: []
        });
    };

    osm = function() {
        return new OpenLayers.Layer.WMS('OpenStreetMap', options.osm.url, {
            layers: options.osm.layers,
            srs: options.osm.srs,
            service: options.osm.type,
            version: options.osm.version,
            format: options.osm.format,
            transparent: false
        }, {
            isBaseLayer: true,
            transparent: false,
            singleTile: false,
            ratio: 1.3
        })
    };

    init = function() {
        var lon = -73.9529;
        var lat = 40.7723;
        var zoom = 10;
        var map, tiled, untiled, center;

        map = new OpenLayers.Map('map', options);
        map.addControl(new OpenLayers.Control.LayerSwitcher());
        map.addControl(new OpenLayers.Control.NavToolbar());
        map.addControl(new OpenLayers.Control.PanZoom({
            position: new OpenLayers.Pixel(2, 10)
        }));
        map.addControl(new OpenLayers.Control.MousePosition());

        tiled = create('TiledWMS', true);
        untiled = create('UntiledWMS', false);
        untiled.setVisibility(false);
        map.addLayers([osm(), tiled, untiled]);
        
        center = new OpenLayers.LonLat(lon, lat);
        map.setCenter(center, zoom);
    };
    init();
});
