using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGGSharp.Drawing.LineStyle;
using AGG;

namespace AGGSharp.Drawing.Interface
{
    internal interface ICalculatejoin
    {
        math_stroke UnderlyingMathStroke { get; }
        void CalculateJoin(IVertexDest vc, vertex_dist v0, vertex_dist v1, vertex_dist v2, double len1, double len2);
        void CalculateCap(IVertexDest vc, vertex_dist v0, vertex_dist v1, double len);
    }

    public interface IStroke
    {
        CapStyle CapStyle { get; set; }
        JoinStyle JoinStyle { get; set; }
        InnerJoinStyle InnerJoinStyle { get; set; }
        double LineWidth { get; set; }
        double MitreLimit { get; set; }
        double InnerMitreLimit { get; set; }
        double ApproximationScale { get; set; }
        double MitreLimitMaxAngle { set; }
    }
}
