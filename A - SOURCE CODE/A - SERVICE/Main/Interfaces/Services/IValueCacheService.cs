using System;
using System.Collections.Generic;

namespace Main.Interfaces.Services
{
    public interface IValueCacheService<TKey, TValue>
    {
        #region Properties

        /// <summary>
        /// List of key-value.
        /// </summary>
        IDictionary<TKey, TValue> Pairs { get; }

        #endregion

        #region Methods
        
        /// <summary>
        /// Add template.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Add(TKey key, TValue value);

        /// <summary>
        /// Get template by using specific key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue Read(TKey key);

        /// <summary>
        /// Remove a value from dictionary.
        /// </summary>
        /// <param name="key"></param>
        void Remove(TKey key);

        /// <summary>
        /// Find key in dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TKey FindKey(TKey key);
        
        #endregion
    }
}