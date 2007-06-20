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
using System.Collections.Generic;
using System.Text;

using SharpMap.Rendering.Rendering2D;

namespace SharpMap.Rendering
{
	/// <summary>
	/// Class defining delegate for label collision detection and static predefined methods
	/// </summary>
	public class LabelCollisionDetection2D
	{
		/// <summary>
		/// Delegate method for filtering labels. Useful for performing custom collision detection on labels.
		/// </summary>
		/// <param name="labels"></param>
		/// <returns></returns>
        public delegate IEnumerable<Label2D> LabelFilterDelegate(IList<Label2D> labels);

		#region Label filter methods
		/// <summary>
		/// Simple and fast label collision detection.
		/// </summary>
		/// <param name="labels"></param>
        public static IEnumerable<Label2D> SimpleCollisionDetection(IList<Label2D> labels)
		{
            List<Label2D> labelList = new List<Label2D>(labels);

			labelList.Sort(); // sort labels by intersectiontests of labelbox

			//remove labels that intersect other labels
			for (int i = labelList.Count - 1; i > 0; i--)
			{
				if (labelList[i].CompareTo(labelList[i - 1]) == 0)
				{
					if (labelList[i].Priority > labelList[i - 1].Priority)
					{
						labelList.RemoveAt(i - 1);
					}
					else
					{
						labelList.RemoveAt(i);
					}
				}
			}

			return labelList;
		}

		/// <summary>
		/// Thorough label collision detection.
		/// </summary>
		/// <param name="labels"></param>
        public static IEnumerable<Label2D> ThoroughCollisionDetection(IList<Label2D> labels)
		{
            List<Label2D> labelList = new List<Label2D>(labels);

            labelList.Sort(); // sort labels by intersectiontests of labelbox

			//remove labels that intersect other labels
            for (int i = labelList.Count - 1; i > 0; i--)
			{
				for (int j = i - 1; j > 0; j--)
				{
                    if (labelList[i].CompareTo(labelList[j]) == 0)
					{
                        if (labelList[i].Priority >= labelList[j].Priority)
						{
                            labelList.RemoveAt(j);
							i--;
						}
						else
						{
                            labelList.RemoveAt(i);
							i--;
							break;
						}
					}
				}
			}

            return labelList;
		}
		#endregion
	}
}
