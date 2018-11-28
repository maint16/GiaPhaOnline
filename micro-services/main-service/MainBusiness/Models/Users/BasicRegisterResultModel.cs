namespace AppBusiness.Models.Users
{
    public class BasicRegisterResultModel
    {
        #region Properties

        public string Email { get; set; }

        public string AccessToken { get; set; }

        public string Nickname { get; set; }

        #endregion

        #region Methods

        public BasicRegisterResultModel()
        {
        }

        public BasicRegisterResultModel(string email, string accessToken, string nickname)
        {
            Email = email;
            AccessToken = accessToken;
            Nickname = nickname;
        }

        #endregion
    }
}