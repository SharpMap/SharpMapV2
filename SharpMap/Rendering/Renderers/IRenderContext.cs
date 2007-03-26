using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Rendering
{
    public interface IRenderContext
    {
        void Add<TContextItem>(TContextItem item);
        void Add<TContextItem>(TContextItem item, string id);
        TContextItem Get<TContextItem>();
        TContextItem Get<TContextItem>(string id);
    }
}
