// README
//
// There are two steps to adding a property:
//
// 1. Create a member variable to store your property
// 2. Add the get_ and set_ accessors for your property
//
// Remember that both are case sensitive!


/// <reference name="MicrosoftAjaxTimer.debug.js" />
/// <reference name="MicrosoftAjaxWebForms.debug.js" />
/// <reference name="AjaxControlToolkit.ExtenderBase.BaseScripts.js" assembly="AjaxControlToolkit" />


Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers');

SharpMap.Presentation.Web.SharpLayers.MapHostBehavior = function(element) {
    SharpMap.Presentation.Web.SharpLayers.MapHostBehavior.initializeBase(this, [element]);



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


    _mapBuilderDelegate: function() {
        var options = this.get_builderParams();
        return new OpenLayers.Map(this.get_element(), options);
    }

}
SharpMap.Presentation.Web.SharpLayers.MapHostBehavior.registerClass('SharpMap.Presentation.Web.SharpLayers.MapHostBehavior', SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior);
