module.exports = (ngModule) => {
    // Import filter.
    const {toUserStatus} = require('./to-user-status-title.filter');
    ngModule.filter('toUserStatus', toUserStatus);

    const {toUserRoleTitle} = require('./to-user-role-title.filter');
    ngModule.filter('toUserRoleTitle', toUserRoleTitle);
};