using System;
using SharpMap.Presentation.Views;

namespace SharpMap.Rendering.Rasterize
{
    public abstract class RasterizeSurface<TSurface, TContext>
        : IRasterizeSurface<TSurface, TContext>, IDisposable
    {
        protected readonly object _backLock = new object();
        protected readonly object _frontLock = new object();

        private TSurface _backSurface;
        private TSurface _frontSurface;

        protected RasterizeSurface(IMapView2D view)
        {
            MapView = view;
        }

        #region IRasterizeSurface<TSurface,TContext> Members

        public virtual TSurface BackSurface
        {
            get
            {
                lock (_backLock)
                    return _backSurface;
            }
            protected set
            {
                lock (_backLock)
                {
                    if (_backSurface is IDisposable)
                        ((IDisposable)_backSurface).Dispose();

                    _backSurface = value;
                }
            }
        }

        public virtual TSurface FrontSurface
        {
            get
            {
                lock (_frontLock)
                    return _frontSurface;
            }
            protected set
            {
                lock (_frontLock)
                {
                    if (_frontSurface is IDisposable)
                        ((IDisposable)_frontSurface).Dispose();

                    _frontSurface = value;
                }
            }
        }

        public virtual IRasterizers<TSurface, TContext> RetrieveSurface()
        {
            if (Equals(BackSurface, default(TSurface)))
                return CreateSurface();

            return CreateRasterizers(BackSurface, CreateExistingContext(BackSurface));
        }


        public virtual IRasterizers<TSurface, TContext> CreateSurface()
        {
            BackSurface = CreateSurfaceInternal();
            TContext context = CreateNewContextInternal(BackSurface);
            return CreateRasterizers(BackSurface, context);
        }


        public IMapView2D MapView { get; protected set; }

        public event EventHandler RenderComplete;

        public virtual void RenderCompleted()
        {
            FrontSurface = BackSurface;
            MapView.Display(FrontSurface);
            OnRenderComplete();
        }

        protected void OnRenderComplete()
        {
            if (RenderComplete != null)
                RenderComplete(this, EventArgs.Empty);
        }

        object IRasterizeSurface.BackSurface
        {
            get { return BackSurface; }
        }

        object IRasterizeSurface.FrontSurface
        {
            get { return FrontSurface; }
        }

        IRasterizers IRasterizeSurface.RetrieveSurface()
        {
            return RetrieveSurface();
        }

        IRasterizers IRasterizeSurface.CreateSurface()
        {
            return CreateSurface();
        }

        #endregion

        protected abstract TContext CreateNewContextInternal(TSurface surface);

        protected abstract TSurface CreateSurfaceInternal();


        protected abstract TContext CreateExistingContext(TSurface surface);

        protected abstract IRasterizers<TSurface, TContext> CreateRasterizers(TSurface surface, TContext context);

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed;
        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            disposed = true;

            FrontSurface = default(TSurface);
            BackSurface = default(TSurface);

        }

        ~RasterizeSurface()
        {
            Dispose(false);
        }
        #endregion
    }
}