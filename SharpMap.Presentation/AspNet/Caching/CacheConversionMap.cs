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
using System;
using System.Collections.Generic;
using System.IO;

namespace SharpMap.Presentation.AspNet.Caching
{
    internal class CacheConversionMap : ICacheConversionMap
    {
        private readonly Dictionary<Type, object> _convertToObjectMap = new Dictionary<Type, object>();
        private readonly Dictionary<Type, object> _convertToStreamMap = new Dictionary<Type, object>();

        #region ICacheConversionMap Members

        public void RegisterStreamConverter<T>(Func<T, Stream> converter)
        {
            _convertToStreamMap.Add(typeof (T), converter);
        }

        public void RegisterObjectConverter<T>(Func<Stream, T> converter)
        {
            _convertToObjectMap.Add(typeof (T), converter);
        }

        public Func<T, Stream> GetStreamConverter<T>()
        {
            return (Func<T, Stream>) _convertToStreamMap[typeof (T)];
        }

        public Func<Stream, T> GetObjectConverter<T>()
        {
            return (Func<Stream, T>) _convertToObjectMap[typeof (T)];
        }

        #endregion
    }
}