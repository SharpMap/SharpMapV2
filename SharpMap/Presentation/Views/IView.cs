// Copyright 2006, 2007 - Rory Plaire (codekaizen@gmail.com)
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

namespace SharpMap.Presentation.Views
{
    /// <summary>
    /// Interface for a view of a map.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Gets or sets whether the view is visible.
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Gets or sets whether the view is shown as enabled and available,
        /// or disabled and unavailable.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Hides the view, causing <see cref="Visible"/> to become false.
        /// </summary>
        void Hide();

        /// <summary>
        /// Shows the view, causeing <see cref="Visible"/> to become true.
        /// </summary>
        void Show();

        /// <summary>
        /// Gets or sets the title to display on the view.
        /// </summary>
        String Title { get; set; }
    }
}