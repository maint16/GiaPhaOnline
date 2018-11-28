namespace Main.Constants
{
    public class AppConfigKeyConstant
    {
        #region Properties

        /// <summary>
        ///     Configuration key of jwt.
        /// </summary>
        public const string AppJwt = "appJwt";

        /// <summary>
        ///     Configuration of Google OAuth.
        /// </summary>
        public const string GoogleCredential = "googleCredential";

        /// <summary>
        ///     Configuration of facebook credential.
        /// </summary>
        public const string FacebookCredential = "facebookCredential";

        /// <summary>
        ///     Configuration of firebase connection.
        /// </summary>
        public const string AppFirebase = "appFirebase";

        /// <summary>
        ///     Sendgrid configuration.
        /// </summary>
        public const string AppSendGrid = "appSendGrid";

        public const string SqliteConnectionString = "sqliteConnectionString";

        public const string SqlConnectionString = "sqlConnectionString";

        #endregion
    }
}