namespace SharpMap.Rendering.Rasterize
{
    public interface IRasterizer
    {
        object Surface { get; }
        object Context { get; }
        void BeginPass();
        void EndPass();
    }


    public interface IRasterizer<TSurface, TContext> : IRasterizer
    {
        TSurface Surface { get; }
        TContext Context { get; }
    }
}