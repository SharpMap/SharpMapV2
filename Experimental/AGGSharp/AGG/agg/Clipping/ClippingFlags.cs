
namespace AGG.Clipping
{
    internal enum ClippingFlags
    {
        X1Clipped = 4,
        X2Clipped = 1,
        Y1Clipped = 8,
        Y2Clipped = 2,
        XClipped = X1Clipped | X2Clipped,
        YClipped = Y1Clipped | Y2Clipped
    };
}
