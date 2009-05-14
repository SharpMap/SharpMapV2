using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Collections;

namespace XSDObjectGenLib
{
	/// <summary>
	/// LanguageBase implementation for C#.
	/// </summary>
	internal class CSharpTemplate : LanguageBase
	{

		// {0} -- collection class name (scrubbed) -- NOT NEEDED ANYMORE WITH GENERIC LIST
		// {1} -- type (keyword scrubbed)
		// {2} -- xml element name
		// {3} -- schema form
		// {4} -- field/property name (scrubbed)
		// {5} -- XmlElement DataType
		// {6} -- namespace
		// {7} -- hidden member prefix
        // {8} -- collection suffix -- NOT NEEDED ANYMORE WITH GENERIC LIST
		// {9} -- IsNullable
		// {10} -- contained type within the collection
		static string sFieldCollectionTemplate =
@"		[XmlIgnore]
		private List<{10}> {7}{4};
		
        [XmlElement(Type=typeof({10}),ElementName=""{2}"",IsNullable={9}{3}{5}{6})]
		public List<{10}> {4}
		{{
			get
			{{
				if ({7}{4} == null) {7}{4} = new List<{10}>();
				return {7}{4};
			}}
			set {{{7}{4} = value;}}
		}}";


		// {0} -- field name (scrubbed)
		// {1} -- type (keyword scrubbed)
		// {2} -- xml element name
		// {3} -- schema form
		// {4} -- property name (keyword scrubbed)
		// {5} -- namespace
		// {6} -- hidden member prefix
		// {7} -- IsNullable
		static string  sFieldClassTemplate = 
@"		[XmlIgnore]
		private {1} {6}{0};
		
        [XmlElement(Type=typeof({1}),ElementName=""{2}"",IsNullable={7}{3}{5})]
		public {1} {4}
		{{
			get
			{{
				if ({6}{0} == null) {6}{0} = new {1}();		
				return {6}{0};
			}}
			set {{{6}{0} = value;}}
		}}";


		// {0} -- field name (scrubbed)
		// {1} -- type (keyword scrubbed)
		// {2} -- xml element name
		// {3} -- schema form
		// {4} -- property name (keyword scrubbed)
		// {5} -- namespace
		// {6} -- hidden member prefix
		// {7} -- IsNullable
		static string  sFieldAbstractClassTemplate =
@"		[XmlIgnore]
		private {1} {6}{0};
		
		[XmlElement(Type=typeof({1}),ElementName=""{2}"",IsNullable={7}{3}{5})]
		public {1} {4}
		{{
			get {{ return {6}{0}; }}
			set {{{6}{0} = value;}}
		}}";


		//For the next 6 templates:
		// {0} - fieldName, {1} fieldType, {2} elementName, {3} schemaForm, {4} XsdDataType, 
		// {5} - propertyName, {6} - namespace
		// {7} -- hidden member prefix, {8} -- IsNullable
		// note: elementName is the unscrubbed version of fieldname
		// note: PropertyName is keyword scrubbed and fieldName is scrubbed

		static string sElementObjectTemplate =
@"		[XmlIgnore]
		private {1} {7}{0};
		
		[XmlElement(ElementName=""{2}"",IsNullable={8}{3}{4}{6})]
		public {1} {5}
		{{ 
			get {{ return {7}{0}; }}
			set {{ {7}{0} = value; }}
		}}";

		static string sElementValueTypeTemplate =
@"		[XmlIgnore]
        private {1} {7}{0};
		
		[XmlIgnore]
		public bool {7}{0}Specified;
		
		
        [XmlElement(ElementName=""{2}"",IsNullable=false{3}{4}{6})]
		public {1} {5}
		{{ 
			get {{ return {7}{0}; }}
			set {{ {7}{0} = value; {7}{0}Specified = true; }}
		}}";

		static string sElementAnyTemplate =
@"		[XmlAnyElement({6})]
		public System.Xml.XmlElement {5};";

		static string sElementAnyMaxOccursTemplate =
@"		[XmlAnyElement({6})]
		public System.Xml.XmlElement[] {5};";
		
		static string sAttributeObjectTemplate =
@"		[XmlIgnore]
		private {1} {7}{0};
		
		[XmlAttribute(AttributeName=""{2}""{3}{4}{6})]
		public {1} {5}
		{{ 
			get {{ return {7}{0}; }}
			set {{ {7}{0} = value; }}
		}}";

		static string sAttributeValueTypeTemplate =
@"		[XmlIgnore]
		private {1} {7}{0};
		
		[XmlIgnore]
		public bool {7}{0}Specified;
		
		[XmlAttribute(AttributeName=""{2}""{3}{4}{6})]
		public {1} {5}
		{{ 
			get {{ return {7}{0}; }}
			set {{ {7}{0} = value; {7}{0}Specified = true; }}
		}}";

		static string sAttributeAnyTemplate =
@"		[XmlAnyAttribute()]
		public System.Xml.XmlAttribute[] {5};";

		static string sElementDateTimeTemplate =
@"		[XmlIgnore]
		private System.DateTime {7}{0};
		
		[XmlIgnore]
		public bool {7}{0}Specified;
		
		
        [XmlElement(ElementName=""{2}"",IsNullable=false{3}{4}{6})]
		public System.DateTime {5}
		{{ 
			get {{ return {7}{0}; }}
			set {{ {7}{0} = value; {7}{0}Specified = true; }}
		}}
		
		[XmlIgnore]
		public System.DateTime {5}Utc
		{{ 
			get {{ return {7}{0}.ToUniversalTime(); }}
			set {{ {7}{0} = value.ToLocalTime(); {7}{0}Specified = true; }}
		}}";

		static string sAttributeDateTimeTemplate =
@"		[XmlIgnore]
		private System.DateTime {7}{0};
		
		[XmlIgnore]
		public bool {7}{0}Specified;
		
		[XmlAttribute(AttributeName=""{2}""{3}{4}{6})]
		public System.DateTime {5}
		{{ 
			get {{ return {7}{0}; }}
			set {{ {7}{0} = value; {7}{0}Specified = true; }}
		}}
		
		[XmlIgnore]
		public System.DateTime {5}Utc
		{{ 
			get {{ return {7}{0}.ToUniversalTime(); }}
			set {{ {7}{0} = value.ToLocalTime(); {7}{0}Specified = true; }}
		}}";


		//For the next 3 templates:
		// {0} - fieldType, {1} XsdDataType, {2} -- hidden member prefix, {3} -- mixedElementFieldName

		static string sMixedObjectTemplate =
@"		[XmlIgnore]
		private {0} {2}{3};
		
		[XmlText(DataType=""{1}"")]
		public {0} {3}
		{{ 
			get {{ return {2}{3}; }}
			set {{ {2}{3} = value; }}
		}}";

		static string sMixedValueTypeTemplate =
@"		[XmlIgnore]
		private {0} {2}{3};
		
		[XmlIgnore]
		public bool {2}{3}Specified;
		
		[XmlText(typeof({1}))]
		public {0} {3}
		{{ 
			get {{ return {2}{3}; }}
			set {{ {2}{3} = value; {2}{3}Specified = true; }}
		}}";

		static string sMixedDateTimeTemplate =
@"		[XmlIgnore]
		private System.DateTime {2}{3};
		
		[XmlIgnore]
		public bool {2}{3}Specified;
		
		[XmlText(DataType=""{1}"")]
		public System.DateTime {3}
		{{ 
			get {{ return {2}{3}; }}
			set {{ {2}{3} = value; {2}{3}Specified = true; }}
		}}
		
		[XmlIgnore]
		public System.DateTime {3}Utc
		{{ 
			get {{ return {2}{3}.ToUniversalTime(); }}
			set {{ {2}{3} = value.ToLocalTime(); {2}{3}Specified = true; }}
		}}";

		// {0} -- fieldname of contained collection (scrubbed)
		// {1} -- type contained in collection -- keyword scrubbed
		// {2} -- collection suffix -- NOT NEEDED ANYMORE
        // {3} -- hiddenMemberPrex
		static string sClassEnumerabilityTemplate =
@"		[System.Runtime.InteropServices.DispIdAttribute(-4)]
		public IEnumerator GetEnumerator() 
		{{
            return {0}.GetEnumerator();
		}}

		public void Add({1} obj)
		{{
			{0}.Add(obj);
		}}

		[XmlIgnore]
		public {1} this[int index]
		{{
			get {{ return ({1}) {0}[index]; }}
		}}

		[XmlIgnore]
        public int Count 
		{{
            get {{ return {0}.Count; }}
        }}

        public void Clear()
		{{
			{0}.Clear();
        }}

		public {1} Remove(int index) 
		{{ 
            {1} obj = {0}[index];
            {0}.Remove(obj);
			return obj;
        }}

        public bool Remove({1} obj)
		{{
            return {0}.Remove(obj);
        }}";

        //ACORD TC Template -- for public codes:
        // {0} - elementName, {1} enumName, {2} -- tcCode, {3} -- valueCode

        static string sAcordTCTemplate =
@"		public void Set_{0}() 
        {{ 
            this.tc = (int){1}_TC._{2}; 
            this.Value = ""{3}""; 
        }}";

        //ACORD TC Template -- for Private codees (organizational private codes)
        // {0} - elementName, {1} enumName, {2} -- tcCode, {3} -- valueCode

        static string sAcordPrivateTCTemplate =
@"		public void Set_{0}() 
        {{ 
            this.tc = {2}; 
            this.Value = ""{3}""; 
        }}";

		private Hashtable keywordsTable;

		String [] keywords = 
				{	"abstract","as","base","bool","break","byte","case","catch","char",
					"checked","class","const","continue","DateTime","decimal","Declarations","default","delegate",
					"do","double","else","enum","event","explicit","extern","false",
					"finally","fixed","float","for","foreach","get","goto","if","implicit",
					"in","int","interface","internal","is","lock","long","namespace",
					"new","null","object","operator","out","override","params",
					"private","protected","public","readonly","ref","return","sbyte", 
					"sealed","set","short","sizeof","stackalloc","static","string","struct",
					"switch","this","throw","Time","true","try","typeof","uint","ulong",
					"unchecked","unsafe","ushort","using","value","virtual","void","while"};


		public CSharpTemplate()
		{
			keywordsTable = new Hashtable(keywords.Length);
			for (int i=0; i<keywords.Length; i++)
				keywordsTable.Add(keywords[i].ToLower(), "");
		}

		// Code string templates.  See documentation in the language child classes
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
		protected override string AttributeAssignmentOperator { get { return "="; } }
		protected override string HideInheritedMethodKeyword { get { return "new "; } }
        protected override string PartialKeyword { get { return "partial "; } }

		public override void NamespaceHeaderCode(StreamWriter outStream, String ns, String schemaFile, 
			Hashtable forwardDeclarations, string targetNamespace, Hashtable enumerations, bool depthFirstTraversalHooks,
			ArrayList importedReferences)
		{
			schemaTargetNamespace = targetNamespace;

            outStream.WriteLine("#pragma warning disable 1591");
            outStream.WriteLine();
			outStream.WriteLine("// Copyright 2008, Microsoft Corporation");
			outStream.WriteLine("// Sample Code - Use restricted to terms of use defined in the accompanying license agreement (EULA.doc)");
			outStream.WriteLine();
			outStream.WriteLine("//--------------------------------------------------------------");
			outStream.WriteLine("// Autogenerated by XSDObjectGen version {0}", 
				this.GetType().Assembly.GetName(false).Version);
			outStream.WriteLine("// Schema file: {0}", schemaFile);
			outStream.WriteLine("// Creation Date: {0}", DateTime.Now.ToString());
			outStream.WriteLine("//--------------------------------------------------------------");
			outStream.WriteLine();
			outStream.WriteLine("using System;");
			outStream.WriteLine("using System.Xml.Serialization;");
            outStream.WriteLine("using System.Collections;");
            outStream.WriteLine("using System.Collections.Generic;");
			outStream.WriteLine("using System.Xml.Schema;");
			outStream.WriteLine("using System.ComponentModel;");
	
			outStream.WriteLine();
			outStream.WriteLine("namespace {0}", ns);
			outStream.WriteLine("{");
			outStream.WriteLine();
			outStream.WriteLine("\tpublic struct Declarations");
			outStream.WriteLine("\t{");
			outStream.WriteLine(string.Format("\t\tpublic const string SchemaVersion = \"{0}\";", targetNamespace));
			outStream.WriteLine("\t}");
			outStream.WriteLine();
			
			if (depthFirstTraversalHooks)
			{
				outStream.WriteLine("\tpublic delegate void DepthFirstTraversalDelegate(object instance, object parent, object context);");
				outStream.WriteLine();
			}

			// Add enumerations
			foreach(string key in enumerations.Keys)
			{
				outStream.WriteLine("\t[Serializable]");
                if (key.StartsWith("OLI_LU_"))
				    outStream.WriteLine("\tpublic enum {0} : int", key);
                else
                    outStream.WriteLine("\tpublic enum {0}", key);
				outStream.WriteLine("\t{");
				
				ArrayList enumValues = (ArrayList) enumerations[key];
				for(int i=0; i<enumValues.Count; i++)
				{
					string [] enumValue = (string []) enumValues[i];
					if (i == (enumValues.Count-1))
					{
                        if (key.StartsWith("OLI_LU_"))
                            outStream.WriteLine("\t\t[XmlEnum(Name=\"{0}\")] {1} = {0}", enumValue[0], CheckForKeywords(enumValue[1]));
                        else
						    outStream.WriteLine("\t\t[XmlEnum(Name=\"{0}\")] {1}", enumValue[0], CheckForKeywords(enumValue[1]));
					}
					else
					{
                        if (key.StartsWith("OLI_LU_"))
                            outStream.WriteLine("\t\t[XmlEnum(Name=\"{0}\")] {1} = {0},", enumValue[0], CheckForKeywords(enumValue[1]));
                        else
						    outStream.WriteLine("\t\t[XmlEnum(Name=\"{0}\")] {1},", enumValue[0], CheckForKeywords(enumValue[1]));
					}
				}

				outStream.WriteLine("\t}");
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
			
            string nameSpace = "";
			if (isSchemaType)
			{
				if (elementFormDefault == XmlSchemaForm.Qualified || elementFormDefault == XmlSchemaForm.Unqualified) 
				{
					nameSpace = CalculateNamespace(schemaTargetNamespace, ns, false);
				}
				if (globalElementAndSchemaTypeHaveSameName)
				{
					outStream.WriteLine("\t[XmlRoot(ElementName=\"{0}\"{1},IsNullable={2}),Serializable]", 
						elementName, nameSpace, isElementNullable.ToString().ToLower());
					outStream.WriteLine("\t[XmlType(TypeName=\"{0}\"{1})]", 
						dotnetClassName, nameSpace);
				}
				else
				{
					outStream.WriteLine("\t[XmlType(TypeName=\"{0}\"{1}),Serializable]", dotnetClassName, nameSpace);
				}
			}
			else if (isLocalComplexType)
			{
				if (elementFormDefault == XmlSchemaForm.Qualified || elementFormDefault == XmlSchemaForm.Unqualified) 
				{
					nameSpace = CalculateNamespace(schemaTargetNamespace, ns, false);
				}
				outStream.WriteLine("\t[XmlType(TypeName=\"{0}\"{1}),Serializable]", 
					dotnetClassName, nameSpace);
			}
			else 
			{
				// possible root node element -- so put namespace on the element if targetNamesapce has been set
				nameSpace = CalculateNamespace(schemaTargetNamespace, ns, false);
				outStream.WriteLine("\t[XmlRoot(ElementName=\"{0}\"{1},IsNullable={2}),Serializable]", 
					elementName, nameSpace, isElementNullable.ToString().ToLower());
			}

			// Add necessary XmlInludes for abstract derived types used in the class
			foreach (string xmlIncludeClass in xmlIncludedClasses)
			{
				outStream.WriteLine("\t[XmlInclude(typeof({0}))]", CheckForKeywords(xmlIncludeClass));
			}
			
			string className = CheckForKeywords(dotnetClassName);
            outStream.Write("\tpublic {1}{2}class {0}", className, isAbstract ? "abstract " : "", partialClasses ? PartialKeyword : "");

			// setup inheritance for <xsd:extension base="class"> 
			if (complexTypeBaseClass != null && complexTypeBaseClass != "")
				outStream.Write(" : {0}", CheckForKeywords(complexTypeBaseClass));

			outStream.WriteLine();
			outStream.WriteLine("\t{");

			// setup enumerability over a contained collection
			if (enumerableClasses.ContainsKey(dotnetClassName))
			{
				ArrayList values = (ArrayList) enumerableClasses[dotnetClassName];
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
                    outStream.WriteLine(MixedDateTimeTemplate, clrType, mixedXsdType, hiddenMemberPrefix, mixedElementFieldName);
                else if (IsValueType(clrType)) 
                    outStream.WriteLine(MixedValueTypeTemplate, ConvertSystemDatatype(clrType), clrType, hiddenMemberPrefix, mixedElementFieldName);
                else
                    outStream.WriteLine(MixedObjectTemplate, "string", "string", hiddenMemberPrefix, mixedElementFieldName);
            }

			bool inherits = (complexTypeBaseClass != null && complexTypeBaseClass != "");

			// Add a class constructor
			outStream.WriteLine();
			//outStream.WriteLine("\t\t//*********************** Constructor ***********************");
			outStream.WriteLine("\t\tpublic {0}(){1}", CheckForKeywords(dotnetClassName), inherits ? " : base()" : "");
			outStream.WriteLine("\t\t{");

			// Default any DateTime fields to a value.
			for (int i=0; i<ctorList.Count; i++)
			{
				ClassConstructor ctor = (ClassConstructor) ctorList[i];
				// make sure the datetime fields are initialized to Now if a constructor default value is not set
				if (ctor.datatype == CtorDatatypeContext.DateTime && (!defaultInitialization || (defaultInitialization  && !ctor.required))) 
				{
					outStream.WriteLine("\t\t\t{1}{0} = System.DateTime.Now;", ReplaceInvalidChars(ctor.fieldName), hiddenMemberPrefix);
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
					else if (ctor.datatype == CtorDatatypeContext.Property)   // class instance field -- so build code to force the constructor to fire and create an instance
					{
						//	outStream.WriteLine("\t\t\t{0} obj{1} = {2};", CheckForKeywords(ctor.fieldName), i, CheckForKeywords(ctor.defaultValue));
					}
					else if (ctor.datatype == CtorDatatypeContext.DateTime) // standard value type with no default value
					{
						outStream.WriteLine("\t\t\t{0} = System.DateTime.Now;", ReplaceInvalidChars(ctor.fieldName));
					}
					else if (ctor.datatype == CtorDatatypeContext.ValueType) // standard value type with no default value
					{
						outStream.WriteLine("\t\t\t{1}{0}Specified = true;", ReplaceInvalidChars(ctor.fieldName), hiddenMemberPrefix);
					}
					else if (ctor.datatype == CtorDatatypeContext.ValueTypeDefault) //  valuetypes field with a default value
					{
						outStream.WriteLine("\t\t\t{0} = {1};", CheckForKeywords(ctor.fieldName), ctor.defaultValue);
					}
					else if (ctor.datatype == CtorDatatypeContext.String)
					{
						if (ctor.defaultValue == "")
							outStream.WriteLine("\t\t\t{0} = string.Empty;", CheckForKeywords(ctor.fieldName), ctor.defaultValue);
						else
							outStream.WriteLine("\t\t\t{0} = \"{1}\";", CheckForKeywords(ctor.fieldName), ctor.defaultValue);
					}
				}
			}

			outStream.WriteLine("\t\t}");

			// Add MakeSchemaCompliant code for required child classes
			if (makeSchemaCompliant)
			{
				outStream.WriteLine();
				//outStream.WriteLine("\t\t//*********************** MakeSchemaCompliant ***********************");
				outStream.WriteLine("\t\t{0}public void MakeSchemaCompliant()", (inherits) ? HideInheritedMethodKeyword : "");
				outStream.WriteLine("\t\t{");
				if (inherits) outStream.WriteLine("\t\t\tbase.MakeSchemaCompliant();");

				for (int i=0; i<ctorList.Count; i++)
				{
					ClassConstructor ctor = (ClassConstructor) ctorList[i];
					if (!ctor.required) continue;

                    // removed makeschemacompliant Add calls due to generic list
					if (ctor.datatype == CtorDatatypeContext.PropertyCollection)  
					{
                        //outStream.WriteLine("\t\t\tif ({0}{1}.Count == 0) {0}{1}.Add();", ReplaceInvalidChars(ctor.defaultValue),
                        //    collectionSuffix);
					}
					else if (ctor.datatype == CtorDatatypeContext.PropertyCollectionString)  
					{
                        //outStream.WriteLine("\t\t\tif ({0}{1}.Count == 0) {0}{1}.Add(\"\");", ReplaceInvalidChars(ctor.defaultValue),
                        //    collectionSuffix);
					}
					else if (ctor.datatype == CtorDatatypeContext.PropertyCollectionComplexType)  
					{
                        outStream.WriteLine("\t\t\tforeach ({0} _c in {1}) _c.MakeSchemaCompliant();", CheckForKeywords(ctor.fieldName),
							ReplaceInvalidChars(ctor.defaultValue));
					}
					else if (ctor.datatype == CtorDatatypeContext.PropertyCollectionAbstractComplexType)  
					{
						// note: there is no .Add() method to call for the "abstract" case -- since the code generator has 
						//  no idea what type will be added into the collection.
						outStream.WriteLine("\t\t\tforeach ({0} _c in {1}) _c.MakeSchemaCompliant();", CheckForKeywords(ctor.fieldName),
							ReplaceInvalidChars(ctor.defaultValue));
					}
					else if (ctor.datatype == CtorDatatypeContext.Property)   // class instance field -- so build code to force the constructor to fire and create an instance
					{
						outStream.WriteLine("\t\t\t{0}.MakeSchemaCompliant();", CheckForKeywords(ctor.defaultValue));
					}
				}

				outStream.WriteLine("\t\t}");
			}

			// Add DepthFirstTraversal hooks
			if (depthFirstTraversalHooks)
			{
				outStream.WriteLine();
				//outStream.WriteLine("\t\t//*********************** DepthFirstTraversal Event ***********************");
				outStream.WriteLine("\t\t{0}public static event DepthFirstTraversalDelegate DepthFirstTraversalEvent;", (inherits) ? HideInheritedMethodKeyword : "");
				outStream.WriteLine("\t\t{0}public void DepthFirstTraversal(object parent, object context)", (inherits) ? HideInheritedMethodKeyword : "");
				outStream.WriteLine("\t\t{");
				outStream.WriteLine("\t\t\tif (DepthFirstTraversalEvent != null) DepthFirstTraversalEvent(this, parent, context);");
				if (inherits) outStream.WriteLine("\t\t\tbase.DepthFirstTraversal(parent, context);");

				for (int i=0; i<ctorList.Count; i++)
				{
					ClassConstructor ctor = (ClassConstructor) ctorList[i];
					if (ctor.datatype == CtorDatatypeContext.PropertyCollectionComplexType || ctor.datatype == CtorDatatypeContext.PropertyCollectionAbstractComplexType)  
					{
						outStream.WriteLine("\t\t\tif ({0}{1} != null) foreach ({3} _d in {4}{5}) _d.DepthFirstTraversal(this, context);", hiddenMemberPrefix, ReplaceInvalidChars(ctor.defaultValue), collectionSuffix,
							CheckForKeywords(ctor.fieldName), hiddenMemberPrefix, ReplaceInvalidChars(ctor.defaultValue));
					}
					else if (ctor.datatype == CtorDatatypeContext.Property)   // class instance field -- so build code to force the constructor to fire and create an instance
					{
						outStream.WriteLine("\t\t\tif ({0}{1} != null) {0}{1}.DepthFirstTraversal(this, context);", hiddenMemberPrefix, ReplaceInvalidChars(ctor.defaultValue));
					}
				}

				outStream.WriteLine("\t\t}");
			}

			outStream.WriteLine("\t}");
		}

		public override void NamespaceTrailerCode(StreamWriter outStream, string ns)
		{
			outStream.WriteLine("}");
            outStream.WriteLine();
            outStream.WriteLine("#pragma warning restore 1591 ");
		}

		public override string ConvertSystemDatatype(string systemType)
		{
			switch (systemType)
			{
				case "System.String" : 
					return "string";
				case "System.SByte" : 
					return "sbyte";
				case "System.Byte" : 
					return "byte";
				case "System.Int16" : 
					return "short";
				case "System.UInt16" : 
					return "ushort";
				case "System.Int32" : 
					return "int";
				case "System.UInt32" : 
					return "uint";
				case "System.Int64" : 
					return "long";
				case "System.UInt64" : 
					return "ulong";
				case "System.Single" : 
					return "float";
				case "System.Double" : 
					return "double";
				case "System.Boolean" : 
					return "bool";
				case "System.Decimal" :
					return "decimal";
				case "System.Char" : 
					return "char";
				case "System.Object" : 
					return "object";
				case "System.Byte[]" :
					return "byte[]";
				case "System.DateTime" :
					return "DateTime";
				case "System.Xml.XmlQualifiedName" :
					return "System.Xml.XmlQualifiedName";
				case "System.Xml.XmlElement" :
					return "System.Xml.XmlElement";
				case "System.Xml.XmlAttribute[]" :
					return "System.Xml.XmlAttribute[]";
				default:
						// other value types, change type to string
					if (systemType.StartsWith("System.")) 
						return "string";
					else // custom enum types
						return CheckForKeywords(systemType);
			}
		}

		// todo: put this into LanguageTemplate as only one line of code differs from VB
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
				modifiedKeyword = "@" + modifiedKeyword;
			
			
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
