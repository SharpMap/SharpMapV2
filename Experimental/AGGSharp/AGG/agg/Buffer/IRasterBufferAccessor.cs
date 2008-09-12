using AGG.PixelFormat;

namespace AGG.Buffer
{
    public interface IRasterBufferAccessor
    {
        unsafe byte* span(int x, int y, uint len);
        unsafe byte* next_x();
        unsafe byte* next_y();

        IPixelFormat PixelFormat
        {
            get;
        }
    };
}
