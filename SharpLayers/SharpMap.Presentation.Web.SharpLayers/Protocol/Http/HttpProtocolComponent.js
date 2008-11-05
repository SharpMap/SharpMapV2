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
Type.registerNamespace('SharpMap.Presentation.Web.SharpLayers.Protocol.Http');

SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent = function() {
    SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent.initializeBase(this);
}
SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent.prototype = {
    initialize: function() {
        this._builderDelegate = Function.createDelegate(this, this._protocolBuilderDelegate);
        SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent.callBaseMethod(this, 'initialize');
    },
    dispose: function() {
        delete this._protocolBuilderDelegate;
        SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent.callBaseMethod(this, 'dispose');
    },
    _protocolBuilderDelegate: function() {
        var options = this.get_builderParams();
        var formats = options["formats"];
        options.format = $olFactory.buildOpenLayersObject(formats[0]);
        delete options.formats;
        return new OpenLayers.Protocol.HTTP(options);
    }
}
SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent.registerClass('SharpMap.Presentation.Web.SharpLayers.Protocol.Http.HttpProtocolComponent', SharpMap.Presentation.Web.SharpLayers.Protocol.ProtocolComponent);
