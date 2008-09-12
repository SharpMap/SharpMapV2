
/*
 *	Portions of this file are  © 2008 Newgrove Consultants Limited, 
 *  http://www.newgrove.com; you can redistribute it and/or modify it under the terms 
 *  of the current GNU Lesser General Public License (LGPL) as published by and 
 *  available from the Free Software Foundation, Inc., 
 *  59 Temple Place, Suite 330, Boston, MA 02111-1307 USA: http://fsf.org/    
 *  This program is distributed without any warranty; 
 *  without even the implied warranty of merchantability or fitness for purpose.  
 *  See the GNU Lesser General Public License for the full details. 
 *  
 *  Author: John Diss 2008
 *
 *  Original notices below.
 * 
 */

using AGG.Rasterizer;
namespace AGG
{
    public class QuickSortCellAA
    {
        public QuickSortCellAA()
        {
        }

        public void Sort(CellAA[] dataToSort)
        {
            Sort(dataToSort, 0, (uint)(dataToSort.Length- 1));
        }

        public void Sort(CellAA[] dataToSort, uint beg, uint end)
        {
            if (end == beg)
            {
                return;
            }
            else
            {
                uint pivot = GetPivotPoint(dataToSort, beg, end);
                if (pivot > beg)
                {
                    Sort(dataToSort, beg, pivot - 1);
                }

                if (pivot < end)
                {
                    Sort(dataToSort, pivot + 1, end);
                }
            }
        }

        private uint GetPivotPoint(CellAA[] dataToSort, uint begPoint, uint endPoint)
        {
            uint pivot = begPoint;
            uint m = begPoint+1;
            uint n = endPoint;
            while ((m < endPoint)
                && dataToSort[pivot].X >= dataToSort[m].X)
            {
                m++;
            }

            while ((n > begPoint) && (dataToSort[pivot].X <= dataToSort[n].X))
            {
                n--;
            }
            while (m < n)
            {
                CellAA temp = dataToSort[m];
                dataToSort[m] = dataToSort[n];
                dataToSort[n] = temp;

                while ((m < endPoint) && (dataToSort[pivot].X >= dataToSort[m].X))
                {
                    m++;
                }

                while ((n > begPoint) && (dataToSort[pivot].X <= dataToSort[n].X))
                {
                    n--;
                }

            }
            if (pivot != n)
            {
                CellAA temp2 = dataToSort[n];
                dataToSort[n] = dataToSort[pivot];
                dataToSort[pivot] = temp2;
                
            }
            return n;
        }
    }

    public class QuickSortRangeAdaptorUint
    {
        public QuickSortRangeAdaptorUint()
        {
        }

        public void Sort(VectorPODRangeAdaptor dataToSort)
        {
            Sort(dataToSort, 0, (uint)(dataToSort.Size() - 1));
        }

        public void Sort(VectorPODRangeAdaptor dataToSort, uint beg, uint end)
        {
            if (end == beg)
            {
                return;
            }
            else
            {
                uint pivot = GetPivotPoint(dataToSort, beg, end);
                if (pivot > beg)
                {
                    Sort(dataToSort, beg, pivot - 1);
                }

                if (pivot < end)
                {
                    Sort(dataToSort, pivot + 1, end);
                }
            }
        }

        private uint GetPivotPoint(VectorPODRangeAdaptor dataToSort, uint begPoint, uint endPoint)
        {
            uint pivot = begPoint;
            uint m = begPoint + 1;
            uint n = endPoint;
            while ((m < endPoint)
                && dataToSort[pivot] >= dataToSort[m])
            {
                m++;
            }

            while ((n > begPoint) && (dataToSort[pivot] <= dataToSort[n]))
            {
                n--;
            }
            while (m < n)
            {
                uint temp = dataToSort[m];
                dataToSort[m] = dataToSort[n];
                dataToSort[n] = temp;

                while ((m < endPoint) && (dataToSort[pivot] >= dataToSort[m]))
                {
                    m++;
                }

                while ((n > begPoint) && (dataToSort[pivot] <= dataToSort[n]))
                {
                    n--;
                }

            }
            if (pivot != n)
            {
                uint temp2 = dataToSort[n];
                dataToSort[n] = dataToSort[pivot];
                dataToSort[pivot] = temp2;

            }
            return n;
        }
    }
}
