using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SharpMap.Rendering.Rendering2D;
using SharpMap.Styles;

namespace SharpMap.Symbology.StyleAdditions
{
    public class LabelStyle : SharpMap.Styles.LabelStyle, IFeatureStyleNames
    {
        private String _name;
        private Description _description;
        private ICollection<String> _semanticTypeIdentifier = new Collection<String>();
        private String _featureTypeName;

        public String Name
        {
            get { return _name;  }
            set { _name = value; }
        }

        public Description Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public ICollection<String> SemanticTypeIdentifier
        {
            get { return _semanticTypeIdentifier; }
            set { _semanticTypeIdentifier = value; }
        }

        public String FeatureTypeName
        {
            get { return _featureTypeName; }
            set { _featureTypeName = value; }
        }

        public LabelStyle() : base()
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
