// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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
using System.ComponentModel;

namespace SharpMap.Presentation.Presenters
{
    /// <summary>
    /// The base presenter for map views.
    /// </summary>
    /// <typeparam name="TView">Type of view to manage.</typeparam>
    public abstract class BasePresenter<TView> : IDisposable
        where TView : class
    {
        private readonly Map _map;
        private readonly TView _view;
        private bool _disposed = false;

        #region Object Construction/Destruction

        /// <summary>
        /// Constructs a new presenter with the given map and view.
        /// </summary>
        /// <param name="map">The map model.</param>
        /// <param name="view">The view to keep synchronized with the model and to accept input from.</param>
        protected BasePresenter(Map map, TView view)
        {
            _map = map;
            Map.PropertyChanged += handleMapPropertyChanged;

            _view = view;
        }

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~BasePresenter()
        {
            Dispose(false);
        }

        #region Dispose Pattern

        #region IDisposable Members

        /// <summary>
        /// Releases all resources deterministically.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            Dispose(true);
            IsDisposed = true;
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// Releases all resources, and removes from finalization 
        /// queue if <paramref name="disposing"/> is true.
        /// </summary>
        /// <param name="disposing">
        /// True if being called deterministically, false if being called from finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }
        }

        /// <summary>
        /// Gets whether this presenter is disposed, and no longer accessible.
        /// </summary>
        public bool IsDisposed
        {
            get { return _disposed; }
            private set
            {
                if (_disposed)
                {
                    return;
                }

                _disposed = true;

                EventHandler e = Disposed;

                if (e != null)
                {
                    e(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Event fired when the presenter is disposed.
        /// </summary>
        public event EventHandler Disposed;

        #endregion

        #endregion

        /// <summary>
        /// The map to present.
        /// </summary>
        /// <remarks>
        /// This is the model which is kept synchronized to the view, 
        /// and which input on the view modifies through this presenter.
        /// </remarks>
        public Map Map
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().ToString());
                }

                return _map;
            }
        }

        /// <summary>
        /// The view used to accept input and keep synchronized with the <see cref="Map">model</see>.
        /// </summary>
        public TView View
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().ToString());
                }

                return _view;
            }
        }

        protected virtual void OnMapPropertyChanged(string propertyName)
        {

        }

        private void handleMapPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnMapPropertyChanged(e.PropertyName);
        }
    }
}