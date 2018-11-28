namespace MainMicroService.Models.AdditionalMessageInfo.CategoryGroup
{
    public class AddCategoryGroupAdditionalInfoModel
    {
        #region Properties

        public string CategoryGroupName { get; set; }

        public string CreatorName { get; set; }

        #endregion

        #region Constructor

        public AddCategoryGroupAdditionalInfoModel()
        {
        }

        public AddCategoryGroupAdditionalInfoModel(string categoryGroupName, string creatorName)
        {
            CategoryGroupName = categoryGroupName;
            CreatorName = creatorName;
        }

        #endregion
    }
}