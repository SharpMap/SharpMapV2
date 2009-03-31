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
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Styles');

SharpMap.Presentation.Web.SharpLayers.Styles.SldComponent = function() {
    SharpMap.Presentation.Web.SharpLayers.Styles.SldComponent.initializeBase(this);
    this._builderDelegate = Function.createDelegate(this, this._buildStyleMapDelegate);
    this._sldInitDone = false;

}
SharpMap.Presentation.Web.SharpLayers.Styles.SldComponent.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Styles.SldComponent.callBaseMethod(this, 'initialize');
    },

    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Styles.SldComponent.callBaseMethod(this, 'dispose');
    },
    set_builderParams: function(params) {
        SharpMap.Presentation.Web.SharpLayers.Styles.SldComponent.callBaseMethod(this, 'set_builderParams', [params]);
        if (params["sldDocumentUri"] != null) {
            var uri = params.sldDocumentUri;
            OpenLayers.loadURL(uri, null, null, Function.createDelegate(this, this._processRequest));

        }
        else if (params["sldDocumentXml"] != null) {
            this._processXml(params["sldDocumentXml"]);
        }
    },
    _processRequest: function(req) {
        this._sldDoc = req.responseXML || req.responseText;
        this._processXml(this._sldDoc);
    },

    _processXml: function(doc) {
        var sld = SharpMap.Presentation.Web.SharpLayers.Styles.SldComponent.sldFormat.read(doc);
        var opts = this.get_builderParams();
        delete opts["sldDocumentUri"];
        delete opts["sldDocumentXml"];

        this.set_hostedItem(sld);

    },

    _buildStyleMapDelegate: function() {
        return this.get_hostedItem();
    },

    set_hostedItem: function(val) {
        var b = this._hostedItem == val;
        this._hostedItem = val;
        if (!b)
            this._raiseSldLoaded();
    },
    buildObject: function() {

        var regItem = new $olRegItem(this.get_id(), {}, "SldComponent");
        //add the registry item to the registry
        $olRegistry.add(regItem);
        //create a reference to the hosted item
    },
    _raiseSldLoaded: function() {
        this._sldInitDone = true;
        var h = this.get_events().getHandler('sldloaded');
        if (h)
            h(this, Sys.EventArgs.Empty);
    },
    get_initDone: function() {
        return this._sldInitDone;
    },

    remove_sldLoaded: function(handler) {
        debugger;
        this.get_events().removeHandler("sldloaded", handler);
    }



}
SharpMap.Presentation.Web.SharpLayers.Styles.SldComponent.sldFormat = new OpenLayers.Format.SLD();

SharpMap.Presentation.Web.SharpLayers.Styles.SldComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Styles.SldComponent', SharpMap.Presentation.Web.SharpLayers.ComponentBase);
