using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGG;

namespace AGGSharp.Drawing.LineStyle
{
    public enum CapStyle
    {
        Butt = math_stroke.line_cap_e.butt_cap,
        Square = math_stroke.line_cap_e.square_cap,
        Round = math_stroke.line_cap_e.round_cap
    }

    public enum JoinStyle
    {
        Miter = math_stroke.line_join_e.miter_join,
        MiterRevert = math_stroke.line_join_e.miter_join_revert,
        Round = math_stroke.line_join_e.round_join,
        MiterRound = math_stroke.line_join_e.miter_join_round,
        Bevel = math_stroke.line_join_e.bevel_join
    }

    public enum InnerJoinStyle
    {
        Bevel = math_stroke.inner_join_e.inner_bevel,
        Miter = math_stroke.inner_join_e.inner_miter,
        Round = math_stroke.inner_join_e.inner_round,
        Jag = math_stroke.inner_join_e.inner_jag
    }
}
