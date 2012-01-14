OpenGIS(r) O&M 1.0.0 - ReadMe.txt

GML Application Schema version of the O&M 1.0.0 model. (OGC 07-022r1)

The general models and XML encodings for observations and measurements,
including but not restricted to those using sensors.  The Observations and
Measurements schema are defined in the OGC document 07-022r1.

2008-02-07  Simon Cox

  * extensions/observationSpecialization_constraint.xsd: fix namespace
    for swe in sch namespace prefix binding
  * extensions/om_extended.xsd: fix om namespace (unused)
  * see attached unified diff

2007-10-05  Simon Cox

  * Published om/1.0.0 schemas from 07-022r1

-----------------------------------------------------------------------

Policies, Procedures, Terms, and Conditions of OGC(r) are available
  http://www.opengeospatial.org/ogc/legal/ .

Copyright (c) 2007-2008 Open Geospatial Consortium, Inc. All Rights Reserved.

-----------------------------------------------------------------------

# 2008-02-07 unified diff fix
Index: extensions/om_extended.xsd
===================================================================
--- extensions/om_extended.xsd	(revision 3013)
+++ extensions/om_extended.xsd	(working copy)
@@ -1,5 +1,5 @@
 <?xml version="1.0" encoding="UTF-8"?>
-<schema xmlns="http://www.w3.org/2001/XMLSchema" xmlns:om="http://www.opengis.net/om/1.0.1" targetNamespace="http://www.opengis.net/om/1.0" elementFormDefault="qualified" attributeFormDefault="unqualified" version="1.0.0">
+<schema xmlns="http://www.w3.org/2001/XMLSchema" xmlns:om="http://www.opengis.net/om/1.0" targetNamespace="http://www.opengis.net/om/1.0" elementFormDefault="qualified" attributeFormDefault="unqualified" version="1.0.0">
 	<annotation>
 		<documentation>om.xsd
 
Index: extensions/observationSpecialization_constraint.xsd
===================================================================
--- extensions/observationSpecialization_constraint.xsd	(revision 3013)
+++ extensions/observationSpecialization_constraint.xsd	(working copy)
@@ -11,7 +11,7 @@
 			<sch:title>Schematron validation</sch:title>
 			<sch:ns prefix="gml" uri="http://www.opengis.net/gml"/>
 			<sch:ns prefix="om" uri="http://www.opengis.net/om/1.0"/>
-			<sch:ns prefix="swe" uri="http://www.opengis.net/swe/1.0"/>
+			<sch:ns prefix="swe" uri="http://www.opengis.net/swe/1.0.1"/>
 			<sch:ns prefix="xlink" uri="http://www.w3.org/1999/xlink"/>
 			<sch:ns prefix="xs" uri="http://www.w3.org/2001/XMLSchema"/>
 			<sch:ns prefix="xsi" uri="http://www.w3.org/2001/XMLSchema-instance"/>

