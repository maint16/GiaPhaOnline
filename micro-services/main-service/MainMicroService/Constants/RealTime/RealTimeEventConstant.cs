namespace MainMicroService.Constants.RealTime
{
    public class RealTimeEventConstant
    {
        #region Properties

        /// <summary>
        ///     Event which is raised when user is registered.
        /// </summary>
        public const string UserRegistration = "event-user_register";

        //category group

        public const string AddCategoryGroup = "event-add_category_group";

        public const string EditCategoryGroup = "event-edit_category_group";

        // category

        public const string AddCategory = "event-add_category";

        public const string EditCategory = "event-edit_category";

        public const string EditUserStatus = "event-edit_user_status";

        // topic

        public const string DeleteTopic = "event-delete_topic";

        #endregion
    }
}