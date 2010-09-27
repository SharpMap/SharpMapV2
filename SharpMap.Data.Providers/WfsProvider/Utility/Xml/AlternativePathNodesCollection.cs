using System;
using System.Collections.Generic;
using System.Xml;

namespace SharpMap.Data.Providers.WfsProvider.Utility.Xml
{
    /// <summary>
    /// This class represents a collection of path nodes that can be used alternatively.
    /// </summary>
    internal class AlternativePathNodesCollection : IPathNode
    {
        #region Fields

        private readonly List<IPathNode> _PathNodes = new List<IPathNode>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AlternativePathNodesCollection"/> class.
        /// </summary>
        /// <param name="pathNodes">A collection of instances implementing <see cref="IPathNode"/></param>
        internal AlternativePathNodesCollection(params IPathNode[] pathNodes)
        {
            if (pathNodes == null) throw new ArgumentNullException();
            _PathNodes.AddRange(pathNodes);
        }

        #endregion

        #region IPathNode Member

        /// <summary>
        /// This method evaluates all inherent instances of <see cref="IPathNode"/>.
        /// </summary>
        /// <param name="reader">An XmlReader instance</param>
        public bool Matches(XmlReader reader)
        {
            foreach (IPathNode pathNode in _PathNodes)
                if (pathNode.Matches(reader)) return true;
            return false;
        }

        /// <summary>
        /// Determines whether the inherent PathNodes shall be active.
        /// If a PathNode is not active, all match operations return true.
        /// </summary>
        public bool IsActive
        {
            get { return _PathNodes[0].IsActive; }
            set
            {
                foreach (IPathNode pathNode in _PathNodes)
                    pathNode.IsActive = value;
            }
        }

        #endregion
    }
}