using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Collections;

namespace XSDObjectGenLib
{
	/// <summary>
	/// LanguageBase implementation for VB.NET
	/// </summary>
	internal class VBTemplate : LanguageBase
	{

		// {0} -- collection class name (scrubbed)
		// {1} -- type (keyword scrubbed)
		// {2} -- xml element name
		// {3} -- schema form
		// {4} -- field/property name (scrubbed)
		// {5} -- XmlElement DataType
		// {6} -- namespace
		// {7} -- hidden member prefix
		// {8} -- collection suffix -- not needed anymore
		// {9} -- IsNullable
		// {10} -- contained type within the collection
		static string sFieldCollectionTemplate =
@"		'*********************** {2} element ***********************
		<XmlElement(Type:=GetType({10}),ElementName:=""{2}"",IsNullable:={9}{3}{5}{6})> _
		Public {7}{4} As List(Of {10})
		
		<XmlIgnore> _
		Public Property {4} As List(Of {10})
			Get
				If {7}{4} Is Nothing Then {7}{4} = new List(Of {10})()
				{4} = {7}{4}
			End Get
			Set(Value As List(Of {10}))
				{7}{4} = Value
			End Set
		End Property";


		// {0} -- field name (scrubbed)
		// {1} -- type (keyword scrubbed)
		// {2} -- xml element name
		// {3} -- schema form
		// {4} -- property name (keyword scrubbed)
		// {5} -- namespace
		// {6} -- hidden member prefix
		// {7} -- IsNullable
		static string  sFieldClassTemplate = 
@"		'*********************** {2} element ***********************
		<XmlElement(Type:=GetType({1}),ElementName:=""{2}"",IsNullable:={7}{3}{5})> _
		Public {6}{0} As {1}
		
		<XmlIgnore> _
		Public Property {4} As {1}
			Get
				If {6}{0} Is Nothing Then {6}{0} = new {1}()
				{4} = {6}{0}
			End Get
			Set(Value As {1})
				{6}{0} = Value
			End Set
		End Property";


		// {0} -- field name (scrubbed)
		// {1} -- type (keyword scrubbed)
		// {2} -- xml element name
		// {3} -- schema form
		// {4} -- property name (keyword scrubbed)
		// {5} -- namespace
		// {6} -- hidden member prefix
		// {7} -- IsNullable
		static string  sFieldAbstractClassTemplate = 
@"		'*********************** {2} element ***********************
		<XmlElement(Type:=GetType({1}),ElementName:=""{2}"",IsNullable:={7}{3}{5})> _
		Public {6}{0} As {1}
		
		<XmlIgnore> _
		Public Property {4} As {1}
			Get
				{4} = {6}{0}
			End Get
			Set(Value As {1})
				{6}{0} = Value
			End Set
		End Property";

		//For the next 6 templates:
		// {0} - fieldName, {1} fieldType, {2} elementName, {3} schemaForm, {4} XsdDataType, 
		// {5} - propertyName, {6} - namespace
		// {7} -- hidden member prefix, {8} -- IsNullable
		// note: elementName is the unscrubbed version of fieldname
		// note: PropertyName is keyword scrubbed and fieldName is scrubbed

		static string  sElementObjectTemplate = 
@"		'*********************** {2} element ***********************
		<XmlElement(ElementName:=""{2}"",IsNullable:={8}{3}{4}{6})> _
		Public {7}{0} As {1}
		
		<XmlIgnore> _
		Public Property {5} As {1}
			Get
				{5} = {7}{0}
			End Get
			Set(Value As {1})
				{7}{0} = Value
			End Set
		End Property";

		static string sElementValueTypeTemplate =
@"		'*********************** {2} element ***********************
		<XmlElement(ElementName:=""{2}"",IsNullable:=False{3}{4}{6})> _
		Public {7}{0} As {1}
		
		<XmlIgnore> _
		Public {7}{0}Specified As Boolean
		
		<XmlIgnore> _
		Public Property {5} As {1}
			Get
				{5} = {7}{0}
			End Get
			Set(Value As {1})
				{7}{0} = Value
				{7}{0}Specified = True
			End Set
		End Property";

		static string  sElementAnyTemplate = 
@"		'*********************** {2} element ***********************
		<XmlAnyElement({6})> _
		Public {5} As System.Xml.XmlElement";

		static string  sElementAnyMaxOccursTemplate = 
@"		'*********************** {2} element ***********************
		<XmlAnyElement({6})> _
		Public {5}() As System.Xml.XmlElement";

		static string  sAttributeObjectTemplate = 
@"		'*********************** {2} attribute ***********************
		<XmlAttribute(AttributeName:=""{2}""{3}{4}{6})> _
		Public {7}{0} As {1}
		
		<XmlIgnore> _
		Public Property {5} As {1}
			Get
				{5} = {7}{0}
			End Get
			Set(Value As {1})
				{7}{0} = Value
			End Set
		End Property";

		static string sAttributeValueTypeTemplate =
@"		'*********************** {2} attribute ***********************
		<XmlAttribute(AttributeName:=""{2}""{3}{4}{6})> _
		Public {7}{0} As {1}
		
		<XmlIgnore> _
		Public {7}{0}Specified As Boolean
		
		<XmlIgnore> _
		Public Property {5} As {1}
			Get
				{5} = {7}{0}
			End Get
			Set(Value As {1})
				{7}{0} = Value
				{7}{0}Specified = True
			End Set
		End Property";

		static string  sAttributeAnyTemplate = 
@"		'*********************** {2} attribute ***********************
		<XmlAnyAttribute()> _
		Public {5}() As System.Xml.XmlAttribute";

		static string sElementDateTimeTemplate =
@"		'*********************** {2} element ***********************
		<XmlElement(ElementName:=""{2}"",IsNullable:=False{3}{4}{6})> _
		Public {7}{0} As DateTime
		
		<XmlIgnore> _
		Public {7}{0}Specified As Boolean
		
		<XmlIgnore> _
		Public Property {5} As DateTime
			Get
				{5} = {7}{0}
			End Get
			Set(Value As DateTime)
				{7}{0} = Value
				{7}{0}Specified = True
			End Set
		End Property
		
		<XmlIgnore> _
		Public Property {5}Utc As DateTime
			Get
				{5}Utc = {7}{0}.ToUniversalTime()
			End Get
			Set(Value As DateTime)
				{7}{0} = Value.ToLocalTime()
				{7}{0}Specified = True
			End Set
		End Property";

		static string sAttributeDateTimeTemplate =
@"		'*********************** {2} attribute ***********************
		<XmlAttribute(AttributeName:=""{2}""{3}{4}{6})> _
		Public {7}{0} As DateTime
		
		<XmlIgnore> _
		Public {7}{0}Specified As Boolean
		
		<XmlIgnore> _
		Public Property {5} As DateTime
			Get
				{5} = {7}{0}
			End Get
			Set(Value As DateTime)
				{7}{0} = Value
				{7}{0}Specified = True
			End Set
		End Property
		
		<XmlIgnore> _
		Public Property {5}Utc As DateTime
			Get
				{5}Utc = {7}{0}.ToUniversalTime()
			End Get
			Set(Value As DateTime)
				{7}{0} = Value.ToLocalTime()
				{7}{0}Specified = True
			End Set
		End Property";

		//For the next 3 templates:
		// {0} - fieldType, {1} XsdDataType, {2} -- hidden member prefix, {3} -- mixedElementFieldName

		static string sMixedObjectTemplate =
@"		'*********************** XmlText field ***********************
		<XmlText(DataType:=""{1}"")> _
		Public {2}{3} As {0}
        
		<XmlIgnore()> _
        Public Property {3}() As {0}
            Get
                {3} = {2}{3}
            End Get
            Set(ByVal val As {0})
                {2}{3} = val
            End Set
        End Property";

		static string sMixedValueTypeTemplate =
@"		'*********************** XmlText field ***********************
		<XmlText(GetType({1}))> _
		Public {2}{3} As {0}
        
		<XmlIgnore()> _
        Public {2}{3}Specified As Boolean
        
		<XmlIgnore()> _
        Public Property {3}() As {0}
            Get
                {3} = {2}{3}
            End Get
            Set(ByVal val As {0})
                {2}{3} = val
                {2}{3}Specified = True
            End Set
        End Property";

		static string sMixedDateTimeTemplate =
@"		'*********************** XmlText field ***********************
		<XmlText(DataType:=""{1}"")> _
		Public {2}{3} As System.DateTime
		
		<XmlIgnore> _
		Public {2}{3}Specified As Boolean
		
		<XmlIgnore> _
		Public Property {3}() As System.DateTime
			Get
				{3} = {2}{3}
			End Get
			Set(val As System.DateTime)
				{2}{3} = val
				{2}{3}Specified = True
			End Set
		End Property
		
		<XmlIgnore> _
		Public Property {3}Utc As System.DateTime
			Get
				{3}Utc = {2}{3}.ToUniversalTime()
			End Get
			Set(val As System.DateTime)
				{2}{3} = val.ToLocalTime()
				{2}{3}Specified = True
			End Set
		End Property";

		// {0} -- fieldname of contained collection (scrubbed)
		// {1} -- type contained in collection -- keyword scrubbed
		// {2} -- collection suffix -- not needed anymore
        // {3} -- hiddenMemberPrex
		static string sClassEnumerabilityTemplate =
@"		<System.Runtime.InteropServices.DispIdAttribute(-4)> _
		Public Function GetEnumerator() As IEnumerator 
			GetEnumerator = {0}.GetEnumerator()
		End Function

		Public Sub Add(ByVal obj As {1})
			{0}.Add(obj)
		End Sub
			
		<XmlIgnore()> _
		Default Public ReadOnly Property Item(ByVal index As Integer) As {1}
			Get
				Item = {0}(index)
			End Get
		End Property

		<XmlIgnore()> _
		Public ReadOnly Property Count() As Integer
			Get
                Count = {0}.Count
			End Get
		End Property

		Public Sub Clear()
            {0}.Clear()
		End Sub

		Public Function Remove(ByVal index As Integer) As {1}
			Dim obj As {1}
			obj = {0}(index)
			Remove = obj
			{0}.Remove(obj)
		End Function

		Public Function Remove(ByVal obj As {1}) As Boolean
			Remove = {0}.Remove(obj)
		End Function";

        //ACORD TC Template:
        // {0} - elementName, {1} enumName, {2} -- tcCode, {3} -- valueCode

        static string sAcordTCTemplate =
@"";

        static string sAcordPrivateTCTemplate =
@"";

		private Hashtable keywordsTable;

		String [] keywords = 
				{	"AddHandler","AddressOf","Alias","And","Ansi",
					"As","Assembly","Auto","Base","Boolean",
					"ByRef","Byte","ByVal","Call","Case",
					"Catch","CBool","CByte","CChar","CDate",
					"CDec","CDbl","Char","CInt","Class",
					"CLng","CObj","Const","CShort","CSng",
					"CStr","CType","Date","Decimal","Declare","Declarations",
					"Default","Delegate","Dim","Do","Double",
					"Each","Else","ElseIf","End","Enum",
					"Erase","Error","Event","Exit","ExternalSource",
					"False","Finalize","Finally","Float",	
					"For","Friend","Function","Get","GetType",
					"Goto",	"Handles","If","Implements","Imports",
					"In","Inherits","Integer","Interface","Is",
					"Let","Lib","Like","Long","Loop",
					"Me","Mod","Module","MustInherit","MustOverride",
					"MyBase","MyClass","Namespace","New","Next",
					"Not","Nothing","NotInheritable","NotOverridable","Object",
					"On","Option","Optional","Or","Overloads",
					"Overridable","Overrides","ParamArray","Preserve","Private",
					"Property","Protected","Public","RaiseEvent","ReadOnly",
					"ReDim","Region","REM","RemoveHandler","Resume",
					"Return","Select","Set","Shadows","Shared",
					"Short","Single","Static","Step","Stop",
					"String","Structure","Sub","SyncLock","Then",
					"Throw","To","True","Try","TypeOf",
					"Unicode","Until","Volatile","When","While",
					"With","WithEvents","WriteOnly","Xor"};


		public VBTemplate()
		{
			keywordsTable = new Hashtable(keywords.Length);
			for (int i=0; i<keywords.Length; i++)
				keywordsTable.Add(keywords[i].ToLower(), "");
		}

		protected override string FieldCollectionTemplate { get { return sFieldCollectionTemplate; } set { sFieldCollectionTemplate = value; } }
		protected override string FieldClassTemplate { get { return sFieldClassTemplate; } set { sFieldClassTemplate = value; } }
		protected override string FieldAbstractClassTemplate { get { return sFieldAbstractClassTemplate; } set { sFieldAbstractClassTemplate = value; } }
		protected override string ElementObjectTemplate { get { return sElementObjectTemplate; } set { sElementObjectTemplate = value; } }
		protected override string ElementValueTypeTemplate { get { return sElementValueTypeTemplate; } set { sElementValueTypeTemplate = value; } }
		protected override string ElementAnyTemplate { get { return sElementAnyTemplate; } set { sElementAnyTemplate = value; } }
		protected override string ElementAnyMaxOccursTemplate { get { return sElementAnyMaxOccursTemplate; } set { sElementAnyMaxOccursTemplate = value; } }
		protected override string AttributeObjectTemplate { get { return sAttributeObjectTemplate; } set { sAttributeObjectTemplate = value; } }
		protected override string AttributeValueTypeTemplate { get { return sAttributeValueTypeTemplate; } set { sAttributeValueTypeTemplate = value; } }
		protected override string AttributeAnyTemplate { get { return sAttributeAnyTemplate; } set { sAttributeAnyTemplate = value; } }
		protected override string ElementDateTimeTemplate { get { return sElementDateTimeTemplate; } set { sElementDateTimeTemplate = value; } }
		protected override string AttributeDateTimeTemplate { get { return sAttributeDateTimeTemplate; } set { sAttributeDateTimeTemplate = value; } }
		protected override string MixedObjectTemplate { get { return sMixedObjectTemplate; } set { sMixedObjectTemplate = value; } }
		protected override string MixedValueTypeTemplate { get { return sMixedValueTypeTemplate; } set { sMixedValueTypeTemplate = value; } }
		protected override string MixedDateTimeTemplate { get { return sMixedDateTimeTemplate; } set { sMixedDateTimeTemplate = value; } }
		protected override string ClassEnumerabilityTemplate { get { return sClassEnumerabilityTemplate; } set { sClassEnumerabilityTemplate = value; } }
        protected override string AcordTCTemplate { get { return sAcordTCTemplate; } set { sAcordTCTemplate = value; } }
        protected override string AcordPrivateTCTemplate { get { return sAcordPrivateTCTemplate; } set { sAcordPrivateTCTemplate = value; } }
        protected override string AttributeAssignmentOperator { get { return ":="; } }
		protected override string HideInheritedMethodKeyword { get { return "Shadows "; } }
        protected override string PartialKeyword { get { return "Partial "; } }

		public override void NamespaceHeaderCode(StreamWriter outStream, String ns, String schemaFile,
			Hashtable forwardDeclarations, string targetNamespace, Hashtable enumerations, bool depthFirstTraversalHooks,
			ArrayList importedReferences)
		{
			schemaTargetNamespace = targetNamespace;

			outStream.WriteLine("' Copyright 2008, Microsoft Corporation");
			outStream.WriteLine("' Sample Code - Use restricted to terms of use defined in the accompanying license agreement (EULA.doc)");
			outStream.WriteLine();
			outStream.WriteLine("'--------------------------------------------------------------");
			outStream.WriteLine("' Autogenerated by XSDObjectGen version {0}", 
				this.GetType().Assembly.GetName(false).Version);
			outStream.WriteLine("' Schema file: {0}", schemaFile);
			outStream.WriteLine("' Creation Date: {0}", DateTime.Now.ToString());
			outStream.WriteLine("'--------------------------------------------------------------");
			outStream.WriteLine();
			outStream.WriteLine("Imports System");
			outStream.WriteLine("Imports System.Xml.Serialization");
            outStream.WriteLine("Imports System.Collections");
			outStream.WriteLine("Imports System.Collections.Generic");
			outStream.WriteLine("Imports System.Xml.Schema");
			outStream.WriteLine("Imports System.ComponentModel");
		
			outStream.WriteLine();
			// namespace can be omitted in VB -- which then defaults to the .vbproj project setting
			if (ns != "") outStream.WriteLine("Namespace {0}", ns);
			outStream.WriteLine();
			outStream.WriteLine("\tPublic Module Declarations");
			outStream.WriteLine(string.Format("\t\tPublic Const SchemaVersion As String = \"{0}\"", targetNamespace));
			outStream.WriteLine("\tEnd Module");
			outStream.WriteLine();

			if (depthFirstTraversalHooks)
			{
				outStream.WriteLine("\tPublic Delegate Sub DepthFirstTraversalDelegate(instance As Object, parent As Object, context As Object)");
				outStream.WriteLine();
			}

			// Add enumerations
			foreach(string key in enumerations.Keys)
			{
				outStream.WriteLine("\t<Serializable> _");
				outStream.WriteLine("\tPublic Enum {0}", key);
				
				ArrayList enumValues = (ArrayList) enumerations[key];
				for(int i=0; i<enumValues.Count; i++)
				{
					string [] enumValue = (string []) enumValues[i];
					outStream.WriteLine("\t\t<XmlEnum(Name:=\"{0}\")> {1}", enumValue[0],
						CheckForKeywords(enumValue[1]));
				}

				outStream.WriteLine("\tEnd Enum");
				outStream.WriteLine();
			}

			outStream.WriteLine();
		}

		public override void ClassHeaderCode(StreamWriter outStream, string dotnetClassName, string elementName, 
			string complexTypeBaseClass, bool baseIsAbstract, bool isSchemaType, bool isAbstract, bool isLocalComplexType, Hashtable enumerableClasses, 
			string ns, XmlSchemaForm elementFormDefault, string annotation, bool isElementNullable, ArrayList xmlIncludedClasses,
			bool globalElementAndSchemaTypeHaveSameName)
		{
			outStream.WriteLine();
			outStream.WriteLine();
			outStream.WriteLine("\t'--------------------------------------------------");
			outStream.WriteLine("\t'{0} {1}", isSchemaType || isLocalComplexType ? dotnetClassName : elementName,
				isSchemaType ? "type" : "element");
			outStream.WriteLine("\t'--------------------------------------------------");
			
			string nameSpace = "";
			if (isSchemaType)
			{	
				if (elementFormDefault == XmlSchemaForm.Qualified || elementFormDefault == XmlSchemaForm.Unqualified) 
				{
					nameSpace = CalculateNamespace(schemaTargetNamespace, ns, false);
				}
				if (globalElementAndSchemaTypeHaveSameName)
				{
					// possible root node element -- so put namespace on the element if targetNamesapce has been set
					nameSpace = CalculateNamespace(schemaTargetNamespace, ns, false);

					outStream.WriteLine("\t<XmlRoot(ElementName:=\"{0}\"{1},IsNullable:={2}),Serializable, _", 
						elementName, nameSpace, isElementNullable.ToString());
					outStream.Write("\tXmlType(TypeName:=\"{0}\"{1})", 
						dotnetClassName, nameSpace);
				}
				else
				{
					outStream.Write("\t<XmlType(TypeName:=\"{0}\"{1}),Serializable", dotnetClassName, nameSpace);
				}
			}
			else if (isLocalComplexType)
			{	
				if (elementFormDefault == XmlSchemaForm.Qualified || elementFormDefault == XmlSchemaForm.Unqualified) 
				{
					nameSpace = CalculateNamespace(schemaTargetNamespace, ns, false);
				}
				outStream.Write("\t<XmlType(TypeName:=\"{0}\"{1}),Serializable", 
					dotnetClassName, nameSpace);
			}
			else 
			{
				// possible root node element -- so put namespace on the element if targetNamesapce has been set
				nameSpace = CalculateNamespace(schemaTargetNamespace, ns, false);
				outStream.Write("\t<XmlRoot(ElementName:=\"{0}\"{1},IsNullable:={2}),Serializable", 
					elementName, nameSpace, isElementNullable.ToString());
			}

			// Add necessary XmlInludes for abstract derived types used in the class
			if (xmlIncludedClasses.Count > 0)
				outStream.WriteLine(", _");
			for (int i=0; i<xmlIncludedClasses.Count; i++)
			{
				outStream.Write("\tXmlInclude(GetType({0}))", CheckForKeywords((string)xmlIncludedClasses[i]));
				if ((i+1) < xmlIncludedClasses.Count) outStream.WriteLine(", _");
			}

			outStream.WriteLine("> _");

			string className = CheckForKeywords(dotnetClassName);
            outStream.WriteLine("\t{2}Public {1}Class {0}", className, isAbstract ? "MustInherit " : "", partialClasses ? PartialKeyword : "");

			// setup inheritance for <xsd:extension base="class">
			if (complexTypeBaseClass != null && complexTypeBaseClass != "")
				outStream.WriteLine("\t\tInherits {0}", CheckForKeywords(complexTypeBaseClass));

			// setup enumerability over a contained collection
			if (enumerableClasses.ContainsKey(dotnetClassName))
			{
				ArrayList values = (ArrayList) enumerableClasses[dotnetClassName];

				outStream.WriteLine();
				string collectionName = (string) values[0];
				collectionName = ReplaceInvalidChars(collectionName);
				outStream.WriteLine(ClassEnumerabilityTemplate, collectionName,
                    ConvertSystemDatatype((string)values[1]), collectionSuffix, hiddenMemberPrefix);
			}
		}

		public override void ClassTrailerCode(StreamWriter outStream, string dotnetClassName, ArrayList ctorList, 
			bool defaultInitialization, bool depthFirstTraversalHooks, bool makeSchemaCompliant, string complexTypeBaseClass, bool baseClassIsMixed, bool mixed, string mixedXsdType)
		{
            // For mixed content (an element that has children and also text), add a special text field.
            // Base class cannot be mixed, otherwise XmlSerializer error will occur.  Can only have 1 XmlText().
            if (mixed && !baseClassIsMixed)
			{
				outStream.WriteLine();

				string clrType;
                if (mixedXsdType.StartsWith("System."))
                {
                    clrType = mixedXsdType;
                    mixedXsdType = XsdTypeMapping(mixedXsdType);
                }
                else
                    clrType = FrameworkTypeMapping(mixedXsdType);

                if (clrType == "System.DateTime")
					outStream.WriteLine(MixedDateTimeTemplate, ConvertSystemDatatype(clrType), mixedXsdType, hiddenMemberPrefix, mixedElementFieldName);
                else if (IsValueType(clrType))
                    outStream.WriteLine(MixedValueTypeTemplate, ConvertSystemDatatype(clrType), clrType, hiddenMemberPrefix, mixedElementFieldName);
                else
                    outStream.WriteLine(MixedObjectTemplate, "String", "String", hiddenMemberPrefix, mixedElementFieldName);
			}

			bool inherits = (complexTypeBaseClass != null && complexTypeBaseClass != "");

			// Add class constructor
			outStream.WriteLine();
			outStream.WriteLine("\t\t'*********************** Constructor ***********************");
			outStream.WriteLine("\t\tPublic Sub New()");
			if (inherits) outStream.WriteLine("\t\t\tMyBase.New()");

			// Default any DateTime fields to a value.
			for (int i=0; i<ctorList.Count; i++)
			{
				ClassConstructor ctor = (ClassConstructor) ctorList[i];
				// make sure the datetime fields are initialized to Now if a constructor default value is not set
				if (ctor.datatype == CtorDatatypeContext.DateTime && (!defaultInitialization || (defaultInitialization  && !ctor.required))) 
				{
					outStream.WriteLine("\t\t\t{1}{0} = DateTime.Now", ReplaceInvalidChars(ctor.fieldName), hiddenMemberPrefix);
				}
			}

			// If some fields in the class have defaults or fixed values, add a constructor.
			// Also force creation of required attributes and elmenets, so the schema is always valid.
			if (defaultInitialization)
			{
				for (int i=0; i<ctorList.Count; i++)
				{
					ClassConstructor ctor = (ClassConstructor) ctorList[i];
					if (!ctor.required) continue;

					if (ctor.datatype == CtorDatatypeContext.PropertyCollection ||
						ctor.datatype == CtorDatatypeContext.PropertyCollectionString || 
						ctor.datatype == CtorDatatypeContext.PropertyCollectionComplexType ||
						ctor.datatype == CtorDatatypeContext.PropertyCollectionAbstractComplexType)
					{
					}
					else if (ctor.datatype == CtorDatatypeContext.Property)  // class instance field -- so build code to force the constructor to fire and create an instance
					{
						//outStream.WriteLine("\t\t\tDim obj{1} As {0} = {2}", CheckForKeywords(ctor.fieldName), i, 
						//	CheckForKeywords(ctor.defaultValue));
					}
					else if (ctor.datatype == CtorDatatypeContext.DateTime) // standard value type with no default value
					{
						outStream.WriteLine("\t\t\t{0} = DateTime.Now", ReplaceInvalidChars(ctor.fieldName));
					}
					else if (ctor.datatype == CtorDatatypeContext.ValueType)   // standard value type with default value 
					{
						outStream.WriteLine("\t\t\t{1}{0}Specified = True", ReplaceInvalidChars(ctor.fieldName), hiddenMemberPrefix);
					}
					else if  (ctor.datatype == CtorDatatypeContext.ValueTypeDefault) // valuetypes field with a default value
					{
						outStream.WriteLine("\t\t\t{0} = {1}", CheckForKeywords(ctor.fieldName), ctor.defaultValue);
					}
					else if (ctor.datatype == CtorDatatypeContext.String)
					{
						if (ctor.defaultValue == "")
							outStream.WriteLine("\t\t\t{0} = String.Empty", CheckForKeywords(ctor.fieldName), ctor.defaultValue);
						else
							outStream.WriteLine("\t\t\t{0} = \"{1}\"", CheckForKeywords(ctor.fieldName), ctor.defaultValue);
					}
				}
			}

			outStream.WriteLine("\t\tEnd Sub");

			// Add MakeSchemaCompliant code for required child classes
			if (makeSchemaCompliant)
			{
				outStream.WriteLine();
				outStream.WriteLine("\t\t'*********************** MakeSchemaCompliant ***********************");
				outStream.WriteLine("\t\tPublic {0}Sub MakeSchemaCompliant()", (inherits) ? HideInheritedMethodKeyword : "");	
				if (inherits) outStream.WriteLine("\t\t\tMyBase.MakeSchemaCompliant()");

				for (int i=0; i<ctorList.Count; i++)
				{
					ClassConstructor ctor = (ClassConstructor) ctorList[i];
					if (!ctor.required) continue;

                    // removed makeschemacompliant Add calls due to generic list
					if (ctor.datatype == CtorDatatypeContext.PropertyCollection)  
					{
						//outStream.WriteLine("\t\t\tIf {0}{1}.Count = 0 Then {0}{1}.Add()", ReplaceInvalidChars(ctor.defaultValue), collectionSuffix);
					}
					else if (ctor.datatype == CtorDatatypeContext.PropertyCollectionString)  
					{
						//outStream.WriteLine("\t\t\tIf {0}{1}.Count = 0 Then {0}{1}.Add(\"\")", ReplaceInvalidChars(ctor.defaultValue), collectionSuffix);
					}
					else if (ctor.datatype == CtorDatatypeContext.PropertyCollectionComplexType)  
					{
                        outStream.WriteLine("\t\t\tFor Each _c as {0} in {1}", CheckForKeywords(ctor.fieldName),
							ReplaceInvalidChars(ctor.defaultValue));
						outStream.WriteLine("\t\t\t\t_c.MakeSchemaCompliant()");
						outStream.WriteLine("\t\t\tNext");
					}
					else if (ctor.datatype == CtorDatatypeContext.PropertyCollectionAbstractComplexType)  
					{
						outStream.WriteLine("\t\t\tFor Each _c as {0} in {1}", CheckForKeywords(ctor.fieldName),
							ReplaceInvalidChars(ctor.defaultValue));
						outStream.WriteLine("\t\t\t\t_c.MakeSchemaCompliant()");
						outStream.WriteLine("\t\t\tNext");
					}
					else if (ctor.datatype == CtorDatatypeContext.Property)   // class instance field -- so build code to force the constructor to fire and create an instance
					{
						outStream.WriteLine("\t\t\t{0}.MakeSchemaCompliant()", CheckForKeywords(ctor.defaultValue));
					}
				}

				outStream.WriteLine("\t\tEnd Sub");
			}

			// Add DepthFirstTraversal hooks
			if (depthFirstTraversalHooks)
			{
				outStream.WriteLine();
				outStream.WriteLine("\t\t'*********************** DepthFirstTraversal Event ***********************");
				outStream.WriteLine("\t\tPublic Shared {0}Event DepthFirstTraversalEvent As DepthFirstTraversalDelegate", (inherits) ? HideInheritedMethodKeyword : "");
				outStream.WriteLine("\t\tPublic {0}Sub DepthFirstTraversal(parent As Object, context As Object)", (inherits) ? HideInheritedMethodKeyword : "");
				outStream.WriteLine("\t\t\tRaiseEvent DepthFirstTraversalEvent(Me, parent, context)");
				if (inherits) outStream.WriteLine("\t\t\tMyBase.DepthFirstTraversal(parent, context)");

				for (int i=0; i<ctorList.Count; i++)
				{
					ClassConstructor ctor = (ClassConstructor) ctorList[i];
					if (ctor.datatype == CtorDatatypeContext.PropertyCollectionComplexType || ctor.datatype == CtorDatatypeContext.PropertyCollectionAbstractComplexType)  
					{
						outStream.WriteLine("\t\t\tIf Not({0}{1} Is Nothing) Then", hiddenMemberPrefix, ReplaceInvalidChars(ctor.defaultValue));
						outStream.WriteLine("\t\t\t\tFor Each _d As {2} in {0}{1}", hiddenMemberPrefix, ReplaceInvalidChars(ctor.defaultValue), CheckForKeywords(ctor.fieldName));
						outStream.WriteLine("\t\t\t\t\t_d.DepthFirstTraversal(Me, context)");
						outStream.WriteLine("\t\t\t\tNext");
						outStream.WriteLine("\t\t\tEnd If");
					}
					else if (ctor.datatype == CtorDatatypeContext.Property)   // class instance field -- so build code to force the constructor to fire and create an instance
					{
						outStream.WriteLine("\t\t\tIf Not({0}{1} is Nothing) Then {0}{1}.DepthFirstTraversal(Me, context)", hiddenMemberPrefix, ReplaceInvalidChars(ctor.defaultValue));
					}
				}

				outStream.WriteLine("\t\tEnd Sub");
			}

			outStream.WriteLine("\tEnd Class");
		}

		public override void NamespaceTrailerCode(StreamWriter outStream, string ns)
		{
			// namespace can be omitted in VB -- which then defaults to the .vbproj project setting
			if (ns != "") outStream.WriteLine("End Namespace");
		}

		public override string ConvertSystemDatatype(string systemType)
		{
			switch (systemType)
			{
				case "System.String" : 
					return "String";
				case "System.SByte" : 
					return "SByte";
				case "System.Byte" : 
					return "Byte";
				case "System.Int16" : 
					return "Short";
				case "System.UInt16" : 
					return "UInt16";
				case "System.Int32" : 
					return "Integer";
				case "System.UInt32" : 
					return "UInt32";
				case "System.Int64" : 
					return "Long";
				case "System.UInt64" : 
					return "UInt64";
				case "System.Single" : 
					return "Single";
				case "System.Double" : 
					return "Double";
				case "System.Boolean" : 
					return "Boolean";
				case "System.Decimal" :
					return "Decimal";
				case "System.Char" : 
					return "Char";
				case "System.Object" : 
					return "Object";
				case "System.Byte[]" :
					return "Byte()";
				case "System.DateTime" :
					return "DateTime";
				case "System.Xml.XmlQualifiedName":
					return "System.Xml.XmlQualifiedName";
				case "System.Xml.XmlElement" :
					return "System.Xml.XmlElement";
				case "System.Xml.XmlAttribute[]" :
					return "System.Xml.XmlAttribute()";
				default:
					// other value types, change type to string
					if (systemType.StartsWith("System.")) 
						return "String";
					else // custom enum types
						return CheckForKeywords(systemType);
			}
		}

		// todo: put this into LanguageTemplate as only one line of code differs from C#
		public override string CheckForKeywords(String keyword)
		{
			string modifiedKeyword = keyword;

			// if a namespace prefixes the word, check the right half of the qualified value
			string [] cct = keyword.Split('.');
			if (cct != null && cct.Length >= 2 && !keyword.StartsWith("System."))
			{
				modifiedKeyword = cct[cct.Length-1];
			}

			modifiedKeyword = ReplaceInvalidChars(modifiedKeyword);
			if (keywordsTable.ContainsKey(modifiedKeyword.ToLower()))
				modifiedKeyword = "[" + modifiedKeyword + "]";
			
			
			if (cct != null && cct.Length >= 2 && !modifiedKeyword.StartsWith("System."))
			{
				string ret = "";
				for (int i=0; i<cct.Length-1; i++)
					ret = ret + cct[i] + ".";
				ret = ret + modifiedKeyword;
				modifiedKeyword = ret;
			}

			return modifiedKeyword;
		}
	}
}
