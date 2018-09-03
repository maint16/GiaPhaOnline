using System;
using System.Collections.Generic;

namespace Main.Interfaces.Services
{
    public interface ITemporaryValueCacheService<TKey, TValue>
    {
        /// <summary>
        /// Add new item to cache with expiration time.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime"></param>
        void Add(TKey key, TValue value, DateTime? expirationTime);

        /// <summary>
        /// Add new item to cache with specific life time.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="lifeTime"></param>
        void Add(TKey key, TValue value, int lifeTime);
    }
}