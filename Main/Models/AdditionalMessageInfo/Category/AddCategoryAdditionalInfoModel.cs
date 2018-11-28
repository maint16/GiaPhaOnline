namespace Main.Models.AdditionalMessageInfo.Category
{
    public class AddCategoryAdditionalInfoModel
    {
        #region Properties

        public string CategoryName { get; set; }

        public string CreatorName { get; set; }

        #endregion

        #region Constructor

        public AddCategoryAdditionalInfoModel()
        {
        }

        public AddCategoryAdditionalInfoModel(string categoryName, string creatorName)
        {
            CategoryName = categoryName;
            CreatorName = creatorName;
        }

        #endregion
    }
}
