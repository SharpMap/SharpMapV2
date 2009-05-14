using System;
using System.Collections;
using XSDObjectGenLib;
using System.Collections.Generic;
using System.Xml;

/// .NET command line utility.  Same functionality as AddInWizard.cs (VS.NET add-in) and 
/// ProjectWizard.cs (VS.NET project wizard).

namespace XSDObjectGen
{
	class XSDGenCommand
	{
		static private string language = "cs";  // default;
		static private string genNamespace = "";
		static private string fileName = "";
		static private string xsdFile = "";
        static private string namespaceList = "";
        static private string filenameList = "";
        static private string[] elementArray = null;
        static private string elementFile = "";
        static private string acordLookupCodes = "";
        static private string acordLookupCodesPrivate = "";
		static private bool constructRequiredSchema = false;
		static private bool depthFirstTraversalHooks = false;
		static private bool defaultInitialization = false;
        static private bool partialKeyword = false;
		static private Hashtable namespaceTable = null;
        static private Hashtable filenameTable = null;
		static private Hashtable namespaceTableClone = null;
        
        // gather namespace data
		static void GatherNamespaceInput()
		{
            namespaceTableClone = (Hashtable)namespaceTable.Clone();

            if (namespaceList != "")
            {
                // list was passed in at command line -- so use that rather than asking for further input

                string[] nsArray = namespaceList.Split("|".ToCharArray());
                string[] fsArray = filenameList.Split("|".ToCharArray());

                if (filenameList != "")
                    filenameTable = new Hashtable();

                if ((nsArray.Length != namespaceTable.Count) || (filenameList != "" && (fsArray.Length != namespaceTable.Count)))
                    throw new XSDObjectGenException("/y and/or /z list of namespaces and files does not match the number of schemas used and imported.");
                int i = 0;
                foreach (string ns in namespaceTable.Keys)
                {
                    if (ns == "") continue; // null targetNamespace
                    Console.WriteLine("Xsd namespace = {0}. .NET namespace: {1}: ", ns, nsArray[i]);
                    namespaceTableClone[ns] = nsArray[i];
                    if (filenameList != "" && fsArray != null && fsArray.Length > 0)
                        filenameTable.Add(ns, fsArray[i]);
                    i++;
                }
            }
            else
            {       
                Console.WriteLine("Imported namespaces were found.  Please enter valid .NET namespace names for each namespace.");
                Console.WriteLine("WARNING. Namespaces chosen must not conflict with types and element names from the schemas. ");
                foreach (string ns in namespaceTable.Keys)
                {
                    if (ns == "") continue; // null targetNamespace
                    Console.Write("Xsd namespace = {0}. Please enter a CLR namespace name for this namespace: ", ns);
                    namespaceTableClone[ns] = Console.ReadLine();
                }
            }
		}
		
		static void Main(string[] args)
		{
			try
			{
				if (args.GetLength(0) == 0)
				{
					OutputInstructions();
					return;
				}

				ParseCommandParameters(args);
				XSDSchemaParser generator = new XSDSchemaParser();
				string [] result;
				namespaceTable = null;

                List<string> optionEElements = null;
                if (elementArray != null)
                {
                    optionEElements = new List<string>();
                    foreach (string e in elementArray) optionEElements.Add(e);
                }
                else if (elementFile != "")
                {
                    optionEElements = new List<string>();
                    XmlDocument doc = new XmlDocument();
                    doc.Load(elementFile);
                    foreach (XmlNode node in doc.DocumentElement.FirstChild.ChildNodes)
                    {
                        if (node.InnerText != "") optionEElements.Add(node.InnerText);
                    }
                }

				if (language.ToLower() == "vb")
				{
					result = generator.Execute(xsdFile, Language.VB, genNamespace, fileName, null, constructRequiredSchema, depthFirstTraversalHooks,
                        defaultInitialization, ref namespaceTable, filenameTable, partialKeyword, optionEElements, acordLookupCodes, acordLookupCodesPrivate);
					if (result == null && namespaceTable != null)
					{
						GatherNamespaceInput();
						generator = null;
						generator = new XSDSchemaParser();
						result = generator.Execute(xsdFile, Language.VB, genNamespace, fileName, null, constructRequiredSchema, depthFirstTraversalHooks,
                            defaultInitialization, ref namespaceTableClone, filenameTable, partialKeyword, optionEElements, acordLookupCodes, acordLookupCodesPrivate);
					}
				}
				else if (language.ToLower() == "cs")
				{
					result = generator.Execute(xsdFile, Language.CS, genNamespace, fileName, null, constructRequiredSchema, depthFirstTraversalHooks,
                        defaultInitialization, ref namespaceTable, filenameTable, partialKeyword, optionEElements, acordLookupCodes, acordLookupCodesPrivate);
					if (result == null && namespaceTable != null)
					{
						GatherNamespaceInput();
						generator = null;
						generator = new XSDSchemaParser();
						result = generator.Execute(xsdFile, Language.CS, genNamespace, fileName, null, constructRequiredSchema, depthFirstTraversalHooks,
                            defaultInitialization, ref namespaceTableClone, filenameTable, partialKeyword, optionEElements, acordLookupCodes, acordLookupCodesPrivate);
					}
				}
				else
				{
					throw new XSDObjectGenException(string.Format("Language {0} not supported.", language));
				}	

				Console.WriteLine("Done. Success");  
				foreach (string f in result)
				{
					Console.WriteLine(" Writing file {0}.", f);
				}
			}
			catch (XSDObjectGenException e)
			{
				// expected possible exceptions
                Console.WriteLine("Application handled error.  Could indicate an issue with the schema");
				Console.WriteLine(e.Message);
			}
			catch(Exception e)
			{
				if (e.InnerException != null) e = e.InnerException;
			//	Console.WriteLine("Unexpected error occured in XSDObjectGen : {0}\nSource : {1}\nStack : {2}", e.Message, e.Source, e.StackTrace);
				Console.WriteLine("Unexpected error occured in XSDObjectGen : {0}", e.Message);
                Console.WriteLine("Please send your schema to support.");
			}
			finally
			{		
			}
		}

		/// <summary>
		/// Command line parameter instructions to be sent to console.
		/// </summary>
		private static void OutputInstructions()
		{
			Console.WriteLine();
			Console.WriteLine("XSDObjectGen: Utility to generate C# or VB.NET classes from an XSD schema.");
			Console.WriteLine("Version {0}", typeof(XSDGenCommand).Assembly.GetName(false).Version);
			Console.WriteLine();
			Console.WriteLine("XSDObjectGen.exe <schema>.xsd [/l:] [/n:] [/f] [/c] [/d] [/p] [/t] [/z] [/e]");
			Console.WriteLine();
			Console.WriteLine("/n:<namespace>");
			Console.WriteLine("\tThe .NET namespace to include the generated types.  This is optional");
			Console.WriteLine("\tfor VB. The generated file will be namespace.cs or namspace.vb.");
			Console.WriteLine();
			Console.WriteLine("/l:<language>");
			Console.WriteLine("\tThe language to be generated. Optional");
			Console.WriteLine("\t\"cs\" for CSharp (C#) -- the default if not provided");
			Console.WriteLine("\t\"vb\" for VB.NET");
			Console.WriteLine();
			Console.WriteLine("/f:<fileName>");
			Console.WriteLine("\tThe output file name generated.  Optional, except for VB if namespace");
			Console.WriteLine("\tis omitted. By by leaving this off, the generated source file will be");
			Console.WriteLine("\tthe namespace name.");
			Console.WriteLine();
			Console.WriteLine("/c");
			Console.WriteLine("\tAdd a MakeSchemaCompliant() function to each class that will construct"); 
			Console.WriteLine("\tand set all schema required nodes and child nodes.  Optional.");
			Console.WriteLine();
			Console.WriteLine("/d");
			Console.WriteLine("\tAdd code to class constructors to initialize fields for schema"); 
			Console.WriteLine("\tdefault values and required elements/attributes. By leaving this off,");
			Console.WriteLine("\tschema default values are not automatically set.  Optional.");
			Console.WriteLine();
            Console.WriteLine("/p");
            Console.WriteLine("\tPut the \"partial\" keyword on every class. .NET 2.0 only. Optional.");
            Console.WriteLine();
			Console.WriteLine("/t");
			Console.WriteLine("\tCreate depth-first traversal hooks to add custom code extensions."); 
			Console.WriteLine("\tHooks are used to plug in custom code executed during a traversal."); 
			Console.WriteLine("\tBy leaving this off, traversal hooks will not be added.  Optional.");
            Console.WriteLine();
            Console.WriteLine("/e");
            Console.WriteLine("\tList of case sensitive elementNames separated by a '|' char.");
            Console.WriteLine("\tAlternatively, provide an xml document with the element names.");
            Console.WriteLine("\tDocGen will only build classes from these elements and whatever");
            Console.WriteLine("\ttypes these elements reference.  Use this only in the case of very");
            Console.WriteLine("\tlarge schemas where minimizing the generated code is desired.");
            Console.WriteLine("\tCould be dangerous when XSD Imports is used within the schema.");
            Console.WriteLine("\tOptional.");
            Console.WriteLine();
            Console.WriteLine("/y");
            Console.WriteLine("\tList of .NET namespaces mapping to XSD namesapces for XSD Import.");
            Console.WriteLine("\tThis automates the generator from asking for the additional namespaces");
            Console.WriteLine("\tthat map to imported schemas.  The order of strings must match the.");
            Console.WriteLine("\tthe answers to the questions that generator would otherwise ask.");
            Console.WriteLine("\tIf this included, then the generator will use this list and not");
            Console.WriteLine("\task for any further input.  This allows the generator to be ");
            Console.WriteLine("\tscripted.  Format: /y:\"namespace_string|namespace_string\"");
            Console.WriteLine("\tand separated by a '|' character.  Optional.");
			Console.WriteLine();
            Console.WriteLine("/z");
            Console.WriteLine("\tList of filenames separated by a '|' char that matches the /y list.");
            Console.WriteLine("\tof .NET namespaces.  This is only needed if /y option is used, and");
            Console.WriteLine("\tit is desired to specifically  choose file names rather than letting");
            Console.WriteLine("\tthe generator pick the names.  Format is the same as /y.  Optional.");
			Console.WriteLine();
            Console.WriteLine("Note on /y and /z");
            Console.WriteLine("\tThe /y and /z options override the /n and /f options respectively.");
            Console.WriteLine("\t/n and /f are for single (non-import) schema scenarios.");
            Console.WriteLine("\tBut /n is still required (but aren't used) with /y.");
            Console.WriteLine("\t/y and /z string list must have the same number of parameters and");
            Console.WriteLine("\tthe order matters (it must match the order that xsdobjectgen asks");
            Console.WriteLine("\tfor the .NET namespaces if /y is omitted.");
			Console.WriteLine();
            Console.WriteLine("/e");
            Console.WriteLine("\tList of public elements separated by a '|' char (case sensitive).");
            Console.WriteLine("\tCode will only be generated for these elements, and everything");
            Console.WriteLine("\treferenced by these element. Essentially schema slicing. Optional.");
            Console.WriteLine();
            Console.WriteLine("/a");
            Console.WriteLine("\tPath to a file containing public ACORD lookup codes. Optional.");
            Console.WriteLine();
            Console.WriteLine("/v");
            Console.WriteLine("\tPath to a file containing private ACORD lookup codes. Optional.");
            Console.WriteLine();
            Console.WriteLine("Example");
            Console.WriteLine("\tXSDObjectGen.exe B.xsd /l:cs /f:BSchema.cs /n:\"UnitTests.XmlInclude.BSchema\" /p /y:\"UnitTests.XmlInclude.ASchema|UnitTests.XmlInclude.BSchema\" /z:\"ASchema|BSchema\" /e:\"Element1|Element2\" /a:\"C:\\temp\\LookupTypesPublic.xml\"");
            Console.WriteLine("");
            
		}

		/// <summary>
		/// Gather the command line parameters passed into code generator.
		/// </summary>
		/// <param name="args"></param>
		private static void ParseCommandParameters(string[] args)
		{
			string param;
			string val;

			xsdFile = args[0];
			if (xsdFile == "" || (xsdFile[0] == '/' && xsdFile[2] == ':'))
			{
				throw new XSDObjectGenException("Invalid Schema File: " + xsdFile);
			}

			for (int i=1; i<args.GetLength(0); i++)
			{
				if (args[i].Length >= 3)
				{
					param = args[i].Substring(0, 3);
					val = args[i].Substring(3);
				}
				else
				{
					param = args[i].Substring(0, 2);
					val = "";
				}

				switch (param)
				{
					case "/l:" : 
						language = val.Trim(); 
						break;
					case "/n:" : 
						genNamespace = val.Trim(); 
						break;
					case "/f:" : 
						fileName = val.Trim(); 
						break;
					case "/c" : 
						constructRequiredSchema = true; 
						break;
					case "/t" : 
						depthFirstTraversalHooks = true; 
						break;
					case "/d" :
						defaultInitialization = true;
						break;
                    case "/p":
                        partialKeyword = true;
                        break;
                    case "/y:":
                        namespaceList = val.Trim();
                        break;
                    case "/z:":
                        filenameList = val.Trim();
                        break;
                    case "/e:":
                        string elementList = val.Trim();
                        if (elementList.EndsWith(".xml"))
                            elementFile = elementList;
                        else
                            elementArray = elementList.Split("|".ToCharArray());
                        break;
                    case "/a:":
                        acordLookupCodes = val.Trim();
                        break;
                    case "/v:":
                        acordLookupCodesPrivate = val.Trim();
                        break;
					default:
                        throw new XSDObjectGenException("Invalid Command Parameter " + param + val);
				}
			}

			if (language.ToLower() == "cs" && genNamespace == "")
				throw new XSDObjectGenException("Error: Namespace /n is a required parameter for CSharp.");
			if (language.ToLower() == "vb"  && genNamespace == "" && fileName == "")
				throw new XSDObjectGenException("Error: Either Namespace /n or Filename /f is required for VB.");
		}
	}
}
