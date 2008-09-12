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
using System;
using System.Threading;

namespace SharpMap.Presentation.AspNet.Utility
{
    internal static class ThreadingUtility
    {
#warning arrange a better threadpool for AsyncHttpMapHandler
        /// <remarks>
        /// by default ThreadPool.QueueUserWorkItem is using the same threads as the IIS worker thread pool 
        /// -  hence multithreading uses up the same thread pool faster.
        /// 
        /// TODO: use a different mechanism for thread pooling.
        /// </remarks>
        public static void QueueWorkItem(Action<Object> action, object value)
        {
            ///uncomment the following 3 lines and comment the ThreadPool.QueueUserWorkItem(new WaitCallback(action), value); line to 
            ///see that the request is being processed on a different thread. 
            ///This will *not* scale well so use it for *investigative* purposes only


            //ParameterizedThreadStart ts = new ParameterizedThreadStart(action);
            //Thread t = new Thread(ts);
            //t.Start(value);

            ThreadPool.QueueUserWorkItem(new WaitCallback(action), value);
        }
    }
}