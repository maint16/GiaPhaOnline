namespace Main.Models
{
    public class ApplicationSetting
    {
        #region Properties

        /// <summary>
        /// Life time of password reset token.
        /// </summary>
        public int PasswordResetTokenLifeTime { get; set; }

        #endregion
    }
}