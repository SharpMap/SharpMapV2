/*
 *  The attached / following is part of SharpMap.Presentation.Web.SharpLayers
 *  SharpMap.Presentation.Web.SharpLayers is free software © 2008 Newgrove Consultants Limited, 
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
using System.Web.Script.Serialization;
using System.Web.UI;
using SharpMap.Presentation.Web.SharpLayers.Layers;

namespace SharpMap.Presentation.Web.SharpLayers
{
    public class BuilderParamsJavascriptConverter : JavaScriptConverter
    {
        private Func<string, Control> _findControlDelegate;

        public BuilderParamsJavascriptConverter(Func<string, Control> findControlDelegate)
        {
            _findControlDelegate = findControlDelegate;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new[]
                           {
                               typeof (MapHostBuilderParams), typeof (BuilderParamsBase), typeof (IBuilderParams),
                               typeof (LayerBuilderParamsBase)
                           };
            }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type,
                                           JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            if (Equals(null, obj))
                return null;

            var dictionary = new Dictionary<string, object>();

            Type t = obj.GetType();

            //dictionary.Add("type", t.FullName);
            if (typeof(IClientClass).IsAssignableFrom(t))
            {
                dictionary.Add("typeToBuild", ((IClientClass)obj).ClientClassName);
            }


            IEnumerable<PropertyInfo> propertyInfos =
                t.GetProperties().Where(
                    o => o.GetCustomAttributes(typeof(SharpLayersSerializationAttribute), true).Length > 0);


            foreach (PropertyInfo pi in propertyInfos)
            {
                SharpLayersSerializationAttribute serializationAttribute =
                    (SharpLayersSerializationAttribute)pi.GetCustomAttributes(typeof(SharpLayersSerializationAttribute), true).First();

                string clientName =
                    serializationAttribute.SerializedName;

                Type propType = pi.PropertyType;

                object v = pi.GetValue(obj, null);
                if (Equals(null, v))
                    continue;

                if (propType.IsEnum)
                {
                    dictionary.Add(clientName, Enum.GetName(propType, pi.GetValue(obj, null)));
                }
                else if (propType.IsPrimitive || propType.IsValueType)
                {
                    dictionary.Add(clientName, v);
                }
                else if (propType == typeof(string))
                {
                    var s = (string)v;
                    if (!string.IsNullOrEmpty(s))
                    {
                        if (SharpLayersSerializationFlags.GetComponent
                            == (serializationAttribute.SerializationFlags & SharpLayersSerializationFlags.GetComponent))
                        {
                            dictionary.Add(clientName,
                                            _findControlDelegate(s).ClientID);

                        }
                        //else if (SharpLayersSerializationFlags.GetElement
                        //    == (serializationAttribute.SerializationFlags & SharpLayersSerializationFlags.GetElement))
                        //{
                        //    dictionary.Add(clientName, s);
                        //}
                        else
                        {
                            dictionary.Add(clientName, s);
                        }
                    }
                }
                else if (typeof(IDictionary).IsAssignableFrom(propType))
                {
                    throw new NotImplementedException();
                }
                else if (typeof(IEnumerable).IsAssignableFrom(propType))
                {
                    var lst = new List<object>();
                    var enumerable = (IEnumerable)v;

                    foreach (object o in enumerable)
                    {
                        if (!Equals(o, null))
                        {
                            Type elType = o.GetType();
                            if (elType.IsPrimitive || elType.IsValueType || elType == typeof(string))
                                lst.Add(o);
                            else if (o as IUICollectionItem != null)
                                lst.Add(((IUICollectionItem)o).GetValue());
                            else
                                lst.Add(Serialize(o, serializer));
                        }
                    }
                    dictionary.Add(clientName, lst);
                }
                else
                {
                    dictionary.Add(clientName, Serialize(v, serializer));
                }
            }

            return dictionary;
        }
    }
}