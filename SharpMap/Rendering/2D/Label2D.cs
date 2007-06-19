// Portions copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
// Portions copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 


using System;
using System.Collections.Generic;
using System.Text;

using SharpMap.Styles;

namespace SharpMap.Rendering.Rendering2D
{
	/// <summary>
	/// Class for storing a label instance.
	/// </summary>
	public class Label2D : IComparable<Label2D>, IComparer<Label2D>
    {
        private string _text;
        private ViewPoint2D _labelPoint;
        private StyleFont _font;
        private int _priority;
        private float _rotation;
        private LabelStyle _style;
        private GraphicsPath2D _labelPath;

		/// <summary>
		/// Initializes a new Label instance.
		/// </summary>
		/// <param name="text">Text to write</param>
		/// <param name="labelpoint">Position of label</param>
		/// <param name="rotation">Rotation</param>
		/// <param name="priority">Label priority used for collision detection</param>
		/// <param name="collisionbox">Box around label for collision detection</param>
		/// <param name="style">The style of the label</param>
		public Label2D(string text, ViewPoint2D position, float rotation, int priority, ViewRectangle2D? collisionArea, LabelStyle style)
		{
			_text = text;
			_labelPoint = position;
			_rotation = rotation;
			_priority = priority;
			_box = collisionArea;
			_style = style;
		}

        public override string ToString()
        {
            return String.Format("[{0}] Text: {1}; LabelPoint: {2}; Font: {3}; Rotation: {4:N}; Priority: {5}; Box: {6}",
                GetType(), Text, LabelPoint, Font, Rotation, Priority, Box);
        }

		/// <summary>
		/// The text of the label
		/// </summary>
		public string Text
		{
			get { return _text; }
			set { _text = value; }
		}

		/// <summary>
		/// Label position
		/// </summary>
        public ViewPoint2D LabelPoint
		{
			get { return _labelPoint; }
			set { _labelPoint = value; }
		}

		/// <summary>
		/// Label font
		/// </summary>
		public StyleFont Font
		{
			get { return _font; }
			set { _font = value; }
		}

		/// <summary>
		/// Label rotation.
		/// </summary>
		public float Rotation
		{
			get { return _rotation; }
			set { _rotation = value; }
		}

		/// <summary>
        /// Priority in layout.
		/// </summary>
		public int Priority
		{
			get { return _priority; }
			set { _priority = value; }
		}

		/// <summary>
		/// Label box.
		/// </summary>
		public ViewRectangle2D? Box
		{
			get { return _box; }
			set { _box = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="SharpMap.Styles.LabelStyle"/> of this label.
		/// </summary>
		public LabelStyle Style
		{
			get { return _style; }
			set { _style = value; }
		}

		#region IComparable<Label> Members

		/// <summary>
		/// Tests if two label boxes intersects
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(Label2D other)
		{
			if (this == other)
				return 0;
			else if (_box == null)
				return -1;
			else if (other.Box == null)
				return 1;
			else
				return _box.Value.CompareTo(other.Box.Value);
		}

		#endregion

		#region IComparer<Label> Members

		/// <summary>
		/// Checks if two labels intersect
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(Label2D x, Label2D y)
		{
			return x.CompareTo(y);
		}

		#endregion
	}
}
