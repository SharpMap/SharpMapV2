/*
 *	This file is part of SharpMap.Demo.FormatConverter
 *  SharpMap.Demo.FormatConverter is free software © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
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
using System;

namespace SharpMap.Demo.FormatConverter.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigureProviderAttribute : Attribute
    {
        public ConfigureProviderAttribute(Type providerType, string name)
        {
            ProviderType = providerType;
            Name = name;
        }

        public Type ProviderType { get; protected set; }
        public string Name { get; protected set; }
    }
}