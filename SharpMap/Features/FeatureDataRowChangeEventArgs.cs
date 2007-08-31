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
using System.Data;

namespace SharpMap.Features
{
    /// <summary>
    /// Occurs after a FeatureDataRow has been changed successfully.
    /// </summary>
#if !DEBUG_STEPINTO
	[System.Diagnostics.DebuggerStepThrough()]
#endif
    public class FeatureDataRowChangeEventArgs : EventArgs
    {
        private readonly FeatureDataRow eventRow;
        private readonly DataRowAction eventAction;

        /// <summary>
        /// Initializes a new instance of the FeatureDataRowChangeEventArgs class.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="action"></param>
        public FeatureDataRowChangeEventArgs(FeatureDataRow row, DataRowAction action)
        {
            eventRow = row;
            eventAction = action;
        }

        /// <summary>
        /// Gets the row upon which an action has occurred.
        /// </summary>
        public FeatureDataRow Row
        {
            get { return this.eventRow; }
        }

        /// <summary>
        /// Gets the action that has occurred on a FeatureDataRow.
        /// </summary>
        public DataRowAction Action
        {
            get { return this.eventAction; }
        }
    }
}
