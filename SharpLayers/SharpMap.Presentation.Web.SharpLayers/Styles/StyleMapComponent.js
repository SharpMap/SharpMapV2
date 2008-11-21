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

SharpMap.Presentation.Web.SharpLayers.Styles.StyleMapComponent = function() {
    SharpMap.Presentation.Web.SharpLayers.Styles.StyleMapComponent.initializeBase(this);
    this._builderDelegate = Function.createDelegate(this, this._buildStyleMapDelegate);
    this._styleMapInitDone = false;
    this._sldDoc = null;

}
SharpMap.Presentation.Web.SharpLayers.Styles.StyleMapComponent.prototype = {
    initialize: function() {
        SharpMap.Presentation.Web.SharpLayers.Styles.StyleMapComponent.callBaseMethod(this, 'initialize');
    },

    dispose: function() {
        SharpMap.Presentation.Web.SharpLayers.Styles.StyleMapComponent.callBaseMethod(this, 'dispose');
    },
    set_builderParams: function(params) {
        SharpMap.Presentation.Web.SharpLayers.Styles.StyleMapComponent.callBaseMethod(this, 'set_builderParams', [params]);
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
        var sld = SharpMap.Presentation.Web.SharpLayers.Styles.StyleMapComponent.sldFormat.read(doc);
        var opts = this.get_builderParams();
        delete opts["sldDocumentUri"];
        delete opts["sldDocumentXml"];

        if (!this._styleMapInitDone)
            this.get_hostedItem().initialize(sld, opts);
    },

    _buildStyleMapDelegate: function() {

        var sty = null;
        var opts = null;
        if (this._sldDoc != null) {
            this._styleMapInitDone = true;
            sty = SharpMap.Presentation.Web.SharpLayers.Styles.StyleMapComponent.sldFormat.read(doc);
            opts = this.get_builderParams();
            delete opts["sldDocumentUri"];
            delete opts["sldDocumentXml"];
        }
        return new OpenLayers.StyleMap(sty, opts);
    }


}
SharpMap.Presentation.Web.SharpLayers.Styles.StyleMapComponent.sldFormat = new OpenLayers.Format.SLD();

SharpMap.Presentation.Web.SharpLayers.Styles.StyleMapComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Styles.StyleMapComponent', SharpMap.Presentation.Web.SharpLayers.ComponentBase);
