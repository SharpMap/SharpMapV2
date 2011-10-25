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
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.OpenLayersFactory');


SharpMap.Presentation.Web.SharpLayers.InitSync = {
    _pageLoaded: false,
    _appLoaded: false,
    _sharpLayersLoaded: false,
    get_pageLoaded: function() {
        return SharpMap.Presentation.Web.SharpLayers.InitSync._pageLoaded;
    },
    get_appLoaded: function() {
        return SharpMap.Presentation.Web.SharpLayers.InitSync._appLoaded;
    },
    get_sharpLayersLoaded: function() {
        return SharpMap.Presentation.Web.SharpLayers.InitSync.get_pageLoaded()
        && SharpMap.Presentation.Web.SharpLayers.InitSync._sharpLayersLoaded;
    },

    _pendingInit: [],
    _pendingLoad: [],
    _pendingPostLoad: [],
    _objectsBuilt: 0,
    _delegatesRun: 0,
    _postLoadDelegatesRun: 0,

    addInit: function(obj) {
        if (SharpMap.Presentation.Web.SharpLayers.InitSync.get_sharpLayersLoaded()) {
            obj.buildObject();
            SharpMap.Presentation.Web.SharpLayers.InitSync._objectsBuilt++;
        }
        else
            Array.add(SharpMap.Presentation.Web.SharpLayers.InitSync._pendingInit, obj);
    },
    addLoad: function(delegate) {
        if (SharpMap.Presentation.Web.SharpLayers.InitSync.get_sharpLayersLoaded()) {
            delegate();
            SharpMap.Presentation.Web.SharpLayers.InitSync._delegatesRun++;
        }
        else
            Array.add(SharpMap.Presentation.Web.SharpLayers.InitSync._pendingLoad, delegate);
    },
    addPostLoad: function(delegate) {
        if (SharpMap.Presentation.Web.SharpLayers.InitSync.get_sharpLayersLoaded()) {
            delegate();
            SharpMap.Presentation.Web.SharpLayers.InitSync._postLoadDelegatesRun++;
        }
        else
            Array.add(SharpMap.Presentation.Web.SharpLayers.InitSync._pendingPostLoad, delegate);
    },
    doInit: function() {
        if (SharpMap.Presentation.Web.SharpLayers.InitSync.get_pageLoaded() != true
        || SharpMap.Presentation.Web.SharpLayers.InitSync.get_appLoaded() != true)
            return;


        while (SharpMap.Presentation.Web.SharpLayers.InitSync._pendingInit.length > 0) {
            var obj = Array.dequeue(SharpMap.Presentation.Web.SharpLayers.InitSync._pendingInit);
            obj.buildObject();
            SharpMap.Presentation.Web.SharpLayers.InitSync._objectsBuilt++;
        }
        while (SharpMap.Presentation.Web.SharpLayers.InitSync._pendingLoad.length > 0) {
            var delegate = Array.dequeue(SharpMap.Presentation.Web.SharpLayers.InitSync._pendingLoad);
            delegate();
            SharpMap.Presentation.Web.SharpLayers.InitSync._delegatesRun++;
        }
        SharpMap.Presentation.Web.SharpLayers.InitSync._sharpLayersLoaded = true;
        while (SharpMap.Presentation.Web.SharpLayers.InitSync._pendingPostLoad.length > 0) {
            var delegate = Array.dequeue(SharpMap.Presentation.Web.SharpLayers.InitSync._pendingPostLoad);
            delegate();
            SharpMap.Presentation.Web.SharpLayers.InitSync._postLoadDelegatesRun++;
        }
        //Sys.WebForms.PageRequestManager.getInstance().remove_pageLoaded(SharpMap.Presentation.Web.SharpLayers.InitSync.pageLoadDone);
        //Sys.Application.remove_load(SharpMap.Presentation.Web.SharpLayers.InitSync.appInitDone);
    },

    appInitDone: function() {
        SharpMap.Presentation.Web.SharpLayers.InitSync._appLoaded = true;
        SharpMap.Presentation.Web.SharpLayers.InitSync.doInit();
    },

    pageLoadDone: function() {
        SharpMap.Presentation.Web.SharpLayers.InitSync._pageLoaded = true;
        SharpMap.Presentation.Web.SharpLayers.InitSync.doInit();
    },
    pageLoading: function() {
        SharpMap.Presentation.Web.SharpLayers.InitSync._pageLoaded = false;
    }


}

Sys.WebForms.PageRequestManager.getInstance().add_pageLoaded(SharpMap.Presentation.Web.SharpLayers.InitSync.pageLoadDone);
Sys.WebForms.PageRequestManager.getInstance().add_pageLoading(SharpMap.Presentation.Web.SharpLayers.InitSync.pageLoading)

Sys.Application.add_load(SharpMap.Presentation.Web.SharpLayers.InitSync.appInitDone);

SharpMap.Presentation.Web.SharpLayers.OpenLayersFactory._factories = {};

SharpMap.Presentation.Web.SharpLayers.OpenLayersFactory.__trackReference = function(storage, obj) {
    if (!Array.contains(storage, obj))
        storage.push(obj);
};
SharpMap.Presentation.Web.SharpLayers.OpenLayersFactory.__slBuilderIgnore = function() { return true; }

SharpMap.Presentation.Web.SharpLayers.OpenLayersFactory.buildParams = function(originalParams, referenceTracker) {
    if (originalParams == null)
        return;
    if (originalParams.slBuilderIgnore && originalParams.slBuilderIgnore() == true)
        return originalParams;

    var trackerCreated = false;
    if (referenceTracker == null) {
        referenceTracker = new Array();
        trackerCreated = true;
    }
    var isArray = originalParams instanceof Array;

    var changed = false;

    var newParams = isArray ? new Array() : {};

    for (var k in originalParams) {

        var v = originalParams[k];
        var cmpr = v;

        if (v == null)
            continue;

        if (typeof v == "object" && !(v.nodeName) && (!v.slBuilderIgnore || !v.slBuilderIgnore()) && !Array.contains(referenceTracker, v)) {

            $olFactory.__trackReference(referenceTracker, v);
            v = $olFactory.buildParams(originalParams[k], referenceTracker);
            $olFactory.__trackReference(referenceTracker, v);

            if (v["typeToBuild"] != null)
                v = $olFactory.buildOpenLayersObject(v);
            if (v["builderAction"] != null) {
                var a = v.builderAction;
                switch (a) {
                    case "reference":
                        {
                            var id = v.refId;
                            var t = v.referenceType;
                            switch (t) {
                                case "Component":
                                    {
                                        v = $find(id);
                                        break;
                                    }
                                case "Element":
                                    {
                                        v = $get(id);
                                        break;
                                    }
                            }
                            break;
                        }
                    case "evaluate":
                        {
                            eval("v=" + v.script);
                            break;
                        }
                    default: throw "Unknown Builder Action";
                }
            }

            if (v != cmpr)
                changed = true;
            if ((v))
                v.slBuilderIgnore = SharpMap.Presentation.Web.SharpLayers.OpenLayersFactory.__slBuilderIgnore;
        }
        if (isArray)
            newParams.push(v);
        else
            newParams[k] = v;
    }
    if (trackerCreated == true)
        delete referenceTracker;


    var retVal = changed ? newParams : originalParams;
    retVal.slBuilderIgnore = SharpMap.Presentation.Web.SharpLayers.OpenLayersFactory.__slBuilderIgnore;
    return retVal;

}
SharpMap.Presentation.Web.SharpLayers.OpenLayersFactory.buildOpenLayersObject = function(buildParams) {

    var typeToBuild = buildParams["typeToBuild"];
    if (typeToBuild == null)
        throw "Invalid build params";
    delete buildParams.typeToBuild;


    var dlgt = $olFactory._factories[typeToBuild];

    if (dlgt != null)
        return dlgt(buildParams);
    throw "Unknown Type" + typeToBuild;


}


SharpMap.Presentation.Web.SharpLayers.OpenLayersFactory._factories["OpenLayers.Bounds"] = function(buildParams) {
    return new OpenLayers.Bounds(buildParams.left, buildParams.bottom, buildParams.right, buildParams.top);
}
SharpMap.Presentation.Web.SharpLayers.OpenLayersFactory._factories["OpenLayers.Size"] = function(buildParams) {
    return new OpenLayers.Size(buildParams.w, buildParams.h);
}
SharpMap.Presentation.Web.SharpLayers.OpenLayersFactory._factories["OpenLayers.Pixel"] = function(buildParams) {
    return new OpenLayers.Pixel(buildParams.x, buildParams.y);
}
SharpMap.Presentation.Web.SharpLayers.OpenLayersFactory._factories["OpenLayers.LonLat"] = function(buildParams) {
    return new OpenLayers.LonLat(buildParams.lon, buildParams.lat);
}

SharpMap.Presentation.Web.SharpLayers.OpenLayersFactory._factories["OpenLayers.Strategy.BBOX"] = function(buildParams) {
    return new OpenLayers.Strategy.BBOX(buildParams);
}

SharpMap.Presentation.Web.SharpLayers.OpenLayersFactory._factories["OpenLayers.Format.GeoJSON"] = function(buildParams) {
    return new OpenLayers.Format.GeoJSON(buildParams);
}

SharpMap.Presentation.Web.SharpLayers.OpenLayersRegistryItem = function(msAjaxId, openLayersId, itemType) {
    this._msAjaxId = msAjaxId;
    this._openLayersId = openLayersId;
    this._itemType = itemType;
}

SharpMap.Presentation.Web.SharpLayers.OpenLayersRegistryItem.prototype = {
    get_msAjaxId: function() { return this._msAjaxId; },
    get_msAjaxObject: function() { return $find(this.get_msAjaxId()); },
    get_openLayersId: function() { return this._openLayersId; },
    get_itemType: function() { return this._itemType; },
    get_openLayersItem: function() { return this.get_msAjaxObject().get_hostedItem(); }
}
SharpMap.Presentation.Web.SharpLayers.OpenLayersRegistryItem.type = "RegistryItem";


///allows a centralized place to lookup an item either by
///its ms ajax id or its open layers id
///note this only holds for objects built via the HostBaseExtender
///or component base extender
SharpMap.Presentation.Web.SharpLayers.OpenLayersRegistry = {
    _list: [],
    findItemByOpenLayersId: function(id) {
        return this._single(
            this.findItems(function(obj) { return obj.get_openLayersId() == id; }));

    },

    _single: function(res) {
        if (res.length > 1)
            throw "multiple objects with the same id";
        if (res.length == 1)
            return res[0];
        return null;
    },

    findItemsByType: function(className) {
        return this.findItems(function(obj) {
            return obj.get_itemType() == className;
        });
    },

    findItemByMsAjaxId: function(id) {
        return this._single(
        this.findItems(function(obj) { return obj.get_msAjaxId() == id; }));
    },

    findItems: function(predicate) {
        if (typeof predicate != "function")
            throw "predicate must be a function";
        var storage = [];

        for (var i = 0; i < this._list.length; i++) {
            if (predicate(this._list[i]))
                storage.push(this._list[i]);
        }
        return storage;

    },

    findFirst: function(predicate) {
        if (typeof predicate != "function")
            throw "predicate must be a function";
        for (var ndx in this._list)
            if (predicate(this._list[ndx]))
            return this._list[ndx];
    },

    add: function(registryItem) {
        if (!registryItem.type == "RegistryItem")
            throw "An attempt was made to add an invalid type to the registry";
        if (registryItem.get_msAjaxId() == null)
            throw "The item has no id";
        this._ensureNotExists(registryItem);
        this._list.push(registryItem);
    },

    _ensureNotExists: function(registryItem) {
        if (this.findFirst(function(obj) { return obj == registryItem; }) != null)
            throw new "An attempt was made to add a duplicate object";
    },

    remove: function(registryItem) {
        var ndx = -1;
        for (var i = 0; i < this._list.length; i++) {
            if (this._list[i] == registryItem) {
                ndx = i;
                break;
            }
        }
        if (ndx == -1)
            return;
        this._list.splice(ndx, 1);
    }
};

SharpMap.Presentation.Web.SharpLayers.OpenLayersRegistry.findByOpenLayersObject = function(obj) {
    return $olRegistry.findItemByOpenLayersId(obj.id);
}

SharpMap.Presentation.Web.SharpLayers.GeometryUtilities = {
    pointOnLineString: function(lineString, factor) {
        if (lineString.CLASS_NAME == null || lineString.CLASS_NAME != "OpenLayers.Geometry.LineString")
            throw "argument lineString must be of type OpenLayers.Geometry.LineString";

        if (factor < 0.0 || factor > 1.0)
            throw "factor should range between 0 and 1";
        var lineLength = lineString.getLength();

        var targetLength = lineLength * factor;

        var points = lineString.components;

        var prevPoint = null;
        var currentLength = 0;
        var ndx = 1;
        var currentPoint = points[0];

        while (currentLength < targetLength) {
            prevPoint = currentPoint;
            currentPoint = points[ndx];
            var d = prevPoint.distanceTo(currentPoint)
            if (currentLength + d > targetLength)
                break;
            currentLength += d;
            ndx++;
        }

        if (currentLength != targetLength);
        {
            var delta = targetLength - currentLength;
            var p = delta / prevPoint.distanceTo(currentPoint);
            var dx = (currentPoint.x - prevPoint.x) * p;
            var dy = (currentPoint.y - prevPoint.y) * p;
            currentPoint = new OpenLayers.Geometry.Point(prevPoint.x + dx, prevPoint.y + dy);
        }
        return currentPoint;
    },

    midPointOnLineString: function(lineString) {
        return $geomUtils.pointOnLineString(lineString, 0.5);
    }

}

SharpMap.Presentation.Web.SharpLayers.Utility = {
    getAttribute: function(object, attributeName) {

        if (object.nodeName)
            return object.getAttribute(attributeName);

        if (object[attributeName])
            return object[attributeName];


        return object;
    },
    zoomToFeature: function(map, feature) {
        map = map || feature.layer.map;
        if (feature.bbox && (feature.bbox.getWidth() > 0 || feature.bbox.getHeight() > 0)) {
            map.zoomToExtent(feature.bbox, true)
        }
        else {
            if (feature.geometry.CLASS_NAME == "OpenLayers.Geometry.Point"
            || (feature.geometry.getBounds().getWidth() == 0 && feature.geometry.getBounds().getHeight() == 0))
                map.setCenter(feature.geometry.getBounds().getCenterLonLat(), map.getZoomForResolution(map.resolutions[map.resolutions.length - 1], true));
            else {
                map.zoomToExtent(feature.geometry.getBounds(), true)
            }
        }
    },
    panToFeature: function(map, feature) {
        map = map || feature;
        var p;
        if (feature.bbox && (feature.bbox.getWidth() > 0 || feature.bbox.getHeight() > 0)) {
            p = feature.bbox.getCenterLonLat();
        }
        else {
            var b = feature.geometry.getBounds();
            p = b.getCenterLonLat();
        }
        map.setCenter(p);
    }
};


$olRegistry = SharpMap.Presentation.Web.SharpLayers.OpenLayersRegistry;
$olRegItem = SharpMap.Presentation.Web.SharpLayers.OpenLayersRegistryItem;
$olFactory = SharpMap.Presentation.Web.SharpLayers.OpenLayersFactory;
$olUtils = SharpMap.Presentation.Web.SharpLayers.Utility;
$geomUtils = SharpMap.Presentation.Web.SharpLayers.GeometryUtilities;

  