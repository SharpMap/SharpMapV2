using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SharpMap.Symbology
{
    [Serializable]
    [XmlInclude(typeof(FeatureStyle))]
    [XmlInclude(typeof(CoverageStyle))]
    public class Style
    {
        private String[] _semanticTypeIdentifier;
        private String _name;
        private Description _description;
        private Object[] _items;
        private OgcSymbologyEncodingVersion _version;
        private Boolean _versionSpecified;

        /// <summary>
        /// Gets or sets the name of this <see cref="Style"/>.
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="Symbology.Description"/> of this <see cref="Style"/>.
        /// </summary>
        public Description Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Gets the set of <see cref="Rule"/>s contained by this <see cref="Style"/>.
        /// </summary>
        public IEnumerable<Rule> Rules
        {
            get
            {
                if (Items == null)
                {
                    yield break;
                }

                foreach (Rule item in Items)
                {
                    if (item != null)
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the set of <see cref="OnlineResource"/>s contained by this <see cref="Style"/>.
        /// </summary>
        public IEnumerable<OnlineResource> OnlineResources
        {
            get
            {
                if (Items == null)
                {
                    yield break;
                }

                foreach (OnlineResource item in Items)
                {
                    if (item != null)
                    {
                        yield return item;
                    }
                }
            }
        }

        /// <summary>
        /// Used by the XML serializer.
        /// </summary>
        [XmlElement("OnlineResource", typeof(OnlineResource))]
        [XmlElement("Rule", typeof(Rule))]
        public Object[] Items
        {
            get { return _items; }
            set { _items = value; }
        }

        /// <summary>
        /// Gets or sets a <see cref="String"/> array that identifies 
        /// the more general "type" of geometry that this style is 
        /// meant to act upon.
        /// </summary>
        [XmlElement("SemanticTypeIdentifier")]
        public String[] SemanticTypeIdentifier
        {
            get { return _semanticTypeIdentifier; }
            set { _semanticTypeIdentifier = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="OgcSymbologyEncodingVersion"/> of this <see cref="Style"/>.
        /// </summary>
        [XmlAttribute(AttributeName = "version")]
        public OgcSymbologyEncodingVersion Version
        {
            get { return _version; }
            set { _version = value; }
        }

        /// <summary>
        /// Used by the XML serializer.
        /// </summary>
        [XmlIgnore]
        public Boolean VersionSpecified
        {
            get { return _versionSpecified; }
            set { _versionSpecified = value; }
        }
    }
}