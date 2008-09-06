using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AGG;
using AGGSharp.Drawing.LineStyle;
using AGGSharp.Drawing.Interface;

namespace AGGSharp.Drawing
{
    public class Stroke
        : IStroke, ICalculatejoin
    {
        private readonly math_stroke _innerstroke;
        public math_stroke UnderlyingMathStroke
        {
            get
            {
                return _innerstroke;
            }
        }

        public Stroke()
        {
            _innerstroke = new math_stroke();
        }

        #region IStroke Members

        public CapStyle CapStyle
        {
            get
            {
                return (CapStyle)_innerstroke.line_cap();
            }
            set
            {
                _innerstroke.line_cap((math_stroke.line_cap_e)value);
            }
        }

        public JoinStyle JoinStyle
        {
            get
            {
                return (JoinStyle)_innerstroke.line_join();
            }
            set
            {
                _innerstroke.line_join((math_stroke.line_join_e)value);
            }
        }

        public InnerJoinStyle InnerJoinStyle
        {
            get
            {
                return (InnerJoinStyle)_innerstroke.inner_join();
            }
            set
            {
                _innerstroke.inner_join((math_stroke.inner_join_e)value);
            }
        }

        public double LineWidth
        {
            get
            {
                return _innerstroke.width();
            }
            set
            {
                _innerstroke.width(value);
            }
        }

        public double MitreLimit
        {
            get
            {
                return _innerstroke.miter_limit();
            }
            set
            {
                _innerstroke.miter_limit(value);
            }
        }

        public double InnerMitreLimit
        {
            get
            {
                return _innerstroke.inner_miter_limit();
            }
            set
            {
                _innerstroke.inner_miter_limit(value);
            }
        }

        public double ApproximationScale
        {
            get
            {
                return _innerstroke.approximation_scale();
            }
            set
            {
                _innerstroke.approximation_scale(value);
            }
        }

        public double MitreLimitMaxAngle
        {
            set { _innerstroke.miter_limit_theta(value); }
        }

        #endregion

        #region ICalculatejoin Members

        public void CalculateJoin(IVertexDest vc, vertex_dist v0, vertex_dist v1, vertex_dist v2, double len1, double len2)
        {
            _innerstroke.calc_join(vc, v0, v1, v2, len1, len2);
        }

        public void CalculateCap(IVertexDest vc, vertex_dist v0, vertex_dist v1, double len)
        {
            _innerstroke.calc_cap(vc, v0, v1, len);
        }

        #endregion
    }
}
