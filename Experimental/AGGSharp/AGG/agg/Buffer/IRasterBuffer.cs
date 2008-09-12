
namespace AGG.Buffer
{
    public interface IRasterBuffer
    {
        uint BitsPerPixel
        {
            get;
        }

        unsafe byte* GetBuffer();

        uint Width { get; }
        uint Height { get; }
        int StrideInBytes { get; }

        uint StrideInBytesAbs { get; }

        unsafe byte* GetPixelPointer(int x, int y);

        unsafe byte* GetPixelPointer(int y);

        void CopyFrom(IRasterBuffer src);
    };
}
