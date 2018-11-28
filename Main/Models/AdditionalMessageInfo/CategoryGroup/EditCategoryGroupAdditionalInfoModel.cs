namespace Main.Models.AdditionalMessageInfo.CategoryGroup
{
    public class EditCategoryGroupAdditionalInfoModel
    {
        #region Properties

        public string CategoryGroupName { get; set; }

        public string EditorName { get; set; }

        #endregion

        #region Constructor

        public EditCategoryGroupAdditionalInfoModel()
        {
        }

        public EditCategoryGroupAdditionalInfoModel(string categoryGroupName, string editorName)
        {
            CategoryGroupName = categoryGroupName;
            EditorName = editorName;
        }

        #endregion
    }
}
