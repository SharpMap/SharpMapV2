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
using System.Collections.Generic;

namespace SharpMap.Tools
{
    public class MapToolSet : MapTool, IMapToolSet
    {
        private readonly List<IMapTool> _tools = new List<IMapTool>();

        public MapToolSet(String name)
            : base(name) { }

        public MapToolSet(String name, IEnumerable<IMapTool> tools)
            : base(name)
        {
            foreach (IMapTool tool in tools)
            {
                Add(tool);
            }
        }

        #region IMapToolSet Members

        public void Remove(String name)
        {
            _tools.RemoveAll(delegate(IMapTool tool) { return tool.Name.Equals(name); });
        }

        public IMapTool this[String name]
        {
            get { return _tools.Find(delegate(IMapTool tool) { return tool.Name.Equals(name); }); }
        }

        public event EventHandler<MapToolSetChangedEventArgs> ToolAdded;
        public event EventHandler<MapToolSetChangedEventArgs> ToolRemoved;

        #endregion

        #region IList<IMapTool> Members

        public Int32 IndexOf(IMapTool item)
        {
            return _tools.IndexOf(item);
        }

        public void Insert(Int32 index, IMapTool item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            _tools.Insert(index, item);
            OnToolAdded(item);
        }

        public void RemoveAt(Int32 index)
        {
            if (index < 0 || index >= _tools.Count)
            {
                throw new ArgumentOutOfRangeException("index", 
                                                      index, 
                                                      "Index must be between 0 and Count.");
            }

            IMapTool tool = _tools[index];
            _tools.RemoveAt(index);
            OnToolRemoved(tool);
        }

        public IMapTool this[Int32 index]
        {
            get
            {
                return _tools[index];
            }
            set
            {
                throw new NotSupportedException("Setting tools via index not supported.");
            }
        }

        #endregion

        #region ICollection<IMapTool> Members

        public void Add(IMapTool item)
        {
            _tools.Add(item);
            OnToolAdded(item);
        }

        public void Clear()
        {
            IMapTool[] tools = _tools.ToArray();
            _tools.Clear();

            foreach (IMapTool tool in tools)
            {
                OnToolRemoved(tool);
            }
        }

        public Boolean Contains(IMapTool item)
        {
            return _tools.Contains(item);
        }

        public void CopyTo(IMapTool[] array, Int32 arrayIndex)
        {
            _tools.CopyTo(array, arrayIndex);
        }

        public Int32 Count
        {
            get { return _tools.Count; }
        }

        public Boolean IsReadOnly
        {
            get { return false; }
        }

        public Boolean Remove(IMapTool item)
        {
            if (_tools.Remove(item))
            {
                OnToolRemoved(item);
                return true;
            }

            return false;
        }

        #endregion

        #region IEnumerable<IMapTool> Members

        public IEnumerator<IMapTool> GetEnumerator()
        {
            foreach (IMapTool mapTool in _tools)
            {
                yield return mapTool;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        protected virtual void OnToolAdded(IMapTool tool)
        {
            EventHandler<MapToolSetChangedEventArgs> e = ToolAdded;

            if (e != null)
            {
                e(this, new MapToolSetChangedEventArgs(tool, MapToolSetChange.ToolAdded));
            }
        }

        protected virtual void OnToolRemoved(IMapTool tool)
        {
            EventHandler<MapToolSetChangedEventArgs> e = ToolAdded;

            if (e != null)
            {
                e(this, new MapToolSetChangedEventArgs(tool, MapToolSetChange.ToolRemoved));
            }
        }
    }
}
