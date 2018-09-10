module.exports = (ngModule) => {
    // Import filter.
    const {toUserStatus} = require('./to-user-status-title.filter');
    ngModule.filter('toUserStatus', toUserStatus);

    const {toUserRoleTitle} = require('./to-user-role-title.filter');
    ngModule.filter('toUserRoleTitle', toUserRoleTitle);

    const {toItemStatusTitle} = require('./to-item-status-title.filter');
    ngModule.filter('toItemStatusTitle', toItemStatusTitle);

    const {toTopicTypeTitle} = require('./to-topic-type-title.filter');
    ngModule.filter('toTopicTypeTitle', toTopicTypeTitle);
};