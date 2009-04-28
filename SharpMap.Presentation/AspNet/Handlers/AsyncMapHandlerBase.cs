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
using System.Diagnostics;
using System.Threading;
using System.Web;
using SharpMap.Data.Providers;
using SharpMap.Presentation.AspNet.Utility;

namespace SharpMap.Presentation.AspNet.Handlers
{
    public abstract class AsyncMapHandlerBase
        : MapHandlerBase, IHttpAsyncHandler
    {
        #region IHttpAsyncHandler Members

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            Debug.WriteLine(string.Format("Request recieved on thread {0}", Thread.CurrentThread.ManagedThreadId));

            AsyncResult result = new AsyncResult(cb, context);
            ThreadingUtility.QueueWorkItem(ProcessRequestAsync, result);
            return result;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            if (result != null)
                ((AsyncResult) result).EndInvoke();
        }


        public override void ProcessRequest(HttpContext context)
        {
            Debug.WriteLine(string.Format("Request processed on thread {0}", Thread.CurrentThread.ManagedThreadId));

            base.ProcessRequest(context);
        }

        #endregion

        private void ProcessRequestAsync(object asyncResult)
        {
            AsyncResult result = asyncResult as AsyncResult;
            if (result != null)
            {
                HttpContext c = (HttpContext) result.AsyncState;
                try
                {
                    ProcessRequest(c);
                    result.SetComplete();
                }
                catch (Exception ex)
                {
                    result.SetComplete(false, ex);
                }
            }
        }
    }
}