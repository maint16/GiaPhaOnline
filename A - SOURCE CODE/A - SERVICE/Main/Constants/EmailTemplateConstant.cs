using Microsoft.CodeAnalysis.Operations;

namespace Main.Constants
{
    public class EmailTemplateConstant
    {
        #region Properties

        /// <summary>
        /// Name of password request submission.
        /// </summary>
        public const string SubmitPasswordRequest = nameof(SubmitPasswordRequest);

        public const string ResendAccountActivationCode = nameof(ResendAccountActivationCode);

        public const string RegisterBasicAccount = nameof(RegisterBasicAccount);

        public const string ForgotPasswordRequest = nameof(ForgotPasswordRequest);

        public const string SubmitPasswordReset = nameof(SubmitPasswordReset);

        #endregion
    }
}