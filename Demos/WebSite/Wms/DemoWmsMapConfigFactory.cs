/*
 *  The attached / following is part of SharpMap.Presentation.AspNet
 *  SharpMap.Presentation.AspNet is free software © 2008 Newgrove Consultants Limited, 
 *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 * 
 */
using System.Web;
using SharpMap.Presentation.AspNet.WmsServer;

namespace SharpMap.Presentation.AspNet.Demo.Wms
{
    public class DemoWmsMapConfigFactory
        : WmsRequestConfigFactoryBase
    {
        private static readonly Capabilities.WmsServiceDescription _description
            = new Capabilities.WmsServiceDescription("Acme Corp. Map Server", "http://roadrunner.acmecorp.com/ambush")
                  {
                      Abstract =
                          @"Map Server maintained by Acme Corporation. Contact: webmaster@wmt.acme.com.
                             High-quality maps showing roadrunner nests and possible ambush locations.",
                      Keywords
                          = new[]
                                {
                                    "bird",
                                    "roadrunner",
                                    "ambush"
                                },
                      ContactInformation
                          = new Capabilities.WmsContactInformation
                                {
                                    PersonPrimary
                                        = new Capabilities.WmsContactInformation.ContactPerson
                                              {
                                                  Person = "John Doe",
                                                  Organisation = "Acme Inc"
                                              },
                                    Address
                                        = new Capabilities.WmsContactInformation.ContactAddress
                                              {
                                                  AddressType = "postal",
                                                  Country = "Neverland"
                                              },
                                    VoiceTelephone
                                        = "1-800-WE DO MAPS"
                                },
                      MaxWidth = 1000,
                      MaxHeight = 500
                  };


        public override Capabilities.WmsServiceDescription Description
        {
            get { return _description; }
        }

        public override string CreateCacheKey(HttpContext context)
        {
            return "MyWmsServer";
        }
    }
}