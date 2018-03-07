using System.Collections.Concurrent;
using System.Collections.Generic;
using Main.Interfaces.Services;

namespace Main.Services
{
    public class ValueCacheBaseService<TKey, TValue> : IValueCacheService<TKey, TValue>
    {
        #region Properties

        /// <summary>
        /// List of key-value pairs.
        /// </summary>
        public IDictionary<TKey, TValue> Pairs { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize service with specific settings.
        /// </summary>
        public ValueCacheBaseService()
        {
            Pairs = new ConcurrentDictionary<TKey, TValue>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add value to dictionary by using specific key. Override the key if it exists.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void Add(TKey key, TValue value)
        {
            var actualKey = FindKey(key);
            if (Pairs.ContainsKey(actualKey))
            {
                Pairs[actualKey] = value;
                return;
            }

            Pairs.Add(actualKey, value);
        }

        /// <summary>
        /// Get value by search for specific key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Read(TKey key)
        {
            var actualKey = FindKey(key);
            if (!Pairs.ContainsKey(actualKey))
                return default(TValue);

            return Pairs[actualKey];
        }

        /// <summary>
        /// Remove value from dictionary.
        /// </summary>
        /// <param name="key"></param>
        public void Remove(TKey key)
        {
            var actualKey = FindKey(key);
            Pairs.Remove(actualKey);
        }


        /// <summary>
        /// Find key in dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TKey FindKey(TKey key)
        {
            return key;
        }

        
        #endregion
    }
}