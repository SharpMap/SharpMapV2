using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using AjaxControlToolkit;

namespace SharpMap.Presentation.Web.SharpLayers
{
    public static class SharpLayersScriptObjectBuilder
    {
        private static readonly Dictionary<Type, List<ResourceEntry>> _cache =
            new Dictionary<Type, List<ResourceEntry>>();

        private static readonly Dictionary<Type, IList<string>> _cssCache = new Dictionary<Type, IList<string>>();

        private static readonly Dictionary<Type, Converter<object, string>> _customConverters =
            new Dictionary<Type, Converter<object, string>>();

        private static readonly object _sync = new object();

        /// <summary>
        /// Static constructor to insert standard custom conversions
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline",
            Justification = "Manipulation of CustomConverters can not be moved inline")]
        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
            MessageId = "System.DateTime.ToString(System.String)", Justification = "Avoiding possibly breaking change")]
        static SharpLayersScriptObjectBuilder()
        {
            CustomConverters.Add(typeof(Color), value => ColorTranslator.ToHtml((Color)value));
            Converter<object, string> dateTimeConverter = delegate(object value)
                                                              {
                                                                  var date = (DateTime?)value;
                                                                  return (date != null)
                                                                             ? date.Value.ToUniversalTime().ToString("r")
                                                                             : null;
                                                              };
            CustomConverters.Add(typeof(DateTime), dateTimeConverter);
            CustomConverters.Add(typeof(DateTime?), dateTimeConverter);
        }

        /// <summary>
        /// Mapping of types to delegates that convert objects of that type to
        /// strings
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Type-aware public API")]
        public static IDictionary<Type, Converter<object, string>> CustomConverters
        {
            get { return _customConverters; }
        }

        /// <summary>
        /// Describes an object to a ScriptComponentDescriptor based on its reflected properties and methods
        /// </summary>
        /// <param name="instance">The object to be described</param>
        /// <param name="descriptor">The script descriptor to fill</param>
        /// <param name="urlResolver">The object used to resolve urls</param>
        /// <param name="controlResolver">The object used to resolve control references</param>
        [SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods",
            Justification = "controlResolver is checked against null before being used")]
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters",
            Justification = "Assembly is not localized")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
            Justification = "Cyclomatic complexity issues not currently being addressed")]
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily",
            Justification = "value is assigned/reassigned numerous times - code below favors clarity")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling",
            Justification = "Coupling issues not currently being addressed")]
        public static void DescribeComponent(object instance, ScriptComponentDescriptor descriptor,
                                             IUrlResolutionService urlResolver, IControlResolver controlResolver)
        {
            // validate preconditions
            if (instance == null) throw new ArgumentNullException("instance");
            if (descriptor == null) throw new ArgumentNullException("descriptor");
            if (urlResolver == null) urlResolver = instance as IUrlResolutionService;
            if (controlResolver == null) controlResolver = instance as IControlResolver;

            // describe properties
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(instance);
            foreach (PropertyDescriptor prop in properties)
            {
                ExtenderControlPropertyAttribute propAttr = null;
                ExtenderControlEventAttribute eventAttr = null;
                string propertyName = prop.Name;

                // Try getting a property attribute
                propAttr = (ExtenderControlPropertyAttribute)prop.Attributes[typeof(ExtenderControlPropertyAttribute)];
                if (propAttr == null || !propAttr.IsScriptProperty)
                {
                    // Try getting an event attribute
                    eventAttr = (ExtenderControlEventAttribute)prop.Attributes[typeof(ExtenderControlEventAttribute)];
                    if (eventAttr == null || !eventAttr.IsScriptEvent)
                    {
                        continue;
                    }
                }

                // attempt to rename the property/event
                var nameAttr = (ClientPropertyNameAttribute)prop.Attributes[typeof(ClientPropertyNameAttribute)];
                if (!string.IsNullOrEmpty(nameAttr.PropertyName))
                {
                    propertyName = nameAttr.PropertyName;
                }

                object value = prop.GetValue(instance);
                if (value == null)
                {
                    continue;
                }

                // determine whether to serialize the value of a property.  readOnly properties should always be serialized
                bool serialize = prop.ShouldSerializeValue(instance) || prop.IsReadOnly;
                if (serialize)
                {
                    // get the value of the property, skip if it is null
                    Control c = null;


                    // convert and resolve the value
                    if (eventAttr != null && prop.PropertyType != typeof(String))
                    {
                        throw new InvalidOperationException(
                            "ExtenderControlEventAttribute can only be applied to a property with a PropertyType of System.String.");
                    }
                    else
                    {
                        if (!prop.PropertyType.IsPrimitive && !prop.PropertyType.IsEnum)
                        {
                            // Check if we can use any of our custom converters
                            // (first do a direct lookup on the property type,
                            // but also check all of its base types if nothing
                            // was found)
                            Converter<object, string> customConverter = null;
                            if (!_customConverters.TryGetValue(prop.PropertyType, out customConverter))
                            {
                                foreach (var pair in _customConverters)
                                {
                                    if (prop.PropertyType.IsSubclassOf(pair.Key))
                                    {
                                        customConverter = pair.Value;
                                        break;
                                    }
                                }
                            }

                            // Use the custom converter if found, otherwise use
                            // its current type converter
                            if (customConverter != null)
                            {
                                value = customConverter(value);
                            }
                            else
                            {
                                // Determine if we should let ASP.NET AJAX handle this type of conversion, as it supports JSON serialization
                                if (propAttr != null && propAttr.UseJsonSerialization)
                                {
                                    if (value is IEnumerable)
                                    {
                                        var lst = new List<object>();
                                        foreach (object a in (IEnumerable)value)
                                        {
                                            object b;
                                            Type typ = a.GetType();
                                            if (a as IUICollectionItem != null)
                                                b = a is UriValue
                                                        ? urlResolver.ResolveClientUrl(((UriValue)a).Value.ToString())
                                                        : ((IUICollectionItem)a).Value;
                                            else if (typ.IsPrimitive || typ.IsValueType)
                                                b = a;
                                            else
                                                b = BuildGraph(a, urlResolver, controlResolver);
                                            if (b != null)
                                                lst.Add(b);
                                        }
                                        if (lst.Count > 0)
                                            value = lst;
                                    }
                                    else if (value.GetType().IsPrimitive || value.GetType().IsValueType)
                                    { }
                                    else
                                        value = BuildGraph(value, urlResolver, controlResolver);
                                    // Use ASP.NET JSON serialization
                                }
                                else
                                {
                                    // Use the property's own converter
                                    TypeConverter conv = prop.Converter;
                                    value = conv.ConvertToString(null, CultureInfo.InvariantCulture, value);
                                }
                            }
                        }
                        if (prop.Attributes[typeof(IDReferencePropertyAttribute)] != null && controlResolver != null)
                        {
                            c = controlResolver.ResolveControl((string)value);
                        }
                        if (prop.Attributes[typeof(UrlPropertyAttribute)] != null && urlResolver != null)
                        {
                            value = urlResolver.ResolveClientUrl((string)value);
                        }
                    }

                    // add the value as an appropriate description
                    if (eventAttr != null)
                    {
                        descriptor.AddEvent(propertyName, (string)value);
                    }
                    else if (prop.Attributes[typeof(ElementReferenceAttribute)] != null)
                    {
                        if (c == null && controlResolver != null) c = controlResolver.ResolveControl((string)value);
                        if (c != null) value = c.ClientID;
                        descriptor.AddElementProperty(propertyName, (string)value);
                    }
                    else if (prop.Attributes[typeof(ComponentReferenceAttribute)] != null)
                    {
                        if (c == null && controlResolver != null) c = controlResolver.ResolveControl((string)value);
                        if (c != null)
                        {
                            var ex = c as ExtenderControlBase;
                            if (ex != null && ex.BehaviorID.Length > 0)
                                value = ex.BehaviorID;
                            else
                                value = c.ClientID;
                        }
                        descriptor.AddComponentProperty(propertyName, (string)value);
                    }
                    else
                    {
                        if (c != null) value = c.ClientID;
                        descriptor.AddProperty(propertyName, value);
                    }
                }
            }


            // determine if we should describe methods
            foreach (
                MethodInfo method in
                    instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public))
            {
                var methAttr =
                    (ExtenderControlMethodAttribute)
                    Attribute.GetCustomAttribute(method, typeof(ExtenderControlMethodAttribute));
                if (methAttr == null || !methAttr.IsScriptMethod)
                {
                    continue;
                }

                // We only need to support emitting the callback target and registering the WebForms.js script if there is at least one valid method
                var control = instance as Control;
                if (control != null)
                {
                    // Force WebForms.js
                    control.Page.ClientScript.GetCallbackEventReference(control, null, null, null);

                    // Add the callback target
                    descriptor.AddProperty("_callbackTarget", control.UniqueID);
                }
                break;
            }
        }

        private static T GetAttribute<T>(PropertyDescriptor d) where T : Attribute
        {
            return (T)d.Attributes[typeof(T)];
        }

        private static Dictionary<string, object> BuildGraph(object obj, IUrlResolutionService uriResolver,
                                                             IControlResolver controlResolver)
        {
            var dict = new Dictionary<string, object>();
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(obj);
            foreach (PropertyDescriptor pd in pdc)
            {
                var ecpa = GetAttribute<ExtenderControlPropertyAttribute>(pd);

                var cpna = GetAttribute<ClientPropertyNameAttribute>(pd);
                var idr = GetAttribute<IDReferencePropertyAttribute>(pd);
                var ura = GetAttribute<UrlPropertyAttribute>(pd);
                var ecma = GetAttribute<ExtenderControlMethodAttribute>(pd);
                var ecea = GetAttribute<ExtenderControlEventAttribute>(pd);
                var era = GetAttribute<ElementReferenceAttribute>(pd);
                var cra = GetAttribute<ComponentReferenceAttribute>(pd);
                if (ecpa == null && cra == null && cpna == null && era == null)
                    continue;


                string propName =
                    cpna != null && !string.IsNullOrEmpty(cpna.PropertyName)
                        ? cpna.PropertyName
                        : pd.Name;


                if (propName == "ClientClassName")
                    propName = "typeToBuild";

                object o = null;
                object value = pd.GetValue(obj);
                if (value == null)
                    continue;

                if (value as IClientClass != null && ((IClientClass)value).NotSet)
                    continue;

                if (pd.PropertyType == typeof(string))
                {
                    o = value;
                    if (ura != null)
                    {
                        o = uriResolver.ResolveClientUrl((string)o);
                    }
                    else if (idr != null || cra != null)
                    {
                        o = BuildGraph(new Reference
                                           {
                                               ReferenceType = ReferenceType.Component,
                                               ClientId = controlResolver.ResolveControl((string)o).ClientID
                                           }, uriResolver, controlResolver);
                    }
                    else if (era != null)
                    {
                        Control c = controlResolver.ResolveControl((string)o);
                        o = BuildGraph(
                            new Reference { ReferenceType = ReferenceType.Element, ClientId = c == null ? (string)o : c.ClientID },
                            uriResolver, controlResolver);
                    }
                }
                else if (value is IEnumerable)
                {
                    var lst = new List<object>();
                    foreach (object a in (IEnumerable)value)
                    {
                        object b;
                        if (a as IUICollectionItem != null)
                            b = a is UriValue
                                    ? uriResolver.ResolveClientUrl(((UriValue)a).Value.ToString())
                                    : ((IUICollectionItem)a).Value;
                        else
                            b = BuildGraph(a, uriResolver, controlResolver);
                        if (b != null)
                            lst.Add(b);
                    }
                    if (lst.Count > 0)
                        o = lst;
                }
                else if (pd.PropertyType.IsEnum) o = Enum.GetName(pd.PropertyType, value);
                else if (pd.PropertyType.IsPrimitive || pd.PropertyType.IsValueType) o = value;
                else o = BuildGraph(value, uriResolver, controlResolver);

                if (o != null)
                    dict.Add(propName, o);
            }
            return dict.Count > 0 ? dict : null;
        }


        /// <summary>
        /// Gets the script references for a type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<ScriptReference> GetScriptReferences(Type type)
        {
            return GetScriptReferences(type, false);
        }

        /// <summary>
        /// Gets the ScriptReferences for a Type
        /// </summary>
        /// <param name="type">Type for which references are to be gotten</param>
        /// <param name="ignoreStartingTypeReferences">true if the ClientScriptResource for the starting type is to be ignored</param>
        /// <returns>list of ScriptReferences for the Type</returns>
        public static IEnumerable<ScriptReference> GetScriptReferences(Type type, bool ignoreStartingTypeReferences)
        {
            List<ResourceEntry> entries = GetScriptReferencesInternal(type, new Stack<Type>(),
                                                                      ignoreStartingTypeReferences);

            return ScriptReferencesFromResourceEntries(entries);
        }

        /// <summary>
        /// Gets the embedded css file references for a type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetCssReferences(Control control)
        {
            return GetCssReferences(control, control.GetType(), new Stack<Type>());
        }

        /// <summary>
        /// Register's the Css references for this control
        /// </summary>
        /// <param name="control"></param>
        public static void RegisterCssReferences(Control control)
        {
            // Add the link to the page header instead of inside the body which is not xhtml compliant
            HtmlHead header = control.Page.Header;

            foreach (string styleSheet in ScriptObjectBuilder.GetCssReferences(control))
            {
                if (null == header)
                {
                    // It would be nice to add the required header here, but it's too late in the page
                    // lifecycle to be modifying the Page.Controls collection - throw an informative
                    // exception instead and let the page author make the simple change.
                    throw new NotSupportedException(
                        "This page is missing a HtmlHead control which is required for the CSS stylesheet link that is being added. Please add <head runat=\"server\" />.");
                }

                bool addIt = true; // looking for this stylesheet already in the header
                foreach (Control c in header.Controls)
                {
                    var l = c as HtmlLink;
                    if (null != l && styleSheet.Equals(l.Href, StringComparison.OrdinalIgnoreCase))
                    {
                        addIt = false;
                        break;
                    }
                }

                if (addIt)
                {
                    var link = new HtmlLink();
                    link.Href = styleSheet;
                    link.Attributes.Add("type", "text/css");
                    link.Attributes.Add("rel", "stylesheet");
                    header.Controls.Add(link);

                    // ASP.NET AJAX doesn't currently send a new head element down during an async postback,
                    // so we do the same thing on the client by registering the appropriate script for after
                    // the update. A HiddenField is used to prevent multiple registrations.
                    ScriptManager scriptManager = ScriptManager.GetCurrent(control.Page);
                    if (null == scriptManager)
                    {
                        throw new InvalidOperationException("No Script Manager Available");
                    }
                    if (scriptManager.IsInAsyncPostBack &&
                        (null == control.Page.Request.Form["__AjaxControlToolkitCalendarCssLoaded"]))
                    {
                        ScriptManager.RegisterClientScriptBlock(control, control.GetType(), "RegisterCssReferences",
                                                                "var head = document.getElementsByTagName('HEAD')[0];" +
                                                                "if (head) {" +
                                                                "var linkElement = document.createElement('link');" +
                                                                "linkElement.type = 'text/css';" +
                                                                "linkElement.rel = 'stylesheet';" +
                                                                "linkElement.href = '" + styleSheet + "';" +
                                                                "head.appendChild(linkElement);" +
                                                                "}"
                                                                , true);
                        ScriptManager.RegisterHiddenField(control, "__AjaxControlToolkitCalendarCssLoaded", "");
                    }
                }
            }
        }

        /// <summary>
        /// Executes a callback capable method on a control
        /// </summary>
        /// <param name="control"></param>
        /// <param name="callbackArgument"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Deliberate attempt to catch and pass-on all exceptions")]
        public static string ExecuteCallbackMethod(Control control, string callbackArgument)
        {
            Type controlType = control.GetType();

            // Deserialize the callback JSON into CLR objects
            var js = new JavaScriptSerializer();
            var callInfo = js.DeserializeObject(callbackArgument) as Dictionary<string, object>;

            // Get the call information
            var methodName = (string)callInfo["name"];
            var args = (object[])callInfo["args"];
            var clientState = (string)callInfo["state"];

            // Attempt to load the client state
            var csm = control as IClientStateManager;
            if (csm != null && csm.SupportsClientState)
            {
                csm.LoadClientState(clientState);
            }

            // call the method
            object result = null;
            string error = null;
            try
            {
                // Find a matching static or instance method.  Only public methods can be invoked
                MethodInfo mi = controlType.GetMethod(methodName,
                                                      BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (mi == null)
                {
                    throw new MissingMethodException(controlType.FullName, methodName);
                }

                // Verify that the method has the corrent number of parameters as well as the ExtenderControlMethodAttribute
                ParameterInfo[] methodParams = mi.GetParameters();
                var methAttr =
                    (ExtenderControlMethodAttribute)
                    Attribute.GetCustomAttribute(mi, typeof(ExtenderControlMethodAttribute));
                if (methAttr == null || !methAttr.IsScriptMethod || args.Length != methodParams.Length)
                {
                    throw new MissingMethodException(controlType.FullName, methodName);
                }

                // Convert each argument to the parameter type if possible
                // NOTE: I'd rather have the ObjectConverter from within System.Web.Script.Serialization namespace for this
                var targetArgs = new object[args.Length];
                for (int i = 0; i < targetArgs.Length; i++)
                {
                    if (args[i] == null)
                        continue;
                    targetArgs[i] = Convert.ChangeType(args[i], methodParams[i].ParameterType,
                                                       CultureInfo.InvariantCulture);
                }
                result = mi.Invoke(control, targetArgs);
            }
            catch (Exception ex)
            {
                // Catch the exception information to relay back to the client
                if (ex is TargetInvocationException)
                {
                    ex = ex.InnerException;
                }
                error = ex.GetType().FullName + ":" + ex.Message;
            }

            // return the result
            var resultInfo = new Dictionary<string, object>();
            if (error == null)
            {
                resultInfo["result"] = result;
                if (csm != null && csm.SupportsClientState)
                {
                    resultInfo["state"] = csm.SaveClientState();
                }
            }
            else
            {
                resultInfo["error"] = error;
            }

            // Serialize the result info into JSON
            return js.Serialize(resultInfo);
        }

        /// <summary>
        /// ScriptReference objects aren't immutable.  The AJAX core adds context to them, so we cant' reuse them.
        /// Therefore, we track only ReferenceEntries internally and then convert them to NEW ScriptReference objects on-demand.        
        /// </summary>
        /// <param name="entries"></param>
        /// <returns></returns>
        private static IEnumerable<ScriptReference> ScriptReferencesFromResourceEntries(IList<ResourceEntry> entries)
        {
            IList<ScriptReference> referenceList = new List<ScriptReference>(entries.Count);

            foreach (ResourceEntry re in entries)
            {
                referenceList.Add(re.ToScriptReference());
            }
            return referenceList;
        }

        /// <summary>
        /// Gets the ScriptReferences for a Type and walks the Type's dependencies with circular-reference checking
        /// </summary>
        /// <param name="type">Type for which references are to be gotten</param>
        /// <param name="typeReferenceStack">Stack of Types to track processed types</param>
        /// <param name="ignoreStartingTypeReferences">true if the ClientScriptResource for the starting type is to be ignored</param>
        /// <returns>list of ScriptReferences for the Type</returns>
        private static List<ResourceEntry> GetScriptReferencesInternal(Type type, Stack<Type> typeReferenceStack,
                                                                       bool ignoreStartingTypeReferences)
        {
            // Verify no circular references
            if (typeReferenceStack.Contains(type))
            {
                throw new InvalidOperationException("Circular reference detected.");
            }

            // Look for a cached set of references outside of the lock for perf.
            //
            List<ResourceEntry> entries;

            // Don't check the cache for "unusual" responses
            if (!ignoreStartingTypeReferences && _cache.TryGetValue(type, out entries))
            {
                return entries;
            }

            // Track this type to prevent circular references
            typeReferenceStack.Push(type);
            try
            {
                lock (_sync)
                {
                    // since we're inside the lock, check again just in case.
                    //
                    if (ignoreStartingTypeReferences || !_cache.TryGetValue(type, out entries))
                    {
                        entries = new List<ResourceEntry>();

                        // Get the required scripts by type
                        var requiredScripts = new List<RequiredScriptAttribute>();
                        foreach (
                            RequiredScriptAttribute attr in
                                type.GetCustomAttributes(typeof(RequiredScriptAttribute), true))
                        {
                            requiredScripts.Add(attr);
                        }

                        requiredScripts.Sort(
                            delegate(RequiredScriptAttribute left, RequiredScriptAttribute right) { return left.LoadOrder.CompareTo(right.LoadOrder); });
                        foreach (RequiredScriptAttribute attr in requiredScripts)
                        {
                            if (attr.ExtenderType != null)
                            {
                                // extrapolate dependent references and add them to the ref list.
                                entries.AddRange(GetScriptReferencesInternal(attr.ExtenderType, typeReferenceStack,
                                                                             false));
                            }
                        }

                        // Get the client script resource values for this type
                        int order = 0;

                        // create a new list so we can sort it independently.
                        //
                        var newEntries = new List<ResourceEntry>();
                        for (Type current = type;
                             current != null && current != typeof(object);
                             current = current.BaseType)
                        {
                            if (ignoreStartingTypeReferences && (current == type))
                            {
                                // Skip the starting type because ScriptPath is being used to override
                                continue;
                            }

                            object[] attrs = Attribute.GetCustomAttributes(current,
                                                                           typeof(ClientScriptResourceAttribute), false);
                            order -= attrs.Length;

                            foreach (ClientScriptResourceAttribute attr in attrs)
                            {
                                var re = new ResourceEntry(attr.ResourcePath, current, order + attr.LoadOrder);

                                // check for dups in the list.
                                //
                                if (!entries.Contains(re) && !newEntries.Contains(re))
                                {
                                    newEntries.Add(re);
                                }
                            }
                        }

                        // sort the list and add it to the array.
                        //
                        newEntries.Sort(
                            delegate(ResourceEntry l, ResourceEntry r) { return l.Order.CompareTo(r.Order); });
                        entries.AddRange(newEntries);

                        // Cache the reference list and return (but don't cache if this response is unusual for some reason)
                        //
                        if (!ignoreStartingTypeReferences)
                        {
                            _cache.Add(type, entries);
                        }
                    }

                    return entries;
                }
            }
            finally
            {
                // Remove the type as further requests will get the cached reference
                typeReferenceStack.Pop();
            }
        }

        /// <summary>
        /// Gets the css references for a type and walks the type's dependencies with circular-reference checking
        /// </summary>
        /// <param name="type"></param>
        /// <param name="typeReferenceStack"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetCssReferences(Control control, Type type, Stack<Type> typeReferenceStack)
        {
            // Verify no circular references
            if (typeReferenceStack.Contains(type))
            {
                throw new InvalidOperationException("Circular reference detected.");
            }

            // Look for a cached set of references
            IList<string> references;
            if (_cssCache.TryGetValue(type, out references))
            {
                return references;
            }

            // Track this type to prevent circular references
            typeReferenceStack.Push(type);
            try
            {
                lock (_sync)
                {
                    // double-checked lock
                    if (_cssCache.TryGetValue(type, out references))
                    {
                        return references;
                    }

                    // build the reference list
                    var referenceList = new List<string>();

                    // Get the required scripts by type
                    var requiredScripts = new List<RequiredScriptAttribute>();
                    foreach (
                        RequiredScriptAttribute attr in type.GetCustomAttributes(typeof(RequiredScriptAttribute), true)
                        )
                    {
                        requiredScripts.Add(attr);
                    }
                    requiredScripts.Sort(
                        delegate(RequiredScriptAttribute left, RequiredScriptAttribute right) { return left.LoadOrder.CompareTo(right.LoadOrder); });
                    foreach (RequiredScriptAttribute attr in requiredScripts)
                    {
                        if (attr.ExtenderType != null)
                        {
                            // extrapolate dependent references
                            referenceList.AddRange(GetCssReferences(control, attr.ExtenderType, typeReferenceStack));
                        }
                    }

                    // Get the client script resource values for this type
                    var entries = new List<ResourceEntry>();
                    int order = 0;
                    for (Type current = type; current != null && current != typeof(object); current = current.BaseType)
                    {
                        object[] attrs = Attribute.GetCustomAttributes(current, typeof(ClientCssResourceAttribute),
                                                                       false);
                        order -= attrs.Length;

                        foreach (ClientCssResourceAttribute attr in attrs)
                        {
                            entries.Add(new ResourceEntry(attr.ResourcePath, current, order + attr.LoadOrder));
                        }
                    }
                    entries.Sort(delegate(ResourceEntry l, ResourceEntry r) { return l.Order.CompareTo(r.Order); });
                    foreach (ResourceEntry entry in entries)
                    {
                        referenceList.Add(control.Page.ClientScript.GetWebResourceUrl(entry.ComponentType,
                                                                                      entry.ResourcePath));
                    }

                    // Remove duplicates from reference list
                    var cookies = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    var newReferenceList = new List<string>();
                    foreach (string refr in referenceList)
                    {
                        if (cookies.ContainsKey(refr))
                            continue;
                        cookies.Add(refr, null);
                        newReferenceList.Add(refr);
                    }

                    // Create a readonly dictionary to hold the values
                    references = new ReadOnlyCollection<string>(newReferenceList);

                    // Cache the reference
                    _cssCache.Add(type, references);

                    // return the list
                    return references;
                }
            }
            finally
            {
                // Remove the type as further requests will get the cached reference
                typeReferenceStack.Pop();
            }
        }

        #region Nested type: Reference

        private struct Reference
        {
            [ClientPropertyName("referenceType")]
            [ExtenderControlProperty]
            public ReferenceType ReferenceType { get; set; }

            [ExtenderControlProperty]
            [ClientPropertyName("refId")]
            public string ClientId { get; set; }

            [ExtenderControlProperty]
            [ClientPropertyName("builderAction")]
            public string BuilderAction
            {
                get { return "reference"; }
            }
        }

        #endregion

        #region Nested type: ReferenceType

        private enum ReferenceType
        {
            Element,
            Component
        }

        #endregion

        #region Nested type: ResourceEntry

        private struct ResourceEntry
        {
            public readonly Type ComponentType;
            public readonly int Order;
            public readonly string ResourcePath;

            public ResourceEntry(string path, Type componentType, int order)
            {
                ResourcePath = path;
                ComponentType = componentType;
                Order = order;
            }

            private string AssemblyName
            {
                get { return ComponentType == null ? "" : ComponentType.Assembly.FullName; }
            }

            public ScriptReference ToScriptReference()
            {
                var refr = new ScriptReference();
                refr.Assembly = AssemblyName;
                refr.Name = ResourcePath;
                return refr;
            }

            public override bool Equals(object obj)
            {
                var other = (ResourceEntry)obj;
                return ResourcePath.Equals(other.ResourcePath, StringComparison.OrdinalIgnoreCase)
                       && AssemblyName.Equals(other.AssemblyName, StringComparison.OrdinalIgnoreCase);
            }

            public static bool operator ==(ResourceEntry obj1, ResourceEntry obj2)
            {
                return obj1.Equals(obj2);
            }

            public static bool operator !=(ResourceEntry obj1, ResourceEntry obj2)
            {
                return !obj1.Equals(obj2);
            }

            public override int GetHashCode()
            {
                return AssemblyName.GetHashCode() ^ ResourcePath.GetHashCode();
            }
        }

        #endregion
    }
}