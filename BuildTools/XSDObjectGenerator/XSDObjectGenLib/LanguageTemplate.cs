using System;
using System.IO;
using System.Collections;
using System.Xml;
using System.Xml.Schema;
using System.Configuration;

namespace XSDObjectGenLib
{
	/// <summary>
	/// Abstract class defining the methods that need to overriden by decendent language classes.
	/// </summary>
	internal abstract class LanguageBase
	{
		static LanguageBase()
		{
			collectionSuffix = ConfigurationSettings.AppSettings["CollectionSuffix"];
			hiddenMemberPrefix = ConfigurationSettings.AppSettings["HiddenMemberPrefix"];
			renameItemPrefix = ConfigurationSettings.AppSettings["RenameItemPrefix"];
			mixedElementFieldName = ConfigurationSettings.AppSettings["MixedElementFieldName"];

			if (collectionSuffix == null) collectionSuffix = "Collection";
			if (hiddenMemberPrefix == null) hiddenMemberPrefix = "__";
			if (renameItemPrefix == null) renameItemPrefix = "_";
			if (mixedElementFieldName == null) mixedElementFieldName = "MixedValue";
		}

		protected string schemaTargetNamespace = "";	// stores the schema targetNamespace string
		public static string collectionSuffix;			// suffix for special collection classes
		public static string hiddenMemberPrefix;		// prefix for hidden members
        public static bool partialClasses;    		// put the partial keyword on every class
		public static string renameItemPrefix;			// Prefix to be placed certain xsd names that wouldn't be legal in .NET -- like xsd enumerations that start with a numeric value
		public static string mixedElementFieldName;		// Special field in a class for mixed = "true" elements.

		protected abstract string FieldCollectionTemplate { get; set; }
		protected abstract string FieldClassTemplate { get; set; }
		protected abstract string FieldAbstractClassTemplate { get; set; }
		protected abstract string ElementObjectTemplate { get; set; }
		protected abstract string ElementValueTypeTemplate { get; set; }
		protected abstract string ElementAnyTemplate { get; set; }
		protected abstract string ElementAnyMaxOccursTemplate { get; set; }
		protected abstract string AttributeObjectTemplate { get; set; }
		protected abstract string AttributeAnyTemplate { get; set; }
		protected abstract string ElementDateTimeTemplate { get; set; }
		protected abstract string AttributeDateTimeTemplate { get; set; }
		protected abstract string AttributeValueTypeTemplate { get; set; }
		protected abstract string MixedObjectTemplate { get; set; }
		protected abstract string MixedValueTypeTemplate { get; set; }
		protected abstract string MixedDateTimeTemplate { get; set; }
		protected abstract string ClassEnumerabilityTemplate { get; set; }
        protected abstract string AcordTCTemplate { get; set; }
        protected abstract string AcordPrivateTCTemplate { get; set; }
		protected abstract string AttributeAssignmentOperator { get; }
		protected abstract string HideInheritedMethodKeyword { get; }
        protected abstract string PartialKeyword { get; }
		
		
	/// <summary>
	/// Add the namespace, using statements, enumerations, and any forward declarations
	/// </summary>
	/// <param name="outStream"></param>
	/// <param name="ns">.NET namespace to contain all generated types</param>
	/// <param name="schemaFile">XSD Schema file used to generate the types</param>
	/// <param name="forwardDeclarations">Forward declarations needed in MC++</param>
	/// <param name="targetNamespace">Optional qualified namespace from the XSD schema</param>
	/// <param name="enumerations">Enumeration list to build enum types</param>
	/// <param name="depthFirstTraversalHooks">Create DepthFirstTraversal hook events</param>
	/// <param name="importedReferences">Namespaces imported from other schemas</param>
	public abstract void NamespaceHeaderCode(StreamWriter outStream, String ns, String schemaFile, 
		Hashtable forwardDeclarations, string targetNamespace, Hashtable enumerations, bool depthFirstTraversalHooks, ArrayList importedReferences);
		
	/// <summary>
	/// Close the namespace containing all of the generated types
	/// </summary>
	/// <param name="outStream"></param>
	/// <param name="ns">.net namespace string</param>
	public abstract void NamespaceTrailerCode(StreamWriter outStream, string ns);
		
	/// <summary>
	/// Add the class keyword and inheritance relationship if necessary
	/// </summary>
	/// <param name="outStream">Out file</param>
	/// <param name="dotnetClassName">Classname to be used in code</param>
	/// <param name="elementName">Xml element for the class</param>
	/// <param name="complexTypeBaseClass">Base class</param>
	/// <param name="baseIsAbstract">Base class is an abstract type</param>
	/// <param name="isSchemaType">Is a global ComplexType defined at the schema level</param>
	/// <param name="isAbstract">Is the complexType an Abstract type -- marked with abstract="true"</param>
	/// <param name="isLocalComplexType">Locally scoped complexType (not a child of schema tag)</schema></param>
	/// <param name="enumerableClasses">List of enumerable classes</param>
	/// <param name="ns">namespace</param>
	/// <param name="elementFormDefault">whether the xml element for the class will be namespace qualified</param>
	/// <param name="annotation">complexType annotation</param>
	/// <param name="isElementNullable">Set the IsNullable element paramter = true, causing xsi:nil="true" for null elements</param>
	/// <param name="xmlIncludedClasses">List of classes to put in XmlIncludeAttributes -- needed for abstract ComplexTypes</param>
	/// <param name="globalElementAndSchemaTypeHaveSameName">For global schema types where an existing global element also has the same name</param>	
	public abstract void ClassHeaderCode(StreamWriter outStream, string dotnetClassName, string elementName, 
		string complexTypeBaseClass, bool baseIsAbstract, bool isSchemaType, bool isAbstract, bool isLocalComplexType, Hashtable enumerableClasses, 
		string ns, XmlSchemaForm elementFormDefault, string annotation, bool isElementNullable, ArrayList xmlIncludedClasses,
		bool globalElementAndSchemaTypeHaveSameName);

	/// <summary>
	/// Close the class containing all of the generated type.  Build a constructor if necessary to initialize fields.
	/// </summary>
	/// <param name="outStream"></param>
	/// <param name="dotnetClassName">Class name to generate</param>
	/// <param name="ctorList">Field list to build and initialize in a constructor</param>
	/// <param name="defaultInitialization">Build schema compliancy and initialization logic into class constructors</param>
	/// <param name="depthFirstTraversalHooks">Build DepthFirstTraversal events, to allow for client DepthFirstTraversal event handers and custom routines</param>
	/// <param name="makeSchemaCompliant">Add MakeSchemaCompliant function to each class to create all required child nodes</param>
	/// <param name="complexTypeBaseClass">Base class</param>
    /// <param name="baseClassIsMixed">Base class is mixed, so child can't be mixed -- otherwise XmlSerializer error</param>
	/// <param name="mixed">If the element has mixed content and can also have text</param>
	/// <param name="mixedType">Type of mixed content.  Can be an empty string if mixed=false.</param>
	public abstract void ClassTrailerCode(StreamWriter outStream, string dotnetClassName, ArrayList ctorList,
        bool defaultInitialization, bool depthFirstTraversalHooks, bool makeSchemaCompliant, string complexTypeBaseClass, bool baseClassIsMixed, bool mixed, string mixedXsdType);

	/// <summary>
	/// Add a field to the class that will persist itself to an attrbute
	/// </summary>
	/// <param name="outStream"></param>
	/// <param name="dotNetDatatype">.NET System datatype</param>
	/// <param name="xsdDatatype">XSD equivelent datatype</param>
	/// <param name="fieldName">Name of the attribute that will be a new field in code</param>
	/// <param name="dotnetFieldName">Name of the .net class field -- different from fieldName if duplicates occur</param>
	/// <param name="attributeFormDefault">whether the XML attribute name generated by the XmlSerializer is qualified</param>
	/// <param name="isSchemaEnumerationType">some languages like MC++ need to perform special logic if the field's type is an enumeration</param>
	/// <param name="ns">attribute namesapce</param>
	public virtual void ClassAttributeFieldCode(StreamWriter outStream, string dotNetDatatype, string xsdDatatype,
		string fieldName, string dotnetFieldName, XmlSchemaForm attributeFormDefault, bool isSchemaEnumerationType, string ns)
	{  
		outStream.WriteLine();

		string attributeName = fieldName;
		string fieldName1 = ReplaceInvalidChars(dotnetFieldName);
		string fieldName2 = CheckForKeywords(dotnetFieldName);
		string fieldType = ConvertSystemDatatype(dotNetDatatype);

		string nameSpace = "";
		string schemaForm = "";
		if (attributeFormDefault == XmlSchemaForm.Qualified || attributeFormDefault == XmlSchemaForm.Unqualified)
		{
			schemaForm = ",Form" + AttributeAssignmentOperator + "XmlSchemaForm." + attributeFormDefault.ToString();
			nameSpace = CalculateNamespace(schemaTargetNamespace, ns, dotNetDatatype == "System.Xml.XmlAttribute[]");
		}

		if (xsdDatatype == "anyType") 
			xsdDatatype = "";
		else if (xsdDatatype != "") xsdDatatype = ",DataType" + AttributeAssignmentOperator + "\"" + xsdDatatype + "\""; 

		switch(dotNetDatatype)
		{
			// object type
			case "System.String": 
			case "System.Byte[]": 
			case "System.Object":
			case "System.Xml.XmlQualifiedName":
				outStream.WriteLine(AttributeObjectTemplate, fieldName1, fieldType, attributeName, schemaForm, xsdDatatype, fieldName2, nameSpace,
					hiddenMemberPrefix);
				break;
			// special xsd:any
			case "System.Xml.XmlAttribute[]" :
				outStream.WriteLine(AttributeAnyTemplate, fieldName1, fieldType, attributeName, schemaForm, xsdDatatype, fieldName2, nameSpace,
					hiddenMemberPrefix);
				break;
			// value type
			case "System.DateTime":
				outStream.WriteLine(AttributeDateTimeTemplate, fieldName1, fieldType, attributeName, schemaForm, xsdDatatype, fieldName2, nameSpace,
					hiddenMemberPrefix);
				break;
			default:
				if (fieldType == "string")   // value types like System.Uri
					outStream.WriteLine(AttributeObjectTemplate, fieldName1, fieldType, attributeName, schemaForm, xsdDatatype, fieldName2, nameSpace,
						hiddenMemberPrefix);
				else
                    if (fieldType.StartsWith("OLI_LU_"))  // special case for ACORD life
					    outStream.WriteLine(AttributeValueTypeTemplate, fieldName1, "int", attributeName, schemaForm, xsdDatatype, fieldName2, nameSpace,
						    hiddenMemberPrefix);
                    else
                        outStream.WriteLine(AttributeValueTypeTemplate, fieldName1, fieldType, attributeName, schemaForm, xsdDatatype, fieldName2, nameSpace,
                            hiddenMemberPrefix);
				break;
		}
	}

	/// <summary>
	/// Add a field to the class that will persist itself to an element (element with no children)
	/// </summary>
	/// <param name="outStream"></param>
	/// <param name="dotNetDatatype">.NET System datatype</param>
	/// <param name="xsdDatatype">XSD equivelent datatype</param>
	/// <param name="fieldName">Name of the element that will be a new field in code</param>
	/// <param name="dotnetFieldName">Name of the .net class field -- different from fieldName if duplicates occur</param>
	/// <param name="fieldOccurs">maxOccurs on the field from XSD</param>
	/// <param name="particleOccurs">maxOccurs on the particle from XSD if the field is an instance</param>
	/// <param name="elementFormDefault">whether the XML attribute name generated by the XmlSerializer is qualified</param>
	/// <param name="isSchemaEnumerationType">some languages like MC++ need to perform special logic if the field's type is an enumeration</param>
	/// <param name="ns">element namespace</param>
	/// <param name="isElementNullable">Set the IsNullable element paramter = true, causing xsi:nil="true" for null elements</param>
	public virtual void ClassElementFieldCode(StreamWriter outStream, string dotNetDatatype, string xsdDatatype,
		string fieldName, string dotnetFieldName, decimal fieldOccurs, decimal particleOccurs, XmlSchemaForm elementFormDefault,
		bool isSchemaEnumerationType, string ns, bool isElementNullable)
	{
		outStream.WriteLine();

		string elementName = fieldName;
		string fieldName1 = ReplaceInvalidChars(dotnetFieldName);
		string fieldName2 = CheckForKeywords(dotnetFieldName);
		string fieldType = ConvertSystemDatatype(dotNetDatatype);
		
		string nameSpace = "";
		string schemaForm = "";
		if (elementFormDefault == XmlSchemaForm.Qualified || elementFormDefault == XmlSchemaForm.Unqualified)
		{
			schemaForm = ",Form" + AttributeAssignmentOperator + "XmlSchemaForm." + elementFormDefault.ToString();
			nameSpace = CalculateNamespace(schemaTargetNamespace, ns, dotNetDatatype == "System.Xml.XmlElement");
			
			// special case for <xs:any> elements
			// <xs:any> has XmlElement dotnet type.
			if (dotNetDatatype == "System.Xml.XmlElement" && nameSpace != "") 
				nameSpace = "Name" + AttributeAssignmentOperator + "\"" + fieldName2 + "\"" + nameSpace;
		}
		
		if (xsdDatatype == "anyType") 
			xsdDatatype = "";
		else if (xsdDatatype != "") 
			xsdDatatype = ",DataType" + AttributeAssignmentOperator + "\"" + xsdDatatype + "\""; 

		if (particleOccurs > 1 || fieldOccurs > 1)
		{
			if (dotNetDatatype == "System.Xml.XmlElement")
			{
				outStream.WriteLine(ElementAnyMaxOccursTemplate, fieldName1, fieldType, elementName, schemaForm, xsdDatatype, fieldName2, nameSpace,
					hiddenMemberPrefix, isElementNullable.ToString().ToLower());
			}
			else
			{
				outStream.WriteLine(FieldCollectionTemplate, fieldName1, fieldType, elementName, schemaForm, fieldName1, xsdDatatype, nameSpace,
					hiddenMemberPrefix, collectionSuffix, isElementNullable.ToString().ToLower(), fieldType);
			}
		}
		else	
		{
			switch(dotNetDatatype)
			{
				// object type
				case "System.String": 
				case "System.Byte[]": 
				case "System.Object":
				case "System.Xml.XmlQualifiedName":
					outStream.WriteLine(ElementObjectTemplate, fieldName1, fieldType, elementName, schemaForm, xsdDatatype, fieldName2, nameSpace,
						hiddenMemberPrefix, isElementNullable.ToString().ToLower());
					break;
				// special xsd:any
				case "System.Xml.XmlElement" :
					// assuming properly formatted nameSpace from above with Name and Namespace parameters
					outStream.WriteLine(ElementAnyTemplate, fieldName1, fieldType, elementName, schemaForm, xsdDatatype, fieldName2, nameSpace,
						hiddenMemberPrefix, isElementNullable.ToString().ToLower());
					break;
				// value type
				case "System.DateTime":
					outStream.WriteLine(ElementDateTimeTemplate, fieldName1, fieldType, elementName, schemaForm, xsdDatatype, fieldName2, nameSpace,
						hiddenMemberPrefix);
					break;
				default:
					if (fieldType == "string")   // value types like System.Uri
						outStream.WriteLine(ElementObjectTemplate, fieldName1, fieldType, elementName, schemaForm, xsdDatatype, fieldName2, nameSpace,
							hiddenMemberPrefix, isElementNullable.ToString().ToLower());
					else
                        if (fieldType.StartsWith("OLI_LU_"))
                            outStream.WriteLine(ElementValueTypeTemplate, fieldName1, "int", elementName, schemaForm, xsdDatatype, fieldName2, nameSpace,
                            hiddenMemberPrefix);
                        else
						    outStream.WriteLine(ElementValueTypeTemplate, fieldName1, fieldType, elementName, schemaForm, xsdDatatype, fieldName2, nameSpace,
							    hiddenMemberPrefix);
					break;
			}
		}
	}
		

	/// <summary>
	/// Add a field to the class that will persist itself to an element -- who can have children (ComplexType)
	/// </summary>
	/// <param name="outStream"></param>
	/// <param name="elementName">Name attribute on a xsd:element.</element></param>
    /// <param name="dupElementName">Rename of the attribute on the xsd:element in case of duplicates in the xsd</element></param>
	/// <param name="dotnetTypeName">.NET class being referenced</param>
	/// <param name="parentContainerClassName">Owner class</param>
	/// <param name="fieldOccurs">maxOccurs on the field from XSD</param>
	/// <param name="particleOccurs">maxOccurs on the particle from XSD if the field is an instance</param>
	/// <param name="elementFormDefault">whether the XML attribute name generated by the XmlSerializer is qualified</param>
	/// <param name="isElementNullable">Set the IsNullable element paramter = true, causing xsi:nil="true" for null elements</param>
	/// <param name="isAbstract">complex type is marked as abstract, so it cannot be created</param>
	public virtual void ClassComplexTypeFieldCode(StreamWriter outStream, String elementName, string dupElementName,
		String dotnetTypeName, String collectionContainedType, String parentContainerClassName, decimal fieldOccurs, decimal particleOccurs,
		XmlSchemaForm elementFormDefault, string ns, bool isElementNullable, bool isAbstract)
	{
		outStream.WriteLine();

        string fieldName1 = ReplaceInvalidChars(dupElementName);
        string fieldName2 = CheckForKeywords(fieldName1);
		string fieldType1 = CheckForKeywords(dotnetTypeName);
		string fieldType2 = ReplaceInvalidChars(dotnetTypeName);
		collectionContainedType = CheckForKeywords(collectionContainedType);
	
	    string nameSpace = "";
		string schemaForm = "";
		if (elementFormDefault == XmlSchemaForm.Qualified || elementFormDefault == XmlSchemaForm.Unqualified)
		{
			schemaForm = ",Form" + AttributeAssignmentOperator + "XmlSchemaForm." + elementFormDefault.ToString();
			nameSpace = CalculateNamespace(schemaTargetNamespace, ns, false);
		}

		if (particleOccurs > 1 || fieldOccurs > 1)
		{
			outStream.WriteLine(FieldCollectionTemplate, fieldType2, fieldType1, elementName, schemaForm, fieldName1, "", nameSpace,
				hiddenMemberPrefix, collectionSuffix, isElementNullable.ToString().ToLower(), collectionContainedType);
		}
		else if (isAbstract)
		{
			outStream.WriteLine(FieldAbstractClassTemplate, fieldName1, fieldType1, elementName, schemaForm, fieldName2, nameSpace, hiddenMemberPrefix,
				isElementNullable.ToString().ToLower());
		}	
		else
		{
			outStream.WriteLine(FieldClassTemplate, fieldName1, fieldType1, elementName, schemaForm, fieldName2, nameSpace, hiddenMemberPrefix,
				isElementNullable.ToString().ToLower());
		}
	}

    /// <summary>
    /// Acord specific helper routines
    /// </summary>
    public virtual void AcordTransactCodes(StreamWriter outStream, String elementName, string enumName, string tcCode, string valueCode, bool privateCodes)
    {
        outStream.WriteLine();

        if (privateCodes)
            outStream.WriteLine(AcordPrivateTCTemplate, elementName, enumName, tcCode, valueCode);
        else
            outStream.WriteLine(AcordTCTemplate, elementName, enumName, tcCode, valueCode);
    }
 
	/// <summary>
	/// Convert from System.type to language specific type
	/// </summary>
	/// <param name="systemType">System.Type</param>
	/// <returns>language specific type</returns>
	public abstract string ConvertSystemDatatype(string systemType);

	/// <summary>
	/// Check for language keywords
	/// </summary>
	/// <param name="keyword">keyword to check</param>
	/// <returns>modified form</returns>
	public abstract string CheckForKeywords(String keyword);

	/// <summary>
	/// Generator supports the schema "import" keyword to import in types from another schema's namespace.
	/// To enable this, when the schema elementFormDefault="qualified", we make all types defined in the schema
	/// equal to the schema's namespace, and all other types equal to the imported namesapce.  This function will
	/// return either the schema namespace or the imported namespace if applicable.
	/// </summary>
	/// <param name="schemaNamespace">schema namespace</param>
	/// <param name="importedNamespace">possible imported namespace</param>
	/// <returns></returns>
	protected virtual string CalculateNamespace(string schemaNamespace, string importedNamespace, bool any)
	{
		if (importedNamespace != "" && (importedNamespace != schemaNamespace))
		{
			return string.Format(",Namespace" + AttributeAssignmentOperator + "\"{0}\"", importedNamespace);
		}
		else if (any && importedNamespace == "")
			return "";
		else if (schemaNamespace == "")
			return "";
		else
		{
			return ",Namespace" + AttributeAssignmentOperator + "Declarations.SchemaVersion";
		}
	}

	/// <summary>
	/// Returns back if the datatype is a .NET value type
	/// </summary>
	/// <param name="datatype"></param>
	/// <returns>If the System type is a value type</returns>
	public bool IsValueType(string datatype)
	{
		switch (datatype)
		{
			case "System.SByte" : 
			case "System.Byte" : 
			case "System.Int16" : 
			case "System.UInt16" : 
			case "System.Int32" : 
			case "System.UInt32" : 
			case "System.Int64" : 
			case "System.UInt64" : 
			case "System.Single" : 
			case "System.Double" : 
			case "System.Boolean" : 
			case "System.Decimal" :
			case "System.Char" :
			case "System.DateTime" :
				return true;
			default:
                if (datatype.EndsWith("Enum")) return true;
				else return false;
		}
	}

	/// <summary>
	/// Scub characters out of fieldnames that are valid in XSD but invalid in supported .NET languages.
	/// First check to see if a .net namespace exists before the scrubbed string.
	/// </summary>
	/// <param name="invalid"></param>
	/// <returns></returns>
	public static string ReplaceInvalidChars(string invalid)
	{
		if (invalid == null)
			return invalid;
		if (invalid.StartsWith("System."))
			return invalid;

		bool b = false;
		string ns = "";
	
		if (invalid.IndexOf(".") > 0)
		{
			foreach (string clrNs in Globals.globalClrNamespaceList)
			{
				if (invalid.StartsWith(clrNs + "."))
				{
					ns = clrNs + ".";
					if (invalid.StartsWith(ns)) invalid = invalid.Remove(0, ns.Length);
					b = true;
					break;
				}
			}
		}

		invalid = invalid.Replace(".","_");
		invalid = invalid.Replace("-","_");
		invalid = invalid.Replace("/", "_");
		invalid = invalid.Replace(":", "_");

		if (b) invalid = ns + invalid;
		return invalid;
	}

	/// <summary>
	/// Scub characters out of fieldnames that are valid in XSD but invalid in a .NET namespace
	/// </summary>
	/// <param name="invalid"></param>
	/// <returns></returns>
	public static string ScrubNamespace(string invalid)
	{
		if (invalid == "") throw new XSDObjectGenException("invalid .net namespace name entered for imported schema namesapce");
		invalid = invalid.Replace(".","_");
		invalid = invalid.Replace("-","_");
		invalid = invalid.Replace(":","_");
		invalid = invalid.Replace("/","_");
		invalid = invalid.Replace("\\","_");
		invalid = invalid.Replace("$","_");
		return invalid;
	}

	/// <summary>
	/// .Net framework maps xsd types to CLR types.  Reason for this method is to handle current bugs in the .net framework
	/// that map xsd "integer" to a decimal -- and other types -- which choke XmlSerializer
	/// </summary>
	/// <param name="invalid"></param>
	/// <returns></returns>
	public string FrameworkTypeMapping(string xsdType)
	{
		//  Note: do to bugs in the .net 1.0 and 1.1 framework, the following are mapped to strings, when
		//		the framework maps them to decimal
		//			case "integer": return "System.String";
		//			case "positiveInteger": return "System.String";
		//			case "negativeInteger": return "System.String";
		//			case "nonPositiveInteger": return "System.String";
		//			case "nonNegativeInteger": return "System.String";
		//		Same with gYearMonth, gYear, gMonthDay, gDay, and gMonth

		switch (xsdType)
		{
			case "base64Binary": return "System.Byte[]";
			case "boolean": return "System.Boolean";
			case "byte": return "System.SByte";
			case "date": return "System.DateTime";
			case "dateTime": return "System.DateTime";
			case "decimal": return "System.Decimal";
			case "double": return "System.Double";
			case "float": return "System.Single";
			case "hexBinary": return "System.Byte[]";
			case "int": return "System.Int32";
			case "long": return "System.Int64";
			case "QName": return "System.XmlQualifiedName";
			case "short": return "System.Int16";
			case "time": return "System.DateTime";
			case "unsignedByte": return "System.Byte";
			case "unsignedInt": return "System.UInt32";
			case "unsignedLong": return "System.UInt64";
			case "unsignedShort": return "System.UInt16";
			case "anyType": return "System.Object";
			default: 
                if (xsdType.EndsWith("Enum")) // special case enum where mixType is an enum
                    return xsdType;
                else return "System.String";
		}
	}

    /// <summary>
    /// Reverse of the above
    /// </summary>
    /// <param name="invalid"></param>
    /// <returns></returns>
    public string XsdTypeMapping(string clrType)
    {
        switch (clrType)
        {
            case "System.Byte[]": return "base64Binary";
            case "System.Boolean": return "boolean";
            case "System.SByte": return "byte";
            case "System.DateTime": return "dateTime";
            case "System.Decimal": return "decimal";
            case "System.Double": return "double";
            case "System.Single": return "float";
            case "System.Int32": return "int";
            case "System.Int64": return "long";
            case "System.XmlQualifiedName": return "QName";
            case "System.Int16": return "short";
            case "System.Byte": return "unsignedByte";
            case "System.UInt32": return "unsignedInt";
            case "System.UInt64": return "unsignedLong";
            case "System.UInt16": return "unsignedShort";
            case "System.Object": return "anyType";
            default: return "string";
        }
    }
}

	/// <summary>
	/// Context of the class constructor datatype -- to build required/default values
	/// </summary>
	internal enum CtorDatatypeContext
	{
		PropertyCollectionComplexType,	// Collection of other complexType classes (non leaf element)
		PropertyCollectionAbstractComplexType,	// Collection of other "Abstract" complexType classes (non leaf element)
		PropertyCollectionString,		// Collection of leaf node clr string elements
		PropertyCollection,				// Collection of leaf node elements
		Property,						// Property exposing a complex type class	
		ValueTypeDefault,				// Valuetype .net field with a default value
		ValueType,						// Value type with no default
		DateTime,						// DateTime type with no default
		String,							// String .net field
		Other							// other
	}

	/// <summary>
	/// Data collected to build class constructors.  Default field values are set here.
	/// </summary>
	internal class ClassConstructor
	{
		public string fieldName;
		public CtorDatatypeContext datatype;
		public string defaultValue;
		public bool required;

		public ClassConstructor()
		{
			fieldName = "";
			datatype = CtorDatatypeContext.Other;
			defaultValue = "";
			required = false;
		}
	}

	/// <summary>
	/// Data collected to build ArrayList decendent collection classes. 
	/// </summary>
	internal class CollectionClass
	{
		public string FieldName;
		public string Datatype;
		public bool IsAbstract;

		public CollectionClass(string fieldName, string datatype, bool isAbstract)
		{
			FieldName = fieldName;
			Datatype = datatype;
			IsAbstract = isAbstract;
		}
	}

	/// <summary>
	/// This is a class to hold info on ComplexType elements that are not defined at the <xsd:schema>
	/// level.  Instead these are private ComplexTypes defined within another parent ComplexType.
	/// </summary>
	internal class ChildComplexType
	{
		public ChildComplexType(XmlSchemaComplexType complexType, string elementName, string dotnetClassName, string nameSpace, XmlQualifiedName qname)
		{
			ComplexType = complexType;
			ElementName = elementName;
			DotnetClassName = dotnetClassName;
			Namespace = nameSpace;
			Qname = qname;
		}

		public XmlSchemaComplexType ComplexType;
		public string ElementName;
		public string Namespace;
		public string DotnetClassName;
		public XmlQualifiedName Qname; 
	}

	internal enum GlobalXsdType
	{
		ComplexType,	// xsd schema type --> .net class
		Element,		// xsd element complexType --> .net class
		Enum			// xsd simpleType --> .net enum
	}

	internal class GlobalSchemaType
	{
		public string XsdNamespaceAndTypeName; // key
		public string XsdNamespace;	
		public string XsdTypeName;	
		public string ClrNamespace;
		public string ClrTypeName;
		public GlobalXsdType Type;
		
		public GlobalSchemaType(string xsdNamespace, string xsdTypeName, GlobalXsdType type, string clrNamespace, string clrTypeName)
		{
			XsdNamespace = xsdNamespace;
			XsdTypeName = xsdTypeName;
			Type = type;
			ClrNamespace = clrNamespace;
			ClrTypeName = clrTypeName;
			
			switch(Type)
			{
				case GlobalXsdType.ComplexType: XsdNamespaceAndTypeName = XsdNamespace + Globals.COMPLEXTYPE_DELIMINATOR + xsdTypeName; break;
				case GlobalXsdType.Element: XsdNamespaceAndTypeName = XsdNamespace + Globals.ELELENT_DELIMINATOR + xsdTypeName; break;
				case GlobalXsdType.Enum: XsdNamespaceAndTypeName = XsdNamespace + Globals.ENUM_DELIMINATOR + xsdTypeName; break;
			}
		}
	}

	internal class Globals
	{
		internal const string XSD_NAMESPACE = "http://www.w3.org/2001/XMLSchema";
		internal const string W3C = "http://www.w3.org/XML/1998/namespace";
		internal const string COMPLEXTYPE_DELIMINATOR = "&&";
		internal const string ELELENT_DELIMINATOR = "$$";
		internal const string ENUM_DELIMINATOR = "%%";
		internal static Hashtable globalSchemaTypeTable = new Hashtable(); // contains info about all global schema types -- needed for adding proper .net references, etc, later
		internal static bool globalSeparateImportedNamespaces = false;			// separate imported namespaces into separate files
		internal static ArrayList globalClrNamespaceList = new ArrayList();
	}
}
