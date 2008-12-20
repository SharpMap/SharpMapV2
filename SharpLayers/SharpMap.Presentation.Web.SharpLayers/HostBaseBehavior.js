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

SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior = function(element) {
    SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior.initializeBase(this, [element]);
    this._hostedItem = null;
    this._builderDelegate = null;
    this._builderParams = null;

}

SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior.callBaseMethod(this, 'initialize');
        //this.buildObject();
        SharpMap.Presentation.Web.SharpLayers.InitSync.addInit(this);

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
        SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior.callBaseMethod(this, 'dispose');
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
SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior.registerClass('SharpMap.Presentation.Web.SharpLayers.HostBaseBehavior', AjaxControlToolkit.BehaviorBase);
