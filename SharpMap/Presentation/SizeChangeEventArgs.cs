using System;
using IVectorD = NPack.Interfaces.IVector<NPack.DoubleComponent>;

namespace SharpMap.Presentation
{
    public class SizeChangeEventArgs<TSize> : EventArgs
        where TSize : IVectorD
    {
        private readonly TSize _size;

        public SizeChangeEventArgs(TSize size)
        {
            _size = size;
        }

        public TSize Size
        {
            get { return _size; }
        }
    }
}
