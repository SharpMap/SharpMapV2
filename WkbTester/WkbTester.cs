// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Collections.Generic;
using System.IO;
using SharpMap.Converters.WellKnownBinary;
using SharpMap.Geometries;
using SharpMap.Data.Providers;

namespace WkbTester
{
    public class WkbTester
    {
        enum ConversionType
        {
            Unknown = 0,
            ShapeToWkb,
            WkbToShape
        }

        class CommandLineParseException : Exception
        {
            public CommandLineParseException(string message)
                : base(message) { }
        }

        [STAThread]
        static void Main(string[] args)
        {
            ConversionType conversionType;
            string shapefilePath, wkbPath;

            try
            {
                parseCommandLine(args, out conversionType, out shapefilePath, out wkbPath);
            }
            catch(CommandLineParseException ex)
            {
                Usage(ex.Message);
#if DEBUG
                Console.ReadKey();
#endif
                return;
            }

            FileStream wkbStream;
            switch (conversionType)
            {
                case ConversionType.Unknown:
                    goto default;
                case ConversionType.ShapeToWkb:
                    if(!File.Exists(shapefilePath))
                        throw new FileNotFoundException("Cannot find the specified shapefile", shapefilePath);
                    using (wkbStream = File.Open(wkbPath, FileMode.OpenOrCreate))
                    {
                        ConvertShapeFile(shapefilePath, wkbStream);
                        wkbStream.Flush();
                    }
                    break;
                case ConversionType.WkbToShape:
                    if (!File.Exists(wkbPath))
                        throw new FileNotFoundException("Cannot find the specified WKB file", wkbPath);
                    using (wkbStream = File.Open(wkbPath, FileMode.Open))
                    {
                        ConvertWkb(shapefilePath, wkbStream);
                        wkbStream.Flush();
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown conversion type: internal program error");
            }
        }

        private static void parseCommandLine(string[] args, out ConversionType conversionType, out string shapefilePath, out string wkbPath)
        {
            shapefilePath = wkbPath = null;

            if(args == null || args.Length < 3) 
                throw new CommandLineParseException("Incorrect number of arguments");

            if(String.IsNullOrEmpty(args[0]) || (args[0].ToLower() != "-convert" && args[0].ToLower() != "/convert" && args[0].ToLower() != "-c" && args[0].ToLower() != "/c"))
                throw new CommandLineParseException("'convert' switch missing");

            if (String.IsNullOrEmpty(args[1]) || (args[1].ToLower() != "shp2wkb" && args[1].ToLower() != "wkb2shp"))
                throw new CommandLineParseException("Unknown convert value; must be 'shp2wkb' or 'wkb2shp'");
            else
                conversionType = args[1].ToLower() == "shp2wkb" ? ConversionType.ShapeToWkb : ConversionType.WkbToShape;

            string pathName, fileName;
            getPathAndFileName(args[2], out pathName, out fileName);

            if (String.IsNullOrEmpty(fileName))
                throw new CommandLineParseException(String.Format("Filename is missing in provided path: {0}", args[2]));

            switch (conversionType)
	        {
		        case ConversionType.Unknown:
                    goto default;
                case ConversionType.ShapeToWkb:
                    if(Path.GetExtension(fileName).ToLower() != ".shp")
                        throw new CommandLineParseException("\nShapefile must have '.shp' extension");

                    shapefilePath = Path.Combine(pathName, fileName);
                    break;
                case ConversionType.WkbToShape:
                    wkbPath = Path.Combine(pathName, fileName);
                    break;
                default:
                    throw new InvalidOperationException("Unknown conversion type: internal program error.");
	        }

            // Handle optional second filename
            if(args.Length == 4)
            {
                getPathAndFileName(args[3], out pathName, out fileName);
                    
                if (String.IsNullOrEmpty(fileName))
                    throw new CommandLineParseException(String.Format("Filename is missing in provided path: {0}", args[3]));

                switch (conversionType)
                {
                    case ConversionType.Unknown:
                        goto default;
                    case ConversionType.ShapeToWkb:
                        wkbPath = Path.Combine(pathName, fileName);
                        break;
                    case ConversionType.WkbToShape:
                        if (Path.GetExtension(fileName).ToLower() != ".shp")
                            throw new CommandLineParseException("\nShapefile must have '.shp' extension");

                        shapefilePath = Path.Combine(pathName, fileName);
                        break;
                    default:
                        throw new InvalidOperationException("Unknown conversion type: internal program error.");
                }
            }
            else
            {
                switch (conversionType)
                {
                    case ConversionType.Unknown:
                        goto default;
                    case ConversionType.ShapeToWkb:
                        wkbPath = Path.Combine(pathName, Path.GetFileNameWithoutExtension(fileName) + ".wkb");
                        break;
                    case ConversionType.WkbToShape:
                        shapefilePath = Path.Combine(pathName, Path.GetFileNameWithoutExtension(fileName) + ".shp");
                        break;
                    default:
                        throw new InvalidOperationException("Unknown conversion type: internal program error.");
                } 
            }
        }

        private static void getPathAndFileName(string commandline, out string pathName, out string fileName)
        {
            try
            {
                pathName = Path.GetDirectoryName(commandline);
            }
            catch (ArgumentException)
            {
                throw new CommandLineParseException(String.Format("\nPath contains invalid characters", commandline));
            }
            catch (PathTooLongException)
            {
                throw new CommandLineParseException(String.Format("\nPath too long", commandline));
            }

            try
            {
                fileName = Path.GetFileName(commandline);
            }
            catch (ArgumentException)
            {
                throw new CommandLineParseException(String.Format("\nFile name {0} contains invalid characters", commandline));
            }
        }

        private static void Usage(string message)
        {
            Console.WriteLine(
@"

{1}

Usage:  {0} -convert [shp2wkb|wkb2shp] [shapefile] [wkbfile]

    Where:  
        wkb2shp specifies conversion from well-known binary to shapefile
        shp2wkb specifies conversion from shapefile to well-known binary
        [shapefile] is the path to the target shapefile. 
                    If not specified, takes the name of the wkb file.
        [wkbfile] is the path to the target wkbfile. 
                    If not specified, takes the name of the shapefile.
", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, message);
        }

        public static void ConvertShapeFile(string shapefilePath, Stream wkbStream)
        {
            ShapeFile shapefile = new ShapeFile(shapefilePath);
            ConvertShapeFile(shapefile, wkbStream);
        }

        public static void ConvertShapeFile(ShapeFile shapefile, Stream wkbStream)
        {
            shapefile.Open();
            BoundingBox bbox = shapefile.GetExtents();
            List<Geometry> allShapes = shapefile.GetGeometriesInView(bbox);
            foreach(Geometry shape in allShapes)
            {
                byte[] wkb = shape.AsBinary();
                wkbStream.Write(wkb, 0, wkb.Length);
            }

            shapefile.Close();
        }

        public static void ConvertWkb(string shapefilePath, Stream wkbStream)
        {
            ShapeFile shapefile;
            if (!File.Exists(shapefilePath))
            {
                shapefile = ShapeFile.Create(Path.GetDirectoryName(shapefilePath), Path.GetFileNameWithoutExtension(shapefilePath), ShapeType.Polygon, null);
            }
            else
                shapefile = new ShapeFile(shapefilePath);

            ConvertWkb(shapefile, wkbStream);
        }

        public static void ConvertWkb(ShapeFile shapefile, Stream wkbStream)
        {
            BinaryReader reader = new BinaryReader(wkbStream);
            SharpMap.Data.FeatureDataTable features = new SharpMap.Data.FeatureDataTable();
            wkbStream.Position = 0;
            while (wkbStream.Position < wkbStream.Length)
            {
                SharpMap.Data.FeatureDataRow feature = features.NewRow();
                feature.Geometry = SharpMap.Converters.WellKnownBinary.GeometryFromWKB.Parse(reader);
                features.AddRow(feature);
            }

            shapefile.Open();
            shapefile.Save(features);
            shapefile.Close();
        }
    }
}
