// Copyright 2007, 2008 - Ariel Yaroshevich (a.k.a blackrussian) (hamlet@inter.net.il)
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
//
//
// Credits:
// This code is based on Ray Hayes' article
// can be found here: http://www.codeproject.com/csharp/genericcache.asp
//
// Changes from the original implemntation:
// 1. Moved purging policy to seperate interface, instead of using byte count of the chachable object
// 


#region Using directives

using System;
using System.Collections.Generic;

#endregion

namespace SharpMap.Rendering.Wpf
{
    enum TouchMode { OnEveryAcess, ExplicitOnly }
    public interface ICachePolicy<TKey, TValue>
    {
        Boolean UpperLimitReached { get; }
        Boolean LowerLimitReached { get; }
        void RecordItem(TKey key);
        void Remove(TKey key);
        TKey Dequeue();
        void Clear();
    }

    public abstract class CachePolicy<TKey, TValue> : ICachePolicy<TKey, TValue>
        where TKey : IEquatable<TKey>
    {
        readonly private Cache<TKey, TValue> _cache;
        protected CachePolicy(Cache<TKey, TValue> cache)
        {
            _cache = cache;
        }

        #region ICachePolicy Members
        public abstract Boolean UpperLimitReached { get; }
        public abstract Boolean LowerLimitReached { get; }
        public abstract void Remove(TKey key);
        public abstract void RecordItem(TKey key);
        public abstract TKey Dequeue();
        public abstract void Clear();
        #endregion

        protected Cache<TKey, TValue> Cache
        {
            get { return _cache; }
        }
    }

    /// <summary>
    /// Interface definition for the <c>ICacheable</c> items.
    /// </summary>
    public interface ICacheable
    {
        int BytesUsed { get; }
    }

    /// <summary>
    /// Class definition for a cacheable collection of items.  Specifically, 
    /// a maximum size for the cache may be defined and objects added will 
    /// be kept-alive whilst there is sufficient cache space.  Once an item
    /// has be pushed out of the cache, it is subject to the normal GC conditions.
    /// </summary>
    /// <typeparam name="TKey">Type of the identifier to use.</typeparam>
    /// <typeparam name="TValue">Type of the values to be stored.</typeparam>
    /// <remarks>Some limitations have been made on the types used in this
    /// generic.  Mainly, the <c>Key</c> must implement the <c>IComparable</c>
    /// interface and the <c>Value</c> must implement the <c>ICacheable</c>
    /// interface.</remarks>
    public class Cache<TKey, TValue>
        where TKey : IEquatable<TKey>
    {
        private readonly ICachePolicy<TKey, TValue> _cachePolicy;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Cache()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cachePolicy"></param>
        public Cache(ICachePolicy<TKey, TValue> cachePolicy)
        {
            _cachePolicy = cachePolicy;
        }

        /// <summary>
        /// The private container representing the cache.
        /// </summary>
        private readonly Dictionary<TKey, TValue> _cacheStore = new Dictionary<TKey, TValue>();

        /// <summary>
        /// Returns the current usage of the cache.
        /// </summary>
        /// <value>Current cache's managed size.</value>
        /// <remarks>The cache is at the mercy of the <c>ICacheable</c> items
        /// returning their true size.</remarks>
        public int CurrentCacheUsage
        {
            get
            {
                int size = 0;
                if (_cacheStore != null)
                {
                    foreach (KeyValuePair<TKey, TValue> c in _cacheStore)
                    {
                        size += (c.Value as ICacheable).BytesUsed;
                    }
                }
                return size;
            }
        }

        /// <summary>
        /// This function is used when it has been determined that the cache is
        /// too full to fit the new item.  The function is called with the parameter
        /// of the number of bytes needed.  A basic purging algorthim is used to make
        /// space.
        /// </summary>
        /// <remarks>This purge function may be improved with some hit-count being
        /// maintained.</remarks>
        private void PurgeSpace()
        {
            if (_cacheStore.Count != 0)
            {
                while (!_cachePolicy.LowerLimitReached)
                {
                    TKey key = _cachePolicy.Dequeue();
                    _cacheStore.Remove(key);
                }
            }
        }



        /// <summary>
        /// Add a new item into the cache.
        /// </summary>
        /// <param name="key">Identifier or key for item to add.</param>
        /// <param name="value">Actual item to store.</param>
        public void Add(TKey key, TValue value)
        {
            _cachePolicy.RecordItem(key);

            if (_cachePolicy.UpperLimitReached)
            {
                PurgeSpace();
            }
            _cacheStore.Add(key, value);
        }



        /// <summary>
        /// Touch or refresh a specified item.  This allows the specified
        /// item to be moved to the end of the dispose queue.  E.g. when it
        /// is known that this item would benifit from not being purged.
        /// </summary>
        /// <param name="key">Identifier of item to touch.</param>
        public void Touch(TKey key)
        {
            _cachePolicy.Remove(key);
            _cachePolicy.RecordItem(key);   // Put at end of queue.
        }

        /// <summary>
        /// Returns the item associated with the supplied identifier.
        /// </summary>
        /// <param name="key">Identifier for the value to be returned.</param>
        /// <returns>Item value corresponding to Key supplied.</returns>
        /// <remarks>Accessing a stored item in this way automatically
        /// forces the item to the end of the purge queue.</remarks>
        public TValue GetValue(TKey key)
        {
            if (_cacheStore != null && _cacheStore.ContainsKey(key))
            {

                Touch(key);
                return _cacheStore[key];
            }
            
            return default(TValue);
        }

        /// <summary>
        /// Determines whether the cache contains the specific key.
        /// </summary>
        /// <param name="key">Key to locate in the cache.</param>
        /// <returns><c>true</c> if the cache contains the specified key; 
        /// otherwise <c>false</c>.</returns>
        public Boolean ContainsKey(TKey key)
        {
            return (_cacheStore != null && _cacheStore.ContainsKey(key));
        }

        /// <summary>
        /// Indexer into the cache using the associated key to specify
        /// the value to return.
        /// </summary>
        /// <param name="key">Key identifying value to return.</param>
        /// <returns>The value asspciated to the supplied key.</returns>
        public TValue this[TKey key] { get { return _cacheStore[key]; } }

        /// <summary>
        /// Returns a ICollection of the keys.
        /// </summary>
        /// <returns>The ICollection for keys.</returns>
        public ICollection<TKey> Keys
        {
            get
            {
                return _cacheStore.Keys;
            }
        }
        /// <summary>
        /// Returns the <c>KeyValuePair&lt;Key,Value&gt;</c> for the cache 
        /// collection.
        /// </summary>
        /// <returns>The enumerator for the cache, returning both the
        /// key and the value as a pair.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEnumerator<KeyValuePair<TKey, TValue>> Items
        {
            get
            {
                return _cacheStore.GetEnumerator();
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                return _cacheStore.Values;
            }
        }

        /// <summary>
        /// Gets the number of items stored in the cache.
        /// </summary>
        /// <value>The number of items stored in the cache. </value>
        public int Count
        {
            get { return (_cacheStore == null) ? 0 : _cacheStore.Count; }
        }

        /// <summary>
        /// Empties the cache of all items.
        /// </summary>
        public void PurgeAll()
        {
            System.Diagnostics.Debug.WriteLine(String.Format("Purging cached collection of {0} items", _cacheStore.Count));
            _cacheStore.Clear();
        }
    }
}
