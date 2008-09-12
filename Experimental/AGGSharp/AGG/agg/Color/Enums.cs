
namespace AGG.Color
{
    public static class ColorConstants
    {
        public const int BaseShift = 8;
        public const uint BaseScale = 1u << BaseShift;
        public const uint BaseMask = BaseScale - 1;

        public const int CoverShift = 8;
        public const int CoverSize = 1 << CoverShift;  //----cover_size 
        public const int CoverMask = CoverSize - 1;    //----cover_mask 
    }

    public enum Component { A, R, G, B }
}
