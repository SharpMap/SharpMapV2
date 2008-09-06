
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

namespace AGG.Scanline
{
    public struct ScanlineSpan
    {
        private int x;

        public int X
        {
            get { return x; }
            set { x = value; }
        }
        private int len;

        public int Len
        {
            get { return len; }
            set { len = value; }
        }
        private uint cover_index;

        public uint CoverIndex
        {
            get { return cover_index; }
            set { cover_index = value; }
        }
    };

    public interface IScanlineCache
    {
        void Finalize(int y);
        void Reset(int min_x, int max_x);
        void ResetSpans();
        uint NumSpans { get; }
        ScanlineSpan Begin();
        ScanlineSpan GetNextScanlineSpan();
        int Y { get; }
        byte[] Covers { get; }
        void AddCell(int x, uint cover);
        void AddSpan(int x, int len, uint cover);
    };
}
