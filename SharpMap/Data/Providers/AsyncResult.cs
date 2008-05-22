// Copyright 2008: Ron Emmert (justsome.handle@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 


using System;
using System.Threading;

namespace SharpMap.Data.Providers
{
    public class AsyncResult : IAsyncResult
    {
        [Flags]
        private enum State
        {
            Pending = 0, 
            Completed = 1,
            Synchronous = 2
        }

        private readonly AsyncCallback _taskCompleted;
        private readonly Object _asyncState;

        private Int32 _taskCompletionState = (Int32)State.Pending;
        private ManualResetEvent _waitHandle;
        private Exception _terminatingException;

        #region task management methods for use by long running task

        /// <summary>
        /// Create result object for task instance
        /// </summary>
        /// <param name="asyncCallback">notification method when task complete</param>
        /// <param name="state">task specific state object</param>
        public AsyncResult(AsyncCallback asyncCallback, Object state)
        {
            _taskCompleted = asyncCallback;
            _asyncState = state;
        }


        /// <summary>
        /// Task marks self as completed successfully asynchronously
        /// </summary>
        public void SetComplete()
        {
            SetComplete(false, null);
        }

        /// <summary>
        /// Task marks self as completed, any outcome
        /// </summary>
        /// <param name="completedSynchronously">true if task completed synchronously</param>
        /// <param name="terminatingException">exception terminating task if any</param>
        public void SetComplete(Boolean completedSynchronously, Exception terminatingException)
        {
            _terminatingException = terminatingException;

            Int32 prevState = Interlocked.Exchange(ref _taskCompletionState,
                                                   (Int32) (completedSynchronously
                                                                ? (State.Completed | State.Synchronous)
                                                                : State.Completed));
            if ((State)prevState != State.Pending)
            {
                throw new InvalidOperationException("The task was completed previously.");
            }

            if (_waitHandle != null)
            {
                _waitHandle.Set();
            }

            onTaskCompleted();
        }


        /// <summary>
        /// Task EndXXX method called
        /// </summary>
        public void EndInvoke()
        {
            if (!IsCompleted)
            {
                AsyncWaitHandle.WaitOne();
                AsyncWaitHandle.Close();
                _waitHandle = null;
            }

            if (_terminatingException != null)
            {
                throw _terminatingException;
            }
        }
        
        #endregion


        #region IAsyncResult

        public Object AsyncState
        {
             get { return _asyncState; }
        }

        public Boolean CompletedSynchronously
        {
            get
            {
                return ((State)_taskCompletionState & State.Synchronous) > 0;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (_waitHandle == null)
                {
                    Boolean taskAlreadyComplete = IsCompleted;
                    ManualResetEvent newWaitHandle = new ManualResetEvent(taskAlreadyComplete);

                    if (Interlocked.CompareExchange(ref _waitHandle, newWaitHandle, null) != null)
                    {
                        newWaitHandle.Close();
                    }
                    else
                    {
                        if (!taskAlreadyComplete && IsCompleted)
                        {
                            _waitHandle.Set();
                        }
                    }
                }
                return _waitHandle;
            }
        }

        public Boolean IsCompleted
        {
            get
            {
                return (State)_taskCompletionState != State.Pending;
            }
        }

        #endregion

        #region private helper methods

        private void onTaskCompleted()
        {
            AsyncCallback taskCompleted = _taskCompleted;
            if (taskCompleted != null)
            {
                _taskCompleted(this);
            }
        }

        #endregion

    }
}