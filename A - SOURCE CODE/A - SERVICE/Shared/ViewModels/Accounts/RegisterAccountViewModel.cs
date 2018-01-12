namespace Shared.ViewModels.Accounts
{
    public class RegisterAccountViewModel
    {
        /// <summary>
        /// Email which is for registering account to gain access into system.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Password which is related to email.
        /// </summary>
        public string Password { get; set; }
    }
}