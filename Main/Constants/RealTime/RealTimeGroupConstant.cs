namespace Main.Constants.RealTime
{
    public class RealTimeGroupConstant
    {
        #region Properties

        /// <summary>
        ///     Group of administrators.
        /// </summary>
        public const string Admin = nameof(Admin);

        /// <summary>
        ///     Group of ordinary users.
        /// </summary>
        public const string User = nameof(User);

        /// <summary>
        ///     User which haven't authenticated into system.
        /// </summary>
        public const string Anonymous = nameof(Anonymous);

        /// <summary>
        ///     Group of user / device who is followign topic.
        /// </summary>
        public const string FollowTopicPrefix = "FollowTopic_";

        #endregion
    }
}