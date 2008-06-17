using System;
using System.Collections;
using System.Text;

namespace SharpMap.Diagnostics
{
    class EnumerablePrinter
    {
        public static String Print(IEnumerable enumerable)
        {
            StringBuilder buffer = new StringBuilder();

            foreach (Object value in enumerable)
            {
                buffer.Append(value);
                buffer.Append(',');
                buffer.Append(' ');
            }

            if (buffer.Length >= 2)
            {
                buffer.Length -= 2;   
            }

            return buffer.ToString();
        }
    }
}
