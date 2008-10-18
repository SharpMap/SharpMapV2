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

        public void AddWorkItem(string statusText, Action workItem, Action callback)
        {
            lock (_workQ)
                _workQ.Enqueue(new WorkItem(statusText, workItem, callback));
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
                w.WorkAction();

                if (w.CallBackAction != null)
                {
                    if (_control.InvokeRequired)
                        _control.Invoke(w.CallBackAction);
                    else
                    {
                        w.CallBackAction();
                    }
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
            public WorkItem(string statusText, Action action, Action callback)
            {
                WorkAction = action;
                CallBackAction = callback;
                StatusText = statusText;
            }

            public string StatusText { get; protected set; }
            public Action WorkAction { get; protected set; }
            public Action CallBackAction { get; protected set; }
        }

        #endregion
    }
}