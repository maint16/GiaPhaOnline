namespace MainMicroService.Models.AdditionalMessageInfo.Category
{
    public class FollowCategoryAdditionalInfoModel
    {
        #region Properties

        public string CategoryName { get; set; }

        public string FollowerName { get; set; }

        #endregion

        #region Constructor

        public FollowCategoryAdditionalInfoModel()
        {
        }

        public FollowCategoryAdditionalInfoModel(string categoryName, string followerName)
        {
            CategoryName = categoryName;
            FollowerName = followerName;
        }

        #endregion
    }
}