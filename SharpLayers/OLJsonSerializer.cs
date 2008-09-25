/*
 *  The attached / following is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SharpLayers
{
    /// <summary>
    /// todo:add some dynamic methods to reduce reflection overhead
    /// </summary>
    public static class OLJsonSerializer
    {
        public static string Serialize(object obj)
        {
            if (!typeof (ISerializeToOLJson).IsAssignableFrom(obj.GetType()))
                throw new InvalidOperationException();

            bool commaRequired = false;

            var sb = new StringBuilder();

            sb.Append("{");

            IEnumerable<PropertyInfo> pinfos = obj.GetType()
                .GetProperties()
                .Where(o => o.GetCustomAttributes(typeof (OLJsonSerializationAttribute), true).Length > 0);

            foreach (PropertyInfo pi in pinfos)
            {
                OLJsonSerializationAttribute attr =
                    pi.GetCustomAttributes(typeof (OLJsonSerializationAttribute), true)
                        .Cast<OLJsonSerializationAttribute>().First();

                object o = pi.GetValue(obj, null);

                if (o == null || (o is string && string.IsNullOrEmpty((string) o)))
                    continue;


                if (commaRequired)
                    sb.Append(",");
                sb.AppendFormat("\"{0}\":", attr.SerializedName);

                string val;

                if (o as IOlJsNewable != null)
                {
                    val = (o as IOlJsNewable).GetNewString();
                }
                else if (o as ISerializeToOLJson != null)
                {
                    val = Serialize(o);
                    if (OLSerializationFlags.CreateOLClass ==
                        (OLSerializationFlags.CreateOLClass & attr.SerializationFlags))
                    {
                        val = string.Format(" $create({0}, {1})", ((IOLClass) o).JsClass, val);
                    }
                }
                else
                    val = SerializeValue(o);


                sb.Append(Format(val, attr.SerializationFlags));

                commaRequired = true;
            }


            sb.Append("}");

            return sb.ToString();
        }

        private static string Format(string val, OLSerializationFlags oLSerializationFlags)
        {
            if (OLSerializationFlags.GetElement == (OLSerializationFlags.GetElement & oLSerializationFlags))
            {
                return string.Format("$get({0})", val);
            }

            if (OLSerializationFlags.FindOLComponent == (OLSerializationFlags.FindOLComponent & oLSerializationFlags))
            {
                return string.Format("$find({0})", val);
            }

            ///TODO

            return val;
        }

        private static string SerializeValue(object o)
        {
            if (o is ICollection)
                return SerializeCollection((ICollection) o);

            Type t = o.GetType();

            if (t.IsEnum)
                return string.Format("\"{0}\"", Enum.GetName(t, o));


            if (t == typeof (string))
                return string.Format("\"{0}\"", o);

            if (o is bool)
                return string.Format("{0}", o).ToLower();

            //if (o is double || o is float || o is int || o is long || o is short)
            return string.Format("{0}", o);
        }

        internal static string SerializeCollection(ICollection iCollection)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            if (iCollection.Count > 0)
            {
                foreach (object o in iCollection)
                {
                    sb.Append(SerializeValue(o));
                    sb.Append(",");
                }
                sb.Remove(sb.Length - 1, 1);
            }

            sb.Append("]");
            return sb.ToString();
        }
    }
}