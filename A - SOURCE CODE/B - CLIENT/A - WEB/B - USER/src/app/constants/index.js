module.exports = function (ngModule) {
    /*
    * Constants declaration.
    * */
    require('./app-settings.constant')(ngModule);
    require('./url-states.constant')(ngModule);
    require('./api-urls.constant')(ngModule);
    require('./post-status.constant')(ngModule);
    require('./o-auth.constant')(ngModule);
};