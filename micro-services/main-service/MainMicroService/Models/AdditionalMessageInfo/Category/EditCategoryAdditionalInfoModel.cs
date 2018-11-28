namespace MainMicroService.Models.AdditionalMessageInfo.Category
{
    public class EditCategoryAdditionalInfoModel
    {
        #region Properties

        public string CategoryName { get; set; }

        public string EditorName { get; set; }

        #endregion

        #region Constructor

        public EditCategoryAdditionalInfoModel()
        {
        }

        public EditCategoryAdditionalInfoModel(string categoryName, string editorName)
        {
            CategoryName = categoryName;
            EditorName = editorName;
        }

        #endregion
    }
}