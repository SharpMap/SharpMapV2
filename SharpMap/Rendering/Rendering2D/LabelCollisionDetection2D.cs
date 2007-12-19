// Copyright 2006 - Morten Nielsen (www.iter.dk)
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
using System.Collections;
using System.Collections.Generic;
using SharpMap.Styles;

namespace SharpMap.Rendering.Rendering2D
{
	/// <summary>
	/// Class defining delegate for label collision detection and static predefined methods
	/// </summary>
	public class LabelCollisionDetection2D
	{
		#region private data
		List<Label2D> labelList = new List<Label2D>();
		private ITextRenderer2D _textRenderer;
		#endregion

		#region properties

		public ITextRenderer2D TextRenderer
		{
			get { return _textRenderer; }
			set { _textRenderer = value; }
		}
		#endregion

		#region public interface

		/// <summary>
		/// Test whether label collides.
		/// </summary>
		/// <param name="newLabel"></param>
		/// <returns>true if label collided with another (more important or earlier) label</returns>
		public Boolean SimpleCollisionTest(Label2D newLabel)
		{
			if (labelList.Contains(newLabel))
			{
				return false;
			}

			foreach (Label2D label in labelList)
			{
				if (label.CompareTo(newLabel) == 0)
				{
					if (label.Priority >= newLabel.Priority)
					{
						return true;
					}
				}
			}

			labelList.Add(newLabel);

			return false;
		}

		/// <summary>
		/// Test whether label collides.
		/// </summary>
		/// <param name="newLabel"></param>
		/// <returns>true if label collided with another (more important or earlier) label</returns>
		public Boolean AdvancedCollisionTest(Label2D newLabel)
		{
			if (labelList.Contains(newLabel))
			{
				return false;
			}
			return AdvancedCollisionTest(newLabel, 0);
		}

		private Boolean AdvancedCollisionTest(Label2D newLabel, int depth)
		{
			newLabel.Style.Halo = new StylePen(newLabel.Style.Foreground, 1);

			Size2D newSize = TextRenderer.MeasureString(newLabel.Text, newLabel.Font);

			newSize = new Size2D(newSize.Width + 2 * newLabel.CollisionBuffer.Width, newSize.Height + 2 * newLabel.CollisionBuffer.Height);
			Rectangle2D newRect = new Rectangle2D(new Point2D(newLabel.Location.X - newLabel.CollisionBuffer.Width, newLabel.Location.Y - newLabel.CollisionBuffer.Height), newSize);

			foreach (Label2D label in labelList)
			{
				Size2D size = TextRenderer.MeasureString(label.Text, label.Font);

				size = new Size2D(size.Width + 2 * label.CollisionBuffer.Width, size.Height + 2 * label.CollisionBuffer.Height);

				Rectangle2D rect = new Rectangle2D(new Point2D(label.Location.X - newLabel.CollisionBuffer.Width, label.Location.Y - label.CollisionBuffer.Height), size);

				if(newRect.Intersects(rect))
				{
					if (label.Text == newLabel.Text)
						return true;

					if (depth == 5)
					{
						/* *
						StyleFont font = newLabel.Font;
						newLabel.Style.Foreground = new SolidStyleBrush(StyleColor.Yellow);
						newLabel.Style.Halo = new StylePen(newLabel.Style.Foreground, 1);
						newLabel.Font = new StyleFont(font.FontFamily, new Size2D(font.Size.Width / 3.0, font.Size.Height / 3.0), font.Style);
						newLabel.Text = newLabel.Text + ":" + label.Text;
						labelList.Add(newLabel);
						/*/
						return true;
						/* */
					}
					else
					{
						if(newRect.Location.Y > (rect.Location.Y + rect.Height/2.0))
						{
							newLabel.Location = new Point2D(newLabel.Location.X, rect.Location.Y + rect.Height + 3);
						}
						else
						{
							newLabel.Location = new Point2D(newLabel.Location.X, rect.Location.Y - newRect.Height - 3);
						}
						newLabel.Style.Foreground = new SolidStyleBrush(StyleColor.Blue);
						//StyleFont font = newLabel.Font;
						//newLabel.Font = new StyleFont(font.FontFamily, new Size2D(font.Size.Width / depth, font.Size.Height / depth), font.Style);
						newLabel.Style.Halo = new StylePen(newLabel.Style.Foreground, 1);
						return AdvancedCollisionTest(newLabel, depth + 1);
					}
				}
				//else
				//{
				//    newLabel.Style.Halo = new StylePen(new SolidStyleBrush(StyleColor.Purple), 1);
				//}
			}

			labelList.Add(newLabel);

			return false;
		}

		public void CleanUp()
		{
			labelList.Clear();
		}

		#endregion
	}
}
