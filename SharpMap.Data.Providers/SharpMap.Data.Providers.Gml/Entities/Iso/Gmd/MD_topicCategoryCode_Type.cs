// /*
//  *  The attached / following is part of SharpMap.Data.Providers.Gml
//  *  SharpMap.Data.Providers.Gml is free software © 2008 Newgrove Consultants Limited, 
//  *  www.newgrove.com; you can redistribute it and/or modify it under the terms 
//  *  of the current GNU Lesser General Public License (LGPL) as published by and 
//  *  available from the Free Software Foundation, Inc., 
//  *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
//  *  This program is distributed without any warranty; 
//  *  without even the implied warranty of merchantability or fitness for purpose.  
//  *  See the GNU Lesser General Public License for the full details. 
//  *  
//  *  Author: John Diss 2009
//  * 
//  */
using System;
using System.Xml.Serialization;

namespace SharpMap.Entities.Iso.Gmd
{
    [Serializable]
    public enum MD_topicCategoryCode_Type
    {
        [XmlEnum(Name = "biota")] Biota = 1,
        [XmlEnum(Name = "boundaries")] Boundaries = 2,
        [XmlEnum(Name = "climatologyMeteorologyAtmosphere")] ClimatologyMeteorologyAtmosphere = 3,
        [XmlEnum(Name = "economy")] Economy = 4,
        [XmlEnum(Name = "elevation")] Elevation = 5,
        [XmlEnum(Name = "environment")] Environment = 6,
        [XmlEnum(Name = "farming")] Farming = 0,
        [XmlEnum(Name = "geoscientificInformation")] GeoscientificInformation = 7,
        [XmlEnum(Name = "health")] Health = 8,
        [XmlEnum(Name = "imageryBaseMapsEarthCover")] ImageryBaseMapsEarthCover = 9,
        [XmlEnum(Name = "inlandWaters")] InlandWaters = 11,
        [XmlEnum(Name = "intelligenceMilitary")] IntelligenceMilitary = 10,
        [XmlEnum(Name = "location")] Location = 12,
        [XmlEnum(Name = "oceans")] Oceans = 13,
        [XmlEnum(Name = "planningCadastre")] PlanningCadastre = 14,
        [XmlEnum(Name = "society")] Society = 15,
        [XmlEnum(Name = "structure")] Structure = 0x10,
        [XmlEnum(Name = "transportation")] Transportation = 0x11,
        [XmlEnum(Name = "utilitiesCommunication")] UtilitiesCommunication = 0x12
    }
}