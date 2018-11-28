namespace AppShared.ViewModels.Jwt
{
    public class JwtViewModel
    {
        #region Properties

        /// <summary>
        ///     Access code which is used for accessing into system.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        ///     Life-time of
        /// </summary>
        public int LifeTime { get; set; }

        /// <summary>
        ///     When token should be expired.
        /// </summary>
        public double Expiration { get; set; }

        #endregion
    }
}