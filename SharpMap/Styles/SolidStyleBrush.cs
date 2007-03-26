using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Styles
{
    public class SolidStyleBrush : StyleBrush
    {
        public SolidStyleBrush(StyleColor color)
            : base(color) { }

        public override string ToString()
        {
            return String.Format("SolidStyleBrush - {0}", Color);
        }
    }
}
