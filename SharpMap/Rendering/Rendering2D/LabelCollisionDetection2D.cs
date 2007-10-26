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

namespace SharpMap.Rendering.Rendering2D
{
	/// <summary>
	/// Class defining delegate for label collision detection and static predefined methods
	/// </summary>
	public class LabelCollisionDetection2D
	{
		#region Label filter methods
		/// <summary>
		/// Simple and fast label collision detection.
		/// </summary>
		/// <param name="labels"></param>
		public static IEnumerable SimpleCollisionDetection(IList labels)
		{
            ArrayList labelList = new ArrayList(labels);

			labelList.Sort(); // sort labels by intersection tests of the label's collision box

			//remove labels that intersect other labels
			for (Int32 i = labelList.Count - 1; i > 0; i--)
			{
                Label2D label1 = labelList[i] as Label2D;
                Label2D label2 = labelList[i - 1] as Label2D;

                if (label1 == null)
                {
                    labelList.RemoveAt(i);
                    continue;
                }

                if (label1.CompareTo(label2) == 0)
				{
                    if (label1.Priority > label2.Priority)
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
        public static IEnumerable ThoroughCollisionDetection(IList labels)
        {
            ArrayList labelList = new ArrayList(labels);

            labelList.Sort(); // sort labels by intersectiontests of labelbox

			//remove labels that intersect other labels
            for (Int32 i = labelList.Count - 1; i > 0; i--)
			{
				for (Int32 j = i - 1; j > 0; j--)
                {
                    Label2D label1 = labelList[i] as Label2D;
                    Label2D label2 = labelList[j] as Label2D;

                    if (label1 == null)
                    {
                        labelList.RemoveAt(i);
                        break;
                    }

                    if (label1.CompareTo(label2) == 0)
					{
                        if (label1.Priority >= label2.Priority)
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
