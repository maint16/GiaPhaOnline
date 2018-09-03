module.exports = (ngModule) => {
    /*
    * Constants declaration.
    * */
    require('./app-settings.constant')(ngModule);
    require('./api-urls.constant')(ngModule);
    require('./post-status.constant')(ngModule);
    require('./comment-status.constant')(ngModule);
    require('./o-auth.constant')(ngModule);
    require('./post-type.constant')(ngModule);
    require('./user-role.constant')(ngModule);
    require('./user-status.constant')(ngModule);
    require('./task-status.constant')(ngModule);
    require('./task-result.constant')(ngModule);
    require('./notification-status.constant')(ngModule);
    require('./item-status.constant')(ngModule);
    require('./notification/notification-category.constant')(ngModule);
    require('./notification/notification-action.constant')(ngModule);
};