namespace SharpMap.Rendering.Rasterize
{
    /* TODO needs a new name badly*/
    public interface IRasterRasterizer : IRasterizer
    {
    }

    public interface IRasterRasterizer<TSurface, TContext> : IRasterizer<TSurface, TContext>, IRasterRasterizer
    {
    }
}