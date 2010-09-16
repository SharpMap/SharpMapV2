using System;

namespace SharpMap.Data.Providers
{
    public class OgrProviderException : Exception
    {
        public OgrProviderException()
        {
            
        }

        public OgrProviderException(String message)
            : base(message)
        {
        }
    }
}