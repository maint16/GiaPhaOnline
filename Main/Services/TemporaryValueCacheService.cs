using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Main.Interfaces.Services;

namespace Main.Services
{
    public class TemporaryValueCacheService<TKey, TValue> : ValueCacheBaseService<TKey, TValue>, ITemporaryValueCacheService<TKey, TValue>
    {
        private readonly IDictionary<TKey, KeyValuePair<TValue, DateTime?>> _pairs;

        public TemporaryValueCacheService()
        {
            _pairs = new ConcurrentDictionary<TKey, KeyValuePair<TValue, DateTime?>>();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public override void Add(TKey key, TValue value)
        {
            Add(key, value, null);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime"></param>
        public virtual void Add(TKey key, TValue value, DateTime? expirationTime)
        {
            // Format key.
            var formattedKey = FindKey(key);

            // Item is already in cache. Replace it with the new one.
            if (_pairs.ContainsKey(formattedKey))
            {
                _pairs[formattedKey] = new KeyValuePair<TValue, DateTime?>(value, expirationTime);
                return;
            }

            // Add new permanant item to cache/
            _pairs.Add(formattedKey, new KeyValuePair<TValue, DateTime?>(value, expirationTime));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="lifeTime"></param>
        public virtual void Add(TKey key, TValue value, int lifeTime)
        {
            var expirationTime = DateTime.Now.AddSeconds(lifeTime);
            Add(key, value, expirationTime);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override TValue Read(TKey key)
        {
            // Get data from cache.
            var formattedKey = FindKey(key);

            // Data doesn't exist.
            if (!_pairs.ContainsKey(formattedKey))
                return default(TValue);

            // Get data from cache.
            var item = _pairs[formattedKey];

            // Item is permanent.
            if (item.Value == null)
                return item.Key;

            // Item life time hasn't been expired.
            var expirationTime = item.Value;
            if (expirationTime <= DateTime.Now)
                return item.Key;

            // Remove item due to its expiration.
            _pairs.Remove(formattedKey);

            return default(TValue);
        }
    }
}