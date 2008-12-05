Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Controls');

SharpMap.Presentation.Web.SharpLayers.Controls.GenericOLControl = function() {
    SharpMap.Presentation.Web.SharpLayers.Controls.GenericOLControl.initializeBase(this);
}
SharpMap.Presentation.Web.SharpLayers.Controls.GenericOLControl.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.GenericOLControl.callBaseMethod(this, 'initialize');
    },

    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Controls.GenericOLControl.callBaseMethod(this, 'dispose');
    },
    _toolBuilderDelegate: function() {
        var opts = this.get_builderParams();
        var openLayersClassName = opts.openLayersClassName;
        return eval('new ' + openLayersClassName + '()');
    },
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
    }
}
SharpMap.Presentation.Web.SharpLayers.Controls.GenericOLControl.registerClass('SharpMap.Presentation.Web.SharpLayers.Controls.GenericOLControl', SharpMap.Presentation.Web.SharpLayers.Controls.ToolBaseComponent);
