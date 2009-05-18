using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


// Note to anyone looking at this code.  Development of this utility started before .NET 1.0 shipped back in 2001 or so.  Development
// and improvement has continuted all the way through 2008.  That explains some of the inconsistancy in style, collection types, parsing methods, etc.

namespace XSDObjectGenLib
{
    /// <summary>
    /// Supported languages
    /// </summary>
    public enum Language
    {
        VB,
        CS
    }

    /// <summary>
    /// Expected possible application exceptions
    /// </summary>
    public class XSDObjectGenException : ApplicationException
    {
        public XSDObjectGenException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// This is used for the /E option to only generate a select list of elements from the schema.  This is useful for huge 
    /// schemas where you only want to generate a subset of classes from the schema.  You then pass in the list of global 
    /// elements that you want classes generated for, and we'll parse the schema to figure out all the references.
    /// </summary>
    public class ElementsSubset
    {
        public SortedList<string, string> Elements = new SortedList<string, string>();
        public SortedList<string, string> ComplexTypes = new SortedList<string, string>();
        public SortedList<string, string> SimpleTypes = new SortedList<string, string>();
    }


    /// <summary>
    /// Parse the XSD schema and build the .NET language classes.
    /// </summary>
    public class XSDSchemaParser
    {
        private XmlSchemaForm elementFormDefault;		// qualified or unqualified
        private XmlSchemaForm attributeFormDefault;		// qualified or unqualified

        private LanguageBase code;
        private XmlSchema schema;
        private StreamWriter outStream;
        string[] outFiles;
        private bool optionConstructRequiredSchema = false;	// add schema compliancy function to class 
        private bool optionDepthFirstTraversalHooks = false;			// add DepthFirstTraversal hooks
        private bool optionDefaultInitialization = false;	// add schema controlled field initializers

        // hash tables are filled during Pass 1 to help build code in Pass 2
        private Hashtable globalComplexTypeClasses;	// global Xsd elements and types that will generate classes in code -- have schema node as parent
        // Keyed by .net class name (value -- xml element name)
        private Hashtable globalQualifiedComplexTypeClasses;  // global UNIQUE xsd type names -- keyed by qualified name (value -- the .net unique class name)
        // Same number of items as globalComplexTypeClasses, but keyed by qualified name.
        private Hashtable globalQualifiedAbstractTypeClasses; // abstract classes to setup XmlInclude attributes
        // Keyed by qualified name (value -- the .net unique class name).  
        // Value -- Hashtable list of types inheriting from the abstract type.
        private Hashtable classesReferencingAbstractTypes;  // Classes who reference abstract types.  Name/Value pairs.  Classes could exist more than once.
        private Hashtable enumerations;			// List of collected enumeration types in the schema.  Can be global or local.
        private Hashtable enumerableClasses;	// Classes that can be enumerable -- meaning they contain a single
        //   "unbounded" child element.  We build enumerator support for the 
        //	 contained collection.
        private Hashtable namespaces = new Hashtable();  // list of target and imported namespaces into target schema
        private ArrayList namespacesList = new ArrayList();  // an ordered list of the namespaces hashtable -- to make sure we build
        // the namespaces in the correct order, bottom up.
        private Hashtable xsdNsToClrNs = new Hashtable();   // map xsd namespaces to clr namespaces
        private XmlDocument acordLookup = null;
        private XmlDocument acordLookupPrivate = null;

        #region Orchestrator

        /// <summary>
        /// Executes the code generator.  Called from either the command line exe or the VS.NET wizard.  Only public function.
        /// </summary>
        /// <param name="xsdFile">Path to the XSD file stored on disk</param>
        /// <param name="language">Language for generated code</param>
        /// <param name="genNamespace">.NET namespace containing the generated types</param>
        /// <param name="fileName">File to be generated.  If null, namespace name is used</param>
        /// <param name="outputLocation">Location for the generated output file</param>
        /// <param name="constructRequiredSchema">Build a schema compliancy function -- MakeSchemaCompliant -- into each class</param>
        /// <param name="depthFirstTraversalHooks">Add DepthFirstTraversal hooks to fire custom DepthFirstTraversal events on each generated class</param>
        /// <param name="defaultInitialization">Set schema default values in class constructors</param>
        /// <param name="separateImportedNamespaces">Searate out imported namespaces into their own source files.  Default is all types in one file.</param>
        /// <param name="namespaceTable">Namespace map table.  Null if no imported namespaces exist</param>
        /// <param name="filenameTable">Filenames matching the namespaceTable.  Null if no imported namespaces exist -- and optional.</param>
        /// <param name="partialKeyword">put the .NET 2.0 partial keyword on every class</param>
        /// <param name="optionEElements"></param>
        /// <param name="acordLookupCodesFile">ACORD lookup codes file -- public</param>
        /// <param name="acordLookupCodesFilePrivate">ACORD lookup codes file -- private</param>
        /// <returns>result string</returns>
        public string[] Execute(string xsdFile, Language language, string genNamespace, string fileName,
            string outputLocation, bool constructRequiredSchema, bool depthFirstTraversalHooks, bool defaultInitialization,
            ref Hashtable namespaceTable, Hashtable filenameTable, bool partialKeyword, List<string> optionEElements, string acordLookupCodesFile, string acordLookupCodesFilePrivate)
        {
            FileStream schemaFile = null;

            try
            {
                schemaFile = new FileStream(xsdFile, FileMode.Open, FileAccess.Read);
                if (schemaFile == null) throw new XSDObjectGenException("Could not open the XSD schema file: " + xsdFile);

                schema = XmlSchema.Read(schemaFile, new ValidationEventHandler(ShowCompileError));
                schema.Compile(new ValidationEventHandler(ShowCompileError));

                elementFormDefault = schema.ElementFormDefault;
                attributeFormDefault = schema.AttributeFormDefault;

                if (language == Language.VB)
                {
                    code = (LanguageBase)new VBTemplate();
                }
                else if (language == Language.CS)
                {
                    code = (LanguageBase)new CSharpTemplate();
                }
                else
                {
                    throw new XSDObjectGenException(string.Format("Language {0} not supported.", language.ToString()));
                }

                if (!string.IsNullOrEmpty(acordLookupCodesFile))
                {
                    acordLookup = new XmlDocument();
                    try
                    {
                        acordLookup.Load(acordLookupCodesFile);
                    }
                    catch (Exception)
                    {
                        throw new XSDObjectGenException("Bad ACORD lookup file path or file");
                    }
                }

                if (!string.IsNullOrEmpty(acordLookupCodesFilePrivate))
                {
                    acordLookupPrivate = new XmlDocument();
                    try
                    {
                        acordLookupPrivate.Load(acordLookupCodesFilePrivate);
                    }
                    catch (Exception)
                    {
                        throw new XSDObjectGenException("Bad ACORD private codes lookup file path or file");
                    }
                }

                optionDefaultInitialization = defaultInitialization;
                optionConstructRequiredSchema = constructRequiredSchema;
                optionDepthFirstTraversalHooks = depthFirstTraversalHooks;
                LanguageBase.partialClasses = partialKeyword;
                globalComplexTypeClasses = new Hashtable();
                globalQualifiedComplexTypeClasses = new Hashtable();
                globalQualifiedAbstractTypeClasses = new Hashtable();
                enumerations = new Hashtable();
                enumerableClasses = new Hashtable();
                classesReferencingAbstractTypes = new Hashtable();
                ArrayList parentClassStack = new ArrayList();  // arraylist to collect a stack of parent owner .NET classes for  nested complexTypes 
                Globals.globalSchemaTypeTable.Clear();			// clear globals schema type table as it's a global object created at assembly load scope
                Globals.globalSeparateImportedNamespaces = false;   // means we have imported namespace and will have multiple source files
                Globals.globalClrNamespaceList.Clear();

                // Get list of all imported schemas -- to be used only if separateImportedNamespaces = true
                // Add the main xsd namespace.  If separateImportedNamespaces is false, there will be only
                //    one .net file generated with all of the types from all the xsd namespaces.  Otherwise multiple files. 
                //    First namespace in the list is the namespace for the base leaf schema -- for which code is being generated.
                ArrayList references = new ArrayList();
                if (namespaceTable != null)
                {
                    if (schema.TargetNamespace != null)
                        genNamespace = (string)namespaceTable[schema.TargetNamespace];  // in case it was changed while changing imported ns names
                    else
                        genNamespace = (string)namespaceTable[""];
                }

                if (schema.TargetNamespace != null)
                {
                    namespaces.Add(schema.TargetNamespace, references);
                    namespacesList.Add(schema.TargetNamespace);
                    xsdNsToClrNs.Add(schema.TargetNamespace, genNamespace);
                }
                else
                {
                    // targetNamespace can be null
                    namespaces.Add("", references);
                    namespacesList.Add("");
                    xsdNsToClrNs.Add("", genNamespace);
                }

                // Break down the imported namespaces if any.  There will be one .cs or .vb file generated for each xsd namespace.
                // Set the globalSelectedImportedNamespaces flag to determine if we generate just one file or many files.
                RecurseImportedNamespaces(schema, references);
                if (namespacesList.Count > 1 && namespaceTable == null)
                {
                    namespaceTable = xsdNsToClrNs;
                    return null;
                }
                else if (namespaceTable != null)
                {
                    xsdNsToClrNs = namespaceTable;
                }

                foreach (string clrNs in xsdNsToClrNs.Values)
                {
                    Globals.globalClrNamespaceList.Add(clrNs);
                }

                /*******/
                //   Walk through, prepare and build the global class collections from the xsd complexTypes 
                //   contained in the global scope -- child of the <schema> element.  Point of this stuff is 
                //   to handle all the combination of naming conflicts.
                //      Build and populate these structures
                //          globalQualifiedAbstractTypeClasses
                //          globalQualifiedComplexTypeClasses
                //          globalComplexTypeClasses
                //          globalSchemaTypeTable
                /*******/

                foreach (XmlSchemaType schemaType in schema.SchemaTypes.Values)
                {
                    if (schemaType is XmlSchemaComplexType)
                    {
                        string dotnetTypeName = CalculateUniqueTypeOrFieldName(schemaType.Name, schemaType.QualifiedName.Namespace, globalComplexTypeClasses);
                        globalComplexTypeClasses.Add(dotnetTypeName, schemaType.Name);

                        GlobalSchemaType gst = new GlobalSchemaType(schemaType.QualifiedName.Namespace, schemaType.Name, GlobalXsdType.ComplexType,
                            (string)xsdNsToClrNs[schemaType.QualifiedName.Namespace], dotnetTypeName);
                        Globals.globalSchemaTypeTable.Add(gst.XsdNamespaceAndTypeName, gst);

                        if (globalQualifiedComplexTypeClasses[schemaType.QualifiedName] == null)
                        {
                            globalQualifiedComplexTypeClasses.Add(schemaType.QualifiedName, dotnetTypeName);

                            // collect the list of types marked as abstract.  This is needed to later add XmlIncludeAttribute to their usage.
                            if (((XmlSchemaComplexType)schemaType).IsAbstract)
                            {
                                globalQualifiedAbstractTypeClasses.Add(schemaType.QualifiedName, new Hashtable());
                            }
                        }
                    }
                }
                foreach (XmlSchemaElement element in schema.Elements.Values)
                {
                    if (element.ElementType is XmlSchemaComplexType)
                    {
                        XmlSchemaComplexType complexType = (XmlSchemaComplexType)element.ElementType;

                        string dotnetTypeName = element.Name;

                        if (globalComplexTypeClasses[element.Name] == null)
                        {
                            // Each namespace can have one global schema level CT (schema type) and global schema level element WITH THE SAME NAME.
                            //  We get here assuming a complexType with the same name hasn't already been added (which will be a .NET class).
                            dotnetTypeName = CalculateUniqueTypeOrFieldName(element.Name, element.QualifiedName.Namespace, globalComplexTypeClasses);
                            globalComplexTypeClasses.Add(dotnetTypeName, element.Name);
                        }

                        GlobalSchemaType gst = new GlobalSchemaType(element.QualifiedName.Namespace, element.Name, GlobalXsdType.Element,
                                (string)xsdNsToClrNs[element.QualifiedName.Namespace], dotnetTypeName);
                        Globals.globalSchemaTypeTable.Add(gst.XsdNamespaceAndTypeName, gst);

                        if (globalQualifiedComplexTypeClasses[element.QualifiedName] == null)
                        {
                            globalQualifiedComplexTypeClasses.Add(element.QualifiedName, dotnetTypeName);
                        }
                        else
                        {
                            // Each namespace can have one global schema level CT (schema type) and global schema level element WITH THE SAME NAME.
                            //  Hence schemaType.QualifiedName == element.QualifiedName for this scenerio.  Use a special Globals.ELELENT_DELIMINATOR to flag this.
                            globalQualifiedComplexTypeClasses.Add(Globals.ELELENT_DELIMINATOR + element.QualifiedName, dotnetTypeName);
                        }
                    }
                    else if (element.ElementType is XmlSchemaSimpleType)
                    {
                        XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType)element.ElementType;
                        if (IsEnumeration(simpleType))
                        {
                            GlobalSchemaType gst = new GlobalSchemaType(element.QualifiedName.Namespace, element.Name, GlobalXsdType.Enum,
                                (string)xsdNsToClrNs[element.QualifiedName.Namespace], element.Name);
                            Globals.globalSchemaTypeTable.Add(gst.XsdNamespaceAndTypeName, gst);
                        }
                    }
                }
                foreach (XmlSchemaType schemaType in schema.SchemaTypes.Values)
                {
                    if (schemaType is XmlSchemaSimpleType)
                    {
                        XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType)schemaType;
                        if (IsEnumeration(simpleType))
                        {
                            GlobalSchemaType gst = new GlobalSchemaType(simpleType.QualifiedName.Namespace, simpleType.Name, GlobalXsdType.Enum,
                                (string)xsdNsToClrNs[simpleType.QualifiedName.Namespace], simpleType.Name);
                            if (Globals.globalSchemaTypeTable[gst.XsdNamespaceAndTypeName] == null)
                                Globals.globalSchemaTypeTable.Add(gst.XsdNamespaceAndTypeName, gst);
                        }
                    }
                }

                outFiles = new string[namespaces.Count];
                int iFiles = 0;

                // Walk through all of the schema namespaces and generate .net types
                for (int i = 0; i < namespacesList.Count; i++)
                {
                    /*******/
                    // Pass1: 
                    // Walk through and add collect infomation for forward declarations (where required),
                    //  collection classes, and enumerations.  The code will be added later.
                    // There are two kinds of complex types.  
                    //	1. ComplexTypes globally defined and "named" in the schema that are not tied to an XML element name.
                    //	2. Globally defined xsd:elements that are complex types and are linked to an xml element
                    // First build classes for the global types, then traverse the elements and add those types.
                    // Note: There are actually 3 different types of ComplexTypes.  The third is a locally defined
                    //  ComplexType that are not a child of <schema>.  These are collected later in the childClasses
                    //  collection.
                    /*******/

                    string ns = (string)namespacesList[i];
                    enumerations.Clear();
                    bool optionE = optionEElements != null && optionEElements.Count > 0;
                    ElementsSubset es = new ElementsSubset();

                    // Pass0.  Uses Pass0 routines.  Only used if Option E is sent in.
                    if (optionE)
                    {
                        // Handle special optionE processing.  Parse down and build the references for this element
                        foreach (XmlSchemaElement element in schema.Elements.Values)
                        {
                            if (optionEElements.Contains(element.QualifiedName.Name))
                            {
                                XmlSchemaComplexType complexType = (XmlSchemaComplexType)element.ElementType;

                                if (!es.Elements.ContainsKey(element.QualifiedName.Name))
                                {
                                    es.Elements.Add(element.QualifiedName.Name, element.QualifiedName.Name);
                                    if (!string.IsNullOrEmpty(complexType.QualifiedName.Name) && !es.ComplexTypes.ContainsKey(complexType.QualifiedName.Name))
                                        es.ComplexTypes.Add(complexType.QualifiedName.Name, complexType.QualifiedName.Name);
                                    ParseComplexTypePass0(complexType, element.QualifiedName.Name, element.QualifiedName, element.QualifiedName.Namespace, es);
                                }
                            }
                        }
                    }

                    // Pass1 officially starts here.  Uses Pass1 routines.

                    foreach (XmlSchemaType schemaType in schema.SchemaTypes.Values)
                    {
                        // Add global schema type enumerations.  Locally scoped ones will be added later during ParseComplexTypePass1.
                        if (schemaType is XmlSchemaSimpleType && (!Globals.globalSeparateImportedNamespaces || ns == schemaType.QualifiedName.Namespace))
                        {
                            XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType)schemaType;
                            if (optionE && !es.SimpleTypes.ContainsKey(simpleType.Name)) continue;  // only build specific classes

                            if (IsEnumeration(simpleType))
                            {
                                string globalEnumName = ParseEnumeration1(simpleType, simpleType.Name);
                            }
                        }
                    }
                    foreach (XmlSchemaElement element in schema.Elements.Values)
                    {
                        // Add global schema element enumerations.  Locally scoped ones will be added later during ParseComplexTypePass1.
                        if (element.ElementType is XmlSchemaSimpleType && (!Globals.globalSeparateImportedNamespaces || ns == element.QualifiedName.Namespace))
                        {
                            XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType)element.ElementType;
                            if (optionE && !es.SimpleTypes.ContainsKey(simpleType.Name)) continue;  // only build specific classes

                            if (IsEnumeration(simpleType))
                            {
                                string globalEnumName = ParseEnumeration1(simpleType, element.Name);
                            }
                        }
                    }

                    // now deep-dive and parse into each global schema element and type

                    foreach (XmlSchemaType schemaType in schema.SchemaTypes.Values)
                    {
                        if (schemaType is XmlSchemaComplexType && (!Globals.globalSeparateImportedNamespaces || ns == schemaType.QualifiedName.Namespace))
                        {
                            if (optionE && !es.ComplexTypes.ContainsKey(schemaType.QualifiedName.Name)) continue;  // only build specific classes

                            parentClassStack.Clear();
                            string dotnetTypeName = (string)globalQualifiedComplexTypeClasses[schemaType.QualifiedName];
                            ParseComplexTypePass1((XmlSchemaComplexType)schemaType, dotnetTypeName, parentClassStack, schemaType.QualifiedName, ns);
                        }
                    }
                    foreach (XmlSchemaElement element in schema.Elements.Values)
                    {
                        if (element.ElementType is XmlSchemaComplexType &&
                            (!Globals.globalSeparateImportedNamespaces || ns == element.QualifiedName.Namespace) &&
                            (element.SchemaTypeName.Name == null || element.SchemaTypeName.Name == ""))
                        {
                            if (optionE && !es.Elements.ContainsKey(element.QualifiedName.Name)) continue;  // only build specific classes

                            // If element.SchemaTypeName.Name is set, then this is a global element with it's type set to a schema type.  
                            //  i.e. <xs:element name="DataServiceResponse" type="DataServiceResponseType" />
                            //  This results in a simple class in code which inherits from the SchemaType and no child fields -- so don't do anything further

                            parentClassStack.Clear();
                            XmlSchemaComplexType complexType = (XmlSchemaComplexType)element.ElementType;

                            string dotnetTypeName = GlobalElementToClrMap(element.QualifiedName);
                            ParseComplexTypePass1(complexType, dotnetTypeName, parentClassStack, element.QualifiedName, ns);
                        }
                    }

                    // ****************************
                    // Start writing out code
                    // ****************************

                    FileStream classFile = null;

                    if (outputLocation == "" || outputLocation == null)
                        outFiles[iFiles] = "";
                    else if (outputLocation[outputLocation.Length - 1] == '\\')
                        outFiles[iFiles] = outputLocation;
                    else if (outputLocation[outputLocation.Length - 1] != '\\')
                        outFiles[iFiles] = outputLocation + "\\";

                    string codeFile;
                    string targetNamespace;
                    string dotnetNamespace;

                    if (i == 0 && namespacesList.Count == 1)  // no imported namespaces
                    {
                        // first namespace in the list is the main xsd namespace we're building from
                        targetNamespace = ns;
                        codeFile = genNamespace;  // this is the default filename if not overriden with /f or /z
                        dotnetNamespace = genNamespace;

                        // use "fileName" for the main xsd generated .net file, if passed in from UI
                        if (fileName != null && fileName != "")
                            codeFile = fileName;
                    }
                    else
                    {
                        // imported namespaces
                        targetNamespace = ns;
                        dotnetNamespace = (string)xsdNsToClrNs[targetNamespace];
                        codeFile = (string)xsdNsToClrNs[targetNamespace];  // this is the default filename if not overriden with /f or /z
                        if (filenameTable != null && filenameTable.Count > 0)
                            codeFile = (string)filenameTable[targetNamespace];  // /z override
                    }


                    if (language == Language.VB)
                    {
                        if (!codeFile.ToLower().EndsWith(".vb"))
                            codeFile = codeFile + ".vb";
                    }
                    else //(language == Language.CS)
                    {
                        if (!codeFile.ToLower().EndsWith(".cs"))
                            codeFile = codeFile + ".cs";
                    }

                    outFiles[iFiles] = outFiles[iFiles] + codeFile;
                    classFile = new FileStream(outFiles[iFiles], FileMode.Create);
                    outStream = new StreamWriter(classFile);

                    // Add namespace, using statement, forward declarations, and enumerations
                    string schemaFileName = schemaFile.Name;
                    int slashIndex = 0;
                    while ((slashIndex = schemaFileName.IndexOf("\\")) >= 0)
                        schemaFileName = schemaFileName.Substring(slashIndex + 1);

                    if (genNamespace == null || genNamespace == "")
                    {
                        // the VB case where explicit namespaces are not in the code
                        code.NamespaceHeaderCode(outStream, genNamespace, schemaFileName, null,
                            targetNamespace, enumerations, optionDepthFirstTraversalHooks, (ArrayList)namespaces[targetNamespace]);
                    }
                    else
                    {
                        code.NamespaceHeaderCode(outStream, dotnetNamespace, schemaFileName, null,
                            targetNamespace, enumerations, optionDepthFirstTraversalHooks, (ArrayList)namespaces[targetNamespace]);
                    }

                    // Re-add all the globally scoped enumerations to the enumerations list.  This is done again so in Pass2 we handle locally scoped
                    //  simpleType enumerations that have duplicate names.  We go through the name calculation again for duplicate enums -- as a way
                    //  to match up the enum type name (when duplicates occur) with the enum field variables.  The assumption is made that the parsing
                    //  order of Pass1 is equal to that of Pass2 -- otherwise the duplicate locally scoped enums won't match their respective
                    //  enumeration type that was collected during Pass1.  
                    //  Note that in most schemas there won't be locally scoped duplicate enumeration names -- so this really won't matter.  And if there is,
                    //  it's probably not a good schema design.
                    enumerations.Clear();  // clear enumerations list as they will be recollected to properly account for locally scoped duplicates
                    foreach (XmlSchemaType schemaType in schema.SchemaTypes.Values)
                    {
                        if (schemaType is XmlSchemaSimpleType && (!Globals.globalSeparateImportedNamespaces || ns == schemaType.QualifiedName.Namespace))
                        {
                            XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType)schemaType;
                            if (optionE && !es.SimpleTypes.ContainsKey(simpleType.Name)) continue;  // only build specific classes

                            if (IsEnumeration(simpleType) && (!Globals.globalSeparateImportedNamespaces || ns == simpleType.QualifiedName.Namespace))
                                enumerations.Add(LanguageBase.ReplaceInvalidChars(simpleType.Name), "");
                        }
                    }
                    foreach (XmlSchemaElement element in schema.Elements.Values)
                    {
                        if (element.ElementType is XmlSchemaSimpleType && (!Globals.globalSeparateImportedNamespaces || ns == element.QualifiedName.Namespace))
                        {
                            XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType)element.ElementType;
                            if (optionE && !es.SimpleTypes.ContainsKey(simpleType.Name)) continue;  // only build specific classes

                            if (IsEnumeration(simpleType) && (!Globals.globalSeparateImportedNamespaces || ns == element.QualifiedName.Namespace))
                            {
                                if (enumerations[LanguageBase.ReplaceInvalidChars(element.Name)] == null)
                                    enumerations.Add(LanguageBase.ReplaceInvalidChars(element.Name), "");
                            }
                        }
                    }

                    /*******/
                    // Pass2: 
                    //  Walk through and build code for all complex types (these will be classes in .NET)
                    //  First add the globally defined classes, then add the classes for the schema level 
                    //  ComplexType elements.
                    /*******/

                    foreach (XmlSchemaType schemaType in schema.SchemaTypes.Values)
                    {
                        if (schemaType is XmlSchemaComplexType && (!Globals.globalSeparateImportedNamespaces || ns == schemaType.QualifiedName.Namespace))
                        {
                            if (optionE && !es.ComplexTypes.ContainsKey(schemaType.QualifiedName.Name)) continue;  // only build specific classes

                            parentClassStack.Clear();
                            string dotnetTypeName = (string)globalQualifiedComplexTypeClasses[schemaType.QualifiedName];

                            // If schema type and a global element have the same name, don't create a class for the element
                            // This will be a special case where the schema type .net class has both XmlTypeAttribute and XmlElementAttribute.
                            bool globalElementAndSchemaTypeHaveSameName = false;
                            if (schema.Elements[schemaType.QualifiedName] != null)
                                globalElementAndSchemaTypeHaveSameName = true;

                            ParseComplexTypePass2((XmlSchemaComplexType)schemaType, dotnetTypeName, schemaType.Name, true, false,
                                schemaType.QualifiedName.Namespace, parentClassStack, "", "", false, globalElementAndSchemaTypeHaveSameName);
                        }
                    }
                    foreach (XmlSchemaElement element in schema.Elements.Values)
                    {
                        if (element.ElementType is XmlSchemaComplexType && (!Globals.globalSeparateImportedNamespaces || ns == element.QualifiedName.Namespace))
                        {
                            if (optionE && !es.Elements.ContainsKey(element.QualifiedName.Name)) continue;  // only build specific classes

                            parentClassStack.Clear();
                            string dotnetTypeName = GlobalElementToClrMap(element.QualifiedName);
                            string elementSchemaType = "";

                            if (element.SchemaTypeName.Name != null && element.SchemaTypeName.Name != "")
                            {
                                // global element with it's type set to a schema type
                                elementSchemaType = (string)globalQualifiedComplexTypeClasses[element.SchemaTypeName];

                                // If schema type and global element have the same name, don't create a class for the element
                                // This will be a special case where the schema type .net class has both XmlTypeAttribute and XmlElementAttribute.
                                if (element.QualifiedName == element.SchemaTypeName)
                                    continue;
                            }

                            //		// don't generate a class for root level schema-typed elements that are abstract -- since these classes can't be created externally.
                            //		if (elementSchemaType != "" && element.IsAbstract)
                            //			continue; 

                            ParseComplexTypePass2((XmlSchemaComplexType)element.ElementType, dotnetTypeName, element.Name, false, false,
                                element.QualifiedName.Namespace, parentClassStack, elementSchemaType, element.SchemaTypeName.Namespace, element.IsNillable, false);
                        }
                    }

                    code.NamespaceTrailerCode(outStream, dotnetNamespace);
                    outStream.Flush();

                    iFiles++;
                    outStream.Close();
                    schemaFile.Close();
                    // Finished writing out .net for a xsd namespace
                }	// End of foreach imported namespace

                System.Diagnostics.EventLog.WriteEntry("XSDSchemaParser", String.Format("Done. Writing files"));
                return outFiles;
            }
            catch (XSDObjectGenException e)
            {
                // code logic specific exceptions
                throw e;
            }
            catch (FileNotFoundException e)
            {
                // inbound xsd file cannot be loaded
                throw new XSDObjectGenException(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                // security 
                throw new XSDObjectGenException(e.Message);
            }
            catch (XmlSchemaException e)
            {
                // xsd schema compiler exception.  xsd has some type of error.
                if (outStream != null)
                {
                    outStream.WriteLine();
                    outStream.WriteLine("LineNumber = {0}", e.LineNumber);
                    outStream.WriteLine("LinePosition = {0}", e.LinePosition);
                    outStream.WriteLine("Message = {0}", e.Message);
                    outStream.WriteLine("Source = {0}", e.Source);
                }

                throw new XSDObjectGenException(string.Format(
                    ".NET Framework XSD Schema compile error.\nError occurred : {0}", e.Message));
            }
            catch (XmlException e)
            {
                // bad xml document -- schema cannot be read because it's not valid xml
                if (outStream != null)
                {
                    outStream.WriteLine();
                    outStream.WriteLine("LineNumber = {0}", e.LineNumber);
                    outStream.WriteLine("LinePosition = {0}", e.LinePosition);
                    outStream.WriteLine("Message = {0}", e.Message);
                    outStream.WriteLine("Source = {0}", e.Source);
                }

                throw new XSDObjectGenException(string.Format(
                    ".NET Framework could not read the XSD file.  Bad XML file.\nError occurred : {0}", e.Message));
            }
            catch (Exception e)
            {
                // unexpected exceptions
                System.Diagnostics.EventLog.WriteEntry("Unexpected XSDObjectGen exception", e.Message, System.Diagnostics.EventLogEntryType.Error);

                if (outStream != null)
                {
                    outStream.WriteLine();
                    outStream.WriteLine("Error message : {0}", e.Message);
                    outStream.WriteLine("Source : {0}", e.Source);
                    outStream.WriteLine("Stack : {0}", e.StackTrace);
                }

                throw new Exception("Unexpected XSDObjectGen Exception", e);
            }
            finally
            {
                if (outStream != null) outStream.Close();
                if (schemaFile != null) schemaFile.Close();
            }
        }

        private static void ShowCompileError(object sender, ValidationEventArgs e)
        {
            throw new XmlSchemaException("Schema Validation Error: " + e.Message, null);
        }
        #endregion

        #region Pass0 Routines
        //********************************************************************************************
        //********************************************************************************************
        // PASS ZERO ROUTINES -- this is only used when someone uses the /E option for telling the code 
        // generator to only generate classes and types for a specific set of elements from the schema.
        // Purpose of this is to fill out the ElementsSubset referenced complexTypes and simpleTypes.
        // Point is to find just the global elements and types referenced by the passed in element list
        // and build code for those elements.  This will result in a smaller compiled assembly.
        //********************************************************************************************
        //********************************************************************************************

        /// <summary>
        /// Pass0. Collect info on classes, forward declarations, collections (ArrayList subclasses), and any local enumerations (globals have already been added).
        /// </summary>
        /// <param name="complex">Instance of a ComplexType</param>
        /// <param name="qname">QualifiedName object for schema element or schema type</param>
        private void ParseComplexTypePass0(XmlSchemaComplexType complex, String dotnetClassName, XmlQualifiedName qname, string currentNamespace, ElementsSubset es)
        {
            ArrayList childClasses = new ArrayList();

            XmlSchemaComplexType baseSchema = complex.BaseSchemaType as XmlSchemaComplexType;
            if (baseSchema != null)
            {
                if (!es.ComplexTypes.ContainsKey(baseSchema.QualifiedName.Name))
                {
                    es.ComplexTypes.Add(baseSchema.QualifiedName.Name, baseSchema.QualifiedName.Name);
                    ParseComplexTypePass0(baseSchema, baseSchema.QualifiedName.Name, baseSchema.QualifiedName, baseSchema.QualifiedName.Name, es);
                }
            }

            // Gather referenced enumeration types if any
            ParseAttributesPass0(complex.Attributes, es);

            if (complex.Particle is XmlSchemaGroupBase)
            {
                ParseGroupBasePass0((XmlSchemaGroupBase)complex.Particle, dotnetClassName, childClasses, complex.QualifiedName.Namespace, currentNamespace, es);
            }
            else if (complex.Particle is XmlSchemaGroupRef)
            {
                XmlSchemaGroup group = (XmlSchemaGroup)schema.Groups[((XmlSchemaGroupRef)complex.Particle).RefName];
                ParseGroupBasePass0(group.Particle, dotnetClassName, childClasses, complex.QualifiedName.Namespace, currentNamespace, es);
            }
            else if (complex.ContentModel is XmlSchemaSimpleContent)
            {
                XmlSchemaSimpleContent simpleContent = (XmlSchemaSimpleContent)complex.ContentModel;
                if (simpleContent.Content is XmlSchemaSimpleContentRestriction)
                {
                    XmlSchemaSimpleContentRestriction contentRestriction = (XmlSchemaSimpleContentRestriction)simpleContent.Content;
                    ParseAttributesPass0(contentRestriction.Attributes, es);
                }
                else if (simpleContent.Content is XmlSchemaSimpleContentExtension)
                {
                    XmlSchemaSimpleContentExtension contentExtension = (XmlSchemaSimpleContentExtension)simpleContent.Content;
                    ParseAttributesPass0(contentExtension.Attributes, es);
                }
            }
            else if (complex.ContentModel is XmlSchemaComplexContent)
            {
                XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent)complex.ContentModel;
                if (complexContent.Content is XmlSchemaComplexContentRestriction)
                {
                    XmlSchemaComplexContentRestriction contentRestriction = (XmlSchemaComplexContentRestriction)complexContent.Content;

                    if (contentRestriction.Attributes != null)
                        ParseAttributesPass0(contentRestriction.Attributes, es);

                    if (contentRestriction.Particle != null)
                    {
                        if (contentRestriction.Particle is XmlSchemaGroupBase)
                            ParseGroupBasePass0((XmlSchemaGroupBase)contentRestriction.Particle, dotnetClassName, childClasses, complex.QualifiedName.Namespace, currentNamespace, es);
                        else if (contentRestriction.Particle is XmlSchemaGroupRef)
                        {
                            XmlSchemaGroup group = (XmlSchemaGroup)schema.Groups[((XmlSchemaGroupRef)contentRestriction.Particle).RefName];
                            ParseGroupBasePass0(group.Particle, dotnetClassName, childClasses, complex.QualifiedName.Namespace, currentNamespace, es);
                        }
                    }
                }
                else if (complexContent.Content is XmlSchemaComplexContentExtension)
                {
                    XmlSchemaComplexContentExtension contentExtension = (XmlSchemaComplexContentExtension)complexContent.Content;

                    if (contentExtension.Attributes != null)
                        ParseAttributesPass0(contentExtension.Attributes, es);
                    if (contentExtension.Particle != null)
                    {
                        if (contentExtension.Particle is XmlSchemaGroupBase)
                            ParseGroupBasePass0((XmlSchemaGroupBase)contentExtension.Particle, dotnetClassName, childClasses, complex.QualifiedName.Namespace, currentNamespace, es);
                        else if (contentExtension.Particle is XmlSchemaGroupRef)
                        {
                            XmlSchemaGroup group = (XmlSchemaGroup)schema.Groups[((XmlSchemaGroupRef)contentExtension.Particle).RefName];
                            ParseGroupBasePass0(group.Particle, dotnetClassName, childClasses, complex.QualifiedName.Namespace, currentNamespace, es);
                        }
                    }
                }
            }

            for (int i = 0; i < childClasses.Count; i++)
            {
                ChildComplexType child = (ChildComplexType)childClasses[i];
                ParseComplexTypePass0(child.ComplexType, child.DotnetClassName, child.Qname, currentNamespace, es);
            }
        }

        /*
         * Pass0. Collect references to globally defined complextypes and simpletypes. 
         */
        private void ParseGroupBasePass0(XmlSchemaGroupBase groupBase, String dotnetClassName, ArrayList childClasses, string parentNamespace, string currentNamespace, ElementsSubset es)
        {
            for (int i = 0; i < groupBase.Items.Count; i++)
            {
                if (groupBase.Items[i] is XmlSchemaElement)
                {
                    XmlSchemaElement elementRef = (XmlSchemaElement)groupBase.Items[i];
                    XmlSchemaElement element = (XmlSchemaElement)schema.Elements[elementRef.QualifiedName];
                    if (element == null) element = elementRef;
                    string ns = element.QualifiedName.Namespace != "" ? element.QualifiedName.Namespace : parentNamespace;

                    if (element.ElementType is XmlSchemaComplexType && element.SchemaTypeName.Namespace != Globals.XSD_NAMESPACE)
                    {
                        XmlSchemaComplexType elementComplex = (XmlSchemaComplexType)element.ElementType;

                        // The complex type is locally defined so a child class needs to be created.  
                        if ((element == elementRef) && (schema.SchemaTypes[elementComplex.QualifiedName] == null))
                        {
                            // recure these later
                            childClasses.Add(new ChildComplexType(elementComplex, element.Name, "", ns, element.QualifiedName));
                        }

                        if ((schema.Elements[elementRef.QualifiedName] != null) && (elementComplex.Name != null && elementComplex.Name != ""))
                        {
                            // global element who's name and type are set.  Both element and complextype are named.
                            if (!es.Elements.ContainsKey(elementRef.QualifiedName.Name))
                            {
                                // add element and recure it's complextype
                                es.Elements.Add(elementRef.QualifiedName.Name, elementRef.QualifiedName.Name);
                                if (!es.ComplexTypes.ContainsKey(elementComplex.QualifiedName.Name) && !string.IsNullOrEmpty(elementComplex.QualifiedName.Name))
                                {
                                    es.ComplexTypes.Add(elementComplex.QualifiedName.Name, elementComplex.QualifiedName.Name);
                                    ParseComplexTypePass0(elementComplex, elementComplex.QualifiedName.Name, elementComplex.QualifiedName, elementComplex.QualifiedName.Namespace, es);
                                }
                            }

                        }
                        else if (elementComplex.Name != null && elementComplex.Name != "")
                        {
                            // globally defined named schema type
                            if (!es.ComplexTypes.ContainsKey(elementComplex.QualifiedName.Name))
                            {
                                es.ComplexTypes.Add(elementComplex.QualifiedName.Name, elementComplex.QualifiedName.Name);
                                ParseComplexTypePass0(elementComplex, elementComplex.QualifiedName.Name, elementComplex.QualifiedName, elementComplex.QualifiedName.Namespace, es);
                            }
                        }
                        else
                        {
                            // global element complexType -- element is named, but complex type is not
                            if (!es.Elements.ContainsKey(elementRef.QualifiedName.Name))
                            {
                                // add element and recure it's complextype
                                es.Elements.Add(elementRef.QualifiedName.Name, elementRef.QualifiedName.Name);
                                ParseComplexTypePass0(elementComplex, elementRef.QualifiedName.Name, elementRef.QualifiedName, elementRef.QualifiedName.Namespace, es);
                            }
                        }
                    }
                    else
                    {
                        // not a ComplexType

                        string xsdTypeName = element.SchemaTypeName.Name;
                        string clrTypeName = code.FrameworkTypeMapping(xsdTypeName);

                        // build SimpleType enumeration if needed
                        if (element.ElementType is XmlSchemaSimpleType)
                        {
                            XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType)element.ElementType;
                            ParseEnumeration0(simpleType, element.Name, es);
                        }
                    }
                }
                else if (groupBase.Items[i] is XmlSchemaGroupRef)
                {
                    XmlSchemaGroup group = (XmlSchemaGroup)schema.Groups[((XmlSchemaGroupRef)groupBase.Items[i]).RefName];
                    ParseGroupBasePass0(group.Particle, dotnetClassName, childClasses, parentNamespace, currentNamespace, es);
                }
                else if (groupBase.Items[i] is XmlSchemaGroupBase)
                {
                    // Particle inside a particle : ie. <xsd:sequence> <xsd:choice/> </xsd:sequence>
                    ParseGroupBasePass0((XmlSchemaGroupBase)groupBase.Items[i], dotnetClassName, childClasses, parentNamespace, currentNamespace, es);
                }
            }
        }


        /*
         * Pass0: Gather enumeration types
         */
        private void ParseAttributesPass0(XmlSchemaObjectCollection attributes, ElementsSubset es)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i] is XmlSchemaAttributeGroupRef)
                {
                    XmlSchemaAttributeGroupRef attributeGroupRef = (XmlSchemaAttributeGroupRef)attributes[i];
                    XmlSchemaAttributeGroup attributeGroup = (XmlSchemaAttributeGroup)schema.AttributeGroups[attributeGroupRef.RefName];

                    ParseAttributesPass0(attributeGroup.Attributes, es);
                }
                else if (attributes[i] is XmlSchemaAttribute)
                {
                    XmlSchemaAttribute attributeRef = (XmlSchemaAttribute)attributes[i];
                    XmlSchemaAttribute attribute = (XmlSchemaAttribute)schema.Attributes[attributeRef.QualifiedName];
                    if (attribute == null) return;  //locally definied attributes -- we don't care about these

                    if (attribute.AttributeType is XmlSchemaSimpleType)
                    {
                        // build SimpleType enumeration if needed
                        XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType)attribute.AttributeType;
                        ParseEnumeration0(simpleType, attribute.Name, es);
                    }
                }
            }
        }

        /*
         * Pass0: Collect globally (schema) scoped enumerations.
         */
        private void ParseEnumeration0(XmlSchemaSimpleType simpleType, String name, ElementsSubset es)
        {
            if (simpleType.Name == null || simpleType.Name == "")
            {
                // locally scoped simpleType enumeration on an element or attribute.  We don't care about these here.	
                return;
            }
            else
            {
                // globally scoped simpleType at the schema level -- this is what we're searching for here
                name = simpleType.Name;
            }

            if (simpleType.Content is XmlSchemaSimpleTypeRestriction)
            {
                XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction)simpleType.Content;

                for (int j = 0; j < restriction.Facets.Count; j++)
                {
                    if (restriction.Facets[j] is XmlSchemaEnumerationFacet)
                    {
                        if (!es.SimpleTypes.ContainsKey(name)) es.SimpleTypes.Add(name, name);
                        return;
                    }
                }
            }
        }
        #endregion

        #region Pass1 Routines
        //********************************************************************************************
        //********************************************************************************************
        // PASS ONE ROUTINES
        //********************************************************************************************
        //********************************************************************************************

        /// <summary>
        /// Pass1. Collect info on classes, List<> types, and any local enumerations (globals have already been added).
        /// </summary>
        /// <param name="complex">Instance of a ComplexType</param>
        /// <param name="complexTypeName">Name of the complexType</param>
        /// <param name="parentClassStack">
        /// A stack of parent complexType names needed for nested complexTypes to ensure a unique class name gets generated.
        /// </param>
        /// <param name="qname">QualifiedName object for schema element or schema type</param>
        private void ParseComplexTypePass1(XmlSchemaComplexType complex, String dotnetClassName, ArrayList parentClassStack, XmlQualifiedName qname, string currentNamespace)
        {
            ArrayList childClasses = new ArrayList();
            parentClassStack.Add(dotnetClassName);

            // Check if class inherits from an abstract type.  If so, add it to the abstract types attached list inside globalQualifiedAbstractTypeClasses.
            XmlSchemaComplexType baseSchema = complex.BaseSchemaType as XmlSchemaComplexType;
            if (baseSchema != null)
            {
                Hashtable listOfDerivedTypes = null;
                listOfDerivedTypes = (Hashtable)globalQualifiedAbstractTypeClasses[baseSchema.QualifiedName];

                if (listOfDerivedTypes != null)
                {
                    if (!listOfDerivedTypes.ContainsKey(dotnetClassName))
                    {
                        listOfDerivedTypes.Add(dotnetClassName, currentNamespace);
                    }
                }
            }

            // Gather enumeration types
            ParseAttributesPass1(complex.Attributes);

            if (complex.Particle is XmlSchemaGroupBase)
            {
                ParseGroupBasePass1((XmlSchemaGroupBase)complex.Particle, dotnetClassName, childClasses, parentClassStack, complex.QualifiedName.Namespace, currentNamespace);
            }
            else if (complex.Particle is XmlSchemaGroupRef)
            {
                XmlSchemaGroup group = (XmlSchemaGroup)schema.Groups[((XmlSchemaGroupRef)complex.Particle).RefName];
                ParseGroupBasePass1(group.Particle, dotnetClassName, childClasses, parentClassStack, complex.QualifiedName.Namespace, currentNamespace);
            }
            else if (complex.ContentModel is XmlSchemaSimpleContent)
            {
                XmlSchemaSimpleContent simpleContent = (XmlSchemaSimpleContent)complex.ContentModel;
                if (simpleContent.Content is XmlSchemaSimpleContentRestriction)
                {
                    XmlSchemaSimpleContentRestriction contentRestriction = (XmlSchemaSimpleContentRestriction)simpleContent.Content;
                    if (contentRestriction.Attributes.Count > 0)
                        ParseAttributesPass1(contentRestriction.Attributes);
                    else if (contentRestriction.Facets.Count > 0)
                    {
                        // this is a complexType who's mixed type is an unnamed enumeration -- so return after collecting enum data.  Odd scenario.
                        /* example
                            <xs:complexType name="CancelEvidenceObtained">
		                        <xs:simpleContent>
			                        <xs:restriction base="OpenEnum">
				                        <xs:enumeration value="N"/>
				                        <xs:enumeration value="O"/>
			                        </xs:restriction>
		                        </xs:simpleContent>
	                        </xs:complexType>
                         */
                        ParseFacets(contentRestriction.Facets, dotnetClassName + "Enum");  // use a special Enum suffix -- otherwise the Enum type will have the same type name as the class
                        return;
                    }
                }
                else if (simpleContent.Content is XmlSchemaSimpleContentExtension)
                {
                    XmlSchemaSimpleContentExtension contentExtension = (XmlSchemaSimpleContentExtension)simpleContent.Content;
                    ParseAttributesPass1(contentExtension.Attributes);
                }
            }
            else if (complex.ContentModel is XmlSchemaComplexContent)
            {
                XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent)complex.ContentModel;
                if (complexContent.Content is XmlSchemaComplexContentRestriction)
                {
                    XmlSchemaComplexContentRestriction contentRestriction = (XmlSchemaComplexContentRestriction)complexContent.Content;

                    if (contentRestriction.Attributes != null)
                        ParseAttributesPass1(contentRestriction.Attributes);

                    if (contentRestriction.Particle != null)
                    {
                        if (contentRestriction.Particle is XmlSchemaGroupBase)
                            ParseGroupBasePass1((XmlSchemaGroupBase)contentRestriction.Particle, dotnetClassName, childClasses, parentClassStack, complex.QualifiedName.Namespace, currentNamespace);
                        else if (contentRestriction.Particle is XmlSchemaGroupRef)
                        {
                            XmlSchemaGroup group = (XmlSchemaGroup)schema.Groups[((XmlSchemaGroupRef)contentRestriction.Particle).RefName];
                            ParseGroupBasePass1(group.Particle, dotnetClassName, childClasses, parentClassStack, complex.QualifiedName.Namespace, currentNamespace);
                        }
                    }
                }
                else if (complexContent.Content is XmlSchemaComplexContentExtension)
                {
                    XmlSchemaComplexContentExtension contentExtension = (XmlSchemaComplexContentExtension)complexContent.Content;

                    if (contentExtension.Attributes != null)
                        ParseAttributesPass1(contentExtension.Attributes);
                    if (contentExtension.Particle != null)
                    {
                        if (contentExtension.Particle is XmlSchemaGroupBase)
                            ParseGroupBasePass1((XmlSchemaGroupBase)contentExtension.Particle, dotnetClassName, childClasses, parentClassStack, complex.QualifiedName.Namespace, currentNamespace);
                        else if (contentExtension.Particle is XmlSchemaGroupRef)
                        {
                            XmlSchemaGroup group = (XmlSchemaGroup)schema.Groups[((XmlSchemaGroupRef)contentExtension.Particle).RefName];
                            ParseGroupBasePass1(group.Particle, dotnetClassName, childClasses, parentClassStack, complex.QualifiedName.Namespace, currentNamespace);
                        }
                    }
                }
            }

            for (int i = 0; i < childClasses.Count; i++)
            {
                ChildComplexType child = (ChildComplexType)childClasses[i];
                ParseComplexTypePass1(child.ComplexType, child.DotnetClassName, parentClassStack, child.Qname, currentNamespace);
            }
        }

        /*
         * Pass1. Collect info on classes, collections (List<> types), and any local enumerations. 
         */
        private void ParseGroupBasePass1(XmlSchemaGroupBase groupBase, String dotnetClassName, ArrayList childClasses, ArrayList parentClassStack, string parentNamespace, string currentNamespace)
        {
            for (int i = 0; i < groupBase.Items.Count; i++)
            {
                if (groupBase.Items[i] is XmlSchemaElement)
                {
                    XmlSchemaElement elementRef = (XmlSchemaElement)groupBase.Items[i];
                    XmlSchemaElement element = (XmlSchemaElement)schema.Elements[elementRef.QualifiedName];
                    if (element == null) element = elementRef;
                    string ns = element.QualifiedName.Namespace != "" ? element.QualifiedName.Namespace : parentNamespace;

                    if (element.ElementType is XmlSchemaComplexType && element.SchemaTypeName.Namespace != Globals.XSD_NAMESPACE)
                    {
                        XmlSchemaComplexType elementComplex = (XmlSchemaComplexType)element.ElementType;

                        string childComplexTypeName = "";  // a nested complexType, who's name may not be unique in the schema

                        // The complex type is locally defined so a child class needs to be created.  
                        if ((element == elementRef) && (schema.SchemaTypes[elementComplex.QualifiedName] == null))
                        {
                            childComplexTypeName = CalculateNestedChildTypeName(element.Name, globalComplexTypeClasses, parentClassStack);
                            childClasses.Add(new ChildComplexType(elementComplex, element.Name, childComplexTypeName, ns, element.QualifiedName));
                            globalComplexTypeClasses.Add(childComplexTypeName, childComplexTypeName);

                            // local complexType elements can repeat within a namespace
                            string parentList = "";
                            foreach (string s in parentClassStack)
                            {
                                parentList += s;
                            }
                            globalQualifiedComplexTypeClasses.Add(element.QualifiedName + parentList, childComplexTypeName);
                        }

                        // For "unbounded" types -- check to see if this fits the IEnumerator pattern.  Collect support for IEnumerator here.
                        if (elementRef.MaxOccurs > 1 || groupBase.MaxOccurs > 1)
                        {
                            // If this is the only element in a contained ComplexType, and this element
                            //   is unbounded, then we can support IEnumerator on the contained class
                            //   so we can easilly enumerate through the contained child collection.
                            string enumerableClass = dotnetClassName;
                            if (childComplexTypeName != "")
                                enumerableClass = childComplexTypeName;
                            if (groupBase.Items.Count == 1 && !enumerableClasses.ContainsKey(enumerableClass))
                            {
                                // enumerable class

                                ArrayList values = new ArrayList();
                                values.Add(element.Name);  //name

                                // type
                                if (childComplexTypeName != "")
                                {
                                    values.Add(childComplexTypeName);  // nested local complextype
                                }
                                else if ((schema.Elements[elementRef.QualifiedName] != null) && (elementComplex.Name != null && elementComplex.Name != ""))
                                {
                                    string q = AddQualifiedNamespaceReference(element.Name, element.QualifiedName.Namespace, currentNamespace, GlobalXsdType.Element);
                                    values.Add(q);  //global element who's name and type are set
                                }
                                else if (elementComplex.Name != null && elementComplex.Name != "")
                                {
                                    string q = AddQualifiedNamespaceReference(elementComplex.Name, elementComplex.QualifiedName.Namespace, currentNamespace, GlobalXsdType.ComplexType);
                                    values.Add(q);     // global named schema type
                                }
                                else
                                {
                                    string q = AddQualifiedNamespaceReference(element.Name, element.QualifiedName.Namespace, currentNamespace, GlobalXsdType.Element);
                                    values.Add(q);  // global element complextype
                                }

                                enumerableClasses.Add(dotnetClassName, values);
                            }
                        }

                        // collect support for abstract classes
                        if (elementComplex.IsAbstract)
                        {
                            ArrayList abstractClasses = null;
                            if (classesReferencingAbstractTypes[dotnetClassName] == null)
                            {
                                abstractClasses = new ArrayList();
                                abstractClasses.Add(elementComplex.QualifiedName);
                                classesReferencingAbstractTypes.Add(dotnetClassName, abstractClasses);
                            }
                            else
                            {
                                abstractClasses = (ArrayList)classesReferencingAbstractTypes[dotnetClassName];
                                abstractClasses.Add(elementComplex.QualifiedName);
                            }
                        }
                    }
                    else
                    {
                        // not a ComplexType

                        string xsdTypeName = element.SchemaTypeName.Name;
                        string clrTypeName = code.FrameworkTypeMapping(xsdTypeName);

                        // build SimpleType enumeration if needed
                        if (element.ElementType is XmlSchemaSimpleType)
                        {
                            XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType)element.ElementType;

                            if (Globals.globalSeparateImportedNamespaces)
                            {
                                // if this is an external imported simpleType element, then it get's added into its appropriate .net source file
                                if (simpleType.QualifiedName.Namespace == "" || (element.QualifiedName.Namespace == currentNamespace && simpleType.QualifiedName.Namespace == currentNamespace))
                                    ParseEnumeration1(simpleType, element.Name);
                            }
                            else
                                ParseEnumeration1(simpleType, element.Name);
                        }

                        // For "unbounded" types, add them to collectionClasses container.  We will build
                        //  special ArrayList subclasses for each of these later.
                        if (elementRef.MaxOccurs > 1 || groupBase.MaxOccurs > 1)
                        {
                            if (element.ElementType is XmlSchemaSimpleType)
                            {
                                XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType)element.ElementType;

                                // if this is the only element in a contained ComplexType, and this element
                                //   is unbounded, then we can implement IEnumerator on the contained class
                                //   so we can easilly enumerate through the child collection.
                                if (groupBase.Items.Count == 1 && !enumerableClasses.ContainsKey(dotnetClassName))
                                {
                                    ArrayList values = new ArrayList();
                                    values.Add(element.Name);  //name
                                    if (IsEnumeration(simpleType))
                                    {
                                        if (simpleType.Name != null)
                                        {
                                            // colinco: 2005-07-05: Enums referenced from external .net namespace aren't prefixed with the .net namespace in ArrayLists or enumerable classes. 
                                            string n = AddQualifiedNamespaceReference(simpleType.QualifiedName.Name, simpleType.QualifiedName.Namespace, currentNamespace, GlobalXsdType.Enum);

                                            values.Add(n);
                                        }
                                        else // simpleType locally defined inline within an element
                                            values.Add(element.Name);  //type
                                    }
                                    else
                                        values.Add(simpleType.Datatype.ValueType.FullName);  //type
                                    enumerableClasses.Add(dotnetClassName, values);
                                }
                            }
                            else
                            {
                                // if this is the only element in a contained ComplexType, and this element
                                //   is unbounded, then we can implement IEnumerator on the contained class
                                //   so we can easilly enumerate through the child collection.
                                if (groupBase.Items.Count == 1 && !enumerableClasses.ContainsKey(dotnetClassName))
                                {
                                    ArrayList values = new ArrayList();
                                    values.Add(element.Name);  //name
                                    values.Add(clrTypeName);  //type
                                    enumerableClasses.Add(dotnetClassName, values);
                                }
                            }
                        }
                    }
                }
                else if (groupBase.Items[i] is XmlSchemaAny)
                {
                    //XmlSchemaAny any = (XmlSchemaAny) groupBase.Items[i];
                    // do nothing for xsd:any.  Add a special array instead of a collection.
                }
                else if (groupBase.Items[i] is XmlSchemaGroupRef)
                {
                    XmlSchemaGroup group = (XmlSchemaGroup)schema.Groups[((XmlSchemaGroupRef)groupBase.Items[i]).RefName];
                    ParseGroupBasePass1(group.Particle, dotnetClassName, childClasses, parentClassStack, parentNamespace, currentNamespace);
                }
                else if (groupBase.Items[i] is XmlSchemaGroupBase)
                {
                    // Particle inside a particle : ie. <xsd:sequence> <xsd:choice/> </xsd:sequence>
                    ParseGroupBasePass1((XmlSchemaGroupBase)groupBase.Items[i], dotnetClassName, childClasses, parentClassStack, parentNamespace, currentNamespace);
                }
            }
        }

        /*
         * Gather enumeration types, etc.  Called in Pass1
         */
        private void ParseAttributesPass1(XmlSchemaObjectCollection attributes)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i] is XmlSchemaAttributeGroupRef)
                {
                    XmlSchemaAttributeGroupRef attributeGroupRef = (XmlSchemaAttributeGroupRef)attributes[i];
                    XmlSchemaAttributeGroup attributeGroup = (XmlSchemaAttributeGroup)schema.AttributeGroups[attributeGroupRef.RefName];

                    ParseAttributesPass1(attributeGroup.Attributes);
                }
                else if (attributes[i] is XmlSchemaAttribute)
                {
                    XmlSchemaAttribute attributeRef = (XmlSchemaAttribute)attributes[i];
                    XmlSchemaAttribute attribute = (XmlSchemaAttribute)schema.Attributes[attributeRef.QualifiedName];
                    if (attribute == null) attribute = attributeRef;  //locally definied attributes

                    object o = null;
                    try
                    {
                        o = attribute.AttributeType;
                    }
                    catch { }

                    if (o != null && o is XmlSchemaSimpleType)
                    {
                        // build SimpleType enumeration if needed
                        XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType)attribute.AttributeType;
                        ParseEnumeration1(simpleType, attribute.Name);
                    }
                }
            }
        }


        /*
         * Collect enumeration info from the SimpleType.  Called in Pass1 only.  Assumption is that the SimpleType is an enumeration.
         */
        private string ParseEnumeration1(XmlSchemaSimpleType simpleType, String name)
        {
            name = LanguageBase.ReplaceInvalidChars(name);

            if (simpleType.Name == null || simpleType.Name == "")
            {
                // locally scoped simpleType enumeration on an element or attribute.  Make sure name is unique since these can be reused.	
                if (enumerations.ContainsKey(name))
                {
                    // name has already been used, so calculate a new unique name for this locally scoped simpleType enum
                    name = CalculateUniqueTypeOrFieldName(name, "", enumerations);
                }
            }
            else
            {
                // globally scoped simpleType at the schema level
                name = LanguageBase.ReplaceInvalidChars(simpleType.Name);
            }

            if (simpleType.Content is XmlSchemaSimpleTypeRestriction)
            {
                XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction)simpleType.Content;
                name = ParseFacets(restriction.Facets, name);
            }
            else if (simpleType.Content is XmlSchemaSimpleTypeUnion) // union  (XmlSchemaSimpleTypeUnion)
            {
                XmlSchemaSimpleTypeUnion union = (XmlSchemaSimpleTypeUnion)simpleType.Content;

                if (union.BaseMemberTypes != null)
                {
                    foreach (XmlSchemaSimpleType unionType in union.BaseMemberTypes)
                    {
                        if (IsEnumeration(unionType))
                        {
                            XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction)unionType.Content;
                            name = ParseFacets(restriction.Facets, name);
                        }
                    }
                }
            }

            return name;
        }

        private string ParseFacets(XmlSchemaObjectCollection facets, string name)
        {
            ArrayList enumList = null;

            for (int j = 0; j < facets.Count; j++)
            {
                if (facets[j] is XmlSchemaEnumerationFacet)
                {
                    if (enumList == null)
                    {
                        // if enumeration is already been added, then return
                        if (enumerations.ContainsKey(name))
                            return name;

                        enumList = new ArrayList();
                        enumerations.Add(name, enumList);
                    }

                    XmlSchemaEnumerationFacet enumeration = (XmlSchemaEnumerationFacet)facets[j];
                    if (enumeration.Value != null && enumeration.Value.Trim() != "")
                    {
                        String[] enumValue = new string[2];
                        enumValue[0] = enumeration.Value.Trim();
                        enumValue[1] = enumeration.Value.Trim();
                        enumValue[1] = enumValue[1].Replace(' ', '_');
                        if (((ushort)enumValue[1][0]) >= 48 && ((ushort)enumValue[1][0]) <= 57)
                            enumValue[1] = LanguageBase.renameItemPrefix + enumValue[1];

                        enumList.Add(enumValue);
                    }
                }
            }

            return name;
        }
        #endregion

        #region Pass2 Routines
        //********************************************************************************************
        //********************************************************************************************
        // PASS TWO ROUTINES
        //********************************************************************************************
        //********************************************************************************************

        /// <summary>
        /// Build code for a ComplexType -- which translates to an object class with fields
        /// </summary>
        /// <param name="complex">Instance of ComplexType</param>
        /// <param name="className">Name of the .net class</param>
        /// <param name="elementName">Name of the element -- possibly different than the class.</param>
        /// <param name="isSchemaType">
        /// True for globally defined complexTypes that are not linked to an xml element.
        /// We will use the XmlTypeAttribute for these, and the XmlRootAttribute for complexTypes
        ///	that are tied to an Element (where isSchemaType is false).</param>
        /// <param name="classNamespace">Namespace of the schema type or element</param>
        /// <param name="parentClassStack">
        /// A stack of parent owner complexType names needed for nested complexTypes to ensure a unique class name gets generated.
        /// </param>
        /// <param name="typedGlobalElement">Global element who's type is a global schema type (it doesn't have any xml child nodes in the schema)</param>
        /// <param name="isElementNullable">Set the IsNullable element paramter = true, causing xsi:nil="true" for null elements</param>
        /// <param name="globalElementAndSchemaTypeHaveSameName">For global schema types where an existing global element also has the same name</param>
        private void ParseComplexTypePass2(XmlSchemaComplexType complex, string className, string elementName,
            bool isSchemaType, bool isLocalComplexType, string classNamespace, ArrayList parentClassStack,
            string typedGlobalElement, string typedGlobalElementNamespace, bool isElementNullable, bool globalElementAndSchemaTypeHaveSameName)
        {
            // If the ComplexType has any privately defined child ComplexTypes, these need to be classes
            //  in code as well.  So we will collect these as the ComplexType is created, then recurse 
            //  through the list at the of this method -- to generate any required child classes.
            ArrayList childClasses = new ArrayList();
            parentClassStack.Add(className);

            // Gather any attributes or elements with DefaultValue or FixedValue attributes set to
            //	set these in a class constructor.
            ArrayList ctorList = new ArrayList();

            // Gather names of fields for a particular .net class being added.  Make sure duplicate fields aren't being
            //   added when an attribute has the same name as an element, or when an imported schema has fields with the 
            //   the same name.
            Hashtable dotnetFieldList = new Hashtable();  //key -- .net field name

            // Add code for the class declaration
            string baseClass = "";
            bool baseIsMixed = false;
            string baseNs = "";
            bool baseIsAbstract = false;
            XmlSchemaType baseSchema = complex.BaseSchemaType as XmlSchemaType;
            if (typedGlobalElement != "")
            {
                baseClass = typedGlobalElement;
                baseNs = typedGlobalElementNamespace;
            }
            else if (baseSchema != null)
            {
                if (baseSchema is XmlSchemaComplexType)
                {
                    baseClass = baseSchema.Name;
                    baseNs = baseSchema.QualifiedName.Namespace;
                    baseIsAbstract = ((XmlSchemaComplexType)baseSchema).IsAbstract;
                }
            }

            // Get list of abstract decendant class types to be included -- if this class references any abstract ComplexTypes.
            ArrayList xmlIncludedClasses = new ArrayList();
            ArrayList abstractClasses = new ArrayList();
            abstractClasses = (ArrayList)classesReferencingAbstractTypes[className];
            if (abstractClasses != null)
            {
                foreach (XmlQualifiedName abstractClass in abstractClasses)
                {
                    Hashtable abstractDerivedClasses = (Hashtable)globalQualifiedAbstractTypeClasses[abstractClass];
                    foreach (string includedClass in abstractDerivedClasses.Keys)
                    {
                        string ns = (string)xsdNsToClrNs[(string)abstractDerivedClasses[includedClass]];
                        xmlIncludedClasses.Add(ns + "." + includedClass);
                    }
                }
            }

            /*** Perform special Mixed processing -- START ***/
            bool isMixed = false;
            string mixedType = "string";	// xsd string type.  This is the default mixed type.  Mixed means that the class element itself can contain data -- and we build an XmlText attribute to support this.

            if ((complex.IsMixed) || (complex.ContentModel is XmlSchemaSimpleContent))
            {
                // if simpleContent extension or simpleContent restriction

                isMixed = true;

                if (complex.ContentModel is XmlSchemaSimpleContent)
                {
                    XmlSchemaSimpleContent simpleContent = (XmlSchemaSimpleContent)complex.ContentModel;
                    if (simpleContent.Content is XmlSchemaSimpleContentExtension)
                    {
                        XmlSchemaSimpleContentExtension contentExtension = (XmlSchemaSimpleContentExtension)simpleContent.Content;
                        if (contentExtension.BaseTypeName.Namespace.StartsWith("http://www.w3.org"))
                        {
                            // Set the type for Xsd types.  For non xsd types, just revert to "string". 
                            mixedType = contentExtension.BaseTypeName.Name;
                        }
                        else if (baseSchema != null && baseSchema.Datatype.ValueType.FullName.StartsWith("System."))
                        {
                            mixedType = baseSchema.Datatype.ValueType.FullName;

                            if (baseSchema.Datatype.ValueType.FullName == "System.Object")
                            {
                                /*
                                   <xsd:complexType name="DateTime">
                                      <xsd:simpleContent>
                                         <xsd:extension base="DateTime_NoID">
                                            <xsd:attribute name="id" type="ID"/>
                                         </xsd:extension>
                                      </xsd:simpleContent>
                                   </xsd:complexType>
                                   <xsd:simpleType name="DateTime_NoID">
                                      <xsd:union memberTypes="xsd:dateTime xsd:string "/>
                                   </xsd:simpleType>

                                   <xsd:element name="TransactionRequestDt" type="DateTime"/>
                                 */

                                XmlSchemaObject o = schema.SchemaTypes[contentExtension.BaseTypeName];

                                if (o != null && o is XmlSchemaSimpleType)
                                {
                                    XmlSchemaSimpleType stUnion = (XmlSchemaSimpleType)o;
                                    if (stUnion.Content is XmlSchemaSimpleTypeUnion)
                                    {
                                        XmlSchemaSimpleTypeUnion union = (XmlSchemaSimpleTypeUnion)stUnion.Content;
                                        if (union.BaseMemberTypes != null)
                                        {
                                            foreach (XmlSchemaSimpleType unionType in union.BaseMemberTypes)
                                            {
                                                if (unionType.Datatype.ValueType != null && unionType.Datatype.ValueType.FullName != "System.String")
                                                {
                                                    mixedType = unionType.Datatype.ValueType.FullName;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (simpleContent.Content is XmlSchemaSimpleContentRestriction)
                    {
                        // looking for this odd scenerio -- schema level complexType who's mixed type is an unnamed enumeration
                        /* example
                            <xs:complexType name="CancelEvidenceObtained">
		                        <xs:simpleContent>
			                        <xs:restriction base="OpenEnum">
				                        <xs:enumeration value="N"/>
				                        <xs:enumeration value="O"/>
			                        </xs:restriction>
		                        </xs:simpleContent>
	                        </xs:complexType>
                         */
                        XmlSchemaSimpleContentRestriction contentRestriction = (XmlSchemaSimpleContentRestriction)simpleContent.Content;
                        if (string.IsNullOrEmpty(typedGlobalElement) && (contentRestriction.Facets.Count > 0))
                        {
                            // don't do this for typedGlobalElement cases

                            baseClass = "";  // reset the baseclass to nothing as this scenario can't have a base class.
                            mixedType = className + "Enum"; // use a special suffix -- otherwise the Enum type will have the same typename as the class
                        }
                    }
                }
            }

            // Special case where you have a simpleContent (mixed) class that descends from a simpleContent (mixed) class.  Can only have one XmlText attribute
            //  in the class hierarchy -- or xmlserializer complains/
            if (baseClass != "" && baseSchema != null)
            {
                if (baseSchema is XmlSchemaComplexType)
                {
                    if ((baseSchema as XmlSchemaComplexType).ContentModel is XmlSchemaSimpleContent)
                    {
                        baseIsMixed = true;
                    }
                }
            }
            /*** Perform special Mixed processing -- END ***/


            className = LanguageBase.ReplaceInvalidChars(className);

            if (baseClass != null && baseClass != "")
            {
                baseClass = AddQualifiedNamespaceReference(baseClass, baseNs, classNamespace, GlobalXsdType.ComplexType);
                baseClass = LanguageBase.ReplaceInvalidChars(baseClass);
            }

            // write out the class header code
            code.ClassHeaderCode(outStream, className, elementName, baseClass, baseIsAbstract, isSchemaType, complex.IsAbstract, isLocalComplexType, enumerableClasses,
                classNamespace, elementFormDefault, "", isElementNullable, xmlIncludedClasses, globalElementAndSchemaTypeHaveSameName);

            Hashtable acordSetters = new Hashtable();

            // special ACORD transaction code processing
            if (acordLookup != null)
            {
                if (elementName.StartsWith("OLI_LU_"))
                {
                    XmlNode parent = acordLookup.SelectSingleNode("//LookupTypes/Lookup[@name='" + elementName + "']");
                    if (parent != null) // && parent.Name == "OLI_LU_PARTY")
                    {
                        foreach (XmlNode node in parent.ChildNodes)
                        {
                            if (node.ChildNodes.Count == 2)
                            {
                                XmlNode nCode = node.ChildNodes[0];
                                XmlNode nDesc = node.ChildNodes[1];

                                string setterName = node.Attributes["name"].Value;
                                if (acordSetters[setterName] != null)
                                    setterName = setterName + "2";
                                acordSetters.Add(setterName, setterName);
                                code.AcordTransactCodes(outStream, setterName, elementName, nCode.InnerText, nDesc.InnerText, false);
                            }
                        }
                    }
                }
            }

            // special ACORD private transaction code processing
            if (acordLookupPrivate != null)
            {
                if (elementName.StartsWith("OLI_LU_"))
                {
                    XmlNode parent = acordLookupPrivate.SelectSingleNode("//LookupTypes/Lookup[@name='" + elementName + "']");
                    if (parent != null) // && parent.Name == "OLI_LU_PARTY")
                    {
                        foreach (XmlNode node in parent.ChildNodes)
                        {
                            if (node.ChildNodes.Count == 2)
                            {
                                XmlNode nCode = node.ChildNodes[0];
                                XmlNode nDesc = node.ChildNodes[1];

                                string setterName = node.Attributes["name"].Value;
                                if (acordSetters[setterName] != null)
                                    setterName = setterName + "2";
                                acordSetters.Add(setterName, setterName);
                                code.AcordTransactCodes(outStream, setterName, elementName, nCode.InnerText, nDesc.InnerText, true);
                            }
                        }
                    }
                }
            }


            // if an element exists at the global level below <schema> that has it's type attribute set, do nothing more as the 
            //  shema type class will already exist, and this class will inherit from it.
            if (typedGlobalElement != "")
            {
                code.ClassTrailerCode(outStream, className, new ArrayList(), false, optionDepthFirstTraversalHooks, optionConstructRequiredSchema, baseClass, baseIsMixed, false, "");
                return;
            }

            // Add fields for any attributes associated with this complex type
            ParseAttributesPass2(complex.Attributes, complex.AnyAttribute, ctorList, dotnetFieldList, classNamespace);

            /*
             * <xsd:complexType> can have one and only one of the following child elements
             *  -simpleContent
             *  -complexContent
             *  -group
             *  -sequence
             *  -choice
             *  -all
             */

            if (complex.Particle is XmlSchemaGroupBase)
            {
                // Add fields for xsd:all, xsd:choice and xsd:sequence elements
                ParseGroupBasePass2((XmlSchemaGroupBase)complex.Particle, className, ctorList, childClasses, parentClassStack, dotnetFieldList,
                    classNamespace, null);
            }
            else if (complex.Particle is XmlSchemaGroupRef)
            {
                XmlSchemaGroup group = (XmlSchemaGroup)schema.Groups[((XmlSchemaGroupRef)complex.Particle).RefName];
                ParseGroupBasePass2(group.Particle, className, ctorList, childClasses, parentClassStack, dotnetFieldList,
                    classNamespace, null);
            }
            else if (complex.ContentModel is XmlSchemaSimpleContent)
            {
                XmlSchemaSimpleContent simpleContent = (XmlSchemaSimpleContent)complex.ContentModel;
                if (simpleContent.Content is XmlSchemaSimpleContentRestriction)
                {
                    //attribute, attributeGroup, anyAttribute
                    XmlSchemaSimpleContentRestriction contentRestriction = (XmlSchemaSimpleContentRestriction)simpleContent.Content;
                    ParseAttributesPass2(contentRestriction.Attributes, contentRestriction.AnyAttribute, ctorList, dotnetFieldList, classNamespace);
                }
                else if (simpleContent.Content is XmlSchemaSimpleContentExtension)
                {
                    //attribute, attributeGroup, anyAttribute
                    XmlSchemaSimpleContentExtension contentExtension = (XmlSchemaSimpleContentExtension)simpleContent.Content;
                    ParseAttributesPass2(contentExtension.Attributes, contentExtension.AnyAttribute, ctorList, dotnetFieldList, classNamespace);
                }
            }
            else if (complex.ContentModel is XmlSchemaComplexContent)
            {
                XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent)complex.ContentModel;
                if (complexContent.Content is XmlSchemaComplexContentRestriction)
                {
                    //group, all, choice, sequence, attribute, attributeGroup, anyAttribute
                    XmlSchemaComplexContentRestriction contentRestriction = (XmlSchemaComplexContentRestriction)complexContent.Content;

                    if (contentRestriction.Attributes != null)
                        ParseAttributesPass2(contentRestriction.Attributes, contentRestriction.AnyAttribute, ctorList, dotnetFieldList, classNamespace);
                    if (contentRestriction.Particle != null)
                    {
                        if (contentRestriction.Particle is XmlSchemaGroupBase)
                        {
                            ParseGroupBasePass2((XmlSchemaGroupBase)contentRestriction.Particle, className, ctorList, childClasses, parentClassStack, dotnetFieldList,
                                classNamespace, null);
                        }
                        else if (contentRestriction.Particle is XmlSchemaGroupRef)
                        {
                            XmlSchemaGroup group = (XmlSchemaGroup)schema.Groups[((XmlSchemaGroupRef)contentRestriction.Particle).RefName];
                            ParseGroupBasePass2(group.Particle, className, ctorList, childClasses, parentClassStack, dotnetFieldList, classNamespace, null);
                        }
                    }
                }
                else if (complexContent.Content is XmlSchemaComplexContentExtension)
                {
                    //attribute, attributeGroup, anyAttribute, choice, all, sequence, group
                    XmlSchemaComplexContentExtension contentExtension = (XmlSchemaComplexContentExtension)complexContent.Content;

                    if (contentExtension.Attributes != null)
                        ParseAttributesPass2(contentExtension.Attributes, contentExtension.AnyAttribute, ctorList, dotnetFieldList, classNamespace);
                    if (contentExtension.Particle != null)
                    {
                        if (contentExtension.Particle is XmlSchemaGroupBase)
                        {
                            ParseGroupBasePass2((XmlSchemaGroupBase)contentExtension.Particle, className, ctorList, childClasses, parentClassStack, dotnetFieldList,
                                classNamespace, null);
                        }
                        else if (contentExtension.Particle is XmlSchemaGroupRef)
                        {
                            XmlSchemaGroup group = (XmlSchemaGroup)schema.Groups[((XmlSchemaGroupRef)contentExtension.Particle).RefName];
                            ParseGroupBasePass2(group.Particle, className, ctorList, childClasses, parentClassStack, dotnetFieldList, classNamespace, null);
                        }
                    }
                }
            }

            // Add trailer class code
            code.ClassTrailerCode(outStream, className, ctorList, optionDefaultInitialization, optionDepthFirstTraversalHooks, optionConstructRequiredSchema, baseClass, baseIsMixed, isMixed, mixedType);

            // Now add the child classes collected when creating this parent class
            for (int i = 0; i < childClasses.Count; i++)
            {
                ChildComplexType child = (ChildComplexType)childClasses[i];
                // recurse -- and pass in the nested child class name (that may be modified from the schema name)
                ParseComplexTypePass2(child.ComplexType, child.DotnetClassName, child.ElementName, false, true, child.Namespace, parentClassStack, "", "", false, false);
            }
        }

        /*
         * Add class fields for xsd:all, xsd:choice and xsd:sequence xml elements.
         * These will either be native CLR typed fields or other schema classes.
         */
        private void ParseGroupBasePass2(XmlSchemaGroupBase groupBase, String className, ArrayList ctorList,
            ArrayList childClasses, ArrayList parentClassStack, Hashtable dotnetFieldList, string parentNamespace, Hashtable classReferencesAdded)
        {
            if (classReferencesAdded == null) classReferencesAdded = new Hashtable();  // avoid duplicate complexType field references within a .net class

            for (int i = 0; i < groupBase.Items.Count; i++)
            {
                decimal maxOccurs = groupBase.MaxOccurs;

                if (groupBase.Items[i] is XmlSchemaElement)
                {
                    XmlSchemaElement elementRef = (XmlSchemaElement)groupBase.Items[i];
                    XmlSchemaElement element = (XmlSchemaElement)schema.Elements[elementRef.QualifiedName];
                    if (element == null) element = elementRef;
                    maxOccurs = elementRef.MaxOccurs > maxOccurs ? elementRef.MaxOccurs : maxOccurs;

                    // for unqualified elements or attributes, element/attribute.QualifiedName.Namespace will be empty -- so use parentNamespace
                    string ns = element.QualifiedName.Namespace != "" ? element.QualifiedName.Namespace : parentNamespace;

                    if (element.ElementType is XmlSchemaComplexType && element.SchemaTypeName.Namespace != Globals.XSD_NAMESPACE)
                    {
                        // this will generate to a class in code

                        string dotnetTypeName = "";
                        XmlSchemaComplexType elementComplex = (XmlSchemaComplexType)element.ElementType;

                        // avoid duplicate references for the same type (allowed in xsd -- not in .net)
                        string fname = element.Name;
                        if (classReferencesAdded.ContainsKey(fname))
                        {
                            continue;
                            //fname = element.Name + "2";
                            //while (classReferencesAdded.ContainsKey(fname))
                            //    fname = fname + "2";
                        }
                        else
                            classReferencesAdded.Add(element.Name, element.Name);

                        // The complex type is locally defined so a child class needs to be created.  
                        if ((element == elementRef) && (schema.SchemaTypes[elementComplex.QualifiedName] == null))
                        {
                            // The complex type is locally defined.  Locally scoped complexTypes may not be unique 
                            string parentList = "";
                            foreach (string s in parentClassStack)
                            {
                                parentList += s;
                            }
                            dotnetTypeName = (string)globalQualifiedComplexTypeClasses[element.QualifiedName + parentList];
                            childClasses.Add(new ChildComplexType(elementComplex, element.Name, dotnetTypeName, ns, element.QualifiedName));

                            dotnetTypeName = LanguageBase.ReplaceInvalidChars(dotnetTypeName);
                            code.ClassComplexTypeFieldCode(outStream, element.Name, fname, dotnetTypeName, dotnetTypeName, className,
                                maxOccurs, 1, elementFormDefault, ns, element.IsNillable, false);
                        }
                        else  // not a locally defined complexType 
                        {
                            // The complexType is either globally defined at the <xsd:schema> level,
                            // or it's tied to a globally defined element.  In the case of the 
                            // globally defined complexType at the <xsd:schema> level, elementComplex.Name 
                            // will not be null because the complexType will have name.  In that case 
                            // we use the complexType name for the class name.

                            string qualifiedTypeName = "";

                            if ((elementComplex.QualifiedName.Name == null || elementComplex.QualifiedName.Name == "") &&
                                (elementRef.QualifiedName.Name != null && elementRef.QualifiedName.Name != ""))
                            {
                                // global element
                                ns = elementRef.QualifiedName.Namespace;  // element's referenced namespace is used
                                //qualifiedTypeName = (string) globalQualifiedComplexTypeClasses[elementRef.QualifiedName];
                                qualifiedTypeName = GlobalElementToClrMap(elementRef.QualifiedName);
                                dotnetTypeName = AddQualifiedNamespaceReference(element.Name, ns, parentNamespace, GlobalXsdType.Element);
                            }
                            else if ((schema.Elements[elementRef.QualifiedName] != null) && (elementComplex.QualifiedName.Name != null && elementComplex.QualifiedName.Name != ""))
                            {
                                // An element who's "ref" attribute points to a global typed element (name and type are set and has no schema children)
                                //  use the ref in this case -- and not the type
                                ns = elementRef.QualifiedName.Namespace;  // element's referenced namespace is used
                                //qualifiedTypeName = (string) globalQualifiedComplexTypeClasses[elementRef.QualifiedName];
                                qualifiedTypeName = GlobalElementToClrMap(elementRef.QualifiedName);
                                dotnetTypeName = AddQualifiedNamespaceReference(element.Name, ns, parentNamespace, GlobalXsdType.Element);
                            }
                            else if (elementRef.QualifiedName.Name != null && elementRef.QualifiedName.Name != "")
                            {
                                // named complexType
                                ns = parentNamespace;  //elements of a "type" take their parent namespace.  The children go into the "types" namespace.
                                qualifiedTypeName = (string)globalQualifiedComplexTypeClasses[elementComplex.QualifiedName];
                                dotnetTypeName = elementComplex.Name;

                                if (elementComplex.QualifiedName.Namespace != ns && elementComplex.QualifiedName.Namespace != null)
                                    dotnetTypeName = AddQualifiedNamespaceReference(dotnetTypeName, elementComplex.QualifiedName.Namespace, parentNamespace, GlobalXsdType.ComplexType);
                                else
                                    dotnetTypeName = AddQualifiedNamespaceReference(dotnetTypeName, ns, parentNamespace, GlobalXsdType.ComplexType);
                            }
                            else
                            {
                                // shouldn't happen
                                throw new ArgumentException("An element points to a global type or global element that isn't properly qualified");
                            }

                            string collectionContainedType = "";
                            if (maxOccurs > 1)
                            {
                                // collections are always local to the namespace
                                collectionContainedType = dotnetTypeName; // fully namespace referenced with namesapce
                                dotnetTypeName = qualifiedTypeName;  // reference removed for collection class
                            }

                            dotnetTypeName = LanguageBase.ReplaceInvalidChars(dotnetTypeName);
                            code.ClassComplexTypeFieldCode(outStream, element.Name, fname, dotnetTypeName, collectionContainedType, className,
                                maxOccurs, 1, elementFormDefault, ns, element.IsNillable, elementComplex.IsAbstract);
                        }

                        // If the subelement is required, then force it's creation through constructor and 
                        //  a special MakeSchemaCompliant method.

                        ClassConstructor ctor = new ClassConstructor();
                        if (elementRef.MinOccurs > 0 && element.MinOccurs > 0)
                            ctor.required = true;   // required child complex types
                        else
                            ctor.required = false;  // non required child complex tyes

                        if (maxOccurs > 1)
                        {
                            ctor.defaultValue = element.Name;
                            ctor.fieldName = dotnetTypeName;

                            if (elementComplex.IsAbstract)
                                ctor.datatype = CtorDatatypeContext.PropertyCollectionAbstractComplexType;
                            else
                                ctor.datatype = CtorDatatypeContext.PropertyCollectionComplexType;
                        }
                        else
                        {
                            ctor.defaultValue = element.Name;
                            ctor.fieldName = dotnetTypeName;
                            ctor.datatype = CtorDatatypeContext.Property;
                        }
                        ctorList.Add(ctor);
                    }
                    else  // not a ComplexType -- so this will be a leaf-node class field in code
                    {
                        string dotnetElementName = CalculateUniqueTypeOrFieldName(element.Name, "", dotnetFieldList);
                        dotnetFieldList.Add(dotnetElementName, element.QualifiedName);

                        if (element.ElementType is XmlSchemaSimpleType)
                        {
                            ParseElementSimpleType(element, elementRef, maxOccurs, dotnetElementName, ctorList, parentNamespace);
                        }
                        else
                        {
                            string xsdTypeName = element.SchemaTypeName.Name;
                            string clrTypeName = code.FrameworkTypeMapping(xsdTypeName);

                            clrTypeName = LanguageBase.ReplaceInvalidChars(clrTypeName);
                            code.ClassElementFieldCode(outStream, clrTypeName, xsdTypeName,
                                element.Name, dotnetElementName, maxOccurs, 1, elementFormDefault, false, ns,
                                element.IsNillable);

                            BuildConstructorList(element.DefaultValue, element.FixedValue, (elementRef.MinOccurs > 0 && element.MinOccurs > 0),
                                maxOccurs, dotnetElementName, clrTypeName, element.Name, ctorList, false);
                        }
                    }
                }
                else if (groupBase.Items[i] is XmlSchemaAny)
                {
                    XmlSchemaAny any = (XmlSchemaAny)groupBase.Items[i];

                    string dotnetElementName = CalculateUniqueTypeOrFieldName("Any", "", dotnetFieldList);
                    dotnetFieldList.Add(dotnetElementName, "Any");

                    string ns = CalculateAnyNamespace(any.Namespace, parentNamespace);
                    code.ClassElementFieldCode(outStream, "System.Xml.XmlElement", "", "Any", dotnetElementName, any.MaxOccurs, 1, elementFormDefault,
                        false, ns, false);
                }
                else if (groupBase.Items[i] is XmlSchemaGroupRef)
                {
                    XmlSchemaGroup group = (XmlSchemaGroup)schema.Groups[((XmlSchemaGroupRef)groupBase.Items[i]).RefName];
                    ParseGroupBasePass2(group.Particle, className, ctorList, childClasses, parentClassStack, dotnetFieldList, parentNamespace, classReferencesAdded);
                }
                else if (groupBase.Items[i] is XmlSchemaGroupBase)
                {
                    // Particle inside a particle : ie. <xsd:sequence> <xsd:choice/> </xsd:sequence>
                    ParseGroupBasePass2((XmlSchemaGroupBase)groupBase.Items[i], className, ctorList, childClasses, parentClassStack, dotnetFieldList, parentNamespace, classReferencesAdded);
                }
            }
        }

        /*
         * Build code for attributes associated with the complexType.  Called in Pass2
         */
        private void ParseAttributesPass2(XmlSchemaObjectCollection attributes, XmlSchemaAnyAttribute anyAttribute, ArrayList ctorList,
            Hashtable dotnetFieldList, string parentNamespace)
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                if (attributes[i] is XmlSchemaAttributeGroupRef)
                {
                    XmlSchemaAttributeGroupRef attributeGroupRef = (XmlSchemaAttributeGroupRef)attributes[i];
                    XmlSchemaAttributeGroup attributeGroup = (XmlSchemaAttributeGroup)schema.AttributeGroups[attributeGroupRef.RefName];

                    ParseAttributesPass2(attributeGroup.Attributes, attributeGroup.AnyAttribute, ctorList, dotnetFieldList, parentNamespace);
                }
                else if (attributes[i] is XmlSchemaAttribute)
                {
                    XmlSchemaAttribute attributeRef = (XmlSchemaAttribute)attributes[i];
                    XmlSchemaAttribute attribute = (XmlSchemaAttribute)schema.Attributes[attributeRef.QualifiedName];
                    XmlSchemaForm attributeForm = attributeFormDefault;
                    if (attribute == null)
                    {
                        //locally definied attributes

                        attribute = attributeRef;
                        // 11-05-04: colinco: commented this out.  Bug.
                        //attributeForm = XmlSchemaForm.Unqualified;  // use unqualified for locally scoped attributes
                    }

                    string ns = attribute.QualifiedName.Namespace != "" ? attribute.QualifiedName.Namespace : parentNamespace;
                    string dotnetAttributeName = CalculateUniqueTypeOrFieldName(attribute.Name, "", dotnetFieldList);
                    dotnetFieldList.Add(dotnetAttributeName, attribute.QualifiedName);

                    object o = null;
                    try
                    {
                        o = attribute.AttributeType;
                    }
                    catch
                    {

                    }

                    if (o != null && o is XmlSchemaSimpleType)
                    {
                        ParseAttributeSimpleType(attribute, dotnetAttributeName, ctorList, parentNamespace, null);
                    }
                    else
                    {
                        string xsdTypeName = attribute.SchemaTypeName.Name;
                        string clrTypeName = code.FrameworkTypeMapping(xsdTypeName);

                        clrTypeName = LanguageBase.ReplaceInvalidChars(clrTypeName);
                        code.ClassAttributeFieldCode(outStream,
                            clrTypeName, xsdTypeName, attribute.Name, dotnetAttributeName, attributeForm, false, ns);

                        BuildConstructorList(attribute.DefaultValue, attribute.FixedValue, (attribute.Use == XmlSchemaUse.Required),
                            0, dotnetAttributeName, clrTypeName, attribute.Name, ctorList, false);
                    }
                }
            }

            if (anyAttribute != null)
            {
                string dotnetElementName = CalculateUniqueTypeOrFieldName("AnyAttr", "", dotnetFieldList);
                dotnetFieldList.Add(dotnetElementName, "AnyAttr");

                string ns = CalculateAnyNamespace(anyAttribute.Namespace, parentNamespace);
                code.ClassAttributeFieldCode(outStream,
                    "System.Xml.XmlAttribute[]", "", "AnyAttr", dotnetElementName, XmlSchemaForm.Unqualified, false, ns);
            }
        }

        /*
         * Build class field code for attributes associated with the simpleType.  Called in Pass2
         */
        private void ParseAttributeSimpleType(XmlSchemaAttribute attribute, string dotnetAttributeName, ArrayList ctorList, string parentNamespace, XmlSchemaSimpleType unionSimpleType)
        {
            XmlSchemaSimpleType simpleType = null;
            if (unionSimpleType != null) simpleType = unionSimpleType;
            else simpleType = (XmlSchemaSimpleType)attribute.AttributeType;

            String name = simpleType.Name != null && simpleType.Name != "" ? simpleType.Name : attribute.Name;

            string ns = attribute.QualifiedName.Namespace != "" ? attribute.QualifiedName.Namespace : parentNamespace;

            if (IsEnumeration(simpleType))// && enumerations.ContainsKey(name))
            {
                if (simpleType.Name == null || simpleType.Name == "")
                {
                    // locally scoped simpleType enumeration on an element or attribute.  Make sure name is unique.	
                    if (enumerations.ContainsKey(LanguageBase.ReplaceInvalidChars(name)))
                    {
                        // name has already been used, so calculate a new unique name for this locally scoped simpleType enum
                        name = CalculateUniqueTypeOrFieldName(LanguageBase.ReplaceInvalidChars(name), "", enumerations);
                    }
                    enumerations.Add(LanguageBase.ReplaceInvalidChars(name), "");	 // this is pass2, so re-add enumeration to list to account for locally scoped duplicates
                }

                string referencedDotnetFieldType;
                if (ns == parentNamespace && simpleType.QualifiedName.Namespace != ns && simpleType.QualifiedName.Namespace != null)
                    referencedDotnetFieldType = AddQualifiedNamespaceReference(name, simpleType.QualifiedName.Namespace, parentNamespace, GlobalXsdType.Enum);
                else
                    referencedDotnetFieldType = AddQualifiedNamespaceReference(name, ns, parentNamespace, GlobalXsdType.Enum);

                code.ClassAttributeFieldCode(outStream, referencedDotnetFieldType, "",
                    attribute.Name, dotnetAttributeName, attributeFormDefault, true, ns);
                BuildConstructorList(attribute.DefaultValue, attribute.FixedValue, (attribute.Use == XmlSchemaUse.Required), 0, dotnetAttributeName,
                    referencedDotnetFieldType, attribute.Name, ctorList, true);
            }
            else if (simpleType.Content is XmlSchemaSimpleTypeRestriction)
            {
                XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction)simpleType.Content;
                string xsdTypeName;
                if (restriction.BaseTypeName.Namespace == Globals.XSD_NAMESPACE)
                    xsdTypeName = restriction.BaseTypeName.Name;
                else if ((simpleType.BaseSchemaType as XmlSchemaSimpleType).Content is XmlSchemaSimpleTypeRestriction)
                {
                    // simpleType inherited from another simpleType
                    XmlSchemaSimpleTypeRestriction str = (XmlSchemaSimpleTypeRestriction)(simpleType.BaseSchemaType as XmlSchemaSimpleType).Content;

                    if (str.BaseTypeName.Namespace == Globals.XSD_NAMESPACE)
                        xsdTypeName = str.BaseTypeName.Name;
                    else
                    {
                        XmlSchemaDatatype tt = simpleType.BaseXmlSchemaType.Datatype;
                        xsdTypeName = tt.ValueType.Name.ToLower();
                    }
                }
                else
                {
                    xsdTypeName = "string";
                }
                string clrTypeName = code.FrameworkTypeMapping(xsdTypeName);

                code.ClassAttributeFieldCode(outStream, clrTypeName,
                    xsdTypeName, attribute.Name, dotnetAttributeName, attributeFormDefault, false, ns);
                BuildConstructorList(attribute.DefaultValue, attribute.FixedValue, (attribute.Use == XmlSchemaUse.Required),
                    0, dotnetAttributeName, clrTypeName, attribute.Name, ctorList, false);
            }
            else if (simpleType.Content is XmlSchemaSimpleTypeUnion) // union  (XmlSchemaSimpleTypeUnion)
            {
                bool foundNonStringType = false;

                XmlSchemaSimpleTypeUnion union = (XmlSchemaSimpleTypeUnion)simpleType.Content;

                if (union.MemberTypes != null)
                {
                    foreach (XmlQualifiedName unionType in union.MemberTypes)
                    {
                        if (unionType.Namespace == Globals.XSD_NAMESPACE && unionType.Name != "string")
                        {
                            foundNonStringType = true;
                            string clrTypeName = code.FrameworkTypeMapping(unionType.Name);
                            code.ClassAttributeFieldCode(outStream, clrTypeName,
                                unionType.Name, attribute.Name, dotnetAttributeName, attributeFormDefault, false, ns);
                            BuildConstructorList(attribute.DefaultValue, attribute.FixedValue, (attribute.Use == XmlSchemaUse.Required),
                                0, dotnetAttributeName, clrTypeName, attribute.Name, ctorList, false);
                            break;
                        }
                    }
                }
                else if (union.BaseMemberTypes != null)
                {
                    /*
                     <xs:complexType name="OLI_LU_STOPOPTION">
                        <xs:simpleContent>
                          <xs:extension base="xs:string">
                            <xs:attribute name="tc" use="required" type="OLI_LU_STOPOPTION_TC" />
                          </xs:extension>
                        </xs:simpleContent>
                      </xs:complexType>
                      <xs:simpleType name="OLI_LU_STOPOPTION_TC">
                        <xs:union>
                          <xs:simpleType>
                            <xs:restriction base="ACORD_TYPE_CODE">
                              <xs:enumeration value="0" />
                              <xs:enumeration value="1" />
                              <xs:enumeration value="2" />
                              <xs:enumeration value="3" />
                              <xs:enumeration value="4" />
                              <xs:enumeration value="5" />
                              <xs:enumeration value="2147483647" />
                            </xs:restriction>
                          </xs:simpleType>
                          <xs:simpleType>
                            <xs:restriction  base="ACORD_PRIVATE_CODE" />
                          </xs:simpleType>
                        </xs:union>
                      </xs:simpleType>
                     */
                    foreach (XmlSchemaSimpleType unionType in union.BaseMemberTypes)
                    {
                        if (IsEnumeration(unionType))
                        {
                            foundNonStringType = true;

                            if (unionType.Name == null || unionType.Name == "")
                            {
                                // locally scoped simpleType enumeration on an element or attribute.  Make sure name is unique.	
                                if (enumerations.ContainsKey(LanguageBase.ReplaceInvalidChars(name)))
                                {
                                    // name has already been used, so calculate a new unique name for this locally scoped simpleType enum
                                    name = CalculateUniqueTypeOrFieldName(LanguageBase.ReplaceInvalidChars(name), "", enumerations);
                                }
                                enumerations.Add(LanguageBase.ReplaceInvalidChars(name), "");	 // this is pass2, so re-add enumeration to list to account for locally scoped duplicates
                            }

                            string referencedDotnetFieldType;
                            if (ns == parentNamespace && simpleType.QualifiedName.Namespace != ns && simpleType.QualifiedName.Namespace != null)
                                referencedDotnetFieldType = AddQualifiedNamespaceReference(name, simpleType.QualifiedName.Namespace, parentNamespace, GlobalXsdType.Enum);
                            else
                                referencedDotnetFieldType = AddQualifiedNamespaceReference(name, ns, parentNamespace, GlobalXsdType.Enum);

                            code.ClassAttributeFieldCode(outStream, referencedDotnetFieldType, "",
                                attribute.Name, dotnetAttributeName, attributeFormDefault, true, ns);
                            BuildConstructorList(attribute.DefaultValue, attribute.FixedValue, (attribute.Use == XmlSchemaUse.Required), 0, dotnetAttributeName,
                                referencedDotnetFieldType, attribute.Name, ctorList, true);


                            break;
                        }
                    }
                }

                if (!foundNonStringType)
                {
                    code.ClassAttributeFieldCode(outStream, "System.String",
                    "", attribute.Name, dotnetAttributeName, attributeFormDefault, false, ns);
                }
            }
            else  //list
            {
                code.ClassAttributeFieldCode(outStream, "System.String",
                    "", attribute.Name, dotnetAttributeName, attributeFormDefault, false, ns);
            }
        }

        /*
         * Build class field code for elements associated with the simpleType.  Called in Pass2
         */
        private void ParseElementSimpleType(XmlSchemaElement element, XmlSchemaElement elementRef, decimal maxOccurs, string dotnetElementName,
            ArrayList ctorList, string parentNamespace)
        {
            XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType)element.ElementType;
            String name = simpleType.Name != null && simpleType.Name != "" ? simpleType.Name : element.Name;

            string ns = element.QualifiedName.Namespace != "" ? element.QualifiedName.Namespace : parentNamespace;

            if (IsEnumeration(simpleType)) // && enumerations.ContainsKey(name))
            {
                if (simpleType.Name == null || simpleType.Name == "")
                {
                    // locally scoped simpleType enumeration on an element or attribute.  Make sure name is unique.	
                    string clean = LanguageBase.ReplaceInvalidChars(name);
                    if (enumerations.ContainsKey(clean))
                    {
                        // name has already been used, so calculate a new unique name for this locally scoped simpleType enum
                        clean = CalculateUniqueTypeOrFieldName(clean, "", enumerations);
                    }
                    enumerations.Add(clean, "");	 // this is pass2, so re-add enumeration to list to account for locally scoped duplicates
                }

                string referencedDotnetFieldType;
                if (ns == parentNamespace && simpleType.QualifiedName.Namespace != ns && simpleType.QualifiedName.Namespace != null)
                    referencedDotnetFieldType = AddQualifiedNamespaceReference(name, simpleType.QualifiedName.Namespace, parentNamespace, GlobalXsdType.Enum);
                else
                    referencedDotnetFieldType = AddQualifiedNamespaceReference(name, ns, parentNamespace, GlobalXsdType.Enum);

                code.ClassElementFieldCode(outStream, referencedDotnetFieldType, "",
                    element.Name, dotnetElementName, maxOccurs, 1, elementFormDefault, true, ns, element.IsNillable);
                BuildConstructorList(element.DefaultValue, element.FixedValue, (elementRef.MinOccurs > 0 && element.MinOccurs > 0), maxOccurs, dotnetElementName,
                    referencedDotnetFieldType, element.Name, ctorList, true);
            }
            else if (simpleType.Content is XmlSchemaSimpleTypeRestriction)
            {
                XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction)simpleType.Content;
                string xsdTypeName;
                if (restriction.BaseTypeName.Namespace == Globals.XSD_NAMESPACE)
                    xsdTypeName = restriction.BaseTypeName.Name;
                else
                {
                    // simpleType inherited from another simpleType
                    XmlSchemaSimpleTypeRestriction str = (XmlSchemaSimpleTypeRestriction)(simpleType.BaseSchemaType as XmlSchemaSimpleType).Content;
                    xsdTypeName = str.BaseTypeName.Name;
                }
                string clrTypeName = code.FrameworkTypeMapping(xsdTypeName);

                code.ClassElementFieldCode(outStream, clrTypeName,
                    xsdTypeName, element.Name, dotnetElementName, maxOccurs, 1, elementFormDefault, false, ns, element.IsNillable);
                BuildConstructorList(element.DefaultValue, element.FixedValue, (elementRef.MinOccurs > 0 && element.MinOccurs > 0),
                    maxOccurs, dotnetElementName, clrTypeName, element.Name, ctorList, false);
            }
            else if (simpleType.Content is XmlSchemaSimpleTypeUnion) // union  (XmlSchemaSimpleTypeUnion)
            {
                bool foundNonStringType = false;
                XmlSchemaSimpleTypeUnion union = (XmlSchemaSimpleTypeUnion)simpleType.Content;

                if (union.MemberTypes != null)
                {
                    foreach (XmlQualifiedName unionType in union.MemberTypes)
                    {
                        if (unionType.Namespace == Globals.XSD_NAMESPACE && unionType.Name != "string")
                        {
                            foundNonStringType = true;
                            string clrTypeName = code.FrameworkTypeMapping(unionType.Name);
                            code.ClassElementFieldCode(outStream, clrTypeName,
                                unionType.Name, element.Name, dotnetElementName, maxOccurs, 1, elementFormDefault, false, ns, element.IsNillable);
                            BuildConstructorList(element.DefaultValue, element.FixedValue, (elementRef.MinOccurs > 0 && element.MinOccurs > 0),
                                maxOccurs, dotnetElementName, clrTypeName, element.Name, ctorList, false);
                            break;
                        }
                    }
                }
                else if (union.BaseMemberTypes != null)
                {
                    /*
                     <xs:complexType name="OLI_LU_STOPOPTION">
                        <xs:simpleContent>
                          <xs:extension base="xs:string">
                            <xs:attribute name="tc" use="required" type="OLI_LU_STOPOPTION_TC" />
                          </xs:extension>
                        </xs:simpleContent>
                      </xs:complexType>
                      <xs:simpleType name="OLI_LU_STOPOPTION_TC">
                        <xs:union>
                          <xs:simpleType>
                            <xs:restriction base="ACORD_TYPE_CODE">
                              <xs:enumeration value="0" />
                              <xs:enumeration value="1" />
                              <xs:enumeration value="2" />
                              <xs:enumeration value="3" />
                              <xs:enumeration value="4" />
                              <xs:enumeration value="5" />
                              <xs:enumeration value="2147483647" />
                            </xs:restriction>
                          </xs:simpleType>
                          <xs:simpleType>
                            <xs:restriction  base="ACORD_PRIVATE_CODE" />
                          </xs:simpleType>
                        </xs:union>
                      </xs:simpleType>
                     */
                    foreach (XmlSchemaSimpleType unionType in union.BaseMemberTypes)
                    {
                        if (IsEnumeration(unionType))
                        {
                            foundNonStringType = true;

                            if (unionType.Name == null || unionType.Name == "")
                            {
                                // locally scoped simpleType enumeration on an element or attribute.  Make sure name is unique.	
                                if (enumerations.ContainsKey(LanguageBase.ReplaceInvalidChars(name)))
                                {
                                    // name has already been used, so calculate a new unique name for this locally scoped simpleType enum
                                    name = CalculateUniqueTypeOrFieldName(LanguageBase.ReplaceInvalidChars(name), "", enumerations);
                                }
                                enumerations.Add(LanguageBase.ReplaceInvalidChars(name), "");	 // this is pass2, so re-add enumeration to list to account for locally scoped duplicates
                            }

                            string referencedDotnetFieldType;
                            if (ns == parentNamespace && simpleType.QualifiedName.Namespace != ns && simpleType.QualifiedName.Namespace != null)
                                referencedDotnetFieldType = AddQualifiedNamespaceReference(name, simpleType.QualifiedName.Namespace, parentNamespace, GlobalXsdType.Enum);
                            else
                                referencedDotnetFieldType = AddQualifiedNamespaceReference(name, ns, parentNamespace, GlobalXsdType.Enum);

                            code.ClassElementFieldCode(outStream, referencedDotnetFieldType, "",
                                element.Name, dotnetElementName, maxOccurs, 1, elementFormDefault, true, ns, element.IsNillable);
                            BuildConstructorList(element.DefaultValue, element.FixedValue, (elementRef.MinOccurs > 0 && element.MinOccurs > 0), maxOccurs, dotnetElementName,
                                referencedDotnetFieldType, element.Name, ctorList, true);

                            break;
                        }
                    }
                }

                if (!foundNonStringType)
                {
                    code.ClassElementFieldCode(outStream, "System.String", "", element.Name, dotnetElementName, maxOccurs, 1, elementFormDefault, false, ns, element.IsNillable);
                }
            }
            else  // list  (XmlSchemaSimpleTypeList)
            {
                code.ClassElementFieldCode(outStream, "System.String", "", element.Name, dotnetElementName, maxOccurs, 1, elementFormDefault, false, ns, element.IsNillable);
            }
        }

        /*
		 * Builds a constructor list for elements and attributes with default or required values
		 */
        private void BuildConstructorList(string defaultValue, string fixedValue, bool required, decimal maxOccurs,
            string fieldName, string clrTypeName, string elmtAtrrName, ArrayList ctorList, bool isEnum)
        {
            ClassConstructor ctor = new ClassConstructor();

            if (defaultValue != null || fixedValue != null)
            {
                // field with a default value specified in the schema

                ctor.required = true;

                if (maxOccurs > 1)
                {
                    ctor.defaultValue = elmtAtrrName;
                    ctor.fieldName = fieldName;
                    if (clrTypeName == "System.String")
                        ctor.datatype = CtorDatatypeContext.PropertyCollectionString;
                    else
                        ctor.datatype = CtorDatatypeContext.PropertyCollection;
                }
                else if (isEnum)
                {
                    ctor.defaultValue = defaultValue != null ? defaultValue : fixedValue;
                    ctor.defaultValue = clrTypeName + "." + ctor.defaultValue;
                    ctor.fieldName = fieldName;
                    ctor.datatype = CtorDatatypeContext.ValueTypeDefault;
                }
                else if (code.IsValueType(clrTypeName))
                {
                    ctor.defaultValue = defaultValue != null ? defaultValue : fixedValue;
                    ctor.fieldName = fieldName;
                    ctor.datatype = CtorDatatypeContext.ValueTypeDefault;
                }
                else if (clrTypeName == "System.String")
                {
                    ctor.defaultValue = defaultValue != null ? defaultValue : fixedValue;
                    ctor.fieldName = fieldName;
                    ctor.datatype = CtorDatatypeContext.String;
                }

                ctorList.Add(ctor);
            }
            else
            {
                // required field with no default value specified in the schema
                ctor.required = required;

                if (maxOccurs > 1)
                {
                    ctor.defaultValue = elmtAtrrName;
                    ctor.fieldName = fieldName;
                    if (clrTypeName == "System.String")
                        ctor.datatype = CtorDatatypeContext.PropertyCollectionString;
                    else
                        ctor.datatype = CtorDatatypeContext.PropertyCollection;
                }
                else
                {
                    ctor.defaultValue = "";
                    ctor.fieldName = fieldName;
                    if (isEnum)
                        ctor.datatype = CtorDatatypeContext.ValueType;
                    else if (clrTypeName == "System.DateTime")
                        ctor.datatype = CtorDatatypeContext.DateTime;
                    else if (code.IsValueType(clrTypeName))  // non datetime value type
                        ctor.datatype = CtorDatatypeContext.ValueType;
                    else if (clrTypeName == "System.String")
                        ctor.datatype = CtorDatatypeContext.String;
                    else
                        ctor.datatype = CtorDatatypeContext.Other;
                }
                ctorList.Add(ctor);
            }
        }
        #endregion

        #region Helper Routines
        //********************************************************************************************
        //********************************************************************************************
        // HELPER ROUTINES
        //********************************************************************************************
        //********************************************************************************************

        /*
         * Discover if a simpleType is an enumeration.  Returns true or false.
         */
        private bool IsEnumeration(XmlSchemaSimpleType simpleType)
        {
            if (simpleType.Content is XmlSchemaSimpleTypeRestriction)
            {
                XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction)simpleType.Content;

                for (int j = 0; j < restriction.Facets.Count; j++)
                {
                    if (restriction.Facets[j] is XmlSchemaEnumerationFacet)
                        return true;
                }
            }

            return false;
        }

        /*
         * Create a unique class or enum name for a nested type, since it's locally scoped in the schema
         *  -- but the class will be globally scoped in .NET code.
         * Rule for the names (executed in the following order):
         * 1. Use the complexType or enumeration name if a global named complexType or element doesn't exist, and the class hasn't been used
         * 2. Continue to prepend the parent (and grandparent, etc) names on the .net type name until it's uniqe.
         * 3. Add a number suffix to the end of the name is abosolutely necessary (although this should not be necessary)
         */
        private string CalculateNestedChildTypeName(string name, Hashtable table, ArrayList parentClassStack)
        {
            int z = 2;
            string newName = name;

            // first concatinate parents until a unique class name is generated
            if (table.ContainsKey(newName))
            {
                for (int i = parentClassStack.Count - 1; i >= 0; i--)
                {
                    newName = (string)parentClassStack[i] + newName;
                    if (!table.ContainsKey(newName))
                        break;
                }
            }

            // as a fallback, use a unique number suffix
            while (table.ContainsKey(newName))
            {
                newName = name + z.ToString();
                z++;
            }

            return newName;
        }

        /*
         * Create a unique class or field name for a globally scoped complex type, enumeration, or a field in a class.
         * Rule for the names:
         * 1. Use a name that doesn't exist and hasn't been used or not equal to special string names (AnyAttribute, etc)
         * 2. Add a number suffix to the end of the name
         * 
         * Note: Value is the name used for the property name of mixed content elements
         */
        private string CalculateUniqueTypeOrFieldName(string name, string ns, Hashtable table)
        {
            int z = 2;

            while (
                table.ContainsKey(name) ||
                name == "docExtension")
            {
                name = name + z.ToString();
                z++;
            }

            return name;
        }

        /*
         * get list of all imported schemas -- to be used only if code is broken out by imported namespace
         */
        private void RecurseImportedNamespaces(XmlSchema s, ArrayList references)
        {
            for (int i = 0; i < s.Includes.Count; i++)
            {
                if (s.Includes[i] is XmlSchemaImport)
                {
                    // import is the schema referenced by the XmlSchema (s)
                    XmlSchemaImport import = (XmlSchemaImport)s.Includes[i];
                    if (import != null) references.Add(import.Namespace);  // add the reference to import

                    ArrayList r = new ArrayList();
                    if (import != null && namespaces[import.Namespace] == null)
                    {
                        if (import.Namespace == Globals.W3C) continue;
                        // if schema hasn't been added to the main hashtable, then add it
                        namespaces.Add(import.Namespace, r);
                        namespacesList.Add(import.Namespace);
                        Globals.globalSeparateImportedNamespaces = true;
                        //Console.Write("Imported namespace = {0}.  Enter CLR namespace name: ", import.Namespace);
                        //xsdNsToClrNs.Add(import.Namespace, LanguageBase.ScrubNamespace(Console.ReadLine()));
                        xsdNsToClrNs.Add(import.Namespace, LanguageBase.ScrubNamespace(import.Namespace));
                    }
                    if (import.Schema != null && import.Schema.Includes.Count > 0)
                    {
                        RecurseImportedNamespaces(import.Schema, r);
                    }
                }
            }
        }

        // Each namespace can have one global schema level CT (schema type) and global schema level element WITH THE SAME NAME.
        //  Poor schema coding practice, but possible nonetheless.
        //  Hence schemaType.QualifiedName == element.QualifiedName for this scenerio.  Use a special Globals.ELELENT_DELIMINATOR to flag this.
        //  NOTE: Only check this for global schema complexType ELEMENTS -- not types (schema level CT's)
        private string GlobalElementToClrMap(XmlQualifiedName qn)
        {
            if (globalQualifiedComplexTypeClasses[Globals.ELELENT_DELIMINATOR + qn] != null)
                return (string)globalQualifiedComplexTypeClasses[Globals.ELELENT_DELIMINATOR + qn];
            else
                return (string)globalQualifiedComplexTypeClasses[qn];
        }

        //
        private string AddQualifiedNamespaceReference(string elementName, string typeNs, string currentNs, GlobalXsdType xsdType)
        {
            GlobalSchemaType gst = null;
            if (typeNs == null || typeNs == "") typeNs = currentNs;

            switch (xsdType)
            {
                case GlobalXsdType.ComplexType:
                    gst = (GlobalSchemaType)Globals.globalSchemaTypeTable[typeNs + Globals.COMPLEXTYPE_DELIMINATOR + elementName];
                    break;
                case GlobalXsdType.Element:
                    gst = (GlobalSchemaType)Globals.globalSchemaTypeTable[typeNs + Globals.ELELENT_DELIMINATOR + elementName];
                    break;
                case GlobalXsdType.Enum:
                    gst = (GlobalSchemaType)Globals.globalSchemaTypeTable[typeNs + Globals.ENUM_DELIMINATOR + elementName];
                    break;
            }

            if (gst == null)
            {
                return LanguageBase.ReplaceInvalidChars(elementName);
            }
            else
            {
                return gst.ClrNamespace + "." + gst.ClrTypeName;
            }
        }

        // Processes the namespace for <xs:any> element and anyAttribute.
        private string CalculateAnyNamespace(string ns, string parentNamespace)
        {
            // <xs:any/> -- no namespace
            // <xs:any namespace="##local" /> -- use default schema namespace
            // <xs:any namespace="some specific namespace"/> -- use specific namespace entered
            // <xs:any namespace="##other"> -- no namespace

            // colinco: 2005-07-05: <xsd:any> element can have a null namespace.  Crashes generator.
            if (ns == null)
                return "";

            switch (ns)
            {
                case "":
                    return "";
                case "##local":
                    return parentNamespace;
                case "##other":
                    return "";
                default:
                    if (ns.StartsWith("##"))
                        return "";
                    else
                        return ns;
            }
        }
        #endregion
    }
}
