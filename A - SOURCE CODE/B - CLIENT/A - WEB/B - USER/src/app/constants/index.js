module.exports = function (ngModule) {
    /*
    * Constants declaration.
    * */
    require('./app-settings.constant')(ngModule);
    require('./url-states.constant')(ngModule);
    require('./api-urls.constant')(ngModule);
    require('./post-status.constant')(ngModule);
    require('./comment-status.constant')(ngModule);
    require('./o-auth.constant')(ngModule);
    require('./post-type.constant')(ngModule);
    require('./user-role.constant')(ngModule);
    require('./task-status.constant')(ngModule);
    require('./task-result.constant')(ngModule);
    require('./notification-status.constant')(ngModule);
    require('../constants/real-time-channel.constant')(ngModule);
    require('../constants/real-time-event.constant')(ngModule);
    require('../constants/pusher-setting.constant')(ngModule);
};