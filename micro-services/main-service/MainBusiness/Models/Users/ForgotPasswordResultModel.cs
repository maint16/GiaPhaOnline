namespace MainBusiness.Models.Users
{
    public class ForgotPasswordResultModel
    {
        #region Properties

        public string Email { get; set; }

        public string Token { get; set; }

        #endregion
    }
}