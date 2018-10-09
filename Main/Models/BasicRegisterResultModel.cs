namespace Main.Models
{
    public class BasicRegisterResultModel
    {
        #region Properties

        public string Email { get; set; }

        public string AccessToken { get; set; }

        #endregion

        #region Methods

        public BasicRegisterResultModel(string email, string accessToken)
        {
            Email = email;
            AccessToken = accessToken;
        }

        #endregion
    }
}