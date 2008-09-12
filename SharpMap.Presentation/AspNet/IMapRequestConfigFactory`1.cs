/*
 *  The attached / following is part of SharpMap.Presentation.AspNet
 *  SharpMap.Presentation.AspNet is free software © 2008 Newgrove Consultants Limited, 
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
using System.Web;

namespace SharpMap.Presentation.AspNet
{
    public interface IMapRequestConfigFactory
    {
        IMapRequestConfig CreateConfig(HttpContext context);
    }


    /// <summary>
    /// A factory to create concrete instances of TMapRequestConfig from the state of the Web application.
    /// Implementations may use the Url, Session, Databases etc to actually construct the Config.
    /// 
    /// It is anticipated that by creating particular structures for an IMapRequestConfig and associated factory a WMS / WFS 
    /// can be easily created.
    /// </summary>
    /// <typeparam name="TMapRequestConfig">The type of configuration object this factory instance builds</typeparam>
    public interface IMapRequestConfigFactory<TMapRequestConfig>
        : IMapRequestConfigFactory
        where TMapRequestConfig : IMapRequestConfig
    {
        /// <summary>
        /// override this method and use the context and any other sources you like to set up and return a new instance of TMapRequestConfig
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        new TMapRequestConfig CreateConfig(HttpContext context);
    }
}