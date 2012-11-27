using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Symbology.StyleAdditions
{
    public class LabelStyle : Styles.LabelStyle, IFeatureStyleNames
    {
        private ICollection<String> _semanticTypeIdentifier = new Collection<String>();

        /// <summary>
        /// Gets or sets a value indicating the symbology encoding name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="IStyleNames.Description"/> of the style
        /// </summary>
        public Description Description { get; set; }

        /// <summary>
        /// Gets or sets a list of semantic type identifiers
        /// </summary>
        public ICollection<String> SemanticTypeIdentifier
        {
            get { return _semanticTypeIdentifier; }
            set { _semanticTypeIdentifier = value; }
        }

        public string FeatureTypeName { get; set; }

        public LabelStyle()
        {}

        public LabelStyle(StyleFont font, StyleBrush foreground) : base(font, foreground)
        {}

        public LabelStyle(StyleFont font,
						  StyleBrush foreground,
						  StyleBrush background,
						  Point2D offset,
						  Size2D collisionBuffer,
						  HorizontalAlignment horizontalAlignment,
						  VerticalAlignment verticalAlignment)
            : base(font, foreground, background, offset, collisionBuffer, horizontalAlignment, verticalAlignment)
        {}
    }
}
