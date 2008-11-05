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
using System.Web.UI;

namespace SharpMap.Presentation.Web.SharpLayers.Protocol
{
    public interface IProtocolComponent : IScriptControl
    {
        string ID { get; set; }
        IProtocolBuilderParams BuilderParams { get; set; }
    }

    public interface IProtocolComponent<TBuilderParams> : IProtocolComponent
        where TBuilderParams : IProtocolBuilderParams
    {
        new TBuilderParams BuilderParams { get; set; }
    }
}