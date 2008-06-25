// Copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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

namespace SharpMap.Tools
{
    public abstract class MapTool : IMapTool
    {
		private readonly String _name;
        private Boolean _enabled;
        private Boolean _visible;
        private Boolean _selected;

        protected MapTool(String name)
            : this(name, true, true) { }

        protected MapTool(String name, Boolean enabled, Boolean visible)
        {
            _name = name;
            _enabled = enabled;
            _visible = visible;
        }

		public String Name
		{
			get { return _name; }
        }

	    public Boolean Enabled
	    {
            get { return _enabled; }
            set
            {
                if (value == _enabled)
                {
                    return;
                }

                Boolean cancel = false;
                
                OnEnabledChanging(ref cancel);
                
                if (cancel)
                {
                    return;
                }

                _enabled = value;

                OnEnabledChanged();
            }
	    }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                if (value == _selected)
                {
                    return;
                }

                Boolean cancel = false;

                OnSelectedChanging(ref cancel);

                if (cancel)
                {
                    return;
                }

                _selected = value;

                OnSelectedChanged();
            }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (value == _visible)
                {
                    return;
                }

                Boolean cancel = false;

                OnVisibleChanging(ref cancel);

                if (cancel)
                {
                    return;
                }

                _visible = value;

                OnVisibleChanged();
            }
        }

        public event EventHandler EnabledChanged;
        public event EventHandler VisibleChanged;
        public event EventHandler SelectedChanged;

        protected virtual void OnEnabledChanging(ref Boolean cancel) { }
        protected virtual void OnVisibleChanging(ref Boolean cancel) { }
        protected virtual void OnSelectedChanging(ref Boolean cancel) { }
        
        protected virtual void OnEnabledChanged()
        {
            EventHandler e = EnabledChanged;

            if (e != null)
            {
                e(this, EventArgs.Empty);
            }
        }

        protected virtual void OnVisibleChanged()
        {
            EventHandler e = VisibleChanged;

            if (e != null)
            {
                e(this, EventArgs.Empty);
            }
        }

        protected virtual void OnSelectedChanged()
        {
            EventHandler e = SelectedChanged;

            if (e != null)
            {
                e(this, EventArgs.Empty);
            }
        }
	}
}
