module.exports = function (ngModule) {
    require('./authentication.service')(ngModule);
    require('./ui.service')(ngModule);
    require('./user.service')(ngModule);
    require('./post.service')(ngModule);
    require('./post-categorization.service')(ngModule);
    require('./post-notification.service')(ngModule);
    require('./comment.service')(ngModule);
    require('./category.service')(ngModule);
    require('./common.service')(ngModule);
    require('./follow-post.service')(ngModule);
    require('./follow-category.service')(ngModule);
    require('./follow-post.service')(ngModule);
    require('./oauth.service')(ngModule);
    require('./comment-report.service')(ngModule);
    require('./post-report.service')(ngModule);
};