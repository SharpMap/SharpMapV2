Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers');

SharpMap.Presentation.Web.SharpLayers.ComponentBase = function(element) {
    SharpMap.Presentation.Web.SharpLayers.ComponentBase.initializeBase(this);

    this._hostedItem = null;
    this._builderDelegate = null;
    this._builderParams = null;
}

SharpMap.Presentation.Web.SharpLayers.ComponentBase.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.ComponentBase.callBaseMethod(this, 'initialize');
        this.buildObject();


    },
    dispose: function() {
        var item = this.get_hostedItem();
        if (item) {
            if (item.dispose != null)
                item.dispose();
            else if (item.destroy != null)
                try { item.destroy(); } catch (e) { }
            this.set_hostedItem(null);
        }
        delete this._hostedItem;
        delete this._builderDelegate;
        delete this._builderParams;

        SharpMap.Presentation.Web.SharpLayers.OpenLayersRegistry.remove(this.get_registryItem());
        SharpMap.Presentation.Web.SharpLayers.ComponentBase.callBaseMethod(this, 'dispose');
    },
    get_hostedItem: function() {
        return this._hostedItem;
    },
    set_hostedItem: function(value) {
        this._hostedItem = value;
    },
    _checkItemCreated: function() {
        if (this._hostedItem === null || this._hostedItem === 'undefined')
            throw "Item has not been created yet.";
    },
    get_builderParams: function() { return this._builderParams; },
    set_builderParams: function(value) {
        this._builderParams = $olFactory.buildParams(value);
    },
    get_builderDelegate: function() { return this._builderDelegate; },
    buildObject: function() {
        if (this._hostedItem != null && this._hostedItem != 'undefined')
            throw "The object has already been built";
        if (this._builderDelegate === null || this._builderDelegate === 'undefined')
            throw "No builder delegate set";

        //build the item
        var obj = this._builderDelegate();
        //create a registry item
        var regItem = new $olRegItem(this.get_id(), obj.id, obj.CLASS_NAME);
        //add the registry item to the registry
        $olRegistry.add(regItem);
        //create a reference to the hosted item
        this.set_hostedItem(obj);
    },
    get_hostedItemType: function() {
        return this.get_hostedItem().CLASS_NAME;
    },
    get_hostedItemId: function() {
        return this.get_hostedItem().id;
    },
    get_registryItem: function() {
        return $olRegistry.findItemByMsAjaxId(this.get_id());
    }
}
SharpMap.Presentation.Web.SharpLayers.ComponentBase.registerClass('SharpMap.Presentation.Web.SharpLayers.ComponentBase', Sys.Component);
SharpMap.Presentation.Web.SharpLayers.ComponentBase.buildViaServer = function(componentId) {
    var component = $find(componentId);
    if (component == null)
        throw "Component '" + componentId + "' not found.";
    component.buildObject();
}