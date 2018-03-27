using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Main.Constants
{
    public class LifeTimeConstant
    {
        /// <summary>
        /// How many seconds that a jwt exists
        /// </summary>
        public const int JwtLifeTime = 18000;

        /// <summary>
        /// How many seconds that a cache exists
        /// </summary>
        public const int ProfileCacheLifeTime = 300;
    }
}
