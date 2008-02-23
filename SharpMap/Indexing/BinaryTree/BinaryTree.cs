// Portions copyright 2005 - 2006: Morten Nielsen (www.iter.dk)
// Portions copyright 2006 - 2008: Rory Plaire (codekaizen@gmail.com)
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
using System.Diagnostics;
using System.Collections.Generic;

namespace SharpMap.Indexing.BinaryTree
{
    // Binary Tree not working yet on Mono 
    // see bug: http://bugzilla.ximian.com/show_bug.cgi?id=78502
#if !MONO

    /// <summary>
    /// The BinaryTree class are used for indexing values to enhance the speed of queries
    /// </summary>
    /// <typeparam name="TKey">Value ID type</typeparam>
    /// <typeparam name="TValue">Value type to be indexed</typeparam>
    [Serializable]
    public class BinaryTree<TKey, TValue> where TValue : IComparable<TValue>
    {
        private BinaryTreeNode<TKey, TValue> root;

        /// <summary>
        /// A value in a <see cref="BinaryTree{TKey, TValue}"/>.
        /// </summary>
        public struct ItemValue
        {
            /// <summary>
            /// Identifier for the value
            /// </summary>
            public TKey Id;

            /// <summary>
            /// Value
            /// </summary>
            public TValue Value;

            /// <summary>
            /// Creates an instance of an item in a <see cref="BinaryTree{TKey, TValue}"/>.
            /// </summary>
            /// <param name="value">Value</param>
            /// <param name="id">Identifier for the value</param>
            public ItemValue(TKey id, TValue value)
            {
                Id = id;
                Value = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the generic binary tree.
        /// </summary>
        public BinaryTree()
        {
            root = new BinaryTreeNode<TKey, TValue>();
        }

        /// <summary>
        /// Inserts a value into the tree
        /// </summary>
        /// <param name="items"></param>
        public void Add(params ItemValue[] items)
        {
            Array.ForEach(items, Add);
        }

        /// <summary>
        /// Inserts a value into the tree
        /// </summary>
        /// <param name="item"></param>
        public void Add(ItemValue item)
        {
            Add(new BinaryTreeNode<TKey, TValue>(item.Id, item.Value), root);
        }

        /// <summary>
        /// Inserts a node into the tree
        /// </summary>
        /// <param name="newNode"></param>
        /// <param name="root"></param>
        private void Add(BinaryTreeNode<TKey, TValue> newNode, BinaryTreeNode<TKey, TValue> root)
        {
            if (newNode > root)
            {
                if (root.RightNode == null)
                {
                    root.RightNode = newNode;
                    return;
                }

                Add(newNode, root.RightNode);
            }

            if (newNode < root)
            {
                if (root.LeftNode == null)
                {
                    root.LeftNode = newNode;
                    return;
                }

                Add(newNode, root.LeftNode);
            }
        }

        #region IEnumerables
        /// <summary>
        /// Gets an enumerator for all the values in the tree in ascending order
        /// </summary>
        public IEnumerable<ItemValue> InOrder
        {
            get { return scanInOrder(root.RightNode); }
        }

        /// <summary>
        /// Gets and enumerator for the values between min and max in ascending order
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns>Values between <paramref name="min"/> and <paramref name="max"/>.</returns>
        public IEnumerable<ItemValue> Between(TValue min, TValue max)
        {
            return scanBetween(min, max, root.RightNode);
        }

        /// <summary>
        /// Enumerates the objects whose String-representation starts with 'str'
        /// </summary>
        /// <param name="str"></param>
        /// <returns>Enumerator</returns>
        public IEnumerable<ItemValue> StartsWith(String str)
        {
            return scanString(str, root.RightNode);
        }

        /// <summary>
        /// Enumerates all objects with the specified value
        /// </summary>
        /// <param name="value">Value to search for</param>
        /// <returns>Enumerator</returns>
        public IEnumerable<ItemValue> Find(TValue value)
        {
            return scanFind(value, root.RightNode);
        }
        #endregion

        /// <summary>
        /// This is the classic computer science binary tree iteration 
        /// </summary>
        internal void TraceTree()
        {
            traceInOrder(root.RightNode);
        }

        #region Private Helper Methods
        private IEnumerable<ItemValue> scanFind(TValue value, BinaryTreeNode<TKey, TValue> root)
        {
            if (root.Item.Value.CompareTo(value) > 0)
            {
                if (root.LeftNode != null)
                {
                    if (root.LeftNode.Item.Value.CompareTo(value) > 0)
                    {
                        foreach (ItemValue item in scanFind(value, root.LeftNode))
                        {
                            yield return item;
                        }
                    }
                }
            }

            if (root.Item.Value.CompareTo(value) == 0)
            {
                yield return root.Item;
            }

            if (root.Item.Value.CompareTo(value) < 0)
            {
                if (root.RightNode != null)
                {
                    if (root.RightNode.Item.Value.CompareTo(value) > 0)
                    {
                        foreach (ItemValue item in scanFind(value, root.RightNode))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        private IEnumerable<ItemValue> scanString(String val, BinaryTreeNode<TKey, TValue> root)
        {
            if (String.Compare(root.Item.Value.ToString().Substring(0, val.Length), val, StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                if (root.LeftNode != null)
                {
                    if (String.Compare(root.LeftNode.Item.Value.ToString(), val, StringComparison.CurrentCultureIgnoreCase) < 0)
                    {
                        foreach (ItemValue item in scanString(val, root.LeftNode))
                        {
                            yield return item;
                        }
                    }
                }
            }

            if (String.Compare(root.Item.Value.ToString().Substring(0, val.Length), val, StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                yield return root.Item;
            }

            if (String.Compare(root.Item.Value.ToString(), val, StringComparison.CurrentCultureIgnoreCase) < 0)
            {
                if (root.RightNode != null)
                {
                    if (String.Compare(root.RightNode.Item.Value.ToString(), val, StringComparison.CurrentCultureIgnoreCase) > 0)
                    {
                        foreach (ItemValue item in scanString(val, root.RightNode))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        private IEnumerable<ItemValue> scanBetween(TValue min, TValue max, BinaryTreeNode<TKey, TValue> root)
        {
            if (root.Item.Value.CompareTo(min) > 0)
            {
                if (root.LeftNode != null)
                {
                    if (root.LeftNode.Item.Value.CompareTo(min) > 0)
                    {
                        foreach (ItemValue item in scanBetween(min, max, root.LeftNode))
                        {
                            yield return item;
                        }
                    }
                }
            }

            if (root.Item.Value.CompareTo(min) > 0 && root.Item.Value.CompareTo(max) < 0)
            {
                yield return root.Item;
            }

            if (root.Item.Value.CompareTo(max) < 0)
            {
                if (root.RightNode != null)
                {
                    if (root.RightNode.Item.Value.CompareTo(min) > 0)
                    {
                        foreach (ItemValue item in scanBetween(min, max, root.RightNode))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        private IEnumerable<ItemValue> scanInOrder(BinaryTreeNode<TKey, TValue> root)
        {
            if (root.LeftNode != null)
            {
                foreach (ItemValue item in scanInOrder(root.LeftNode))
                {
                    yield return item;
                }
            }

            yield return root.Item;

            if (root.RightNode != null)
            {
                foreach (ItemValue item in scanInOrder(root.RightNode))
                {
                    yield return item;
                }
            }
        }

        private void traceInOrder(BinaryTreeNode<TKey, TValue> root)
        {
            if (root.LeftNode != null)
            {
                traceInOrder(root.LeftNode);
            }

            Trace.WriteLine(root.Item.ToString());

            if (root.RightNode != null)
            {
                traceInOrder(root.RightNode);
            }
        }
        #endregion
    }
#endif
}
