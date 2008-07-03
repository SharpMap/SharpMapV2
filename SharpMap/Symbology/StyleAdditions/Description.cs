using System;

namespace SharpMap.Symbology
{
    public class Description
    {
        private String _title;
        private String _abstract;

        public Description() : this(null, null) {}

        public Description(String title) : this(title, null) {}

        public Description(String title, String descriptionAbstract)
        {
            _title = title;
            _abstract = descriptionAbstract;
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Abstract
        {
            get { return _abstract; }
            set { _abstract = value; }
        }
    }
}
