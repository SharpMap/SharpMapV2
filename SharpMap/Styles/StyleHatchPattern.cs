namespace SharpMap.Styles
{
    /// <summary>
    /// Hatch styles enum
    /// </summary>
    public enum StyleHatchPattern
    {
        /// <summary>
        /// Specifies the default hatch style, which is currently set to <see cref="Horizontal"/>
        /// </summary>
        Default = 0,

        /// <summary>
        /// A pattern of horizontal lines
        /// </summary>
        Horizontal = 0,

        /// <summary>
        /// A pattern of vertical lines
        /// </summary>
        Vertical = 1,

        /// <summary>
        /// A pattern of diagonal lines from left top to right bottom
        /// </summary>
        ForwardDiagonal = 2,

        /// <summary>
        /// A pattern of diagonal lines from right top to left bottom
        /// </summary>
        BackwardDiagonal = 3,

        /// <summary>
        /// A pattern of crossing horizontal and vertical lines
        /// </summary>
        Cross = 4,

        /// <summary>
        /// A pattern of crossing diagonal lines
        /// </summary>
        DiagonalCross = 5,

        /// <summary>
        /// A pattern of 5 percent. The ratio <see cref="HatchStyleBrush.Color"/> to <see cref="HatchStyleBrush.BackColor"/> is 5:100
        /// </summary>
        Percent05 = 6,

        /// <summary>
        /// A pattern of 10 percent. The ratio <see cref="HatchStyleBrush.Color"/> to <see cref="HatchStyleBrush.BackColor"/> is 10:100
        /// </summary>
        Percent10 = 7,

        /// <summary>
        /// A pattern of 20 percent. The ratio <see cref="HatchStyleBrush.Color"/> to <see cref="HatchStyleBrush.BackColor"/> is 20:100
        /// </summary>
        Percent20 = 8,

        /// <summary>
        /// A pattern of 25 percent. The ratio <see cref="HatchStyleBrush.Color"/> to <see cref="HatchStyleBrush.BackColor"/> is 25:100
        /// </summary>
        Percent25 = 9,

        /// <summary>
        /// A pattern of 30 percent. The ratio <see cref="HatchStyleBrush.Color"/> to <see cref="HatchStyleBrush.BackColor"/> is 30:100
        /// </summary>
        Percent30 = 10,

        /// <summary>
        /// A pattern of 40 percent. The ratio <see cref="HatchStyleBrush.Color"/> to <see cref="HatchStyleBrush.BackColor"/> is 40:100
        /// </summary>
        Percent40 = 11,

        /// <summary>
        /// A pattern of 50 percent. The ratio <see cref="HatchStyleBrush.Color"/> to <see cref="HatchStyleBrush.BackColor"/> is 50:100
        /// </summary>
        Percent50 = 12,

        /// <summary>
        /// A pattern of 60 percent. The ratio <see cref="HatchStyleBrush.Color"/> to <see cref="HatchStyleBrush.BackColor"/> is 60:100
        /// </summary>
        Percent60 = 13,

        /// <summary>
        /// A pattern of 70 percent. The ratio <see cref="HatchStyleBrush.Color"/> to <see cref="HatchStyleBrush.BackColor"/> is 70:100
        /// </summary>
        Percent70 = 14,

        /// <summary>
        /// A pattern of 75 percent. The ratio <see cref="HatchStyleBrush.Color"/> to <see cref="HatchStyleBrush.BackColor"/> is 75:100
        /// </summary>
        Percent75 = 15,

        /// <summary>
        /// A pattern of 80 percent. The ratio <see cref="HatchStyleBrush.Color"/> to <see cref="HatchStyleBrush.BackColor"/> is 80:100
        /// </summary>
        Percent80 = 16,

        /// <summary>
        /// A pattern of 90 percent. The ratio <see cref="HatchStyleBrush.Color"/> to <see cref="HatchStyleBrush.BackColor"/> is 90:100
        /// </summary>
        Percent90 = 17,

        /// <summary>
        /// A pattern with diagonal lines from left top to right bottom. The lines distance is 50 percent of <see cref="ForwardDiagonal"/>.
        /// </summary>
        LightDownwardDiagonal = 18,

        /// <summary>
        /// A pattern with diagonal lines from right top to left bottom. The lines distance is 50 percent of <see cref="BackwardDiagonal"/>.
        /// </summary>
        LightUpwardDiagonal = 19,

        /// <summary>
        /// A pattern with diagonal lines from left top to right bottom. The lines distance is 50 percent of <see cref="ForwardDiagonal"/>, but double density.
        /// </summary>
        DarkDownwardDiagonal = 20,

        /// <summary>
        /// A pattern with diagonal lines from right top to left bottom. The lines distance is 50 percent of <see cref="BackwardDiagonal"/>, but double density.
        /// </summary>
        DarkUpwardDiagonal = 21,

        /// <summary>
        /// A pattern with diagonal lines from left top to right bottom. The lines distance is same as <see cref="ForwardDiagonal"/>, but triple density.
        /// </summary>
        WideDownwardDiagonal = 22,

        /// <summary>
        /// A pattern with diagonal lines from right top to left bottom. The lines distance is same as <see cref="BackwardDiagonal"/>, but triple density.
        /// </summary>
        WideUpwardDiagonal = 23,

        /// <summary>
        /// A pattern with vertical lines. The distance is 50 percent less than <see cref="Vertical"/>
        /// </summary>
        LightVertical = 24,

        /// <summary>
        /// A pattern with hoizontal lines. The distance is 50 percent less than <see cref="Horizontal"/>
        /// </summary>
        LightHorizontal = 25,

        /// <summary>
        /// A pattern with vertical lines. The distance is 75 percent less than <see cref="Vertical"/>
        /// </summary>
        NarrowVertical = 26,

        /// <summary>
        /// A pattern with horizontal lines. The distance is 75 percent less than <see cref="Horizontal"/>
        /// </summary>
        NarrowHorizontal = 27,

        /// <summary>
        /// A pattern with vertical lines. The distance is 50 percent less than <see cref="Vertical"/>. The density is double
        /// </summary>
        DarkVertical = 28,

        /// <summary>
        /// A pattern with horizontal lines. The distance is 50 percent less than <see cref="Horizontal"/>. The density is double
        /// </summary>
        DarkHorizontal = 29,

        /// <summary>
        /// A pattern like <see cref="ForwardDiagonal"/> but dotted.
        /// </summary>
        DashedDownwardDiagonal = 30,

        /// <summary>
        /// A pattern like <see cref="BackwardDiagonal"/>, but dotted.
        /// </summary>
        DashedUpwardDiagonal = 31,

        /// <summary>
        /// A pattern like <see cref="Horizontal"/> but dashed.
        /// </summary>
        DashedHorizontal = 32,

        /// <summary>
        /// A pattern like <see cref="Vertical"/> but dashed.
        /// </summary>
        DashedVertical = 33,

        /// <summary>
        /// A small confetti pattern
        /// </summary>
        SmallConfetti = 34,
        
        /// <summary>
        /// A large confetti pattern.
        /// </summary>
        LargeConfetti = 35,

        /// <summary>
        /// A zig-zag pattern
        /// </summary>
        ZigZag = 36,

        /// <summary>
        /// A wave pattern
        /// </summary>
        Wave = 37,

        /// <summary>
        /// A diagonal brick pattern 
        /// </summary>
        DiagonalBrick = 38,

        /// <summary>
        /// A horizontal brick pattern
        /// </summary>
        HorizontalBrick = 39,

        /// <summary>
        /// A weave pattern
        /// </summary>
        Weave = 40,

        /// <summary>
        /// A plaid pattern
        /// </summary>
        Plaid = 41,

        /// <summary>
        /// A pattern of divots
        /// </summary>
        Divot = 42,

        /// <summary>
        /// A pattern of dotted crossing vertical an horizontal lines
        /// </summary>
        DottedGrid = 43,

        /// <summary>
        /// A pattern of crossing diagonal dotted lines.
        /// </summary>
        DottedDiamond = 44,

        /// <summary>
        /// A pattern of overlapping shingles
        /// </summary>
        Shingle = 45,

        /// <summary>
        /// A grid pattern
        /// </summary>
        Trellis = 46,

        /// <summary>
        /// A pattern of touching circles
        /// </summary>
        Sphere = 47,

        /// <summary>
        /// A pattern with crossing vertical and horizontal lines. The lines distance is 50 percent of <see cref="Cross"/>
        /// </summary>
        SmallGrid = 48,

        /// <summary>
        /// A pattern that looks like a checker board.
        /// </summary>
        SmallCheckerBoard = 49,

        /// <summary>
        /// A pattern that looks like a checker board. The rectangles have twice the size as in <see cref="SmallCheckerBoard"/>.
        /// </summary>
        LargeCheckerBoard = 50,

        /// <summary>
        /// A pattern with crossing diagonal lines, not antialiased.
        /// </summary>
        OutlinedDiamond = 51,

        /// <summary>
        /// A pattern that looks like a diagonal aligned checker board
        /// </summary>
        SolidDiamond = 52,
    }
}