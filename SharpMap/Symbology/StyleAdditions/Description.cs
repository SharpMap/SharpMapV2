using System;

namespace SharpMap.Symbology.StyleAdditions
{
    public class Description
    {
        public Description() : this(null, null) {}

        public Description(String title) : this(title, null) {}

        public Description(String title, String descriptionAbstract)
        {
            Title = title;
            Abstract = descriptionAbstract;
        }

        public string Title { get; set; }

        public string Abstract { get; set; }
    }
}
