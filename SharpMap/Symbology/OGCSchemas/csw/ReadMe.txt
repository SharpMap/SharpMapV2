OpenGIS Catalog ReadMe.txt

2007-08-16  Uwe Voges

  * v2.0.2: added ISO Metadata Application Profile (1.0.0) for Catalogue
    Services Specification 2.0.2 (OGC 07-045)

2007-07-27  Peter Vretanos

  * v2.0.1: rename CSW_2.0.1_changes.txt 2.0.1/CSW_2.0.1_changes.txt
  * v2.0.2: added ChangeLog.txt
  * v2.0.2/CSW-publication.xsd: import owsAll.xsd, rather than
    owsGetCapabilities.xsd, for the http://www.opengis.net/ows namespace
    to prevent validation errors caused by namespace caching in a number
    of XML parsers (Xerces,XSV)
  * v2.0.2/record.xsd: import owsAll.xsd, rather than owsCommon.xsd, for the
    http://www.opengis.net/ows namespace to prevent validation errors caused by
    namespace caching in a number of XML parsers (Xerces,XSV)

2007-04-30  Doug Nebert, Arliss Whiteside, Peter Vretanos

  * v2.0.2: See OGC 07-006r1 for associated specification and OGC 07-010
    for revision notes.
  * v2.0.2: validated using XML Spy 2007 - pVretanos

2005-11-22  Arliss Whiteside

  * v2.0.1, v2.0.0: The sets of XML Schema Documents for OpenGIS Catalog
  Versions 2.0.0 and 2.0.1 have been edited to reflect the corrigendum
  to document OGC 04-021r2 that is based on the change requests: 
  OGC 05-068r1 "Store xlinks.xsd file at a fixed location"
  OGC 05-081r2 "Change to use relative paths"
 
  * Note: check each OGC numbered document for detailed changes.

2005-11-02  Doug Nebert, Arliss Whiteside, Peter Vretanos

  * v2.0.1: See http://schemas.opengis.net/csw/2.0.1/CSW_2.0.1_changes.txt

