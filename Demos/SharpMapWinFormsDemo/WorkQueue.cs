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
        private readonly Queue<WorkQueueItem> _queue = new Queue<WorkQueueItem>();
        private readonly BackgroundWorker _worker = new BackgroundWorker();

        public WorkQueue(Control control)
        {
            _control = control;
            _worker.DoWork += _worker_DoWork;
        }

        private void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (_queue.Count > 0)
            {
                WorkQueueItem item;
                lock (_queue)
                    item = _queue.Dequeue();

                OnWorking(item.StatusText);
                item.WorkItem();

                if (item.Callback != null)
                {
                    if (_control.InvokeRequired)
                        _control.Invoke(item.Callback);
                    else
                        item.Callback();
                }
            }

            OnWorking("Finished");
        }

        private void OnWorking(string p)
        {
            if (Working != null)
                Working(this, new WorkQueueProcessEventArgs(p));
        }

        public void AddWorkItem(string statusText, Action work, Action callback)
        {
            lock (_queue)
                _queue.Enqueue(new WorkQueueItem(statusText, work, callback));

            if (!_worker.IsBusy)
                _worker.RunWorkerAsync();
        }


        public event EventHandler<WorkQueueProcessEventArgs> Working;

        #region Nested type: WorkQueueItem

        private class WorkQueueItem
        {
            public WorkQueueItem(string status, Action work, Action callback)
            {
                StatusText = status;
                WorkItem = work;
                Callback = callback;
            }

            public string StatusText { get; protected set; }

            public Action WorkItem { get; protected set; }

            public Action Callback { get; set; }
        }

        #endregion
    }
}