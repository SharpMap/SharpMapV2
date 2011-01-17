$(document).ready(function() {
    var po, mercator, layerUrl, container, map;

    po = org.polymaps;
    mercator = new GlobalMercator();
    layerUrl = function(name, data) {
        var bounds = mercator.TileLatLonBounds(data.column, data.row, data.zoom)
        log(bounds);
        return [
            '/Wms.ashx?HEIGHT=256&WIDTH=256&STYLES=&',
            'CRS=EPSG%3A4326&FORMAT=image%2Fpng&SERVICE=WMS&VERSION=1.3.0&REQUEST=GetMap&',
            'EXCEPTIONS=application%2Fvnd.ogc.se_inimage&transparent=true&',
            'LAYERS=', name, '&',
            'BBOX=', bounds[1], ',', -bounds[2], ',', bounds[3], ',', -bounds[0]
        ].join('');
    };

    container = $("#map").get(0).appendChild(po.svg("svg"));
    map = po.map()
        .container(container)
        .center({ lon: -73.9529, lat: 40.7723 })
        .zoom(10)
        .add(po.interact())
        .add(po.hash());

    map.add(po.image().url(function(data) {
        return layerUrl('poly_landmarks,tiger_roads,poi', data);
    }));
    map.add(po.grid());
    map.add(po.compass());
});






















