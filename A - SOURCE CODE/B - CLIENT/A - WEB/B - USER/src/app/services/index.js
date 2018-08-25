module.exports = (ngModule) => {
    require('./authentication.service')(ngModule);
    require('./ui.service')(ngModule);
    require('./user.service')(ngModule);
    require('./post.service')(ngModule);
    require('./post-notification.service')(ngModule);
    require('./category.service')(ngModule);
    require('./common.service')(ngModule);
    require('./follow-post.service')(ngModule);
    require('./follow-category.service')(ngModule);
    require('./oauth.service')(ngModule);
    require('./comment-report.service')(ngModule);
    require('./post-report.service')(ngModule);
    require('./push-notification.service')(ngModule);


    // Mocking api.
    require('./mock/category-group.service')(ngModule);
    require('./mock/category.service')(ngModule);
    require('./mock/topic.service')(ngModule);
};