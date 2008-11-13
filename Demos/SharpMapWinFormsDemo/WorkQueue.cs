using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace MapViewer
{
    public class WorkQueueProcessEventArgs : EventArgs
    {
        public WorkQueueProcessEventArgs(string status)
        {
            Status = status;
        }

        public string Status { get; protected set; }
    }

    public class WorkQueue
    {
        private readonly Control _control;
        private readonly BackgroundWorker _worker = new BackgroundWorker();
        private readonly Queue<WorkItem> _workQ = new Queue<WorkItem>();

        public WorkQueue(Control control)
        {
            _control = control;
            _worker.DoWork += _worker_DoWork;
        }

        public event EventHandler<WorkQueueProcessEventArgs> Working;

        public void AddWorkItem(string statusText, Action workItem, Action successCallback, Action<Exception> errorCallback)
        {
            lock (_workQ)
                _workQ.Enqueue(new WorkItem(statusText, workItem, successCallback, errorCallback));
            if (!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (_workQ.Count > 0)
            {
                WorkItem w;
                lock (_workQ)
                    w = _workQ.Dequeue();
                OnProcessingItem(w.StatusText);

                Exception ex = null;
                try
                {
                    w.WorkAction();
                }
                catch (Exception exception)
                {
                    ex = exception;
                }

                if (ex != null)
                {
                    if (w.ErrorCallbackHandler != null)
                    {
                        if (_control.InvokeRequired)
                            _control.Invoke(w.ErrorCallbackHandler, ex);
                        else
                            w.ErrorCallbackHandler(ex);
                    }
                }

                else if (w.SuccessCallbackAction != null)
                {
                    if (_control.InvokeRequired)
                        _control.Invoke(w.SuccessCallbackAction);
                    else
                        w.SuccessCallbackAction();
                }
            }

            OnProcessingItem("Finished Loading Data");
        }

        private void OnProcessingItem(string p)
        {
            if (Working != null)
                Working(this, new WorkQueueProcessEventArgs(p));
        }

        #region Nested type: WorkItem

        private class WorkItem
        {
            public WorkItem(string statusText, Action action, Action successCallback, Action<Exception> errorCallback)
            {
                WorkAction = action;
                SuccessCallbackAction = successCallback;
                StatusText = statusText;
                ErrorCallbackHandler = errorCallback;
            }

            public string StatusText { get; protected set; }
            public Action WorkAction { get; protected set; }
            public Action SuccessCallbackAction { get; protected set; }
            public Action<Exception> ErrorCallbackHandler { get; protected set; }
        }

        #endregion
    }
}