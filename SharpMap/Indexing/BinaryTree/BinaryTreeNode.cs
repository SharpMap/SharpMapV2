using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMap.Indexing.BinaryTree
{
    // Binary Tree not working yet on Mono 
    // see bug: http://bugzilla.ximian.com/show_bug.cgi?id=78502
#if !MONO
    [Serializable]
    internal class BinaryTreeNode<TKey, TValue> where TValue : IComparable<TValue>
    {
        public BinaryTreeNode<TKey, TValue> LeftNode;
        public BinaryTreeNode<TKey, TValue> RightNode;
        public BinaryTree<TKey, TValue>.ItemValue Item;

        public BinaryTreeNode()
            : this(default(TKey), default(TValue), null, null)
        {
        }

        public BinaryTreeNode(TKey key, TValue item)
            : this(key, item, null, null)
        {
        }

        public BinaryTreeNode(BinaryTree<TKey, TValue>.ItemValue value)
            : this(value.Id, value.Value, null, null)
        {
        }

        public BinaryTreeNode(TKey key, TValue item, BinaryTreeNode<TKey, TValue> right, BinaryTreeNode<TKey, TValue> left)
        {
            RightNode = right;
            LeftNode = left;
            Item = new BinaryTree<TKey, TValue>.ItemValue();
            Item.Id = key;
            Item.Value = item;
        }

        public static Boolean operator >(BinaryTreeNode<TKey, TValue> lhs, BinaryTreeNode<TKey, TValue> rhs)
        {
            Int32 res = lhs.Item.Value.CompareTo(rhs.Item.Value);
            return res > 0;
        }

        public static Boolean operator <(BinaryTreeNode<TKey, TValue> lhs, BinaryTreeNode<TKey, TValue> rhs)
        {
            Int32 res = lhs.Item.Value.CompareTo(rhs.Item.Value);
            return res < 0;
        }
    }
#endif
}
