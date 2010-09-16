GdalForSharpMap
===============

Installation instructions
-------------------------

1. Prerequisites
In order to use this library you need to provide native gdal 
binaries along with their csharp bindings. To achive this you 
have mainly three choices:

- Compile GDAL and its C# bindings yourself.
  You are on your own. As far as I'm concerned this is not a
  trivial task.

- Download a recent FWTools package from http://fwtools.maptools.org
  and install it on your machine.

- Use the precompiled packages provided by Tamas Szerekes
  available at: http://vbkto.dyndns.org/sdk/. Especially if you 
  target x64 platform this is the way to go. 
  I chose 
  - MSVC2008 (Win32) -stable and 
  - MSVC2008 (Win64) -stable
  Extract the zip-file to a location of your choice.

2. Project settings
Update the references for gdal_csharp and gdalconst_csharp in the 
project file, e.g. do this using a text editor and update the paths
according to your needs.

Then compile GdalForSharpMap.

If you succeed, add reference for GdalForSharpMap to your project and
add a app|web.config file containing to following entries:
%<--
<add key="GdalBinPath" value="D:\Gdal\Native\x86;D:\Gdal\Native\x86\gdal\csharp;D:\Gdal\Native\x86\gdal\apps" />
<add key="GdalDataPath" value="D:\Gdal\Native\x86\gdal-data" />
<add key="GdalDriverPath" value="D:\Gdal\Native\x86\gdal\plugins;" />
<add key="ProjLibPath" value="D:\Gdal\Native\x86\proj\SHARE" />
-->%
Change the values according to your needs.

02/2010
FObermaier
