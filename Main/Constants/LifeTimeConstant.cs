namespace Main.Constants
{
    public class LifeTimeConstant
    {
        /// <summary>
        ///     How many seconds that a jwt exists
        /// </summary>
        public const int JwtLifeTime = 18000;

        /// <summary>
        ///     How many seconds that user cache exists
        /// </summary>
        public const int ProfileCacheLifeTime = 300;

        /// <summary>
        ///     How many seconds that category cache exists
        /// </summary>
        public const int CategoryCacheLifeTime = 300;
    }
}