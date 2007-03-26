using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Styles
{
    [Serializable]
    public sealed class FontFamily
    {
        private string _familyName;

        public FontFamily(string name)
        {
            _familyName = name;
        }

        public string Name
        {
            get { return _familyName; }
            set { _familyName = value; }
        }
    }
}
