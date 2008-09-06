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

namespace SharpMap.Presentation.AspNet
{
    public abstract class XmlFormatableExceptionBase : Exception
    {
        protected string _message;

        protected XmlFormatableExceptionBase() : this("")
        {
        }

        protected XmlFormatableExceptionBase(string message)
            : base(message)
        {
            _message = message;
        }

        protected XmlFormatableExceptionBase(string message, Exception inner)
            : base(message, inner)
        {
            _message = message;
        }

        public abstract string XmlExceptionString { get; }
    }

    public class XmlFormatableExceptionWrapper
        : XmlFormatableExceptionBase
    {
        public XmlFormatableExceptionWrapper(Exception ex)
            : this("An Error Occured. ", ex)
        {
        }

        public XmlFormatableExceptionWrapper(string message, Exception ex)
            : base(message, ex)
        {
        }

        public XmlFormatableExceptionWrapper(string message)
            : base(message)
        {
        }

        public override string XmlExceptionString
        {
            get { return string.Format("<Exception>{0}</Exception>", _message); }
        }
    }
}